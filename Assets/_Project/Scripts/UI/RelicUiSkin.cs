using UnityEngine;
using UnityEngine.UI;
namespace Project.UI
{
    // Atlas alpha is original ImageGen output; only Sprite.Create rectangles are used.
    static class RelicUiSkin
    {
        static Sprite[] sprites;
        public static Sprite Get(int index)
        {
            if(sprites==null)
            {
                var t=Resources.Load<Texture2D>("RelicSkin/atlas");if(!t)return null;
                var rects=new[]{new Rect(68,1019,509,187),new Rect(655,1019,511,188),new Rect(168,655,321,309),new Rect(746,654,320,310),new Rect(216,335,187,267),new Rect(830,335,188,266),new Rect(195,48,256,253),new Rect(602,118,578,100)};
                sprites=new Sprite[8];
                for(int i=0;i<8;i++)sprites[i]=Sprite.Create(t,rects[i],Vector2.one*.5f,100,0,SpriteMeshType.FullRect,i>=4&&i<=6?Vector4.zero:i==7?new Vector4(50,45,50,45):new Vector4(70,70,70,70));
            }
            return sprites[index];
        }
        public static void Apply(Image image,int index,Color tint)
        {
            var sprite=Get(index);if(!sprite)return;
            image.sprite=sprite;image.type=index>=4&&index<=6?Image.Type.Simple:Image.Type.Sliced;
            image.color=tint;image.preserveAspect=index>=4&&index<=6;
        }
    }
}
