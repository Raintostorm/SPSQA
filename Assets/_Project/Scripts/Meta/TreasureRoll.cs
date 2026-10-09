using System.Collections.Generic;
using Project.Data;
using UnityEngine;

namespace Project.Meta
{
    /// <summary>
    /// Mở rương: một lượt gacha, bậc càng hiếm tỉ lệ càng thấp.
    ///
    /// Tĩnh và thuần theo nghĩa: cùng một trạng thái save và cùng một hạt
    /// giống thì ra cùng kết quả. Nhờ vậy kiểm được bằng cách chạy hàng vạn
    /// lần rồi đếm, không phải ngồi bấm.
    /// </summary>
    public static class TreasureRoll
    {
        public struct Result
        {
            public TreasureItem Item;
            public bool LeveledUp;
            public int NewLevel;

            /// <summary>Món này vừa nhận lần đầu.</summary>
            public bool IsNew;
        }

        /// <summary>Quay <paramref name="count"/> lượt. Trừ chìa trước khi gọi.</summary>
        public static List<Result> Roll(int count, TreasureDatabase db,
                                        GameBalance balance, System.Random rng)
        {
            var results = new List<Result>();
            if (db == null || balance == null || count <= 0) return results;

            var save = SaveSystem.Current;
            var floorTier = balance.TenRollFloor;
            var needFloor = count >= 10;

            for (var i = 0; i < count; i++)
            {
                var tier = PickTier(balance, save, rng);

                // Bao dam cua luot x10: neu den luot CUOI van chua co mon nao
                // dat nguong thi nang luot nay len. Nang luot cuoi chu khong
                // phai luot dau — nang dau thi nhung luot sau co the tu ra
                // mon xin hon, va bao dam thanh ra lam te di.
                if (needFloor && i == count - 1 && tier < floorTier)
                    tier = floorTier;
                if (tier >= floorTier) needFloor = false;

                // Bo dem bao hiem: dem tu lan trung Legendary gan nhat.
                save.RollsSinceLegendary =
                    tier == Rank.Legendary ? 0 : save.RollsSinceLegendary + 1;

                var pool = db.OfTier(tier);
                if (pool.Count == 0) continue;
                var item = pool[rng.Next(pool.Count)];

                results.Add(Grant(save, item));
            }

            SaveSystem.Save();
            return results;
        }

        static Rank PickTier(GameBalance b, SaveData save, System.Random rng)
        {
            // Bao hiem cung: cham nguong la ep ra Legendary. Kiem TRUOC khi
            // quay, khong thi co the quay trung roi lai ep them lan nua.
            if (b.LegendaryPity > 0 && save.RollsSinceLegendary >= b.LegendaryPity)
                return Rank.Legendary;

            var w = b.TreasureWeights;
            if (w == null || w.Length == 0) return Rank.Common;

            var total = 0;
            foreach (var x in w) total += Mathf.Max(0, x);
            if (total <= 0) return Rank.Common;

            var pick = rng.Next(total);
            for (var i = 0; i < w.Length; i++)
            {
                pick -= Mathf.Max(0, w[i]);
                if (pick < 0) return (Rank)i;
            }
            return Rank.Common;
        }

        /// <summary>
        /// Ghi món vào save. Chưa có thì nhận cấp 1; đã có thì cộng mảnh,
        /// đủ thì lên cấp.
        ///
        /// Trùng lặp là chuyện THƯỜNG XUYÊN: 100 lượt ra ~70 món Common cho
        /// 12 loại, tức mỗi loại ~5.8 bản. Không có luật xử lý thì từ lượt
        /// thứ hai trở đi người chơi chỉ thấy "lại nó".
        /// </summary>
        static Result Grant(SaveData save, TreasureItem item)
        {
            var owned = save.FindItem(item.Id);
            if (owned == null)
            {
                save.Items.Add(new SaveData.OwnedItem { Id = item.Id, Level = 1, Shards = 0 });
                return new Result { Item = item, IsNew = true, NewLevel = 1 };
            }

            if (owned.Level >= item.MaxLevel)
                return new Result { Item = item, NewLevel = owned.Level };

            owned.Shards++;
            var leveled = false;
            while (owned.Level < item.MaxLevel &&
                   owned.Shards >= TreasureItem.ShardsToNext(owned.Level))
            {
                owned.Shards -= TreasureItem.ShardsToNext(owned.Level);
                owned.Level++;
                leveled = true;
            }
            return new Result { Item = item, LeveledUp = leveled, NewLevel = owned.Level };
        }

        /// <summary>
        /// Tổng một chỉ số từ TẤT CẢ món đang sở hữu, ở đúng cấp của chúng.
        /// Đây là chỗ duy nhất biến kho đồ thành sức mạnh.
        /// </summary>
        public static float Total(TreasureStat stat, TreasureDatabase db)
        {
            if (db == null) return 0f;
            var sum = 0f;
            foreach (var owned in SaveSystem.Current.Items)
            {
                var item = db.Find(owned.Id);
                if (item != null) sum += item.ValueAt(stat, owned.Level);
            }
            return sum;
        }
    }
}
