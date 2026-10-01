using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>Bản cài đặt IUnitVisual cho Spine. Một file = một việc: dịch
    /// lệnh của combat thành lệnh của Spine.</summary>
    [RequireComponent(typeof(SkeletonAnimation))]
    public sealed class SpineUnitVisual : MonoBehaviour, IUnitVisual
    {
        const int Track = 0;

        [Header("Tên animation trong file Spine")]
        [SerializeField] string idleName = "idle";
        [SerializeField] string walkName = "walk";
        [SerializeField] string attackName = "attack";
        [SerializeField] string hitName = "hit";
        [SerializeField] string deathName = "dead";

        [Header("Tên event trong clip attack")]
        [SerializeField] string impactEventName = "impact";

        SkeletonAnimation skeletonAnimation;
        Spine.AnimationState state;

        Action pendingImpact;
        Action pendingFinished;
        bool isDead;
        float attackSpeedMultiplier = 1f;

        void Awake()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            state = skeletonAnimation.AnimationState;

            state.Event += HandleSpineEvent;
            state.Complete += HandleSpineComplete;

            WarnIfMissing(idleName, walkName, attackName, deathName);
            WarnIfImpactEventMissing();
        }

        void OnDestroy()
        {
            if (state == null) return;
            state.Event -= HandleSpineEvent;
            state.Complete -= HandleSpineComplete;
        }

        // ── IUnitVisual ───────────────────────────────────────────────

        public void PlayIdle()
        {
            ClearPending();
            Play(idleName, loop: true);
        }

        public void SetMoving(bool isMoving)
        {
            ClearPending();
            Play(isMoving ? walkName : idleName, loop: true);
        }

        public void PlayAttack(Action onImpact, Action onFinished = null)
        {
            pendingImpact = onImpact;
            pendingFinished = onFinished;

            var entry = Play(attackName, loop: false, restart: true);
            if (entry != null)
                entry.TimeScale = attackSpeedMultiplier;
        }

        /// <summary>KHÔNG clear pending: bị đánh giữa lúc đang vung thì cú vung
        /// đó vẫn phải ra damage, nếu không lính bị đánh liên tục sẽ không bao
        /// giờ gây được damage.</summary>
        public void PlayHit()
        {
            Play(hitName, loop: false, restart: true);
        }

        public void PlayDeath()
        {
            ClearPending();
            isDead = true;
            PlayInternal(deathName, loop: false);
        }

        /// <summary>Chỗ DUY NHẤT trong cả project lật hướng nhân vật.
        /// Không bao giờ dùng đồng thời với Skeleton.ScaleX.</summary>
        public void SetFacing(float dirX)
        {
            if (Mathf.Abs(dirX) < 0.01f) return;
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (dirX < 0f ? -1f : 1f);
            transform.localScale = s;
        }

        public void SetAttackSpeedMultiplier(float multiplier)
        {
            attackSpeedMultiplier = Mathf.Max(0.01f, multiplier);

            var current = state.GetCurrent(Track);
            if (current != null && current.Animation.Name == attackName)
                current.TimeScale = attackSpeedMultiplier;
        }

        // ── Nội bộ ────────────────────────────────────────────────────

        /// <param name="restart">
        /// Mặc định false. Nếu thiếu kiểm tra này, SetMoving(true) được gọi mỗi
        /// frame sẽ restart clip walk MỖI FRAME → nhân vật dậm chân tại chỗ,
        /// không bao giờ qua frame 2. Đây là lỗi "walk trông như không đi".
        /// </param>
        TrackEntry Play(string animationName, bool loop, bool restart = false)
        {
            if (isDead) return null;
            if (string.IsNullOrEmpty(animationName)) return null;

            var current = state.GetCurrent(Track);
            if (!restart && current != null && current.Animation.Name == animationName)
                return current;

            return PlayInternal(animationName, loop);
        }

        TrackEntry PlayInternal(string animationName, bool loop)
        {
            if (skeletonAnimation.Skeleton.Data.FindAnimation(animationName) == null)
                return null;                       // đã cảnh báo ở Awake

            return state.SetAnimation(Track, animationName, loop);
        }

        void HandleSpineEvent(TrackEntry entry, Spine.Event e)
        {
            if (e.Data.Name != impactEventName) return;

            var impact = pendingImpact;
            pendingImpact = null;                  // nhả TRƯỚC khi gọi → không
            impact?.Invoke();                      // thể gọi hai lần, và callback
        }                                          // gọi lại PlayAttack vẫn an toàn

        void HandleSpineComplete(TrackEntry entry)
        {
            if (isDead) return;
            if (entry.Animation.Name != attackName) return;

            // Mạng lưới an toàn: clip attack kết thúc mà event impact chưa bắn
            // (quên đặt event trong Spine) thì vẫn phải ra damage — nhưng KÊU
            // một lần, không nuốt lỗi im lặng.
            if (pendingImpact != null)
            {
                Debug.LogWarning(
                    $"[SpineUnitVisual] '{attackName}' kết thúc mà chưa bắn event " +
                    $"'{impactEventName}'. Damage vẫn ra, nhưng sai thời điểm. " +
                    $"Kiểm tra event trong Spine. ({name})", this);

                var impact = pendingImpact;
                pendingImpact = null;
                impact?.Invoke();
            }

            var finished = pendingFinished;
            pendingFinished = null;
            finished?.Invoke();

            Play(idleName, loop: true);
        }

        void ClearPending()
        {
            pendingImpact = null;
            pendingFinished = null;
        }

        void WarnIfMissing(params string[] names)
        {
            var data = skeletonAnimation.Skeleton.Data;
            foreach (var n in names)
            {
                if (string.IsNullOrEmpty(n) || data.FindAnimation(n) != null) continue;
                Debug.LogWarning(
                    $"[SpineUnitVisual] Không tìm thấy animation '{n}' trong " +
                    $"skeleton. Lệnh tương ứng sẽ không làm gì. ({name})", this);
            }
        }

        void WarnIfImpactEventMissing()
        {
            var attack = skeletonAnimation.Skeleton.Data.FindAnimation(attackName);
            if (attack == null) return;

            foreach (var timeline in attack.Timelines)
                if (timeline is EventTimeline) return;

            Debug.LogWarning(
                $"[SpineUnitVisual] Clip '{attackName}' KHÔNG có event nào. " +
                $"Damage sẽ rơi vào lúc clip kết thúc thay vì lúc vũ khí chạm. " +
                $"Thêm event '{impactEventName}' trong Spine. ({name})", this);
        }
    }
}
