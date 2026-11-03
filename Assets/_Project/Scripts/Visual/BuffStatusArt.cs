using System.Collections.Generic;
using UnityEngine;
namespace Project.Visual
{
    public static class BuffStatusArt
    {
        static readonly Dictionary<string,Sprite> sprites=new();
        public static Sprite Get(string key)
        {
            if(sprites.TryGetValue(key,out var sprite))return sprite;
            var data=Resources.Load<TextAsset>("EffectBuffs/Vfx/"+key);if(!data)return null;
            var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);texture.LoadImage(data.bytes);texture.filterMode=FilterMode.Bilinear;texture.wrapMode=TextureWrapMode.Clamp;
            return sprites[key]=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.one*.5f,100);
        }
        public static Sprite Flame(float time)=>Get("burn-"+((int)(time*13)%8));
        public static Sprite Mana(float time)=>Get("mana-status-"+((int)(time*12)%8));
    }
}
