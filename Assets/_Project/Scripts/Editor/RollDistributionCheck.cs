using System;
using System.Text;
using Project.Data;
using Project.Gacha;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools
{
    /// <summary>
    /// Quay thử 100 000 lượt rồi đếm.
    ///
    /// Vì sao cần: luật roll là XÁC SUẤT, nên chạy tay vài lượt không chứng
    /// minh được gì. Một bảng trọng số sai vẫn cho ra kết quả "trông hợp lý"
    /// ở mười lượt đầu, và chỉ lộ sau vài trăm lượt — tức là lộ với người
    /// chơi chứ không lộ với mình.
    ///
    /// Nó gọi ĐÚNG code mà game chạy, không phải bản mô phỏng.
    /// </summary>
    public static class RollDistributionCheck
    {
        const int Trials = 100_000;

        [MenuItem("SpinSquad/Kiểm phân bố roll")]
        static void Run()
        {
            var balance = AssetDatabase.LoadAssetAtPath<GameBalance>(
                "Assets/_Project/Data/GameBalance.asset");
            if (balance == null)
            {
                Debug.LogError("[RollCheck] Không thấy GameBalance.asset");
                return;
            }

            var rng = new System.Random(12345);      // co dinh de chay lai ra y het
            var byAlly = new int[7];                 // bao nhieu luot co n o Ally
            var byCluster = new int[7];              // cum lon nhat bang bao nhieu
            var kindWins = new int[3];
            var eraGrants = new int[5];
            var jackpots = 0;
            long coinOut = 0;
            var emptyRolls = 0;

            for (var i = 0; i < Trials; i++)
            {
                var cells = SixSlotRollGenerator.Generate(rng, balance.AllyCountWeights);
                var count = new int[3];
                foreach (var c in cells) count[(int)c]++;
                byAlly[count[(int)RollCellKind.Ally]]++;

                var best = Math.Max(count[0], Math.Max(count[1], count[2]));
                byCluster[best]++;
                for (var k = 0; k < 3; k++)
                    if (count[k] == best && best >= RollResolver.MinCluster) kindWins[k]++;

                var p = RollResolver.Resolve(cells, balance, rng);
                if (p.Jackpot) jackpots++;
                coinOut += p.CoinAwarded;
                foreach (var g in p.AllyGrants) eraGrants[(int)g.Rank]++;
                if (p.CoinAwarded == 0 && p.AllyGrants.Count == 0 && p.BuffAdds.Count == 0)
                    emptyRolls++;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"[RollCheck] {Trials:N0} lượt, giá roll {balance.RollCost}\n");

            sb.AppendLine("Số ô Ally mỗi lượt (phải bám trọng số AllyCountWeights):");
            var wsum = 0f;
            foreach (var w in balance.AllyCountWeights) wsum += w;
            for (var n = 0; n < 7; n++)
                sb.AppendLine($"   {n} ô : {Pct(byAlly[n])}   mong đợi {balance.AllyCountWeights[n] / wsum:P2}");

            sb.AppendLine("\nCụm lớn nhất trong lượt:");
            for (var n = 2; n <= 6; n++)
                if (byCluster[n] > 0) sb.AppendLine($"   {n} ô : {Pct(byCluster[n])}");

            sb.AppendLine($"\nLoại ăn thưởng (cộng > 100% vì HOÀ thì cả hai cùng ăn):");
            sb.AppendLine($"   Coin {Pct(kindWins[0])}   Ally {Pct(kindWins[1])}   Buff {Pct(kindWins[2])}");

            sb.AppendLine("\nBậc lính nhận được:");
            for (var e = 0; e < 4; e++)
                if (eraGrants[e] > 0) sb.AppendLine($"   {(Rank)e,-10} {Pct(eraGrants[e])}");

            sb.AppendLine($"\nJackpot      : {Pct(jackpots)}");
            sb.AppendLine($"Coin thu về   : {coinOut / (double)Trials:0.0} / lượt " +
                          $"(bỏ ra {balance.RollCost}) → lãi {coinOut / (double)Trials - balance.RollCost:+0.0;-0.0}");
            sb.AppendLine($"Lượt trắng tay: {emptyRolls}   (PHẢI bằng 0 — luật (2,2,2) bị loại)");

            if (emptyRolls > 0) Debug.LogError(sb.ToString());
            else Debug.Log(sb.ToString());
        }

        static string Pct(int n) => $"{n / (double)Trials:P2}".PadLeft(8);
    }
}
