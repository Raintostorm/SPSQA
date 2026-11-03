using System;
using UnityEngine;

namespace Project.Data
{
    /// <summary>
    /// Bộ khung hình của một con lính: đứng, đi, đánh, chết, chiêu.
    ///
    /// Vì sao là ScriptableObject chứ không phải AnimatorController: lính
    /// chạy khung hình bằng mã (<c>FrameUnitVisual</c>), không qua Animator.
    /// Lý do nằm ở chỗ Animator bắt phải dùng animation event để biết lúc
    /// nào đòn đánh chạm và lúc nào clip xong — mà event thiếu thì lính
    /// đứng chết cứng và KHÔNG CÓ LOG NÀO. Đã vấp đúng thế.
    ///
    /// Đọc một số thay vì đợi một sự kiện thì không có gì để thiếu.
    /// </summary>
    [CreateAssetMenu(menuName = "SpinSquad/Unit Animation Set")]
    public sealed class UnitAnimationSet : ScriptableObject
    {
        [Serializable]
        public sealed class Clip
        {
            public Sprite[] Frames;

            [Tooltip("Giây cho từng khung. Phải cùng số phần tử với Frames.")]
            public float[] Durations;

            [Tooltip("Thứ tự phát, đếm từ 0. Để trống là chạy thẳng 0,1,2,…")]
            public int[] Sequence;

            public bool Loop = true;

            [Tooltip("Bật khi clip tự khai báo cách phát. Clip cũ để tắt sẽ giữ " +
                     "quy tắc idle/walk phát tới-lui như trước.")]
            public bool UseExplicitPlayback;

            [Tooltip("Phát tới rồi phát lui. Chỉ đọc khi UseExplicitPlayback bật.")]
            public bool PingPong;

            public bool IsValid => Frames != null && Frames.Length > 0;

            public int Count => Sequence != null && Sequence.Length > 0
                ? Sequence.Length : (Frames?.Length ?? 0);

            /// <summary>Sprite của bước thứ <paramref name="step"/> trong thứ tự phát.</summary>
            public Sprite FrameAt(int step)
            {
                if (!IsValid) return null;
                var i = Sequence != null && Sequence.Length > 0
                    ? Sequence[Mathf.Clamp(step, 0, Sequence.Length - 1)]
                    : step;
                return Frames[Mathf.Clamp(i, 0, Frames.Length - 1)];
            }

            public float DurationAt(int step)
            {
                if (Durations == null || Durations.Length == 0) return 0.25f;
                return Mathf.Max(0.01f, Durations[Mathf.Clamp(step, 0, Durations.Length - 1)]);
            }

            public float Total
            {
                get
                {
                    var t = 0f;
                    for (var i = 0; i < Count; i++) t += DurationAt(i);
                    return t;
                }
            }
        }

        public Clip Idle;
        public Clip Walk;
        public Clip Attack;
        public Clip EmpoweredAttack;
        public Clip Dead;

        [Tooltip("Chỉ bậc Legendary có. Để trống là không có chiêu.")]
        public Clip Skill;

        [Tooltip("Chuyển động và effect đã bake từ demo: không cộng nhún/lao lần thứ hai.")]
        public bool BakedMotion;
        [Tooltip("Source sprites face left instead of the default right.")]
        public bool SourceFacesLeft;

        [Header("Hiệu ứng hình ảnh — để trống nếu nhân vật chưa có VFX")]
        public Clip WalkVfx;
        public Clip AttackVfx;
        public Clip DeathVfx;
        public Clip SkillVfx;
        public int SkillVfxStartStep;
        public int SkillVfxEndStep;
        public Vector2[] SkillVfxOffsets;
        public float SkillVfxScale = 1f;
        public int SkillImpactStep = -1;
        public Vector2 WalkVfxOffset;
        [Min(0f)] public float WalkVfxFootSpacing = 0.1f;
        public Vector2 AttackVfxOffset;
        public Vector2 DeathVfxOffset;
        [Min(0.01f)] public float WalkVfxScale = 1f;
        [Min(0.01f)] public float AttackVfxScale = 1f;
        [Min(0.01f)] public float DeathVfxScale = 1f;
        [Range(0f, 1f)] public float WalkVfxAlpha = 1f;
        [Range(0f, 1f)] public float AttackVfxAlpha = 1f;
        [Range(0f, 1f)] public float DeathVfxAlpha = 1f;

        [Tooltip("Đòn ra sát thương ở bước thứ mấy của clip đánh, đếm từ 0. " +
                 "Thay cho animation event — không có gì để quên đặt.")]
        public int ImpactStep = -1;

        /// <summary>
        /// Bước thực sự trừ máu. -1 nghĩa là "giữa clip".
        ///
        /// Trước đây mặc định cứng là 2, đúng khi clip có 8 khung. Bộ art mới
        /// chỉ 3 khung mỗi clip (đo trên game tham chiếu: trung vị 3 khung cho
        /// idle và attack, 2 cho walk, 1 cho dead) — lúc đó bước 2 là khung
        /// CUỐI, tức đánh xong mới trừ máu, cảm giác hụt hẳn.
        ///
        /// Lấy giữa clip thì đúng với mọi độ dài, không phải nhớ sửa.
        /// </summary>
        public int ImpactStepFor(Clip clip)
        {
            if (ImpactStep >= 0) return ImpactStep;
            var n = clip?.Sequence != null && clip.Sequence.Length > 0
                ? clip.Sequence.Length
                : (clip?.Frames?.Length ?? 1);
            return Mathf.Max(0, (n - 1) / 2);
        }
    }
}
