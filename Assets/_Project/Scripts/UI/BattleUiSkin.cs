using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Shared sliced geometry: corners keep their radius at every aspect ratio.
    public static class BattleUiSkin
    {
        static Sprite navy, blue, orange, dark, purple, red;
        static Sprite green, gold, neutral;
        public static Sprite Neutral => neutral ?? (neutral = Make(Color.white,Color.white));
        public static Sprite Green => green ?? (green = Make(new Color32(109,218,170,255),new Color32(17,187,129,255)));
        public static Sprite Gold => ApprovedUiArt.Get("gold",true) ?? ButtonFamilySkin.Get(2) ?? gold ?? (gold = Make(new Color32(255,219,74,255),new Color32(255,200,31,255)));
        public static Sprite Red => ButtonFamilySkin.Get(5) ?? red ?? (red = Make(new Color32(255,92,110,255), new Color32(221,35,67,255)));
        public static Sprite Dark => dark ?? (dark = Make(new Color32(21,31,46,255), new Color32(21,31,46,255)));
        public static Sprite Purple => ButtonFamilySkin.Get(3) ?? purple ?? (purple = Make(new Color32(166,100,232,255), new Color32(126,62,196,255)));
        public static Sprite Navy => ApprovedUiArt.Get("navy",true) ?? navy ?? (navy = Make(new Color32(15,39,57,255), new Color32(10,27,40,255)));
        public static Sprite Blue => ApprovedUiArt.Get("cyan",true) ?? blue ?? (blue = Make(new Color32(43,174,244,255), new Color32(0,119,226,255)));
        public static Sprite Orange => ApprovedUiArt.Get("gold",true) ?? ButtonFamilySkin.Get(0) ?? orange ?? (orange = Make(new Color32(255,205,38,255), new Color32(255,144,0,255)));
        public static Sprite Summon => ButtonFamilySkin.Get(1) ?? Purple;
        public static Sprite Filter => ButtonFamilySkin.Get(4) ?? Dark;
        public static Sprite Back => ApprovedUiArt.Get("back") ?? ButtonFamilySkin.Get(6) ?? Blue;
        public static Sprite Info => ApprovedUiArt.Get("info") ?? ButtonFamilySkin.Get(7) ?? Navy;
        static Sprite Make(Color upper, Color lower)
        {
            const int n=128, radius=24;
            var tex=new Texture2D(n,n,TextureFormat.RGBA32,false);
            var pixels=new Color[n*n];
            for(int y=0;y<n;y++) for(int x=0;x<n;x++)
            {
                float dx=Mathf.Max(Mathf.Abs(x-63.5f)-(64-radius),0);
                float dy=Mathf.Max(Mathf.Abs(y-63.5f)-(64-radius),0);
                float distance=Mathf.Sqrt(dx*dx+dy*dy);
                var c=distance>radius-6 ? Color.black : y>76 ? upper : lower;
                if(distance<radius-6 && y>115) c=Color.Lerp(upper,Color.white,.35f);
                c.a=Mathf.Clamp01(radius-distance);
                pixels[y*n+x]=c;
            }
            tex.SetPixels(pixels);tex.Apply();tex.wrapMode=TextureWrapMode.Clamp;
            return Sprite.Create(tex,new Rect(0,0,n,n),Vector2.one*.5f,100,0,SpriteMeshType.FullRect,new Vector4(26,26,26,26));
        }
        public static void Place(RectTransform r, Vector2 anchor, Vector2 pos, Vector2 size)
        { r.anchorMin=r.anchorMax=r.pivot=anchor;r.anchoredPosition=pos;r.sizeDelta=size; }
        public static void Skin(Image image, Sprite sprite)
        { if(!image)return;image.sprite=sprite;image.type=Image.Type.Sliced;image.pixelsPerUnitMultiplier=1;image.color=Color.white;image.preserveAspect=false;
            if(sprite==ButtonFamilySkin.Get(6)||sprite==ButtonFamilySkin.Get(7)){image.type=Image.Type.Simple;image.preserveAspect=true;} }
        public static void Button(Button button, Sprite sprite, string label)
        {
            if(!button)return;Skin(button.GetComponent<Image>(),sprite);
            button.transition=Selectable.Transition.ColorTint;
            var colors=button.colors;colors.pressedColor=new Color(.75f,.85f,.95f);colors.disabledColor=new Color(.55f,.55f,.55f);button.colors=colors;ButtonFamilySkin.States(button);
            var text=button.GetComponentInChildren<Text>();if(text){text.text=label;text.color=Color.white;text.fontSize=44;}
        }
        public static void Icon(Button button,string name,bool iconOnly=false)
        {
            if(!button)return;
            var sprite=Resources.Load<Sprite>("SharedUI/"+name);
            if(!sprite)return;
            var legacy=button.transform.Find("Pictogram");if(legacy)legacy.gameObject.SetActive(false);
            var existing=button.transform.Find("ActionIcon");
            var art=existing?existing.GetComponent<Image>():new GameObject("ActionIcon",typeof(RectTransform),typeof(Image)).GetComponent<Image>();
            art.transform.SetParent(button.transform,false);art.sprite=sprite;art.preserveAspect=true;art.raycastTarget=false;art.color=Color.white;
            var size=button.GetComponent<RectTransform>().sizeDelta;
            float side=Mathf.Min(size.y*.72f,120);
            Place(art.rectTransform,new Vector2(iconOnly?.5f:0,.5f),new Vector2(iconOnly?0:20,0),new Vector2(side,side));
            var text=button.GetComponentInChildren<Text>(true);
            if(text){text.enabled=!iconOnly;if(!iconOnly){text.rectTransform.anchorMin=new Vector2(0,0);text.rectTransform.anchorMax=Vector2.one;text.rectTransform.offsetMin=new Vector2(side+34,8);text.rectTransform.offsetMax=new Vector2(-12,-8);text.resizeTextForBestFit=true;text.resizeTextMinSize=24;text.resizeTextMaxSize=text.fontSize;}}
        }
    }
}
