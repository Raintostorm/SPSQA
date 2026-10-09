using System.Collections.Generic;
using Project.Battle;
using Project.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Project.EditorTools
{
    /// <summary>
    /// Dựng lớp chữ và nút nằm trên trận đấu.
    ///
    /// Vì sao dựng bằng code chứ không làm prefab: một Canvas là mấy chục
    /// RectTransform với anchor, pivot và offset — kéo tay thì không ai đọc
    /// lại được vì sao nó ở chỗ đó. Ở đây mỗi con số nằm cạnh lời giải thích.
    ///
    /// Chạy lại được: tìm thấy "HUD" thì xoá sạch con rồi dựng lại.
    /// </summary>
    public static class BattleHudBuilder
    {
        const string PanelSprite  = "Assets/_Project/Art/UI/panel.png";
        const string UiDir        = "Assets/_Project/Art/UI/";
        const string ButtonSprite = "Assets/_Project/Art/UI/button.png";
        const string ButtonDown   = "Assets/_Project/Art/UI/button_down.png";

        // Khung tham chiếu. Mọi toạ độ dưới đây tính theo khung này; Unity
        // tự co giãn khi màn thật khác kích thước.
        // MAN DOC. Co dinh theo BE NGANG (matchWidthOrHeight = 0), nen tren
        // may dai hon thi hai dai HUD duoc rong ra, con nut giu nguyen co.
        static readonly Vector2 Reference = new Vector2(1125f, 2436f);

        /// <summary>
        /// Luoi 4x4 chiem khoang nao theo chiều cao khung nhìn, tính từ tâm.
        ///
        /// HUD phải nằm NGOÀI khoảng này. Số đo: ô cao 1.227 đơn vị, 4 hàng
        /// quanh tâm lệch +0.31 — nên mép trên 2.76, mép dưới −2.14, trên
        /// khung nhìn cao 11.91. Quy ra tỉ lệ.
        /// </summary>
        const float GridTopFrac = 2.76f / 11.91f;     // 0.232 tính từ tâm lên
        const float GridBotFrac = 2.14f / 11.91f;     // 0.180 tính từ tâm xuống

        /// <summary>
        /// Chiều cao chung của mọi khung trên dải trên.
        ///
        /// KHÔNG được nhỏ hơn hai lần bề rộng viền 9 mảnh (26px ⇒ 52px),
        /// không thì bốn góc đè lên nhau và khung méo. Đã vấp: hai ô đếm
        /// quân từng đặt 64px với viền 36px.
        /// </summary>
        const float BoxH = 76f;

        /// <summary>
        /// Đáy hai nút ROLL / XUẤT QUÂN, tính từ đáy màn.
        ///
        /// Màn dọc: lưới 4×4 nằm giữa, mép dưới cách tâm 0.18 chiều cao
        /// khung nhìn (xem GridBotFrac). Dải HUD dưới có 0.32 — thừa chỗ,
        /// nên nút chỉ cần cách đáy một chút cho dễ bấm bằng ngón cái.
        /// </summary>
        const float ButtonY = 34f;

        /// <summary>Cao nút lớn dưới đáy. Màn dọc nên nút to, dễ bấm bằng ngón.</summary>
        const float BigButtonH = 140f;

        /// <summary>Cao cả cụm bảng chỉ số (ba thẻ + khung), đơn vị UI.</summary>
        const float StatPanelH = 620f;

        /// <summary>Số dòng mỗi phe trong bảng. Khung chỉ cao chừng này.</summary>
        const int StatRows = 3;

        /// <summary>
        /// Bề rộng hai đầu trang trí, đơn vị UI. Đo từ chính ảnh bằng
        /// tools/import_ui_art.py — nắp gỗ-kim loại của bộ art này chiếm
        /// 112 đơn vị mỗi bên, nên KHÔNG nút nào được hẹp hơn 224.
        /// </summary>
        const float SliceBorder = 112f;

        public static void Build(BattleFlow flow)
        {
            var panel  = Load(PanelSprite);
            var button = Load(ButtonSprite);
            var down   = Load(ButtonDown);
            if (panel == null || button == null) return;

            EnsureEventSystem();

            var root = Find("HUD") ?? New("HUD");
            for (var i = root.transform.childCount - 1; i >= 0; i--)
                Undo.DestroyObjectImmediate(root.transform.GetChild(i).gameObject);

            var canvas = Need<Canvas>(root);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = Need<CanvasScaler>(root);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = Reference;
            // 0.5 = co theo CẢ hai chiều. Để 0 (chỉ theo bề ngang) thì màn
            // thấp hơn 16:9 sẽ đẩy nút xuống dưới mép.
            // 0 = co theo BE NGANG. Man doc thi be ngang la thu co dinh;
            // de 0.5 thi may dai hon se lam nut be lai, ma cho thi thua ra.
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f;

            Need<GraphicRaycaster>(root);

            // ── dải trên: đếm quân · nút xem địch · tiêu đề · buff ─────
            var ally = Label(
                Box(root.transform, "AllyCount", panel,
                    anchor: new Vector2(0f, 1f), pos: new Vector2(26f, -26f),
                    size: new Vector2(300f, BoxH)),
                "TA  0", 36, FontStyle.Bold, new Color(0.62f, 0.88f, 1f), SliceBorder * 0.9f);

            // Nut doi ban: xem doi hinh dich cua wave sap danh, bam lai thi
            // ve quan ta. Thay cho o dem dich cu — mot cho, hai viec.
            var viewRect = Box(root.transform, "EnemyToggle", button,
                anchor: new Vector2(1f, 1f), pos: new Vector2(-26f, -26f),
                size: new Vector2(420f, BoxH), ppuMul: 1.15f);
            var enemy = Label(viewRect, "XEM ĐỊCH", 34, FontStyle.Bold,
                              new Color(0.16f, 0.1f, 0.03f), SliceBorder / 1.15f);
            var viewBtn = Need<Button>(viewRect.gameObject);
            viewBtn.targetGraphic = viewRect.GetComponent<Image>();
            if (down != null)
            {
                viewBtn.transition = Selectable.Transition.SpriteSwap;
                viewBtn.spriteState = new SpriteState { pressedSprite = down, selectedSprite = button };
            }

            var status = Label(
                Box(root.transform, "TopBar", panel,
                    anchor: new Vector2(0.5f, 1f), pos: new Vector2(0f, -(26f + BoxH + 14f)),
                    size: new Vector2(Reference.x - 52f, BoxH)),
                "", 44, FontStyle.Bold, Color.white, SliceBorder + 8f);

            // ── nút dưới ──────────────────────────────────────────────
            var btnRect = Box(root.transform, "StartButton", button,
                anchor: new Vector2(0.5f, 0f),
                pos: new Vector2(0f, ButtonY),
                size: new Vector2(Reference.x - 52f, BigButtonH));
            Label(btnRect, "XUẤT QUÂN", 54, FontStyle.Bold, new Color(0.16f, 0.1f, 0.03f), SliceBorder);

            var btn = Need<Button>(btnRect.gameObject);
            btn.targetGraphic = btnRect.GetComponent<Image>();
            if (down != null)
            {
                // Đổi ẢNH chứ không nhuộm màu: nhuộm là phép nhân, nó làm tối
                // cả nét viền đen luôn nên nhìn như nút bị mờ chứ không như
                // nút đang bị nhấn.
                btn.transition = Selectable.Transition.SpriteSwap;
                btn.spriteState = new SpriteState { pressedSprite = down, selectedSprite = button };
            }

            // ── bảng buff, ngay dưới thanh tiêu đề ────────────────────
            const float buffPpu = 2.0f;
            var buffBox = Box(root.transform, "Buffs", panel,
                anchor: new Vector2(0.5f, 1f),
                pos: new Vector2(0f, -(26f + BoxH * 2f + 28f)),
                size: new Vector2(Reference.x - 52f, 76f), ppuMul: buffPpu);
            var buffTexts = BuffSlots(buffBox);

            // ── ví coin, cạnh ô đếm quân ta ───────────────────────────
            // Vi coin xuong DAY, canh nut toc do — dai tren da chat.
            var coin = Label(
                Box(root.transform, "Coins", panel,
                    anchor: new Vector2(0f, 0f),
                    pos: new Vector2(26f, ButtonY + BigButtonH + 16f),
                    size: new Vector2(300f, BoxH)),
                "0¢", 38, FontStyle.Bold, new Color(1f, 0.86f, 0.35f), SliceBorder * 0.9f);

            // ── nút ROLL, góc dưới trái ───────────────────────────────
            // ĐÈ LÊN nút XUẤT QUÂN, cùng toạ độ cùng cỡ. Hai nút không bao
            // giờ cùng hiện (còn tiền thì roll, hết tiền thì xuất quân), nên
            // chiếm hai chỗ là phí chỗ và bắt mắt phải đi tìm.
            var rollRect = Box(root.transform, "RollButton", button,
                anchor: new Vector2(0.5f, 0f),
                pos: new Vector2(0f, ButtonY),
                size: new Vector2(Reference.x - 52f, BigButtonH));
            Label(rollRect, "ROLL", 54, FontStyle.Bold, new Color(0.16f, 0.1f, 0.03f), SliceBorder);
            var rollBtn = Need<Button>(rollRect.gameObject);
            rollBtn.targetGraphic = rollRect.GetComponent<Image>();
            if (down != null)
            {
                rollBtn.transition = Selectable.Transition.SpriteSwap;
                rollBtn.spriteState = new SpriteState { pressedSprite = down, selectedSprite = button };
            }

            // ── nút tốc độ, góc dưới phải ─────────────────────────────
            // Hiện ở MỌI giai đoạn, khác nút XUẤT QUÂN: người chơi muốn tua
            // nhanh chính là lúc đang xem đánh nhau.
            // Lên hàng trên, sát trái ô đếm địch. ppuMul 1.6 cho nút gọn —
            // viền còn 70 mỗi bên nên 170 vẫn dư chỗ, hai nắp không chồng.
            const float spdPpu = 1.6f;
            var spdRect = Box(root.transform, "SpeedButton", button,
                anchor: new Vector2(1f, 0f),
                pos: new Vector2(-26f, ButtonY + BigButtonH + 16f),
                size: new Vector2(220f, BoxH), ppuMul: spdPpu);
            var spdLabel = Label(spdRect, "x1", 38, FontStyle.Bold,
                                 new Color(0.16f, 0.1f, 0.03f), SliceBorder / spdPpu);
            var spdBtn = Need<Button>(spdRect.gameObject);
            spdBtn.targetGraphic = spdRect.GetComponent<Image>();
            if (down != null)
            {
                spdBtn.transition = Selectable.Transition.SpriteSwap;
                spdBtn.spriteState = new SpriteState { pressedSprite = down, selectedSprite = button };
            }

            // ── bảng 6 ô kết quả roll, giữa màn ───────────────────────
            var rollPanel = BuildRollPanel(root.transform, panel);

            BuildStatPanel(root.transform, flow);

            var cellPanel = BuildCellPanel(root.transform, panel, button, down);
            BuildResultPanel(root.transform, panel, button, down);

            // ── nối dây ───────────────────────────────────────────────
            var hud = Need<BattleHud>(root);
            Set(hud, so =>
            {
                so.FindProperty("flow").objectReferenceValue = flow;
                so.FindProperty("statusLabel").objectReferenceValue = status;
                so.FindProperty("allyLabel").objectReferenceValue = ally;
                so.FindProperty("enemyLabel").objectReferenceValue = enemy;
                so.FindProperty("startButton").objectReferenceValue = btn;
                so.FindProperty("speedButton").objectReferenceValue = spdBtn;
                so.FindProperty("speedLabel").objectReferenceValue = spdLabel;
                so.FindProperty("rollButton").objectReferenceValue = rollBtn;
                so.FindProperty("coinLabel").objectReferenceValue = coin;
                FillArray(so.FindProperty("buffValues"), buffTexts);
                so.FindProperty("roll").objectReferenceValue =
                    Object.FindFirstObjectByType<Gacha.RollService>();
                so.FindProperty("viewEnemyButton").objectReferenceValue = viewBtn;
            });

            Debug.Log("[BattleHudBuilder] Dựng xong HUD.", root);
        }

        /// <summary>
        /// Bảng xếp hạng trong trận: ba thẻ + khung, nằm dưới mặt đường.
        ///
        /// Thiếu art thì bỏ qua và kêu một tiếng — phần HUD còn lại vẫn phải
        /// dựng được.
        /// </summary>
        static void BuildStatPanel(Transform root, BattleFlow flow)
        {
            var frame = Load(UiDir + "stat_frame.png");
            var tabArt = new[] { Load(UiDir + "tab_damage.png"),
                                 Load(UiDir + "tab_taken.png"),
                                 Load(UiDir + "tab_healing.png") };
            if (frame == null || System.Array.Exists(tabArt, t => t == null))
            {
                Debug.LogWarning("[BattleHudBuilder] Thiếu stat_frame/tab_* — bỏ qua " +
                                 "bảng chỉ số. Chạy tools/import_portrait.py --write.");
                return;
            }

            var go = New("StatPanel", root);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.offsetMin = new Vector2(26f, ButtonY + BigButtonH + BoxH + 34f);
            rt.offsetMax = new Vector2(-26f, rt.offsetMin.y + StatPanelH);
            var grp = Need<CanvasGroup>(go);
            grp.alpha = 0f;
            grp.blocksRaycasts = false;

            // Ba the tren cung, chia deu be ngang.
            const float tabH = 96f, gap = 12f;
            var tabs = new Button[3];
            for (var i = 0; i < 3; i++)
            {
                var t = New($"Tab{i}", go.transform);
                var trt = Need<RectTransform>(t);
                trt.anchorMin = new Vector2(i / 3f, 1f);
                trt.anchorMax = new Vector2((i + 1) / 3f, 1f);
                trt.pivot = new Vector2(0.5f, 1f);
                trt.offsetMin = new Vector2(i == 0 ? 0f : gap / 2f, -tabH);
                trt.offsetMax = new Vector2(i == 2 ? 0f : -gap / 2f, 0f);
                var img = Need<Image>(t);
                img.sprite = tabArt[i];
                img.type = Image.Type.Simple;
                img.preserveAspect = true;
                tabs[i] = Need<Button>(t);
                tabs[i].targetGraphic = img;
            }

            // Khung bang: 9 manh, vien do san trong .meta nen keo gian khong
            // lam meo bon goc.
            var body = New("Body", go.transform);
            var brt = Need<RectTransform>(body);
            brt.anchorMin = Vector2.zero;
            brt.anchorMax = Vector2.one;
            brt.offsetMin = Vector2.zero;
            brt.offsetMax = new Vector2(0f, -(tabH + 8f));
            var bimg = Need<Image>(body);
            bimg.sprite = frame;
            bimg.type = Image.Type.Sliced;

            var allyRows = new List<StatPanel.Row>();
            var enemyRows = new List<StatPanel.Row>();
            var allyHead = StatHeader(body.transform, 0, "QUÂN TA", new Color(0.62f, 0.88f, 1f));
            for (var i = 0; i < StatRows; i++) allyRows.Add(StatRow(body.transform, 1 + i));
            var foeHead = StatHeader(body.transform, 1 + StatRows, "ĐỊCH", new Color(1f, 0.64f, 0.58f));
            for (var i = 0; i < StatRows; i++) enemyRows.Add(StatRow(body.transform, 2 + StatRows + i));

            var sp = Need<StatPanel>(go);
            Set(sp, so =>
            {
                so.FindProperty("group").objectReferenceValue = grp;
                so.FindProperty("flow").objectReferenceValue = flow;
                so.FindProperty("allyHeader").objectReferenceValue = allyHead;
                so.FindProperty("enemyHeader").objectReferenceValue = foeHead;
                FillArray(so.FindProperty("tabs"), tabs);
                FillRows(so.FindProperty("allyRows"), allyRows);
                FillRows(so.FindProperty("enemyRows"), enemyRows);
            });
        }

        static void FillArray(SerializedProperty p, Button[] v)
        {
            p.arraySize = v.Length;
            for (var i = 0; i < v.Length; i++)
                p.GetArrayElementAtIndex(i).objectReferenceValue = v[i];
        }

        static void FillRows(SerializedProperty p, List<StatPanel.Row> rows)
        {
            p.arraySize = rows.Count;
            for (var i = 0; i < rows.Count; i++)
            {
                var e = p.GetArrayElementAtIndex(i);
                e.FindPropertyRelative("Root").objectReferenceValue = rows[i].Root;
                e.FindPropertyRelative("Name").objectReferenceValue = rows[i].Name;
                e.FindPropertyRelative("Bar").objectReferenceValue = rows[i].Bar;
                e.FindPropertyRelative("Value").objectReferenceValue = rows[i].Value;
            }
        }

        /// <summary>Một dòng trong bảng: số thứ tự + tên, thanh số, con số.</summary>
        static StatPanel.Row StatRow(Transform parent, int slot)
        {
            var go = New($"Row{slot}", parent);
            var rt = StatSlot(go, slot);

            var name = MakeText(go.transform, "Name", 26, TextAnchor.MiddleLeft, Color.white);
            Stretch(name.rectTransform, new Vector2(34f, 0f), new Vector2(-170f, 0f));

            var barBg = New("BarBg", go.transform);
            var bg = Need<Image>(barBg);
            bg.color = new Color(0f, 0f, 0f, 0.45f);
            var bgr = Need<RectTransform>(barBg);
            bgr.anchorMin = new Vector2(0f, 0f); bgr.anchorMax = new Vector2(1f, 0f);
            bgr.pivot = new Vector2(0.5f, 0f);
            bgr.offsetMin = new Vector2(34f, 6f); bgr.offsetMax = new Vector2(-170f, 20f);

            var barGo = New("Bar", barBg.transform);
            var bar = Need<Image>(barGo);
            bar.type = Image.Type.Filled;
            bar.fillMethod = Image.FillMethod.Horizontal;
            bar.fillAmount = 0f;
            Stretch(Need<RectTransform>(barGo), Vector2.zero, Vector2.zero);

            var val = MakeText(go.transform, "Value", 26, TextAnchor.MiddleRight,
                               new Color(1f, 0.86f, 0.35f));
            Stretch(val.rectTransform, new Vector2(-160f, 0f), new Vector2(-24f, 0f));
            val.rectTransform.anchorMin = new Vector2(1f, 0f);
            val.rectTransform.anchorMax = new Vector2(1f, 1f);
            val.rectTransform.offsetMin = new Vector2(-160f, 0f);
            val.rectTransform.offsetMax = new Vector2(-24f, 0f);

            return new StatPanel.Row { Root = go, Name = name, Bar = bar, Value = val };
        }

        static Text StatHeader(Transform parent, int slot, string text, Color c)
        {
            var go = New($"Head{slot}", parent);
            StatSlot(go, slot);
            var t = MakeText(go.transform, "Text", 24, TextAnchor.MiddleLeft, c);
            Stretch(t.rectTransform, new Vector2(34f, 0f), new Vector2(-24f, 0f));
            return t;
        }

        /// <summary>Chia khung thành các ô ngang đều nhau, đếm từ trên xuống.</summary>
        static RectTransform StatSlot(GameObject go, int slot)
        {
            var rt = Need<RectTransform>(go);
            const int total = 2 + StatRows * 2;
            rt.anchorMin = new Vector2(0f, 1f - (slot + 1f) / total);
            rt.anchorMax = new Vector2(1f, 1f - slot / (float)total);
            rt.offsetMin = new Vector2(30f, 0f);
            rt.offsetMax = new Vector2(-30f, 0f);
            return rt;
        }

        static void Stretch(RectTransform rt, Vector2 min, Vector2 max)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = min;
            rt.offsetMax = max;
        }

        /// <summary>
        /// Bảng kết quả, phủ kín màn. Dựng SAU CÙNG trong HUD nên nó nằm
        /// trên mọi thứ khác — Unity vẽ UI theo thứ tự anh em trong cây.
        /// </summary>
        static void BuildResultPanel(Transform root, Sprite panelSprite,
                                     Sprite btn, Sprite btnDown)
        {
            var go = New("ResultPanel", root);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            var group = Need<CanvasGroup>(go);
            group.alpha = 0f;

            // Màn mờ phủ hết: vừa làm nổi bảng, vừa chặn chuột xuống lưới.
            var dim = Need<Image>(go);
            dim.color = new Color(0f, 0f, 0f, 0.62f);

            var box = Box(rt.transform, "Box", panelSprite,
                anchor: new Vector2(0.5f, 0.5f), pos: new Vector2(0f, 30f),
                size: new Vector2(760f, 320f));

            var title = Label(Box(box, "Title", panelSprite,
                anchor: new Vector2(0.5f, 1f), pos: new Vector2(0f, -18f),
                size: new Vector2(700f, BoxH)), "", 54, FontStyle.Bold,
                Color.white, SliceBorder * 0.7f);

            var detailGo = New("Detail", box);
            var drt = Need<RectTransform>(detailGo);
            drt.anchorMin = new Vector2(0f, 0f);
            drt.anchorMax = new Vector2(1f, 1f);
            drt.offsetMin = new Vector2(60f, 128f);
            drt.offsetMax = new Vector2(-60f, -110f);
            var detail = Need<Text>(detailGo);
            detail.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            detail.fontSize = 32;
            detail.alignment = TextAnchor.MiddleCenter;
            detail.color = new Color(0.92f, 0.9f, 0.84f);
            detail.raycastTarget = false;

            // Hai nút cạnh nhau. Mỗi nút tối thiểu 224 (bề rộng hai nắp),
            // nên bảng phải rộng ít nhất 2×300 + lề = 700.
            var again = MakeButton(box, "Again", btn, btnDown,
                new Vector2(0f, 0f), new Vector2(26f, 22f), new Vector2(320f, 100f),
                "CHƠI LẠI", out var againLabel);
            var home = MakeButton(box, "Home", btn, btnDown,
                new Vector2(1f, 0f), new Vector2(-26f, 22f), new Vector2(320f, 100f),
                "TRANG CHỦ", out _);

            var rp = Need<UI.ResultPanel>(go);
            Set(rp, so =>
            {
                so.FindProperty("group").objectReferenceValue = group;
                // GHI tu day, khong trong vao viec ai keo tay vao Inspector.
                so.FindProperty("balance").objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<Project.Data.GameBalance>(
                        "Assets/_Project/Data/GameBalance.asset");
                so.FindProperty("dim").objectReferenceValue = dim;
                so.FindProperty("title").objectReferenceValue = title;
                so.FindProperty("detail").objectReferenceValue = detail;
                so.FindProperty("againButton").objectReferenceValue = again;
                so.FindProperty("againLabel").objectReferenceValue = againLabel;
                so.FindProperty("homeButton").objectReferenceValue = home;
            });
        }

        /// <summary>
        /// Bảng thông tin một ô. Nổi bên PHẢI màn, không che lưới ally.
        /// </summary>
        static UI.CellPanel BuildCellPanel(Transform root, Sprite panelSprite,
                                           Sprite btn, Sprite btnDown)
        {
            // 700 chứ không phải 520: hai nút dưới đáy mỗi cái tối thiểu
            // 224 (bề rộng hai nắp trang trí), cộng khe và lề là 700. Ép
            // 520 thì hai nắp gần chạm nhau và chữ bị bóp.
            const float w = 700f, h = 360f;
            var go = New("CellPanel", root);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, 20f);
            rt.sizeDelta = new Vector2(w, h);

            var group = Need<CanvasGroup>(go);
            group.alpha = 0f;

            var bg = Need<Image>(go);
            bg.sprite = panelSprite;
            bg.type = Image.Type.Sliced;

            var title = Label(Box(rt.transform, "Title", panelSprite,
                anchor: new Vector2(0.5f, 1f), pos: new Vector2(0f, -16f),
                size: new Vector2(w - 40f, BoxH)), "", 38, FontStyle.Bold,
                Color.white, SliceBorder * 0.7f);

            var statsGo = New("Stats", rt.transform);
            var srt = Need<RectTransform>(statsGo);
            srt.anchorMin = new Vector2(0f, 0f);
            srt.anchorMax = new Vector2(1f, 1f);
            srt.offsetMin = new Vector2(52f, 140f);
            srt.offsetMax = new Vector2(-52f, -112f);
            var stats = Need<Text>(statsGo);
            stats.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            stats.fontSize = 28;
            stats.alignment = TextAnchor.MiddleCenter;
            stats.color = new Color(0.92f, 0.9f, 0.84f);
            stats.raycastTarget = false;
            stats.resizeTextForBestFit = true;
            stats.resizeTextMinSize = 14;
            stats.resizeTextMaxSize = 28;

            // Ba nút xếp ngang dưới đáy. Bề rộng 224 là tối thiểu dùng được
            // với bộ art này — xem SliceBorder.
            var sell = MakeButton(rt.transform, "Sell", btn, btnDown,
                new Vector2(0f, 0f), new Vector2(26f, 22f), new Vector2(300f, 96f),
                "BÁN", out var sellLabel);
            var evolve = MakeButton(rt.transform, "Evolve", btn, btnDown,
                new Vector2(1f, 0f), new Vector2(-26f, 22f), new Vector2(300f, 96f),
                "TIẾN HOÁ", out _);

            // KHÔNG có nút Đóng. Bấm ra chỗ trống là đóng — CellInspector đã
            // làm sẵn. Thêm nút nữa thì hàng dưới có ba nút, mỗi nút tối
            // thiểu 224, bảng phải rộng 820 và che gần hết lưới.

            var cp = Need<UI.CellPanel>(go);
            Set(cp, so =>
            {
                so.FindProperty("group").objectReferenceValue = group;
                so.FindProperty("title").objectReferenceValue = title;
                so.FindProperty("stats").objectReferenceValue = stats;
                so.FindProperty("sellButton").objectReferenceValue = sell;
                so.FindProperty("sellLabel").objectReferenceValue = sellLabel;
                so.FindProperty("evolveButton").objectReferenceValue = evolve;
            });
            return cp;
        }

        static Button MakeButton(Transform parent, string name, Sprite up, Sprite down,
                                 Vector2 anchor, Vector2 pos, Vector2 size,
                                 string text, out Text label)
        {
            var rt = Box(parent, name, up, anchor, pos, size);
            label = Label(rt, text, 34, FontStyle.Bold, new Color(0.16f, 0.1f, 0.03f),
                          SliceBorder * 0.72f);
            var b = Need<Button>(rt.gameObject);
            b.targetGraphic = rt.GetComponent<Image>();
            if (down != null)
            {
                b.transition = Selectable.Transition.SpriteSwap;
                b.spriteState = new SpriteState { pressedSprite = down, selectedSprite = up };
            }
            return b;
        }

        /// <summary>
        /// Sáu ô kết quả, xếp ngang giữa màn. Ẩn cho tới khi roll xong.
        ///
        /// Đặt ở giữa chứ không nhét vào góc: đây là khoảnh khắc chính của
        /// game, người chơi phải nhìn thấy ngay chứ không phải đi tìm.
        /// </summary>
        static UI.RollPanel BuildRollPanel(Transform root, Sprite panelSprite)
        {
            const int n = 6;
            const float slot = 150f, gap = 14f;
            var width = n * slot + (n - 1) * gap;

            var go = New("RollPanel", root);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, 40f);
            rt.sizeDelta = new Vector2(width, slot + 110f);

            var group = Need<CanvasGroup>(go);
            group.alpha = 0f;

            var slots = new Image[n];
            var icons = new Image[n];
            var slotSprite = AssetDatabase.LoadAssetAtPath<Sprite>(UiDir + "reel_slot.png");
            for (var i = 0; i < n; i++)
            {
                var cell = New($"Slot{i}", rt.transform);
                var crt = Need<RectTransform>(cell);
                crt.anchorMin = crt.anchorMax = crt.pivot = new Vector2(0f, 1f);
                crt.anchoredPosition = new Vector2(i * (slot + gap), 0f);
                crt.sizeDelta = new Vector2(slot, slot);
                slots[i] = Need<Image>(cell);
                slots[i].sprite = slotSprite;
                slots[i].raycastTarget = false;

                var ic = New("Icon", crt);
                var irt = Need<RectTransform>(ic);
                irt.anchorMin = Vector2.zero;
                irt.anchorMax = Vector2.one;
                // Thụt 22% mỗi bên: icon phải nằm GỌN trong lòng ô, không
                // đè lên khung gỗ-kim loại của ô.
                irt.offsetMin = new Vector2(slot * 0.22f, slot * 0.22f);
                irt.offsetMax = new Vector2(-slot * 0.22f, -slot * 0.22f);
                icons[i] = Need<Image>(ic);
                icons[i].raycastTarget = false;
            }

            var bar = Box(rt.transform, "Summary", panelSprite,
                anchor: new Vector2(0.5f, 0f), pos: new Vector2(0f, -14f),
                size: new Vector2(width, BoxH));
            var sum = Label(bar, "", 34, FontStyle.Bold, Color.white, SliceBorder * 0.75f);

            var rp = Need<UI.RollPanel>(go);
            Set(rp, so =>
            {
                so.FindProperty("group").objectReferenceValue = group;
                so.FindProperty("summary").objectReferenceValue = sum;
                FillArray(so.FindProperty("slots"), slots);
                FillArray(so.FindProperty("icons"), icons);
                Sprite(so, "slotNormal", "reel_slot");
                Sprite(so, "slotWin", "reel_slot_win");
                Sprite(so, "coinIcon", "icon_coin");
                Sprite(so, "allyIcon", "icon_ally");
                Sprite(so, "buffIcon", "icon_buff");
            });
            return rp;
        }

        static void FillArray<T>(SerializedProperty prop, T[] items) where T : Object
        {
            prop.arraySize = items.Length;
            for (var i = 0; i < items.Length; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
        }

        static void Sprite(SerializedObject so, string field, string file) =>
            so.FindProperty(field).objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<Sprite>(UiDir + file + ".png");

        // ── các mảnh ──────────────────────────────────────────────────

        /// <summary>
        /// Bốn ô "icon + số" chia đều bề ngang thanh buff.
        ///
        /// Vì sao là icon chứ không phải chữ: bốn cái tên đầy đủ
        /// ("TỐC ĐÁNH", "CHÍ MẠNG"...) cộng bốn con số dài hơn thanh buff,
        /// nên chữ phải co nhỏ tới mức khó đọc. Icon nói đúng một việc mà
        /// không tốn bề ngang.
        ///
        /// Chia bằng anchor theo PHẦN TRĂM chứ không bằng toạ độ pixel: hộp
        /// cha rộng theo màn hình, gán pixel thì máy khác tỉ lệ khác là bốn
        /// ô lệch khỏi hộp.
        ///
        /// Thứ tự PHẢI khớp thứ tự BattleHud.RefreshBuffs ghi số vào.
        /// </summary>
        static Text[] BuffSlots(RectTransform box)
        {
            var files = new[] { "icon_hp", "icon_dmg", "icon_speed", "icon_crit" };
            var texts = new Text[files.Length];

            for (var i = 0; i < files.Length; i++)
            {
                var slot = Need<RectTransform>(New($"Buff{i}", box));
                slot.anchorMin = new Vector2(i / (float)files.Length, 0f);
                slot.anchorMax = new Vector2((i + 1) / (float)files.Length, 1f);
                slot.offsetMin = Vector2.zero;
                slot.offsetMax = Vector2.zero;

                // Icon vuong, neo TRAI, cao bang hop tru le. Dat sizeDelta
                // vuong chu khong keo gian: icon da duoc can ve khung vuong
                // chung o import_buff_icons.py, keo gian la pha cong do.
                var ic = New("Icon", slot);
                var irt = Need<RectTransform>(ic);
                irt.anchorMin = irt.anchorMax = new Vector2(0f, 0.5f);
                irt.pivot = new Vector2(0f, 0.5f);
                irt.anchoredPosition = new Vector2(22f, 0f);
                irt.sizeDelta = new Vector2(52f, 52f);
                var img = Need<Image>(ic);
                img.sprite = Load(UiDir + files[i] + ".png");
                img.type = Image.Type.Simple;
                img.preserveAspect = true;
                img.raycastTarget = false;

                var t = MakeText(slot, "Value", 30, TextAnchor.MiddleLeft,
                                 new Color(0.72f, 0.95f, 0.72f));
                var trt = t.GetComponent<RectTransform>();
                trt.anchorMin = new Vector2(0f, 0f);
                trt.anchorMax = new Vector2(1f, 1f);
                trt.offsetMin = new Vector2(84f, 0f);
                trt.offsetMax = new Vector2(-8f, 0f);
                texts[i] = t;
            }
            return texts;
        }

        /// <summary>
        /// Một khung 9 mảnh neo vào một góc màn hình.
        /// <paramref name="anchor"/> dùng làm CẢ anchor lẫn pivot, nên
        /// <paramref name="pos"/> luôn là khoảng cách tới đúng góc đó —
        /// không phải nhớ dấu âm dương theo từng góc.
        /// </summary>
        /// <param name="ppuMul">
        /// Co bề rộng viền 9 mảnh lại: viền hiệu dụng = SliceBorder / ppuMul.
        /// Nhờ nó mà nút hẹp hơn 224 vẫn vẽ được — hai nắp co theo chứ không
        /// chồng lên nhau.
        /// </param>
        static RectTransform Box(Transform parent, string name, Sprite sprite,
                                 Vector2 anchor, Vector2 pos, Vector2 size,
                                 float ppuMul = 1f)
        {
            if (size.x < 2f * SliceBorder || size.y < 2f * SliceBorder)
                Debug.LogWarning(
                    $"[BattleHudBuilder] '{name}' đặt {size.x}×{size.y}, nhỏ hơn " +
                    $"hai lần viền 9 mảnh ({2f * SliceBorder}). Bốn góc sẽ đè lên " +
                    $"nhau và khung méo.");

            var go = New(name, parent);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            var img = Need<Image>(go);
            img.sprite = sprite;
            // Sliced: giữ nguyên bốn góc, chỉ kéo giãn phần giữa. Để Simple
            // thì nét viền béo ra theo chiều dài nút.
            img.type = Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = ppuMul;
            return rt;
        }

        /// <summary>Chữ phủ kín khung cha, căn giữa.</summary>
        /// <summary>
        /// Chữ căn giữa, THỤT VÀO khỏi hai đầu trang trí.
        ///
        /// Phủ kín khung cha thì chữ dài leo lên mặt nắp gỗ-kim loại và
        /// không đọc được. Thụt đúng bề rộng nắp thì chữ chỉ sống trong
        /// khúc giữa — đúng phần phẳng.
        /// </summary>
        /// <summary>Chữ tự do, căn trái/phải tuỳ ý — khác Label() luôn căn giữa.</summary>
        static Text MakeText(Transform parent, string name, int size,
                             TextAnchor align, Color color)
        {
            var go = New(name, parent);
            Need<RectTransform>(go);
            var t = Need<Text>(go);
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = size;
            t.fontStyle = FontStyle.Bold;
            t.alignment = align;
            t.color = color;
            t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            return t;
        }

        static Text Label(RectTransform parent, string text, int size,
                          FontStyle style, Color? color = null, float inset = 0f)
        {
            var go = New("Label", parent);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(inset, 0f);
            rt.offsetMax = new Vector2(-inset, 0f);

            var t = Need<Text>(go);
            // TMP Essentials chưa nhập trong dự án này, nên Text cổ điển với
            // font dựng sẵn là thứ chạy được NGAY, không cần bấm gì thêm.
            // Muốn đẹp hơn thì nhập TMP rồi đổi sau — bố cục không đổi.
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.text = text;
            t.fontSize = size;
            t.fontStyle = style;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = color ?? Color.white;
            t.raycastTarget = false;     // chữ không được nuốt cú bấm của nút

            // Tự co khi chuỗi dài hơn chỗ trống. "x1" giữ nguyên cỡ to, còn
            // "XẾP QUÂN VÀO LƯỚI" tự nhỏ lại thay vì tràn ra ngoài. Không có
            // cái này thì mỗi lần đổi chữ lại phải dò lại cỡ bằng tay.
            t.resizeTextForBestFit = true;
            t.resizeTextMinSize = 14;
            t.resizeTextMaxSize = size;
            return t;
        }

        /// <summary>
        /// Không có EventSystem thì nút không nhận được cú bấm nào — và Unity
        /// KHÔNG báo lỗi, chỉ là bấm không ăn.
        ///
        /// Phải là InputSystemUIInputModule chứ không phải StandaloneInputModule:
        /// dự án đặt activeInputHandler = 1 (chỉ Input System mới), module cũ
        /// sẽ ném lỗi ngay frame đầu.
        /// </summary>
        static void EnsureEventSystem()
        {
            var es = Object.FindFirstObjectByType<EventSystem>();
            var go = es != null ? es.gameObject : New("EventSystem");
            Need<EventSystem>(go);

            var old = go.GetComponent<StandaloneInputModule>();
            if (old != null) Undo.DestroyObjectImmediate(old);
            Need<InputSystemUIInputModule>(go);
        }

        // ── tiện ích ──────────────────────────────────────────────────

        static Sprite Load(string path)
        {
            var s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (s == null)
                Debug.LogError($"[BattleHudBuilder] Không thấy {path}. " +
                               $"Chạy: python3 tools/make_ui.py");
            return s;
        }

        static GameObject New(string name, Transform parent = null)
        {
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Dựng HUD");
            if (parent != null) go.transform.SetParent(parent, false);
            return go;
        }

        /// <summary>
        /// Tìm object gốc theo tên. Bọc lại GameObject.Find chỉ để mọi chỗ
        /// trong file dùng chung một lối — đỡ phải nhớ chỗ nào trả null thật.
        /// </summary>
        static GameObject Find(string name)
        {
            var go = GameObject.Find(name);
            return go != null ? go : null;
        }

        /// <summary>
        /// Lấy component, chưa có thì thêm.
        ///
        /// KHÔNG được viết `GetComponent<T>() ?? AddComponent<T>()`. Toán tử
        /// `??` của C# so với null THẬT, nó KHÔNG gọi toán tử `==` mà Unity
        /// nạp chồng. Một component đã bị huỷ ở phía C++ vẫn còn vỏ bọc C#
        /// khác null thật, nên `??` trả về cái vỏ rỗng đó thay vì thêm mới.
        ///
        /// `c != null` thì gọi đúng toán tử của Unity, và coi vỏ rỗng là null.
        /// </summary>
        static T Need<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            return c != null ? c : Undo.AddComponent<T>(go);
        }

        static void Set<T>(T target, System.Action<SerializedObject> edit) where T : Object
        {
            var so = new SerializedObject(target);
            edit(so);
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
