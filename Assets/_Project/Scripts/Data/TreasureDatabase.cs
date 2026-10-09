using UnityEngine;

namespace Project.Data
{
    /// <summary>
    /// Cả 30 vật phẩm. Một asset duy nhất, sinh từ tools/treasure_items.json.
    ///
    /// Vì sao là ScriptableObject chứ không đọc JSON lúc chạy: Sprite không
    /// nằm trong JSON được. Asset thì giữ được tham chiếu tới icon đã nhập.
    /// </summary>
    [CreateAssetMenu(menuName = "SpinSquad/Treasure Database")]
    public sealed class TreasureDatabase : ScriptableObject
    {
        public TreasureItem[] Items = System.Array.Empty<TreasureItem>();

        public TreasureItem Find(string id)
        {
            foreach (var it in Items)
                if (it.Id == id) return it;
            return null;
        }

        /// <summary>Các món thuộc một bậc. Dùng khi roll ra bậc rồi mới chọn món.</summary>
        public System.Collections.Generic.List<TreasureItem> OfTier(Rank tier)
        {
            var list = new System.Collections.Generic.List<TreasureItem>();
            foreach (var it in Items)
                if (it.Tier == tier) list.Add(it);
            return list;
        }
    }
}
