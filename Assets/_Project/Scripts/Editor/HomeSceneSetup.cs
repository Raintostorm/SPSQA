using Project.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Project.EditorTools
{
    /// <summary>
    /// Dựng nội dung trang chủ vào scene ĐANG MỞ.
    ///
    /// Cùng lối với BattleSceneSetup, và cùng lý do: file .unity do Unity
    /// giữ trong bộ nhớ nên tôi không ghi được — nhưng một lệnh menu thì
    /// dựng được mọi thứ bên trong nó.
    ///
    /// Chạy lại được: xoá sạch con rồi dựng lại.
    /// </summary>
    public static class HomeSceneSetup
    {
        const string Bg = "Assets/_Project/Art/Backgrounds/bg_home_portrait.png";
        const string BarSprite = "Assets/_Project/Art/UI/bar_resource.png";
        const string GoldIcon = "Assets/_Project/Art/UI/icon_gold.png";
        const string KeyIcon = "Assets/_Project/Art/UI/icon_key.png";
        const string PanelSprite = "Assets/_Project/Art/UI/panel.png";
        const string ButtonSprite = "Assets/_Project/Art/UI/button.png";
        const string ButtonDown = "Assets/_Project/Art/UI/button_down.png";

        // Man DOC, khop BattleHudBuilder. Truoc day la (1920,1080) — so cua
        // thoi game con ngang, va no lam moi toa do duoi day sai het.
        static readonly Vector2 Reference = new Vector2(1125f, 2436f);
        internal const float SliceBorder = 112f;
        internal static readonly Vector2 PageReference = new Vector2(1125f, 2436f);
        const int Levels = 3;

        [MenuItem("SpinSquad/Dựng TRANG CHỦ")]
        internal static void Build()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.name != "Home")
            {
                Debug.LogError($"[HomeSceneSetup] Đang mở scene '{scene.name}'. " +
                               $"Mở Assets/_Project/Scenes/Home.unity rồi chạy lại.");
                return;
            }

            SetCamera();
            EnsureEventSystem();

            var root = Find("HomeUI") ?? New("HomeUI");
            for (var i = root.transform.childCount - 1; i >= 0; i--)
                Undo.DestroyObjectImmediate(root.transform.GetChild(i).gameObject);

            var canvas = Need<Canvas>(root);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = Need<CanvasScaler>(root);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = Reference;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            // 0 = bam BE NGANG, khop BattleHudBuilder. Man dai ngan khac
            // nhau o chieu CAO, nen bam be ngang thi moi toa do ngang giu
            // nguyen va chi phan tren/duoi co gian.
            scaler.matchWidthOrHeight = 0f;
            Need<GraphicRaycaster>(root);

            // Nền phủ kín. Là Image trong Canvas chứ không phải SpriteRenderer
            // ngoài scene: như vậy nó co giãn theo đúng cùng một CanvasScaler
            // với chữ và nút, không bao giờ lệch nhau.
            var bgGo = New("Background", root.transform);
            var brt = Need<RectTransform>(bgGo);
            brt.anchorMin = Vector2.zero;
            brt.anchorMax = Vector2.one;
            brt.offsetMin = brt.offsetMax = Vector2.zero;
            var bgImg = Need<Image>(bgGo);
            bgImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Bg);
            bgImg.type = Image.Type.Simple;
            bgImg.preserveAspect = false;    // phu kin, chap nhan cat nhe
            bgImg.raycastTarget = false;

            var panel = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSprite);
            var btn = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonSprite);
            var down = AssetDatabase.LoadAssetAtPath<Sprite>(ButtonDown);

            // ── thanh tài nguyên, đỉnh màn ────────────────────────
            // Vàng bên trái, chìa bên phải. Khung 9 mảnh bar_resource rộng
            // 384 gốc, nên ppuMul 2 cho viền co lại còn 56 — vừa ô 430.
            var bar = AssetDatabase.LoadAssetAtPath<Sprite>(BarSprite);
            var goldLabel = Resource(root.transform, "Gold", bar,
                new Vector2(0f, 1f), new Vector2(26f, -40f), GoldIcon);
            var keyLabel = Resource(root.transform, "Keys", bar,
                new Vector2(1f, 1f), new Vector2(-26f, -40f), KeyIcon);

            // ── tên game ──────────────────────────────────────────────
            const float titlePpu = 2f;
            Label(Box(root.transform, "Title", panel,
                new Vector2(0.5f, 1f), new Vector2(0f, -240f),
                new Vector2(760f, 120f), titlePpu),
                "SPINSQUAD", 76, FontStyle.Bold,
                new Color(1f, 0.82f, 0.3f), SliceBorder / titlePpu);

            // ── ba thẻ chọn màn ───────────────────────────────────────
            // Thẻ RỘNG chứ không phải nút vuông nhỏ: màn dọc thừa bề ngang
            // mà thiếu bề cao, nên trải ngang là dùng đúng chỗ đang có.
            const float cardW = 900f, cardH = 200f, cardGap = 40f, cardPpu = 1.4f;
            var buttons = new Button[Levels];
            var labels = new Text[Levels];
            for (var i = 0; i < Levels; i++)
            {
                var y = -760f - i * (cardH + cardGap);
                var rt = Box(root.transform, $"Level{i + 1}", btn,
                    new Vector2(0.5f, 1f), new Vector2(0f, y),
                    new Vector2(cardW, cardH), cardPpu);
                labels[i] = Label(rt, $"MÀN {i + 1}", 58, FontStyle.Bold,
                                  new Color(0.16f, 0.1f, 0.03f), SliceBorder / cardPpu);
                buttons[i] = Need<Button>(rt.gameObject);
                buttons[i].targetGraphic = rt.GetComponent<Image>();
                if (down != null)
                {
                    buttons[i].transition = Selectable.Transition.SpriteSwap;
                    buttons[i].spriteState = new SpriteState
                        { pressedSprite = down, selectedSprite = btn };
                }
            }

            // ── hai nút phụ, tầm ngón cái ─────────────────────────────
            // Đặt GIỮA màn chứ không ở góc: cầm một tay thì ngón cái với
            // tới vùng này dễ nhất, góc trên là vùng khó nhất.
            const float sideW = 430f, sideH = 150f, sidePpu = 1.6f;
            var upgradeBtn = SideButton(root.transform, "UpgradeButton", btn, down,
                new Vector2(-240f, -1560f), new Vector2(sideW, sideH), sidePpu,
                "NÂNG CẤP");
            var treasureBtn = SideButton(root.transform, "TreasureButton", btn, down,
                new Vector2(240f, -1560f), new Vector2(sideW, sideH), sidePpu,
                "KHO BÁU");

            // ── nút CHƠI, đáy màn ─────────────────────────────────────
            // Cùng toạ độ với nút XUẤT QUÂN trong trận: hai màn dùng chung
            // một chỗ cho "hành động chính", tay không phải học lại.
            const float playPpu = 1.2f;
            var playRt = Box(root.transform, "PlayButton", btn,
                new Vector2(0.5f, 0f), new Vector2(0f, 34f),
                new Vector2(1040f, 160f), playPpu);
            Label(playRt, "CHƠI", 64, FontStyle.Bold,
                  new Color(0.16f, 0.1f, 0.03f), SliceBorder / playPpu);
            var playBtn = Need<Button>(playRt.gameObject);
            playBtn.targetGraphic = playRt.GetComponent<Image>();
            if (down != null)
            {
                playBtn.transition = Selectable.Transition.SpriteSwap;
                playBtn.spriteState = new SpriteState
                    { pressedSprite = down, selectedSprite = btn };
            }

            // ── dòng gợi ý, ngay trên nút CHƠI ────────────────────────
            const float hintPpu = 2f;
            var hint = Label(Box(root.transform, "Hint", panel,
                new Vector2(0.5f, 0f), new Vector2(0f, 230f),
                new Vector2(700f, 76f), hintPpu),
                "", 30, FontStyle.Bold, Color.white, SliceBorder / hintPpu);

            var home = Need<HomeScreen>(root);
            Set(home, so =>
            {
                Fill(so.FindProperty("levelButtons"), buttons);
                Fill(so.FindProperty("levelLabels"), labels);
                so.FindProperty("hint").objectReferenceValue = hint;
                so.FindProperty("goldLabel").objectReferenceValue = goldLabel;
                so.FindProperty("keyLabel").objectReferenceValue = keyLabel;
                so.FindProperty("playButton").objectReferenceValue = playBtn;
                so.FindProperty("upgradeButton").objectReferenceValue = upgradeBtn;
                so.FindProperty("treasureButton").objectReferenceValue = treasureBtn;
                so.FindProperty("balance").objectReferenceValue =
                    UnityEditor.AssetDatabase.LoadAssetAtPath<Project.Data.GameBalance>(
                        "Assets/_Project/Data/GameBalance.asset");
            });

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[HomeSceneSetup] Xong. Lưu scene rồi bấm Play.", root);
        }

        /// <summary>Một ô "icon + số" cho thanh tài nguyên.</summary>
        static Text Resource(Transform parent, string name, Sprite bar,
                             Vector2 anchor, Vector2 pos, string iconPath)
        {
            const float ppu = 2f;
            var rt = Box(parent, name, bar, anchor, pos, new Vector2(430f, 128f), ppu);

            var ic = New("Icon", rt);
            var irt = Need<RectTransform>(ic);
            irt.anchorMin = irt.anchorMax = new Vector2(0f, 0.5f);
            irt.pivot = new Vector2(0f, 0.5f);
            irt.anchoredPosition = new Vector2(30f, 0f);
            irt.sizeDelta = new Vector2(86f, 86f);
            var img = Need<Image>(ic);
            img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
            img.preserveAspect = true;
            img.raycastTarget = false;

            // Chữ THỤT khỏi icon chứ không phủ kín ô: phủ kín thì số dài
            // chạy đè lên icon.
            var t = Label(rt, "0", 44, FontStyle.Bold,
                          new Color(1f, 0.9f, 0.5f), 0f);
            var trt = t.GetComponent<RectTransform>();
            trt.offsetMin = new Vector2(130f, 0f);
            trt.offsetMax = new Vector2(-26f, 0f);
            t.alignment = TextAnchor.MiddleLeft;
            return t;
        }

        static Button SideButton(Transform parent, string name, Sprite normal,
                                 Sprite down, Vector2 pos, Vector2 size,
                                 float ppu, string text)
        {
            var rt = Box(parent, name, normal, new Vector2(0.5f, 1f), pos, size, ppu);
            Label(rt, text, 40, FontStyle.Bold,
                  new Color(0.16f, 0.1f, 0.03f), SliceBorder / ppu);
            var b = Need<Button>(rt.gameObject);
            b.targetGraphic = rt.GetComponent<Image>();
            if (down != null)
            {
                b.transition = Selectable.Transition.SpriteSwap;
                b.spriteState = new SpriteState
                    { pressedSprite = down, selectedSprite = normal };
            }
            return b;
        }

        internal static void SetCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = New("Main Camera");
                go.tag = "MainCamera";
                cam = Need<Camera>(go);
            }
            Undo.RecordObject(cam, "Camera trang chủ");
            cam.orthographic = true;
            cam.backgroundColor = Color.black;
            cam.clearFlags = CameraClearFlags.SolidColor;
            EditorUtility.SetDirty(cam);
        }

        internal static void EnsureEventSystem()
        {
            var es = Object.FindFirstObjectByType<EventSystem>();
            var go = es != null ? es.gameObject : New("EventSystem");
            Need<EventSystem>(go);
            var old = go.GetComponent<StandaloneInputModule>();
            if (old != null) Undo.DestroyObjectImmediate(old);
            Need<InputSystemUIInputModule>(go);
        }

        // ── tiện ích, giống BattleHudBuilder ──────────────────────────

        internal static void Fill<T>(SerializedProperty prop, T[] items) where T : Object
        {
            prop.arraySize = items.Length;
            for (var i = 0; i < items.Length; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
        }

        /// <param name="ppuMul">
        /// Co bề rộng viền 9 mảnh lại. Viền hiệu dụng = 112 / ppuMul đơn vị.
        ///
        /// Tôi từng nói nút không thể hẹp hơn 224 — SAI. Con số đó chỉ đúng
        /// khi ppuMul = 1. Unity có sẵn tham số này để vẽ cùng một ảnh ở cỡ
        /// nhỏ hơn mà hai nắp co theo, không bị chồng lên nhau.
        /// </param>
        internal static RectTransform Box(Transform parent, string name, Sprite sprite,
                                 Vector2 anchor, Vector2 pos, Vector2 size,
                                 float ppuMul = 1f)
        {
            var go = New(name, parent);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var img = Need<Image>(go);
            img.sprite = sprite;
            img.type = Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = ppuMul;
            return rt;
        }

        internal static Text Label(RectTransform parent, string text, int size,
                          FontStyle style, Color color, float inset)
        {
            var go = New("Label", parent);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(inset, 0f);
            rt.offsetMax = new Vector2(-inset, 0f);
            var t = Need<Text>(go);
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.text = text;
            t.fontSize = size;
            t.fontStyle = style;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = color;
            t.raycastTarget = false;
            t.resizeTextForBestFit = true;
            t.resizeTextMinSize = 14;
            t.resizeTextMaxSize = size;
            return t;
        }

        internal static GameObject Find(string name)
        {
            var go = GameObject.Find(name);
            return go != null ? go : null;
        }

        internal static GameObject New(string name, Transform parent = null)
        {
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Dựng trang chủ");
            if (parent != null) go.transform.SetParent(parent, false);
            return go;
        }

        internal static T Need<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            return c != null ? c : Undo.AddComponent<T>(go);
        }

        internal static void Set<T>(T target, System.Action<SerializedObject> edit) where T : Object
        {
            var so = new SerializedObject(target);
            edit(so);
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
