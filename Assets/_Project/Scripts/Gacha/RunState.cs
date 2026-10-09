using System;
using Project.Battle;
using Project.Data;

namespace Project.Gacha
{
    /// <summary>
    /// Mọi thứ tích luỹ trong MỘT trận: ví coin và các buff cộng dồn.
    ///
    /// Thuần C#, không phải MonoBehaviour — nhờ vậy test được mà không cần
    /// scene, và không ai vô tình kéo hai cái vào Inspector.
    ///
    /// Buff cộng dồn cả run và reset khi bắt đầu trận mới. Coin cũng vậy:
    /// coin trong trận KHÔNG phải gold ở meta, hết trận là mất.
    /// </summary>
    public sealed class RunState
    {
        /// <summary>Bắn mỗi lần ví đổi, kèm số dư MỚI.</summary>
        public event Action<int> CoinsChanged;

        public int Coins { get; private set; }

        /// <summary>Số lính Legendary được phép có trên sân. Treasure nâng cái này.</summary>
        public int LegendaryLimit { get; private set; }

        // Phan so, KHONG phai phan tram: 0.1 = +10%. CritChance cung 0-1.
        public float HpPct { get; private set; }
        public float DmgPct { get; private set; }
        public float AtkSpeedPct { get; private set; }
        public float CritChance { get; private set; }

        public RunState(GameBalance balance) => Reset(balance);

        /// <summary>
        /// Về trạng thái đầu ván, NGAY TRÊN đối tượng này.
        ///
        /// Không tạo RunState mới khi chơi lại: HUD đang đăng ký
        /// CoinsChanged của đối tượng cũ, thay đối tượng là nó mất đăng ký
        /// và số coin đứng hình mà không ai báo.
        /// </summary>
        public void Reset(GameBalance balance)
        {
            HpPct = DmgPct = AtkSpeedPct = CritChance = 0f;
            LegendaryLimit = balance != null ? balance.StartingLegendaryLimit : 0;
            RunBuffs.Reset();
            if (balance != null) RunBuffs.CritMultiplier = balance.CritMultiplier;

            Coins = balance != null ? balance.StartingCoins : 0;
            CoinsChanged?.Invoke(Coins);
        }

        /// <summary>
        /// Trừ tiền nếu đủ. Trả false và KHÔNG đụng vào ví nếu thiếu.
        ///
        /// Gộp "kiểm tra" và "trừ" vào một hàm là cố ý: tách ra thì bên gọi
        /// có thể kiểm xong rồi quên trừ, hoặc trừ mà quên kiểm.
        /// </summary>
        public bool TrySpend(int amount)
        {
            if (amount < 0 || Coins < amount) return false;
            Coins -= amount;
            CoinsChanged?.Invoke(Coins);
            return true;
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0) return;
            Coins += amount;
            CoinsChanged?.Invoke(Coins);
        }

        public void Apply(BuffRollAdd add)
        {
            switch (add.Stat)
            {
                case BuffStatKind.HpPct:       HpPct += add.Value; break;
                case BuffStatKind.DmgPct:      DmgPct += add.Value; break;
                case BuffStatKind.AtkSpeedPct: AtkSpeedPct += add.Value; break;
                case BuffStatKind.CritChance:  CritChance += add.Value; break;
            }
            Publish();
        }

        /// <summary>
        /// Đẩy sang RunBuffs cho phía trận đấu đọc.
        ///
        /// Hai bản số trông thừa, nhưng chúng ở hai tầng khác nhau: RunState
        /// là sổ sách của gacha, RunBuffs là hộp phẳng mà lính đọc. Cho lính
        /// đọc thẳng RunState là Battle phải biết Gacha.
        /// </summary>
        void Publish()
        {
            RunBuffs.HpPct = HpPct;
            RunBuffs.DmgPct = DmgPct;
            RunBuffs.AtkSpeedPct = AtkSpeedPct;
            RunBuffs.CritChance = CritChance;
        }

        public override string ToString() =>
            $"{Coins} coin · HP +{HpPct:P0} · DMG +{DmgPct:P0} · " +
            $"ATKSPD +{AtkSpeedPct:P0} · CRIT +{CritChance:P0}";
    }
}
