using UnityEngine;
namespace Project.Data
{
    /// <summary>Recipe reference catalogue. Inventory/effect execution is intentionally separate.</summary>
    public static class BattleItemRecipes
    {
        public sealed class Entry
        {
            public readonly string Id,Name;public readonly Rect Art;
            public Entry(string id,string name,Rect art){Id=id;Name=name;Art=art;}
        }
        // Pixel rectangles in the approved concept sheet, measured from its top-left.
        public static readonly Entry[] Items={
            new("kunai","Kunai",new Rect(170,270,290,230)),
            new("mana_stone","Mana Stone",new Rect(595,275,270,225)),
            new("twin_fang","Twin Fang",new Rect(160,735,295,225)),
            new("runic_kunai","Runic Kunai",new Rect(590,735,295,225)),
            new("mana_heart","Mana Heart",new Rect(170,1115,285,200))};
        public static readonly int[][] Recipes={new[]{0,0,2},new[]{0,1,3},new[]{1,1,4}};
        public static string Effect(int type)=>type switch{
            0=>"ATK SPD +10%",1=>"Starting MP +20",2=>"ATK SPD ×1.5",
            3=>"ATK SPD +10% · Starting MP +20\nOn kill: slash another enemy for 20% ATK",
            4=>"Starting MP +40 · All mana gained +50%",_=>""};
        static Texture2D atlas;static readonly Sprite[] sprites=new Sprite[5];
        public static Sprite Icon(int index)
        {
            if(sprites[index])return sprites[index];
            if(!atlas){var data=Resources.Load<TextAsset>("BattleItems/approved-icons");if(!data)return null;atlas=new Texture2D(2,2,TextureFormat.RGBA32,false);atlas.LoadImage(data.bytes);atlas.filterMode=FilterMode.Bilinear;}
            var r=Items[index].Art;r.y=atlas.height-r.y-r.height;
            return sprites[index]=Sprite.Create(atlas,r,Vector2.one*.5f,100);
        }
    }
}
