using Project.Battle;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Project.UI
{
    /// <summary>
    /// Dedicated opaque full-screen Result view on its own canvas.
    /// Existing scene bindings remain only for backward compatibility.
    /// </summary>
    public sealed class ResultPanel : MonoBehaviour
    {
        [SerializeField] BattleFlow flow;

        [Tooltip("Để đọc số vàng/chìa thưởng khi qua màn. Thiếu thì vẫn " +
                 "mở được màn kế, chỉ là không cộng tài nguyên.")]
        [SerializeField] Data.GameBalance balance;

        [Header("Do BattleHudBuilder gán")]
        [SerializeField] CanvasGroup group;
        [SerializeField] Image dim;
        [SerializeField] Text title;
        [SerializeField] Text detail;
        [SerializeField] Button againButton;
        [SerializeField] Text againLabel;
        [SerializeField] Button homeButton;
        GameObject resultScreen;
        Font resultFont;
        float playSeconds;
        bool presented;

        void Awake()
        {
            if (flow == null) flow = FindFirstObjectByType<BattleFlow>();
            if (againButton != null) againButton.onClick.AddListener(Again);
            if (homeButton != null) homeButton.onClick.AddListener(SceneRoutes.GoHome);
            Show(false);
        }

        void OnEnable()
        {
            if (flow != null) flow.PhaseChanged += OnPhase;
        }

        void OnDisable()
        {
            if (flow != null) flow.PhaseChanged -= OnPhase;
        }

        void Update()
        {if(flow && !presented)playSeconds+=Time.deltaTime;}

        void OnPhase(BattleFlow.Phase phase)
        {
            var win = phase == BattleFlow.Phase.LevelCleared;
            var lose = phase == BattleFlow.Phase.Defeated;
            if (!win && !lose) { Show(false); return; }
            if(presented)return;
            presented=true;

            // Settlement already happened in BattleFlow before this event; only display it.
            var reward=win&&flow&&flow.Settlement.HasValue?flow.Settlement.Value:default;
            BuildResult(win,reward.Gold,reward.Keys,reward.Gems);
        }

        RectTransform Rect(string name,Transform parent,Vector2 anchor,Vector2 pos,Vector2 size)
        {
            var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);
            BattleUiSkin.Place(r,anchor,pos,size);return r;
        }

        Text Label(Transform parent,string value,Vector2 anchor,Vector2 pos,Vector2 size,int fontSize,Color color)
        {
            var r=Rect(value,parent,anchor,pos,size);var text=r.gameObject.AddComponent<Text>();text.font=resultFont;text.text=value;text.fontSize=fontSize;text.fontStyle=FontStyle.Bold;text.color=color;text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;
            var ink=r.gameObject.AddComponent<Outline>();ink.effectColor=new Color(0,0,0,.8f);ink.effectDistance=new Vector2(2,-2);return text;
        }

        Button Action(Transform parent,string label,string glyph,float top,bool secondary,UnityEngine.Events.UnityAction action)
        {
            var size=new Vector2(1000,secondary?135:175);
            var r=Rect(label,parent,new Vector2(.5f,1),new Vector2(0,-top),size);
            var image=r.gameObject.AddComponent<Image>();
            var button=r.gameObject.AddComponent<Button>();button.targetGraphic=image;ButtonFamilySkin.States(button);
            // glyph la ten sprite trong Resources/Glyphs. Truoc day day la ky tu Unicode
            // ("↻", "⌂") do font he thong ve — lac han khoi phan con lai cua game.
            ScreenChrome.Action(button,secondary?ScreenChrome.Role.Cancel:ScreenChrome.Role.Primary,glyph);
            button.onClick.AddListener(action);
            Label(r,label,new Vector2(.5f,.5f),new Vector2(70,0),new Vector2(720,130),secondary?48:60,Color.white);
            return button;
        }

        void BuildResult(bool win,int gold,int keys,int gems)
        {
            // Presentation only. Reward settlement lives in BattleFlow.Settlement, never in this panel.
            Show(false);
            if(resultScreen)Destroy(resultScreen);
            resultFont=UiFonts.Display;
            resultScreen=new GameObject("ResultScreen",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
            var canvas=resultScreen.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=500;
            var scaler=resultScreen.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1125,2436);scaler.matchWidthOrHeight=0;
            var bg=Rect("Background",resultScreen.transform,Vector2.zero,Vector2.zero,Vector2.zero);bg.anchorMax=Vector2.one;bg.offsetMin=bg.offsetMax=Vector2.zero;bg.gameObject.AddComponent<Image>().color=new Color(.075f,.09f,.17f);
            var safe=Rect("SafeArea",resultScreen.transform,Vector2.zero,Vector2.zero,Vector2.zero);var area=Screen.safeArea;safe.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);safe.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);safe.offsetMin=safe.offsetMax=Vector2.zero;
            var content=Rect("ResultContent",safe,new Vector2(.5f,.5f),Vector2.zero,new Vector2(1060,2230));
            for(int i=0;i<16;i++)
            {
                var mark=Rect("BackgroundDiamond",bg,new Vector2(.5f,.5f),new Vector2((i%3-1)*410+(i%2)*90,(i/3-2)*420),Vector2.one*(i%2==0?110:65));
                mark.localRotation=Quaternion.Euler(0,0,45);var image=mark.gameObject.AddComponent<Image>();image.color=new Color(.6f,.5f,.9f,.045f);image.raycastTarget=false;
            }
            rewardCards.Clear();
            var ribbon=Art(content,"ResultBanner",win?0:1,25,new Vector2(1040,315));
            Label(ribbon,win?"VICTORY!":"DEFEAT",new Vector2(.5f,.5f),new Vector2(0,25),new Vector2(900,180),110,Color.white);
            Label(content,$"Chapter 1-{GameProgress.CurrentLevel}",new Vector2(.5f,1),new Vector2(0,-365),new Vector2(1000,80),48,Color.white);
            Art(content,"ResultIllustration",win?2:3,460,new Vector2(990,600));
            var stats=Rect("BattleSummary",content,new Vector2(.5f,1),new Vector2(0,-1090),new Vector2(1000,220));BattleUiSkin.Skin(stats.gameObject.AddComponent<Image>(),BattleUiSkin.Navy);
            int total=flow?Mathf.Max(1,flow.TotalWaves):Mathf.Max(1,balance?balance.WavesForLevel(GameProgress.CurrentLevel):3);
            int cleared=win?total:Mathf.Clamp(flow?flow.Wave-1:0,0,total);
            Label(stats,"Waves Cleared",new Vector2(.5f,.5f),new Vector2(-250,55),new Vector2(460,70),36,new Color(.72f,.77f,.86f));
            Label(stats,$"{cleared} / {total}",new Vector2(.5f,.5f),new Vector2(-250,-35),new Vector2(460,110),76,Color.white);
            int seconds=Mathf.Max(0,Mathf.FloorToInt(playSeconds));
            Label(stats,"Time",new Vector2(.5f,.5f),new Vector2(250,55),new Vector2(460,70),36,new Color(.72f,.77f,.86f));
            Label(stats,$"{seconds/60:00}:{seconds%60:00}",new Vector2(.5f,.5f),new Vector2(250,-35),new Vector2(460,110),76,Color.white);
            var heading=Label(content,"REWARDS",new Vector2(.5f,1),new Vector2(0,-1350),new Vector2(990,75),42,new Color(.72f,.77f,.86f));heading.alignment=TextAnchor.MiddleLeft;
            if(win)
            {
                Reward(content,"Gold",gold,-340,"gold");Reward(content,"Keys",keys,0,"key");Reward(content,"Gems",gems,340,"gem");
            }
            else
            {
                var empty=Rect("NoRewards",content,new Vector2(.5f,1),new Vector2(0,-1450),new Vector2(1000,350));BattleUiSkin.Skin(empty.gameObject.AddComponent<Image>(),BattleUiSkin.Navy);
                Label(empty,"No rewards this time",new Vector2(.5f,.5f),Vector2.zero,new Vector2(900,100),42,new Color(.72f,.77f,.86f));
            }
            int level=GameProgress.CurrentLevel;
            bool hasNext=win&&level<(balance?balance.LevelCount:3);
            Action(content,hasNext?"NEXT LEVEL":"RETRY",hasNext?"next":"replay",1860,false,()=>{Time.timeScale=1;SceneRoutes.PlayLevel(hasNext?level+1:level);});
            Action(content,"HOME","home",2070,true,()=>{Time.timeScale=1;SceneRoutes.GoHome();});
            Canvas.ForceUpdateCanvases();
            float fit=Mathf.Min(1,Mathf.Min((safe.rect.width-40)/1060,(safe.rect.height-50)/2230));
            StartCoroutine(Reveal(content,Mathf.Max(.1f,fit)));
            StartCoroutine(Celebrate(ribbon,win));
        }

        RectTransform Art(Transform parent,string name,int index,float top,Vector2 size)
        {
            var r=Rect(name,parent,new Vector2(.5f,1),new Vector2(0,-top),size);var image=r.gameObject.AddComponent<Image>();
            image.sprite=ResultScreenArt.Get(index);image.preserveAspect=true;image.raycastTarget=false;return r;
        }

        readonly System.Collections.Generic.List<(RectTransform card,Text value,int amount)> rewardCards=new System.Collections.Generic.List<(RectTransform,Text,int)>();
        void Reward(Transform parent,string name,int amount,float x,string icon)
        {
            var r=Rect(name,parent,new Vector2(.5f,1),new Vector2(x,-1450),new Vector2(320,350));BattleUiSkin.Skin(r.gameObject.AddComponent<Image>(),BattleUiSkin.Navy);
            var art=Rect("Icon",r,new Vector2(.5f,1),new Vector2(0,-25),new Vector2(155,155)).gameObject.AddComponent<Image>();
            art.sprite=Resources.Load<Sprite>(name=="Gems"?"Summon/gem":"HomeChrome/"+(name=="Gold"?"coin":"key"));art.preserveAspect=true;art.raycastTarget=false;art.enabled=art.sprite;
            Label(r,name,new Vector2(.5f,.5f),new Vector2(0,-35),new Vector2(295,60),36,Color.white);
            rewardCards.Add((r,Label(r,"+"+amount.ToString("N0"),new Vector2(.5f,0),new Vector2(0,20),new Vector2(300,100),64,new Color(1,.85f,.25f)),amount));
        }

        IEnumerator Reveal(RectTransform content,float fit)
        {
            var cg=content.gameObject.AddComponent<CanvasGroup>();float t=0;
            while(content&&t<.35f){t+=Time.unscaledDeltaTime;float p=Mathf.Clamp01(t/.35f);cg.alpha=p;content.localScale=Vector3.one*(fit*Mathf.Lerp(.94f,1,1-Mathf.Pow(1-p,3)));yield return null;}
            if(content){cg.alpha=1;content.localScale=Vector3.one*fit;}
        }

        // ANIM-007: Thang — tia vang xoay sau ru-bang, ru-bang bat ra, hat lap lanh; Thua — ru-bang roi
        // xuong nhe, khong tia. Sau do the thuong bat ra lan luot, so chay tu 0. Chi trinh dien.
        IEnumerator Celebrate(RectTransform ribbon,bool win)
        {
            if(!ribbon)yield break;
            RectTransform ray=null;
            var raySprite=Resources.Load<Sprite>("Anim/Common/rarity-ray");
            if(win&&raySprite)
            {
                // Khung trung vi tri ru-bang, chen ngay TRUOC ru-bang de tia nam sau; tia canh giua khung.
                var holder=new GameObject("VictoryRays",typeof(RectTransform)).GetComponent<RectTransform>();holder.SetParent(ribbon.parent,false);
                holder.anchorMin=ribbon.anchorMin;holder.anchorMax=ribbon.anchorMax;holder.pivot=ribbon.pivot;holder.anchoredPosition=ribbon.anchoredPosition;holder.sizeDelta=ribbon.sizeDelta;
                holder.SetSiblingIndex(ribbon.GetSiblingIndex());
                ray=Rect("Rays",holder,new Vector2(.5f,.5f),Vector2.zero,new Vector2(1300,1300));
                var img=ray.gameObject.AddComponent<Image>();img.sprite=raySprite;img.preserveAspect=true;img.raycastTarget=false;img.color=new Color(1,.82f,.3f,0);
                ray.gameObject.AddComponent<UiSpin>().DegreesPerSecond=14;
            }
            foreach(var c in rewardCards)c.card.localScale=Vector3.zero;
            var home=ribbon.anchoredPosition;
            yield return UiTween.Run(win?.45f:.55f,t=>
            {
                if(win)ribbon.localScale=Vector3.one*Mathf.LerpUnclamped(.3f,1,t);
                else ribbon.anchoredPosition=home+new Vector2(0,Mathf.LerpUnclamped(160,0,t));
                if(ray){var i=ray.GetComponent<Image>();var k=i.color;k.a=.55f*Mathf.Clamp01(t);i.color=k;ray.localScale=Vector3.one*Mathf.Lerp(.5f,1,Mathf.Clamp01(t));}
            },UiTween.OutBack);
            if(win)Confetti.Play((RectTransform)ribbon.root.GetComponentInChildren<CanvasScaler>().transform);
            for(int i=0;i<rewardCards.Count;i++)
            {
                var c=rewardCards[i];
                StartCoroutine(UiTween.Run(.25f,t=>{if(c.card)c.card.localScale=Vector3.one*t;},UiTween.OutBack));
                StatCountUp.Play(c.value,0,c.amount,"+","","N0",.1f);
                yield return UiTween.Wait(.15f);
            }
        }

        void OnDestroy(){if(resultScreen)Destroy(resultScreen);}

        void Again()
        {
            Show(false);
            flow?.RestartRun();
        }

        void Show(bool on)
        {
            if (group == null) return;
            group.alpha = on ? 1f : 0f;
            // Chan chuot khi hien: khong chan thi nguoi choi van keo tha duoc
            // linh phia sau tam man ket qua.
            group.blocksRaycasts = on;
            group.interactable = on;
        }
    }
}
