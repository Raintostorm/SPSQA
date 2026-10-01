using System;

namespace Project.Visual
{
    /// <summary>
    /// Hợp đồng giữa COMBAT và ANIMATION.
    /// Combat gọi interface này và KHÔNG biết bên dưới là Spine, sprite-frame
    /// hay gì khác — nhờ vậy đổi pipeline animation không phải sửa combat.
    /// </summary>
    public interface IUnitVisual
    {
        /// <param name="onImpact">
        /// Gọi ĐÚNG MỘT LẦN tại khoảnh khắc vũ khí chạm. Đây là lúc trừ máu.
        /// Không phải lúc bắt đầu vung — cảm giác đánh đấm nằm ở chỗ này.
        /// </param>
        /// <param name="onFinished">Gọi khi clip attack kết thúc.</param>
        void PlayAttack(Action onImpact, Action onFinished = null);

        void SetMoving(bool isMoving);
        void PlayIdle();
        void PlayHit();
        void PlayDeath();

        /// <param name="dirX">&lt;0 quay trái, &gt;0 quay phải.</param>
        void SetFacing(float dirX);

        /// <summary>
        /// Bội số tốc độ phát. Buff AtkSpeed +50% → truyền 1.5f.
        /// Handoff 04 mục 6 chưa có method này; thêm vào vì buff AtkSpeed
        /// phải tua nhanh animation chứ không chỉ rút ngắn bộ đếm giờ.
        /// </summary>
        void SetAttackSpeedMultiplier(float multiplier);
    }
}
