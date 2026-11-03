#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Project.Battle;
using Project.Data;
using Project.Gacha;
using UnityEditor;
using UnityEngine;
namespace Project.EditorTools
{
    public static class GroundBuffMechanicsAudit
    {
        const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
        [MenuItem("SpinSquad/Diagnostics/Audit Ground Buff Mechanics (temporary units)")]
        static void Audit()
        {
            var system=UnityEngine.Object.FindFirstObjectByType<EffectBuffSystem>();
            var flow=UnityEngine.Object.FindFirstObjectByType<BattleFlow>();
            var spawner=UnityEngine.Object.FindFirstObjectByType<UnitSpawner>();
            var pool=UnityEngine.Object.FindFirstObjectByType<UnitPool>();
            var roll=UnityEngine.Object.FindFirstObjectByType<RollService>();
            if(!EditorApplication.isPlaying||!system||!flow||flow.Current!=BattleFlow.Phase.Prep||!spawner||!pool||!roll)return;
            system.PreviewGroundTiles();
            var type=typeof(EffectBuffSystem);var boardField=type.GetField("board",Private);var rosterField=type.GetField("waveOccupants",Private);
            var timerField=type.GetField("healTimer",Private);var phaseProperty=typeof(BattleFlow).GetProperty("Current");
            var oldBoard=boardField.GetValue(system);var roster=(IDictionary)rosterField.GetValue(system);var previousRoster=new List<DictionaryEntry>();
            foreach(DictionaryEntry pair in roster)previousRoster.Add(pair);
            var ledgerField=typeof(BattleContribution).GetField("rows",BindingFlags.NonPublic|BindingFlags.Static);
            var ledger=(IDictionary)ledgerField.GetValue(null);var previousLedger=new List<DictionaryEntry>();foreach(DictionaryEntry pair in ledger)previousLedger.Add(pair);
            int coinsBefore=roll.Run.Coins;float timer=(float)timerField.GetValue(system);var phase=flow.Current;
            var units=new List<Unit>();var temporary=new GameObject("Temporary Ground Buff Audit Board");var board=temporary.AddComponent<BattleBoard>();
            void Check(bool ok,string message){if(!ok)throw new Exception(message);}
            Unit Spawn(UnitTeam team)
            {
                var definition=pool.Random(Rank.Common,new System.Random(5));var unit=spawner.Spawn(definition,team,new Vector3(100,100),temporary.transform);
                Check(unit,"audit spawn");units.Add(unit);var brain=unit.GetComponent<UnitBrain>();if(brain)brain.enabled=false;unit.Mythic.Deployed=true;return unit;
            }
            try
            {
                boardField.SetValue(system,board);
                int bloodCell=-1,goldCell=-1;Vector3 bloodPosition=Vector3.zero,goldPosition=Vector3.zero;
                foreach(var tile in (IEnumerable)type.GetField("tiles",Private).GetValue(system))
                {
                    var t=tile.GetType();var kind=(EffectBuffKind)t.GetField("Kind").GetValue(tile);int cell=(int)t.GetField("Cell").GetValue(tile);
                    if(kind==EffectBuffKind.BloodTile){bloodCell=cell;bloodPosition=(Vector3)t.GetField("Position").GetValue(tile);}
                    if(kind==EffectBuffKind.GoldTile){goldCell=cell;goldPosition=(Vector3)t.GetField("Position").GetValue(tile);}
                }
                Check(bloodCell>=0&&goldCell>=0&&bloodCell!=goldCell,"distinct actual tile cells");
                var blood=Spawn(UnitTeam.Ally);var gold=Spawn(UnitTeam.Ally);
                Check(board.Place(blood,bloodCell/board.Grid.Cols,bloodCell%board.Grid.Cols),"place blood ally");
                Check(board.Place(gold,goldCell/board.Grid.Cols,goldCell%board.Grid.Cols),"place gold ally");
                blood.TakeDamage(blood.MaxHp*.1f,null,false);float prepHp=blood.CurrentHp;
                timerField.SetValue(system,1f);type.GetMethod("TickTiles",Private).Invoke(system,null);
                Check(blood.CurrentHp==prepHp,"no ground healing during Prep");
                type.GetMethod("TilePhase",Private).Invoke(system,new object[]{BattleFlow.Phase.Marching});
                phaseProperty.SetValue(flow,BattleFlow.Phase.Fighting);
                blood.transform.position=goldPosition;gold.transform.position=bloodPosition;
                Check(Mathf.Abs(system.TileLifesteal(blood)-EffectBuffMath.Lifesteal(system.Level(EffectBuffKind.BloodTile)))<.0001f,"blood assignment survives movement");
                Check(system.TileLifesteal(gold)==0,"gold ally never inherits blood effect from movement");
                blood.TakeDamage(blood.MaxHp*.4f,null,false);float hpBefore=blood.CurrentHp;
                timerField.SetValue(system,1f);type.GetMethod("TickTiles",Private).Invoke(system,null);
                float expected=blood.MaxHp*EffectBuffMath.HealFraction(system.Level(EffectBuffKind.BloodTile));
                Check(Mathf.Abs(blood.CurrentHp-hpBefore-expected)<.01f,"real 1% max HP tick");
                blood.Heal(blood.MaxHp);float max=blood.MaxHp,healing=blood.Healing;
                timerField.SetValue(system,1f);type.GetMethod("TickTiles",Private).Invoke(system,null);
                Check(blood.CurrentHp==max&&blood.MaxHp==max&&blood.Healing==healing,"full HP feedback never increases HP/max HP/ledger");
                var victim=Spawn(UnitTeam.Enemy);victim.TakeDamage(victim.MaxHp*100,gold,false);
                Check(!victim.IsAlive&&roll.Run.Coins==coinsBefore+EffectBuffMath.Coins(system.Level(EffectBuffKind.GoldTile)),"gold ally actual kill grants exact coins");
                int afterKill=roll.Run.Coins;victim.TakeDamage(victim.MaxHp*100,gold,false);
                Check(roll.Run.Coins==afterKill,"dead victim cannot pay twice");
                var other=Spawn(UnitTeam.Enemy);other.TakeDamage(other.MaxHp*100,blood,false);
                Check(!other.IsAlive&&roll.Run.Coins==afterKill,"blood ally kill never grants gold");
                Debug.Log("[GroundBuffAudit] PASS: distinct cells; prep assignment survives movement; no cross-effect; actual max-HP healing; full HP unchanged; actual gold kill +coins exactly once; non-gold kill +0. Temporary units and wallet restored.");
            }
            catch(Exception e){Debug.LogError("[GroundBuffAudit] FAIL: "+e);}
            finally
            {
                phaseProperty.SetValue(flow,phase);boardField.SetValue(system,oldBoard);timerField.SetValue(system,timer);
                roster.Clear();foreach(var pair in previousRoster)roster.Add(pair.Key,pair.Value);
                if(roll.Run.Coins>coinsBefore)roll.Run.TrySpend(roll.Run.Coins-coinsBefore);
                foreach(var unit in units)if(unit)UnityEngine.Object.DestroyImmediate(unit.gameObject);
                UnityEngine.Object.DestroyImmediate(temporary);ledger.Clear();foreach(var pair in previousLedger)ledger.Add(pair.Key,pair.Value);
            }
        }
    }
}
#endif
