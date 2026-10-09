using System;
using System.Collections.Generic;

namespace Project.Meta
{
    /// <summary>
    /// Toàn bộ tiến độ lâu dài, ở dạng ghi ra file được.
    ///
    /// Thuần dữ liệu, không hàm nào ngoài tra cứu. Mọi luật chơi nằm ở nơi
    /// khác — ở đây chỉ là tờ giấy ghi số.
    ///
    /// Dùng List chứ không Dictionary: JsonUtility của Unity KHÔNG
    /// serialize được Dictionary, và nó im lặng ghi ra object rỗng chứ
    /// không báo lỗi. Một buổi debug trôi đi vì đúng chuyện đó.
    /// </summary>
    [Serializable]
    public sealed class SaveData
    {
        /// <summary>Tăng khi đổi cấu trúc file, để còn biết đường nâng cấp.</summary>
        public int Version = 1;

        // Von ban dau de co cai ma thu ngay, khong phai cay mot tieng moi
        // mo duoc ruong lan dau. Doi o day thi chi an voi file save MOI —
        // file da ton tai giu nguyen so cu.
        public int Gold = 15000;
        public int TreasureKeys = 35;
        public int HighestLevelUnlocked = 1;

        /// <summary>
        /// Số lượt roll kể từ lần trúng Legendary gần nhất.
        ///
        /// PHẢI nằm trong file, không được để trong RAM: người chơi quay 79
        /// lượt rồi thoát game, mở lại mà bộ đếm về 0 là mất trắng 79 lượt —
        /// và mất âm thầm, họ chỉ thấy "sao mãi không ra".
        /// </summary>
        public int RollsSinceLegendary;

        public List<OwnedItem> Items = new();
        public List<UnitUpgrade> Units = new();

        [Serializable]
        public sealed class OwnedItem
        {
            public string Id;
            public int Level = 1;

            /// <summary>Mảnh đã gom, chưa đủ để lên cấp kế.</summary>
            public int Shards;
        }

        [Serializable]
        public sealed class UnitUpgrade
        {
            public string UnitId;
            public int Level = 1;
        }

        public OwnedItem FindItem(string id)
        {
            foreach (var it in Items)
                if (it.Id == id) return it;
            return null;
        }

        public UnitUpgrade FindUnit(string unitId)
        {
            foreach (var u in Units)
                if (u.UnitId == unitId) return u;
            return null;
        }
    }
}
