using System;
using System.Collections.Generic;
using System.Linq;
using Project.Data;
using Project.Meta;
using UnityEngine;
namespace Project.Battle
{
    /// <summary>Temporary stage 1-2 playtest grants. Nothing is written to the meta save.</summary>
    public sealed class BattleItemInventory:MonoBehaviour
    {
        public sealed class Item {public int InstanceId,Type;public Unit EquippedTo;}
        public readonly List<Item> Items=new();public int Revision{get;private set;}
        BattleFlow flow;bool starter;readonly HashSet<int> awarded=new();int nextId;
        void Start(){flow=FindFirstObjectByType<BattleFlow>();if(flow)flow.RunRestarted+=ResetRun;}
        void OnDestroy(){if(flow)flow.RunRestarted-=ResetRun;}
        void ResetRun(){starter=false;awarded.Clear();Items.Clear();Revision++;}
        void Update()
        {
            if(GameProgress.CurrentLevel!=2||!flow||flow.Current!=BattleFlow.Phase.Prep)return;
            var setup=FindFirstObjectByType<BattleSetup>();if(setup&&setup.IsChoosingLegend)return;
            if(!starter)starter=GrantStarter();
            if(flow.Wave>=3&&awarded.Add(3))Add(0);
            if(flow.Wave>=6&&awarded.Add(6))Add(1);
        }
        public bool TryGrantBasic(int type)
        {
            if(type<0||type>1||Items.Count(i=>!i.EquippedTo)>=10)return false;
            Add(type);return true;
        }
        void Add(int type){Items.Add(new Item{InstanceId=++nextId,Type=type});Revision++;Debug.Log("[BattleItems] Stage 1-2 received "+BattleItemRecipes.Items[type].Name);}
        bool GrantStarter()
        {
            var board=FindObjectsByType<BattleBoard>(FindObjectsSortMode.None).FirstOrDefault(x=>x.name.ToLowerInvariant().Contains("ally"));
            var pool=FindFirstObjectByType<Project.Gacha.UnitPool>();var spawner=FindFirstObjectByType<UnitSpawner>();if(!board||!pool||!spawner)return false;
            var made=new List<Unit>();
            foreach(var id in MythicFusion.For(MythicCatalog.Find(MythicKind.Guardian)).ingredients)
            {
                var def=new[]{Rank.Common,Rank.Rare,Rank.Epic,Rank.Legendary}.SelectMany(r=>pool.Of(r)).FirstOrDefault(d=>d.UnitId==id);
                if(!def||!board.FindSpotFor(def,out int r,out int c)){Rollback();return false;}
                var unit=spawner.Spawn(def,UnitTeam.Ally,board.Grid.CellToWorld(r,c),board.transform);if(!unit){Rollback();return false;}
                made.Add(unit);var brain=unit.GetComponent<UnitBrain>();if(brain)brain.enabled=false;
                if(!board.Place(unit,r,c)){Rollback();return false;}
            }
            Debug.Log("[BattleItems] Stage 1-2: Crescent Mage + 2 Sickle Militia ready for Dawn Guardian.");return true;
            void Rollback(){foreach(var u in made){board.Remove(u);u.gameObject.SetActive(false);Destroy(u.gameObject);}}
        }
        public bool Equip(int id,Unit target)
        {
            if(!flow||flow.Current!=BattleFlow.Phase.Prep||Project.UI.SharedSettings.IsPaused||!target||!target.IsAlive||target.Team!=UnitTeam.Ally||target.Definition.Rank!=Rank.Mythic)return false;
            var item=Items.FirstOrDefault(i=>i.InstanceId==id);if(item==null||item.EquippedTo)return false;
            item.EquippedTo=target;target.Equipment.Add(item.Type);Revision++;return true;
        }
        public Unit TargetAt(Vector2 screen)
        {
            var camera=Camera.main;if(!camera)return null;var world=camera.ScreenToWorldPoint(screen);world.z=0;
            var board=FindObjectsByType<BattleBoard>(FindObjectsSortMode.None).FirstOrDefault(b=>b.name.ToLowerInvariant().Contains("ally"));if(!board)return null;
            // Accept the visible body as well as its placement cell, including its head.
            foreach(var unit in UnitRegistry.All)
            {if(!unit||unit.Team!=UnitTeam.Ally||!unit.IsAlive||unit.Definition.Rank!=Rank.Mythic)continue;
                var body=unit.GetComponent<Project.Visual.FrameUnitVisual>()?.BodyRenderer;
                if(body){var bounds=body.bounds;bounds.Expand(.12f);if(world.x>=bounds.min.x&&world.x<=bounds.max.x&&world.y>=bounds.min.y&&world.y<=bounds.max.y)return unit;}}
            if(board.Grid.WorldToCell(world,out int r,out int c))return board.UnitsAt(r,c).FirstOrDefault(u=>u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Definition.Rank==Rank.Mythic);
            return null;
        }
        public bool Combine(int source,int target)
        {
            if(source==target||!flow||flow.Current!=BattleFlow.Phase.Prep||Project.UI.SharedSettings.IsPaused)return false;
            var a=Items.FirstOrDefault(i=>i.InstanceId==source);var b=Items.FirstOrDefault(i=>i.InstanceId==target);if(a==null||b==null||a.EquippedTo||b.EquippedTo)return false;
            var recipe=BattleItemRecipes.Recipes.FirstOrDefault(r=>(r[0]==a.Type&&r[1]==b.Type)||(r[1]==a.Type&&r[0]==b.Type));if(recipe==null)return false;
            Items.Remove(a);Items.Remove(b);Add(recipe[2]);return true;
        }
    }
}
