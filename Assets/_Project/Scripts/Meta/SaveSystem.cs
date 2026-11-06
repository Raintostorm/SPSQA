using System;
using System.IO;
using UnityEngine;

namespace Project.Meta
{
    /// <summary>
    /// Đọc/ghi <see cref="SaveData"/> ra một file JSON.
    ///
    /// Vì sao bỏ PlayerPrefs (Handoff/03 mục 6): PlayerPrefs lưu từng khoá
    /// rời rạc. Thêm Gold, Keys, bộ đếm bảo hiểm, 30 món và 20 cấp nhân vật
    /// vào đó là mấy chục khoá rải rác, không xem được, không backup được,
    /// không chép sang máy khác được. Một file JSON thì mở ra đọc bằng mắt.
    ///
    /// Giữ MỘT bản trong bộ nhớ (<see cref="Current"/>) và ghi đĩa khi được
    /// bảo. Không ghi mỗi lần đổi một con số: quay x100 là 100 lần chạm đĩa.
    /// </summary>
    public static class SaveSystem
    {
        const string FileName = "save.json";
        const string BackupName = "save.bak.json";

        // Bản tốt gần nhất. File.Replace để lại nó sau mỗi lần ghi, nên
        // save.json hỏng thì vẫn còn một bản cách đúng một lần ghi.
        const string PreviousName = "save.prev.json";

        static SaveData current;

        static string Path => System.IO.Path.Combine(
            Application.persistentDataPath, FileName);

        static string BackupPath => System.IO.Path.Combine(
            Application.persistentDataPath, BackupName);

        static string PreviousPath => System.IO.Path.Combine(
            Application.persistentDataPath, PreviousName);

        static string TempPath => Path + ".tmp";

        /// <summary>Bản đang dùng. Tự nạp ở lần gọi đầu.</summary>
        public static SaveData Current
        {
            get
            {
                current ??= Load();
                if(current.AllyLevelResetRevision<1)
                {
                    // Keep a recovery snapshot before the explicitly requested reset.
                    try { if(File.Exists(Path) && !File.Exists(Path+".before-ally-reset"))File.Copy(Path,Path+".before-ally-reset"); }
                    catch(IOException e){Debug.LogWarning("[SaveSystem] Reset backup: "+e.Message);}
                    if(current.Units==null)current.Units=new System.Collections.Generic.List<SaveData.UnitUpgrade>();
                    foreach(var unit in current.Units)if(unit!=null)unit.Level=1;
                    current.AllyLevelResetRevision=1;
                    Save();
                }
                return current;
            }
        }

        /// <summary>
        /// Xoá bản trong bộ nhớ, KHÔNG đụng file.
        ///
        /// Cần vì biến static sống dai hơn scene: ai tắt "Reload Domain" cho
        /// nhanh thì bản của lần Play trước còn nguyên ở lần Play sau. Cùng
        /// một cái bẫy RunBuffs đã dính.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void DropCache() => current = null;

        /// <summary>
        /// Thử lần lượt save.json → .tmp → save.prev.json, lấy bản đầu tiên
        /// đọc được.
        ///
        /// Vì sao phải thử .tmp: nếu game bị giết đúng lúc save.json đã mất mà
        /// .tmp chưa kịp đổi tên, thì .tmp là bản MỚI NHẤT và còn nguyên. Bản
        /// cũ bỏ qua nó và trả về save trống — người chơi mất sạch.
        /// </summary>
        static SaveData Load()
        {
            if (TryRead(Path, out var data)) return Normalize(data);

            // save.json có mà hỏng: đổi tên để còn cứu tay, rồi mới thử bản khác.
            if (File.Exists(Path))
            {
                Debug.LogError($"[SaveSystem] Không đọc được {Path}. Đổi tên thành {BackupName}.");
                try
                {
                    if (File.Exists(BackupPath)) File.Delete(BackupPath);
                    File.Move(Path, BackupPath);
                }
                catch (IOException) { /* khong doi duoc ten thi cung khong lam gi hon */ }
            }

            foreach (var fallback in new[] { TempPath, PreviousPath })
            {
                if (!TryRead(fallback, out data)) continue;
                Debug.LogWarning($"[SaveSystem] Khôi phục từ {fallback}.");
                return Normalize(data);
            }
            return new SaveData();
        }

        /// <summary>
        /// Đọc một file. Rỗng, cụt giữa chừng hay không phải JSON đều trả
        /// false — FromJson trả null cho file rỗng, không ném lỗi, nên phải
        /// tự chặn, không thì NullReference nổ ở chỗ không liên quan.
        /// </summary>
        static bool TryRead(string path, out SaveData data)
        {
            data = null;
            if (!File.Exists(path)) return false;
            try { data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path)); }
            catch (Exception e) { Debug.LogWarning($"[SaveSystem] {path}: {e.Message}"); }
            return data != null;
        }

        /// <summary>
        /// Đọc được JSON chưa có nghĩa là save hợp lệ. Sửa những gì làm game
        /// nổ hoặc ra số âm: danh sách null, dòng null, cấp dưới 1, ví âm.
        /// Không đặt TRẦN cấp ở đây — trần là luật cân bằng, không phải của save.
        /// </summary>
        internal static SaveData Normalize(SaveData d)
        {
            d.Items ??= new System.Collections.Generic.List<SaveData.OwnedItem>();
            d.Units ??= new System.Collections.Generic.List<SaveData.UnitUpgrade>();
            d.DailyShopOffers ??= new System.Collections.Generic.List<SaveData.ShopOffer>();
            d.EncounteredMonsters ??= new System.Collections.Generic.List<string>();
            d.Items.RemoveAll(i => i == null || string.IsNullOrEmpty(i.Id));
            d.Units.RemoveAll(u => u == null || string.IsNullOrEmpty(u.UnitId));
            d.DailyShopOffers.RemoveAll(o => o == null);
            foreach (var i in d.Items) { i.Level = Math.Max(1, i.Level); i.Shards = Math.Max(0, i.Shards); }
            foreach (var u in d.Units) { u.Level = Math.Max(1, u.Level); u.Shards = Math.Max(0, u.Shards); }
            d.Gold = Math.Max(0, d.Gold);
            d.TreasureKeys = Math.Max(0, d.TreasureKeys);
            d.Gems = Math.Max(0, d.Gems);
            d.MythicStones = Math.Max(0, d.MythicStones);
            d.HighestLevelUnlocked = Math.Max(1, d.HighestLevelUnlocked);
            d.RollsSinceLegendary = Math.Max(0, d.RollsSinceLegendary);
            d.SummonsSinceLegendary = Math.Max(0, d.SummonsSinceLegendary);
            return d;
        }

        /// <summary>
        /// Ghi bản trong bộ nhớ xuống đĩa. Trả false nếu KHÔNG chắc đã ghi.
        ///
        /// Ghi ra .tmp trước, rồi File.Replace đổi chỗ .tmp với save.json trong
        /// một thao tác của hệ điều hành, đồng thời giữ bản cũ làm save.prev.json.
        /// Bản trước đây xoá save.json rồi mới đổi tên .tmp — giữa hai bước đó
        /// trên đĩa không có save nào, game bị giết đúng lúc ấy là mất sạch.
        /// </summary>
        public static bool Save()
        {
            if (current == null) return false;
            try
            {
                File.WriteAllText(TempPath, JsonUtility.ToJson(current, true));
                if (!File.Exists(Path)) { File.Move(TempPath, Path); return true; }
                try { File.Replace(TempPath, Path, PreviousPath); }
                catch (PlatformNotSupportedException)
                {
                    // Nền tảng không có Replace: chép bản cũ ra .prev TRƯỚC khi
                    // xoá. Nếu chết giữa Delete và Move, Load vẫn còn .tmp nguyên
                    // vẹn và .prev để lấy lại.
                    File.Copy(Path, PreviousPath, true);
                    File.Delete(Path);
                    File.Move(TempPath, Path);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Không ghi được {Path}: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Đổi save theo kiểu ĂN CẢ hoặc KHÔNG GÌ: hoặc mọi thay đổi trong
        /// <paramref name="change"/> cùng lên đĩa, hoặc save trở về y như trước.
        ///
        /// Dùng cho thao tác nhiều bước như mở rương (trừ chìa → quay → phát
        /// quà → đếm bảo hiểm). Trước đây mỗi bước tự Save, nên chết giữa
        /// chừng là mất chìa mà không có quà.
        ///
        /// <paramref name="change"/> trả false (thiếu chìa, kho trống...) hoặc
        /// ném lỗi → huỷ. Ghi đĩa thất bại → cũng huỷ, vì báo "thành công" mà
        /// tắt game là mất thì còn tệ hơn báo lỗi.
        ///
        /// Khôi phục bằng FromJsonOverwrite để giữ nguyên ĐỐI TƯỢNG Current —
        /// ai đang cầm tham chiếu tới nó vẫn thấy dữ liệu đúng.
        /// </summary>
        public static bool Transact(Func<SaveData, bool> change)
        {
            var save = Current;
            var before = JsonUtility.ToJson(save);
            bool ok;
            try { ok = change(save); }
            catch (Exception e) { Debug.LogError($"[SaveSystem] Huỷ giao dịch: {e}"); ok = false; }
            if (ok && Save()) return true;
            JsonUtility.FromJsonOverwrite(before, save);
            Normalize(save);
            return false;
        }

        /// <summary>Xoá sạch tiến độ. Chỉ dùng từ nút trong Editor.</summary>
        public static void Wipe()
        {
            current = new SaveData();
            foreach (var path in new[] { Path, TempPath, PreviousPath })
            {
                try { if (File.Exists(path)) File.Delete(path); }
                catch (IOException) { }
            }
        }

        /// <summary>Đường dẫn thật, để in ra khi cần đi tìm file.</summary>
        public static string FilePath => Path;
    }
}
