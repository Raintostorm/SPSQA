using System;
using Project.Data;
using Project.Visual;
using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Một con lính biết mình còn bao nhiêu máu.
    ///
    /// CHỈ thế thôi. Nó không tìm địch, không quyết định khi nào đánh, không
    /// di chuyển — đó là việc của UnitBrain. Nếu thấy mình muốn thêm field
    /// "target" vào đây thì nghĩa là đang viết nhầm file.
    ///
    /// Hướng phụ thuộc: Unit (LOGIC) gọi xuống IUnitVisual (VIEW).
    /// Chiều ngược lại không bao giờ xảy ra.
    /// </summary>
    public sealed class Unit : MonoBehaviour
    {
        [SerializeField] UnitDefinition definition;

        [Tooltip("Phe nằm ở con lính, không nằm trong UnitDefinition — " +
                 "nhờ vậy ally và enemy dùng chung một asset.")]
        [SerializeField] UnitTeam team = UnitTeam.Ally;

        [Tooltip("Đạn và hiệu ứng nhắm vào đây, tính từ gốc lính. Gốc nằm ở " +
                 "mặt đất nên nhắm thẳng vào transform.position sẽ trúng chân.")]
        [SerializeField] float hitHeight = 0.5f;

        /// <summary>Bắn ĐÚNG MỘT LẦN, ngay khi máu chạm 0.</summary>
        public event Action<Unit> Died;

        /// <summary>
        /// Điểm giữa thân — chỗ đạn nhắm tới, không phải dưới chân.
        /// Dùng TransformPoint để hitHeight CO THEO scale của lính. Cộng thẳng
        /// Vector3.up thì lính thu nhỏ 0.65 vẫn bị nhắm ở độ cao cũ, đạn bay
        /// qua đầu.
        /// </summary>
        public Vector3 HitPoint => transform.TransformPoint(new Vector3(0f, hitHeight, 0f));

        public UnitDefinition Definition => definition;
        public UnitTeam Team => team;
        public IUnitVisual Visual { get; private set; }

        public bool IsAlive { get; private set; }
        public float CurrentHp { get; private set; }

        // Nhân buff TẠI CHỖ ĐỌC, không nhân một lần lúc spawn: buff mua ở
        // wave 3 phải ăn vào cả lính đã đứng trên sân từ wave 1. Nhân lúc
        // spawn thì lính cũ mãi mãi yếu hơn lính mới, mà chẳng có gì nói thế.
        // Ba nguon nhan CHONG len nhau, khong cong gop: cap nang cua con
        // lính, vat pham kho bau, va buff mua trong tran la ba he doc lap.
        // Cong gop thi mot he manh len lam hai he kia nhe di — khong ai
        // mong doi vay.
        public float MaxHp => definition == null ? 0f
            : definition.BaseHp
              * (1f + UnitUpgrades.Bonus(definition.UnitId, team))
              * (1f + RunBuffs.Hp(team));

        public float AttackPower => definition == null ? 0f
            : definition.BaseAttack
              * (1f + UnitUpgrades.Bonus(definition.UnitId, team))
              * (1f + RunBuffs.Dmg(team));

        // Ghi danh vao UnitRegistry de nguoi khac tim duoc, va RUT TEN khi tat.
        // Thieu OnDisable thi xac van nam trong danh sach -> lính khac cu nham
        // vao mot con da chet.
        void OnEnable()  => UnitRegistry.Add(this);
        void OnDisable() => UnitRegistry.Remove(this);

        void Awake()
        {
            Visual = GetComponent<IUnitVisual>();
            if (Visual == null)
                Debug.LogError(
                    $"[Unit] '{name}' không có component nào cài IUnitVisual. " +
                    $"Thêm FrameUnitVisual vào cùng GameObject.", this);

            ResetState();
        }

        /// <summary>
        /// Chỗ DUY NHẤT than phiền về việc thiếu UnitDefinition.
        ///
        /// Không để trong Awake được: UnitSpawner gọi Initialize NGAY SAU
        /// Instantiate, nhưng Awake của con lính đã chạy xong từ bên trong
        /// Instantiate rồi. Báo ở Awake là báo oan mọi con lính sinh lúc
        /// chạy — sáu con sinh ra là sáu dòng đỏ, trong khi chẳng con nào
        /// thiếu gì cả.
        ///
        /// Start chạy sau toàn bộ Awake của khung hình đó, nên lúc này
        /// definition đã được gán xong. Con nào tới đây vẫn trống thì là
        /// lính đặt tay trong scene mà quên gán thật.
        /// </summary>
        void Start()
        {
            if (definition != null) return;
            Debug.LogError(
                $"[Unit] '{name}' chưa gán UnitDefinition. " +
                $"Máu = 0, con này chết ngay khi bị đánh.", this);
        }

        /// <summary>
        /// Dùng khi sinh lính lúc chạy (gacha, evolution). Lính đặt sẵn trong
        /// scene thì không cần gọi — Awake đã lo.
        /// </summary>
        public void Initialize(UnitDefinition def, UnitTeam unitTeam)
        {
            definition = def;
            team = unitTeam;
            ResetState();
        }

        void ResetState()
        {
            // Bang chi so dem CA VAN, khong phai tung wave: linh song sot
            // qua nhieu wave thi thanh tich cong don lai moi co nghia. Dat
            // lai o day la dung — ResetState chi chay luc SINH ra con moi.
            DamageDealt = DamageTaken = Healing = 0f;

            if (definition == null)
            {
                // Im lặng ở đây. Việc kêu là của Start — xem ghi chú ở trên.
                CurrentHp = 0f;
                IsAlive = false;
                return;
            }

            // Gán Ở ĐÂY chứ không gán chỗ khai báo field: lúc field được khởi
            // tạo thì Unity chưa nạp xong 'definition', đọc ra sẽ là null.
            CurrentHp = MaxHp;
            IsAlive = true;
        }

        /// <summary>
        /// Trừ máu. Gọi từ onImpact của bên tấn công — tức đúng khung vũ khí
        /// chạm, không phải lúc bắt đầu vung.
        /// </summary>
        /// <summary>Sát thương con này đã GÂY RA, cộng dồn cả ván.</summary>
        public float DamageDealt { get; private set; }

        /// <summary>Sát thương con này đã CHỊU, cộng dồn cả ván.</summary>
        public float DamageTaken { get; private set; }

        /// <summary>
        /// Máu con này đã HỒI, cộng dồn cả ván.
        ///
        /// Hiện luôn bằng 0: game chưa có cơ chế hồi phục nào. Để sẵn đây
        /// cùng <see cref="Heal"/> để khi thêm thì chỉ phải gọi một chỗ,
        /// không phải sửa cả bảng chỉ số.
        /// </summary>
        public float Healing { get; private set; }

        /// <summary>Ghi công sát thương vừa gây ra cho con khác.</summary>
        public void AddDamageDealt(float amount)
        {
            if (amount > 0f) DamageDealt += amount;
        }

        /// <summary>Hồi máu, không vượt quá máu tối đa.</summary>
        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f) return;
            var before = CurrentHp;
            CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
            Healing += CurrentHp - before;
        }

        /// <summary>
        /// <paramref name="source"/> là con gây ra đòn này — dùng để ghi công
        /// vào bảng chỉ số. Truyền null cho sát thương không của ai (bẫy, độc).
        /// </summary>
        public void TakeDamage(float amount, Unit source = null)
        {
            // Hai con cùng đánh trúng trong một frame: cú thứ hai phải rơi vào
            // hư không. Thiếu dòng này thì Died bắn hai lần và mọi thứ đang
            // nghe event đó (đếm quân, chia thưởng) sẽ sai.
            if (!IsAlive) return;

            if (amount <= 0f) return;

            // Giap tru TRUOC moi phep dem: con so vao bang chi so, con so
            // hien tren man, va so mau that mat phai la cung mot con so.
            amount = RunBuffs.DamageAfterArmor(amount, team);
            if (amount <= 0f) return;

            // Ghi cong theo so MAU THAT SU MAT, khong theo so dinh gay: con
            // sap chet chi con 5 mau ma an cu 200 thi chi tinh 5. Khong vay
            // thi bang chi so cong len cao hon tong mau ca doi dich.
            var real = Mathf.Min(amount, CurrentHp);
            DamageTaken += real;
            source?.AddDamageDealt(real);

            // Hien dung so MAU THAT SU MAT, cung con so vao bang chi so.
            // Hien so dinh gay thi con sap chet con 5 mau an cu 200 se bay
            // len "200" roi nga — nguoi choi doc duoc mot con so khong co
            // that o dau ca.
            Project.Visual.DamagePopup.Show(HitPoint, real, team);

            // Hut mau theo MAU THAT MAT, khong theo so dinh gay. Danh vao
            // con sap chet con 5 mau ma hoi theo 200 thi hut mau thanh ra
            // manh nhat luc doi thu gan het mau — nguoc hoan toan.
            if (source != null && source != this)
            {
                var steal = RunBuffs.Steal(source.Team);
                if (steal > 0f) source.Heal(real * steal);
            }

            CurrentHp -= amount;

            if (CurrentHp > 0f)
            {
                Visual?.PlayHit();
                return;
            }

            CurrentHp = 0f;
            Die();
        }

        /// <summary>
        /// Hồi đầy máu. BattleFlow gọi khi vào lại sân chuẩn bị.
        ///
        /// Không hồi thì lính sống sót qua wave 1 bước vào wave 2 với máu
        /// thủng, và game thành vòng xoáy chết: càng đánh càng yếu, không
        /// cách nào gỡ. Đây là một QUYẾT ĐỊNH LUẬT CHƠI, không phải kỹ thuật
        /// — muốn giữ máu qua wave thì xoá chỗ gọi trong BattleFlow.
        /// </summary>
        public void RestoreFull()
        {
            if (definition == null) return;
            CurrentHp = MaxHp;
            if (IsAlive) return;

            IsAlive = true;
            Visual?.Revive();

            // Thanh máu là con của chính lính này nên tìm bằng GetComponentInChildren.
            // Unit KHÔNG giữ tham chiếu tới nó — hướng phụ thuộc là
            // Unit (LOGIC) → IUnitVisual (VIEW), và HealthBar không nằm sau
            // giao diện đó. Tìm một lần mỗi wave thì rẻ.
            GetComponentInChildren<Project.Visual.HealthBar>(true)?.Restore();
        }

        /// <summary>
        /// Keo CurrentHp len bang MaxHp HIEN TAI, khong hoi sinh ai.
        ///
        /// Vi sao can rieng khoi RestoreFull: buff mua TRONG luc chuan bi,
        /// tuc la SAU khi EnterPrep da hoi mau xong. MaxHp doc buff ngay tai
        /// cho nen no nhay len lap tuc, con CurrentHp thi dung yen — linh
        /// buoc vao wave voi 160/200 mau du vua mua buff mau.
        ///
        /// Chi KEO LEN, khong bao gio keo xuong: mua buff ma mat mau la vo ly.
        /// Va khong dung RestoreFull vi ham do con hoi sinh xac — goi no o
        /// dau wave la xac chet tu wave truoc dung day di danh tiep.
        /// </summary>
        public void TopUpToMax()
        {
            if (!IsAlive || definition == null) return;
            if (CurrentHp < MaxHp) CurrentHp = MaxHp;
        }

        void Die()
        {
            IsAlive = false;
            Visual?.PlayDeath();

            // Xác NẰM LẠI, không Destroy. Lý do: Destroy ngay thì animation
            // chết không kịp chạy, và bên đang giữ tham chiếu tới con này sẽ
            // cầm một MissingReference. Ai dọn xác là việc của BattleFlow (M6).
            // KHÔNG xoá Died nữa.
            //
            // Trước đây gán null để chắc chắn không bắn hai lần. Nhưng chốt
            // thật nằm ở `if (!IsAlive) return` đầu TakeDamage, cộng với
            // IsAlive = false đặt ngay trên — Die() không thể vào lần hai.
            //
            // Và xoá nó thì hỏng chuyện hồi sinh: lính chết rồi sống lại ở
            // wave sau, nhưng HealthBar đã mất đăng ký nên lần chết tiếp theo
            // thanh máu không ẩn đi. Một biện pháp phòng thừa đổi thành một
            // lỗi thật.
            Died?.Invoke(this);
        }

#if UNITY_EDITOR
        // Chỉ để nhìn máu trong Inspector lúc Play. Không ảnh hưởng bản build.
        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || MaxHp <= 0f) return;
            Gizmos.color = IsAlive ? Color.green : Color.red;
            var ratio = CurrentHp / MaxHp;
            var from = transform.position + Vector3.up * 1.3f + Vector3.left * 0.3f;
            Gizmos.DrawLine(from, from + Vector3.right * 0.6f * ratio);
        }
#endif
    }
}
