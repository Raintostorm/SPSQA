using System;
using System.Text;
using Project.Data;

namespace Project.Gacha
{
    /// <summary>
    /// 6 ô đã quay → nhận được gì.
    ///
    /// Luật (Handoff/01 mục 3.3): loại nào NHIỀU Ô NHẤT thì ăn thưởng, và
    /// cụm phải ≥ 3 ô. Hoà thì CẢ HAI cùng ăn — đó là lý do phải duyệt cả
    /// ba loại chứ không chỉ lấy loại lớn nhất.
    ///
    /// Tĩnh và thuần: cùng một bộ ô và cùng một hạt giống thì luôn ra cùng
    /// một kết quả. Nhờ vậy kiểm được bằng cách chạy hàng vạn lần và đếm,
    /// xem SpinSquad → Kiểm phân bố roll.
    /// </summary>
    public static class RollResolver
    {
        public const int MinCluster = 3;

        public static RollPayout Resolve(RollCellKind[] cells, GameBalance balance, Random rng)
        {
            var payout = new RollPayout(cells);
            if (cells == null || cells.Length != 6 || balance == null) return payout;

            var count = new int[3];
            foreach (var c in cells) count[(int)c]++;

            var best = Math.Max(count[0], Math.Max(count[1], count[2]));
            payout.Jackpot = best == 6;

            var sb = new StringBuilder();
            for (var k = 0; k < 3; k++)
            {
                // CHI loai dat dinh moi an. Khong phai "moi loai >= 3" — voi
                // 6 o thi 3-3-0 la hoa (ca hai an), con 4-2-0 thi chi loai 4
                // an, du 2 kia khong dat nguong nao ca.
                if (count[k] < MinCluster || count[k] != best) continue;

                for (var i = 0; i < cells.Length; i++)
                    if ((int)cells[i] == k) payout.CellWins[i] = true;

                var tier = count[k] - MinCluster;        // 0..3 <-> cum 3..6
                switch ((RollCellKind)k)
                {
                    case RollCellKind.Coin:
                        payout.CoinAwarded += At(balance.ClusterRewardCoin, tier);
                        sb.Append($"+{At(balance.ClusterRewardCoin, tier)}¢  ");
                        break;

                    case RollCellKind.Ally:
                        // Cum to -> bac cao. Cum 3 ra Common, cum 6 ra Legendary.
                        var rank = (Rank)Math.Min(tier, (int)Rank.Legendary);
                        var race = (Race)rng.Next(0, 5);
                        payout.AllyGrants.Add(new AllyRollGrant(race, rank));
                        // Chi ghi BAC. So o thang va giong loai la chuyen noi
                        // bo — con linh hien ra ngay tren luoi, nguoi choi
                        // nhin thay la biet, khong can doc lai.
                        sb.Append($"{rank}  ");
                        break;

                    case RollCellKind.Buff:
                        var stat = (BuffStatKind)rng.Next(0, 4);
                        var value = BuffValue(balance, stat, tier);
                        payout.BuffAdds.Add(new BuffRollAdd(stat, value));
                        sb.Append($"{Label(stat)} +{value:P0}  ");
                        break;
                }
            }

            if (payout.Jackpot) sb.Append(" JACKPOT!");
            payout.SummaryLine = sb.ToString().TrimEnd();
            return payout;
        }

        /// <summary>Tên buff cho người đọc, không phải tên enum.</summary>
        static string Label(BuffStatKind s) => s switch
        {
            BuffStatKind.HpPct       => "MÁU",
            BuffStatKind.DmgPct      => "SÁT THƯƠNG",
            BuffStatKind.AtkSpeedPct => "TỐC ĐÁNH",
            _                        => "CHÍ MẠNG",
        };

        /// <summary>
        /// Bảng trong GameBalance ghi theo PHẦN TRĂM (10 nghĩa là +10%), còn
        /// BuffRollAdd dùng PHÂN SỐ (0.1). Chia 100 đúng một chỗ duy nhất là
        /// đây — rải phép chia ra nhiều nơi là sớm muộn sót một chỗ và buff
        /// nào đó mạnh gấp trăm lần.
        /// </summary>
        static float BuffValue(GameBalance b, BuffStatKind stat, int tier) => stat switch
        {
            BuffStatKind.HpPct       => At(b.ClusterRewardHP, tier) / 100f,
            BuffStatKind.DmgPct      => At(b.ClusterRewardDamage, tier) / 100f,
            BuffStatKind.AtkSpeedPct => At(b.ClusterRewardAtkSpeed, tier) / 100f,
            BuffStatKind.CritChance  => At(b.ClusterRewardCritChance, tier) / 100f,
            _ => 0f,
        };

        /// <summary>Kẹp chỉ số: mảng trong asset có thể bị sửa ngắn đi.</summary>
        static int At(int[] table, int i) =>
            table == null || table.Length == 0
                ? 0
                : table[Math.Clamp(i, 0, table.Length - 1)];
    }
}
