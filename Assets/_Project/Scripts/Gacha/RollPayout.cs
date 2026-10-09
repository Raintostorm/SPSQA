using System.Collections.Generic;
using Project.Data;

namespace Project.Gacha
{
    /// <summary>Một ô Ally thắng cụm → cấp 1 lính. Giống random, bậc theo cỡ cụm.</summary>
    public readonly struct AllyRollGrant
    {
        /// <summary>Giong loai — nhanh ngang, quay ngau nhien.</summary>
        public readonly Race Race;

        /// <summary>Bac — truc tien hoa, cum cang to bac cang cao.</summary>
        public readonly Rank Rank;

        public AllyRollGrant(Race race, Rank rank)
        {
            Race = race;
            Rank = rank;
        }
    }

    /// <summary>Một ô Buff thắng cụm → cộng dồn 1 stat vào RunState.</summary>
    public readonly struct BuffRollAdd
    {
        public readonly BuffStatKind Stat;

        /// <summary>Phân số, KHÔNG phải phần trăm: 0.1 = +10%. CritChance cũng 0–1: 0.01 = 1%.</summary>
        public readonly float Value;

        public BuffRollAdd(BuffStatKind stat, float value)
        {
            Stat = stat;
            Value = value;
        }
    }

    /// <summary>Kết quả một lượt roll: 6 ô đã quay + mọi thứ nhận được.</summary>
    public sealed class RollPayout
    {
        public RollCellKind[] Cells { get; }

        /// <summary>
        /// Ô thứ i có thuộc cụm thắng không. Dài đúng 6, khớp Cells.
        ///
        /// Để ở đây chứ không bắt UI tự đếm lại: luật "hoà thì cả hai cùng
        /// ăn" có một chỗ định nghĩa duy nhất là RollResolver. UI đếm lại là
        /// hai bản luật, và sớm muộn chúng lệch nhau.
        /// </summary>
        public bool[] CellWins { get; }
        public int CoinAwarded { get; set; }

        /// <summary>Cả 6 ô cùng một loại.</summary>
        public bool Jackpot { get; set; }

        public List<AllyRollGrant> AllyGrants { get; } = new();
        public List<BuffRollAdd> BuffAdds { get; } = new();
        public string SummaryLine { get; set; } = string.Empty;

        public RollPayout(RollCellKind[] cells)
        {
            Cells = cells;
            CellWins = new bool[cells?.Length ?? 0];
        }
    }
}
