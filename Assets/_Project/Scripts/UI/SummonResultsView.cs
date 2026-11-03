using System.Collections.Generic;
using System.Linq;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Presentation only: TryRoll has already granted and saved the rewards.
    public sealed class SummonResultsView : MonoBehaviour
    {
        RectTransform safe, page;
        Font font;
        public static void Show(Transform parent, List<AllySummon.Result> results, int rolls)
        {
            if(parent.GetComponentInChildren<SummonResultsView>())return;
            var go=new GameObject("SummonResults",typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var full=go.GetComponent<RectTransform>();full.anchorMin=Vector2.zero;full.anchorMax=Vector2.one;full.offsetMin=full.offsetMax=Vector2.zero;
            go.GetComponent<Image>().color=new Color(.075f,.09f,.17f);
            var canvas=go.AddComponent<Canvas>();canvas.overrideSorting=true;canvas.sortingOrder=450;go.AddComponent<GraphicRaycaster>();
            var view=go.AddComponent<SummonResultsView>();view.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");view.Build(results,rolls);
        }
        void LateUpdate()
        {
            var area=Screen.safeArea;safe.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);safe.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);
            page.localScale=Vector3.one*Mathf.Max(.1f,Mathf.Min(1,Mathf.Min((safe.rect.width-35)/1060,(safe.rect.height-40)/2250)));
        }
        void Build(List<AllySummon.Result> results,int rolls)
        {
            safe=R(transform,"SafeArea",0,0,0,0);safe.anchorMin=Vector2.zero;safe.anchorMax=Vector2.one;safe.offsetMin=safe.offsetMax=Vector2.zero;
            page=R(safe,"Page",0,0,1060,2250);page.anchorMin=page.anchorMax=page.pivot=Vector2.one*.5f;page.anchoredPosition=Vector2.zero;
            var header=Box(page,"Header",0,20,1020,220,Color.white);BattleUiSkin.Skin(header,BattleUiSkin.Navy);
            T(header.transform,"SUMMON RESULTS",0,20,980,110,66);
            T(header.transform,rolls.ToString("N0")+" summons",0,135,980,65,38);
            var groups=results.Where(r=>r.MythicStone||r.Unit).GroupBy(r=>r.MythicStone?"__mythic_stone":r.Unit.UnitId).OrderByDescending(g=>g.First().MythicStone?4:(int)g.First().Unit.Rank).ThenBy(g=>g.Key).ToArray();
            Summary(-260,"Ally shards",results.Where(r=>!r.MythicStone).Sum(r=>(long)r.Shards),Resources.Load<Sprite>("Shop/fragment"));
            Summary(260,"Mythic Stones",results.Where(r=>r.MythicStone).Sum(r=>(long)r.Shards),MythicStoneArt.Icon);
            var viewport=R(page,"RewardsScroll",0,555,1040,1310);viewport.gameObject.AddComponent<RectMask2D>();viewport.gameObject.AddComponent<Image>().color=new Color(0,0,0,.001f);
            var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.viewport=viewport;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=65;
            var content=R(viewport,"Content",0,0,1020,Mathf.Max(1310,Mathf.CeilToInt(groups.Length/3f)*550));scroll.content=content;CollectionScrollHint.Install(scroll);
            foreach(var pair in groups.Select((g,i)=>new {Group=g,Index=i}))
            {
                bool stone=pair.Group.First().MythicStone;var unit=pair.Group.First().Unit;var color=stone?new Color(.75f,.3f,.07f):unit.Rank switch{Project.Data.Rank.Rare=>new Color(.06f,.40f,.62f),Project.Data.Rank.Epic=>new Color(.45f,.14f,.63f),Project.Data.Rank.Legendary=>new Color(.65f,.43f,.06f),_=>new Color(.40f,.30f,.22f)};
                var card=Box(content,"AllyReward",(pair.Index%3-1)*340,pair.Index/3*550,320,530,color);RelicUiSkin.Apply(card,3,color);
                var crop=R(card.transform,"PortraitCrop",0,20,290,270);crop.gameObject.AddComponent<RectMask2D>();
                var sprite=stone?MythicStoneArt.Icon:Resources.Load<Sprite>("ShopV2/Heads/"+unit.UnitId);if(!sprite&&!stone)sprite=unit.Portrait;if(!stone&&!sprite && unit.Animations!=null && unit.Animations.Idle!=null && unit.Animations.Idle.IsValid)sprite=unit.Animations.Idle.FrameAt(0);
                var art=Box(crop,"Portrait",0,5,260,260,Color.white);art.sprite=sprite;art.preserveAspect=true;art.enabled=sprite;
                T(card.transform,stone?"Mythic Stone":unit.DisplayName,0,295,300,105,38);
                T(card.transform,"×"+pair.Group.Sum(r=>(long)r.Shards).ToString("N0"),0,405,290,100,72);
            }
            T(page,"Rewards saved · Mythic Stones upgrade any Mythic",0,1910,1020,100,32);
            var done=Box(page,"Continue",0,2090,1020,145,Color.white);BattleUiSkin.Skin(done,BattleUiSkin.Blue);done.raycastTarget=true;
            var button=done.gameObject.AddComponent<Button>();button.targetGraphic=done;ButtonFamilySkin.States(button);button.onClick.AddListener(()=>{gameObject.SetActive(false);Destroy(gameObject);});T(done.transform,"CONTINUE",0,0,1000,145,64);
            Canvas.ForceUpdateCanvases();content.anchoredPosition=Vector2.zero;scroll.verticalNormalizedPosition=1;LateUpdate();
        }
        void Summary(float x,string label,long total,Sprite sprite)
        {
            var tile=Box(page,label,x,285,500,220,Color.white);BattleUiSkin.Skin(tile,BattleUiSkin.Navy);
            var icon=Box(tile.transform,"Icon",-150,40,110,120,Color.white);icon.sprite=sprite;icon.preserveAspect=true;
            T(tile.transform,total.ToString("N0"),65,15,330,120,88);
            T(tile.transform,label,40,145,400,65,35);
        }
        RectTransform R(Transform p,string n,float x,float y,float w,float h)
        {var r=new GameObject(n,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(p,false);r.anchorMin=r.anchorMax=new Vector2(.5f,1);r.pivot=new Vector2(.5f,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        Image Box(Transform p,string n,float x,float y,float w,float h,Color c)
        {var im=R(p,n,x,y,w,h).gameObject.AddComponent<Image>();im.color=c;im.raycastTarget=false;return im;}
        void T(Transform p,string value,float x,float y,float w,float h,int size)
        {var t=R(p,"Label",x,y,w,h).gameObject.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.raycastTarget=false;var o=t.gameObject.AddComponent<Outline>();o.effectDistance=new Vector2(2,-2);o.effectColor=new Color(.02f,.02f,.04f);}
    }
}
