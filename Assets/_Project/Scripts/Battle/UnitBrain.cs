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

        [Tooltip("Điểm sinh đạn. Để trống thì dùng điểm nhả theo vũ khí trong khung hình. " +
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
        Unit attackTarget; // Frozen at wind-up; never redirect an in-flight hit.
        Vector3 focusPosition;
        bool hasFocus; // Keep the last position if this enemy dies before contact.
        float retargetTimer;
        float cooldownTimer;
        bool attackInProgress;
        bool empoweredAttack;
        public float SummonedCooldown = -1f, SummonedMoveSpeed = -1f;

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

        public void RefreshDefinition(){def=self.Definition;}

        void Start()
        {
            def = self.Definition;
            if (def == null) enabled = false;
        }

        void Update()
        {
            if (!self.IsAlive) return;
            if (self.Combat.Stunned || self.Combat.Casting || self.Mythic.Casting) return;
            visual?.SetAttackSpeedMultiplier((1f + Mathf.Max(0f, RunBuffs.Speed(self.Team))) * self.Combat.SpeedMultiplier * self.Mythic.SpeedMultiplier * (1f + self.Equipment.SpeedBonus) * self.Effects.SpeedMultiplier);

            TickTimers();
            Watchdog();
            // Finish the current swing even if its victim dies. Do not retarget or
            // rotate the actor until the animation reports completion.
            if (attackInProgress) return;
            EnsureTarget();

            if (!hasFocus)
            {
                GoIdle();
                return;
            }

            FaceTarget();

            if (self.Combat.TryCast(target)) { state = State.Idle; return; }

            if (DistanceToTarget() > EffectiveAttackRange)
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
            if (target != null && target.IsAlive && target.Team != self.Team && target.Mythic.Deployed)
            {
                focusPosition = target.transform.position;
                hasFocus = true;
                return;
            }
            // A death commits one final, harmless swing at the last known position.
            // A living unit that changes sides or leaves the field is not a valid focus.
            if (hasFocus && (target == null || !target.IsAlive)) return;
            hasFocus = false;
            target = null;
            if (retargetTimer > 0f) return;
            retargetTimer = retargetInterval;
            target = UnitRegistry.FindNearestEnemy(transform.position, self.Team);
            if (target != null)
            {
                focusPosition = target.transform.position;
                hasFocus = true;
            }
        }

        float DistanceToTarget() => Vector3.Distance(transform.position, focusPosition);
        public float EffectiveAttackRange
        {
            get { var definition=self?self.Definition:null;return !definition?0:definition.IsRanged?definition.AttackRange:Mathf.Min(definition.AttackRange,.55f); }
        }
        // UI reads the same speed factors as animation playback and cooldown.
        public float AttacksPerSecond
        {
            get
            {
                // In preparation this component is disabled before Start assigns def.
                var definition=self?self.Definition:null;if(!definition)return 0;
                float multiplier=(1f+Mathf.Max(0f,RunBuffs.Speed(self.Team)))*(self.Combat?self.Combat.SpeedMultiplier:1)*(self.Mythic?self.Mythic.SpeedMultiplier:1)*(1f+(self.Equipment?self.Equipment.SpeedBonus:0))*self.Effects.SpeedMultiplier;
                return multiplier/Mathf.Max(.01f,(definition.Animations?.Attack?.Total??0f)+(SummonedCooldown>=0f?SummonedCooldown:definition.AttackCooldown));
            }
        }

        void FaceTarget()
        {
            visual?.SetFacing(focusPosition.x - transform.position.x);
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
            var t = focusPosition;
            var moveSpeed = (SummonedMoveSpeed >= 0f ? SummonedMoveSpeed : def.MoveSpeed)*self.Effects.SpeedMultiplier;
            p.x = Mathf.MoveTowards(p.x, t.x, moveSpeed * Time.deltaTime);

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
            if (Mathf.Abs(dy) > EffectiveAttackRange * 0.9f)
                p.y = Mathf.MoveTowards(p.y, t.y, moveSpeed * 0.5f * Time.deltaTime);

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

            attackTarget = target;
            attackInProgress = true;empoweredAttack=self.Mythic.Charged;
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
            if(self.IsAlive&&!self.Combat.Stunned&&!def.IsRanged)MeleeAttackEffect.Play(self,empoweredAttack);
            var victim = attackTarget;
            if (!self.IsAlive || self.Combat.Stunned || victim == null || !victim.IsAlive || victim.Team==self.Team || !victim.Mythic.Deployed) return;

            // Recheck at contact: a target may move away during wind-up.
            if (!def.IsRanged && Vector3.Distance(transform.position, victim.transform.position) > EffectiveAttackRange + .05f) return;

            // Quay chí mạng MỘT LẦN cho cả hai lối đánh. Quay riêng trong
            // FireProjectile thì cận chiến và đánh xa thành hai luật khác nhau.
            var damage = RollDamage();
            if (def.IsRanged) FireProjectile(damage, victim);
            else {victim.TakeDamage(damage, self);self.Mythic.AfterBasicAttack(victim,empoweredAttack);}
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
            (SummonedCooldown >= 0f ? SummonedCooldown : def.AttackCooldown) / ((1f + Mathf.Max(0f, RunBuffs.Speed(self.Team))) * self.Combat.SpeedMultiplier * self.Mythic.SpeedMultiplier * (1f + self.Equipment.SpeedBonus) * self.Effects.SpeedMultiplier);

        void OnAttackFinished()
        {
            attackInProgress = false;
            attackTarget = null;
            if (target == null || !target.IsAlive || target.Team == self.Team || !target.Mythic.Deployed)
            { target = null; hasFocus = false; retargetTimer = 0f; }

            // Hồi chiêu tính TỪ LÚC CLIP XONG, không phải từ lúc bắt đầu vung.
            // Đó là nghĩa của UnitDefinition.AttackCooldown.
            cooldownTimer = Cooldown;
        }

        void FireProjectile(float damage, Unit victim)
        {
            if (def.ProjectilePrefab == null && self.Mythic.Active)
            {
                var bolt=new GameObject("Mythic energy bolt");bolt.transform.position=RangedShotStyle.Origin(self,muzzle);
                var image=bolt.AddComponent<SpriteRenderer>();image.sortingOrder=151;bolt.transform.localScale=Vector3.one*.45f;
                bolt.AddComponent<MythicProjectileArt>().Bind(self.Mythic.Kind==MythicKind.Clock?"clock-bolt":"elf-bolt");
                bolt.AddComponent<Projectile>().Launch(victim,damage,8,0,self);return;
            }
            if (def.ProjectilePrefab == null)
            {
                Debug.LogWarning(
                    $"[UnitBrain] '{name}' đánh xa nhưng UnitDefinition " +
                    $"'{def.name}' chưa gán ProjectilePrefab. Cú này mất trắng.",
                    this);
                return;
            }

            var origin = RangedShotStyle.Origin(self,muzzle);
            var go = Instantiate(def.ProjectilePrefab, origin, Quaternion.identity);
            if(def.ProjectileSpriteOverride)
            {
                var renderers=go.GetComponentsInChildren<SpriteRenderer>();
                for(int i=0;i<renderers.Length;i++)
                {
                    renderers[i].enabled=i==0;
                    if(i!=0)continue;
                    renderers[i].sprite=def.ProjectileSpriteOverride;
                    renderers[i].color=def.ProjectileTint;
                    renderers[i].transform.localScale=Vector3.one;
                }
            }

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
            p.Launch(victim, damage,
                     def.ProjectileSpeed, def.ProjectileArcHeight, self);
        }

        void OnSelfDied(Unit _)
        {
            target = attackTarget = null;
            hasFocus = false;
            attackInProgress = false;
            attackClock = stallClock = 0f;
            enabled = false;      // ngừng Update, xác nằm yên
        }
    }
}
