using System;
using System.Collections;
using Project.Data;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>
    /// Vẽ lính bằng khung hình rời, tự chạy, không qua Animator.
    ///
    /// Thay cho SpriteUnitVisual (hệ ghép thân/đầu/vũ khí). Ba thứ đổi hẳn:
    ///
    /// 1. KHÔNG dùng animation event. Đòn chạm ở bước <c>ImpactStep</c>, clip
    ///    xong khi hết bước cuối — đều là đọc một con số. Event thiếu thì im
    ///    lặng và lính đứng chết cứng; con số thì không thể thiếu.
    ///
    /// 2. CHUYỂN ĐỘNG PHỤ bằng mã, cộng lên trên khung hình. Bộ art chỉ có 4
    ///    khung mỗi animation (đo được: nhịp 4/giây, hình bóng đổi trung bình
    ///    10% giữa hai khung đứng) nên tự nó trông cứng. Nhún, nghiêng và lao
    ///    người là thứ làm nó sống, mà không phải vẽ thêm khung nào.
    ///
    /// 3. ĐẢO CHIỀU khung cho đứng và đi: 1,2,3,4,3,2 biến 4 pose thành 6
    ///    bước. Nhịp cảm nhận tăng rưỡi, miễn phí. KHÔNG dùng cho đánh và
    ///    chết — hai cái đó phải một chiều.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class FrameUnitVisual : MonoBehaviour, IUnitVisual
    {
        enum State { Idle, Walk, Attack, Dead }

        [SerializeField] UnitAnimationSet set;
        [SerializeField] SpriteRenderer target;

        [Header("Chuyển động phụ — sinh bằng mã, không tốn khung hình nào")]
        [Tooltip("Biên nhún dọc lúc đứng, đơn vị thế giới.")]
        [SerializeField] float idleBob = 0.018f;

        [Tooltip("Số nhịp nhún mỗi giây lúc đứng.")]
        [SerializeField] float idleBobHz = 1.1f;

        [Tooltip("Biên nhún dọc lúc đi. Nhanh gấp đôi nhịp bước.")]
        [SerializeField] float walkBob = 0.030f;

        [Tooltip("Độ nghiêng tối đa khi đi, độ.")]
        [SerializeField] float walkTilt = 2.5f;

        [Tooltip("Lao tới bao xa khi vung đòn, đơn vị thế giới.")]
        [SerializeField] float lungeDistance = 0.10f;

        [Header("Bị đánh và chết")]
        [SerializeField] Color flashColor = Color.white;
        [SerializeField] float flashSeconds = 0.09f;
        [SerializeField] float fadeSeconds = 0.6f;

        static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");
        static readonly int FlashColorId = Shader.PropertyToID("_FlashColor");

        MaterialPropertyBlock block;
        bool canFlash;
        bool visualsVisible = true;
        SpriteRenderer walkVfxLeft;
        SpriteRenderer walkVfxRight;
        SpriteRenderer attackVfx;
        SpriteRenderer deathVfx;
        bool deathVfxPlaying;
        int deathVfxStep;
        float deathVfxTimer;

        State state = State.Idle;
        int step;
        float stepTimer;
        float speedMul = 1f;
        bool isDead;

        Action pendingImpact;
        Action pendingFinished;
        bool impactFired;

        Coroutine flashRoutine;
        Vector3 restLocal;          // vi tri goc, moi chuyen dong phu cong len day
        float phase;                // dong ho rieng cho nhun/nghieng

        /// <summary>Gọi lúc sinh lính. UnitSpawner truyền bộ khung hình vào.</summary>
        public void Bind(UnitAnimationSet animations)
        {
            set = animations;
            state = State.Idle;
            step = 0;
            stepTimer = 0f;
            isDead = false;
            deathVfxPlaying = false;
            deathVfxStep = 0;
            deathVfxTimer = 0f;
            SetupVfx();
            Apply();
        }

        void Awake()
        {
            if (target == null) target = GetComponent<SpriteRenderer>();
            restLocal = target.transform.localPosition;
            block = new MaterialPropertyBlock();
            canFlash = target.sharedMaterial != null &&
                       target.sharedMaterial.HasProperty(FlashAmountId);
        }

        // ── vòng chạy ─────────────────────────────────────────────────

        void Update()
        {
            if (set == null) return;
            Tick(Time.deltaTime);
            TickDeathVfx(Time.deltaTime);
            Secondary(Time.deltaTime);
        }

        void Tick(float dt)
        {
            var clip = Current;
            if (clip == null || !clip.IsValid) return;

            // Chi clip DANH chay theo toc do danh. Nhun va di bo khong — tang
            // toc danh ma linh rung lac nhanh hon thi nhin nhu tua bang.
            var scale = state == State.Attack ? Mathf.Max(0.01f, speedMul) : 1f;
            stepTimer += dt * scale;

            var n = Steps(clip);
            while (stepTimer >= StepTime(clip, step))
            {
                stepTimer -= StepTime(clip, step);

                if (state == State.Attack && !impactFired && step >= ImpactStep)
                {
                    impactFired = true;
                    var cb = pendingImpact; pendingImpact = null;
                    cb?.Invoke();
                }

                step++;
                if (step < n) { Apply(); continue; }

                // Het clip.
                if (state == State.Attack) { Finish(); return; }
                if (!clip.Loop) { step = n - 1; stepTimer = 0f; Apply(); return; }
                step = 0;
                Apply();
            }
        }

        /// <summary>
        /// Nhún, nghiêng, lao người. Cộng lên vị trí gốc chứ không gán đè —
        /// gán đè thì mọi chỉnh tay trong Inspector bị xoá mỗi khung hình.
        /// </summary>
        void Secondary(float dt)
        {
            if (isDead) return;
            phase += dt;
            var t = target.transform;

            var bob = 0f;
            var tilt = 0f;
            if (state == State.Walk)
            {
                var w = phase * Mathf.PI * 2f * 2f;      // hai nhip moi vong buoc
                bob = Mathf.Abs(Mathf.Sin(w)) * walkBob;
                tilt = Mathf.Sin(w * 0.5f) * walkTilt;
            }
            else if (state == State.Idle)
            {
                bob = Mathf.Sin(phase * Mathf.PI * 2f * idleBobHz) * idleBob;
            }

            var lunge = 0f;
            if (state == State.Attack)
            {
                var clip = Current;
                var p = clip == null ? 0f : Progress(clip);
                // Len nhanh toi luc cham, ve cham sau do: do la nhip cua mot cu vung.
                lunge = lungeDistance * (p < 0.5f ? p / 0.5f : 1f - (p - 0.5f) / 0.5f);
            }

            t.localPosition = restLocal + new Vector3(lunge, bob, 0f);
            t.localRotation = Quaternion.Euler(0f, 0f, tilt);
        }

        // ── IUnitVisual ───────────────────────────────────────────────

        public void PlayAttack(Action onImpact, Action onFinished = null)
        {
            if (isDead) { onFinished?.Invoke(); return; }

            var clip = set != null ? set.Attack : null;
            if (clip == null || !clip.IsValid)
            {
                // Thieu clip danh thi van PHAI ra damage va PHAI bao xong —
                // khong thi UnitBrain ket o attackInProgress vinh vien.
                Debug.LogWarning($"[FrameUnitVisual] '{name}' không có clip đánh.", this);
                onImpact?.Invoke();
                onFinished?.Invoke();
                return;
            }

            pendingImpact = onImpact;
            pendingFinished = onFinished;
            impactFired = false;
            Switch(State.Attack);
        }

        public void SetMoving(bool isMoving)
        {
            if (isDead || state == State.Attack) return;
            Switch(isMoving ? State.Walk : State.Idle);
        }

        public void PlayIdle()
        {
            if (isDead) return;
            Switch(State.Idle);
        }

        public void PlayHit()
        {
            if (isDead) return;
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(Flash());
        }

        public void SetVisible(bool visible)
        {
            visualsVisible = visible;
            if (target != null) target.enabled = visible;
            ApplyVfx();
        }

        public void PlayDeath()
        {
            if (isDead) return;
            isDead = true;

            // Cat chop sang TRUOC khi mo dan, khong thi xac sang bat tro lai
            // giua chung roi moi mo lan nua.
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
                flashRoutine = null;
                SetFlash(0f);
            }

            pendingImpact = null;
            pendingFinished = null;
            deathVfxPlaying = false;
            Disable(deathVfx);
            target.transform.localPosition = restLocal;
            target.transform.localRotation = Quaternion.identity;
            Switch(State.Dead);
            StartCoroutine(FadeOut());
        }

        public void Revive()
        {
            if (!isDead) return;
            StopAllCoroutines();
            flashRoutine = null;
            isDead = false;
            deathVfxPlaying = false;
            deathVfxStep = 0;
            deathVfxTimer = 0f;
            SetFlash(0f);
            SetAlpha(1f);
            Switch(State.Idle);
        }

        /// <summary>Chỗ DUY NHẤT lật hướng nhân vật. Không dùng cùng flipX.</summary>
        public void SetFacing(float dirX)
        {
            if (Mathf.Abs(dirX) < 0.01f) return;
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (dirX < 0f ? -1f : 1f);
            transform.localScale = s;
        }

        public void SetAttackSpeedMultiplier(float multiplier)
            => speedMul = Mathf.Max(0.01f, multiplier);

        // ── bên trong ─────────────────────────────────────────────────

        UnitAnimationSet.Clip Current => set == null ? null : state switch
        {
            State.Walk => Pick(set.Walk, set.Idle),
            State.Attack => set.Attack,
            State.Dead => Pick(set.Dead, set.Idle),
            _ => set.Idle,
        };

        static UnitAnimationSet.Clip Pick(UnitAnimationSet.Clip a, UnitAnimationSet.Clip fallback)
            => a != null && a.IsValid ? a : fallback;

        int ImpactStep => set == null ? 2 : Mathf.Max(0, set.ImpactStep);

        /// <summary>Số bước của clip, đã tính cả phần đảo chiều.</summary>
        int Steps(UnitAnimationSet.Clip clip)
        {
            var n = clip.Count;
            return PingPong && n > 2 ? n * 2 - 2 : n;
        }

        bool PingPong
        {
            get
            {
                var clip = Current;
                return clip != null && clip.UseExplicitPlayback
                    ? clip.PingPong
                    : state == State.Idle || state == State.Walk;
            }
        }

        int Folded(UnitAnimationSet.Clip clip, int s)
        {
            var n = clip.Count;
            if (!PingPong || n <= 2) return Mathf.Clamp(s, 0, n - 1);
            return s < n ? s : n * 2 - 2 - s;      // 0,1,2,3,2,1
        }

        float StepTime(UnitAnimationSet.Clip clip, int s) => clip.DurationAt(Folded(clip, s));

        float Progress(UnitAnimationSet.Clip clip)
        {
            var total = clip.Total;
            if (total <= 0f) return 0f;
            var done = 0f;
            for (var i = 0; i < step && i < clip.Count; i++) done += clip.DurationAt(i);
            return Mathf.Clamp01((done + stepTimer) / total);
        }

        void Switch(State s)
        {
            if (state == s) return;
            state = s;
            step = 0;
            stepTimer = 0f;
            Apply();
        }

        void Apply()
        {
            var clip = Current;
            if (clip == null || !clip.IsValid || target == null) return;
            target.sprite = clip.FrameAt(Folded(clip, step));
            ApplyVfx();
        }

        void SetupVfx()
        {
            walkVfxLeft = MakeVfxRenderer("WalkVfxLeft", -1, set != null ? set.WalkVfx : null);
            walkVfxRight = MakeVfxRenderer("WalkVfxRight", -1, set != null ? set.WalkVfx : null);
            attackVfx = MakeVfxRenderer("AttackVfx", 1, set != null ? set.AttackVfx : null);
            deathVfx = MakeVfxRenderer("DeathVfx", 1, set != null ? set.DeathVfx : null);
        }

        SpriteRenderer MakeVfxRenderer(string nodeName, int orderDelta, UnitAnimationSet.Clip clip)
        {
            var node = transform.Find(nodeName);
            if (clip == null || !clip.IsValid)
            {
                if (node != null) node.gameObject.SetActive(false);
                return node == null ? null : node.GetComponent<SpriteRenderer>();
            }

            if (node == null)
            {
                var go = new GameObject(nodeName);
                node = go.transform;
                node.SetParent(transform, false);
            }

            node.gameObject.SetActive(true);
            var renderer = node.GetComponent<SpriteRenderer>();
            if (renderer == null) renderer = node.gameObject.AddComponent<SpriteRenderer>();
            renderer.sortingLayerID = target.sortingLayerID;
            renderer.sortingOrder = target.sortingOrder + orderDelta;
            renderer.sharedMaterial = target.sharedMaterial;
            renderer.enabled = false;
            return renderer;
        }

        void ApplyVfx()
        {
            Disable(walkVfxLeft);
            Disable(walkVfxRight);
            Disable(attackVfx);
            Disable(deathVfx);
            if (!visualsVisible || set == null) return;

            switch (state)
            {
                case State.Walk:
                    if (set.WalkVfx != null && set.WalkVfx.IsValid)
                    {
                        var spacing = set.WalkVfxFootSpacing;
                        Show(walkVfxLeft, set.WalkVfx,
                             set.WalkVfxOffset + Vector2.left * spacing,
                             set.WalkVfxScale, set.WalkVfxAlpha, step);
                        Show(walkVfxRight, set.WalkVfx,
                             set.WalkVfxOffset + Vector2.right * spacing,
                             set.WalkVfxScale, set.WalkVfxAlpha,
                             (step + Mathf.Max(1, set.WalkVfx.Count / 2)) % set.WalkVfx.Count);
                    }
                    break;
                case State.Attack:
                    Show(attackVfx, set.AttackVfx, set.AttackVfxOffset,
                         set.AttackVfxScale, set.AttackVfxAlpha, step);
                    break;
                case State.Dead:
                    if (deathVfxPlaying)
                        Show(deathVfx, set.DeathVfx, set.DeathVfxOffset,
                             set.DeathVfxScale, set.DeathVfxAlpha, deathVfxStep);
                    break;
            }
        }

        void Show(SpriteRenderer renderer, UnitAnimationSet.Clip clip, Vector2 offset,
                  float scale, float alpha, int frameStep)
        {
            if (renderer == null || clip == null || !clip.IsValid) return;
            renderer.sprite = clip.FrameAt(Mathf.Clamp(frameStep, 0, clip.Count - 1));
            renderer.transform.localPosition = new Vector3(offset.x, offset.y, 0f);
            renderer.transform.localScale = Vector3.one * Mathf.Max(0.01f, scale);
            renderer.color = new Color(1f, 1f, 1f, Mathf.Clamp01(alpha));
            renderer.enabled = true;
        }

        void StartDeathVfx()
        {
            if (set == null || set.DeathVfx == null || !set.DeathVfx.IsValid) return;
            deathVfxPlaying = true;
            deathVfxStep = 0;
            deathVfxTimer = 0f;
            ApplyVfx();
        }

        void TickDeathVfx(float dt)
        {
            if (!deathVfxPlaying || set == null || set.DeathVfx == null) return;
            var clip = set.DeathVfx;
            deathVfxTimer += dt;
            while (deathVfxTimer >= clip.DurationAt(deathVfxStep))
            {
                deathVfxTimer -= clip.DurationAt(deathVfxStep);
                deathVfxStep++;
                if (deathVfxStep >= clip.Count)
                {
                    deathVfxPlaying = false;
                    Disable(deathVfx);
                    return;
                }
                ApplyVfx();
            }
        }

        static void Disable(SpriteRenderer renderer)
        {
            if (renderer != null) renderer.enabled = false;
        }

        void Finish()
        {
            var cb = pendingFinished;
            pendingImpact = null;
            pendingFinished = null;
            Switch(State.Idle);
            cb?.Invoke();
        }

        IEnumerator Flash()
        {
            SetFlash(1f);
            yield return new WaitForSeconds(flashSeconds);
            SetFlash(0f);
            flashRoutine = null;
        }

        IEnumerator FadeOut()
        {
            // Doi clip chet chay het roi moi mo — mo ngay thi khong ai thay
            // con linh nga xuong.
            var clip = Current;
            if (clip != null && clip.IsValid) yield return new WaitForSeconds(clip.Total);
            StartDeathVfx();

            var t = 0f;
            while (t < fadeSeconds)
            {
                t += Time.deltaTime;
                SetAlpha(1f - t / fadeSeconds);
                yield return null;
            }
            SetAlpha(0f);
        }

        void SetFlash(float amount)
        {
            if (!canFlash || target == null) return;
            target.GetPropertyBlock(block);
            block.SetColor(FlashColorId, flashColor);
            block.SetFloat(FlashAmountId, amount);
            target.SetPropertyBlock(block);
        }

        void SetAlpha(float a)
        {
            if (target == null) return;
            var c = target.color;
            c.a = Mathf.Clamp01(a);
            target.color = c;
        }
    }
}
