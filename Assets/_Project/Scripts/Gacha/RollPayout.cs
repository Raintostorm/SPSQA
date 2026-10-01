using System.Collections.Generic;
using Project.Data;

namespace Project.Gacha
{
    /// <summary>Một ô Ally thắng cụm → cấp 1 lính. Vai random, thời đại theo cỡ cụm.</summary>
    public readonly struct AllyRollGrant
    {
        public readonly UnitRole Role;
        public readonly Era EraTier;

        public AllyRollGrant(UnitRole role, Era eraTier)
        {
            Role = role;
            EraTier = eraTier;
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
        public int CoinAwarded { get; set; }

        /// <summary>Cả 6 ô cùng một loại.</summary>
        public bool Jackpot { get; set; }

        public List<AllyRollGrant> AllyGrants { get; } = new();
        public List<BuffRollAdd> BuffAdds { get; } = new();
        public string SummaryLine { get; set; } = string.Empty;

        public RollPayout(RollCellKind[] cells)
        {
            Cells = cells;
        }
    }
}
