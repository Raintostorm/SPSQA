using System.Collections.Generic;
using Project.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Read-only status portraits; skill timing remains owned by LegendaryCombat.
    public sealed class BattleAllyStrip : MonoBehaviour
    {
        sealed class Slot { public RectTransform Root; public Image Portrait,Fill; public Text State; }
        readonly List<Unit> units=new List<Unit>();
        readonly List<Slot> slots=new List<Slot>();
        RectTransform root,content;
        Font font;
        BattleFlow flow;
        float nextRefresh;
        void Start()
        {
            flow=FindFirstObjectByType<BattleFlow>();font=GetComponentInChildren<Text>().font;
            root=Rect(transform,"AllyStatusStrip",Vector2.zero,new Vector2(1060,190));
            root.gameObject.AddComponent<RectMask2D>();
            var hit=root.gameObject.AddComponent<Image>();hit.color=new Color(0,0,0,.01f);
            var scroll=root.gameObject.AddComponent<ScrollRect>();scroll.viewport=root;scroll.vertical=false;
            scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=35;
            content=Rect(root,"Portraits",Vector2.zero,new Vector2(1060,190));
            content.anchorMin=content.anchorMax=content.pivot=new Vector2(0,.5f);scroll.content=content;
            Refresh();
        }
        void Update()
        {
            if(!root)return;
            bool prep=flow&&flow.Current==BattleFlow.Phase.Prep;
            BattleUiSkin.Place(root,new Vector2(.5f,0),new Vector2(0,prep?355:40),new Vector2(1060,190));
            root.gameObject.SetActive(!flow||flow.Current!=BattleFlow.Phase.Finished);
            if(Time.unscaledTime<nextRefresh)return;nextRefresh=Time.unscaledTime+.15f;Refresh();
        }
        void Refresh()
        {
            units.Clear();foreach(var u in UnitRegistry.All)if(u&&u.Team==Project.Data.UnitTeam.Ally)units.Add(u);
            units.Sort((a,b)=>a.GetInstanceID().CompareTo(b.GetInstanceID()));
            content.sizeDelta=new Vector2(Mathf.Max(1060,units.Count*185),190);
            while(slots.Count<units.Count)slots.Add(CreateSlot());
            for(int i=0;i<slots.Count;i++)
            {
                var slot=slots[i];slot.Root.gameObject.SetActive(i<units.Count);if(i>=units.Count)continue;
                var u=units[i];slot.Root.anchoredPosition=new Vector2(90+i*185,0);
                slot.Portrait.sprite=u.Definition?(u.Definition.Portrait?u.Definition.Portrait:u.Definition.Animations?.Idle?.FrameAt(0)):null;
                slot.Portrait.enabled=slot.Portrait.sprite;
                slot.Portrait.color=u.IsAlive?Color.white:new Color(.45f,.45f,.45f);
                var combat=u.Combat;bool mana=combat&&combat.UsesMana;
                float fraction=mana?combat.Mana/LegendaryCombat.MaxMana:u.MaxHp>0?u.CurrentHp/u.MaxHp:0;
                slot.Fill.fillAmount=Mathf.Clamp01(fraction);
                slot.Fill.color=mana?new Color(.05f,.8f,1):new Color(.15f,.8f,.4f);
                slot.State.text=!u.IsAlive?"KO":combat&&combat.Casting?"CAST":mana?"MP":"HP";
            }
        }
        Slot CreateSlot()
        {
            var slot=new Slot();slot.Root=Rect(content,"Ally",Vector2.zero,new Vector2(170,180));
            slot.Root.anchorMin=slot.Root.anchorMax=new Vector2(0,.5f);
            BattleControlArt.Skin(slot.Root.gameObject.AddComponent<Image>(),5);
            var crop=Rect(slot.Root,"Crop",new Vector2(0,12),new Vector2(140,120));crop.gameObject.AddComponent<RectMask2D>();
            slot.Portrait=Rect(crop,"Portrait",new Vector2(0,-32),new Vector2(170,200)).gameObject.AddComponent<Image>();slot.Portrait.preserveAspect=true;slot.Portrait.raycastTarget=false;
            var track=Rect(slot.Root,"Track",new Vector2(0,-62),new Vector2(135,17)).gameObject.AddComponent<Image>();track.color=new Color(.02f,.07f,.12f);track.raycastTarget=false;
            slot.Fill=Rect(track.transform,"Fill",Vector2.zero,new Vector2(135,17)).gameObject.AddComponent<Image>();
            slot.Fill.sprite=BattleUiSkin.Neutral;slot.Fill.type=Image.Type.Filled;slot.Fill.fillMethod=Image.FillMethod.Horizontal;slot.Fill.raycastTarget=false;
            slot.State=Rect(slot.Root,"State",new Vector2(0,65),new Vector2(130,30)).gameObject.AddComponent<Text>();
            slot.State.font=font;slot.State.fontSize=22;slot.State.fontStyle=FontStyle.Bold;slot.State.color=Color.white;slot.State.alignment=TextAnchor.MiddleCenter;slot.State.raycastTarget=false;
            return slot;
        }
        static RectTransform Rect(Transform parent,string name,Vector2 pos,Vector2 size)
        {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);BattleUiSkin.Place(r,Vector2.one*.5f,pos,size);return r;}
    }
}
