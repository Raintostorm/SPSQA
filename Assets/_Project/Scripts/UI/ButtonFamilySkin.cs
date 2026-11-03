using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Regions measured against the original transparent ImageGen atlas.
    // Text/icons remain separate; wide backgrounds use 9-slice borders.
    static class ButtonFamilySkin
    {
        static Sprite[] sprites;
        public static Sprite Get(int index)
        {
            if(sprites==null)
            {
                var t=Resources.Load<Texture2D>("ButtonFamilies/atlas");if(!t)return null;
                var rects=new[]{
                    new Rect(67,762,681,187),new Rect(798,763,675,195),
                    new Rect(106,554,602,153),new Rect(832,549,607,166),
                    new Rect(82,324,651,151),new Rect(1028,307,216,195),
                    new Rect(290,47,232,231),new Rect(1018,47,231,232)};
                var borders=new[]{new Vector4(65,45,65,45),new Vector4(80,45,80,85),
                    new Vector4(60,42,60,42),new Vector4(60,45,60,45),
                    new Vector4(50,40,50,40),new Vector4(48,48,48,48),Vector4.zero,Vector4.zero};
                sprites=new Sprite[rects.Length];
                for(int i=0;i<sprites.Length;i++)
                {
                    sprites[i]=Sprite.Create(t,rects[i],Vector2.one*.5f,100,0,SpriteMeshType.FullRect,borders[i]);
                    sprites[i].name="ButtonFamily"+i;
                }
            }
            return sprites[index];
        }
        public static void Apply(Image image,int index)
        {
            var sprite=Get(index);if(!image||!sprite)return;
            image.sprite=sprite;image.color=Color.white;image.pixelsPerUnitMultiplier=1;
            image.type=index>=6?Image.Type.Simple:Image.Type.Sliced;
            image.preserveAspect=index>=6;
        }
        public static void States(Button button)
        {
            if(!button)return;
            button.transition=Selectable.Transition.ColorTint;
            var c=button.colors;c.normalColor=Color.white;c.highlightedColor=new Color(.96f,.97f,1);
            c.pressedColor=new Color(.72f,.76f,.84f);c.selectedColor=new Color(.9f,.94f,1);
            c.disabledColor=new Color(.58f,.56f,.6f);c.fadeDuration=.08f;button.colors=c;
        }
    }
}
