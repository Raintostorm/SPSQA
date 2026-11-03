using Project.Battle;
using UnityEngine;
namespace Project.UI
{
    public static class EffectBuffArt
    {
        static Texture2D atlas;static readonly Sprite[] icons=new Sprite[15];
        public static Sprite Get(EffectBuffKind kind)
        {
            int i=(int)kind;
            if(icons[i])return icons[i];
            if(i>=10){var names=new[]{"ember","mana-flow","wave-income","mythic-might","soul-harvest"};var data=Resources.Load<TextAsset>("EffectBuffs/"+names[i-10]);if(!data)return null;var texture=new Texture2D(2,2);texture.LoadImage(data.bytes);icons[i]=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.one*.5f,100);return icons[i];}
            if(kind==EffectBuffKind.BloodTile||kind==EffectBuffKind.GoldTile)
            {
                var texture=Resources.Load<Texture2D>(kind==EffectBuffKind.BloodTile?"EffectBuffs/blood-tile-v2":"EffectBuffs/gold-tile-v2");
                if(texture)
                {
                    icons[i]=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.one*.5f,100);return icons[i];
                }
            }

            if(!atlas){var data=Resources.Load<TextAsset>("EffectBuffs/catalog");if(!data)return null;atlas=new Texture2D(2,2);atlas.LoadImage(data.bytes);}
            if(!icons[i]){float sx=atlas.width/1536f,sy=atlas.height/1024f;int row=i%5;icons[i]=Sprite.Create(atlas,new Rect((i<5?35:795)*sx,(1024-306-row*161)*sy,200*sx,137*sy),Vector2.one*.5f,100);}
            return icons[i];
        }
    }
}
