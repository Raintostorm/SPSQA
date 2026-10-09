using Project.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Trang chủ: tên game và ba nút chọn màn.
    ///
    /// Nút của màn CHƯA MỞ bị xám và không bấm được. Ẩn hẳn thì người chơi
    /// không biết còn gì phía trước — mà biết còn gì phía trước chính là lý
    /// do họ chơi tiếp.
    /// </summary>
    public sealed class HomeScreen : MonoBehaviour
    {
        [Header("Do HomeSceneSetup gán")]
        [SerializeField] Button[] levelButtons;
        [SerializeField] Text[] levelLabels;
        [SerializeField] Text hint;

        [Header("Thanh tài nguyên")]
        [SerializeField] Text goldLabel;
        [SerializeField] Text keyLabel;

        [Header("Nút")]
        [SerializeField] Button playButton;
        [SerializeField] Button upgradeButton;
        [SerializeField] Button treasureButton;

        [Tooltip("Chi de doc so wave moi man. Thieu thi dong goi y ghi so " +
                 "mac dinh — khong hong gi, chi la chu co the sai.")]
        [SerializeField] Data.GameBalance balance;

        void Start()
        {
            // Start chu khong phai Awake: GameProgress doc file save, va o
            // Awake thi Application.persistentDataPath chua chac san sang.

            if (goldLabel != null) goldLabel.text = $"{GameProgress.Gold:N0}";
            if (keyLabel != null)  keyLabel.text  = $"{GameProgress.Keys:N0}";

            // Nut CHOI = vao man CAO NHAT da mo. Nguoi choi mo game len gan
            // nhu luon muon di tiep, khong phai choi lai man cu — nen de no
            // o nut to nhat, con chon man cu thi bam vao the.
            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(
                    () => SceneRoutes.PlayLevel(GameProgress.Unlocked));
            }

            Wire(upgradeButton, SceneRoutes.GoUpgrade);
            Wire(treasureButton, SceneRoutes.GoTreasure);

            void Wire(Button b, UnityEngine.Events.UnityAction go)
            {
                if (b == null) return;
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(go);
            }
            var unlocked = GameProgress.Unlocked;

            for (var i = 0; i < levelButtons.Length; i++)
            {
                var level = i + 1;
                var open = level <= unlocked;

                if (levelButtons[i] != null)
                {
                    levelButtons[i].interactable = open;
                    levelButtons[i].onClick.RemoveAllListeners();
                    if (open) levelButtons[i].onClick.AddListener(() => SceneRoutes.PlayLevel(level));
                }

                if (levelLabels[i] != null)
                {
                    levelLabels[i].text = open ? $"MÀN {level}" : $"MÀN {level}  🔒";
                    levelLabels[i].color = open ? new Color(0.16f, 0.1f, 0.03f)
                                                : new Color(0.35f, 0.33f, 0.3f);
                }
            }

            if (hint != null)
                hint.text = unlocked > 1
                    ? $"Đã mở {Mathf.Min(unlocked, levelButtons.Length)}/{levelButtons.Length} màn"
                    // Doc tu GameBalance chu KHONG go so vao chu. Go tay thi
                    // doi can bang xong, man hinh van day nguoi choi lam du
                    // mot so wave khong con dung nua.
                    : $"Qua {(balance != null ? balance.WavesPerLevel : 3)} wave " +
                      "để mở màn tiếp theo";
        }
    }
}
