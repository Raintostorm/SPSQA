using System;
using Project.Battle;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Project.UI
{
    public sealed class BattleItemDrag:MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IBeginDragHandler,IDragHandler,IEndDragHandler,IDropHandler
    {
        public BattleItemInventory Inventory;public int Id;public Action Changed,Inspect;GameObject ghost;RectTransform ghostRect;Unit hovered;Vector2 press;
        public static bool IsDragging{get;private set;}
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void ResetDrag()=>IsDragging=false;
        public void OnPointerDown(PointerEventData e){press=e.position;if(e.button==PointerEventData.InputButton.Left)OnBeginDrag(e);}
        public void OnPointerUp(PointerEventData e){bool tap=ghost&&(e.position-press).sqrMagnitude<100;if(ghost)OnEndDrag(e);if(tap)Inspect?.Invoke();}
        public void OnBeginDrag(PointerEventData e)
        {
            if(ghost||IsDragging||SharedSettings.IsPaused||e.button!=PointerEventData.InputButton.Left)return;IsDragging=true;var canvas=GetComponentInParent<Canvas>().rootCanvas;
            ghost=new GameObject("Dragged item",typeof(RectTransform),typeof(Image),typeof(CanvasGroup));ghost.transform.SetParent(canvas.transform,false);ghostRect=ghost.GetComponent<RectTransform>();ghostRect.sizeDelta=new Vector2(125,125);
            var images=GetComponentsInChildren<Image>();var image=ghost.GetComponent<Image>();image.sprite=images.Length>1?images[1].sprite:images[0].sprite;image.raycastTarget=false;image.preserveAspect=true;ghost.GetComponent<CanvasGroup>().blocksRaycasts=false;OnDrag(e);
        }
        public void OnDrag(PointerEventData e)
        {
            if(!ghostRect)return;RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)ghostRect.parent,e.position,e.pressEventCamera,out var p);ghostRect.anchoredPosition=p+new Vector2(0,60);
            var next=Inventory.TargetAt(e.position);if(next==hovered)return;ClearTarget();hovered=next;if(hovered){var ring=hovered.GetComponent<Project.Visual.PrepUnitRing>();if(ring)ring.EquipmentTarget=true;}
        }
        public void OnEndDrag(PointerEventData e)
        {
            bool equipped=false;Unit equippedUnit=null;
            if(ghost&&!SharedSettings.IsPaused)
            {
                var hit=e.pointerCurrentRaycast.gameObject;
                // Never equip through an inventory/control panel to a unit behind it.
                if(!hit||(!hit.transform.IsChildOf(transform.parent)&&!hit.GetComponentInParent<Button>()&&hit.name!="Item inventory"))
                {equippedUnit=Inventory.TargetAt(e.position);equipped=Inventory.Equip(Id,equippedUnit);}
            }
            Cleanup();if(equipped){BattleItemFeedback.Equipped(equippedUnit);Changed?.Invoke();}
        }
        void ClearTarget(){if(hovered){var ring=hovered.GetComponent<Project.Visual.PrepUnitRing>();if(ring)ring.EquipmentTarget=false;}hovered=null;}
        void Cleanup(){ClearTarget();IsDragging=false;if(ghost)Destroy(ghost);ghost=null;}
        void OnDisable()=>Cleanup();
        public void OnDrop(PointerEventData e){var other=e.pointerDrag?e.pointerDrag.GetComponent<BattleItemDrag>():null;if(other&&other.Inventory==Inventory&&Inventory.Combine(other.Id,Id))Changed?.Invoke();}
    }
}
