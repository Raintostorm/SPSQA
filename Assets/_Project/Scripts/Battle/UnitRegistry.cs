using System.Collections.Generic;
using Project.Data;
using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Danh sách mọi lính đang sống trên sân.
    ///
    /// Tồn tại để KHÔNG phải gọi FindObjectsByType mỗi frame. Hàm đó quét
    /// toàn bộ scene; với 48 lính mỗi phe và mỗi con tìm địch 1 lần/giây thì
    /// nó thành thứ nặng nhất trong game. Lính tự ghi danh khi bật, tự rút
    /// tên khi tắt — O(1).
    /// </summary>
    public static class UnitRegistry
    {
        static readonly List<Unit> all = new List<Unit>(64);

        public static IReadOnlyList<Unit> All => all;

        public static void Add(Unit u)
        {
            if (u != null && !all.Contains(u)) all.Add(u);
        }

        public static void Remove(Unit u) => all.Remove(u);

        /// <summary>
        /// Lính còn sống thuộc phe KHÁC <paramref name="team"/> và gần
        /// <paramref name="from"/> nhất. Trả null nếu phe kia sạch bóng.
        /// </summary>
        public static Unit FindNearestEnemy(Vector3 from, UnitTeam team)
        {
            Unit best = null;
            var bestSqr = float.MaxValue;

            // Duyệt bằng chỉ số, không dùng foreach: foreach trên List tạo
            // enumerator mỗi lần gọi, và hàm này chạy liên tục.
            for (var i = 0; i < all.Count; i++)
            {
                var u = all[i];
                if (u == null || !u.IsAlive || u.Team == team) continue;

                // So bình phương khoảng cách, khỏi tính căn bậc hai.
                var sqr = (u.transform.position - from).sqrMagnitude;
                if (sqr >= bestSqr) continue;

                bestSqr = sqr;
                best = u;
            }
            return best;
        }

        /// <summary>Gọi khi đổi trận. Không gọi thì xác trận cũ còn nằm lại.</summary>
        public static void Clear() => all.Clear();

        /// <summary>
        /// Đếm quân CÒN SỐNG của một phe — kể cả con đang đứng trong lưới,
        /// chưa ra đường. Nhờ vậy gọi lúc đang xuất quân cũng không nhầm là
        /// phe đó đã bị diệt.
        ///
        /// Xác vẫn nằm trong danh sách (Unit không Destroy khi chết) nên
        /// BẮT BUỘC lọc IsAlive, đếm suông là ra số sai.
        /// </summary>
        public static int CountAlive(UnitTeam team)
        {
            var n = 0;
            for (var i = 0; i < all.Count; i++)
            {
                var u = all[i];
                if (u != null && u.IsAlive && u.Team == team) n++;
            }
            return n;
        }
    }
}
