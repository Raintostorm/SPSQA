using UnityEngine;
namespace Project.UI
{
    // Dedicated rarity card art. Avoid stretching a shared blue button into a card.
    public static class EffectBuffCardArt
    {
        static readonly Sprite[] cards=new Sprite[2];
        public static Sprite Get(bool legendary)
        {
            int i=legendary?1:0;if(cards[i])return cards[i];
            var data=Resources.Load<TextAsset>("EffectBuffs/"+(legendary?"legend-card":"epic-card"));
            if(!data)return legendary?BattleUiSkin.Gold:BattleUiSkin.Purple;
            var texture=new Texture2D(2,2,TextureFormat.RGBA32,false);texture.LoadImage(data.bytes);texture.filterMode=FilterMode.Bilinear;texture.wrapMode=TextureWrapMode.Clamp;
            return cards[i]=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
        }
    }
}
