using UnityEngine;
using UnityEngine.UI;
using Project.Meta;

namespace Project.UI
{
    public sealed class HomeChrome : MonoBehaviour
    {
        Font font;
        GameObject notice;
        Text gemLabel;
        int shownGems=-1;
        Image alliesDot,relicsDot;
        float dotTimer;
        void Update()
        {
            if(gemLabel && shownGems!=GameProgress.Gems){shownGems=GameProgress.Gems;AnimatedCounter.Wallet(gemLabel,shownGems,"gems");}
            // Quet lai dinh ky chu khong chi luc dung: nguoi choi nang cap roi quay ve Home,
            // cham do phai tat ngay, khong doi mo lai man.
            dotTimer-=Time.unscaledDeltaTime;if(dotTimer>0)return;dotTimer=.5f;
            if(alliesDot)alliesDot.enabled=HomeSignals.AlliesReady();
            if(relicsDot)relicsDot.enabled=HomeSignals.RelicsReady();
        }

        /// <summary>Cham do goc tren phai mot nut dock.</summary>
        Image Dot(Transform parent)
        {
            var r=Rect("Signal",parent,Vector2.one*.5f,new Vector2(72,62),new Vector2(42,42));
            var image=r.gameObject.AddComponent<Image>();
            image.sprite=Resources.Load<Sprite>("Badges/dot");
            image.preserveAspect=true;image.raycastTarget=false;image.enabled=false;
            return image;
        }
        public static HomeChrome Build(Transform parent,Text gold,Text keys,Button mercenaries,Button relics)
        {
            var go=new GameObject("HomeChrome",typeof(RectTransform));go.transform.SetParent(parent,false);
            var self=go.AddComponent<HomeChrome>();self.font=UiFonts.Display;
            var r=go.GetComponent<RectTransform>();var safe=Screen.safeArea;
            r.anchorMin=new Vector2(safe.xMin/Screen.width,safe.yMin/Screen.height);r.anchorMax=new Vector2(safe.xMax/Screen.width,safe.yMax/Screen.height);r.offsetMin=r.offsetMax=Vector2.zero;
            self.Wallet(gold,"coin",new Vector2(-710,-160),new Vector2(290,95));
            self.Wallet(keys,"key",new Vector2(-380,-160),new Vector2(250,95));
            self.gemLabel=new GameObject("GemBalance",typeof(RectTransform),typeof(Text)).GetComponent<Text>();self.gemLabel.transform.SetParent(go.transform,false);self.gemLabel.font=self.font;
            self.Wallet(self.gemLabel,"gem",new Vector2(-40,-160),new Vector2(270,95));
            var dock=self.Rect("Navigation",r,new Vector2(.5f,0),Vector2.zero,new Vector2(1125,200));
            dock.gameObject.AddComponent<Image>().color=new Color(.18f,.14f,.28f);
            for(int i=0;i<5;i++)
            {
                float x=(i-2)*220;
                Button button=i==2?mercenaries:i==3?relics:null;
                if(!button){var b=self.Rect("Nav"+i,dock,Vector2.one*.5f,new Vector2(x,0),new Vector2(210,180));b.gameObject.AddComponent<Image>().color=Color.clear;button=b.gameObject.AddComponent<Button>();}
                else {button.transform.SetParent(dock,false);BattleUiSkin.Place(button.GetComponent<RectTransform>(),Vector2.one*.5f,new Vector2(x,0),new Vector2(210,180));}
                var bg=button.GetComponent<Image>();bg.sprite=null;bg.color=Color.clear;button.transition=Selectable.Transition.ColorTint;
                foreach(Transform child in button.transform)child.gameObject.SetActive(false);
                if(i<4)self.Art(button.transform,new[]{"shop","orb","mercenaries","relics"}[i],Vector2.zero,new Vector2(170,160));
                if(i==2)self.alliesDot=self.Dot(button.transform);
                if(i==3)self.relicsDot=self.Dot(button.transform);
                if(i==4){var menu=Resources.Load<Sprite>("Glyphs/menu");if(menu){var g=new GameObject("Glyph",typeof(RectTransform),typeof(Image)).GetComponent<Image>();g.transform.SetParent(button.transform,false);g.sprite=menu;g.preserveAspect=true;g.raycastTarget=false;g.rectTransform.sizeDelta=new Vector2(110,110);}else self.Label(button.transform,"☰",Vector2.zero,new Vector2(130,130),85);}
                if(i==0)button.onClick.AddListener(()=>GemEconomyScreen.Show(parent,true));
                if(i==1)button.onClick.AddListener(()=>GemEconomyScreen.Show(parent,false));
                if(i==4)button.onClick.AddListener(SharedSettings.OpenHomeMenu);
                if(i<4){var line=self.Rect("Divider",dock,Vector2.one*.5f,new Vector2(x+110,0),new Vector2(3,115));var separator=line.gameObject.AddComponent<Image>();separator.color=new Color(.1f,.08f,.18f);separator.raycastTarget=false;}
            }
            return self;
        }
        void Wallet(Text value,string icon,Vector2 position,Vector2 size)
        {
            if(!value)return;
            var old=value.transform.parent;
            var wallet=Rect("Wallet",transform,Vector2.one,position,size);
            var bg=wallet.gameObject.AddComponent<Image>();BattleUiSkin.Skin(bg,BattleUiSkin.Navy);
            value.transform.SetParent(wallet,false);BattleUiSkin.Place(value.rectTransform,Vector2.one*.5f,new Vector2(30,0),new Vector2(size.x-80,80));
            value.fontSize=48;value.resizeTextForBestFit=true;value.resizeTextMinSize=26;value.resizeTextMaxSize=48;value.fontStyle=FontStyle.Bold;value.alignment=TextAnchor.MiddleCenter;value.color=Color.white;
            if(old && old!=transform)old.gameObject.SetActive(false);
            Art(wallet,icon,new Vector2(-size.x*.5f+15,0),new Vector2(135,135));
        }
        RectTransform Rect(string name,Transform parent,Vector2 anchor,Vector2 pos,Vector2 size)
        {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);BattleUiSkin.Place(r,anchor,pos,size);return r;}
        void Art(Transform parent,string icon,Vector2 pos,Vector2 size)
        {var im=Rect(icon,parent,Vector2.one*.5f,pos,size).gameObject.AddComponent<Image>();im.sprite=Resources.Load<Sprite>((icon=="orb"||icon=="gem"?"Summon/":"HomeChrome/")+icon);im.preserveAspect=true;im.raycastTarget=false;}
        void Label(Transform parent,string text,Vector2 pos,Vector2 size,int fontSize)
        {var label=Rect("Label",parent,Vector2.one*.5f,pos,size).gameObject.AddComponent<Text>();label.font=font;label.text=text;label.fontSize=fontSize;label.fontStyle=FontStyle.Bold;label.alignment=TextAnchor.MiddleCenter;label.color=Color.white;label.raycastTarget=false;}
        void ShowNotice(string heading,string message)
        {
            if(notice)return;
            var shade=Rect("Notice",transform,Vector2.one*.5f,Vector2.zero,Vector2.zero);shade.anchorMin=Vector2.zero;shade.anchorMax=Vector2.one;shade.offsetMin=shade.offsetMax=Vector2.zero;notice=shade.gameObject;
            shade.gameObject.AddComponent<Image>().color=new Color(0,0,0,.7f);shade.gameObject.AddComponent<Button>().onClick.AddListener(()=>Destroy(notice));
            var panel=Rect("Panel",shade,Vector2.one*.5f,Vector2.zero,new Vector2(850,360));panel.gameObject.AddComponent<Image>().color=new Color(.12f,.15f,.23f);
            Label(panel,heading,new Vector2(0,100),new Vector2(800,90),56);Label(panel,message,Vector2.zero,new Vector2(800,90),36);Label(panel,"Tap to close",new Vector2(0,-110),new Vector2(800,70),30);
        }
    }
}
