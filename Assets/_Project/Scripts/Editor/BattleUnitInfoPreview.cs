#if UNITY_EDITOR
using System.Linq;
using Project.Battle;
using Project.Data;
using Project.UI;
using UnityEditor;
using UnityEngine;
namespace Project.EditorTools
{
    public static class BattleUnitInfoPreview
    {
        [MenuItem("SpinSquad/Diagnostics/Preview Common unit info")]
        static void Common()
        {
            if(!Application.isPlaying)return;
            var unit=UnitRegistry.All.FirstOrDefault(u=>u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Definition.Rank==Rank.Common);
            if(unit){Debug.Assert(unit.GetComponent<UnitBrain>().AttacksPerSecond>0,"Preparation speed must be available before the brain starts");BattleUnitInfo.Show(unit);}
        }
        [MenuItem("SpinSquad/Diagnostics/Preview Mythic unit info")]
        static void Mythic()
        {
            if(!Application.isPlaying)return;
            var unit=UnitRegistry.All.FirstOrDefault(u=>u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Definition.Rank==Rank.Mythic);
            if(!unit)
            {
                var board=Object.FindObjectsByType<BattleBoard>(FindObjectsSortMode.None).FirstOrDefault(b=>b.name.ToLowerInvariant().Contains("ally"));
                if(!board)return;
                MythicFusion.Fuse(board,MythicCatalog.Find(MythicKind.Guardian),Object.FindFirstObjectByType<UnitSpawner>());
                unit=UnitRegistry.All.FirstOrDefault(u=>u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Definition.Rank==Rank.Mythic);
            }
            if(unit)BattleUnitInfo.Show(unit);
        }
    }
}
#endif
