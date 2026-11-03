#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using Project.Battle;
using Project.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Project.EditorTools
{
    public static class SlimeEnemyChecks
    {
        static int playFrames;
        public static void RunPlay()
        {
            SessionState.SetBool("slime-check-play",true);
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/CharacterLab.unity");
            EditorApplication.EnterPlaymode();
        }
        public static void RunBossSkillPlay()
        {
            SessionState.SetBool("slime-boss-skill-check", true);
            RunPlay();
        }
        [InitializeOnLoadMethod]
        static void WatchPlay()
        {
            if(SessionState.GetBool("slime-check-play",false))EditorApplication.update+=CheckPlay;
        }
        static void CheckPlay()
        {
            if(!EditorApplication.isPlaying || ++playFrames<30)return;
            EditorApplication.update-=CheckPlay;
            SessionState.SetBool("slime-check-play",false);
            if (SessionState.GetBool("slime-boss-skill-check", false))
            {
                SessionState.SetBool("slime-boss-skill-check", false);
                var runner = UnityEngine.Object.FindFirstObjectByType<LegendaryCombat>();
                runner.StartCoroutine(CheckBossSkill());
                return;
            }
            try
            {
                var enemies=UnityEngine.Object.FindObjectsByType<Unit>(FindObjectsSortMode.None).Where(u=>u.Team==UnitTeam.Enemy).ToArray();
                if(enemies.Length!=3 || enemies.Any(u=>!u.Definition.UnitId.StartsWith("enemy_slime_")))throw new Exception("Scene enemies were not replaced: "+enemies.Length);
                if(enemies.Any(u=>u.GetComponent<Project.Visual.FrameUnitVisual>().BodyRenderer.sprite==null))throw new Exception("Missing enemy body sprite");
                File.WriteAllText("artsrc/enemies/chapter1-v1/play-check.txt","PASS: actual CharacterLab wave 1 spawns 3 slime enemies with live sprites: "+string.Join(", ",enemies.Select(u=>u.Definition.UnitId)));
                EditorApplication.Exit(0);
            }
            catch(Exception ex){Debug.LogException(ex);EditorApplication.Exit(1);}
        }
        static System.Collections.IEnumerator CheckBossSkill()
        {
            foreach(var brain in UnityEngine.Object.FindObjectsByType<UnitBrain>(FindObjectsSortMode.None)) brain.enabled=false;
            var melee=AssetDatabase.LoadAssetAtPath<UnitDefinition>("Assets/_Project/Data/Units/Unit_dead_common.asset");
            var ranged=AssetDatabase.LoadAssetAtPath<UnitDefinition>("Assets/_Project/Data/Units/Unit_elf_common.asset");
            var roster=new ChapterOneSlimes(new[]{melee,ranged});
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Units/Unit.prefab");
            var bossObject=UnityEngine.Object.Instantiate(prefab);
            var targetObject=UnityEngine.Object.Instantiate(prefab);
            var bossUnit=bossObject.GetComponent<Unit>();
            var targetUnit=targetObject.GetComponent<Unit>();
            var targetDef=UnityEngine.Object.Instantiate(melee);targetDef.BaseHp=1000000f;
            bossUnit.Initialize(roster.Pick(0,10,10,true),UnitTeam.Enemy);
            targetUnit.Initialize(targetDef,UnitTeam.Ally);
            bossObject.transform.position=new Vector3(100,100,0);targetObject.transform.position=new Vector3(100.3f,100,0);
            bossObject.GetComponent<UnitBrain>().enabled=false;targetObject.GetComponent<UnitBrain>().enabled=false;
            bossObject.GetComponent<Project.Visual.FrameUnitVisual>().Bind(bossUnit.Definition.Animations);
            yield return null;
            typeof(LegendaryCombat).GetField("flow",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(bossUnit.Combat,null);
            for(int i=0;i<5;i++)bossUnit.Combat.BasicHit();
            bool started=bossUnit.Combat.TryCast(targetUnit);
            float deadline=Time.time+5f;
            while(bossUnit.Combat.Casting && Time.time<deadline)yield return null;
            float expected=RunBuffs.DamageAfterArmor(bossUnit.AttackPower,UnitTeam.Ally)*2f;
            bool pass=started && !bossUnit.Combat.Casting && bossUnit.Combat.Mana==0 && Mathf.Abs(targetUnit.DamageTaken-expected)<.1f;
            File.WriteAllText("artsrc/enemies/chapter1-v1/boss-skill-check.txt",(pass?"PASS":"FAIL")+": double bite actual damage="+targetUnit.DamageTaken+", expected="+expected+", mana="+bossUnit.Combat.Mana);
            Debug.Log("[BossSkillCheck] "+(pass?"PASS":"FAIL"));
            EditorApplication.Exit(pass?0:1);
        }
        public static void Run()
        {
            var a=AssetDatabase.LoadAssetAtPath<UnitDefinition>("Assets/_Project/Data/Units/Unit_elf_common.asset");
            var b=AssetDatabase.LoadAssetAtPath<UnitDefinition>("Assets/_Project/Data/Units/Unit_dead_common.asset");
            var c=AssetDatabase.LoadAssetAtPath<UnitDefinition>("Assets/_Project/Data/Units/Unit_monster_common.asset");
            string old=a.UnitId;var hp=a.BaseHp;
            using(var roster=new ChapterOneSlimes(new[]{a,b,c}))
            {
                if(!roster.Ready)throw new Exception("Slime roster did not load");
                int melee=0,ranged=0;
                for(int i=0;i<4;i++)
                {
                    var d=roster.Pick(i,1,10,false);
                    if(d.IsRanged)ranged++;else melee++;
                    if(!d.IsRanged && (d.Animations.Attack.Count!=6 || d.Animations.ImpactStep!=3 || Mathf.Abs(d.Animations.Attack.Total-.85f)>.001f))throw new Exception("Melee bite playback/timing");
                    if(!d.Animations.SourceFacesLeft||!d.Portrait)throw new Exception(d.name+" visuals");
                    foreach(var clip in new[]{d.Animations.Idle,d.Animations.Walk,d.Animations.Attack,d.Animations.Dead})
                        if(!clip.IsValid||clip.Count!=clip.Durations.Length)throw new Exception(d.name+" clips");
                }
                if(melee!=2||ranged!=2)throw new Exception("Expected 2 melee / 2 ranged");
                if(roster.Pick(0,3,10,true).EnemyTier!=UnitDefinition.EnemyClass.Elite)throw new Exception("Elite schedule");
                if(roster.Pick(0,10,10,true).EnemyTier!=UnitDefinition.EnemyClass.Boss)throw new Exception("Boss schedule");
                if(roster.Pick(0,10,10,false).EnemyTier!=UnitDefinition.EnemyClass.Common)throw new Exception("Extra boss leak");
                var probe = new GameObject("Boss mana audit");
                try
                {
                    var u = probe.AddComponent<Unit>();
                    var combat = probe.AddComponent<LegendaryCombat>();
                    typeof(LegendaryCombat).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(combat, null);
                    u.Initialize(roster.Pick(0,10,10,true), UnitTeam.Enemy);
                    combat.ReceiveBasicHit();
                    if(combat.Mana != 0f)throw new Exception("Boss gained mana when hit");
                    for(int i=0;i<5;i++)combat.BasicHit();
                    if(combat.Mana != 100f || !combat.UsesMana)throw new Exception("Boss attack mana");
                    combat.ReceiveBasicHit();
                    if(combat.Mana != 100f)throw new Exception("Boss mana cap");
                }
                finally { UnityEngine.Object.DestroyImmediate(probe); }
                if(a.UnitId!=old||a.BaseHp!=hp)throw new Exception("Mutated ally template");
            }
            File.WriteAllText("artsrc/enemies/chapter1-v1/unity-check.txt","PASS: textures loaded, 4 common identities, 2 melee/2 ranged, all clips, elite/boss routing, ally definitions untouched.");
            Debug.Log("[SlimeEnemyChecks] PASS");
        }
    }
}
#endif
