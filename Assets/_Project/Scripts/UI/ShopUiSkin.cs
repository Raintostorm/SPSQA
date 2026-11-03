using UnityEngine;
using UnityEngine.UI;
namespace Project.UI
{
    static class ShopUiSkin
    {
        static Sprite[] sprites;
        public static void Apply(Image image,int index)
        {
            if(sprites==null)
            {
                var texture=Resources.Load<Texture2D>("ShopV2/kit");if(!texture)return;
                var rects=new[]{new Rect(164,280,496,663),new Rect(877,280,496,663),new Rect(51,83,697,139),new Rect(798,90,685,124)};
                sprites=new Sprite[4];
                for(int i=0;i<4;i++)sprites[i]=Sprite.Create(texture,rects[i],Vector2.one*.5f,100,0,SpriteMeshType.FullRect,i<2?new Vector4(45,145,45,45):new Vector4(100,45,100,45));
            }
            image.sprite=sprites[index];image.type=Image.Type.Sliced;image.pixelsPerUnitMultiplier=1.5f;image.color=Color.white;
        }
    }
}
