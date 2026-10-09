using Project.Battle;
using Project.Data;
using UnityEngine;

namespace Project.Gacha
{
    /// <summary>
    /// Luật tiến hoá: 3 con giống hệt trong một ô → 1 con NGẪU NHIÊN ở thời
    /// đại kế tiếp, nằm lại chính ô đó.
    ///
    /// Handoff/09 chốt tầng: **logic TRẢ LỜI, UI HỎI**. Nên ở đây có
    /// <see cref="CanEvolve"/> cho UI hỏi trước khi vẽ nút, và
    /// <see cref="Evolve"/> để thực hiện. Lớp này KHÔNG được đi tìm cái nút
    /// để ẩn nó.
    /// </summary>
    public static class EvolutionSystem
    {
        /// <summary>
        /// Ô này tiến hoá được không, và nếu được thì ra thời đại nào.
        ///
        /// Ba điều kiện, thiếu một là không:
        ///   1. ô ĐẦY (đúng MaxPerCell con)
        ///   2. cả ô cùng MỘT UnitDefinition — "cùng vai + cùng thời đại"
        ///   3. thời đại kế tiếp CÓ lính trong pool
        ///
        /// Điều kiện 3 là dữ liệu chứ không phải luật cứng: hiện chưa có
        /// bậc nào trên Legendary nên Legendary tự động không tiến hoá được,
        /// cần viết riêng một ngoại lệ cho Future.
        /// </summary>
        public static bool CanEvolve(BattleBoard board, int row, int col, UnitPool pool,
                                     out UnitDefinition from, out Rank to)
        {
            from = null; to = default;
            if (board == null || pool == null) return false;

            var units = board.UnitsAt(row, col);
            if (units == null || units.Count < board.Grid.MaxPerCell) return false;

            from = units[0]?.Definition;
            if (from == null) return false;
            foreach (var u in units)
                if (u == null || !u.IsAlive || u.Definition != from) return false;

            if (from.Rank >= Rank.Legendary) return false;   // het bac de len
            to = from.Rank + 1;
            return pool.HasAny(to);
        }

        /// <summary>
        /// Thực hiện. Trả con lính mới, hoặc null nếu ô không đủ điều kiện.
        ///
        /// Huỷ cả 3 con TRƯỚC khi sinh con mới: sinh trước thì ô có 4 con
        /// trong một khoảnh khắc, và BattleBoard.Place sẽ từ chối.
        /// </summary>
        public static Unit Evolve(BattleBoard board, int row, int col, UnitPool pool,
                                  UnitSpawner spawner, System.Random rng)
        {
            if (!CanEvolve(board, row, col, pool, out var from, out var to)) return null;

            // Giu nguyen GIONG LOAI khi len bac: gop ba con elf phai ra elf.
            // Chi quay ngau nhien khi bac tren khong co giong do.
            var next = pool.Find(to, from.Race) ?? pool.Random(to, rng);
            if (next == null) return null;

            var team = board.UnitsAt(row, col)[0].Team;
            foreach (var u in new System.Collections.Generic.List<Unit>(board.UnitsAt(row, col)))
            {
                if (u == null) continue;
                board.Remove(u);
                Object.Destroy(u.gameObject);
            }

            // Con moi phai GOP vao o da co cung loai, neu co. Ban truoc dat
            // thang vao o vua gop, nen tien hoa xong con moi dung mot minh
            // du cho khac da co san may con y het — va nguoi choi phai keo
            // tay. FindSpotFor luot 1 tim dung o cung loai con cho.
            //
            // Khong co o nao nhu vay thi QUAY VE o vua gop. Khong chot dieu
            // kien nay thi luot 2 cua FindSpotFor tra ve o trong dau tien,
            // va con moi nhay sang goc ban ma khong ai hieu vi sao.
            var r = row;
            var c = col;
            if (board.FindSpotFor(next, out var fr, out var fc) && board.CountAt(fr, fc) > 0)
            {
                r = fr;
                c = fc;
            }

            var unit = spawner.Spawn(next, team, board.Grid.CellToWorld(r, c),
                                     board.transform);
            if (unit == null) return null;
            board.Place(unit, r, c);
            unit.Visual?.SetFacing(team == UnitTeam.Ally ? 1f : -1f);

            var brain = unit.GetComponent<UnitBrain>();
            if (brain != null) brain.enabled = false;   // dang o man chuan bi
            return unit;
        }

        /// <summary>Bán cả ô. Trả số coin nhận được.</summary>
        public static int Sell(BattleBoard board, int row, int col, GameBalance balance)
        {
            if (board == null || balance == null) return 0;

            var total = 0;
            foreach (var u in new System.Collections.Generic.List<Unit>(board.UnitsAt(row, col)))
            {
                if (u?.Definition == null) continue;

                var i = (int)u.Definition.Rank;
                var table = balance.SellPriceByRank;
                total += table != null && i < table.Length ? table[i] : 0;
                board.Remove(u);
                Object.Destroy(u.gameObject);
            }
            return total;
        }
    }
}
