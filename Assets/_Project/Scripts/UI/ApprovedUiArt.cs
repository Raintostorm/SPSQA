using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Individual alpha cuts of the approved sheet. Text and prices stay live UI.
    public static class ApprovedUiArt
    {
        static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();
        public static Texture StatTexture(string name)
        {
            var sprite = Get(name);
            return sprite ? sprite.texture : Resources.Load<Texture2D>("StatIcons/" + name);
        }
        public static Sprite Get(string name, bool sliced = false)
        {
            var key = name + (sliced ? ":sliced" : "");
            if (cache.TryGetValue(key, out var sprite)) return sprite;
            var data = Resources.Load<TextAsset>("UiRefresh/" + name);
            if (!data) return null;
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.LoadImage(data.bytes); texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            var border = sliced ? new Vector4(40, 40, 40, 40) : Vector4.zero;
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * .5f, 100, 0, SpriteMeshType.FullRect, border);
            sprite.name = "ApprovedUI/" + name; cache[key] = sprite; return sprite;
        }
        public static void Apply(Image image, string name, bool sliced = false)
        {
            if (!image) return;
            var sprite = Get(name, sliced); if (!sprite) return;
            image.sprite = sprite; image.color = Color.white;
            image.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
            image.preserveAspect = !sliced; image.pixelsPerUnitMultiplier = 1;
        }
        public static void Action(Button button, string background, string icon)
        {
            Apply(button.GetComponent<Image>(), background, true);
            var old = button.transform.Find("Approved icon");
            var image = old ? old.GetComponent<Image>() : new GameObject("Approved icon", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(button.transform, false); Apply(image, icon); image.raycastTarget = false;
            BattleUiSkin.Place(image.rectTransform, Vector2.one * .5f, Vector2.zero, button.GetComponent<RectTransform>().sizeDelta * .72f);
            ButtonFamilySkin.States(button);
        }
        public static void GoldStates(Button button)
        {
            Apply(button.GetComponent<Image>(), "gold", true);
            button.transition = Selectable.Transition.SpriteSwap;
            button.spriteState = new SpriteState { highlightedSprite = Get("gold", true), selectedSprite = Get("gold", true), pressedSprite = Get("pressed", true), disabledSprite = Get("disabled", true) };
        }
    }
}
