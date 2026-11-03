using UnityEngine;

namespace Project.UI
{
    static class ResultScreenArt
    {
        static Sprite[] sprites;
        public static Sprite Get(int index)
        {
            if(sprites==null)
            {
                var texture=Resources.Load<Texture2D>("ResultScreen/atlas");if(!texture)return null;
                var regions=new[]{new Rect(29,668,708,214),new Rect(793,669,707,216),
                    new Rect(31,74,743,391),new Rect(833,79,659,310)};
                sprites=new Sprite[4];
                for(int i=0;i<4;i++)sprites[i]=Sprite.Create(texture,regions[i],Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
            }
            return sprites[index];
        }
    }
}
