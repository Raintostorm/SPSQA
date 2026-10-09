using System;
using UnityEngine;

namespace Project.Data
{
    public enum TreasureStat
    {
        Atk = 0, Hp, AtkSpeed, CritDamage,   // kiểu NHÂN — % chỉ số gốc
        CritChance, LifeSteal, Armor,        // kiểu XÁC SUẤT — điểm tuyệt đối
    }

    /// <summary>
    /// Một vật phẩm kho báu. Cộng dồn THỤ ĐỘNG: có nó là cả đội được cộng.
    ///
    /// Sinh ra từ tools/treasure_items.json qua menu, không gõ tay 30 lần.
    /// </summary>
    [Serializable]
    public sealed class TreasureItem
    {
        public string Id;
        public string DisplayName;
        public Rank Tier = Rank.Common;
        public int MaxLevel = 10;
        public Sprite Icon;

        [Serializable]
        public struct Entry
        {
            public TreasureStat Stat;
            public float Base;

            /// <summary>Cộng thêm mỗi cấp. Luôn bằng 10% của Base.</summary>
            public float PerLevel;
        }

        public Entry[] Stats = Array.Empty<Entry>();

        /// <summary>
        /// Giá trị một chỉ số ở cấp <paramref name="level"/>.
        /// Cấp 1 = Base. Mỗi cấp sau cộng thêm PerLevel.
        /// </summary>
        public float ValueAt(TreasureStat stat, int level)
        {
            foreach (var e in Stats)
                if (e.Stat == stat)
                    return e.Base + e.PerLevel * (Mathf.Clamp(level, 1, MaxLevel) - 1);
            return 0f;
        }

        /// <summary>Số mảnh cần để lên cấp kế. Lên cấp N tốn N mảnh.</summary>
        public static int ShardsToNext(int level) => level + 1;
    }
}
