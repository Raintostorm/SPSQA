using System;
using Project.Data;
using UnityEngine;

namespace Project.Gacha
{
    /// <summary>
    /// Một lượt roll, từ đầu đến cuối: trừ tiền → quay 6 ô → chia thưởng →
    /// cộng vào ví và buff.
    ///
    /// Gom cả chuỗi vào MỘT hàm là cố ý. Tách ra thì bên gọi có thể trừ tiền
    /// rồi quên quay, hoặc quay rồi quên trừ — và lỗi kiểu đó chỉ lộ ra khi
    /// người chơi phát hiện roll miễn phí.
    ///
    /// Lính thắng được chỉ nằm trong <see cref="RollPayout.AllyGrants"/>;
    /// đặt nó lên lưới là việc của lớp khác. Ở đây không biết gì về lưới.
    /// </summary>
    public sealed class RollService : MonoBehaviour
    {
        [SerializeField] GameBalance balance;

        [Tooltip("Kho 30 vật phẩm. Thiếu thì trận vẫn chạy, chỉ là không " +
                 "cộng chỉ số từ kho báu.")]
        [SerializeField] TreasureDatabase treasures;

        [Tooltip("Để 0 là mỗi phiên một dãy khác. Đặt khác 0 để lặp lại được " +
                 "đúng dãy cũ khi đi tìm lỗi.")]
        [SerializeField] int seed;


        /// <summary>Bắn sau khi đã áp xong mọi thứ, để UI vẽ lại.</summary>
        public event Action<RollPayout> Rolled;
        public event Action<RollPayout> RewardReady;
        bool presenting;
        Project.Battle.EffectBuffSystem effects;
        public bool IsReelPresenting => presenting;
        public bool IsPresenting => presenting || effects && effects.HasPendingChoice;
        RollPayout pending;
        int successfulRolls;

        /// <summary>Bắn khi bấm roll mà không đủ tiền.</summary>
        public event Action NotEnoughCoins;

        public RunState Run { get; private set; }
        public int RollCost => balance != null ? balance.RollCost : 0;

        System.Random rng;

        [SerializeField] Project.Battle.BattleFlow flow;

        void OnEnable()
        {
            if (flow == null) flow = FindFirstObjectByType<Project.Battle.BattleFlow>();
            if (flow != null) flow.RunRestarted += NewRun;
        }

        void OnDisable()
        {
            if (flow != null) flow.RunRestarted -= NewRun;
        }

        /// <summary>Ví và buff về mốc đầu ván. GIỮ NGUYÊN đối tượng RunState.</summary>
        void NewRun()
        {
            pending = null;
            successfulRolls = 0;
            presenting = false;
            if (balance != null) Run?.Reset(balance);
            // SAU Reset: Reset xoá RunBuffs về 0.
            Meta.TreasureBuffs.Apply(treasures, balance);
        }

        void Awake()
        {
            if (balance == null)
            {
                Debug.LogError("[RollService] Chưa gán GameBalance.", this);
                enabled = false;
                return;
            }
            rng = seed == 0 ? new System.Random() : new System.Random(seed);
            Run = new RunState(balance);
            effects=gameObject.AddComponent<Project.Battle.EffectBuffSystem>();effects.Bind(this);
            Meta.TreasureBuffs.Apply(treasures, balance);
        }

        /// <summary>
        /// Trả null nếu không đủ tiền — và khi đó ví KHÔNG bị đụng tới.
        /// </summary>
        public RollPayout TryRoll()
        {
            var setup = FindFirstObjectByType<Project.Battle.BattleSetup>();
            if (setup && setup.IsChoosingLegend) return null;
            if (Run == null || IsPresenting || flow != null && flow.Current != Project.Battle.BattleFlow.Phase.Prep) return null;

            if (!Run.TrySpend(RollCost))
            {
                NotEnoughCoins?.Invoke();
                return null;
            }

            var cells = StageRollPreview.FirstEpicBuff(Project.Meta.GameProgress.CurrentLevel,successfulRolls)
                ?? SixSlotRollGenerator.Generate(rng, balance.AllyCountWeights);
            successfulRolls++;
            var payout = RollResolver.Resolve(cells, balance, rng);

            pending = payout;
            presenting = true;
            Rolled?.Invoke(payout);
            var panel = FindFirstObjectByType<Project.UI.RollPanel>();
            if (!panel || !panel.isActiveAndEnabled) { CommitPayout(payout); FinishPresentation(); }
            // Chi in thu NHAN DUOC, khong in lai ca bang chi so. Bang day du
            // van doc duoc bang Run.ToString() khi can soi loi.
            Debug.Log($"[RollService] {payout.SummaryLine}   (con {Run.Coins}¢)", this);
            return payout;
        }

        public void CommitPayout(RollPayout payout)
        {
            if (payout == null || !ReferenceEquals(pending, payout)) return;
            pending = null; // Clear before invoking listeners: exactly-once commit.
            Run.AddCoins(payout.CoinAwarded);
            Project.Battle.BattleContribution.Roll(payout);
            foreach (var b in payout.BuffAdds)
                if(b.Rank>=Rank.Epic)effects.QueueChoice(b.Rank);else Run.Apply(b);
            RewardReady?.Invoke(payout);
        }
        public void FinishPresentation() { presenting = false; }
        public void FlushPresentation() { CommitPayout(pending); presenting = false; }

        /// <summary>Gọi khi qua wave. Tiền thưởng nằm trong GameBalance.</summary>
        public void AwardWaveClear()
        {
            if (balance != null) Run?.AddCoins(balance.CoinPerWaveClear);
        }
    }
}
