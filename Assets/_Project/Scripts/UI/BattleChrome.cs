using UnityEngine;
using UnityEngine.UI;
namespace Project.UI
{
    public static class BattleChrome
    {
        static Sprite[] icons;
        static Sprite[] buttons;
        public static Sprite Part(int index)
        {
            if(buttons==null){var data=Resources.Load<TextAsset>("BattleChrome/buttons-v2");if(!data)return null;var tex=new Texture2D(2,2,TextureFormat.RGBA32,false);tex.LoadImage(data.bytes);tex.filterMode=FilterMode.Bilinear;tex.wrapMode=TextureWrapMode.Clamp;
                var rects=new[]{new Rect(36,101,244,213),new Rect(306,121,381,178),new Rect(707,100,236,217),new Rect(962,100,265,217),
                    new Rect(35,378,245,216),new Rect(306,398,383,181),new Rect(706,378,237,217),new Rect(962,378,265,218),
                    new Rect(34,661,248,216),new Rect(306,681,379,181),new Rect(706,662,238,215),new Rect(962,661,265,218),
                    new Rect(39,933,235,236),new Rect(382,956,191,204),new Rect(699,967,188,190),new Rect(987,946,216,218)};
                buttons=new Sprite[rects.Length];for(int i=0;i<rects.Length;i++){var r=rects[i];r.y=tex.height-r.y-r.height;buttons[i]=Sprite.Create(tex,r,Vector2.one*.5f,100,0,SpriteMeshType.FullRect,(i%4==1&&i<12)||i==12?new Vector4(40,40,40,40):Vector4.zero);}}
            return buttons[index];
        }
        public static void Button(Button button,int family)
        {
            if(family==0 || family==2){ApprovedUiArt.Action(button,"navy",family==0?"recipe":"inventory");return;}
            ApprovedUiArt.Apply(button.GetComponent<Image>(),"cyan",true);
            button.transition=Selectable.Transition.ColorTint;ButtonFamilySkin.States(button);
        }
        public static void Cell(Image image){image.sprite=Part(12);image.type=Image.Type.Sliced;image.color=Color.white;}
        public static Sprite Icon(int index)
        {
            if(icons==null){var data=Resources.Load<TextAsset>("BattleChrome/icons");if(!data)return null;var tex=new Texture2D(2,2,TextureFormat.RGBA32,false);tex.LoadImage(data.bytes);tex.filterMode=FilterMode.Bilinear;
                var top=new[]{new Rect(27,195,448,356),new Rect(498,165,360,392),new Rect(888,126,396,448),new Rect(1325,188,428,367),new Rect(1796,193,350,357)};
                icons=new Sprite[5];for(int i=0;i<5;i++){var r=top[i];r.y=tex.height-r.y-r.height;icons[i]=Sprite.Create(tex,r,Vector2.one*.5f,100);}}
            return icons[index];
        }
        public static void Frame(Image image)
        {
            BattleUiSkin.Skin(image,BattleUiSkin.Navy);var outline=image.GetComponent<Outline>();if(!outline)outline=image.gameObject.AddComponent<Outline>();outline.effectColor=new Color32(104,148,183,255);outline.effectDistance=new Vector2(3,-3);outline.useGraphicAlpha=true;
        }
        public static Image Draw(Transform parent,int index,Vector2 pos,Vector2 size)
        {
            var image=new GameObject("Action pictogram",typeof(RectTransform),typeof(Image)).GetComponent<Image>();image.transform.SetParent(parent,false);image.sprite=ApprovedUiArt.Get(index==1?"dice":index==4?"close":index==2?"inventory":"recipe");image.preserveAspect=true;image.raycastTarget=false;BattleUiSkin.Place(image.rectTransform,Vector2.one*.5f,pos,size);return image;
        }
    }
}
