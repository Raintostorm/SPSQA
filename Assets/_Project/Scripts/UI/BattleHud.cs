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
        public RectTransform RewardTarget(int stat) => stat < 0 ? coinLabel?.rectTransform :
            buffValues != null && stat < buffValues.Length ? buffValues[stat]?.rectTransform : null;
        int speedIndex;

        // Dem lai so cua khung truoc. Gan chuoi vao Text la Unity dung lai
        // ca luoi chu, lam moi frame thi phi — ma so chi doi khi co con chet.
        int shownAlly = -1, shownEnemy = -1;
        RollPanel reel;
        Vector4 displayedBuffs = new Vector4(float.NaN,0,0,0);
        GameObject actionDock, dockHint;
        Text rollPrice;

        void Start()
        {
            MythicFusionPanel.Install(this);
            reel = FindFirstObjectByType<RollPanel>();
            var header=new GameObject("WaveHeader",typeof(RectTransform),typeof(Image));header.transform.SetParent(transform,false);header.transform.SetAsFirstSibling();
            BattleUiSkin.Place(header.GetComponent<RectTransform>(),new Vector2(.5f,1),new Vector2(0,-100),new Vector2(1060,120));
            BattleUiSkin.Skin(header.GetComponent<Image>(),BattleUiSkin.Navy);header.GetComponent<Image>().raycastTarget=false;
            header.GetComponent<Image>().color=new Color(1,1,1,.7f);
            foreach (var name in new[] { "AllyCount", "EnemyToggle", "Buffs", "Coins", "SpeedButton" })
            {
                var child=transform.Find(name);
                if(child){BattleUiSkin.Skin(child.GetComponent<Image>(), BattleUiSkin.Navy);var image=child.GetComponent<Image>();if(image)image.color=new Color(1,1,1,.7f);}
            }
            BattleUiSkin.Place(allyLabel.transform.parent.GetComponent<RectTransform>(),new Vector2(0,1),new Vector2(32,-110),new Vector2(240,100));
            BattleUiSkin.Place(viewEnemyButton.GetComponent<RectTransform>(),new Vector2(1,1),new Vector2(-32,-110),new Vector2(240,100));
            BattleUiSkin.Place(statusLabel.rectTransform,new Vector2(.5f,1),new Vector2(0,-110),new Vector2(520,100));
            statusLabel.fontSize=44;
            var buffs=transform.Find("Buffs");
            if(buffs)
            {
                BattleUiSkin.Place(buffs.GetComponent<RectTransform>(),new Vector2(.5f,1),new Vector2(0,-230),new Vector2(1060,140));
                BattleUiSkin.Skin(buffs.GetComponent<Image>(),BattleUiSkin.Dark);
                for(int i=0;i<buffValues.Length;i++)
                {
                    var value=buffValues[i];if(!value)continue;
                    value.fontSize=44;value.resizeTextForBestFit=true;value.resizeTextMinSize=30;value.resizeTextMaxSize=44;value.color=new Color(.75f,1,.72f,1);
                    value.horizontalOverflow=HorizontalWrapMode.Wrap;value.verticalOverflow=VerticalWrapMode.Truncate;
                    value.rectTransform.offsetMin=new Vector2(100,0);value.rectTransform.offsetMax=new Vector2(-8,0);
                    var icon=value.transform.parent.Find("Icon");
                    if(icon){BattleUiSkin.Place(icon.GetComponent<RectTransform>(),new Vector2(0,.5f),new Vector2(12,0),new Vector2(80,80));var image=icon.GetComponent<Image>();if(image)image.color=Color.white;}
                }
            }
            var dock=new GameObject("ActionDock",typeof(RectTransform),typeof(Image));dock.transform.SetParent(transform,false);dock.transform.SetAsFirstSibling();
            actionDock=dock;
            BattleUiSkin.Place(dock.GetComponent<RectTransform>(),new Vector2(.5f,0),new Vector2(0,32),new Vector2(1060,290));
            BattleUiSkin.Skin(dock.GetComponent<Image>(),BattleUiSkin.Navy);
            BattleUiSkin.Place(rollButton.GetComponent<RectTransform>(),new Vector2(.5f,0),new Vector2(0,60),new Vector2(520,180));
            BattleUiSkin.Place(startButton.GetComponent<RectTransform>(),new Vector2(.5f,0),new Vector2(0,60),new Vector2(520,180));
            BattleUiSkin.Button(rollButton,BattleUiSkin.Blue,$"{roll.RollCost}");
            BattleUiSkin.Button(startButton,BattleUiSkin.Orange,"Battle");
            BattleUiSkin.Button(speedButton,BattleUiSkin.Navy,"x1");
            BattleUiSkin.Button(viewEnemyButton,BattleUiSkin.Navy,"Enemy");
            StyleActions();
            BattleUiSkin.Icon(viewEnemyButton,"enemy",true);
            BattleUiSkin.Place(coinLabel.transform.parent.GetComponent<RectTransform>(),new Vector2(.5f,0),new Vector2(0,315),new Vector2(420,80));
            coinLabel.fontSize=40;
            if(speedLabel)speedLabel.fontSize=48;
            BattleUiSkin.Place(speedButton.GetComponent<RectTransform>(),new Vector2(1,0),new Vector2(-60,244),new Vector2(220,72));
            var hint=new GameObject("DockHint",typeof(RectTransform),typeof(Text));hint.transform.SetParent(transform,false);
            dockHint=hint;
            var text=hint.GetComponent<Text>();text.font=statusLabel.font;text.fontSize=26;text.color=new Color(.65f,.82f,.93f);text.alignment=TextAnchor.MiddleCenter;text.text="Roll to recruit · Drag to arrange";text.raycastTarget=false;
            BattleUiSkin.Place(text.rectTransform,new Vector2(.5f,0),new Vector2(0,34),new Vector2(950,30));
            OnCoinsChanged(roll.Run.Coins);
        }

        void StyleActions()
        {
            BattleChrome.Button(rollButton,1);
            BattleUiSkin.Skin(startButton.GetComponent<Image>(),BattleUiSkin.Gold);
            startButton.GetComponent<Image>().preserveAspect=false;ApprovedUiArt.GoldStates(startButton);
            var swords=new GameObject("Start swords",typeof(RectTransform),typeof(Image)).GetComponent<Image>();swords.transform.SetParent(startButton.transform,false);swords.sprite=ApprovedUiArt.Get("battle");swords.preserveAspect=true;swords.raycastTarget=false;BattleUiSkin.Place(swords.rectTransform,Vector2.one*.5f,Vector2.zero,new Vector2(150,140));
            ActionLabel(rollButton,"",Vector2.zero,new Vector2(160,130),48);
            ActionLabel(startButton,"",Vector2.zero,new Vector2(270,130),50);
            BattleChrome.Draw(rollButton.transform,1,new Vector2(-95,0),new Vector2(170,150));
            var coin=BattleChrome.Draw(rollButton.transform,1,new Vector2(75,0),new Vector2(65,65));coin.sprite=ApprovedUiArt.Get("coin");
            var price=new GameObject("RollPrice",typeof(RectTransform),typeof(Text));price.transform.SetParent(rollButton.transform,false);
            rollPrice=price.GetComponent<Text>();rollPrice.font=statusLabel.font;rollPrice.fontSize=44;rollPrice.fontStyle=FontStyle.Bold;
            rollPrice.color=Color.white;rollPrice.alignment=TextAnchor.MiddleCenter;rollPrice.raycastTarget=false;
            BattleUiSkin.Place(rollPrice.rectTransform,Vector2.one*.5f,new Vector2(155,0),new Vector2(110,100));
            var coinParent=coinLabel.transform.parent;
            ActionIcon(coinParent,2,new Vector2(-170,0),new Vector2(55,55));
            BattleUiSkin.Place(coinLabel.rectTransform,Vector2.one*.5f,new Vector2(25,0),new Vector2(330,70));
        }
        void ActionLabel(Button button,string value,Vector2 pos,Vector2 size,int fontSize)
        {
            foreach(var name in new[]{"Pictogram","ActionIcon"}){var old=button.transform.Find(name);if(old)old.gameObject.SetActive(false);}
            var label=button.GetComponentInChildren<Text>();if(!label)return;
            label.text=value;label.fontSize=fontSize;label.fontStyle=FontStyle.Bold;label.alignment=TextAnchor.MiddleCenter;label.raycastTarget=false;
            BattleUiSkin.Place(label.rectTransform,Vector2.one*.5f,pos,size);
        }
        void ActionIcon(Transform parent,int index,Vector2 pos,Vector2 size)
        {
            var go=new GameObject("BattleIcon",typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var image=go.GetComponent<Image>();BattleControlArt.Skin(image,index);image.raycastTarget=false;
            BattleUiSkin.Place(image.rectTransform,Vector2.one*.5f,pos,size);
        }

        void Awake()
        {
            var canvas=GetComponent<Canvas>();if(canvas)canvas.enabled=true;
            SharedSettings.InstallInitialScene();
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
            if (roll?.Run != null) UpdateButtons(roll.Run.Coins);
            var run=roll?.Run;
            var buffs=run!=null?new Vector4(run.HpPct,run.DmgPct,run.AtkSpeedPct,run.CritChance):Vector4.zero;
            if(!buffs.Equals(displayedBuffs)){displayedBuffs=buffs;RefreshBuffs();}
            bool prep=flow && flow.Current==BattleFlow.Phase.Prep;
            if(actionDock)actionDock.SetActive(prep);
            if(dockHint)dockHint.SetActive(prep);
            if(coinLabel)coinLabel.transform.parent.gameObject.SetActive(prep);
            if(speedButton)BattleUiSkin.Place(speedButton.GetComponent<RectTransform>(),prep?new Vector2(1,0):new Vector2(0,1),prep?new Vector2(-60,244):new Vector2(32,-360),new Vector2(220,72));
            var ally  = UnitRegistry.CountAlive(UnitTeam.Ally);
            var enemy = UnitRegistry.CountAlive(UnitTeam.Enemy);

            if (ally != shownAlly && allyLabel != null)
            {
                allyLabel.text = $"Allies {ally}";
                shownAlly = ally;
            }
            if (enemy != shownEnemy && enemyLabel != null)
            {
                enemyLabel.text = $"Enemy {enemy}";
                shownEnemy = enemy;
            }
        }

        void OnCoinsChanged(int coins)
        {
            AnimatedCounter.Set(coinLabel, coins, "Gold  ",false,
                roll != null && roll.IsPresenting ? .7f : 0f);
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
                rollButton.gameObject.SetActive(prep && !looking && canRoll);
                rollButton.interactable = canRoll && !(reel && reel.IsSpinning);
            }
            if (startButton != null)
            { startButton.gameObject.SetActive(prep && !looking && !canRoll);startButton.interactable = !canRoll && !(reel && reel.IsSpinning); }

            RefreshBuffs();
            RefreshStatus();
        }

        void UpdateButtons(int coins)
        {
            if(rollPrice)rollPrice.text=roll.RollCost.ToString("N0");
            bool busy=SharedSettings.IsPaused || (reel && reel.IsSpinning);
            bool visible=flow && flow.Current==BattleFlow.Phase.Prep && !(boards && boards.ShowingEnemy);
            if(rollButton)rollButton.gameObject.SetActive(visible && coins>=roll.RollCost);
            if(startButton)startButton.gameObject.SetActive(visible && coins<roll.RollCost);
            if(rollButton)rollButton.interactable=!busy && coins>=roll.RollCost;
            if(startButton)startButton.interactable=!busy && coins<roll.RollCost;
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
            txt.text = looking ? "Allies" : "Enemy";
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
            // This strip represents earned Roll buffs, not permanent Relic bonuses.
            var run=roll?.Run;
            Set(0, run?.HpPct ?? 0f);
            Set(1, run?.DmgPct ?? 0f);
            Set(2, run?.AtkSpeedPct ?? 0f);
            Set(3, run?.CritChance ?? 0f);

            void Set(int i, float value)
            {
                AnimatedCounter.Set(buffValues[i], value, "+", true,
                    roll != null && roll.IsPresenting ? .85f : 0f);
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
            var w = $"Wave {flow.Wave}";
            statusLabel.text = phase switch
            {
                BattleFlow.Phase.Prep     => w,
                BattleFlow.Phase.Marching => w + " · Deploying",
                BattleFlow.Phase.Fighting => w + " · Battle",
                BattleFlow.Phase.Finished => flow.Winner switch
                {
                    UnitTeam.Ally  => "Victory",
                    UnitTeam.Enemy => "Defeat",
                    _              => "Draw",
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
