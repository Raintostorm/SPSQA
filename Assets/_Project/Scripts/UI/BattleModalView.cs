using System;
using System.Linq;
using Project.Battle;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Real uGUI controls: no baked text or fake statistical values from the mockup.
    public sealed class BattleModalView
    {
        readonly Font font;
        public readonly RectTransform Panel;
        static Sprite frame,inset,plaque,beige;
        static Sprite Surface(Color top,Color bottom)
        {
            var texture=new Texture2D(128,128,TextureFormat.RGBA32,false);
            var pixels=new Color[128*128];
            for(int y=0;y<128;y++)for(int x=0;x<128;x++)
            {
                float dx=Mathf.Max(Mathf.Abs(x-63.5f)-48,0),dy=Mathf.Max(Mathf.Abs(y-63.5f)-48,0);
                float d=Mathf.Sqrt(dx*dx+dy*dy);var c=d>10?Color.black:y>113?top:bottom;c.a=Mathf.Clamp01(16-d);pixels[y*128+x]=c;
            }
            texture.SetPixels(pixels);texture.Apply();texture.wrapMode=TextureWrapMode.Clamp;
            return Sprite.Create(texture,new Rect(0,0,128,128),Vector2.one*.5f,100,0,SpriteMeshType.FullRect,new Vector4(20,20,20,20));
        }
        public BattleModalView(Transform parent,Font font,string title,Vector2 size,Action close)
        {
            this.font=font;
            if(!frame)frame=Surface(new Color(.27f,.47f,.59f),new Color(.045f,.10f,.15f));
            if(!inset)inset=Surface(new Color(.08f,.17f,.23f),new Color(.035f,.08f,.12f));
            if(!plaque)plaque=Surface(new Color(.21f,.21f,.21f),new Color(.1f,.1f,.1f));
            if(!beige)beige=Surface(new Color(.86f,.77f,.58f),new Color(.68f,.56f,.4f));
            Panel=Box(parent,"Panel",Vector2.zero,size,frame);
            float top=size.y/2;
            var header=Box(Panel,"Header",new Vector2(0,top-80),new Vector2(580,100),plaque);
            Label(header,title,Vector2.zero,new Vector2(530,90),52);
            foreach(float x in new[]{-262f,262f})Label(header,"•",new Vector2(x,0),new Vector2(32,60),30,new Color(1,.72f,.28f));
            if(close!=null){var x=Button(Panel,"",new Vector2(size.x/2-75,top-80),new Vector2(80,80),BattleUiSkin.Red,close);BattleChrome.Draw(x,4,Vector2.zero,new Vector2(80,80));}
        }
        public RectTransform Rect(Transform parent,string name,Vector2 pos,Vector2 size)
        {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.anchoredPosition=pos;r.sizeDelta=size;return r;}
        public RectTransform Box(Transform parent,string name,Vector2 pos,Vector2 size,Sprite skin=null)
        {var r=Rect(parent,name,pos,size);var im=r.gameObject.AddComponent<Image>();BattleUiSkin.Skin(im,skin?skin:inset);return r;}
        public Text Label(Transform parent,string value,Vector2 pos,Vector2 size,int fontsize,Color? color=null,TextAnchor align=TextAnchor.MiddleCenter)
        {
            var t=Rect(parent,"Label",pos,size).gameObject.AddComponent<Text>();t.text=value;t.font=font;t.fontSize=fontsize;t.fontStyle=FontStyle.Bold;t.color=color??Color.white;t.alignment=align;t.raycastTarget=false;
            var o=t.gameObject.AddComponent<Outline>();o.effectColor=new Color(0,0,0,.9f);o.effectDistance=new Vector2(2,-2);return t;
        }
        public RectTransform Button(Transform parent,string label,Vector2 pos,Vector2 size,Sprite sprite,Action action,int fontSize=42)
        {
            var r=Box(parent,label,pos,size,sprite);var b=r.gameObject.AddComponent<Button>();b.targetGraphic=r.GetComponent<Image>();b.onClick.AddListener(()=>action());ButtonFamilySkin.States(b);Label(r,label,Vector2.zero,size-Vector2.one*12,fontSize);return r;
        }
        public void Settings(bool battle,Action close,Action redraw,Action feedback)
        {
            string[] titles={"Sound","Damage Numbers","Effects","Vibration"};
            string[] icons={"sound","polish-damage","polish-effects","vibration"};
            string[] keys={"ui.sound","ui.damage","ui.effects","ui.vibration"};
            int count=Application.isMobilePlatform?4:3;
            for(int i=0;i<count;i++)ToggleRow(titles[i],icons[i],keys[i],270-i*170,redraw,feedback);
            ScreenChrome.Action(Button(Panel,battle?"Resume":"Close",new Vector2(0,-420),new Vector2(810,120),BattleUiSkin.Gold,close),ScreenChrome.Role.Primary);
        }
        void ToggleRow(string title,string icon,string key,float y,Action redraw,Action feedback)
        {
            bool on=PlayerPrefs.GetInt(key,key=="ui.vibration"?0:1)!=0;
            var row=Box(Panel,title,new Vector2(0,y),new Vector2(820,145));
            var b=row.gameObject.AddComponent<Button>();b.targetGraphic=row.GetComponent<Image>();ButtonFamilySkin.States(b);
            b.onClick.AddListener(()=>{PlayerPrefs.SetInt(key,on?0:1);PlayerPrefs.Save();feedback();redraw();});
            if(icon.StartsWith("polish-"))
            {
                var art=Rect(row,"Icon",new Vector2(-325,0),new Vector2(100,85)).gameObject.AddComponent<RawImage>();
                art.texture=Resources.Load<Texture2D>("SettingsIcons/toggles");art.uvRect=new Rect(icon=="polish-damage"?0:.5f,0,.5f,1);art.raycastTarget=false;art.enabled=art.texture!=null;
            }
            else {var art=Rect(row,"Icon",new Vector2(-325,0),new Vector2(85,85)).gameObject.AddComponent<Image>();art.sprite=Resources.Load<Sprite>("SharedUI/"+icon);art.preserveAspect=true;art.raycastTarget=false;}
            Label(row,title,new Vector2(-65,0),new Vector2(390,100),36,null,TextAnchor.MiddleLeft);
            var state=Box(row,"Switch",new Vector2(285,0),new Vector2(190,85),on?BattleUiSkin.Green:BattleUiSkin.Navy);
            Label(state,on?"ON":"OFF",new Vector2(on?-28:28,0),new Vector2(100,70),30);
            Box(state,"Thumb",new Vector2(on?57:-57,0),new Vector2(57,57),BattleUiSkin.Blue);
        }
        public void Contribution(int filter,bool taken,bool rolls,bool skills,Action<int,bool,bool,bool> select,Action close)
        {
            Button(Panel,"Damage Dealt",new Vector2(-215,680),new Vector2(415,95),taken?BattleUiSkin.Navy:BattleUiSkin.Blue,()=>select(filter,false,rolls,skills),34);
            Button(Panel,"Damage Taken",new Vector2(215,680),new Vector2(415,95),taken?BattleUiSkin.Blue:BattleUiSkin.Navy,()=>select(filter,true,rolls,skills),34);
            string[] filters={"All","Legendary","Mythic"};
            for(int i=0;i<3;i++){int choice=i;Button(Panel,filters[i],new Vector2((i-1)*285,565),new Vector2(270,80),filter==i?BattleUiSkin.Blue:BattleUiSkin.Navy,()=>select(choice,taken,rolls,skills),30);}
            var viewport=Box(Panel,"Viewport",new Vector2(0,-55),new Vector2(860,1120));viewport.gameObject.AddComponent<RectMask2D>();
            var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.viewport=viewport;scroll.horizontal=false;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=45;
            var content=Rect(viewport,"Content",Vector2.zero,new Vector2(820,2000));content.anchorMin=content.anchorMax=content.pivot=new Vector2(.5f,1);scroll.content=content;
            float y=20;
            var rows=BattleContribution.Rows.Where(e=>e.Definition&&(filter==0||e.Definition.Rank==(filter==1?Rank.Legendary:Rank.Mythic))).ToArray();
            Ranking(content,taken?"Total damage taken":"Total damage dealt",rows,taken,ref y);
            Button(content,(rolls?"−":"+")+"  Roll Summary",new Vector2(0,-y-45),new Vector2(790,90),BattleUiSkin.Navy,()=>select(filter,taken,!rolls,skills),32);y+=105;
            if(rolls){Pair(content,"Total Rolls",BattleContribution.Rolls,"Recruited",BattleContribution.Recruited,ref y);Pair(content,"Common",BattleContribution.Rarity[0],"Rare",BattleContribution.Rarity[1],ref y);Pair(content,"Epic",BattleContribution.Rarity[2],"Legendary",BattleContribution.Rarity[3],ref y);}
            Button(content,(skills?"−":"+")+"  Allies & Skills",new Vector2(0,-y-45),new Vector2(790,90),BattleUiSkin.Navy,()=>select(filter,taken,rolls,!skills),32);y+=105;
            if(skills){Pair(content,"Merges",BattleContribution.Merges,"Skill Casts",BattleContribution.SkillCasts,ref y);Pair(content,"Summoned Allies",BattleContribution.Summons,"",0,ref y);}
            foreach(Transform child in content){var r=(RectTransform)child;var pos=r.anchoredPosition;r.anchorMin=r.anchorMax=new Vector2(.5f,1);r.anchoredPosition=pos;}
            content.sizeDelta=new Vector2(820,Mathf.Max(1120,y+20));content.anchoredPosition=Vector2.zero;
            Canvas.ForceUpdateCanvases();scroll.verticalNormalizedPosition=1;
            Label(Panel,"Battle totals · Includes removed allies",new Vector2(0,-665),new Vector2(820,45),25,new Color(.65f,.8f,.88f));
            Button(Panel,"Close",new Vector2(0,-770),new Vector2(810,110),BattleUiSkin.Navy,close,36);
        }
        void Section(Transform p,string text,ref float y)
        {Label(p,text,new Vector2(0,-y-35),new Vector2(780,70),35,new Color(1,.85f,.5f),TextAnchor.MiddleLeft);y+=90;}
        void Ranking(Transform p,string title,BattleContribution.Entry[] entries,bool taken,ref float y)
        {
            // Percentages and bars use the same denominator: the displayed group's total.
            // Each spawned ally keeps its own ledger row, including removed units.
            var sorted=entries.OrderByDescending(e=>taken?e.Taken:e.Dealt).ToArray();
            float total=sorted.Sum(e=>Mathf.Max(0,taken?e.Taken:e.Dealt));
            var header=Box(p,"RankingHeader",new Vector2(0,-y-42),new Vector2(800,84),BattleUiSkin.Navy);
            Label(header,title,new Vector2(-135,0),new Vector2(480,70),34,null,TextAnchor.MiddleLeft);
            Label(header,total.ToString("N0"),new Vector2(270,0),new Vector2(220,70),36,new Color(1,.86f,.43f),TextAnchor.MiddleRight);
            y+=100;
            if(sorted.Length==0 || total<=0)
            {Label(p,"No damage recorded yet",new Vector2(0,-y-40),new Vector2(780,80),30);y+=100;}
            else for(int i=0;i<sorted.Length;i++)
            {
                var entry=sorted[i];var def=entry.Definition;
                float value=Mathf.Max(0,taken?entry.Taken:entry.Dealt);
                float share=total>0?value/total:0;
                var row=Rect(p,"UnitRow",new Vector2(0,-y-70),new Vector2(800,140));
                Label(row,(i+1).ToString(),new Vector2(-365,0),new Vector2(45,60),28,new Color(.68f,.77f,.86f));
                var portrait=Rect(row,"Portrait",new Vector2(-300,0),new Vector2(80,95)).gameObject.AddComponent<Image>();portrait.sprite=def.Portrait;
                if(!portrait.sprite&&def.Animations!=null&&def.Animations.Idle!=null&&def.Animations.Idle.IsValid)portrait.sprite=def.Animations.Idle.FrameAt(0);
                portrait.preserveAspect=true;portrait.raycastTarget=false;portrait.enabled=portrait.sprite;
                var name=Label(row,def.DisplayName,new Vector2(50,34),new Vector2(580,48),30,null,TextAnchor.MiddleLeft);
                name.horizontalOverflow=HorizontalWrapMode.Wrap;name.verticalOverflow=VerticalWrapMode.Truncate;
                Label(row,value.ToString("N0"),new Vector2(90,-15),new Vector2(360,48),34,new Color(1,.86f,.43f),TextAnchor.MiddleLeft);
                Label(row,(share*100).ToString("0.0")+"%",new Vector2(310,-15),new Vector2(140,48),30,new Color(.7f,.84f,.94f),TextAnchor.MiddleRight);
                var track=Rect(row,"Track",new Vector2(65,-48),new Vector2(610,16));track.gameObject.AddComponent<Image>().color=new Color(.18f,.27f,.36f);
                var fill=Rect(track,"Fill",Vector2.zero,Vector2.zero);fill.anchorMin=Vector2.zero;fill.anchorMax=new Vector2(share,1);fill.offsetMin=fill.offsetMax=Vector2.zero;fill.gameObject.AddComponent<Image>().color=taken?new Color(1,.35f,.4f):new Color(.1f,.78f,.95f);
                y+=150;
            }
            y+=35;
        }
        void Pair(Transform p,string a,int va,string b,int vb,ref float y)
        {
            Label(p,a+"\n"+va.ToString("N0"),new Vector2(-205,-y-50),new Vector2(380,105),30,null,TextAnchor.MiddleLeft);
            if(b!="")Label(p,b+"\n"+vb.ToString("N0"),new Vector2(205,-y-50),new Vector2(380,105),30,null,TextAnchor.MiddleLeft);
            y+=120;
        }
    }
}
