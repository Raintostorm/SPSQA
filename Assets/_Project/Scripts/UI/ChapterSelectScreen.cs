using System;
using Project.Data;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>Chapter selection presentation only. Never grants rewards or alters progress.</summary>
    public sealed partial class ChapterSelectScreen : MonoBehaviour
    {
        GameBalance balance;
        Action<int> confirm;
        Action cancel;
        Font font;
        RectTransform safe, content;
        Button[] rows=new Button[3];
        Text[] status=new Text[3];
        Image[] cleared=new Image[3];
        Text gold,keys,gems;
        RectTransform[] rewardIcons=new RectTransform[3];
        Button enter;
        int selected;
        GameObject notice;
        public static ChapterSelectScreen Build(Transform parent,GameBalance balance,Action<int> confirm,Action cancel)
        {
            var go=new GameObject("ChooseLevelScreen",typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var view=go.AddComponent<ChapterSelectScreen>();view.balance=balance;view.confirm=confirm;view.cancel=cancel;
            view.font=UiFonts.Display;
            var r=go.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
            go.GetComponent<Image>().color=new Color(.035f,.09f,.15f);
            view.safe=view.Rect("SafeArea",r,Vector2.zero,Vector2.zero);
            view.safe.anchorMin=Vector2.zero;view.safe.anchorMax=Vector2.one;view.safe.offsetMin=view.safe.offsetMax=Vector2.zero;
            view.content=view.Rect("Content",view.safe,Vector2.zero,new Vector2(1050,2300));
            view.BuildContent();view.Fit();return view;
        }
        void OnDisable(){if(notice)Destroy(notice);}
        void OnDestroy(){monsterRoster?.Dispose();}
        void LateUpdate(){Fit();}
        void Fit()
        {
            if(!safe || !content)return;
            var area=Screen.safeArea;safe.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);safe.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);
            float scale=Mathf.Min(safe.rect.width/1080f,safe.rect.height/2330f);content.localScale=Vector3.one*Mathf.Max(.01f,scale);
        }
        void BuildContent()
        {
            for(int i=0;i<24;i++)
            {
                var mark=Rect("Pattern",content,new Vector2((i%4-1.5f)*290,(i/4-2.5f)*400),Vector2.one*85);mark.localRotation=Quaternion.Euler(0,0,45);
                var image=mark.gameObject.AddComponent<Image>();image.color=new Color(1,1,1,.025f);image.raycastTarget=false;
            }
            // Shared chrome (UI-005): back top-left replaces the red X, info mirrors it top-right.
            ScreenChrome.Title(content,"SELECT CHAPTER",new Vector2(0,1065),760);
            ScreenChrome.Back(content,new Vector2(-460,1065),()=>cancel());
            var frame=Panel("BannerFrame",content,new Vector2(0,690),new Vector2(1000,560),new Color(.025f,.035f,.06f));
            Art(frame,"ChapterSelect/banner",Vector2.zero,new Vector2(980,540));
            var info=Button(content,"i",new Vector2(460,1065),new Vector2(90,90),()=>ShowNotice("Chapter 1","Choose a stage, then Confirm to return Home.\nBattle starts the selected stage."));
            BattleUiSkin.Skin(info.GetComponent<Image>(),BattleUiSkin.Info);
            var badge=Panel("ChapterBadge",content,new Vector2(0,340),new Vector2(280,75),Color.white);BattleUiSkin.Skin(badge.GetComponent<Image>(),BattleUiSkin.Green);
            Label(badge,"CHAPTER 1",Vector2.zero,new Vector2(260,70),38);
            Label(content,"Verdant Frontier",new Vector2(0,240),new Vector2(1000,110),68);
            for(int i=0;i<3;i++)
            {
                int level=i+1;
                rows[i]=Button(content,"",new Vector2(0,75-i*190),new Vector2(980,175),()=>Select(level));
                var label=rows[i].GetComponentInChildren<Text>();label.gameObject.SetActive(false);
                var left=Label(rows[i].transform,$"1-{level}",new Vector2(-300,30),new Vector2(300,80),62);left.alignment=TextAnchor.MiddleLeft;
                var waves=Label(rows[i].transform,$"{Waves(level)} Waves",new Vector2(-300,-43),new Vector2(300,65),38);waves.alignment=TextAnchor.MiddleLeft;
                status[i]=Label(rows[i].transform,"",new Vector2(300,0),new Vector2(300,90),34);
                // Dau tick nam ben trai chu trang thai, khong de len chu.
                var tick=Rect("Cleared",rows[i].transform,new Vector2(130,0),new Vector2(64,64));
                cleared[i]=tick.gameObject.AddComponent<Image>();
                cleared[i].sprite=Resources.Load<Sprite>("Badges/check");
                cleared[i].preserveAspect=true;cleared[i].raycastTarget=false;cleared[i].enabled=false;
            }
            var rewards=Panel("Rewards",content,new Vector2(0,-560),new Vector2(980,240),Color.white);BattleUiSkin.Skin(rewards.GetComponent<Image>(),BattleUiSkin.Navy);
            ScreenChrome.Title(rewards,"REWARDS",new Vector2(0,100),380,ScreenChrome.Banner.Section);
            var icons=new[]{"HomeChrome/coin","HomeChrome/key","Summon/gem"};var values=new Text[3];
            for(int i=0;i<3;i++)
            {
                rewardIcons[i]=Rect("Reward",rewards,new Vector2(0,-25),new Vector2(300,100));
                Art(rewardIcons[i],icons[i],new Vector2(-80,0),new Vector2(85,85));
                values[i]=Label(rewardIcons[i],"",new Vector2(50,0),new Vector2(180,100),48);values[i].alignment=TextAnchor.MiddleLeft;
            }
            gold=values[0];keys=values[1];gems=values[2];
            var names=new[]{"rewards","codex"};
            for(int i=0;i<2;i++)
            {
                int tab=i;
                var button=Button(content,"",new Vector2((i-.5f)*490,-845),new Vector2(450,230),()=>Shortcut(tab));
                button.GetComponentInChildren<Text>().gameObject.SetActive(false);
                Art(button.transform,"ChapterSelect/"+names[i],new Vector2(0,25),new Vector2(170,150));
                Label(button.transform,i==0?"Rewards":"Monster Codex",new Vector2(0,-80),new Vector2(430,60),36);

            }
            enter=Button(content,"CONFIRM",new Vector2(0,-1060),new Vector2(570,150),()=>{if(selected<=GameProgress.Unlocked)confirm(selected);});
            ScreenChrome.Action(enter,ScreenChrome.Role.Primary);
        }
        int Waves(int level)=>balance?balance.WavesForLevel(level):level==2?10:3;
        public void Refresh(int level){selected=Mathf.Clamp(level,1,Mathf.Min(3,GameProgress.Unlocked));Draw();}
        void Select(int level){if(level>GameProgress.Unlocked)return;selected=level;Draw();}
        void Draw()
        {
            for(int i=0;i<3;i++)
            {
                bool open=i+1<=GameProgress.Unlocked,chosen=i+1==selected;
                rows[i].interactable=open;
                BattleUiSkin.Skin(rows[i].GetComponent<Image>(),chosen?BattleUiSkin.Blue:BattleUiSkin.Navy);
                rows[i].GetComponent<Image>().color=chosen?Color.white:new Color(.8f,.77f,.95f);
                bool done=SaveSystem.Current.HighestLevelUnlocked>i+1;
                status[i].text=!open?"Locked":chosen?"Selected":done?"Completed":"";
                // Hien ca khi dang chon: man da qua van la da qua, khong vi chon ma mat dau.
                if(cleared[i])cleared[i].enabled=open&&done;
            }
            bool first=SaveSystem.Current.HighestLevelUnlocked<=selected;
            gold.text=(balance?balance.GoldForLevel(selected,first):0).ToString("N0");
            keys.text=(balance?balance.KeysForLevel(selected,first):0).ToString("N0");
            gems.text=GameProgress.GemsForLevel(selected).ToString("N0");
            // Zero rewards hide; the rest stay centred (same rule as the Rewards page).
            var paid=new System.Collections.Generic.List<RectTransform>();
            for(int i=0;i<3;i++){bool any=new[]{gold,keys,gems}[i].text!="0";rewardIcons[i].gameObject.SetActive(any);if(any)paid.Add(rewardIcons[i]);}
            for(int i=0;i<paid.Count;i++)paid[i].anchoredPosition=new Vector2((i-(paid.Count-1)*.5f)*310,-25);
        }
        void Shortcut(int tab)
        {
            if(tab==0)ShowRewards();else ShowCodex();
        }
        void ShowNotice(string title,string message)
        {
            if(notice)Destroy(notice);
            var shade=Rect("Notice",content,Vector2.zero,new Vector2(1050,2300));notice=shade.gameObject;shade.gameObject.AddComponent<Image>().color=new Color(0,0,0,.85f);
            Panel("Panel",shade,Vector2.zero,new Vector2(960,660),new Color(.13f,.14f,.23f));
            Label(shade,title,new Vector2(0,230),new Vector2(900,100),54);
            Label(shade,message,Vector2.zero,new Vector2(900,340),38);
            ScreenChrome.Action(Button(shade,"Close",new Vector2(0,-230),new Vector2(400,100),()=>Destroy(notice)),ScreenChrome.Role.Cancel);
        }
        RectTransform Rect(string name,Transform parent,Vector2 pos,Vector2 size)
        {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);BattleUiSkin.Place(r,Vector2.one*.5f,pos,size);return r;}
        RectTransform Panel(string name,Transform parent,Vector2 pos,Vector2 size,Color color)
        {var r=Rect(name,parent,pos,size);r.gameObject.AddComponent<Image>().color=color;return r;}
        void Art(Transform parent,string path,Vector2 pos,Vector2 size)
        {var im=Rect(path,parent,pos,size).gameObject.AddComponent<Image>();im.sprite=Resources.Load<Sprite>(path);im.enabled=im.sprite!=null;im.preserveAspect=true;im.raycastTarget=false;}
        Text Label(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize)
        {var t=Rect("Label",parent,pos,size).gameObject.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=fontSize;t.fontStyle=FontStyle.Bold;t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;var outline=t.gameObject.AddComponent<Outline>();outline.effectColor=new Color(0,0,0,.85f);outline.effectDistance=new Vector2(2,-2);return t;}
        Button Button(Transform parent,string value,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction action)
        {var r=Panel(value,parent,pos,size,Color.white);BattleUiSkin.Skin(r.GetComponent<Image>(),BattleUiSkin.Dark);var b=r.gameObject.AddComponent<Button>();b.targetGraphic=r.GetComponent<Image>();b.onClick.AddListener(action);ButtonFamilySkin.States(b);b.gameObject.AddComponent<ScreenChrome.Press>();Label(r,value,Vector2.zero,size-Vector2.one*12,48);return b;}
    }
}
