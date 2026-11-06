using Project.Meta;
using Project.Battle;
using Project.Data;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Project.UI
{
    // Runtime overlay: does not rebuild or overwrite the user's scenes.
    public sealed class SharedSettings : MonoBehaviour
    {
        const string SoundKey = "ui.sound", VibrationKey = "ui.vibration";
        RectTransform root;
        GameObject modal;
        Font font;
        Sprite buttonSprite;
        AudioSource audioSource;
        AudioClip click;
        float previousSpeed = 1;
        bool paused, leaving;
        RectTransform menu, menuButton;
        bool closing;
        public static bool IsPaused { get; private set; }
        public static bool DamageNumbersEnabled=>PlayerPrefs.GetInt("ui.damage",1)!=0;
        public static bool EffectsEnabled=>PlayerPrefs.GetInt("ui.effects",1)!=0;
        bool Battle => SceneManager.GetActiveScene().name == SceneRoutes.Battle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            IsPaused = false;
            SceneManager.sceneLoaded -= Install;
            SceneManager.sceneLoaded += Install;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InstallInitialScene() => Install(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        static void Install(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single || Object.FindFirstObjectByType<SharedSettings>() != null) return;
            if (scene.name != SceneRoutes.Home && scene.name != SceneRoutes.Battle &&
                scene.name != SceneRoutes.Upgrade && scene.name != SceneRoutes.Treasure) return;
            new GameObject("SharedSettings").AddComponent<SharedSettings>();
        }

        void Start()
        {
            font = UiFonts.Display;
            buttonSprite = Resources.Load<Sprite>("SharedUI/btn_blue");
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 300;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1125, 2436);
            scaler.matchWidthOrHeight = 0;
            gameObject.AddComponent<GraphicRaycaster>();
            root = Rect("SafeArea", transform, Vector2.zero, Vector2.zero);
            var safe = Screen.safeArea;
            root.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            root.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            root.offsetMin = root.offsetMax = Vector2.zero;
            var open = Button(root, "Menu", new Vector2(0, -245), ToggleMenu);
            menuButton=open;
            open.anchorMin = open.anchorMax = new Vector2(1, 1);
            open.anchoredPosition = new Vector2(-85, -470);
            open.sizeDelta = new Vector2(140, 140);
            foreach (Transform child in open) Destroy(child.gameObject);
            // Nut tron chung (chat 012): mat nut navy + ky hieu ba vach roi.
            var openImage = open.GetComponent<Image>();
            openImage.type = Image.Type.Simple; openImage.preserveAspect = true; openImage.color = Color.white;
            var openPlate = Resources.Load<Sprite>("SmallButtons/navy");
            if (openPlate)
            {
                openImage.sprite = openPlate;
                var openButton = open.GetComponent<Button>();
                if (openButton)
                {
                    openButton.transition = Selectable.Transition.SpriteSwap;
                    openButton.spriteState = new SpriteState { pressedSprite = Resources.Load<Sprite>("SmallButtons/navy-pressed") };
                }
                var openMark = new GameObject("Glyph", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
                openMark.SetParent(open, false);
                openMark.anchorMin = openMark.anchorMax = openMark.pivot = Vector2.one * .5f;
                openMark.anchoredPosition = Vector2.zero; openMark.sizeDelta = Vector2.one * 73;
                var openMarkImage = openMark.GetComponent<Image>();
                openMarkImage.sprite = Resources.Load<Sprite>("Glyphs/menu");
                openMarkImage.preserveAspect = true; openMarkImage.raycastTarget = false;
                openMarkImage.enabled = openMarkImage.sprite;
                if (!open.GetComponent<ScreenChrome.Press>()) open.gameObject.AddComponent<ScreenChrome.Press>();
            }
            else { BattleUiSkin.Skin(openImage,BattleUiSkin.Dark); ApprovedUiArt.Apply(openImage,"menu"); }
            // Home has its dock menu; Relics uses Back and should not expose this overlay.
            if(SceneManager.GetActiveScene().name==SceneRoutes.Home ||
                SceneManager.GetActiveScene().name==SceneRoutes.Treasure)open.gameObject.SetActive(false);
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.ignoreListenerPause = true;
            click = AudioClip.Create("UI click", 1764, 1, 44100, false);
            var samples = new float[1764];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = Mathf.Sin(i * 2 * Mathf.PI * 880 / 44100) * .07f * (1f - (float)i / samples.Length);
            click.SetData(samples, 0);
        }

        RectTransform Rect(string name, Transform parent, Vector2 size, Vector2 position)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.sizeDelta = size; rect.anchoredPosition = position;
            return rect;
        }

        Text Label(Transform parent, string value, Vector2 position, int size = 40)
        {
            var rect = Rect(value, parent, new Vector2(850, 90), position);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = font; text.text = value; text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter; text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        RectTransform Button(Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction action)
        {
            var rect = Rect(label, parent, new Vector2(760, 100), position);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = buttonSprite; image.type = Image.Type.Sliced;
            image.color = Color.white;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors; colors.pressedColor = new Color(.7f, .7f, .7f); button.colors = colors;
            button.onClick.AddListener(() => { Feedback(); action(); });
            var text = Label(rect, label, Vector2.zero, 36);
            text.rectTransform.sizeDelta = new Vector2(700, 90);
            string icon = label.StartsWith("Sound Effects") ? "sound" :
                label.StartsWith("Vibration") ? "vibration" :
                label == "Resume" ? "resume" : label == "Restart" ? "restart" :
                label == "Home" || label == "Leave" ? "home" : label == "Close" || label == "Cancel" ? "close" : null;
            if (icon != null)
            {
                var art = Rect("Pictogram", rect, new Vector2(110, 110), new Vector2(-300, 0));
                var pictogram = art.gameObject.AddComponent<Image>();
                // Reuse the approved control glyphs; keep SharedUI/home (tent)
                // for Select Chapter, which has a different meaning from Go Home.
                string resource = icon == "restart" ? "Glyphs/replay" :
                    icon == "home" ? "Glyphs/home" :
                    icon == "close" ? "Glyphs/close" : "SharedUI/" + icon;
                pictogram.sprite = Resources.Load<Sprite>(resource);
                pictogram.preserveAspect = true; pictogram.raycastTarget = false;
                text.rectTransform.anchoredPosition = new Vector2(60, 0);
                text.rectTransform.sizeDelta = new Vector2(550, 90);
            }
            if (label == "Resume") image.sprite = Resources.Load<Sprite>("SharedUI/btn_primary");
            return rect;
        }

        void Feedback()
        {
            if (PlayerPrefs.GetInt(SoundKey, 1) != 0 && audioSource != null) audioSource.PlayOneShot(click);
            if (PlayerPrefs.GetInt(VibrationKey, 0) != 0 && Application.isMobilePlatform) Handheld.Vibrate();
        }

        void Open()
        {
            if (modal != null || leaving) return;
            if (Battle) { previousSpeed = Time.timeScale; Time.timeScale = 0; paused = true; IsPaused = true; }
            BuildSettings();
        }

        void Pause()
        {if(Battle && !paused){previousSpeed=Time.timeScale;Time.timeScale=0;paused=true;IsPaused=true;}}

        public static void OpenHomeMenu()
        {
            var settings=FindFirstObjectByType<SharedSettings>();
            if(settings && settings.root)settings.ToggleMenu();
        }

        void ToggleMenu()
        {
            if(closing||leaving)return;
            if(modal){Close();return;}
            Pause();
            var shade=Rect("MenuOverlay",root,Vector2.zero,Vector2.zero);shade.anchorMin=Vector2.zero;shade.anchorMax=Vector2.one;shade.offsetMin=shade.offsetMax=Vector2.zero;
            modal=shade.gameObject;
            var blocker=shade.gameObject.AddComponent<Image>();blocker.color=new Color(0,0,0,.12f);
            shade.gameObject.AddComponent<Button>().onClick.AddListener(Close);
            menu=Rect("Dropdown",shade,new Vector2(440,Battle?340:220),Vector2.zero);menu.anchorMin=menu.anchorMax=menu.pivot=Vector2.one;menu.anchoredPosition=new Vector2(-30,-445);
            if(SceneManager.GetActiveScene().name==SceneRoutes.Home)
            {menu.anchorMin=menu.anchorMax=menu.pivot=new Vector2(1,0);menu.anchoredPosition=new Vector2(-30,210);}
            BattleUiSkin.Skin(menu.gameObject.AddComponent<Image>(),BattleUiSkin.Dark);
            var settings=Button(menu,"Settings",new Vector2(-220,-60),()=>{BuildSettings();});settings.sizeDelta=new Vector2(400,90);BattleUiSkin.Button(settings.GetComponent<Button>(),BattleUiSkin.Purple,"Settings");ScreenChrome.Action(settings.GetComponent<Button>(),ScreenChrome.Role.Secondary);BattleUiSkin.Icon(settings.GetComponent<Button>(),"settings");
            if(Battle){var stats=Button(menu,"Contribution",new Vector2(-220,-165),BuildContribution);stats.sizeDelta=new Vector2(400,90);BattleUiSkin.Button(stats.GetComponent<Button>(),BattleUiSkin.Purple,"Contribution");ScreenChrome.Action(stats.GetComponent<Button>(),ScreenChrome.Role.Secondary);BattleUiSkin.Icon(stats.GetComponent<Button>(),"ButtonIcons/contribution");}
            var leave=Button(menu,Battle?"Leave":"Close",new Vector2(-220,Battle?-270:-165),()=>{if(Battle)Confirm(true);else Close();});leave.sizeDelta=new Vector2(400,90);BattleUiSkin.Button(leave.GetComponent<Button>(),BattleUiSkin.Orange,Battle?"Leave":"Close");ScreenChrome.Action(leave.GetComponent<Button>(),Battle?ScreenChrome.Role.Danger:ScreenChrome.Role.Cancel);BattleUiSkin.Icon(leave.GetComponent<Button>(),Battle?"Glyphs/home":"Glyphs/close");
            // Children use the top-right pivot of the unfolding menu.
            foreach(Transform child in menu){var r=(RectTransform)child;r.anchorMin=r.anchorMax=new Vector2(1,1);}
            StartCoroutine(Pop(menu));
        }

        IEnumerator Pop(RectTransform panel)
        {
            if(!panel)yield break;
            var cg=EnsureCanvasGroup(panel.gameObject);cg.interactable=false;
            float elapsed=0;while(panel && elapsed<.22f){elapsed+=Time.unscaledDeltaTime;float t=Mathf.Clamp01(elapsed/.22f);float ease=1-Mathf.Pow(1-t,3);panel.localScale=Vector3.one*Mathf.Lerp(.82f,1,ease);cg.alpha=t;yield return null;}
            if(panel){panel.localScale=Vector3.one;cg.alpha=1;cg.interactable=true;}
        }

        RectTransform Panel(string title)
        {
            if (modal != null) { modal.SetActive(false); Destroy(modal); }
            var shade = Rect("Modal", root, Vector2.zero, Vector2.zero);
            shade.anchorMin = Vector2.zero; shade.anchorMax = Vector2.one;
            shade.offsetMin = shade.offsetMax = Vector2.zero;
            modal = shade.gameObject;
            shade.gameObject.AddComponent<Image>().color = new Color(0, 0, 0, .72f);
            var panel = Rect("Panel", shade, new Vector2(940, 1050), Vector2.zero);
            BattleUiSkin.Skin(panel.gameObject.AddComponent<Image>(),BattleUiSkin.Dark);
            Label(panel, title, new Vector2(0, 425), 56);
            var close=Button(panel,"Close",new Vector2(385,425),Close);close.sizeDelta=new Vector2(95,95);BattleUiSkin.Icon(close.GetComponent<Button>(),"Glyphs/close",true);
            StartCoroutine(Pop(panel));
            return panel;
        }

        void BuildSettings()
        {
            var view=NewModal("Settings",new Vector2(940,1100),Close);
            view.Settings(Battle,Close,BuildSettings,Feedback);
        }

        BattleModalView NewModal(string title,Vector2 size,System.Action close)
        {
            if(modal){modal.SetActive(false);Destroy(modal);}
            var shade=Rect("BattleModal",root,Vector2.zero,Vector2.zero);shade.anchorMin=Vector2.zero;shade.anchorMax=Vector2.one;shade.offsetMin=shade.offsetMax=Vector2.zero;modal=shade.gameObject;
            shade.gameObject.AddComponent<Image>().color=new Color(.025f,.015f,.065f,.76f);
            var fit=Rect("FittedContent",shade,new Vector2(1125,2436),Vector2.zero);
            Canvas.ForceUpdateCanvases();
            fit.localScale=Vector3.one*Mathf.Min(1,root.rect.width/1020f,root.rect.height/(size.y+100));
            var view=new BattleModalView(fit,font,title,size,close);StartCoroutine(Pop(view.Panel));return view;
        }

        void IconToggle(Transform parent,string title,string icon,string key,int fallback,Vector2 pos)
        {
            bool on=PlayerPrefs.GetInt(key,fallback)!=0;
            var r=Button(parent,title,pos,()=>{PlayerPrefs.SetInt(key,on?0:1);PlayerPrefs.Save();BuildSettings();});r.sizeDelta=new Vector2(320,210);
            BattleUiSkin.Button(r.GetComponent<Button>(),on?BattleUiSkin.Purple:BattleUiSkin.Dark,title);BattleUiSkin.Icon(r.GetComponent<Button>(),icon,true);
            var state=Label(parent,title+" · "+(on?"ON":"OFF"),pos+new Vector2(0,-140),32);state.rectTransform.sizeDelta=new Vector2(360,60);
            if(key==VibrationKey&&!Application.isMobilePlatform)r.GetComponent<Button>().interactable=false;
        }

        void BuildContribution()
        {
            ShowContribution(0,false,false,false);
        }
        void ShowContribution(int filter,bool taken,bool rolls,bool skills)
        {var view=NewModal("Contribution",new Vector2(960,1760),Close);view.Contribution(filter,taken,rolls,skills,ShowContribution,Close);}

        void ContributionRows(Transform parent,Unit[] units,bool taken,float y)
        {
            Label(parent,taken?"Damage Taken":"Damage Dealt",new Vector2(0,y),38);
            var sorted=units.OrderByDescending(u=>taken?u.DamageTaken:u.DamageDealt).Take(4).ToArray();
            if(sorted.Length==0){Label(parent,"No battle data yet",new Vector2(0,y-100),30);return;}
            float max=sorted.Max(u=>taken?u.DamageTaken:u.DamageDealt);
            for(int i=0;i<sorted.Length;i++){
                var u=sorted[i];float value=taken?u.DamageTaken:u.DamageDealt;
                var row=Rect("Rank",parent,new Vector2(810,65),new Vector2(0,y-75-i*70));BattleUiSkin.Skin(row.gameObject.AddComponent<Image>(),BattleUiSkin.Navy);
                var rank=Label(row,(i+1).ToString(),new Vector2(-365,0),30);rank.rectTransform.sizeDelta=new Vector2(50,60);
                var portrait=Rect("Portrait",row,new Vector2(60,60),new Vector2(-300,0)).gameObject.AddComponent<Image>();portrait.sprite=u.Definition?u.Definition.Portrait:null;portrait.preserveAspect=true;portrait.raycastTarget=false;portrait.enabled=portrait.sprite;
                var track=Rect("Bar",row,new Vector2(440,16),new Vector2(-25,0));track.gameObject.AddComponent<Image>().color=new Color(.18f,.25f,.34f);
                var fill=Rect("Fill",track,Vector2.zero,Vector2.zero);fill.anchorMin=Vector2.zero;fill.anchorMax=new Vector2(max>0?value/max:0,1);fill.offsetMin=fill.offsetMax=Vector2.zero;fill.gameObject.AddComponent<Image>().color=new Color(.1f,.75f,1);
                var number=Label(row,Mathf.RoundToInt(value).ToString(),new Vector2(335,0),30);number.rectTransform.sizeDelta=new Vector2(125,60);
            }
        }

        void Toggle(Transform parent, string name, string key, int fallback, float y)
        {
            var rect = Button(parent, name + ": " + (PlayerPrefs.GetInt(key, fallback) != 0 ? "ON" : "OFF"),
                new Vector2(0, y), () =>
                {
                    PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key, fallback) == 0 ? 1 : 0);
                    PlayerPrefs.Save(); BuildSettings();
                });
            if (key == VibrationKey && !Application.isMobilePlatform) rect.GetComponent<Button>().interactable = false;
        }

        void Confirm(bool home)
        {
            // Deliberately no X: the only visible actions are Stay and Leave.
            var view=NewModal(home?"Leave Battle":"Restart Battle",new Vector2(940,800),null);
            view.Box(view.Panel,"Message",new Vector2(0,25),new Vector2(850,420));
            view.Label(view.Panel,home?"Are you sure you want\nto leave?":"Restart the current battle?",new Vector2(0,80),new Vector2(800,180),48);
            view.Label(view.Panel,"The current attempt will end.",new Vector2(0,-70),new Vector2(800,75),34,new Color(.82f,.8f,.86f));
            ScreenChrome.Action(view.Button(view.Panel,"Stay",new Vector2(-225,-285),new Vector2(420,140),BattleUiSkin.Purple,Close),ScreenChrome.Role.Cancel);
            ScreenChrome.Action(view.Button(view.Panel,home?"Leave":"Restart",new Vector2(225,-285),new Vector2(420,140),BattleUiSkin.Red,() =>
            {
                if (leaving) return;
                leaving = true; paused = false; IsPaused = false; Time.timeScale = 1;
                if (home) SceneRoutes.GoHome(); else SceneRoutes.PlayLevel(GameProgress.CurrentLevel);
            }),ScreenChrome.Role.Danger);
        }

        void Close()
        {
            if(closing)return;
            if(modal)StartCoroutine(CloseAnimated());
            else ResumeTime();
        }

        void ResumeTime(){if(paused){Time.timeScale=previousSpeed;paused=false;IsPaused=false;}}
        // Unity missing components can be fake-null objects: C# ?? does not
        // honour Unity's overloaded null check. Always test with Unity truthiness.
        static CanvasGroup EnsureCanvasGroup(GameObject target)
        {
            var cg=target.GetComponent<CanvasGroup>();
            if(!cg)cg=target.AddComponent<CanvasGroup>();
            return cg;
        }

        IEnumerator CloseAnimated()
        {
            closing=true;
            var target=modal;
            var cg=EnsureCanvasGroup(target);cg.interactable=false;
            // Keep the overlay blocking clicks until it has disappeared.
            cg.blocksRaycasts=true;
            float t=0;
            while(target&&t<.14f){t+=Time.unscaledDeltaTime;cg.alpha=1-Mathf.Clamp01(t/.14f);yield return null;}
            if(target)Destroy(target);
            modal=null;closing=false;ResumeTime();
        }

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            { if (modal != null) Close(); else ToggleMenu(); }
        }

        void OnDestroy()
        {
            if (paused) Time.timeScale = previousSpeed;
            IsPaused = false;
            if (click != null) Destroy(click);
        }
    }
}
