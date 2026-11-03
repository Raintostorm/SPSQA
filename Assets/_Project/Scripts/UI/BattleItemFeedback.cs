using Project.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>Short UI-only acknowledgement, following the equipped unit.</summary>
    public sealed class BattleItemFeedback : MonoBehaviour
    {
        Unit target; RectTransform label; float end; Project.Visual.PrepUnitRing ring;
        public static void Equipped(Unit target)
        {
            if(!target)return;
            var go=new GameObject("Item equipped feedback");var feedback=go.AddComponent<BattleItemFeedback>();feedback.target=target;feedback.Build();
        }
        void Build()
        {
            end=Time.unscaledTime+1.35f;ring=target.GetComponent<Project.Visual.PrepUnitRing>();if(ring)ring.EquipmentTarget=true;
            var canvas=gameObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=480;
            var scaler=gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1125,2436);
            label=new GameObject("Equipped",typeof(RectTransform),typeof(Image)).GetComponent<RectTransform>();label.SetParent(transform,false);label.sizeDelta=new Vector2(320,80);BattleUiSkin.Skin(label.GetComponent<Image>(),BattleUiSkin.Dark);label.GetComponent<Image>().raycastTarget=false;
            var text=new GameObject("Text",typeof(RectTransform),typeof(Text)).GetComponent<Text>();text.transform.SetParent(label,false);BattleUiSkin.Place(text.rectTransform,Vector2.one*.5f,Vector2.zero,new Vector2(310,75));text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.fontSize=36;text.fontStyle=FontStyle.Bold;text.alignment=TextAnchor.MiddleCenter;text.color=new Color(.5f,1,.7f);text.text="✓  Equipped";text.raycastTarget=false;
        }
        void LateUpdate()
        {
            if(!target||!target.IsAlive||Time.unscaledTime>=end){Destroy(gameObject);return;}
            var camera=Camera.main;if(!camera)return;
            var point=camera.WorldToScreenPoint(target.HitPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform,point,null,out var local);
            var rect=(RectTransform)transform;local.x=Mathf.Clamp(local.x,rect.rect.xMin+170,rect.rect.xMax-170);local.y=Mathf.Clamp(local.y+130,rect.rect.yMin+60,rect.rect.yMax-60);label.anchoredPosition=local;
        }
        void OnDestroy(){if(ring&&!BattleItemDrag.IsDragging)ring.EquipmentTarget=false;}
    }
}
