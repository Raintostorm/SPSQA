using Project.Battle;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Bảng kết quả khi qua màn hoặc thua. Tạm thời đóng luôn vai trò
    /// "trang chủ" — chỗ duy nhất ra khỏi một ván.
    ///
    /// Là một lớp phủ trong CÙNG scene, không phải scene riêng. Lý do kỹ
    /// thuật: file .unity do Unity giữ trong bộ nhớ, tôi không ghi được
    /// (xem Handoff/11 mục E). Khi nào cần trang chủ thật có chọn level thì
    /// bạn tạo scene, tôi dựng nội dung.
    /// </summary>
    public sealed class ResultPanel : MonoBehaviour
    {
        [SerializeField] BattleFlow flow;

        [Tooltip("Để đọc số vàng/chìa thưởng khi qua màn. Thiếu thì vẫn " +
                 "mở được màn kế, chỉ là không cộng tài nguyên.")]
        [SerializeField] Data.GameBalance balance;

        [Header("Do BattleHudBuilder gán")]
        [SerializeField] CanvasGroup group;
        [SerializeField] Image dim;
        [SerializeField] Text title;
        [SerializeField] Text detail;
        [SerializeField] Button againButton;
        [SerializeField] Text againLabel;
        [SerializeField] Button homeButton;

        void Awake()
        {
            if (flow == null) flow = FindFirstObjectByType<BattleFlow>();
            if (againButton != null) againButton.onClick.AddListener(Again);
            if (homeButton != null) homeButton.onClick.AddListener(SceneRoutes.GoHome);
            Show(false);
        }

        void OnEnable()
        {
            if (flow != null) flow.PhaseChanged += OnPhase;
        }

        void OnDisable()
        {
            if (flow != null) flow.PhaseChanged -= OnPhase;
        }

        void OnPhase(BattleFlow.Phase phase)
        {
            var win = phase == BattleFlow.Phase.LevelCleared;
            var lose = phase == BattleFlow.Phase.Defeated;
            if (!win && !lose) { Show(false); return; }

            // Mở màn kế NGAY khi thắng, không đợi người chơi bấm gì. Đợi
            // bấm thì ai thoát game ở màn này là mất tiến độ.
            if (win) GameProgress.Clear(GameProgress.CurrentLevel, balance);

            if (title != null)
            {
                title.text = win ? "QUA MÀN" : "THUA RỒI";
                title.color = win ? new Color(1f, 0.87f, 0.35f)
                                  : new Color(1f, 0.5f, 0.45f);
            }
            if (detail != null)
                detail.text = win
                    ? $"Màn {GameProgress.CurrentLevel} — trụ hết {flow.Wave} wave"
                    : $"Màn {GameProgress.CurrentLevel} — gục ở wave {flow.Wave}";
            if (againLabel != null) againLabel.text = win ? "CHƠI TIẾP" : "CHƠI LẠI";
            Show(true);
        }

        void Again()
        {
            Show(false);
            flow?.RestartRun();
        }

        void Show(bool on)
        {
            if (group == null) return;
            group.alpha = on ? 1f : 0f;
            // Chan chuot khi hien: khong chan thi nguoi choi van keo tha duoc
            // linh phia sau tam man ket qua.
            group.blocksRaycasts = on;
            group.interactable = on;
        }
    }
}
