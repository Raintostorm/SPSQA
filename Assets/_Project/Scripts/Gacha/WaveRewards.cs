using Project.Battle;
using Project.Data;
using UnityEngine;

namespace Project.Gacha
{
    /// <summary>
    /// Cộng coin khi qua được một wave.
    ///
    /// Một file riêng, ba chục dòng, chỉ để giữ đúng chiều phụ thuộc:
    /// BattleFlow không được biết ví tiền là gì, và RollService không được
    /// biết trận đấu là gì. Cái biết cả hai là component keo dán này.
    ///
    /// Nhét phép cộng coin vào BattleFlow thì nhanh hơn, nhưng từ đó trở đi
    /// mọi thay đổi về kinh tế đều phải đụng vào luật trận đánh.
    /// </summary>
    public sealed class WaveRewards : MonoBehaviour
    {
        [SerializeField] BattleFlow flow;
        [SerializeField] RollService roll;

        void OnEnable()
        {
            if (flow == null) flow = FindFirstObjectByType<BattleFlow>();
            if (roll == null) roll = FindFirstObjectByType<RollService>();
            if (flow != null) flow.PhaseChanged += OnPhase;
        }

        void OnDisable()
        {
            if (flow != null) flow.PhaseChanged -= OnPhase;
        }

        void OnPhase(BattleFlow.Phase phase)
        {
            // Thuong lúc KET THUC, khong phai luc ve san: ve san la mot nhip
            // sau, va neu sau nay them man hinh ket qua thi so tien phai co
            // san de hien len do.
            if (phase != BattleFlow.Phase.Finished) return;
            if (roll == null || flow.Winner != UnitTeam.Ally) return;
            roll.AwardWaveClear();
        }
    }
}
