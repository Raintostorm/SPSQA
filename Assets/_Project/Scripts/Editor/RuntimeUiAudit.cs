#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
namespace Project.EditorTools
{
    public static class RuntimeUiAudit
    {
        [MenuItem("SpinSquad/Diagnostics/Audit active UI")]
        static void Audit()
        {
            var report=new StringBuilder();report.AppendLine("SCENE "+UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            var es=EventSystem.current;report.AppendLine("EVENTSYSTEM "+(es?es.name:"MISSING"));
            foreach(var module in Object.FindObjectsByType<InputSystemUIInputModule>(FindObjectsSortMode.None))report.AppendLine("INPUT "+module.name+" active="+module.isActiveAndEnabled+" point="+module.point?.action?.enabled+" click="+module.leftClick?.action?.enabled);
            foreach(var b in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
            {
                if(!b.gameObject.activeInHierarchy)continue;var rect=(RectTransform)b.transform;var canvas=b.GetComponentInParent<Canvas>();var camera=canvas&&canvas.renderMode!=RenderMode.ScreenSpaceOverlay?canvas.worldCamera:null;
                var center=RectTransformUtility.WorldToScreenPoint(camera,rect.TransformPoint(rect.rect.center));var hits=new List<RaycastResult>();if(es)es.RaycastAll(new PointerEventData(es){position=center},hits);
                var hit=hits.Count>0?hits[0].gameObject:null;bool reaches=hit&&(hit.transform==b.transform||hit.transform.IsChildOf(b.transform));
                report.AppendLine("BUTTON "+PathOf(b.transform)+" enabled="+b.IsInteractable()+" size="+rect.rect.size+" center="+center+" hit="+(hit?PathOf(hit.transform):"NONE")+" reachable="+reaches);
            }
            foreach(var t in Object.FindObjectsByType<Text>(FindObjectsSortMode.None))
            {
                if(!t.gameObject.activeInHierarchy||string.IsNullOrWhiteSpace(t.text)||t.resizeTextForBestFit)continue;
                if(t.verticalOverflow==VerticalWrapMode.Truncate&&t.preferredHeight>t.rectTransform.rect.height+3)report.AppendLine("TEXT_HEIGHT "+PathOf(t.transform)+" area="+t.rectTransform.rect.size+" needs="+t.preferredHeight+" text="+t.text.Replace('\n','|'));
            }
            Directory.CreateDirectory("outdata/ui-audit");File.WriteAllText("outdata/ui-audit/"+UnityEngine.SceneManagement.SceneManager.GetActiveScene().name+".txt",report.ToString());Debug.Log("[RuntimeUiAudit] report saved");
        }
        static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
    }
}
#endif
