using System.Collections.Generic;
using Project.Data;
using UnityEngine;
using UnityEngine.Rendering;

namespace Project.Battle
{
    /// <summary>
    /// Giữ danh sách ai đang đứng ô nào, và canh luật đặt lính.
    ///
    /// Tách khỏi BattleGrid: cái kia là TOÁN (ô ↔ toạ độ), cái này là LUẬT
    /// (ô chứa được bao nhiêu, chứa được ai).
    ///
    /// Hai luật, lấy từ Handoff/09:
    ///   · một ô tối đa MaxUnitsPerCell con (mặc định 3)
    ///   · các con trong một ô phải CÙNG MỘT UnitDefinition
    /// </summary>
    [RequireComponent(typeof(BattleGrid))]
    public sealed class BattleBoard : MonoBehaviour
    {
        BattleGrid grid;
        List<Unit>[,] cells;

        /// <summary>
        /// Lấy lười, KHÔNG gán trong Awake. Lý do: script Editor cần đọc Grid
        /// khi không Play, mà Awake chỉ chạy lúc Play — gán trong Awake thì
        /// ngoài Editor nó là null và mọi thứ lặng lẽ không làm gì.
        /// </summary>
        public BattleGrid Grid =>
            grid != null ? grid : (grid = GetComponent<BattleGrid>());

        /// <summary>
        /// Cấp phát LƯỜI, không cấp trong Awake.
        ///
        /// Unity không bảo đảm thứ tự Awake giữa hai GameObject. Cấp trong
        /// Awake thì bất kỳ ai gọi tới board sớm hơn đều nhận NullReference —
        /// và lỗi chỉ hiện khi Unity tình cờ chạy sai thứ tự, nên lúc có lúc
        /// không. Lấy lười thì thứ tự không còn ý nghĩa.
        /// </summary>
        List<Unit>[,] Cells
        {
            get
            {
                if (cells != null) return cells;
                cells = new List<Unit>[Grid.Rows, Grid.Cols];
                for (var r = 0; r < Grid.Rows; r++)
                for (var c = 0; c < Grid.Cols; c++)
                    cells[r, c] = new List<Unit>(Grid.MaxPerCell);
                return cells;
            }
        }

        // ── hỏi ───────────────────────────────────────────────────────

        public IReadOnlyList<Unit> UnitsAt(int row, int col) =>
            Grid.IsInside(row, col) ? Cells[row, col] : System.Array.Empty<Unit>();

        public int CountAt(int row, int col) => UnitsAt(row, col).Count;

        public bool IsFull(int row, int col) => CountAt(row, col) >= Grid.MaxPerCell;

        /// <summary>Ô này nhận thêm được con mang định nghĩa đó không.</summary>
        public bool CanPlace(int row, int col, UnitDefinition def)
        {
            if (!Grid.IsInside(row, col) || def == null) return false;
            if (IsFull(row, col)) return false;

            var list = Cells[row, col];
            // Ô trống thì nhận ai cũng được. Ô có người thì phải cùng loại.
            return list.Count == 0 || list[0].Definition == def;
        }

        /// <summary>
        /// Ô ưu tiên cho một lính mới: ô đã có con TRÙNG và chưa đủ 3 trước,
        /// hết thì tới ô trống. Trả false nếu bàn không còn chỗ.
        /// </summary>
        public bool FindSpotFor(UnitDefinition def, out int row, out int col)
        {
            // Lượt 1 — ô trùng loại, còn chỗ. Đây là luật "spawn gộp" ở Handoff/09.
            for (var r = 0; r < Grid.Rows; r++)
            for (var c = 0; c < Grid.Cols; c++)
                if (Cells[r, c].Count > 0 && CanPlace(r, c, def))
                { row = r; col = c; return true; }

            // Lượt 2 — ô trống.
            for (var r = 0; r < Grid.Rows; r++)
            for (var c = 0; c < Grid.Cols; c++)
                if (Cells[r, c].Count == 0)
                { row = r; col = c; return true; }

            row = col = -1;
            return false;
        }

        // ── đổi ───────────────────────────────────────────────────────

        /// <summary>Đặt lính vào ô. Trả false nếu luật không cho.</summary>
        public bool Place(Unit unit, int row, int col)
        {
            if (unit == null || !CanPlace(row, col, unit.Definition)) return false;

            Remove(unit);                 // rút khỏi ô cũ trước, tránh đứng hai nơi
            Cells[row, col].Add(unit);
            Reflow(row, col);
            return true;
        }

        /// <summary>Rút lính khỏi ô đang đứng. Không tìm thấy thì bỏ qua.</summary>
        public void Remove(Unit unit)
        {
            if (unit == null) return;
            for (var r = 0; r < Grid.Rows; r++)
            for (var c = 0; c < Grid.Cols; c++)
                if (Cells[r, c].Remove(unit)) { Reflow(r, c); return; }
        }

        /// <summary>
        /// Xếp lại chỗ đứng VÀ thứ tự vẽ trong một ô.
        /// Gọi sau MỌI thay đổi — thiếu thì ba con chồng khít lên nhau, hoặc
        /// ô còn một con mà nó vẫn đứng lệch sang bên.
        /// </summary>
        void Reflow(int row, int col)
        {
            var list = Cells[row, col];
            for (var i = 0; i < list.Count; i++)
            {
                var u = list[i];
                if (u == null) continue;

                var pos = Grid.SlotToWorld(row, col, i, list.Count, u.Definition);
                u.transform.position = pos;
                ApplySorting(u, pos);
            }
        }

        /// <summary>
        /// Con đứng thấp hơn phải vẽ đè lên con đứng cao.
        ///
        /// Phải dùng SortingGroup chứ không sửa từng SpriteRenderer: mỗi lính
        /// có 7 bộ phận với order 0..6 cố định (thân dưới mũ dưới tay...).
        /// Đổi order từng cái thì phá mất thứ tự BÊN TRONG một con. SortingGroup
        /// gom cả cây thành một khối, sắp xếp cả khối so với khối khác, mà bên
        /// trong vẫn giữ nguyên.
        /// </summary>
        void ApplySorting(Unit unit, Vector3 pos)
        {
            var group = unit.GetComponent<SortingGroup>();
            if (group == null) return;        // chưa gắn thì bỏ qua, không crash
            group.sortingOrder = Grid.SortingOrderFor(pos);
        }
    }
}
