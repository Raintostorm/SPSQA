using UnityEngine;
namespace Project.UI
{
    public static class MythicStoneArt
    {
        static Sprite icon;
        public static Sprite Icon
        {
            get
            {
                if(icon)return icon;
                var data=Resources.Load<TextAsset>("Summon/mythic-stone");if(!data)return null;
                var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);texture.LoadImage(data.bytes);texture.filterMode=FilterMode.Bilinear;
                return icon=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.one*.5f,100);
            }
        }
    }
}
