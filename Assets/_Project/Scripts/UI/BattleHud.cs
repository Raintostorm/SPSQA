using Project.Battle;
using Project.Data;
using Project.Gacha;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Lớp chữ và nút nằm trên trận đấu.
    ///
    /// CHỈ ĐỌC trạng thái rồi vẽ lại. Nó không quyết định gì — bấm nút thì
    /// nó gọi <see cref="BattleFlow.StartWave"/> và để BattleFlow tự lo.
    /// Nếu thấy mình muốn viết luật trận đánh vào đây thì là đang viết nhầm
    /// file: hướng phụ thuộc là UI → LOGIC, không bao giờ ngược lại.
    ///
    /// Mọi object con do BattleHudBuilder dựng lúc chạy lệnh menu, không
    /// phải kéo tay trong Inspector.
    /// </summary>
    public sealed class BattleHud : MonoBehaviour
    {
        [SerializeField] BattleFlow flow;

        [Header("Do BattleHudBuilder gán — không sửa tay")]
        [SerializeField] Text statusLabel;
        [SerializeField] Text allyLabel;
        [SerializeField] Text enemyLabel;
        [SerializeField] Button startButton;
        [SerializeField] Button speedButton;
        [SerializeField] Text speedLabel;
        [SerializeField] RollService roll;
        [SerializeField] Button rollButton;
        [SerializeField] Text coinLabel;
        [Tooltip("Bốn ô số buff, cùng thứ tự với icon trên thanh: " +
                 "máu, sát thương, tốc đánh, chí mạng.")]
        [SerializeField] Text[] buffValues = new Text[4];

        [Tooltip("Nút đổi giữa xem quân ta và xem đội hình địch.")]
        [SerializeField] Button viewEnemyButton;

        [Tooltip("Để trống thì tự tìm trong scene.")]
        [SerializeField] BoardView boards;

        /// <summary>Các mức tốc độ bấm vòng qua lại.</summary>
        static readonly float[] Speeds = { 1f, 2f, 3f };
        int speedIndex;

        // Dem lai so cua khung truoc. Gan chuoi vao Text la Unity dung lai
        // ca luoi chu, lam moi frame thi phi — ma so chi doi khi co con chet.
        int shownAlly = -1, shownEnemy = -1;

        void Awake()
        {
            if (flow == null) flow = FindFirstObjectByType<BattleFlow>();
            if (flow == null)
            {
                Debug.LogError("[BattleHud] Không thấy BattleFlow trong scene. " +
                               "Chạy lại SpinSquad → Dựng scene.", this);
                enabled = false;
                return;
            }

            if (startButton != null)
                startButton.onClick.AddListener(flow.StartWave);
            if (speedButton != null)
                speedButton.onClick.AddListener(CycleSpeed);

            if (boards == null) boards = FindFirstObjectByType<BoardView>();
            if (viewEnemyButton != null)
                viewEnemyButton.onClick.AddListener(ToggleBoards);

            if (roll == null) roll = FindFirstObjectByType<RollService>();
            if (rollButton != null && roll != null)
                rollButton.onClick.AddListener(() => roll.TryRoll());
        }

        /// <summary>
        /// Đổi tốc độ bằng Time.timeScale, KHÔNG phải nhân riêng từng chỗ.
        ///
        /// timeScale co giãn Time.deltaTime, nên mọi thứ đọc deltaTime đều
        /// nhanh theo cùng một nhịp: bước chân, hồi chiêu, clip animation,
        /// đạn bay. Nhân tay từng chỗ thì chắc chắn sẽ sót một chỗ, và sót
        /// chỗ nào thì chỗ đó lệch pha với phần còn lại.
        /// </summary>
        void CycleSpeed()
        {
            speedIndex = (speedIndex + 1) % Speeds.Length;
            ApplySpeed();
        }

        void ApplySpeed()
        {
            Time.timeScale = Speeds[speedIndex];
            if (speedLabel != null) speedLabel.text = $"x{Speeds[speedIndex]:0}";
        }

        // Dang ky o OnEnable chu khong phai Awake, va BO DANG KY o OnDisable.
        // Thieu ve ban kia thi BattleFlow con giu tham chieu toi HUD da tat,
        // va lan sau bat len se nghe event hai lan.
        void OnEnable()
        {
            if (flow == null) return;
            flow.PhaseChanged += OnPhaseChanged;
            OnPhaseChanged(flow.Current);   // bat kip trang thai dang co
            ApplySpeed();

            // Run chỉ có sau Awake của RollService, nên đăng ký ở OnEnable
            // chứ không phải Awake — thứ tự Awake giữa hai object là KHÔNG
            // xác định, xem lỗi B6.
            if (roll?.Run != null)
            {
                roll.Run.CoinsChanged += OnCoinsChanged;
                OnCoinsChanged(roll.Run.Coins);
            }
        }

        void OnDisable()
        {
            if (flow != null) flow.PhaseChanged -= OnPhaseChanged;
            if (roll?.Run != null) roll.Run.CoinsChanged -= OnCoinsChanged;

            // BẮT BUỘC trả về 1. Time.timeScale là biến TOÀN CỤC và nó sống
            // dai hơn scene — thoát Play lúc đang để x3 thì lần Play sau
            // game chạy x3 ngay từ đầu, mà chẳng có gì trên màn hình nói thế.
            Time.timeScale = 1f;
        }

        void Update()
        {
            var ally  = UnitRegistry.CountAlive(UnitTeam.Ally);
            var enemy = UnitRegistry.CountAlive(UnitTeam.Enemy);

            if (ally != shownAlly && allyLabel != null)
            {
                allyLabel.text = $"TA  {ally}";
                shownAlly = ally;
            }
            if (enemy != shownEnemy && enemyLabel != null)
            {
                enemyLabel.text = $"{enemy}  ĐỊCH";
                shownEnemy = enemy;
            }
        }

        void OnCoinsChanged(int coins)
        {
            if (coinLabel != null) coinLabel.text = $"{coins}¢";
            if (roll == null) return;

            var canRoll = coins >= roll.RollCost;
            var prep = flow != null && flow.Current == BattleFlow.Phase.Prep;

            // Hai nút ĐÈ LÊN NHAU nên phải ẩn hẳn, không chỉ xám: xám thì
            // nút dưới bị nút trên che, bấm không tới.
            //
            // Luật: còn tiền thì roll, hết tiền mới được xuất quân. Phím
            // Space vẫn xuất quân được bất kể — lối thoát phòng khi kinh tế
            // lạm phát tới mức coin không bao giờ xuống dưới giá roll.
            // Dang xem doi hinh dich thi KHONG cho roll cung khong cho xuat
            // quan: hai nut do thuoc ve viec xep quan, ma luc nay bàn ta
            // dang bi dời đi — bam vao chi gay roi.
            var looking = boards != null && boards.ShowingEnemy;

            if (rollButton != null)
            {
                rollButton.gameObject.SetActive(prep && canRoll && !looking);
                rollButton.interactable = canRoll;
            }
            if (startButton != null)
                startButton.gameObject.SetActive(prep && !canRoll && !looking);

            RefreshBuffs();
            RefreshStatus();
        }

        void OnPhaseChanged(BattleFlow.Phase phase)
        {
            RefreshViewButton();
            // Tiền không đổi khi sang giai đoạn mới nên OnCoinsChanged không
            // tự chạy — mà chính nó quyết định nút nào hiện. Gọi tay.
            if (roll?.Run != null) OnCoinsChanged(roll.Run.Coins);
            else
            {
                var prep = phase == BattleFlow.Phase.Prep;
                if (rollButton != null) rollButton.gameObject.SetActive(false);
                if (startButton != null) startButton.gameObject.SetActive(prep);
            }

            RefreshBuffs();
            RefreshStatus();
        }

        /// <summary>
        /// Bốn buff đang có. Đọc RunBuffs chứ không đọc RunState: RunBuffs
        /// là thứ lính THẬT SỰ dùng, nên hiện nó thì không bao giờ lệch với
        /// chỉ số trong trận.
        /// </summary>
        /// <summary>Đổi bàn, rồi vẽ lại mọi thứ phụ thuộc vào nó.</summary>
        void ToggleBoards()
        {
            if (boards == null) return;
            boards.Toggle();
            RefreshViewButton();
            if (roll?.Run != null) OnCoinsChanged(roll.Run.Coins);
            RefreshStatus();
        }

        void RefreshViewButton()
        {
            if (viewEnemyButton == null) return;
            var txt = viewEnemyButton.GetComponentInChildren<Text>();
            if (txt == null) return;
            var looking = boards != null && boards.ShowingEnemy;
            txt.text = looking ? "QUÂN TA" : "XEM ĐỊCH";
            // Chi xem duoc o giai doan chuan bi — ra tran thi khong con ban nao.
            viewEnemyButton.gameObject.SetActive(
                flow == null || flow.Current == BattleFlow.Phase.Prep);
        }

        void RefreshBuffs()
        {
            // Icon da noi ro buff nao, nen o day chi con con so. Thu tu
            // PHAI khop thu tu icon ma BattleHudBuilder dung — doi mot ben
            // ma quen ben kia la so hien duoi sai icon, va khong co gi keu.
            if (buffValues == null || buffValues.Length < 4) return;
            Set(0, RunBuffs.HpPct);
            Set(1, RunBuffs.DmgPct);
            Set(2, RunBuffs.AtkSpeedPct);
            Set(3, RunBuffs.CritChance);

            void Set(int i, float value)
            {
                if (buffValues[i] != null) buffValues[i].text = $"+{value:P0}";
            }
        }

        /// <summary>
        /// Một chỗ duy nhất quyết định dòng chữ trên bảng. Hai nơi cùng ghi
        /// vào nó — đổi giai đoạn và đổi tiền — nên tách ra, không thì một
        /// bên ghi đè bên kia và chữ sai mà không ai biết tại sao.
        /// </summary>
        void RefreshStatus()
        {
            if (statusLabel == null || flow == null) return;
            var phase = flow.Current;

            // Kèm số wave: không có nó thì người chơi không biết mình đang ở
            // vòng thứ mấy, mà cả game là chuỗi wave nối nhau.
            var w = $"WAVE {flow.Wave}  ·  ";
            statusLabel.text = phase switch
            {
                BattleFlow.Phase.Prep     => w + (roll != null && roll.Run != null
                                                 && roll.Run.Coins >= roll.RollCost
                                                 ? "TIÊU HẾT COIN ĐÃ" : "XẾP QUÂN VÀO LƯỚI"),
                BattleFlow.Phase.Marching => w + "ĐANG XUẤT QUÂN",
                BattleFlow.Phase.Fighting => w + "GIAO TRANH",
                BattleFlow.Phase.Finished => flow.Winner switch
                {
                    UnitTeam.Ally  => "THẮNG",
                    UnitTeam.Enemy => "THUA",
                    _              => "HOÀ",
                },
                _ => string.Empty,
            };
            statusLabel.color = phase == BattleFlow.Phase.Finished
                ? (flow.Winner == UnitTeam.Ally ? new Color(0.55f, 0.95f, 0.5f)
                                                : new Color(1f, 0.45f, 0.45f))
                : Color.white;
        }
    }
}
