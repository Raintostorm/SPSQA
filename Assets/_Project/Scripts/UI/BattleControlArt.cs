using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Exact alpha-island cuts from the approved, text-free ImageGen atlas.
    static class BattleControlArt
    {
        static Sprite[] sprites;
        public static Sprite Get(int index)
        {
            if(sprites==null)
            {
                var texture=Resources.Load<Texture2D>("BattleControls/atlas");
                if(!texture)return null;
                var rects=new[]{new Rect(65,546,419,332),new Rect(613,541,339,324),
                    new Rect(1127,542,310,316),new Rect(48,160,496,198),
                    new Rect(585,160,497,198),new Rect(1139,111,339,318)};
                sprites=new Sprite[6];
                for(int i=0;i<6;i++)
                {
                    var border=i>=3?new Vector4(50,50,50,50):Vector4.zero;
                    sprites[i]=Sprite.Create(texture,rects[i],Vector2.one*.5f,100,0,SpriteMeshType.FullRect,border);
                    sprites[i].name="BattleControl"+i;
                }
            }
            return sprites[index];
        }
        public static void Skin(Image image,int index)
        {
            if(!image)return;
            image.sprite=Get(index);image.type=index>=3?Image.Type.Sliced:Image.Type.Simple;
            image.preserveAspect=index<3;image.color=Color.white;image.pixelsPerUnitMultiplier=1;
        }
    }
}
