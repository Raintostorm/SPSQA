#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using Project.Battle;
using Project.Data;
using UnityEditor;
using UnityEngine;
namespace Project.EditorTools
{
    // Disposable visual review; Stop Play restores the run. No saves or meta rewards.
    public static class BuffStatusPlaytest
    {
        [MenuItem("SpinSquad/Diagnostics/Preview Mana and Burn statuses")]
        static void Prepare()
        {
            var flow=Object.FindFirstObjectByType<BattleFlow>();var spawn=Object.FindFirstObjectByType<UnitSpawner>();var system=EffectBuffSystem.Current;
            if(!Application.isPlaying||!flow||!spawn||!system){Debug.LogWarning("Open battle in Play first.");return;}
            system.ResetRun();typeof(EffectBuffSystem).GetField("openingQueued",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(system,true);
            foreach(var unit in UnitRegistry.All.ToArray())if(unit)Object.Destroy(unit.gameObject);
            flow.StopAllCoroutines();flow.enabled=false;typeof(BattleFlow).GetMethod("SetPhase",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(flow,new object[]{BattleFlow.Phase.Fighting});
            var actors=new Unit[2];
            for(int i=0;i<2;i++)
            {
                var def=Object.Instantiate(MythicCatalog.All.First(d=>d.Mythic==MythicKind.Guardian));def.BaseHp=1000;
                var p=Camera.main.ViewportToWorldPoint(new Vector3(.35f+i*.3f,.49f,Mathf.Abs(Camera.main.transform.position.z)));p.z=0;
                actors[i]=spawn.Spawn(def,UnitTeam.Ally,p);actors[i].Mythic.Deployed=true;actors[i].GetComponent<UnitBrain>().enabled=false;
            }
            var loop=new GameObject("Temporary status review").AddComponent<BuffStatusPreviewLoop>();loop.actors=actors;
            Debug.Log("[BuffStatusReview] Two temporary Guardians; right has burn + small shield + mana. Repeats every 3 seconds; actual DOT/GainMana, no save or rewards. Stop Play to restore.");
        }
        [MenuItem("SpinSquad/Diagnostics/Capture Mana and Burn statuses")]
        static void Capture(){if(Application.isPlaying){System.IO.Directory.CreateDirectory("artsrc/ui/buff-proc-review-v4");ScreenCapture.CaptureScreenshot("artsrc/ui/buff-proc-review-v4/unity-installed.png");}}
    }
    public sealed class BuffStatusPreviewLoop:MonoBehaviour
    {
        public Unit[] actors;float next;
        void Update()
        {
            if(actors==null||Time.time<next)return;next=Time.time+3;
            foreach(var unit in actors)if(unit){unit.Heal(unit.MaxHp);typeof(MythicCombat).GetField("mana",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(unit.Mythic,0f);unit.Mythic.GainMana(20);}
            if(actors[1]){typeof(EffectBuffUnit).GetField("shield",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(actors[1].Effects,5f);actors[1].Effects.Burn(actors[0],20);}
        }
    }
}
#endif
