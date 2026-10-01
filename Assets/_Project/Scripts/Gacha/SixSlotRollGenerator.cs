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
        public static RollCellKind[] Generate(
        Random rng,
        int[] allyCountWeights
)
        {
            var (coin, ally, buff) = PickPartitionTripleWeighted(rng, allyCountWeights);

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

        static (int coin, int ally, int buff) PickPartitionTripleWeighted(
        Random rng,
        int[] allyCountWeights
)
{
        if (allyCountWeights == null || allyCountWeights.Length != 7)
            throw new ArgumentException(
                "allyCountWeights must contain exactly 7 values."
            );

        var partitions = new List<(int c, int a, int b)>(27);
        var countByAlly = new int[7];

        // Pass 1:
        // Tìm tất cả partition hợp lệ và đếm theo số ô Ally.
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

                partitions.Add((c, a, b));
                countByAlly[a]++;
            }
        }
    }

    // Pass 2:
    // Chia weight của mỗi Ally count cho các partition
    // thuộc cùng nhóm Ally count đó.
    var entries =
        new List<(int c, int a, int b, int w)>(partitions.Count);

    foreach (var partition in partitions)
    {
        var count = countByAlly[partition.a];

        var groupWeight = allyCountWeights[partition.a];

        if (groupWeight <= 0)
        continue;

        var weight = Math.Max(
        1,
        groupWeight * 1000 / count
        );

        entries.Add((
            partition.c,
            partition.a,
            partition.b,
            weight
        ));
    }

    // Weighted random
    var total = 0;

    foreach (var entry in entries)
        total += entry.w;

    var r = rng.Next(total);

    foreach (var entry in entries)
    {
        r -= entry.w;

        if (r < 0)
            return (entry.c, entry.a, entry.b);
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
