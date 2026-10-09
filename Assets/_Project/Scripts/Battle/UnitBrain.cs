using Project.Data;
using Project.Visual;
using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Một con lính biết phải làm gì: tìm địch, trượt tới, đánh.
    ///
    /// Tách khỏi Unit vì hai việc khác nhau — Unit biết mình còn bao nhiêu
    /// máu, Brain biết phải làm gì tiếp. Unit sống được mà không có Brain
    /// (xác, lính trưng bày trong UI); Brain không sống được nếu thiếu Unit.
    ///
    /// Máy trạng thái chỉ có ba trạng thái, cố ý giữ nhỏ:
    ///   Idle    — chưa có địch
    ///   Chasing — có địch, chưa vào tầm
    ///   Fighting— trong tầm, đang đánh hoặc đang chờ hồi
    /// </summary>
    [RequireComponent(typeof(Unit))]
    public sealed class UnitBrain : MonoBehaviour
    {
        enum State { Idle, Chasing, Fighting }

        [Tooltip("Giây giữa hai lần quét tìm địch. Không quét mỗi frame — " +
                 "địch không dịch chuyển tức thời, 0.2s là quá đủ.")]
        [SerializeField] float retargetInterval = 0.2f;

        [Tooltip("Điểm sinh đạn. Để trống thì đạn sinh ngay tại gốc lính. " +
                 "Chỉ dùng khi UnitDefinition.IsRanged = true.")]
        [SerializeField] Transform muzzle;

        Unit self;
        IUnitVisual visual;
        UnitDefinition def;

        [Header("Chó canh — chỉ để bắt lỗi, không phải luật chơi")]
        [Tooltip("Giây một cú vung được phép kéo dài. Quá thì coi như clip " +
                 "không bắn sự kiện kết thúc, tự gỡ kẹt và kêu một tiếng.")]
        [SerializeField] float attackTimeout = 4f;

        [Tooltip("Giây đứng yên trong lúc ĐANG ĐUỔI thì coi là kẹt.")]
        [SerializeField] float stallTimeout = 2.5f;

        State state = State.Idle;
        Unit target;
        float retargetTimer;
        float cooldownTimer;
        bool attackInProgress;

        float attackClock;          // bao lau roi ke tu luc bat dau vung
        float stallClock;           // bao lau roi ma chua nhich duoc
        float lastX;
        bool  warned;               // moi con keu DUNG MOT LAN

        void Awake()
        {
            self = GetComponent<Unit>();
            visual = GetComponent<IUnitVisual>();
        }

        void OnEnable()
        {
            // Nghe tin mình chết để tự tắt. Dùng event thay vì kiểm tra
            // self.IsAlive mỗi frame — rẻ hơn, và quan trọng hơn là nó chạy
            // ĐÚNG MỘT LẦN nên không có chuyện dọn dẹp hai lượt.
            if (self != null) self.Died += OnSelfDied;
        }

        void OnDisable()
        {
            if (self != null) self.Died -= OnSelfDied;
        }

        void Start()
        {
            def = self.Definition;
            if (def == null) enabled = false;
        }

        void Update()
        {
            if (!self.IsAlive) return;

            TickTimers();
            Watchdog();
            EnsureTarget();

            if (target == null)
            {
                GoIdle();
                return;
            }

            FaceTarget();

            // Đang vung thì KHÔNG đụng vào gì cả: không đi, không đánh tiếp.
            // Thiếu chốt này thì lính vừa trượt vừa chém, và clip bị restart
            // giữa chừng nên onImpact không bao giờ bắn.
            if (attackInProgress) return;

            if (DistanceToTarget() > def.AttackRange)
            {
                Chase();
                return;
            }

            Fight();
        }

        // ── các bước trong Update ─────────────────────────────────────

        void TickTimers()
        {
            retargetTimer -= Time.deltaTime;
            cooldownTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Hai kiểu kẹt vĩnh viễn, cả hai đều IM LẶNG nếu không canh.
        ///
        /// 1. Cú vung không bao giờ xong. attackInProgress chặn mọi thứ ở
        ///    đầu Update, nên lính đứng chết cứng giữa trận mà log sạch trơn.
        ///    Xảy ra khi clip thiếu sự kiện kết thúc, hoặc animator.speed
        ///    mắc kẹt ở 0 sau khi chết rồi hồi sinh.
        ///
        /// 2. Đuổi mà không nhích. Chase() chỉ đổi trục X, nên nếu địch nằm
        ///    đúng cùng X mà lệch Y quá tầm đánh thì vòng lặp này chạy mãi.
        ///
        /// Gỡ kẹt rồi kêu MỘT lần mỗi con — kêu mỗi frame thì Console ngập
        /// và chính nó thành thứ làm game đứng.
        /// </summary>
        void Watchdog()
        {
            if (attackInProgress)
            {
                attackClock += Time.deltaTime;
                if (attackClock > attackTimeout)
                {
                    Report($"cú vung kéo dài {attackClock:0.0}s mà chưa xong — " +
                           $"clip '{(def != null ? def.name : "?")}' nhiều khả năng " +
                           $"không bắn sự kiện kết thúc. Tự gỡ kẹt.");
                    OnAttackFinished();
                }
            }
            else attackClock = 0f;

            if (state == State.Chasing && Mathf.Abs(transform.position.x - lastX) < 0.005f)
            {
                stallClock += Time.deltaTime;
                if (stallClock > stallTimeout)
                {
                    var d = target != null ? DistanceToTarget() : -1f;
                    var dy = target != null
                        ? Mathf.Abs(target.transform.position.y - transform.position.y) : -1f;
                    Report($"đuổi {stallClock:0.0}s mà không nhích. " +
                           $"cách địch {d:0.00} (tầm {def?.AttackRange:0.00}), " +
                           $"lệch trục y {dy:0.00}, tốc độ {def?.MoveSpeed:0.00}.");
                    stallClock = 0f;
                }
            }
            else { stallClock = 0f; lastX = transform.position.x; }
        }

        void Report(string what)
        {
            if (warned) return;
            warned = true;
            Debug.LogWarning($"[UnitBrain] '{name}' ({self.Team}) {what}", this);
        }

        void EnsureTarget()
        {
            // Đổi mục tiêu khi: chưa có, mục tiêu chết, hoặc tới hẹn quét lại.
            var needsNew = target == null || !target.IsAlive;
            if (!needsNew && retargetTimer > 0f) return;

            retargetTimer = retargetInterval;
            target = UnitRegistry.FindNearestEnemy(transform.position, self.Team);
        }

        float DistanceToTarget() =>
            Vector3.Distance(transform.position, target.transform.position);

        void FaceTarget()
        {
            visual?.SetFacing(target.transform.position.x - transform.position.x);
        }

        void GoIdle()
        {
            if (state == State.Idle) return;
            state = State.Idle;
            visual?.SetMoving(false);
        }

        void Chase()
        {
            if (state != State.Chasing)
            {
                state = State.Chasing;
                visual?.SetMoving(true);
            }

            // Đi theo TRỤC X, giữ nguyên y của mình.
            //
            // Trước đây MoveTowards cả hai trục về phía địch, nên mọi con dồn
            // hết về một điểm — xếp đội hình tam giác xong ra đường là tan,
            // nhìn lộn xộn. Giữ y thì độ lệch đội hình được bảo toàn suốt
            // chặng đi, hàng lối còn nguyên tới lúc chạm trán.
            //
            // Hợp lệ vì đây là trận ĐƯỜNG THẲNG: hai phe luôn ở hai đầu trục
            // x, không bao giờ cần đi lên xuống để tới được nhau.
            var p = transform.position;
            var t = target.transform.position;
            p.x = Mathf.MoveTowards(p.x, t.x, def.MoveSpeed * Time.deltaTime);

            // Luoi an toan cho mot the be tac: DistanceToTarget do theo CA HAI
            // truc, con doan tren chi di theo X. Neu lech truc Y mot minh no
            // da lon hon tam danh thi di het co X van khong bao gio vao tam —
            // vong Chase chay mai, linh dung yen, khong co gi bao.
            //
            // Hien tai khong xay ra: doi hinh chi gian 0.065 don vi theo y,
            // con tam danh nho nhat la 3.4. Nhung do la hai con so o hai file
            // khac nhau, khong ai rang buoc chung. Nen vuot nguong thi khep
            // Y lai — bang mot nua toc do, de doi hinh van giu duoc hinh.
            var dy = t.y - p.y;
            if (Mathf.Abs(dy) > def.AttackRange * 0.9f)
                p.y = Mathf.MoveTowards(p.y, t.y, def.MoveSpeed * 0.5f * Time.deltaTime);

            transform.position = p;
        }

        void Fight()
        {
            if (state != State.Fighting)
            {
                state = State.Fighting;
                visual?.SetMoving(false);
            }

            if (cooldownTimer > 0f) return;

            attackInProgress = true;
            visual?.PlayAttack(OnImpact, OnAttackFinished);
        }

        // ── hai callback của animation ────────────────────────────────

        /// <summary>
        /// Khung vũ khí chạm (melee) hoặc khung nhả đạn (ranged).
        /// Tên "impact" là của IUnitVisual; với ranged nó mang nghĩa "bắn ra".
        /// </summary>
        void OnImpact()
        {
            // Địch có thể chết trong lúc mình đang vung.
            if (target == null || !target.IsAlive) return;

            // Quay chí mạng MỘT LẦN cho cả hai lối đánh. Quay riêng trong
            // FireProjectile thì cận chiến và đánh xa thành hai luật khác nhau.
            var damage = RollDamage();
            if (def.IsRanged) FireProjectile(damage);
            else target.TakeDamage(damage, self);
        }

        /// <summary>Sát thương một nhát, đã quay chí mạng.</summary>
        float RollDamage()
        {
            var dmg = self.AttackPower;
            var crit = RunBuffs.Crit(self.Team);
            if (crit > 0f && Random.value < crit)
                dmg *= RunBuffs.CritMultiplier;
            return dmg;
        }

        /// <summary>
        /// Nhịp nghỉ sau đòn, đã chia cho tốc đánh.
        ///
        /// CHIA chứ không trừ: +100% tốc đánh nghĩa là nghỉ còn một nửa, dù
        /// nhịp gốc là bao nhiêu. Trừ thẳng thì con nhịp ngắn hoá ra đánh
        /// liên tục còn con nhịp dài gần như không đổi.
        /// </summary>
        float Cooldown =>
            def.AttackCooldown / (1f + Mathf.Max(0f, RunBuffs.Speed(self.Team)));

        void OnAttackFinished()
        {
            attackInProgress = false;

            // Hồi chiêu tính TỪ LÚC CLIP XONG, không phải từ lúc bắt đầu vung.
            // Đó là nghĩa của UnitDefinition.AttackCooldown.
            cooldownTimer = Cooldown;
        }

        void FireProjectile(float damage)
        {
            if (def.ProjectilePrefab == null)
            {
                Debug.LogWarning(
                    $"[UnitBrain] '{name}' đánh xa nhưng UnitDefinition " +
                    $"'{def.name}' chưa gán ProjectilePrefab. Cú này mất trắng.",
                    this);
                return;
            }

            var origin = muzzle != null ? muzzle.position : transform.position;
            var go = Instantiate(def.ProjectilePrefab, origin, Quaternion.identity);

            var p = go.GetComponent<Projectile>();
            if (p == null)
            {
                Debug.LogError(
                    $"[UnitBrain] ProjectilePrefab '{def.ProjectilePrefab.name}' " +
                    $"không có component Projectile.", this);
                Destroy(go);
                return;
            }

            // Truyen "self" de dan biet ai ban: ghi cong luc TRUNG chu khong
            // luc ban. Ghi luc ban thi cu truot van duoc tinh diem.
            p.Launch(target, damage,
                     def.ProjectileSpeed, def.ProjectileArcHeight, self);
        }

        void OnSelfDied(Unit _)
        {
            target = null;
            attackInProgress = false;
            attackClock = stallClock = 0f;
            enabled = false;      // ngừng Update, xác nằm yên
        }
    }
}
