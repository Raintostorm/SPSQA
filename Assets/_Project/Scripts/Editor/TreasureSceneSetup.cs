using Project.Data;
using Project.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static Project.EditorTools.HomeSceneSetup;

namespace Project.EditorTools
{
    /// <summary>
    /// Dựng màn Kho báu vào scene Treasure đang mở.
    ///
    /// Bố cục đo trước ở outdata/PROMPT_TREASURE_UI.md: lưới 5 cột × 6 hàng,
    /// ô 180, khe 25, lề 62 mỗi bên. Dải nút quay và lưới nằm đúng hai vùng
    /// đã yêu cầu ảnh nền để trống (độ lệch chuẩn đo được 0.7).
    /// </summary>
    public static class TreasureSceneSetup
    {
        const string Dir = "Assets/_Project/Art/UI/";
        const string Bg = "Assets/_Project/Art/Backgrounds/bg_treasure_portrait.png";

        const int Cols = 5, Rows = 6;
        const float Slot = 180f, Gap = 25f;
        const float GridTop = -420f;       // từ mép trên
        const float RollY = -1180f;
        const float GridY = -1360f;

        [MenuItem("SpinSquad/Dựng màn KHO BÁU")]
        internal static void Build()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.name != "Treasure")
            {
                Debug.LogError($"[TreasureSceneSetup] Đang mở '{scene.name}'. " +
                               $"Mở Assets/_Project/Scenes/Treasure.unity rồi chạy lại.");
                return;
            }

            SetCamera();
            EnsureEventSystem();

            var root = Find("TreasureUI") ?? New("TreasureUI");
            for (var i = root.transform.childCount - 1; i >= 0; i--)
                Undo.DestroyObjectImmediate(root.transform.GetChild(i).gameObject);

            var canvas = Need<Canvas>(root);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = Need<CanvasScaler>(root);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = PageReference;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f;
            Need<GraphicRaycaster>(root);

            Backdrop(root.transform, Bg);

            var panel = S("panel");
            var btn = S("button");
            var down = S("button_down");
            var bar = S("bar_resource");

            // ── thanh chìa + nút quay lại ─────────────────────────────
            var keyLabel = ResourceBar(root.transform, "Keys", bar,
                new Vector2(0f, 1f), new Vector2(26f, -40f), S("icon_key"));
            var back = TextButton(root.transform, "BackButton", btn, down,
                new Vector2(1f, 1f), new Vector2(-26f, -40f),
                new Vector2(200f, 128f), 2.2f, "✕", 48);

            Label(Box(root.transform, "Title", panel, new Vector2(0.5f, 1f),
                      new Vector2(0f, -200f), new Vector2(620f, 110f), 2f),
                  "KHO BÁU", 64, FontStyle.Bold, new Color(1f, 0.86f, 0.35f),
                  SliceBorder / 2f);

            // ── rương ở giữa ──────────────────────────────────────────
            var chestGo = New("Chest", root.transform);
            var crt = Need<RectTransform>(chestGo);
            crt.anchorMin = crt.anchorMax = crt.pivot = new Vector2(0.5f, 1f);
            crt.anchoredPosition = new Vector2(0f, -360f);
            crt.sizeDelta = new Vector2(420f, 420f);
            var chest = Need<Image>(chestGo);
            chest.sprite = S("chest_closed");
            chest.preserveAspect = true;
            chest.raycastTarget = false;

            var result = Label(Box(root.transform, "Result", panel,
                new Vector2(0.5f, 1f), new Vector2(0f, -820f),
                new Vector2(980f, 150f), 2f),
                "", 32, FontStyle.Bold, Color.white, SliceBorder / 2f);

            // ── ba nút quay ───────────────────────────────────────────
            // Mỗi nút một ảnh riêng, phân biệt bằng MÀU chứ không chỉ chữ.
            var rolls = new Button[3];
            var sprites = new[] { "btn_roll1", "btn_roll10", "btn_roll100" };
            var texts = new[] { "x1", "x10", "x100" };
            const float rw = 320f, rgap = 30f;
            for (var i = 0; i < 3; i++)
            {
                var x = (i - 1) * (rw + rgap);
                rolls[i] = TextButton(root.transform, $"Roll{texts[i]}",
                    S(sprites[i]), S(sprites[i] + "_down"),
                    new Vector2(0.5f, 1f), new Vector2(x, RollY),
                    new Vector2(rw, 150f), 1.6f, texts[i], 44);
            }

            // ── lưới 30 ô, trong vùng CUỘN ────────────────────────────
            // 6 hàng × 205 = 1230px, mà chỗ trống dưới ba nút quay chỉ còn
            // ~1050. Không cuộn thì hàng cuối bị cắt mất.
            var viewGo = New("GridView", root.transform);
            var vrt = Need<RectTransform>(viewGo);
            vrt.anchorMin = new Vector2(0f, 0f);
            vrt.anchorMax = new Vector2(1f, 1f);
            vrt.offsetMin = new Vector2(40f, 24f);
            vrt.offsetMax = new Vector2(-40f, GridY);
            Need<RectMask2D>(viewGo);
            var scroll = Need<ScrollRect>(viewGo);
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Elastic;

            var content = New("Content", viewGo.transform);
            var cort = Need<RectTransform>(content);
            cort.anchorMin = new Vector2(0f, 1f);
            cort.anchorMax = new Vector2(1f, 1f);
            cort.pivot = new Vector2(0.5f, 1f);
            cort.offsetMin = cort.offsetMax = Vector2.zero;
            cort.sizeDelta = new Vector2(0f, Rows * (Slot + Gap) + Gap);
            scroll.content = cort;
            scroll.viewport = vrt;

            var frames = new Image[Cols * Rows];
            var icons = new Image[Cols * Rows];
            var levels = new Text[Cols * Rows];
            var slotBtns = new Button[Cols * Rows];
            var left = -(Cols * Slot + (Cols - 1) * Gap) * 0.5f + Slot * 0.5f;
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Cols; c++)
            {
                var i = r * Cols + c;
                var rt = Box(content.transform, $"Slot{i}", S("slot_locked"),
                    new Vector2(0.5f, 1f),
                    new Vector2(left + c * (Slot + Gap),
                                -Gap - Slot * 0.5f - r * (Slot + Gap)),
                    new Vector2(Slot, Slot), 2.4f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                frames[i] = rt.GetComponent<Image>();
                slotBtns[i] = Need<Button>(rt.gameObject);
                slotBtns[i].targetGraphic = frames[i];
                // Khong doi anh khi bam: khung o doi theo BAC, doi them theo
                // trang thai bam la hai luat tranh nhau mot cho.
                slotBtns[i].transition = Selectable.Transition.ColorTint;

                // Icon NHỎ hơn khung: lòng khung đo được 209/304 ≈ 0.69 của
                // khung, nên 0.66 là vừa lọt cả năm loại khung.
                var ic = New("Icon", rt);
                var irt = Need<RectTransform>(ic);
                irt.anchorMin = irt.anchorMax = irt.pivot = new Vector2(0.5f, 0.5f);
                irt.anchoredPosition = Vector2.zero;
                irt.sizeDelta = new Vector2(Slot * 0.66f, Slot * 0.66f);
                icons[i] = Need<Image>(ic);
                icons[i].preserveAspect = true;
                icons[i].raycastTarget = false;

                var lv = Label(rt, "", 24, FontStyle.Bold,
                               new Color(1f, 0.92f, 0.55f), 0f);
                var lrt = lv.GetComponent<RectTransform>();
                lrt.offsetMin = new Vector2(0f, 6f);
                lrt.offsetMax = new Vector2(0f, -Slot * 0.62f);
                lv.alignment = TextAnchor.LowerCenter;
                levels[i] = lv;
            }

            // ── bảng chi tiết, phủ lên trên ───────────────────────────
            var dg = New("Detail", root.transform);
            var drt = Need<RectTransform>(dg);
            drt.anchorMin = Vector2.zero;
            drt.anchorMax = Vector2.one;
            drt.offsetMin = drt.offsetMax = Vector2.zero;
            var dgroup = Need<CanvasGroup>(dg);

            // Lớp chắn tối: không có thì bấm xuyên qua xuống lưới phía dưới.
            var dim = New("Dim", dg.transform);
            var dimRt = Need<RectTransform>(dim);
            dimRt.anchorMin = Vector2.zero;
            dimRt.anchorMax = Vector2.one;
            dimRt.offsetMin = dimRt.offsetMax = Vector2.zero;
            var dimImg = Need<Image>(dim);
            dimImg.color = new Color(0f, 0f, 0f, 0.75f);

            var card = Box(dg.transform, "Card", panel, new Vector2(0.5f, 0.5f),
                           Vector2.zero, new Vector2(900f, 700f), 1.2f);

            var digo = New("Icon", card);
            var dirt = Need<RectTransform>(digo);
            dirt.anchorMin = dirt.anchorMax = dirt.pivot = new Vector2(0.5f, 1f);
            dirt.anchoredPosition = new Vector2(0f, -60f);
            dirt.sizeDelta = new Vector2(200f, 200f);
            var dIcon = Need<Image>(digo);
            dIcon.preserveAspect = true;
            dIcon.raycastTarget = false;

            var dTitle = Row(card, "Title", new Vector2(0f, -290f),
                             new Vector2(820f, 60f), 34, Color.white);
            var dBody = Row(card, "Body", new Vector2(0f, -370f),
                            new Vector2(820f, 240f), 30,
                            new Color(0.78f, 0.95f, 0.78f));
            dBody.alignment = TextAnchor.UpperCenter;

            var dClose = TextButton(card, "Close", btn, down,
                new Vector2(0.5f, 0f), new Vector2(0f, 40f),
                new Vector2(320f, 110f), 2.2f, "ĐÓNG", 36);

            var screen = Need<TreasureScreen>(root);
            Set(screen, so =>
            {
                so.FindProperty("database").objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<TreasureDatabase>(
                        "Assets/_Project/Data/TreasureDatabase.asset");
                so.FindProperty("balance").objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<GameBalance>(
                        "Assets/_Project/Data/GameBalance.asset");
                so.FindProperty("keyLabel").objectReferenceValue = keyLabel;
                so.FindProperty("resultLabel").objectReferenceValue = result;
                so.FindProperty("chestImage").objectReferenceValue = chest;
                so.FindProperty("chestClosed").objectReferenceValue = S("chest_closed");
                so.FindProperty("chestOpen").objectReferenceValue = S("chest_open");
                so.FindProperty("backButton").objectReferenceValue = back;
                so.FindProperty("detailGroup").objectReferenceValue = dgroup;
                so.FindProperty("detailIcon").objectReferenceValue = dIcon;
                so.FindProperty("detailTitle").objectReferenceValue = dTitle;
                so.FindProperty("detailBody").objectReferenceValue = dBody;
                so.FindProperty("detailClose").objectReferenceValue = dClose;
                Fill(so.FindProperty("rollButtons"), rolls);
                Fill(so.FindProperty("slotButtons"), slotBtns);
                Fill(so.FindProperty("slotFrames"), frames);
                Fill(so.FindProperty("slotIcons"), icons);
                Fill(so.FindProperty("slotLevels"), levels);
                Fill(so.FindProperty("frameByTier"), new[]
                {
                    S("slot_locked"), S("slot_common"), S("slot_rare"),
                    S("slot_epic"), S("slot_legendary"),
                });
            });

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log($"[TreasureSceneSetup] Xong. {Cols}×{Rows} ô.", root);
        }

        static Text Row(Transform parent, string name, Vector2 pos,
                        Vector2 size, int font, Color color)
        {
            var go = New(name, parent);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var t = Need<Text>(go);
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = font;
            t.fontStyle = FontStyle.Bold;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = color;
            t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            return t;
        }

        internal static Sprite S(string name) =>
            AssetDatabase.LoadAssetAtPath<Sprite>(Dir + name + ".png");

        internal static void Backdrop(Transform parent, string path)
        {
            var go = New("Background", parent);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var img = Need<Image>(go);
            img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            img.preserveAspect = false;
            img.raycastTarget = false;
        }

        internal static Text ResourceBar(Transform parent, string name, Sprite bar,
                                         Vector2 anchor, Vector2 pos, Sprite icon)
        {
            var rt = Box(parent, name, bar, anchor, pos, new Vector2(430f, 128f), 2f);
            var ic = New("Icon", rt);
            var irt = Need<RectTransform>(ic);
            irt.anchorMin = irt.anchorMax = new Vector2(0f, 0.5f);
            irt.pivot = new Vector2(0f, 0.5f);
            irt.anchoredPosition = new Vector2(30f, 0f);
            irt.sizeDelta = new Vector2(86f, 86f);
            var img = Need<Image>(ic);
            img.sprite = icon;
            img.preserveAspect = true;
            img.raycastTarget = false;

            var t = Label(rt, "0", 44, FontStyle.Bold, new Color(1f, 0.9f, 0.5f), 0f);
            var trt = t.GetComponent<RectTransform>();
            trt.offsetMin = new Vector2(130f, 0f);
            trt.offsetMax = new Vector2(-26f, 0f);
            t.alignment = TextAnchor.MiddleLeft;
            return t;
        }

        internal static Button TextButton(Transform parent, string name, Sprite normal,
                                          Sprite down, Vector2 anchor, Vector2 pos,
                                          Vector2 size, float ppu, string text, int font)
        {
            var rt = Box(parent, name, normal, anchor, pos, size, ppu);
            Label(rt, text, font, FontStyle.Bold,
                  new Color(0.16f, 0.1f, 0.03f), SliceBorder / ppu);
            var b = Need<Button>(rt.gameObject);
            b.targetGraphic = rt.GetComponent<Image>();
            if (down != null)
            {
                b.transition = Selectable.Transition.SpriteSwap;
                b.spriteState = new SpriteState { pressedSprite = down, selectedSprite = normal };
            }
            return b;
        }
    }
}
