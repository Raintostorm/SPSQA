using System;
using System.Collections.Generic;
using System.Linq;
using Project.Data;
using Project.Gacha;
using UnityEngine;
namespace Project.Battle
{
    public sealed partial class EffectBuffSystem:MonoBehaviour
    {
        public static EffectBuffSystem Current{get;private set;}
        readonly Dictionary<EffectBuffKind,int> active=new();readonly Queue<Rank> choices=new();
        readonly Queue<UnitDefinition> pendingAllies=new();readonly Queue<int> pendingItems=new();
        readonly System.Random random=new();
        RollService roll;BattleFlow flow;BattleBoard board;UnitPool pool;UnitSpawner spawner;BattleItemInventory inventory;float nextDelivery;
        Project.UI.EffectBuffChoiceView view;bool selecting,openingQueued;int openingRemaining,openingTotal;
        public bool HasPendingChoice=>choices.Count>0||selecting;
        public bool CombatActive=>Fighting;
        public bool RescueArmed=>rescueArmed;
        public int RescueWave=>rescueWave;
        public int PendingAllies=>pendingAllies.Count;public int PendingItems=>pendingItems.Count;
        public IEnumerable<EffectBuffKind> Active=>active.Keys;
        public int Level(EffectBuffKind kind)=>active.TryGetValue(kind,out int level)?level:0;
        public bool Has(EffectBuffKind kind)=>active.ContainsKey(kind);
        public void Bind(RollService owner){roll=owner;Current=this;}
        void Start()
        {
            flow=FindFirstObjectByType<BattleFlow>();pool=FindFirstObjectByType<UnitPool>();spawner=FindFirstObjectByType<UnitSpawner>();
            board=FindObjectsByType<BattleBoard>(FindObjectsSortMode.None).FirstOrDefault(b=>b.name.ToLowerInvariant().Contains("ally"));
            if(flow){flow.PhaseChanged+=Phase;flow.RunRestarted+=ResetRun;}
            gameObject.AddComponent<Project.UI.EffectBuffHud>().Bind(this);
        }
        void OnDestroy(){if(flow){flow.PhaseChanged-=Phase;flow.RunRestarted-=ResetRun;}if(view)Destroy(view.gameObject);if(Current==this)Current=null;ClearTiles();}
        public void ResetRun(){harvest.Reset();paidWave=-1;openingQueued=false;openingRemaining=openingTotal=0;active.Clear();choices.Clear();pendingAllies.Clear();pendingItems.Clear();selecting=false;if(view)Destroy(view.gameObject);ClearTiles();rescueWave=-1;rescueArmed=false;foreach(var u in UnitRegistry.All)if(u)u.GetComponent<EffectBuffUnit>()?.ResetRun();}
#if UNITY_EDITOR
        // Explicit diagnostic action: temporary run buffs only, never changes save data.
        public void PreviewGroundTiles()
        {
            if(!flow||flow.Current!=BattleFlow.Phase.Prep)return;
            if(!Has(EffectBuffKind.BloodTile))Apply(EffectBuffKind.BloodTile);
            if(!Has(EffectBuffKind.GoldTile))Apply(EffectBuffKind.GoldTile);
        }
#endif
        public void QueueChoice(Rank rank){choices.Enqueue(rank);}
        bool Allowed(EffectBuffKind kind)
        {
            if(EffectBuffCatalog.Persistent(kind))return true;
            if(kind==EffectBuffKind.GrandAscension)return TryPlanAscension(out _,out _,out _);
            if(kind==EffectBuffKind.Reinforcements)return board&&spawner&&pool&&pool.HasAny(Rank.Legendary);
            if(kind==EffectBuffKind.WeaponSupplies)return FindFirstObjectByType<BattleItemInventory>();
            if(kind==EffectBuffKind.SecondWind)return !rescueArmed;
            return true;
        }
        void Update()
        {
            if(!flow||!roll)return;
            if(flow.Current==BattleFlow.Phase.Prep)
            {
                if(!openingQueued)
                {
                    openingQueued=true;
                    var opening=StageRollPreview.OpeningChoices(Project.Meta.GameProgress.CurrentLevel,flow.Wave);
                    openingRemaining=openingTotal=opening.Length;
                    foreach(var rank in opening)QueueChoice(rank);
                }
                if(Time.unscaledTime>=nextDelivery){nextDelivery=Time.unscaledTime+.15f;DeliverPending();}
                if(!selecting&&choices.Count>0&&!roll.IsReelPresenting&&!Project.UI.SharedSettings.IsPaused)
                {
                    var rank=choices.Dequeue();var options=EffectBuffCatalog.Draw(rank,random,kind=>Allowed(kind)&&(openingRemaining<=0||(EffectBuffCatalog.Persistent(kind)&&!Has(kind))));
                    if(options.Length>0)
                    {
                        bool opening=openingRemaining>0;
                        string progress=opening?"Opening buffs · "+(openingTotal-openingRemaining+1)+" / "+openingTotal:null;
                        selecting=true;view=Project.UI.EffectBuffChoiceView.Show(options,kind=>{Apply(kind);if(opening)openingRemaining--;selecting=false;},progress);
                    }
                    else if(openingRemaining>0)openingRemaining--;
                }
            }
            TickTiles();
        }
        void Apply(EffectBuffKind kind)
        {
            if(EffectBuffCatalog.Persistent(kind)){active[kind]=Level(kind)+1;if(Level(kind)==1&&(kind==EffectBuffKind.BloodTile||kind==EffectBuffKind.GoldTile))CreateTile(kind);return;}
            switch(kind)
            {
                case EffectBuffKind.WarChest:roll.Run.AddCoins(200);break;
                case EffectBuffKind.Reinforcements:for(int i=0;i<3;i++){var def=pool.Random(Rank.Legendary,random);if(def)pendingAllies.Enqueue(def);}break;
                case EffectBuffKind.WeaponSupplies:for(int i=0;i<2;i++)pendingItems.Enqueue(random.Next(2));break;
                case EffectBuffKind.GrandAscension:Ascend();break;
                case EffectBuffKind.SecondWind:rescueArmed=true;rescueWave=flow.Wave;break;
            }
            DeliverPending();
        }
        bool TryPlanAscension(out Unit[] army,out UnitDefinition[] targets,out Vector2Int[] spots)
        {
            army=Array.Empty<Unit>();targets=Array.Empty<UnitDefinition>();spots=Array.Empty<Vector2Int>();
            if(!board||!pool)return false;
            army=UnitRegistry.All.Where(u=>u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Definition.Rank<Rank.Legendary&&u.transform.IsChildOf(board.transform)).ToArray();
            if(army.Length==0)return false;
            targets=army.Select(u=>pool.Find(u.Definition.Rank+1,u.Definition.Race)).ToArray();
            if(targets.Any(d=>!d))return false;
            // Preserve occupied Legendary/Mythic cells, including manually split stacks.
            // A count grouped only by definition underestimates their actual space.
            int rows=board.Grid.Rows,cols=board.Grid.Cols;
            var definitions=new UnitDefinition[rows,cols];var counts=new int[rows,cols];
            for(int r=0;r<rows;r++)for(int c=0;c<cols;c++)
                foreach(var u in board.UnitsAt(r,c))if(u&&!army.Contains(u)){definitions[r,c]=u.Definition;counts[r,c]++;}
            if(!EffectBuffPlacement.TryPlan(definitions,counts,targets,board.Grid.MaxPerCell,out var cells))return false;
            spots=cells.Select(cell=>new Vector2Int(cell/cols,cell%cols)).ToArray();return true;
        }
        void Ascend()
        {
            if(!TryPlanAscension(out var army,out var targets,out var spots))return;
            // Plan every destination before touching identities or board occupancy.
            foreach(var u in army)board.Remove(u);
            for(int i=0;i<army.Length;i++)army[i].Promote(targets[i]);
            for(int i=0;i<army.Length;i++)board.Place(army[i],spots[i].x,spots[i].y);
        }
        void DeliverPending()
        {
            if(!board||!spawner||!flow||flow.Current!=BattleFlow.Phase.Prep)return;
            int remaining=pendingAllies.Count;
            while(remaining-->0&&pendingAllies.Count>0)
            {
                var def=pendingAllies.Peek();if(!board.FindSpotFor(def,out int r,out int c)){pendingAllies.Dequeue();pendingAllies.Enqueue(def);continue;}
                var u=spawner.Spawn(def,UnitTeam.Ally,board.Grid.CellToWorld(r,c),board.transform);if(!u)break;
                if(!board.Place(u,r,c)){Destroy(u.gameObject);break;}
                var brain=u.GetComponent<UnitBrain>();if(brain)brain.enabled=false;u.Mythic.Deployed=false;pendingAllies.Dequeue();
                Project.Visual.MergeReveal.Play(u);
            }
            if(pendingItems.Count==0)return;
            if(!inventory)inventory=FindFirstObjectByType<BattleItemInventory>();
            while(inventory&&pendingItems.Count>0&&inventory.TryGrantBasic(pendingItems.Peek()))pendingItems.Dequeue();
        }
        void Phase(BattleFlow.Phase phase)
        {
            if(phase==BattleFlow.Phase.Marching&&rescueArmed&&rescueWave!=flow.Wave)rescueArmed=false;
            if(phase==BattleFlow.Phase.Marching&&Has(EffectBuffKind.Bodyguard))foreach(var u in UnitRegistry.All)if(u&&u.IsAlive&&u.Team==UnitTeam.Ally)u.Effects?.WaveShield(flow.Wave);
            if(phase==BattleFlow.Phase.Finished&&flow.Winner==UnitTeam.Ally&&paidWave!=flow.Wave)
            {paidWave=flow.Wave;int coins=EffectBuffMath.WaveCoins(Level(EffectBuffKind.WaveDividend),flow.Wave);if(coins>0){roll.Run.AddCoins(coins);FloatingCoin(Vector3.zero,coins);}}
            if(phase==BattleFlow.Phase.Prep)
            {
                bool growing=harvest.PendingPercent>0;harvest.NextWave();
                if(growing)foreach(var u in UnitRegistry.All)if(u&&u.Team==UnitTeam.Ally)Project.Visual.BuffProcVisual.Play(u,EffectBuffKind.SoulHarvest);
            }
            if(phase==BattleFlow.Phase.Marching&&Has(EffectBuffKind.MythicMight))foreach(var u in UnitRegistry.All)if(u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Definition.Rank==Rank.Mythic)Project.Visual.BuffProcVisual.Play(u,EffectBuffKind.MythicMight);
            TilePhase(phase);
        }
        readonly HarvestGrowth harvest=new();int paidWave=-1;
        public float HpMultiplier(Unit unit)=>unit&&unit.Team==UnitTeam.Ally?harvest.Multiplier:1;
        public float AttackMultiplier(Unit unit)=>HpMultiplier(unit)*(unit&&unit.Team==UnitTeam.Ally&&unit.Definition.Rank==Rank.Mythic?1+.25f*Level(EffectBuffKind.MythicMight):1);
        public float ManaMultiplier(Unit unit)=>unit&&unit.Team==UnitTeam.Ally?1+.2f*Level(EffectBuffKind.ManaFlow):1;
        public string Describe(EffectBuffKind kind)=>EffectBuffScaling.Describe(kind,Level(kind))+(kind==EffectBuffKind.SoulHarvest?$"\nActive: +{harvest.ActivePercent:0}% HP / ATK · Next wave: +{harvest.PendingPercent:0}%":"");
        public void EnemyDefeated(){if(Fighting)harvest.Kill(Level(EffectBuffKind.SoulHarvest));}
        int rescueWave=-1;bool rescueArmed;
        public bool TryRescue()
        {
            if(!flow||!EffectBuffMath.RescueEligible(rescueArmed,rescueWave,flow.Wave,UnitRegistry.CountAlive(UnitTeam.Ally),UnitRegistry.CountAlive(UnitTeam.Enemy)))return false;
            var fallen=UnitRegistry.All.Where(u=>u&&!u.IsAlive&&u.Team==UnitTeam.Ally&&u.Mythic.Deployed).ToArray();
            if(fallen.Length==0)return false;
            rescueArmed=false;
            foreach(var u in fallen){u.ReviveInBattle(1);Project.Visual.SkillEffects.Ring(u.transform.position,.45f,Color.green,.8f);}
            foreach(var u in UnitRegistry.All)if(u&&u.IsAlive&&u.Team==UnitTeam.Ally)u.Heal(u.MaxHp);
            return true;
        }
        public void StrikeVisual(Unit source,Unit target){Project.Visual.SkillEffects.Beam(source.HitPoint,target.HitPoint,new Color(.6f,.9f,1),.15f);}
        // Ground-zone implementation is kept in a separate partial file.
    }
}
