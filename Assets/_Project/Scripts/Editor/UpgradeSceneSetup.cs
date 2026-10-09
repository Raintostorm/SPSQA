using System.Collections.Generic;
using System.Linq;
using Project.Data;
using Project.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static Project.EditorTools.HomeSceneSetup;
using static Project.EditorTools.TreasureSceneSetup;

namespace Project.EditorTools
{
    /// <summary>
    /// Dựng màn Nâng cấp: 20 nhân vật, mỗi con một dòng cao 200, cuộn được.
    ///
    /// Ảnh nhân vật lấy KHUNG IDLE ĐẦU TIÊN của chính con đó — không tốn
    /// thêm art nào. 20/20 con đều có idle_1.png.
    /// </summary>
    public static class UpgradeSceneSetup
    {
        const string Bg = "Assets/_Project/Art/Backgrounds/bg_upgrade_portrait.png";
        const string UnitDir = "Assets/_Project/Data/Units";
        const string ArtDir = "Assets/_Project/Art/Units";

        const float RowH = 200f, RowGap = 16f;
        const float ListTop = -420f, ListBottom = 60f;

        [MenuItem("SpinSquad/Dựng màn NÂNG CẤP")]
        internal static void Build()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.name != "Upgrade")
            {
                Debug.LogError($"[UpgradeSceneSetup] Đang mở '{scene.name}'. " +
                               $"Mở Assets/_Project/Scenes/Upgrade.unity rồi chạy lại.");
                return;
            }

            var units = AssetDatabase.FindAssets("t:UnitDefinition", new[] { UnitDir })
                .Select(g => AssetDatabase.LoadAssetAtPath<UnitDefinition>(
                    AssetDatabase.GUIDToAssetPath(g)))
                .Where(u => u != null)
                .OrderBy(u => (int)u.Rank).ThenBy(u => u.UnitId)
                .ToList();
            if (units.Count == 0)
            {
                Debug.LogError($"[UpgradeSceneSetup] {UnitDir} rỗng. Chạy 'roster' trước.");
                return;
            }

            SetCamera();
            EnsureEventSystem();

            var root = Find("UpgradeUI") ?? New("UpgradeUI");
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

            var goldLabel = ResourceBar(root.transform, "Gold", S("bar_resource"),
                new Vector2(0f, 1f), new Vector2(26f, -40f), S("icon_gold"));
            var back = TextButton(root.transform, "BackButton", btn, down,
                new Vector2(1f, 1f), new Vector2(-26f, -40f),
                new Vector2(200f, 128f), 2.2f, "✕", 48);

            Label(Box(root.transform, "Title", panel, new Vector2(0.5f, 1f),
                      new Vector2(0f, -200f), new Vector2(620f, 110f), 2f),
                  "LÒ RÈN", 64, FontStyle.Bold, new Color(1f, 0.86f, 0.35f),
                  SliceBorder / 2f);

            // ── vùng cuộn ─────────────────────────────────────────────
            // 20 dòng × 216 = 4320px, mà chỗ trống chỉ ~1950 — bắt buộc cuộn.
            var viewGo = New("List", root.transform);
            var vrt = Need<RectTransform>(viewGo);
            vrt.anchorMin = new Vector2(0f, 0f);
            vrt.anchorMax = new Vector2(1f, 1f);
            vrt.offsetMin = new Vector2(50f, ListBottom);
            vrt.offsetMax = new Vector2(-50f, ListTop);
            Need<RectMask2D>(viewGo);
            var scroll = Need<ScrollRect>(viewGo);
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Elastic;

            var content = New("Content", viewGo.transform);
            var crt = Need<RectTransform>(content);
            crt.anchorMin = new Vector2(0f, 1f);
            crt.anchorMax = new Vector2(1f, 1f);
            crt.pivot = new Vector2(0.5f, 1f);
            crt.offsetMin = new Vector2(0f, 0f);
            crt.offsetMax = new Vector2(0f, 0f);
            crt.sizeDelta = new Vector2(0f, units.Count * (RowH + RowGap));
            scroll.content = crt;
            scroll.viewport = vrt;

            var names = new Text[units.Count];
            var stats = new Text[units.Count];
            var costs = new Text[units.Count];
            var buttons = new Button[units.Count];
            var portraits = new Button[units.Count];

            for (var i = 0; i < units.Count; i++)
            {
                var rt = Box(content.transform, $"Row{i}", panel,
                    new Vector2(0.5f, 1f), new Vector2(0f, -i * (RowH + RowGap) - RowH * 0.5f),
                    new Vector2(1000f, RowH), 1.6f);
                rt.pivot = new Vector2(0.5f, 0.5f);

                // Chân dung: khung idle đầu tiên, không cần art riêng.
                var portrait = AssetDatabase.LoadAssetAtPath<Sprite>(
                    $"{ArtDir}/{units[i].UnitId}/idle_1.png");
                var pg = New("Portrait", rt);
                var prt = Need<RectTransform>(pg);
                prt.anchorMin = prt.anchorMax = new Vector2(0f, 0.5f);
                prt.pivot = new Vector2(0f, 0.5f);
                prt.anchoredPosition = new Vector2(24f, 0f);
                prt.sizeDelta = new Vector2(150f, 150f);
                var pimg = Need<Image>(pg);
                pimg.sprite = portrait;
                pimg.preserveAspect = true;
                var pbtn = Need<Button>(pg);
                pbtn.targetGraphic = pimg;
                pbtn.transition = Selectable.Transition.ColorTint;
                portraits[i] = pbtn;

                names[i] = Row(rt, "Name", new Vector2(200f, 10f),
                               new Vector2(480f, 60f), 36, TextAnchor.MiddleLeft,
                               Color.white);
                stats[i] = Row(rt, "Stat", new Vector2(200f, -50f),
                               new Vector2(480f, 50f), 28, TextAnchor.MiddleLeft,
                               new Color(0.72f, 0.95f, 0.72f));

                buttons[i] = TextButton(rt, "Buy", btn, down,
                    new Vector2(1f, 0.5f), new Vector2(-24f, 0f),
                    new Vector2(260f, 110f), 2.2f, "NÂNG", 34);
                costs[i] = Row(rt, "Cost", new Vector2(-170f, -60f),
                               new Vector2(240f, 44f), 26, TextAnchor.MiddleCenter,
                               new Color(1f, 0.86f, 0.35f));
                var ca = costs[i].GetComponent<RectTransform>();
                ca.anchorMin = ca.anchorMax = ca.pivot = new Vector2(1f, 0.5f);
                ca.anchoredPosition = new Vector2(-24f, -62f);
            }

            // ── bảng chi tiết, phủ lên trên ───────────────────────────
            var dg = New("Detail", root.transform);
            var drt = Need<RectTransform>(dg);
            drt.anchorMin = Vector2.zero;
            drt.anchorMax = Vector2.one;
            drt.offsetMin = drt.offsetMax = Vector2.zero;
            var dgroup = Need<CanvasGroup>(dg);

            var dim = New("Dim", dg.transform);
            var dimRt = Need<RectTransform>(dim);
            dimRt.anchorMin = Vector2.zero;
            dimRt.anchorMax = Vector2.one;
            dimRt.offsetMin = dimRt.offsetMax = Vector2.zero;
            Need<Image>(dim).color = new Color(0f, 0f, 0f, 0.78f);

            var card = Box(dg.transform, "Card", panel, new Vector2(0.5f, 0.5f),
                           Vector2.zero, new Vector2(940f, 1100f), 1.1f);

            var pgo = New("Portrait", card);
            var pgrt = Need<RectTransform>(pgo);
            pgrt.anchorMin = pgrt.anchorMax = pgrt.pivot = new Vector2(0.5f, 1f);
            pgrt.anchoredPosition = new Vector2(0f, -70f);
            pgrt.sizeDelta = new Vector2(260f, 260f);
            var dPortrait = Need<Image>(pgo);
            dPortrait.preserveAspect = true;
            dPortrait.raycastTarget = false;

            var dTitle = Card(card, "Title", new Vector2(0f, -360f),
                              new Vector2(860f, 60f), 36, TextAnchor.MiddleCenter,
                              Color.white);
            var dBody = Card(card, "Body", new Vector2(0f, -440f),
                             new Vector2(860f, 480f), 30, TextAnchor.UpperLeft,
                             new Color(0.80f, 0.95f, 0.80f));

            var dClose = TextButton(card, "Close", btn, down,
                new Vector2(0.5f, 0f), new Vector2(0f, 40f),
                new Vector2(340f, 110f), 2.2f, "ĐÓNG", 36);

            var screen = Need<UpgradeScreen>(root);
            Set(screen, so =>
            {
                so.FindProperty("balance").objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<GameBalance>(
                        "Assets/_Project/Data/GameBalance.asset");
                so.FindProperty("goldLabel").objectReferenceValue = goldLabel;
                so.FindProperty("backButton").objectReferenceValue = back;
                so.FindProperty("treasures").objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<TreasureDatabase>(
                        "Assets/_Project/Data/TreasureDatabase.asset");
                so.FindProperty("detailGroup").objectReferenceValue = dgroup;
                so.FindProperty("detailPortrait").objectReferenceValue = dPortrait;
                so.FindProperty("detailTitle").objectReferenceValue = dTitle;
                so.FindProperty("detailBody").objectReferenceValue = dBody;
                so.FindProperty("detailClose").objectReferenceValue = dClose;
                Fill(so.FindProperty("portraitButtons"), portraits);
                Fill(so.FindProperty("units"), units.ToArray());
                Fill(so.FindProperty("nameLabels"), names);
                Fill(so.FindProperty("statLabels"), stats);
                Fill(so.FindProperty("costLabels"), costs);
                Fill(so.FindProperty("upgradeButtons"), buttons);
            });

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log($"[UpgradeSceneSetup] Xong. {units.Count} dòng.", root);
        }

        static Text Card(Transform parent, string name, Vector2 pos, Vector2 size,
                         int font, TextAnchor align, Color color)
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
            t.alignment = align;
            t.color = color;
            t.raycastTarget = false;
            t.lineSpacing = 1.15f;
            return t;
        }

        static Text Row(Transform parent, string name, Vector2 pos, Vector2 size,
                        int font, TextAnchor align, Color color)
        {
            var go = New(name, parent);
            var rt = Need<RectTransform>(go);
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var t = Need<Text>(go);
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = font;
            t.fontStyle = FontStyle.Bold;
            t.alignment = align;
            t.color = color;
            t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            return t;
        }
    }
}
