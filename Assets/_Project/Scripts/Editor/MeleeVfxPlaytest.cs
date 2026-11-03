#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Project.Battle;
using Project.Data;
namespace Project.EditorTools
{
    // Disposable in-play visual review. Never saves progress, scenes or definitions.
    public static class MeleeVfxPlaytest
    {
        static int group;static bool waiting;
        static readonly string[][] groups={
            new[]{"human_common","human_rare","machine_legendary","machine_common"},
            new[]{"dead_legendary","elf_rare","human_legendary","monster_rare"},
            new[]{"machine_epic","monster_epic","monster_legendary","dead_common"},
            new[]{"mythic_undead","mythic_robot","mythic_swordsman","mythic_bear"}};
        [MenuItem("SpinSquad/Diagnostics/Melee VFX review next group %&k")]
        static void Next()
        {
            if(!Application.isPlaying){Debug.LogWarning("[MeleeReview] Enter Play first.");return;}
            if(!Object.FindFirstObjectByType<BattleFlow>())
            {if(waiting)return;waiting=true;SceneManager.LoadScene("CharacterLab");EditorApplication.update+=Wait;return;}
            Prepare();
        }
        static void Wait(){if(!Application.isPlaying){waiting=false;EditorApplication.update-=Wait;return;}if(!Object.FindFirstObjectByType<BattleFlow>())return;waiting=false;EditorApplication.update-=Wait;Prepare();}
        [MenuItem("SpinSquad/Diagnostics/Melee VFX review x1")]
        static void X1(){if(Application.isPlaying)Time.timeScale=1;}
        [MenuItem("SpinSquad/Diagnostics/Melee VFX review x3")]
        static void X3(){if(Application.isPlaying)Time.timeScale=3;}
        [MenuItem("SpinSquad/Diagnostics/Capture Melee VFX review %&j")]
        static void Capture(){if(Application.isPlaying)ScreenCapture.CaptureScreenshot("artsrc/audits/melee-effects/unity-review.png");}
        static void Prepare()
        {
            var flow=Object.FindFirstObjectByType<BattleFlow>();var spawner=Object.FindFirstObjectByType<UnitSpawner>();var cam=Camera.main;if(!flow||!spawner||!cam)return;
            foreach(var unit in UnitRegistry.All.ToArray())if(unit)Object.Destroy(unit.gameObject);
            flow.StopAllCoroutines();flow.enabled=false;
            typeof(BattleFlow).GetMethod("SetPhase",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(flow,new object[]{BattleFlow.Phase.Fighting});
            var ids=groups[group++%groups.Length];
            for(int i=0;i<ids.Length;i++)
            {
                var original=ids[i].StartsWith("mythic_")?MythicCatalog.All.First(d=>d.UnitId==ids[i]):AssetDatabase.LoadAssetAtPath<UnitDefinition>("Assets/_Project/Data/Units/Unit_"+ids[i]+".asset");
                var def=Object.Instantiate(original);def.Rank=Rank.Common;def.Mythic=MythicKind.None;def.BaseAttack=1;def.BaseHp=10000000;def.MoveSpeed=1;def.AttackCooldown=.25f;
                var pos=cam.ViewportToWorldPoint(new Vector3(.38f,.48f-i*.07f,Mathf.Abs(cam.transform.position.z)));pos.z=0;
                var ally=spawner.Spawn(def,UnitTeam.Ally,pos);ally.GetComponent<UnitBrain>().enabled=true;
                var dummy=Object.Instantiate(def);dummy.UnitId="review_dummy";dummy.DisplayName="Review target";
                var enemy=spawner.Spawn(dummy,UnitTeam.Enemy,pos+Vector3.right*.45f);enemy.GetComponent<UnitBrain>().enabled=false;enemy.GetComponent<Project.Visual.FrameUnitVisual>().SetFacing(-1);
            }
            Debug.Log("[MeleeReview] Actual UnitBrain/attack callbacks, group="+string.Join(",",ids)+"; huge temporary HP, damage=1, skills disabled; no save/rewards. Stop Play to restore.");
        }
    }
}
#endif
