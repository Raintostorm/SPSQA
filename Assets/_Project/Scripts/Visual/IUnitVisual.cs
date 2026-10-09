using System;

namespace Project.Visual
{
    /// <summary>
    /// Hợp đồng giữa COMBAT và ANIMATION.
    /// Combat gọi interface này và KHÔNG biết bên dưới vẽ bằng gì — nhờ vậy
    /// đổi pipeline animation không phải sửa combat.
    ///
    /// Bản cài đặt duy nhất hiện nay: FrameUnitVisual (khung hình rời).
    /// Spine 3.8 từng được thử rồi bỏ — xem Handoff/09 mục 7.
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

        /// <summary>
        /// Ẩn/hiện phần hình ảnh mà không tắt GameObject. Lính đang xếp hàng
        /// vẫn phải nằm trong UnitRegistry, chỉ không được lộ trước khi ra sân.
        /// </summary>
        void SetVisible(bool visible);

        /// <summary>
        /// Phép ngược của PlayDeath: sống lại, hiện rõ, chạy lại Idle.
        ///
        /// Cần vì lính chết KHÔNG bị xoá — sang wave sau nó đứng lại chỗ cũ.
        /// Thiếu hàm này thì nó sống lại nhưng vẫn nằm im ở khung chết, mờ
        /// tịt, và animator vẫn đứng ở speed 0.
        /// </summary>
        void Revive();

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
