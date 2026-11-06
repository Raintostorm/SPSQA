using System;
using System.Linq;
using Project.Battle;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;
namespace Project.UI
{
    public sealed class EffectBuffChoiceView:MonoBehaviour
    {
        RectTransform safe,page;Font font;bool chosen;
        public static EffectBuffChoiceView Show(EffectBuffDefinition[] options,Action<EffectBuffKind> select,string progress=null)
        {
            var go=new GameObject("EffectBuffChoice",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
            var canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=600;
            var scale=go.GetComponent<CanvasScaler>();scale.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scale.referenceResolution=new Vector2(1125,2436);
            var view=go.AddComponent<EffectBuffChoiceView>();view.font=UiFonts.Display;view.Build(options,select,progress);return view;
        }
        RectTransform R(Transform p,string n,float x,float y,float w,float h)
        {var r=new GameObject(n,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(p,false);r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);return r;}
        Image Box(Transform p,string n,float x,float y,float w,float h,Color color)
        {var im=R(p,n,x,y,w,h).gameObject.AddComponent<Image>();im.color=color;return im;}
        void Text(Transform p,string value,float x,float y,float w,float h,int size,Color? color=null)
        {var t=R(p,"Label",x,y,w,h).gameObject.AddComponent<Text>();t.font=font;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.text=value;t.color=color??Color.white;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;}
        void Build(EffectBuffDefinition[] options,Action<EffectBuffKind> select,string progress)
        {
            var bg=Box(transform,"Backdrop",0,0,0,0,new Color(.035f,.055f,.16f));bg.rectTransform.anchorMin=Vector2.zero;bg.rectTransform.anchorMax=Vector2.one;bg.rectTransform.offsetMin=bg.rectTransform.offsetMax=Vector2.zero;
            safe=R(transform,"SafeArea",0,0,0,0);safe.anchorMin=Vector2.zero;safe.anchorMax=Vector2.one;safe.offsetMin=safe.offsetMax=Vector2.zero;
            page=R(safe,"Page",0,0,1080,2260);
            for(int i=0;i<20;i++){var d=Box(page,"Pattern",(i%4-1.5f)*290,(i/4-2)*470,75,75,new Color(.3f,.4f,.85f,.075f));d.transform.localRotation=Quaternion.Euler(0,0,45);d.raycastTarget=false;}
            bool legend=options[0].Rank==Rank.Legendary;
            // Ribbon title (UI-005): the old full-width "UPGRADE" bar looked like a button. Purple for Epic,
            // navy-with-gold for Legendary so it sits with the gold cards.
            ScreenChrome.Title(page,"CHOOSE A BUFF",new Vector2(0,720),820,legend?ScreenChrome.Banner.Screen:ScreenChrome.Banner.Buff);
            var tier=legend?new Color(1,.76f,.12f):new Color(.72f,.25f,1);
            Text(page,legend?"LEGENDARY":"EPIC",0,535,1000,100,60,tier);Text(page,"Choose 1 of 3",0,430,1000,80,40);
            var mythics=UnitRegistry.All.Where(u=>u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Definition.Rank==Rank.Mythic).GroupBy(u=>u.Definition).ToArray();
            for(int i=0;i<options.Length;i++)
            {
                var def=options[i];float x=(i-(options.Length-1)*.5f)*350;
                var card=Box(page,def.Name,x,-40,330,860,Color.white);BattleUiSkin.Skin(card,EffectBuffCardArt.Get(legend));card.type=Image.Type.Simple;
                var line=card.gameObject.AddComponent<Outline>();line.effectColor=tier;line.effectDistance=new Vector2(4,-4);
                var button=card.gameObject.AddComponent<Button>();button.targetGraphic=card;ButtonFamilySkin.States(button);
                button.onClick.AddListener(()=>{if(chosen)return;chosen=true;gameObject.SetActive(false);Destroy(gameObject);select(def.Kind);});
                var icon=Box(card.transform,"Icon",0,255,280,230,Color.white);icon.sprite=EffectBuffArt.Get(def.Kind);icon.preserveAspect=true;icon.raycastTarget=false;
                int level=EffectBuffSystem.Current?EffectBuffSystem.Current.Level(def.Kind):0;
                Text(card.transform,def.Name+(!EffectBuffCatalog.Persistent(def.Kind)?"":"\nLv."+level+" → "+(level+1)),0,60,305,150,36);
                Text(card.transform,!EffectBuffCatalog.Persistent(def.Kind)?def.Description:EffectBuffScaling.Describe(def.Kind,level+1),0,-115,300,250,34);
                bool relevant=def.Kind==EffectBuffKind.MythicMight||def.Kind==EffectBuffKind.ManaFlow||def.Kind==EffectBuffKind.EmberTouch||def.Kind==EffectBuffKind.SoulHarvest||def.Kind==EffectBuffKind.DoubleStrike||def.Kind==EffectBuffKind.Bodyguard||def.Kind==EffectBuffKind.FrostTouch||def.Kind==EffectBuffKind.WeaponSupplies||def.Kind==EffectBuffKind.SecondWind;
                if(relevant&&mythics.Length>0)
                {
                    // Affected Mythics live INSIDE the card (UI-002): a separate box under only some cards
                    // read as a fourth option. Up to 3 portraits, then "+N" for the rest.
                    var related=Box(card.transform,"RelatedMythics",0,-335,300,150,new Color(0,0,0,.28f));related.raycastTarget=false;
                    Text(related.transform,"MYTHICS",0,55,290,34,24);
                    var shown=mythics.Take(3).ToArray();int extra=mythics.Length-shown.Length;float step=extra>0?72:92;
                    for(int j=0;j<shown.Length;j++)
                    {
                        var g=shown[j];float px=(j-(shown.Length-(extra>0?0:1))*.5f)*step;
                        var im=Box(related.transform,"Mythic",px,-2,90,90,Color.white);im.sprite=g.Key.Portrait??g.Key.Animations?.Idle?.FrameAt(0);im.preserveAspect=true;im.raycastTarget=false;
                        Text(related.transform,"×"+g.Count(),px+26,-46,60,30,26);
                    }
                    if(extra>0)Text(related.transform,"+"+extra,(shown.Length*.5f)*step,-5,70,60,32);
                }

            }
            Text(page,progress??"Tap a card to choose",0,-930,1000,90,36);LateUpdate();
        }
        void LateUpdate(){if(!safe||!page)return;var area=Screen.safeArea;safe.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);safe.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);page.localScale=Vector3.one*Mathf.Min(safe.rect.width/1080,safe.rect.height/2260);}
    }
}
