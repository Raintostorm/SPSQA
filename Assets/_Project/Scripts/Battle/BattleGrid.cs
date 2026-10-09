using Project.Data;
using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Đổi (hàng, cột) thành toạ độ thế giới, và ngược lại.
    ///
    /// CHỈ thế. Nó không vẽ lưới, không giữ danh sách lính, không biết ô nào
    /// đầy. Lưới nhìn thấy được là mấy sprite bạn đặt trong Editor; ai đứng
    /// ô nào là việc của lớp khác (M5).
    ///
    /// Mỗi phe một BattleGrid riêng — đặt hai cái, khác Origin.
    ///
    /// Gizmo chỉ để căn chỉnh lúc dựng, không phải hình của game.
    /// </summary>
    public sealed class BattleGrid : MonoBehaviour
    {
        [SerializeField] GameBalance balance;

        [Tooltip("Tâm ô (0,0) — hàng dưới cùng, cột trái nhất.")]
        [SerializeField] Vector2 origin = new Vector2(-1.5f, -3f);

        [Tooltip("Bề NGANG một ô, đơn vị thế giới.")]
        [SerializeField] float cellSize = 1f;

        [Tooltip("Bề DỌC một ô. Để 0 nghĩa là ô vuông (dùng luôn bề ngang).\n\n" +
                 "Tách riêng vì ô vẽ sẵn trong ảnh nền KHÔNG vuông — đo được " +
                 "1.00 × 0.73. Nhìn nghiêng kiểu này thì ô bao giờ cũng bẹt " +
                 "hơn chiều ngang, ép nó vuông là lính đứng lệch khỏi ô vẽ.")]
        [SerializeField] float cellHeight = 0f;

        [Tooltip("Độ giãn đội hình trong một ô, HAI TRỤC riêng.\n\n" +
                 "Một con số cho cả hai trục là sai: ô rộng 1.03 nhưng chỉ " +
                 "cao 0.73, còn lính cao 0.59 rộng 0.42. Giãn đều thì ba con " +
                 "tràn lên xuống trong khi bỏ phí một phần ba bề ngang. " +
                 "BattleSceneSetup tính lại theo kích thước ô.")]
        [SerializeField] Vector2 formationSpread = new Vector2(0.30f, 0.072f);

        [Tooltip("Gốc toạ độ của lính nằm ở CHÂN, nhưng ô thì neo ở TÂM. " +
                 "Không bù thì thân người mọc từ giữa ô lên trên và tràn ra " +
                 "ngoài. Đo được: thân cao 0.78, nên lệch xuống -0.39.")]
        [SerializeField] Vector2 unitAnchor = new Vector2(0f, -0.39f);

        public int Rows => balance != null ? balance.GridRows : 4;
        public int Cols => balance != null ? balance.GridCols : 4;
        public int MaxPerCell => balance != null ? balance.MaxUnitsPerCell : 3;
        public float CellWidth => cellSize;

        /// <summary>Bề dọc thật. 0 trong Inspector nghĩa là "vuông".</summary>
        public float CellHeight => cellHeight > 0f ? cellHeight : cellSize;

        // ── đổi qua lại ───────────────────────────────────────────────

        public bool IsInside(int row, int col) =>
            row >= 0 && row < Rows && col >= 0 && col < Cols;

        /// <summary>Tâm ô, trong toạ độ thế giới.</summary>
        public Vector3 CellToWorld(int row, int col)
        {
            var p = (Vector2)transform.position + origin
                  + new Vector2(col * CellWidth, row * CellHeight);
            return new Vector3(p.x, p.y, 0f);
        }

        /// <summary>
        /// Ô nào chứa điểm này. Trả false nếu điểm nằm ngoài lưới —
        /// gọi phải kiểm giá trị trả về, đừng dùng row/col khi nó false.
        /// </summary>
        public bool WorldToCell(Vector3 world, out int row, out int col)
        {
            var local = (Vector2)world - ((Vector2)transform.position + origin);

            // +0.5 vì origin là TÂM ô (0,0), không phải góc. Thiếu nửa ô này
            // thì mọi phép đổi lệch đúng nửa ô — sai rất khó thấy bằng mắt.
            col = Mathf.FloorToInt(local.x / CellWidth + 0.5f);
            row = Mathf.FloorToInt(local.y / CellHeight + 0.5f);
            return IsInside(row, col);
        }

        /// <summary>
        /// Đội hình trong một ô, theo số lượng. Ghi thẳng từng vị trí thay vì
        /// tính bằng công thức — ba con xếp tam giác không phải một dãy đều,
        /// và công thức chung sẽ phải đầy ngoại lệ.
        ///
        /// Đơn vị: bội số của formationSpread, mỗi trục nhân riêng. Giá trị
        /// chỉ chạy trong [-1, 1] nên ±1 nghĩa là "chạm mép chỗ cho phép" —
        /// nhờ vậy đổi kích thước ô chỉ cần đổi formationSpread, không phải
        /// đụng vào bảng này.
        ///
        /// y âm = đứng trước (gần người xem).
        /// </summary>
        static readonly Vector2[][] Formations =
        {
            new Vector2[0],                                   // 0 con
            new[] { new Vector2( 0f, 0f) },                   // 1 — giữa ô
            new[] { new Vector2(-1f, 1f),                     // 2 — chéo
                    new Vector2( 1f,-1f) },
            new[] { new Vector2( 0f, 1f),                     // 3 — tam giác
                    new Vector2(-1f,-1f),                     //     1 sau, 2 trước
                    new Vector2( 1f,-1f) },
        };

        /// <summary>
        /// Chỗ đứng của lính thứ <paramref name="slot"/> trong ô, khi ô đang
        /// có <paramref name="occupancy"/> con.
        /// </summary>
        public Vector3 SlotToWorld(int row, int col, int slot, int occupancy) =>
            SlotToWorld(row, col, slot, occupancy, null);

        /// <summary>
        /// Như trên, nhưng bù thêm độ lệch RIÊNG của nhân vật này.
        ///
        /// <paramref name="def"/> null thì rơi về hành vi cũ — dùng cho chỗ
        /// chỉ cần hình dạng đội hình chứ không đặt một con cụ thể.
        /// </summary>
        public Vector3 SlotToWorld(int row, int col, int slot, int occupancy,
                                   UnitDefinition def) =>
            CellToWorld(row, col) + (Vector3)unitAnchor
            + (Vector3)FormationOffset(slot, occupancy)
            + (def != null ? (Vector3)def.CellOffset : Vector3.zero);

        /// <summary>
        /// Độ lệch của lính thứ <paramref name="slot"/> so với tâm nhóm, khi
        /// nhóm có <paramref name="occupancy"/> con.
        ///
        /// Tách khỏi SlotToWorld để BattleFlow dùng lại được khi thả cả ô ra
        /// đường — ra trận thì không còn ô nào nữa, nhưng đội hình tam giác
        /// phải giữ nguyên, nếu không thì ba con chồng khít lên một điểm.
        /// </summary>
        public Vector2 FormationOffset(int slot, int occupancy)
        {
            // Kẹp lại phòng khi MaxUnitsPerCell bị đổi lớn hơn bảng đội hình.
            var n = Mathf.Clamp(occupancy, 0, Formations.Length - 1);
            if (n == 0 || slot < 0 || slot >= Formations[n].Length) return Vector2.zero;

            // Trừ đi TÂM của chính đội hình đó, để cụm luôn cân quanh neo.
            //
            // Bảng tam giác 3 con là (0,+1) (−1,−1) (+1,−1): hai con dưới một
            // con trên, nên tâm cụm tụt xuống 1/3 độ giãn. Không trừ thì cụm
            // 3 con nằm thấp hơn cụm 1 con, và ô nào đủ 3 con cũng trông như
            // bị lệch xuống. Cụm 1 và 2 con vốn đã cân nên phép trừ này
            // không đụng tới chúng.
            var mid = Center(Formations[n]);
            var f = Formations[n][slot] - mid;
            return new Vector2(f.x * formationSpread.x, f.y * formationSpread.y);
        }

        /// <summary>
        /// Tâm KHUNG BAO của đội hình, không phải tâm khối.
        ///
        /// Hai cái khác nhau ở đội hình tam giác 3 con (0,+1) (−1,−1) (+1,−1):
        /// tâm khối tụt xuống −1/3, còn khung bao thì cân đúng ở 0. Cụm phải
        /// nằm giữa Ô, mà "nằm giữa ô" là chuyện khung bao — nên lấy khung bao.
        ///
        /// Trước đây hàm này trả tâm khối, rồi bù phần chênh bằng một hằng số
        /// −spread.y/3 cộng vào unitAnchor ở ngoài. Hằng số đó áp cho MỌI ô,
        /// kể cả ô chỉ có một con — nơi chẳng có tam giác nào để bù. Kết quả:
        /// con nào cũng bị đẩy xuống, khe trên hở hơn khe dưới 0.12 đơn vị.
        ///
        /// Với 1 và 2 con, tâm khối và tâm khung bao trùng nhau, nên đổi
        /// sang đây không đụng gì tới chúng.
        /// </summary>
        static Vector2 Center(Vector2[] f)
        {
            if (f.Length == 0) return Vector2.zero;
            var lo = f[0];
            var hi = f[0];
            foreach (var v in f)
            {
                lo = Vector2.Min(lo, v);
                hi = Vector2.Max(hi, v);
            }
            return (lo + hi) * 0.5f;
        }

        /// <summary>
        /// Thứ tự vẽ cho lính đứng ở vị trí này. Con đứng THẤP hơn phải vẽ
        /// ĐÈ LÊN con đứng cao, nếu không thì con sau chồng lên con trước và
        /// trông như lộn ngược chiều sâu.
        ///
        /// Giá trị là -y*100, tức nó TRÔI THEO vị trí lưới trên màn hình.
        /// Lính đứng ở y = +2.1 ra -215; lính ở y = -1.5 ra +153. Ảnh nền và
        /// ô đất thì dùng hằng số cố định, nên hai thang số này phải cách
        /// nhau đủ xa — xem <see cref="BackgroundOrder"/>.
        /// </summary>
        public int SortingOrderFor(Vector3 worldPosition) =>
            Mathf.RoundToInt(-worldPosition.y * 100f);

        /// <summary>
        /// Trần của thang số lính: lính cao nhất có thể đứng ở y = +10,
        /// tức order -1000. Ảnh nền và ô đất phải nằm dưới mức này.
        ///
        /// Vì sao phải viết ra: trước đây nền để -100 và ô đất để -10, chọn
        /// hồi màn còn NGANG — lúc đó lưới nằm quanh y = 0 nên order lính
        /// chỉ chạy trong khoảng ±60 và mọi thứ tình cờ đúng. Chuyển sang
        /// màn DỌC, lưới dời lên y = +2.1, order lính thành -215 — thấp hơn
        /// cả ảnh nền. Tám ô hàng trên biến mất, và không có gì báo lỗi.
        /// </summary>
        public const int UnitOrderFloor = -1000;

        /// <summary>Ô đất: dưới mọi con lính, trên ảnh nền.</summary>
        public const int TileOrder = UnitOrderFloor - 1000;      // -2000

        /// <summary>Ảnh nền: dưới tất cả.</summary>
        public const int BackgroundOrder = TileOrder - 1000;     // -3000

        // ── chỉ để căn chỉnh trong Editor ─────────────────────────────

        void OnDrawGizmos()
        {
            var half = CellWidth * 0.5f;
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Cols; c++)
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
                Gizmos.DrawWireCube(CellToWorld(r, c),
                                    new Vector3(CellWidth, CellHeight, 0f));
            }

            // Đánh dấu ô (0,0) để biết lưới mọc theo hướng nào.
            Gizmos.color = new Color(0.3f, 1f, 0.4f, 0.8f);
            Gizmos.DrawWireSphere(CellToWorld(0, 0), half * 0.25f);

            // Chấm vàng = chỗ CHÂN lính đứng. Lệch khỏi tâm ô vì unitAnchor.
            Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.7f);
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Cols; c++)
                Gizmos.DrawWireSphere(SlotToWorld(r, c, 0, 1), 0.05f);
        }
    }
}
