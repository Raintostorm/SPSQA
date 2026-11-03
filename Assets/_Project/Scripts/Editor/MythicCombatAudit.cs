#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Project.Battle;
using Project.Data;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;
namespace Project.EditorTools
{
    public static class MythicCombatAudit
    {
        [MenuItem("SpinSquad/Diagnostics/Audit Mythic Integration %&y")]
        static void Run()
        {
            if(!Application.isPlaying||Object.FindFirstObjectByType<BattleFlow>()){Debug.LogWarning("[MythicAudit] Run outside battle in Play mode.");return;}
            var root=new GameObject("Mythic audit fixture");var spawner=root.AddComponent<UnitSpawner>();
            typeof(UnitSpawner).GetField("unitPrefab",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(spawner,AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Units/Unit.prefab"));
            spawner.StartCoroutine(Check(spawner,root));
        }
        static IEnumerator Check(UnitSpawner spawner,GameObject root)
        {
            float speed=Time.timeScale;Time.timeScale=1;var log=new List<string>();
            void Test(bool value,string label){log.Add((value?"PASS ":"FAIL ")+label);}
            Unit Spawn(MythicKind kind,UnitTeam team,Vector3 pos){var u=spawner.Spawn(MythicCatalog.Find(kind),team,pos,root.transform);u.GetComponent<UnitBrain>().enabled=false;u.SetSummonedStats(10000,100);return u;}
            Test(MythicCatalog.All.Count==8,"8 approved identities");
            foreach(var d in MythicCatalog.All)Test(d.Animations.Idle.Count==4&&d.Animations.Walk.Count==4&&d.Animations.Attack.Count==4&&d.Animations.Dead.Count==4&&(d.Mythic==MythicKind.Undead||d.Animations.Skill.Count==4),d.UnitId+" complete isolated clips");
            var g=Spawn(MythicKind.Guardian,UnitTeam.Ally,new Vector3(100,0));var enemy=Spawn(MythicKind.Swordsman,UnitTeam.Enemy,new Vector3(100.4f,0));
            yield return null;
            for(int i=0;i<5;i++)enemy.TakeDamage(1,g);
            Test(g.Combat.Mana==100,"5 landed basic attacks charge Mythic mana");Test(g.Combat.TryCast(enemy),"Guardian begins cast");
            yield return new WaitForSeconds(.85f);float mana=g.Combat.Mana;enemy.TakeDamage(1,g);g.TakeDamage(1,enemy);
            Test(g.Combat.Mana<=mana&&g.Mythic.AuraActive,"Aura rejects both mana gain sources");
            float hp=enemy.CurrentHp;yield return new WaitForSeconds(1.1f);Test(enemy.CurrentHp<hp,"Aura ticks damage while free to act");
            Object.Destroy(g.gameObject);yield return null;
            var clock=Spawn(MythicKind.Clock,UnitTeam.Ally,new Vector3(100,0));for(int i=0;i<5;i++)enemy.TakeDamage(1,clock);clock.Combat.TryCast(enemy);
            yield return new WaitForSeconds(.85f);Test(enemy.Combat.Stunned,"Time field stops enemy");yield return new WaitForSeconds(5.1f);Test(!enemy.Combat.Stunned,"Time stop expires after 5 seconds");Object.Destroy(clock.gameObject);yield return null;
            var undead=Spawn(MythicKind.Undead,UnitTeam.Ally,new Vector3(100,0));
            var commonDef=Object.Instantiate(AssetDatabase.LoadAssetAtPath<UnitDefinition>("Assets/_Project/Data/Units/Unit_human_common.asset"));commonDef.EnemyTier=UnitDefinition.EnemyClass.Elite;
            var victim=spawner.Spawn(commonDef,UnitTeam.Enemy,new Vector3(101,0),root.transform);victim.GetComponent<UnitBrain>().enabled=false;
            victim.TakeDamage(100000,undead,false);Test(victim.IsAlive&&victim.Team==UnitTeam.Ally,"Elite converts to allied temporary unit");victim.GetComponent<UnitBrain>().enabled=false;
            victim.TakeDamage(100000,null,false);Test(victim.IsAlive&&victim.Mythic.RevivalUsed,"Ally revives once with seal");victim.GetComponent<UnitBrain>().enabled=false;
            victim.TakeDamage(100000,null,false);Test(!victim.IsAlive,"Second ally death is final");
            commonDef.EnemyTier=UnitDefinition.EnemyClass.Boss;var boss=spawner.Spawn(commonDef,UnitTeam.Enemy,new Vector3(102,0),root.transform);boss.GetComponent<UnitBrain>().enabled=false;boss.TakeDamage(100000,undead,false);Test(!boss.IsAlive&&boss.Team==UnitTeam.Enemy,"Boss never converts");
            foreach(var key in new[]{"time","devour","heal","slash","aura","revive","buff","shield","seal","relic","electric","elf-bolt","clock-bolt"})Test(MythicCatalog.Effect(key).Length>0,"VFX loaded "+key);
            var bear=Spawn(MythicKind.Bear,UnitTeam.Ally,new Vector3(100,0));
            typeof(MythicCombat).GetField("until",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(bear.Mythic,Time.time+6);
            Unit Extra(float x){var u=spawner.Spawn(commonDef,UnitTeam.Enemy,new Vector3(x,0),root.transform);u.SetSummonedStats(10000,1);u.GetComponent<UnitBrain>().enabled=false;return u;}
            var chainA=Extra(100.8f);var chainB=Extra(101.1f);bear.Mythic.AfterBasicAttack(enemy,true);float chainDeadline=Time.time+2;while((chainA.DamageTaken<=0||chainB.DamageTaken<=0)&&Time.time<chainDeadline)yield return null;
            Test(Mathf.Approximately(chainA.DamageTaken,50)&&Mathf.Approximately(chainB.DamageTaken,50),"Bear chains once to two different targets at 50% ATK ("+chainA.DamageTaken+", "+chainB.DamageTaken+")");Object.Destroy(bear.gameObject);yield return null;
            var robot=Spawn(MythicKind.Robot,UnitTeam.Ally,new Vector3(100,0));var resolve=typeof(MythicCombat).GetMethod("Resolve",BindingFlags.Instance|BindingFlags.NonPublic);
            resolve.Invoke(robot.Mythic,new object[]{enemy,enemy.transform.position});resolve.Invoke(robot.Mythic,new object[]{enemy,enemy.transform.position});
            Test(Mathf.Approximately(robot.Mythic.Shield,2500),"Robot shield refreshes instead of stacking");robot.TakeDamage(100,null,false);Test(Mathf.Approximately(robot.CurrentHp,10000)&&Mathf.Approximately(robot.Mythic.Shield,2400),"Mythic shield absorbs actual damage");Object.Destroy(robot.gameObject);yield return null;
            var oracle=Spawn(MythicKind.Elf,UnitTeam.Ally,new Vector3(100,0));resolve.Invoke(oracle.Mythic,new object[]{enemy,enemy.transform.position});resolve.Invoke(oracle.Mythic,new object[]{enemy,enemy.transform.position});
            Test(Mathf.Approximately(undead.AttackPower,120)&&Mathf.Approximately(undead.Mythic.SpeedMultiplier,1.2f),"Team buff uses derived multipliers, never compounds");Object.Destroy(oracle.gameObject);yield return null;
            var mimic=Spawn(MythicKind.Mimic,UnitTeam.Ally,new Vector3(100,0));var bossTarget=Extra(100.4f);float bossX=bossTarget.transform.position.x;float beforeHp=bossTarget.CurrentHp;
            mimic.TakeDamage(500,null,false);resolve.Invoke(mimic.Mythic,new object[]{bossTarget,bossTarget.transform.position});
            Test(Mathf.Approximately(bossTarget.transform.position.x,bossX)&&Mathf.Approximately(beforeHp-bossTarget.CurrentHp,300),"Mimic damages Boss without pulling it");Test(Mathf.Approximately(mimic.CurrentHp,9650),"Mimic heals 50% actual damage");Object.Destroy(mimic.gameObject);yield return null;
            var ronin=Spawn(MythicKind.Swordsman,UnitTeam.Ally,new Vector3(100,0));float damageBefore=chainA.DamageTaken;resolve.Invoke(ronin.Mythic,new object[]{chainB,chainB.transform.position});yield return new WaitForSeconds(.4f);
            Test(Mathf.Approximately(chainA.DamageTaken-damageBefore,200),"Dash sweeps corridor and hits each target once");
            foreach(var u in root.GetComponentsInChildren<Unit>())Object.Destroy(u.gameObject);Object.Destroy(commonDef);yield return null;
            var flow=root.AddComponent<BattleFlow>();flow.enabled=false;
            var boardObject=new GameObject("Ally test board");boardObject.transform.SetParent(root.transform);var board=boardObject.AddComponent<BattleBoard>();
            void Ingredients(MythicKind kind)
            {
                foreach(var id in MythicFusion.For(MythicCatalog.Find(kind)).ingredients)
                {
                    var def=AssetDatabase.LoadAssetAtPath<UnitDefinition>("Assets/_Project/Data/Units/Unit_"+id+".asset");
                    var u=spawner.Spawn(def,UnitTeam.Ally,new Vector3(100,0),root.transform);u.GetComponent<UnitBrain>().enabled=false;
                    if(board.FindSpotFor(def,out int r,out int c))board.Place(u,r,c);
                }
            }
            foreach(var def in MythicCatalog.All)Test(MythicFusion.For(def)?.ingredients.Length==3,"3 exact ingredients "+def.UnitId);
            var guardian=MythicCatalog.Find(MythicKind.Guardian);
            Test(!MythicFusion.Fuse(board,guardian,spawner)&&MythicFusion.Count==0,"Missing ingredients do not spawn or consume");
            Ingredients(MythicKind.Guardian);Test(MythicFusion.Fuse(board,guardian,spawner)&&MythicFusion.Count==1,"Exact recipe consumes 3 units for one Mythic");
            int ordinary=0;foreach(var u in UnitRegistry.All)if(u&&u.gameObject.activeSelf&&u.Team==UnitTeam.Ally&&u.Definition.Rank!=Rank.Mythic)ordinary++;
            Test(ordinary==0,"No consumed ingredient remains registered");
            for(int r=0;r<board.Grid.Rows;r++)for(int c=0;c<board.Grid.Cols;c++)if(board.CountAt(r,c)>0)Test(board.CountAt(r,c)==1&&!board.CanPlace(r,c,guardian),"Mythic cell refuses a second occupant");
            Ingredients(MythicKind.Robot);Test(MythicFusion.Fuse(board,MythicCatalog.Find(MythicKind.Robot),spawner)&&MythicFusion.Count==2,"Second distinct recipe works");
            Ingredients(MythicKind.Guardian);Test(MythicFusion.MatchIngredients(board,guardian,out var readyIngredients,out _)&&readyIngredients.Count==3,"Ingredient readiness remains visible when Mythic cap is full");
            int beforeCount=UnitRegistry.All.Count;Test(!MythicFusion.Fuse(board,guardian,spawner)&&beforeCount==UnitRegistry.All.Count,"Battle cap rejects fusion atomically");
            Directory.CreateDirectory("artsrc/units/mythic-integration");File.WriteAllLines("artsrc/units/mythic-integration/audit.txt",log);Debug.Log("[MythicAudit]\n"+string.Join("\n",log));
            Time.timeScale=speed;Object.Destroy(root);
        }
    }
}
#endif
