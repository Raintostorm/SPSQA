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

        static SaveData current;

        static string Path => System.IO.Path.Combine(
            Application.persistentDataPath, FileName);

        static string BackupPath => System.IO.Path.Combine(
            Application.persistentDataPath, BackupName);

        /// <summary>Bản đang dùng. Tự nạp ở lần gọi đầu.</summary>
        public static SaveData Current => current ??= Load();

        /// <summary>
        /// Xoá bản trong bộ nhớ, KHÔNG đụng file.
        ///
        /// Cần vì biến static sống dai hơn scene: ai tắt "Reload Domain" cho
        /// nhanh thì bản của lần Play trước còn nguyên ở lần Play sau. Cùng
        /// một cái bẫy RunBuffs đã dính.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void DropCache() => current = null;

        static SaveData Load()
        {
            if (!File.Exists(Path))
                return new SaveData();

            try
            {
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(Path));

                // FromJson tra ve null khi file rong hoac khong phai JSON.
                // Khong chan thi moi cho doc Current.Gold se NullReference,
                // va loi hien ra o cho khong lien quan gi toi file save.
                if (data == null)
                    throw new InvalidDataException("file rỗng hoặc không phải JSON");

                return data;
            }
            catch (Exception e)
            {
                // KHONG xoa file hong. Doi ten de con cuu duoc neu do la
                // tien do that cua nguoi choi — xoa thang la het duong.
                Debug.LogError(
                    $"[SaveSystem] Không đọc được {Path}: {e.Message}. " +
                    $"Đổi tên thành {BackupName} và bắt đầu lại từ đầu.");
                try
                {
                    if (File.Exists(BackupPath)) File.Delete(BackupPath);
                    File.Move(Path, BackupPath);
                }
                catch (IOException) { /* khong doi duoc ten thi cung khong lam gi hon */ }

                return new SaveData();
            }
        }

        /// <summary>Ghi bản trong bộ nhớ xuống đĩa.</summary>
        public static void Save()
        {
            if (current == null) return;
            try
            {
                // Ghi ra file TAM roi doi ten de. Ghi thang vao save.json ma
                // game bi giet giua chung la file cut doi — tien do mat sach.
                // Doi ten la thao tac nguyen tu cua he dieu hanh.
                var tmp = Path + ".tmp";
                File.WriteAllText(tmp, JsonUtility.ToJson(current, true));
                if (File.Exists(Path)) File.Delete(Path);
                File.Move(tmp, Path);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Không ghi được {Path}: {e.Message}");
            }
        }

        /// <summary>Xoá sạch tiến độ. Chỉ dùng từ nút trong Editor.</summary>
        public static void Wipe()
        {
            current = new SaveData();
            try { if (File.Exists(Path)) File.Delete(Path); }
            catch (IOException) { }
        }

        /// <summary>Đường dẫn thật, để in ra khi cần đi tìm file.</summary>
        public static string FilePath => Path;
    }
}
