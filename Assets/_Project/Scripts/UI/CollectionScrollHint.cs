using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // A non-interactive thumb; all input stays with the existing ScrollRect.
    public sealed class CollectionScrollHint : MonoBehaviour
    {
        ScrollRect scroll;
        Image thumb;
        public static void Install(ScrollRect target)
        {
            var hint=target.gameObject.AddComponent<CollectionScrollHint>();hint.scroll=target;
            var go=new GameObject("ScrollIndicator",typeof(RectTransform),typeof(Image));
            go.transform.SetParent(target.viewport,false);hint.thumb=go.GetComponent<Image>();
            hint.thumb.color=new Color(.8f,.72f,.9f,.65f);hint.thumb.raycastTarget=false;
            var r=hint.thumb.rectTransform;r.anchorMin=r.anchorMax=new Vector2(1,1);r.pivot=new Vector2(1,1);r.sizeDelta=new Vector2(7,80);
        }
        void LateUpdate()
        {
            if(!scroll||!scroll.content||!scroll.viewport)return;
            float view=scroll.viewport.rect.height,total=scroll.content.rect.height;
            thumb.enabled=total>view+1;if(!thumb.enabled)return;
            float height=Mathf.Max(50,view*view/total);
            thumb.rectTransform.sizeDelta=new Vector2(7,height);
            thumb.rectTransform.anchoredPosition=new Vector2(-4,-(view-height)*(1-scroll.verticalNormalizedPosition));
            thumb.transform.SetAsLastSibling();
        }
    }
}
