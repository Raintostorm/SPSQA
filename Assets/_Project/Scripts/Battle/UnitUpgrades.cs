using System.Collections.Generic;
using Project.Data;
using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Cấp nâng của từng nhân vật, ở dạng lính ĐỌC ĐƯỢC.
    ///
    /// Cùng lối với <see cref="RunBuffs"/>: một hộp phẳng ở giữa, Meta ghi
    /// vào, Battle đọc ra, không bên nào phải tham chiếu bên kia.
    ///
    /// Vì sao phải có bộ nhớ đệm chứ không đọc thẳng file save: MaxHp và
    /// AttackPower được đọc nhiều lần mỗi khung hình, cho mỗi con lính. Dò
    /// một List 20 phần tử mỗi lần là phí không cần thiết.
    ///
    /// CHỈ phe ta. Địch không có cấp nâng — chúng không phải tài sản của
    /// người chơi.
    /// </summary>
    public static class UnitUpgrades
    {
        /// <summary>Mỗi cấp cộng bao nhiêu. Cùng quy tắc với vật phẩm.</summary>
        public const float PerLevel = 0.10f;

        static readonly Dictionary<string, int> levels = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetOnPlay() => levels.Clear();

        /// <summary>Nạp lại từ bảng (unitId, cấp). Gọi khi vào trận.</summary>
        public static void Load(IEnumerable<KeyValuePair<string, int>> rows)
        {
            levels.Clear();
            if (rows == null) return;
            foreach (var kv in rows)
                if (!string.IsNullOrEmpty(kv.Key)) levels[kv.Key] = kv.Value;
        }

        /// <summary>Phân số cộng thêm. Cấp 1 = 0, cấp 2 = 0.10, ...</summary>
        public static float Bonus(string unitId, UnitTeam team)
        {
            if (team != UnitTeam.Ally || string.IsNullOrEmpty(unitId)) return 0f;
            return levels.TryGetValue(unitId, out var lv)
                ? Mathf.Max(0, lv - 1) * PerLevel : 0f;
        }

        public static int LevelOf(string unitId) =>
            levels.TryGetValue(unitId, out var lv) ? lv : 1;
    }
}
