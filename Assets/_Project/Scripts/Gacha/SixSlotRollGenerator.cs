using System;
using System.Collections.Generic;

namespace Project.Gacha
{
    /// <summary>
    /// Sinh 6 ô. KHÔNG random từng ô độc lập — random cái *phân hoạch* (coin, ally, buff),
    /// loại bỏ (2,2,2) để LUÔN có ít nhất một loại đạt ≥3 ô.
    /// Nhờ vậy người chơi không bao giờ có lượt trắng tay.
    ///
    /// Mỗi số chạy 0..6 (CHO PHÉP 0). Nếu ép cả ba ≥1 thì số lớn nhất chỉ tới
    /// 6-1-1 = 4, nghĩa là cụm 5 và 6 KHÔNG BAO GIỜ xảy ra — Jackpot chết,
    /// và roll không bao giờ ra lính Epic/Legendary. Bản cũ dính đúng lỗi này.
    /// </summary>
    public static class SixSlotRollGenerator
    {
        /// <summary>Lũy thừa trọng số theo số ô Ally (cao hơn = ally xuất hiện nhiều hơn).</summary>
        const double AllyCountBiasPow = 1.5d;

        /// <summary>Trọng số theo số ô Coin+Buff. = 1.0 nghĩa là KHÔNG ảnh hưởng gì.</summary>
        const double CoinBuffCountBiasPow = 1.0d;

        public static RollCellKind[] Generate(Random rng)
        {
            var (coin, ally, buff) = PickPartitionTripleWeighted(rng);

            var kinds = new List<RollCellKind>(6);
            for (var i = 0; i < coin; i++)
                kinds.Add(RollCellKind.Coin);
            for (var i = 0; i < ally; i++)
                kinds.Add(RollCellKind.Ally);
            for (var i = 0; i < buff; i++)
                kinds.Add(RollCellKind.Buff);

            Shuffle(kinds, rng);

            var arr = new RollCellKind[6];
            for (var i = 0; i < 6; i++)
                arr[i] = kinds[i];
            return arr;
        }

        /// <summary>Chọn (coin, ally, buff) tổng = 6, mỗi số 0..6, bỏ (2,2,2), trọng số nghiêng về Ally.</summary>
        static (int coin, int ally, int buff) PickPartitionTripleWeighted(Random rng)
        {
            var entries = new List<(int c, int a, int b, int w)>(27);
            for (var c = 0; c <= 6; c++)
            {
                for (var a = 0; a <= 6; a++)
                {
                    for (var b = 0; b <= 6; b++)
                    {
                        if (c + a + b != 6)
                            continue;
                        if (c == 2 && a == 2 && b == 2)
                            continue;

                        var wf = Math.Pow(AllyCountBiasPow, a)
                                 * Math.Pow(CoinBuffCountBiasPow, c + b);
                        var w = Math.Max(1, (int)Math.Round(wf * 1000d));
                        entries.Add((c, a, b, w));
                    }
                }
            }

            var total = 0;
            foreach (var e in entries)
                total += e.w;

            var r = rng.Next(total);
            foreach (var e in entries)
            {
                r -= e.w;
                if (r < 0)
                    return (e.c, e.a, e.b);
            }

            var fallback = entries[0];
            return (fallback.c, fallback.a, fallback.b);
        }

        /// <summary>Fisher-Yates.</summary>
        static void Shuffle(List<RollCellKind> list, Random rng)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
