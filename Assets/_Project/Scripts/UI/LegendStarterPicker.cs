using System;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>Session-only Chapter 1-2 starter. Selection grants through BattleSetup.</summary>
    public static class LegendStarterPicker
    {
        public static GameObject Show(UnitDefinition[] choices, Action<UnitDefinition> choose, int wave = 1)
        {
            var go=new GameObject("LegendStarterChoice",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
            var canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=700;
            var scaler=go.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1125,2436);scaler.matchWidthOrHeight=0;
            var bg=Rect(go.transform,"Backdrop",Vector2.zero,Vector2.zero);bg.anchorMin=Vector2.zero;bg.anchorMax=Vector2.one;bg.offsetMin=bg.offsetMax=Vector2.zero;
            bg.gameObject.AddComponent<Image>().color=new Color(.035f,.055f,.1f,.97f);
            var content=Rect(go.transform,"Choices",Vector2.zero,new Vector2(1050,1350));
            Label(content,$"CHAPTER 1-2 · WAVE {wave}",new Vector2(0,610),new Vector2(1000,80),42,Color.white);
            Label(content,"CHOOSE YOUR LEGEND",new Vector2(0,500),new Vector2(1000,100),62,new Color(1,.84f,.25f));
            Label(content,"Pick one legend for this wave · Replaces the previous test legend",new Vector2(0,400),new Vector2(1000,80),30,Color.white);
            int index=0;
            foreach(var definition in choices)
            {
                if(!definition || definition.Rank!=Rank.Legendary)continue;
                var picked=definition;
                bool bottom=index>=3;
                var position=new Vector2(bottom?(index-3-.5f)*320:(index-1)*320,bottom?-270:110);
                var card=Rect(content,definition.name,position,new Vector2(290,330));
                var image=card.gameObject.AddComponent<Image>();BattleUiSkin.Skin(image,BattleUiSkin.Navy);
                var button=card.gameObject.AddComponent<Button>();button.targetGraphic=image;
                button.onClick.AddListener(()=>choose(picked));
                var portrait=Rect(card,"Portrait",new Vector2(0,38),new Vector2(180,180)).gameObject.AddComponent<Image>();
                portrait.sprite=definition.Portrait;
                if(!portrait.sprite && definition.Animations && definition.Animations.Idle!=null && definition.Animations.Idle.IsValid)portrait.sprite=definition.Animations.Idle.FrameAt(0);
                portrait.preserveAspect=true;portrait.raycastTarget=false;portrait.enabled=portrait.sprite!=null;
                Label(card,definition.Race.ToString(),new Vector2(0,131),new Vector2(270,45),30,new Color(1,.84f,.25f));
                Label(card,definition.DisplayName,new Vector2(0,-88),new Vector2(270,70),28,Color.white);
                Label(card,"SELECT",new Vector2(0,-139),new Vector2(270,40),26,new Color(1,.84f,.25f));
                index++;
            }
            return go;
        }
        static RectTransform Rect(Transform parent,string name,Vector2 position,Vector2 size)
        {
            var rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();rect.SetParent(parent,false);
            BattleUiSkin.Place(rect,Vector2.one*.5f,position,size);return rect;
        }
        static void Label(Transform parent,string value,Vector2 position,Vector2 size,int fontSize,Color color)
        {
            var text=Rect(parent,"Label",position,size).gameObject.AddComponent<Text>();text.text=value;text.font=UiFonts.Display;text.fontSize=fontSize;text.fontStyle=FontStyle.Bold;text.alignment=TextAnchor.MiddleCenter;text.color=color;text.raycastTarget=false;
        }
    }
}
