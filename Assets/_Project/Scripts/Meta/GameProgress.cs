using System.Collections.Generic;
using UnityEngine;

namespace Project.Meta
{
    /// <summary>
    /// Tiến độ lâu dài: màn nào đã mở, vàng và chìa đang có bao nhiêu.
    ///
    /// Chỉ là lớp mỏng trên <see cref="SaveSystem"/>. Giữ lại vì năm chỗ
    /// trong game đang gọi tên này, và vì nó che đi chuyện dữ liệu nằm ở
    /// đâu — hôm nào đổi từ file sang đám mây thì sửa SaveSystem, không ai
    /// khác phải biết.
    ///
    /// Trước đây lưu bằng PlayerPrefs. Bỏ khi thêm Gold và Keys: PlayerPrefs
    /// là mấy chục khoá rời rạc, không xem được, không backup được. Một file
    /// JSON thì mở ra đọc bằng mắt. (Handoff/03 mục 6 đã nói từ đầu.)
    /// </summary>
    public static class GameProgress
    {
        /// <summary>Màn cao nhất được phép chơi, đếm từ 1.</summary>
        public static int Unlocked
        {
            get => Mathf.Max(2, SaveSystem.Current.HighestLevelUnlocked);
            private set
            {
                SaveSystem.Current.HighestLevelUnlocked = Mathf.Max(1, value);
                SaveSystem.Save();
            }
        }

        public static int Gold => SaveSystem.Current.Gold;
        public static int Keys => SaveSystem.Current.TreasureKeys;
        public static int Gems => SaveSystem.Current.Gems;
        public static int GemsForLevel(int level) => level==1?80:level==2?250:0;
        public static void AddGems(int amount)
        {
            if(amount<=0)return;
            SaveSystem.Current.Gems=checked(SaveSystem.Current.Gems+amount);
            SaveSystem.Save();
        }
        public static bool SpendGems(int amount)
        {
            if(amount<0 || Gems<amount)return false;
            SaveSystem.Current.Gems-=amount;SaveSystem.Save();return true;
        }

        /// <summary>
        /// Màn đang chơi. Trang chủ đặt, màn trận đọc.
        ///
        /// KHÔNG lưu vào file: nó là trạng thái của phiên chơi này, không
        /// phải tiến độ. Lưu nó thì mở game lần sau tự nhảy vào màn giữa
        /// chừng mà người chơi không chọn.
        /// </summary>
        public static int CurrentLevel { get; set; } = 1;

        /// <summary>
        /// Qua màn n → mở màn n+1, cộng vàng và chìa.
        ///
        /// Thưởng chơi lại tách riêng khỏi thưởng lần đầu. Hai con số, vì
        /// chơi lại được thưởng đủ thì vàng thành vô hạn — xem
        /// outdata/PROMPT_HOMEPAGE.md mục 5b. Hiện cả hai bằng nhau đúng
        /// như đã chốt; hôm nào thấy lạm phát thì hạ đúng một con số.
        /// </summary>
        public static ClearReward Clear(int level, Data.GameBalance balance)
        {
            var reward = new ClearReward();
            reward.Saved = SaveSystem.Transact(save =>
            {
                reward.FirstTime = level + 1 > save.HighestLevelUnlocked;
                if (balance != null)
                {
                    reward.Gold = balance.GoldForLevel(level, reward.FirstTime);
                    reward.Keys = balance.KeysForLevel(level, reward.FirstTime);
                }
                // Award on every successful clear, including replays. No retroactive grants.
                reward.Gems = GemsForLevel(level);

                save.Gold = checked(save.Gold + reward.Gold);
                save.TreasureKeys = checked(save.TreasureKeys + reward.Keys);
                save.Gems = checked(save.Gems + reward.Gems);
                if (reward.FirstTime) save.HighestLevelUnlocked = level + 1;
                return true;
            });
            if (!reward.Saved) reward = new ClearReward { FirstTime = reward.FirstTime };

            Debug.Log($"[GameProgress] Qua màn {level}" +
                      (reward.FirstTime ? " (lần đầu)" : " (chơi lại)") +
                      (reward.Saved ? $" — +{reward.Gold} vàng, +{reward.Keys} chìa, +{reward.Gems} gem."
                                    : " — KHÔNG ghi được save, chưa phát thưởng."));
            return reward;
        }

        /// <summary>Thứ một lần qua màn đã thật sự phát. Màn kết quả chỉ ĐỌC cái này.</summary>
        public struct ClearReward
        {
            public int Gold, Keys, Gems;
            public bool FirstTime;
            /// <summary>false = ghi đĩa hỏng, save đã được trả về như trước, không phát gì.</summary>
            public bool Saved;
        }

        /// <summary>
        /// Mở rương: trừ <paramref name="count"/> chìa, quay, phát quà, đếm bảo
        /// hiểm — tất cả trong MỘT giao dịch, ghi đĩa đúng một lần.
        ///
        /// Thiếu chìa, kho thiếu bậc hoặc ghi đĩa hỏng → trả false, chìa và
        /// bảo hiểm còn nguyên, <paramref name="results"/> rỗng.
        /// </summary>
        public static bool TryOpenChest(int count, Data.TreasureDatabase db, Data.GameBalance balance,
                                        System.Random rng, out List<TreasureRoll.Result> results)
        {
            var got = new List<TreasureRoll.Result>();
            var ok = count > 0 && SaveSystem.Transact(save =>
            {
                if (save.TreasureKeys < count) return false;
                save.TreasureKeys -= count;
                return TreasureRoll.RollInto(save, count, db, balance, rng, got);
            });
            results = ok ? got : new List<TreasureRoll.Result>();
            return ok;
        }

        /// <summary>Trừ vàng nếu đủ. Trả false và KHÔNG đụng ví nếu thiếu.</summary>
        public static bool SpendGold(int amount)
        {
            var save = SaveSystem.Current;
            if (amount < 0 || save.Gold < amount) return false;
            save.Gold -= amount;
            SaveSystem.Save();
            return true;
        }

        /// <summary>Trừ chìa nếu đủ. Trả false và KHÔNG đụng ví nếu thiếu.</summary>
        public static bool SpendKeys(int amount)
        {
            var save = SaveSystem.Current;
            if (amount < 0 || save.TreasureKeys < amount) return false;
            save.TreasureKeys -= amount;
            SaveSystem.Save();
            return true;
        }

        public static void ResetAll()
        {
            SaveSystem.Wipe();
            CurrentLevel = 1;
        }
    }
}
