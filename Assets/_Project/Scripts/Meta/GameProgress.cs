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
            get => Mathf.Max(1, SaveSystem.Current.HighestLevelUnlocked);
            private set
            {
                SaveSystem.Current.HighestLevelUnlocked = Mathf.Max(1, value);
                SaveSystem.Save();
            }
        }

        public static int Gold => SaveSystem.Current.Gold;
        public static int Keys => SaveSystem.Current.TreasureKeys;

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
        public static void Clear(int level, Data.GameBalance balance)
        {
            var save = SaveSystem.Current;
            var firstTime = level + 1 > save.HighestLevelUnlocked;

            if (balance != null)
            {
                save.Gold += firstTime ? balance.GoldPerLevelClear
                                       : balance.GoldPerLevelReplay;
                save.TreasureKeys += firstTime ? balance.KeysPerLevelClear
                                               : balance.KeysPerLevelReplay;
            }

            if (firstTime) save.HighestLevelUnlocked = level + 1;
            SaveSystem.Save();

            Debug.Log($"[GameProgress] Qua màn {level}" +
                      (firstTime ? " (lần đầu)" : " (chơi lại)") +
                      $" — {save.Gold} vàng, {save.TreasureKeys} chìa.");
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
