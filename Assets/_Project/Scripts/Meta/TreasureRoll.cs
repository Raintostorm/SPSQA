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
        public const int ShardsPerRoll = 1;
        public struct Result
        {
            public TreasureItem Item;
            public bool LeveledUp;
            public int NewLevel;

            /// <summary>Món này vừa nhận lần đầu.</summary>
            public bool IsNew;
            public int ShardsGranted;
        }

        /// <summary>
        /// Quay rồi ghi đĩa, KHÔNG trừ chìa. Chỉ còn để test và công cụ dùng;
        /// màn rương phải đi qua <see cref="GameProgress.TryOpenChest"/>.
        /// </summary>
        public static List<Result> Roll(int count, TreasureDatabase db,
                                        GameBalance balance, System.Random rng)
        {
            var results = new List<Result>();
            if (!RollInto(SaveSystem.Current, count, db, balance, rng, results)) return new List<Result>();
            SaveSystem.Save();
            return results;
        }

        /// <summary>
        /// Quay <paramref name="count"/> lượt vào <paramref name="save"/>, KHÔNG
        /// ghi đĩa. Trả false nếu một lượt rơi vào bậc không có món nào — khi
        /// đó save đã bị đổi dở dang, người gọi phải huỷ (SaveSystem.Transact
        /// tự làm việc này).
        ///
        /// Bản cũ cập nhật bộ đếm bảo hiểm TRƯỚC khi kiểm kho rồi lặng lẽ bỏ
        /// qua lượt đó: bảo hiểm 80 gặp kho Legendary trống thì tụt về 0 mà
        /// người chơi không nhận gì.
        /// </summary>
        public static bool RollInto(SaveData save, int count, TreasureDatabase db,
                                    GameBalance balance, System.Random rng, List<Result> results)
        {
            if (save == null || db == null || balance == null || count <= 0) return false;

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

                var pool = db.OfTier(tier);
                if (pool == null || pool.Count == 0) return false;

                // Bo dem bao hiem: dem tu lan trung Legendary gan nhat.
                save.RollsSinceLegendary =
                    tier == Rank.Legendary ? 0 : save.RollsSinceLegendary + 1;

                results.Add(Grant(save, pool[rng.Next(pool.Count)]));
            }
            return true;
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
        /// chỉ cộng mảnh; người chơi nâng thủ công bằng gold và mảnh.
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
                save.Items.Add(new SaveData.OwnedItem { Id = item.Id, Level = 1, Shards = ShardsPerRoll });
                return new Result { Item = item, IsNew = true, NewLevel = 1, ShardsGranted = ShardsPerRoll };
            }

            if (owned.Level >= item.MaxLevel)
                return new Result { Item = item, NewLevel = owned.Level };

            int granted = (int)System.Math.Min(ShardsPerRoll, (long)int.MaxValue - owned.Shards);
            owned.Shards += granted;
            return new Result { Item = item, NewLevel = owned.Level, ShardsGranted = granted };
        }

        // Initial manual-upgrade price; displayed before purchase. No save migration.
        public static int UpgradeCost(TreasureItem item, int level) => 1000 * ((int)item.Tier + 1) * Mathf.Max(1, level);
        public static bool Upgrade(TreasureItem item)
        {
            if (!TryUpgrade(SaveSystem.Current, item)) return false;
            SaveSystem.Save();
            return true;
        }
        public static bool TryUpgrade(SaveData save, TreasureItem item)
        {
            if (item == null) return false;
            var owned = save.FindItem(item.Id);
            if (owned == null || owned.Level >= item.MaxLevel) return false;
            int shards = TreasureItem.ShardsToNext(owned.Level), gold = UpgradeCost(item, owned.Level);
            if (owned.Shards < shards || save.Gold < gold) return false;
            save.Gold -= gold; owned.Shards -= shards; owned.Level++;
            return true;
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
