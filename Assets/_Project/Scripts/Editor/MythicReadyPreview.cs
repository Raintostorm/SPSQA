#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Project.Battle;
using Project.Data;
using UnityEditor;
using UnityEngine;
namespace Project.EditorTools
{
    public static class MythicReadyPreview
    {
        [MenuItem("SpinSquad/Diagnostics/Prepare Mythic Ready UI (no meta grants)")]
        static void Prepare()
        {
            var flow=Object.FindFirstObjectByType<BattleFlow>();if(!Application.isPlaying||!flow||flow.Current!=BattleFlow.Phase.Prep)return;
            var board=Object.FindObjectsByType<BattleBoard>(FindObjectsSortMode.None).FirstOrDefault(b=>b.name.ToLowerInvariant().Contains("ally"));var spawner=Object.FindFirstObjectByType<UnitSpawner>();if(!board||!spawner)return;
            var output=MythicCatalog.Find(MythicKind.Guardian);var made=new List<Unit>();
            foreach(var id in MythicFusion.For(output).ingredients)
            {
                var def=AssetDatabase.LoadAssetAtPath<UnitDefinition>("Assets/_Project/Data/Units/Unit_"+id+".asset");
                if(!def||!board.FindSpotFor(def,out int r,out int c)){foreach(var old in made){board.Remove(old);Object.Destroy(old.gameObject);}return;}
                var u=spawner.Spawn(def,UnitTeam.Ally,board.Grid.CellToWorld(r,c),board.transform);if(!u)return;
                var brain=u.GetComponent<UnitBrain>();if(brain)brain.enabled=false;board.Place(u,r,c);made.Add(u);
            }
            Debug.Log("[MythicReadyPreview] Three temporary board ingredients added. No save, gold or shards changed.");
        }
        [MenuItem("SpinSquad/Diagnostics/Capture Mythic Detail UI")]
        static void Capture()
        {if(Application.isPlaying)ScreenCapture.CaptureScreenshot("artsrc/units/mythic-integration/mythic-detail.png");}
    }
}
#endif
