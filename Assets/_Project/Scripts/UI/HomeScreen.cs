using Project.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public sealed class HomeScreen : MonoBehaviour
    {
        [SerializeField] Button[] levelButtons;
        [SerializeField] Text[] levelLabels;
        [SerializeField] Text hint, goldLabel, keyLabel;
        [SerializeField] Button playButton, upgradeButton, treasureButton;
        [SerializeField] Data.GameBalance balance;
        [SerializeField] Data.UnitAnimationSet[] homeUnits;
        [SerializeField] Sprite secondaryButtonSprite;
        sealed class Preview
        {
            public Image Image;
            public Data.UnitAnimationSet.Clip Clip;
            public Vector2 Position;
            public float Scale, Timer;
            public int Step, Direction = 1;
        }
        readonly System.Collections.Generic.List<Preview> previews = new System.Collections.Generic.List<Preview>();
        int selectedLevel;
        bool selecting;
        Text chapterLabel;
        RectTransform chooseScreen;
        ChapterSelectScreen chapterSelect;
        HomeMarchingBackground marchingBackground;
        HomeChrome chrome;
        Text waveCount;
        RectTransform chapterBadge;
        Button selectChapterButton;
        Text waveHeading;
        readonly Color ink = new Color(.06f, .08f, .12f);

        void Start()
        {
            SharedSettings.InstallInitialScene();
            selectedLevel = Mathf.Clamp(GameProgress.Unlocked, 1, 3);
            if (goldLabel != null) goldLabel.text = $"{GameProgress.Gold:N0}";
            if (keyLabel != null) keyLabel.text = $"{GameProgress.Keys:N0}";
            Wire(playButton, () => SceneRoutes.PlayLevel(selectedLevel), "Battle");
            Wire(upgradeButton, SceneRoutes.GoUpgrade, "Upgrade");
            Wire(treasureButton, SceneRoutes.GoTreasure, "Relics");
            Place(playButton, new Vector2(.5f, 0), new Vector2(0, 240), new Vector2(680, 170));
            Place(upgradeButton, new Vector2(.5f, 0), new Vector2(-225, 95), new Vector2(400, 125));
            Place(treasureButton, new Vector2(.5f, 0), new Vector2(225, 95), new Vector2(400, 125));
            SetSecondaryStyle(upgradeButton);
            SetSecondaryStyle(treasureButton);
            if (hint != null)
            {
                var panel = hint.transform.parent as RectTransform;
                Place(panel, new Vector2(.5f, 0), new Vector2(0, 450), new Vector2(500, 110));
                panel.gameObject.SetActive(false);
                var go = new GameObject("CurrentChapter", typeof(RectTransform), typeof(Text));
                go.transform.SetParent(transform, false);
                chapterLabel = go.GetComponent<Text>();
                chapterLabel.font = hint.font;
                chapterLabel.fontSize = 52;
                chapterLabel.fontStyle = FontStyle.Bold;
                chapterLabel.color = Color.white;
                chapterLabel.alignment = TextAnchor.MiddleCenter;
                chapterLabel.raycastTarget = false;
                Place(chapterLabel, new Vector2(.5f, 1), new Vector2(0, -720), new Vector2(700, 110));
                OutlineText(chapterLabel);
            }
            marchingBackground = HomeMarchingBackground.Install(transform);
            BuildCampPreview();
            BuildChooseScreen();
            if(levelButtons!=null)foreach(var button in levelButtons)if(button)button.gameObject.SetActive(false);
            Refresh();
            StyleButton(playButton, 76);
            StyleButton(upgradeButton, 40);
            StyleButton(treasureButton, 40);
            BattleUiSkin.Icon(playButton,"battle");
            BattleUiSkin.Skin(playButton.GetComponent<Image>(),BattleUiSkin.Orange);
            playButton.transition=Selectable.Transition.ColorTint;
            var battleIcon=playButton.transform.Find("ActionIcon");if(battleIcon)battleIcon.gameObject.SetActive(false);
            var battleText=playButton.GetComponentInChildren<Text>();
            if(battleText){battleText.text="BATTLE";battleText.rectTransform.anchorMin=Vector2.zero;battleText.rectTransform.anchorMax=Vector2.one;battleText.rectTransform.offsetMin=battleText.rectTransform.offsetMax=Vector2.zero;OutlineText(battleText);}
            BattleUiSkin.Icon(upgradeButton,"upgrade",true);
            BattleUiSkin.Icon(treasureButton,"relics",true);
            chrome=HomeChrome.Build(transform,goldLabel,keyLabel,upgradeButton,treasureButton);
            var oldTitle=transform.Find("Title");if(oldTitle)oldTitle.gameObject.SetActive(false);
            if(chapterLabel)
            {
                chapterBadge=new GameObject("ChapterBadge",typeof(RectTransform),typeof(Image)).GetComponent<RectTransform>();chapterBadge.SetParent(transform,false);
                Place(chapterBadge,new Vector2(.5f,1),new Vector2(0,-400),new Vector2(510,105));BattleUiSkin.Skin(chapterBadge.GetComponent<Image>(),BattleUiSkin.Green);
                chapterLabel.transform.SetParent(chapterBadge,false);Place(chapterLabel,Vector2.one*.5f,Vector2.zero,new Vector2(490,100));chapterLabel.color=ink;
                var chapterOutline=chapterLabel.GetComponent<Outline>();if(chapterOutline)chapterOutline.enabled=false;
                waveHeading=new GameObject("WaveHeading",typeof(RectTransform),typeof(Text)).GetComponent<Text>();waveHeading.transform.SetParent(transform,false);
                waveHeading.font=chapterLabel.font;waveHeading.text="WAVE";waveHeading.fontSize=86;waveHeading.fontStyle=FontStyle.Bold;waveHeading.alignment=TextAnchor.MiddleCenter;waveHeading.color=new Color(1,.85f,.15f);waveHeading.raycastTarget=false;
                Place(waveHeading,new Vector2(.5f,1),new Vector2(0,-550),new Vector2(650,120));OutlineText(waveHeading);
                var go=new GameObject("WaveCount",typeof(RectTransform),typeof(Text));go.transform.SetParent(transform,false);
                waveCount=go.GetComponent<Text>();waveCount.font=chapterLabel.font;waveCount.fontSize=220;waveCount.fontStyle=FontStyle.Bold;waveCount.color=Color.white;waveCount.alignment=TextAnchor.MiddleCenter;waveCount.raycastTarget=false;
                Place(waveCount,new Vector2(.5f,1),new Vector2(0,-660),new Vector2(650,290));OutlineText(waveCount);waveCount.GetComponent<Outline>().effectDistance=new Vector2(5,-5);
            }
            BuildChapterButton();
            Refresh();
        }

        void BuildChapterButton()
        {
            var go=new GameObject("SelectChapter",typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(transform,false);
            selectChapterButton=go.GetComponent<Button>();go.GetComponent<Image>().color=Color.clear;
            Place(selectChapterButton,new Vector2(.5f,0),new Vector2(0,440),new Vector2(500,140));
            selectChapterButton.onClick.AddListener(()=>{selecting=true;Refresh();});
            var icon=new GameObject("Tent",typeof(RectTransform),typeof(Image)).GetComponent<Image>();icon.transform.SetParent(go.transform,false);icon.sprite=Resources.Load<Sprite>("SharedUI/home");icon.preserveAspect=true;icon.raycastTarget=false;
            Place(icon,Vector2.one*.5f,new Vector2(0,30),new Vector2(100,100));
            var label=new GameObject("Label",typeof(RectTransform),typeof(Text)).GetComponent<Text>();label.transform.SetParent(go.transform,false);label.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");label.text="Select Chapter";label.fontSize=46;label.fontStyle=FontStyle.Bold;label.color=Color.white;label.alignment=TextAnchor.MiddleCenter;label.raycastTarget=false;
            Place(label,Vector2.one*.5f,new Vector2(0,-46),new Vector2(490,70));OutlineText(label);
        }

        void SetSecondaryStyle(Button button)
        {
            if (button == null) return;
            var image = button.GetComponent<Image>();
            if (image == null) return;
            // Keep the scene's approved normal/pressed sprites. The old
            // runtime override replaced blue buttons with a grey card.
            image.color = Color.white;
            button.transition = Selectable.Transition.SpriteSwap;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(.95f, 1, .95f);
            colors.pressedColor = new Color(.7f, .77f, .72f);
            button.colors = colors;
        }

        void BuildCampPreview()
        {
            if (homeUnits == null) return;
            // Visual-only Images: no colliders, combat scripts or pools.
            for (var i = 0; i < Mathf.Min(3, homeUnits.Length); i++)
            {
                var set = homeUnits[i];
                if (set == null) continue;
                var clip = set.Walk != null && set.Walk.IsValid ? set.Walk : set.Idle;
                if (clip == null || !clip.IsValid) continue;
                var go = new GameObject("CampUnit" + i, typeof(RectTransform), typeof(Image));
                go.transform.SetParent(transform, false);
                var image = go.GetComponent<Image>();
                image.raycastTarget = false;
                float height = 1;
                foreach (var frame in clip.Frames)
                    if (frame != null) height = Mathf.Max(height, frame.rect.height);
                var preview = new Preview { Image = image, Clip = clip,
                    Position = new Vector2((i - 1) * 250, i == 1 ? -20 : 0),
                    Scale = 170 / height, Timer = i * .13f };
                previews.Add(preview);
                DrawPreview(preview);
            }
        }

        void Update()
        {
            if (marchingBackground) marchingBackground.Moving = !selecting;
            if (selecting || !Application.isFocused) return;
            foreach (var preview in previews)
            {
                // Match the approved slow 15fps playback of the 30fps Walk clips.
                preview.Timer += Mathf.Min(Time.unscaledDeltaTime,.05f) * .5f;
                while (preview.Timer >= preview.Clip.DurationAt(preview.Step))
                {
                    preview.Timer -= preview.Clip.DurationAt(preview.Step);
                    var pingPong = preview.Clip.UseExplicitPlayback && preview.Clip.PingPong;
                    if (pingPong && preview.Clip.Count > 1)
                    {
                        preview.Step += preview.Direction;
                        if (preview.Step >= preview.Clip.Count)
                        { preview.Direction = -1; preview.Step = preview.Clip.Count - 2; }
                        else if (preview.Step < 0)
                        { preview.Direction = 1; preview.Step = 1; }
                    }
                    else preview.Step = (preview.Step + 1) % preview.Clip.Count;
                    DrawPreview(preview);
                }
            }
        }

        static void DrawPreview(Preview preview)
        {
            var sprite = preview.Clip.FrameAt(preview.Step);
            if (sprite == null) return;
            preview.Image.sprite = sprite;
            var rect = preview.Image.rectTransform;
            // Feet align with the centre of the horizontally scrolling path.
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .26f);
            // Keep native sprite pivot and one scale for the entire clip.
            // Never squash poses into a square or normalize each frame.
            rect.pivot = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
            rect.sizeDelta = sprite.rect.size * preview.Scale;
            rect.anchoredPosition = preview.Position;
        }

        void BuildChooseScreen()
        {
            chapterSelect=ChapterSelectScreen.Build(transform,balance,
                level=>{selectedLevel=level;selecting=false;Refresh();},
                ()=>{selecting=false;Refresh();});
            chooseScreen=chapterSelect.GetComponent<RectTransform>();
        }

        void StyleButton(Button button, int size)
        {
            if (button == null) return;
            var text = button.GetComponentInChildren<Text>(true);
            if (text == null) return;
            text.fontSize = size;
            text.resizeTextMaxSize = size;
            text.color = Color.white;
            OutlineText(text);
        }

        void OutlineText(Text text)
        {
            var outline = text.GetComponent<Outline>();
            if(!outline)outline=text.gameObject.AddComponent<Outline>();
            outline.effectColor = ink;
            outline.effectDistance = new Vector2(2, -2);
        }

        void Refresh()
        {
            if (chapterLabel != null) chapterLabel.text = $"Chapter 1-{selectedLevel}";
            if(waveCount){waveCount.text=$"{(balance?balance.WavesForLevel(selectedLevel):selectedLevel==2?10:3)}";waveCount.gameObject.SetActive(!selecting);}
            if(waveHeading)waveHeading.gameObject.SetActive(!selecting);
            if(chapterBadge)chapterBadge.gameObject.SetActive(!selecting);
            if(selectChapterButton)selectChapterButton.gameObject.SetActive(!selecting);
            if(chrome)chrome.gameObject.SetActive(!selecting);
            if(selecting && chooseScreen)chooseScreen.SetAsLastSibling();
            if (chooseScreen != null) chooseScreen.gameObject.SetActive(selecting);
            if(selecting && chapterSelect)chapterSelect.Refresh(selectedLevel);
        }

        static void Wire(Button button, UnityEngine.Events.UnityAction action, string label)
        {
            if (button == null) return;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
            var text = button.GetComponentInChildren<Text>(true);
            if (text != null) text.text = label;
        }

        static void Place(Component component, Vector2 anchor, Vector2 position, Vector2 size)
        {
            if (component == null) return;
            var rect = component.transform as RectTransform;
            if (rect == null) return;
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
    }
}
