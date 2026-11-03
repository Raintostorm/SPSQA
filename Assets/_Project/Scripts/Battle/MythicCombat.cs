using System.Collections;
using System.Collections.Generic;
using Project.Data;
using Project.Visual;
using UnityEngine;

namespace Project.Battle
{
    [RequireComponent(typeof(Unit))]
    public sealed class MythicCombat:MonoBehaviour
    {
        Unit unit;BattleFlow flow;FrameUnitVisual visual;float mana,aura,until,buffUntil,shieldUntil,shield,stopUntil;
        MythicEffect auraArt;bool casting,dashing;bool revived;
        public MythicKind Kind=>unit&&unit.Definition?unit.Definition.Mythic:MythicKind.None;
        public bool Active=>Kind!=MythicKind.None;
        public float Mana=>mana;
        public bool Casting=>casting||dashing;
        public bool Stopped=>Time.time<stopUntil;
        public bool AuraActive=>aura>0;
        public float AttackMultiplier=>Time.time<buffUntil?1.2f:1;
        public float SpeedMultiplier=>AttackMultiplier;
        public float Shield=>Time.time<shieldUntil?shield:0;
        public bool Charged=>Kind==MythicKind.Bear && Time.time<until;
        public bool RevivalUsed=>revived;
        public bool Converted {get;private set;}
        public bool Deployed {get;set;}=true;
        bool InWave=>!flow||flow.Current==BattleFlow.Phase.Marching||flow.Current==BattleFlow.Phase.Fighting;
        void Awake(){unit=GetComponent<Unit>();visual=GetComponent<FrameUnitVisual>();}
        void Start(){flow=FindFirstObjectByType<BattleFlow>();if(flow){flow.PhaseChanged+=Phase;flow.RunRestarted+=ResetRun;}RefreshStartingMana();}
        void OnDestroy(){if(flow){flow.PhaseChanged-=Phase;flow.RunRestarted-=ResetRun;}}
        void ResetRun(){revived=false;}
        public void RefreshStartingMana()
        {if(unit&&Active&&Kind!=MythicKind.Undead&&(!flow||flow.Current==BattleFlow.Phase.Prep))mana=Mathf.Clamp(unit.Equipment.StartingMana*unit.Equipment.ManaGainMultiplier*(EffectBuffSystem.Current?EffectBuffSystem.Current.ManaMultiplier(unit):1),0,100);}
        void Phase(BattleFlow.Phase phase)
        {
            if(phase==BattleFlow.Phase.Fighting||phase==BattleFlow.Phase.Marching)return;
            mana=aura=until=buffUntil=shield=shieldUntil=stopUntil=0;casting=dashing=false;StopAllCoroutines();if(auraArt)Destroy(auraArt.gameObject);if(phase==BattleFlow.Phase.Prep)RefreshStartingMana();
        }
        void Update()
        {
            if(!Active)return;
            if(visual)visual.EmpoweredAttack=Charged;
            if(!unit.IsAlive||!InWave){if(!unit.IsAlive){aura=0;mana=0;casting=false;}return;}
            if(aura>0)
            {
                float dt=Mathf.Min(Time.deltaTime,mana/15f);mana=Mathf.Max(0,mana-15*dt);aura+=dt;
                while(aura>=1f){aura-=1f;float healed=0;foreach(var enemy in Snapshot())if(Enemy(enemy)&&Distance(enemy)<=1.2f){float before=enemy.DamageTaken;enemy.TakeDamage(unit.AttackPower*.2f,unit,false);healed+=Mathf.Max(0,enemy.DamageTaken-before);}
                    unit.Heal(healed*.5f);if(healed>0)MythicEffect.Play("heal",unit,unit.HitPoint,1,.5f);}
                // A separate positive marker keeps the accumulator active between ticks.
                if(mana<=0){aura=0;if(auraArt)Destroy(auraArt.gameObject);}else if(aura<=0)aura=.00001f;
            }
        }
        static Unit[] Snapshot(){var list=new List<Unit>(UnitRegistry.All);return list.ToArray();}
        float Distance(Unit other)=>Vector2.Distance(unit.transform.position,other.transform.position);
        bool Enemy(Unit u)=>u&&u.IsAlive&&u.Team!=unit.Team&&u.Mythic.Deployed;
        public void GainMana(float amount){if(amount>0&&Active&&Kind!=MythicKind.Undead&&!AuraActive&&unit.IsAlive&&InWave){float before=mana;mana=Mathf.Min(100,mana+amount*unit.Equipment.ManaGainMultiplier*(EffectBuffSystem.Current?EffectBuffSystem.Current.ManaMultiplier(unit):1));if(mana>before&&unit.Team==UnitTeam.Ally)Project.Visual.BuffProcVisual.Play(unit,EffectBuffKind.ManaFlow);}}
        public void ReceiveBasicHit()=>GainMana(4);
        public void BasicHit()=>GainMana(20);
        public float Absorb(float damage){float absorbed=Time.time<shieldUntil?Mathf.Min(shield,damage):0;shield-=absorbed;return damage-absorbed;}
        public void StopFor(float seconds){stopUntil=Mathf.Max(stopUntil,Time.time+seconds);}
        public bool TryCast(Unit target)
        {
            if(!Active||Kind==MythicKind.Undead||AuraActive||mana<100||casting||!InWave||!Enemy(target))return false;
            if(!unit.Definition.IsRanged && Distance(target)>.6f)return false;
            casting=true;unit.Visual?.SetMoving(false);var centre=target.transform.position;
            if(visual)visual.PlaySkill(()=>Resolve(target,centre),()=>casting=false);
            else{Resolve(target,centre);casting=false;}
            return true;
        }
        void Resolve(Unit target,Vector3 centre)
        {
            if(!unit.IsAlive||!InWave)return;mana=0;if(unit.Team==UnitTeam.Ally)BattleContribution.SkillCasts++;
            switch(Kind)
            {
                case MythicKind.Guardian:
                    mana=100;aura=.00001f;auraArt=MythicEffect.Play("aura",unit,unit.transform.position,2.4f,100/15f,true,true);break;
                case MythicKind.Elf:
                    foreach(var ally in Snapshot())if(ally&&ally.IsAlive&&ally.Team==unit.Team){ally.Mythic.buffUntil=Time.time+8;MythicEffect.Play("buff",ally,ally.transform.position,Mathf.Max(.5f,Mathf.Abs(ally.transform.lossyScale.x)*1.2f),8f,true,true);}break;
                case MythicKind.Bear:until=Time.time+6;MythicEffect.Play("buff",unit,unit.transform.position,1.3f,.8f);break;
                case MythicKind.Robot:
                    foreach(var ally in Snapshot())if(ally&&ally.IsAlive&&ally.Team==unit.Team&&Distance(ally)<=2){ally.Mythic.shield=ally.MaxHp*.25f;ally.Mythic.shieldUntil=Time.time+6;MythicEffect.Play("shield",ally,ally.transform.position,Mathf.Max(.65f,Mathf.Abs(ally.transform.lossyScale.x)*1.6f),6,true,true);}break;
                case MythicKind.Clock:StartCoroutine(TimeField(centre));break;
                case MythicKind.Mimic:
                    if(!Enemy(target))break;
                    if(target.Definition.EnemyTier!=UnitDefinition.EnemyClass.Boss)target.transform.position=Vector3.MoveTowards(target.transform.position,unit.transform.position,.4f);
                    float before=target.DamageTaken;target.TakeDamage(unit.AttackPower*3,unit,false);unit.Heal(Mathf.Max(0,target.DamageTaken-before)*.5f);
                    MythicEffect.Play("devour",unit,centre,1.6f,.65f);MythicEffect.Play("heal",unit,unit.HitPoint,1,.6f);
                    if(unit.Team==UnitTeam.Ally)MythicBattleRewards.Add(flow,5);break;
                case MythicKind.Swordsman:StartCoroutine(Dash(target,centre));break;
            }
        }
        IEnumerator TimeField(Vector3 centre)
        {
            float end=Time.time+5;MythicEffect.Play("time",unit,centre,3f,5,true);
            while(Time.time<end&&InWave){var units=UnitRegistry.All;for(int i=0;i<units.Count;i++){var enemy=units[i];if(Enemy(enemy)&&(enemy.transform.position-centre).sqrMagnitude<=2.25f)enemy.Mythic.StopFor(Mathf.Min(.08f,end-Time.time));}yield return null;}
        }
        IEnumerator Dash(Unit target,Vector3 centre)
        {
            dashing=true;Vector3 start=unit.transform.position;float sign=centre.x>=start.x?1:-1;Vector3 end=new Vector3(centre.x+sign*.6f,start.y,start.z);
            var hit=new HashSet<Unit>();float elapsed=0;
            MythicEffect.Play("slash",unit,(start+end)*.5f,Mathf.Max(1,(end-start).magnitude),.45f);
            while(elapsed<.25f&&unit.IsAlive&&InWave)
            {
                var previous=unit.transform.position;elapsed+=Time.deltaTime;unit.transform.position=Vector3.Lerp(start,end,elapsed/.25f);
                foreach(var enemy in Snapshot())if(Enemy(enemy)&&!hit.Contains(enemy)&&Mathf.Abs(enemy.transform.position.y-start.y)<=.5f&&enemy.transform.position.x>=Mathf.Min(previous.x,unit.transform.position.x)-.15f&&enemy.transform.position.x<=Mathf.Max(previous.x,unit.transform.position.x)+.15f){hit.Add(enemy);enemy.TakeDamage(unit.AttackPower*2,unit,false);}
                yield return null;
            }
            dashing=false;
        }
        public void AfterBasicAttack(Unit primary,bool empoweredAttack)
        {
            if(!empoweredAttack)return;if(primary)MythicEffect.Play("electric",unit,primary.transform.position,.8f,.3f);StartCoroutine(Chain(primary));
        }
        IEnumerator Chain(Unit primary)
        {
            Vector3 point=primary?primary.HitPoint:unit.HitPoint;var hit=new HashSet<Unit>{primary};
            for(int i=0;i<2;i++)
            {
                yield return new WaitForSeconds(.075f);if(!unit.IsAlive||!InWave)yield break;
                Unit next=null;float nearest=2.25f;
                foreach(var enemy in Snapshot())if(Enemy(enemy)&&!hit.Contains(enemy)){float d=(enemy.HitPoint-point).sqrMagnitude;if(d<nearest){nearest=d;next=enemy;}}
                                if(!next)yield break;hit.Add(next);MythicEffect.Lightning(point,next.HitPoint);MythicEffect.Play("electric",unit,next.transform.position,.8f,.3f);next.TakeDamage(unit.AttackPower*.5f,unit,false);point=next.HitPoint;
            }
        }
        // Synchronous revival prevents the wave-end check from racing a delayed resurrection.
        public bool TryRevive()
        {
            if(!InWave||!Deployed)return false;Unit keeper=null;
            foreach(var u in UnitRegistry.All)if(u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Mythic.Kind==MythicKind.Undead&&u.Mythic.Deployed){keeper=u;break;}
            if(!keeper)return false;
            if(unit.Team==UnitTeam.Enemy)
            {
                if(unit.Definition.EnemyTier==UnitDefinition.EnemyClass.Boss)return false;
                Converted=true;unit.ChangeTeam(UnitTeam.Ally);unit.Combat.MarkTemporary();
            }
            else {if(revived)return false;revived=true;if(!GetComponent<RevivalSeal>())gameObject.AddComponent<RevivalSeal>();}
            unit.ReviveInBattle(.5f);MythicEffect.Play("revive",unit,unit.transform.position,1.3f,.9f);return true;
        }
    }
}
