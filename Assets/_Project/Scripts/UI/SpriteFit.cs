using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Đặt một sprite theo phần NHÌN THẤY của nó, không theo cả khung PNG.
    ///
    /// Head crop và icon relic có lề trong suốt khác nhau (đầu Crescent Mage chỉ
    /// chiếm giữa ảnh, Royal Hero chiếm gần hết), nên cùng một ô mà con to con
    /// nhỏ, con nổi con chìm. Texture không bật Read/Write nên lúc chạy không đo
    /// được pixel: lề được đo sẵn bằng tools/build_sprite_bounds.py.
    /// </summary>
    public static class SpriteFit
    {
        static Dictionary<string, Rect> bounds;

        static Rect Visible(Sprite sprite)
        {
            if (bounds == null)
            {
                bounds = new Dictionary<string, Rect>();
                var data = Resources.Load<TextAsset>("UiSpriteBounds");
                if (data)
                    foreach (var line in data.text.Split('\n'))
                    {
                        var f = line.Trim().Split(' ');
                        if (f.Length != 5) continue;
                        float v(int i) => float.Parse(f[i], CultureInfo.InvariantCulture);
                        bounds[f[0]] = Rect.MinMaxRect(v(1), v(2), v(3), v(4));
                    }
            }
            return bounds.TryGetValue(sprite.name, out var r) ? r : new Rect(0, 0, 1, 1);
        }

        /// <summary>
        /// Vẽ <paramref name="sprite"/> trong ô <paramref name="box"/> (tâm ô = gốc của parent).
        /// fillWidth/fillHeight: phần nhìn thấy chiếm tối đa bao nhiêu ô. bottom: đáy hình
        /// chạm đáy ô (head cắt ngang vai), không thì căn giữa (icon).
        /// </summary>
        public static Image Place(Transform parent, Sprite sprite, Vector2 box, float fillWidth, float fillHeight, bool bottom)
        {
            var image = new GameObject("Fitted", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(parent, false); image.raycastTarget = false;
            if (!sprite) { image.enabled = false; return image; }
            image.sprite = sprite;
            var visible = Visible(sprite);
            Vector2 full = sprite.rect.size;
            float k = Mathf.Min(box.x * fillWidth / Mathf.Max(1, visible.width * full.x),
                                box.y * fillHeight / Mathf.Max(1, visible.height * full.y));
            var size = full * k;
            float x = -(visible.center.x - .5f) * size.x;
            float y = bottom ? -box.y * .5f - (visible.yMin - .5f) * size.y : -(visible.center.y - .5f) * size.y;
            BattleUiSkin.Place(image.rectTransform, Vector2.one * .5f, new Vector2(x, y), size);
            return image;
        }
    }
}
