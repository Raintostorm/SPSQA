#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
namespace Project.EditorTools
{
    public static class BattleItemPlaytest
    {
        [MenuItem("SpinSquad/Diagnostics/Audit stage 1-2 item grants")]
        static void Audit()
        {
            var inventory=Object.FindFirstObjectByType<Project.Battle.BattleItemInventory>();var flow=Object.FindFirstObjectByType<Project.Battle.BattleFlow>();
            if(!Application.isPlaying||!inventory||!flow||Project.Meta.GameProgress.CurrentLevel!=2||flow.Current!=Project.Battle.BattleFlow.Phase.Prep)return;
            var wave=typeof(Project.Battle.BattleFlow).GetField("<Wave>k__BackingField",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
            var update=typeof(Project.Battle.BattleItemInventory).GetMethod("Update",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
            int old=flow.Wave;try{
                wave.SetValue(flow,3);update.Invoke(inventory,null);update.Invoke(inventory,null);
                bool first=inventory.Items.Count==1&&inventory.Items[0].Type==0;
                wave.SetValue(flow,6);update.Invoke(inventory,null);update.Invoke(inventory,null);
                bool second=inventory.Items.Count==2&&inventory.Items[1].Type==1;
                bool combined=second&&inventory.Combine(inventory.Items[0].InstanceId,inventory.Items[1].InstanceId)&&inventory.Items.Count==1&&inventory.Items[0].Type==3;
                bool noDuplicate=!inventory.Combine(-1,-2);update.Invoke(inventory,null);noDuplicate&=inventory.Items.Count==1;
                Debug.Log("[StageItemAudit] wave3="+first+" wave6="+second+" combine="+combined+" noDuplicate="+noDuplicate);
            }finally{wave.SetValue(flow,old);}
        }
        [MenuItem("SpinSquad/Diagnostics/Play stage 1-2 item test")]
        static void Open(){if(!Application.isPlaying)return;Project.Meta.GameProgress.CurrentLevel=2;SceneManager.LoadScene("CharacterLab");}
        [MenuItem("SpinSquad/Diagnostics/Prepare item drag test")]
        static void PrepareDrag()
        {
            if(!Application.isPlaying)return;
            var board=Object.FindObjectsByType<Project.Battle.BattleBoard>(FindObjectsSortMode.None).FirstOrDefault(b=>b.name.ToLowerInvariant().Contains("ally"));
            var inventory=Object.FindFirstObjectByType<Project.Battle.BattleItemInventory>();
            if(!board||!inventory)return;
            Project.Battle.MythicFusion.Fuse(board,Project.Data.MythicCatalog.Find(Project.Data.MythicKind.Guardian),Object.FindFirstObjectByType<Project.Battle.UnitSpawner>());
            var add=typeof(Project.Battle.BattleItemInventory).GetMethod("Add",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
            add.Invoke(inventory,new object[]{0});add.Invoke(inventory,new object[]{1});add.Invoke(inventory,new object[]{3});
        }
        [MenuItem("SpinSquad/Diagnostics/Audit equipped items")]
        static void AuditEquipment()
        {
            var inventory=Object.FindFirstObjectByType<Project.Battle.BattleItemInventory>();if(!Application.isPlaying||!inventory)return;
            var mythic=Project.Battle.UnitRegistry.All.FirstOrDefault(u=>u&&u.Definition.Rank==Project.Data.Rank.Mythic);
            if(!mythic)return;
            Debug.Log("[ItemEquipAudit] count="+mythic.Equipment.Types.Count+" speed="+mythic.Equipment.SpeedBonus+" startingMana="+mythic.Equipment.StartingMana+" mana="+mythic.Mythic.Mana+" assigned="+inventory.Items.Count(i=>i.EquippedTo==mythic));
        }
        [MenuItem("SpinSquad/Diagnostics/Audit item pointer routing")]
        static void AuditPointer()
        {
            foreach(var drag in Object.FindObjectsByType<Project.UI.BattleItemDrag>(FindObjectsSortMode.None))
            {
                var rect=(RectTransform)drag.transform;var position=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center));
                var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=position};var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointer,hits);
                Debug.Log("[ItemPointerAudit] id="+drag.Id+" pos="+position+" hits="+string.Join(",",hits.Select(h=>h.gameObject.name)));
            }
        }
        [MenuItem("SpinSquad/Diagnostics/Test item drop handlers")]
        static void TestDrops()
        {
            if(!Application.isPlaying)return;
            var inventory=Object.FindFirstObjectByType<Project.Battle.BattleItemInventory>();
            var mythic=Project.Battle.UnitRegistry.All.FirstOrDefault(u=>u&&u.Definition.Rank==Project.Data.Rank.Mythic);
            var common=Project.Battle.UnitRegistry.All.FirstOrDefault(u=>u&&u.Team==Project.Data.UnitTeam.Ally&&u.Definition.Rank==Project.Data.Rank.Common);
            if(!inventory||!mythic)return;
            int tested=0;bool blocked=true;
            foreach(var drag in Object.FindObjectsByType<Project.UI.BattleItemDrag>(FindObjectsSortMode.None))
            {
                if(common)blocked&=!inventory.Equip(drag.Id,common);
                var body=mythic.GetComponent<Project.Visual.FrameUnitVisual>().BodyRenderer;
                var position=(Vector2)Camera.main.WorldToScreenPoint(body.bounds.center);
                var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=position,pointerDrag=drag.gameObject};
                drag.OnBeginDrag(pointer);drag.OnDrag(pointer);drag.OnEndDrag(pointer);
                blocked&=!inventory.Equip(drag.Id,mythic);tested++;
            }
            Debug.Log("[ItemDropTest] handlers="+tested+" invalidAndDuplicateBlocked="+blocked+" speed="+mythic.Equipment.SpeedBonus+" mana="+mythic.Mythic.Mana);
        }
    }
}
#endif
