using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Khung chung của mọi màn (UI-005): MỘT kiểu tiêu đề, MỘT nút back.
    ///
    /// Trước đây mỗi màn tự vẽ tiêu đề (chữ trơn, hộp xanh, nút tím...) và
    /// tự đặt back (góc trên, nút to dưới đáy, X đỏ). Gom về đây để đổi một
    /// chỗ là cả game đổi theo.
    /// </summary>
    public static class ScreenChrome
    {
        public enum Banner { Screen, Buff, Section }

        public const float TitleHeight = 150, SectionHeight = 90, BackSize = 110;

        // Chiều cao thật của sprite trong Resources/TitleBanners. Border dọc
        // của ruy băng chỉ 26 px: ép thấp mà giữ PPU 1 thì đuôi gập bị bóp
        // dẹt, nên thu cả sprite theo tỉ lệ bằng pixelsPerUnitMultiplier.
        const float RibbonPixels = 243, PlaquePixels = 230;

        static Sprite SpriteFor(Banner banner) => Resources.Load<Sprite>("TitleBanners/" + banner switch
        {
            Banner.Buff => "purple-ribbon",
            Banner.Section => "navy-plaque",
            _ => "navy-ribbon",
        });

        /// <summary>Tiêu đề trên nền banner. Trả về Text để màn còn đổi chữ được.</summary>
        public static Text Title(Transform parent, string value, Vector2 position, float width, Banner banner = Banner.Screen, float height = 0)
        {
            // height 0 = standard size; a smaller one for screens whose header row is already full.
            if (height <= 0) height = banner == Banner.Section ? SectionHeight : TitleHeight;
            var frame = new GameObject("Title " + value, typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            frame.SetParent(parent, false);
            BattleUiSkin.Place(frame, Vector2.one * .5f, position, new Vector2(width, height));
            var image = frame.GetComponent<Image>();
            image.sprite = SpriteFor(banner); image.enabled = image.sprite; image.raycastTarget = false;
            image.type = Image.Type.Sliced;
            float scale = (banner == Banner.Section ? PlaquePixels : RibbonPixels) / height;
            image.pixelsPerUnitMultiplier = scale;

            // Chữ chỉ nằm trên phần giữa phẳng, không đè lên đuôi gập.
            float inset = image.sprite ? image.sprite.border.x / scale : 0;
            var label = new GameObject("Label", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
            label.transform.SetParent(frame, false);
            var r = label.rectTransform; r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
            r.offsetMin = new Vector2(inset * .6f, height * .12f); r.offsetMax = new Vector2(-inset * .6f, -height * .16f);
            label.font = UiFonts.Display; label.text = value; label.color = Color.white;
            label.alignment = TextAnchor.MiddleCenter; label.raycastTarget = false;
            label.fontSize = Mathf.RoundToInt(height * .5f);
            label.resizeTextForBestFit = true; label.resizeTextMinSize = 20; label.resizeTextMaxSize = label.fontSize;
            var outline = label.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(.02f, .03f, .06f, .95f); outline.effectDistance = new Vector2(3, -3);
            return label;
        }

        /// <summary>
        /// Cho màn dựng theo MÉP TRÊN (pivot trên, y đi xuống): ghim khối chrome vào
        /// toạ độ đó thay vì tâm parent. top = mép trên của khối.
        /// </summary>
        public static RectTransform PinTop(Component part, float x, float top)
        {
            var rt = part is Text text ? (RectTransform)text.transform.parent : (RectTransform)part.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, 1);
            rt.anchoredPosition = new Vector2(x, -top);
            return rt;
        }

        /// <summary>
        /// Vai trò của một nút. MÀU theo vai trò, không theo sở thích từng màn (UI-006):
        /// vàng = việc chính của màn, xanh = việc phụ, xám = quay lại/huỷ, đỏ = nguy hiểm.
        /// </summary>
        public enum Role { Primary, Secondary, Cancel, Danger }

        static string Art(Role role) => role switch
        {
            Role.Secondary => "blue",
            Role.Cancel => "slate",
            Role.Danger => "red",
            _ => "gold",
        };

        /// <summary>
        /// Gắn art nút đã duyệt (Resources/Buttons, chat 002) theo vai trò, kèm trạng thái
        /// bấm/tắt. Dùng thay cho việc mỗi màn tự chọn một sprite trong BattleUiSkin.
        /// </summary>
        public static Button Action(Button button, Role role)
        {
            if (!button) return button;
            var image = button.targetGraphic as Image ?? button.GetComponent<Image>();
            ButtonArt(image, Art(role));
            // Action can run again on a refresh, so never stack a second copy of the helper.
            // Art của bản "pressed" chỉ đậm hơn một chút, nên hạ nội dung xuống cho thấy rõ là đang bấm.
            if (!button.GetComponent<Press>()) button.gameObject.AddComponent<Press>();
            // Sprite "disabled" chỉ thay mặt nút; chữ và icon bên trong vẫn sáng nguyên nên
            // nút tắt vẫn trông bấm được (OPEN ALL khi hết chìa, SOLD OUT trong Shop).
            if (!button.GetComponent<DimWhenDisabled>()) button.gameObject.AddComponent<DimWhenDisabled>();
            return button;
        }

        public static void Action(Component button, Role role)
        {
            if (button) Action(button.GetComponent<Button>(), role);
        }

        /// <summary>
        /// Như <see cref="Action(Button, Role)"/> nhưng gắn thêm icon (Resources/ButtonIcons,
        /// chat 010) ở mép trái. Icon bạc viền đen nên đọc được trên cả bốn màu nút.
        /// </summary>
        public static Button Action(Button button, Role role, string icon)
        {
            if (!button) return button;
            // Icon nut (chat 010) va ky hieu nut tron (chat 012) nam hai thu muc khac nhau;
            // nhan ca hai de cho goi khong phai biet art nam o dau.
            var art = Resources.Load<Sprite>("ButtonIcons/" + icon) ?? Resources.Load<Sprite>("Glyphs/" + icon);
            if (art)
            {
                var holder = button.transform.Find("RoleIcon") as RectTransform;
                if (!holder)
                {
                    holder = new GameObject("RoleIcon", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
                    holder.SetParent(button.transform, false);
                    holder.SetAsFirstSibling();
                }
                var graphic = holder.GetComponent<Image>();
                graphic.sprite = art; graphic.preserveAspect = true; graphic.raycastTarget = false; graphic.color = Color.white;
                // Rect của nút thường chỉ có cỡ thật sau khi layout chạy, nên để component tự tính lại.
                var fit = holder.GetComponent<RoleIconFit>() ?? holder.gameObject.AddComponent<RoleIconFit>();
                fit.Apply();
            }
            return Action(button, role);
        }

        public static void Action(Component button, Role role, string icon)
        {
            if (button) Action(button.GetComponent<Button>(), role, icon);
        }

        /// <summary>Giữ icon vuông, cao 62% nút và cách mép trái 20% — tính lại khi nút đổi cỡ.</summary>
        sealed class RoleIconFit : MonoBehaviour
        {
            void OnEnable() => Apply();
            void OnRectTransformDimensionsChange() => Apply();
            public void Apply()
            {
                var rect = (RectTransform)transform;
                var parent = rect.parent as RectTransform;
                if (!parent) return;
                float height = Mathf.Max(1, parent.rect.height);
                float size = height * .62f, margin = height * .20f;
                rect.anchorMin = rect.anchorMax = new Vector2(0, .5f);
                rect.pivot = Vector2.one * .5f;
                rect.sizeDelta = new Vector2(size, size);
                rect.anchoredPosition = new Vector2(margin + size * .5f, 0);
                // Chữ vẫn căn giữa nút, nên phải đẩy sang phải đúng nửa phần icon chiếm chỗ,
                // nếu không nhãn ngắn như "Summon x1" sẽ đè lên icon.
                if (labels == null)
                {
                    var found = parent.GetComponentsInChildren<Text>(true);
                    labels = new RectTransform[found.Length];
                    home = new float[found.Length];
                    for (int i = 0; i < found.Length; i++) { labels[i] = found[i].rectTransform; home[i] = labels[i].anchoredPosition.x; }
                }
                float shift = (margin + size) * .5f;
                for (int i = 0; i < labels.Length; i++)
                    if (labels[i]) labels[i].anchoredPosition = new Vector2(home[i] + shift, labels[i].anchoredPosition.y);
            }

            RectTransform[] labels;
            float[] home;
        }

        /// <summary>Làm xám chữ và icon bên trong khi nút không bấm được.</summary>
        public sealed class DimWhenDisabled : MonoBehaviour
        {
            static readonly Color Off = new Color(.60f, .60f, .60f, .80f);
            Button button;
            Graphic[] parts;
            Color[] home;
            bool dimmed;

            void OnEnable() { button = GetComponent<Button>(); dimmed = !Ready(); Paint(); }
            bool Ready() => button && button.IsInteractable();
            void Update() { bool now = !Ready(); if (now != dimmed) { dimmed = now; Paint(); } }

            void Paint()
            {
                if (parts == null)
                {
                    var found = GetComponentsInChildren<Graphic>(true);
                    var kept = new System.Collections.Generic.List<Graphic>();
                    // Mặt nút tự đổi sprite rồi, chỉ xử lý phần nội dung bên trong.
                    foreach (var g in found) if (g && g.gameObject != gameObject) kept.Add(g);
                    parts = kept.ToArray();
                    home = new Color[parts.Length];
                    for (int i = 0; i < parts.Length; i++) home[i] = parts[i].color;
                }
                for (int i = 0; i < parts.Length; i++)
                    if (parts[i]) parts[i].color = dimmed ? home[i] * Off : home[i];
            }
        }

        /// <summary>Hạ mọi thứ bên trong nút 5 px khi đang bấm, rồi trả về chỗ cũ.</summary>
        public sealed class Press : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
        {
            const float Drop = 5;
            Vector3[] origin;
            Transform[] children;
            void Cache()
            {
                if (children != null) return;
                children = new Transform[transform.childCount];
                origin = new Vector3[children.Length];
                for (int i = 0; i < children.Length; i++) { children[i] = transform.GetChild(i); origin[i] = children[i].localPosition; }
            }
            void Shift(float dy)
            {
                Cache();
                for (int i = 0; i < children.Length; i++)
                    if (children[i]) children[i].localPosition = origin[i] + Vector3.down * dy;
            }
            public void OnPointerDown(PointerEventData e) { if (GetComponent<Button>()?.IsInteractable() ?? false) Shift(Drop); }
            public void OnPointerUp(PointerEventData e) => Shift(0);
            void OnDisable() { if (children != null) Shift(0); }
        }

        /// <summary>
        /// Phủ art nút đã duyệt (Resources/Buttons, chat 002) lên một Image có sẵn:
        /// gold = chính, blue = phụ, slate = back/huỷ, red = nguy hiểm. Bản bấm/tắt
        /// đổi sprite. UI-006 sẽ chuyển dần các nút cũ sang hàm này.
        /// </summary>
        public static void ButtonArt(Image image, string color)
        {
            var normal = Resources.Load<Sprite>("ButtonsV2/" + color);
            if (!image || !normal) return;
            // Bo ButtonsV2 ve hep 260x150 va khoa 125px moi dau (doc/UI_ART_SPEC.md muc 1),
            // nen 9-slice tu giu dung hinh o moi co; khong con phai scale theo chieu cao.
            image.sprite = normal; image.color = Color.white; image.type = Image.Type.Sliced; image.preserveAspect = false;
            image.pixelsPerUnitMultiplier = 1;
            var button = image.GetComponent<Button>(); if (!button) return;
            button.transition = Selectable.Transition.SpriteSwap;
            button.spriteState = new SpriteState
            {
                pressedSprite = Resources.Load<Sprite>("ButtonsV2/" + color + "-pressed"),
                disabledSprite = Resources.Load<Sprite>("ButtonsV2/disabled"),
            };
        }

        /// <summary>Phủ art ruy-băng/biển tên lên một Image tĩnh (không phải nút).</summary>
        public static void BannerArt(Image image, Banner banner, float height)
        {
            var sprite = SpriteFor(banner);
            if (!image || !sprite) return;
            image.sprite = sprite; image.color = Color.white; image.type = Image.Type.Sliced; image.preserveAspect = false;
            image.pixelsPerUnitMultiplier = sprite.rect.height / Mathf.Max(1f, height);
        }

        /// <summary>Nút back tròn đã duyệt (Resources/UiRefresh/back), đặt ở góc trên trái.</summary>
        public static Button Back(Transform parent, Vector2 position, UnityAction onClick)
            => Round(parent, "back", "blue", position, BackSize, onClick);

        /// <summary>
        /// Nút tròn nhỏ (back, ☰, đóng, ‹ ›): mặt nút ở Resources/SmallButtons, ký hiệu rời
        /// ở Resources/Glyphs — cùng cỡ chuẩn 150 (doc/UI_ART_SPEC.md mục 2). Tách mặt và
        /// ký hiệu để một mặt dùng lại cho nhiều ký hiệu, khỏi vẽ lại cả bộ.
        /// </summary>
        public static Button Round(Transform parent, string glyph, string colour, Vector2 position, float size, UnityAction onClick)
        {
            var r = new GameObject("Round-" + glyph, typeof(RectTransform), typeof(Image), typeof(Button)).GetComponent<RectTransform>();
            r.SetParent(parent, false);
            BattleUiSkin.Place(r, Vector2.one * .5f, position, Vector2.one * size);
            var image = r.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>("SmallButtons/" + colour) ?? BattleUiSkin.Back;
            image.preserveAspect = true;

            var mark = new GameObject("Glyph", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            mark.SetParent(r, false);
            BattleUiSkin.Place(mark, Vector2.one * .5f, Vector2.zero, Vector2.one * (size * .52f));
            var markImage = mark.GetComponent<Image>();
            markImage.sprite = Resources.Load<Sprite>("Glyphs/" + glyph);
            markImage.preserveAspect = true; markImage.raycastTarget = false;
            markImage.enabled = markImage.sprite;

            var button = r.GetComponent<Button>(); button.targetGraphic = image;
            if (onClick != null) button.onClick.AddListener(onClick);
            button.transition = Selectable.Transition.SpriteSwap;
            button.spriteState = new SpriteState { pressedSprite = Resources.Load<Sprite>("SmallButtons/" + colour + "-pressed") };
            r.gameObject.AddComponent<Press>();
            r.gameObject.AddComponent<DimWhenDisabled>();
            return button;
        }
    }
}
