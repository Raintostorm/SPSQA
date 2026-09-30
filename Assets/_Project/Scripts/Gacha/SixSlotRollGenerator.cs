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
        /// <summary>
        /// 27 bộ ba (coin, ally, buff) hợp lệ. CỐ ĐỊNH — không phụ thuộc trọng số,
        /// không đổi trong suốt vòng đời game. Tính MỘT LẦN ở static constructor.
        /// </summary>
        static readonly (int c, int a, int b)[] Partitions;

        /// <summary>Số bộ ba ứng với mỗi số ô Ally. Index 0..6 → giá trị {7,6,4,4,3,2,1}.</summary>
        static readonly int[] CountByAlly = new int[7];

        static SixSlotRollGenerator()
        {
            var list = new List<(int c, int a, int b)>(27);

            for (var c = 0; c <= 6; c++)
            {
                for (var a = 0; a <= 6; a++)
                {
                    for (var b = 0; b <= 6; b++)
                    {
                        if (c + a + b != 6)
                            continue;

                        // (2,2,2) là bộ DUY NHẤT không có loại nào đạt ≥3 ô.
                        // Loại nó đi = đảm bảo mọi lượt roll luôn có thưởng.
                        if (c == 2 && a == 2 && b == 2)
                            continue;

                        list.Add((c, a, b));
                        CountByAlly[a]++;
                    }
                }
            }

            Partitions = list.ToArray();
        }

        public static RollCellKind[] Generate(Random rng, int[] allyCountWeights)
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

        /// <summary>
        /// Chọn một bộ ba (coin, ally, buff) theo trọng số.
        /// Xác suất do <paramref name="allyCountWeights"/> quyết định — 7 phần tử,
        /// index = số ô Ally (0..6). Sau chuẩn hoá, mảng đó CHÍNH LÀ phân bố mong muốn.
        /// </summary>
        static (int coin, int ally, int buff) PickPartitionTripleWeighted(
            Random rng,
            int[] allyCountWeights)
        {
            if (allyCountWeights == null || allyCountWeights.Length != 7)
                throw new ArgumentException("allyCountWeights must contain exactly 7 values.");

            // Trọng số MỖI BỘ BA = trọng số nhóm / số bộ ba trong nhóm.
            // Không chia thì nhóm a=3 (4 bộ ba) ăn trọn 40×4 = 160 thay vì 40.
            //
            // * 1000 trước khi chia vì đây là số nguyên: a=0 có W=1, count=7
            // → 1/7 = 0, nhóm đó biến mất. Còn 1000/7 = 142, giữ được 0,994%.
            //
            // Phải tính lại mỗi lượt: allyCountWeights là mảng CÓ THỂ SỬA từ
            // Inspector. Cache theo tham chiếu mảng sẽ lỗi thời âm thầm khi
            // người chơi/designer đổi số bên trong mà tham chiếu không đổi.
            var total = 0;
            for (var i = 0; i < Partitions.Length; i++)
            {
                var a = Partitions[i].a;
                total += Math.Max(1, allyCountWeights[a] * 1000 / CountByAlly[a]);
            }

            // Weighted random: trải mọi trọng số thành một "thanh thước" dài `total`,
            // ném phi tiêu vào vị trí r, rồi trừ dần cho tới khi âm.
            var r = rng.Next(total);

            for (var i = 0; i < Partitions.Length; i++)
            {
                var p = Partitions[i];
                r -= Math.Max(1, allyCountWeights[p.a] * 1000 / CountByAlly[p.a]);

                if (r < 0)
                    return (p.c, p.a, p.b);
            }

            // Lưới an toàn: về lý thuyết không bao giờ tới, vì r < total.
            var fallback = Partitions[0];
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
