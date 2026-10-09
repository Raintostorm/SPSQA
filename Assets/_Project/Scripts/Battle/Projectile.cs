using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Một viên đạn bay tới đích rồi gây sát thương.
    ///
    /// Sinh ra bởi UnitBrain lúc onImpact. Sau khi sinh, nó sống đời riêng —
    /// con lính bắn ra không giữ tham chiếu tới nó, và nó không hỏi lại con
    /// lính điều gì. Nhờ vậy lính chết giữa chừng thì đạn vẫn bay tiếp.
    ///
    /// Bay thẳng hay vòng cung do arcHeight quyết định:
    ///   0    → tên, đạn súng, tia laser
    ///   &gt; 0  → lao ném
    /// </summary>
    public sealed class Projectile : MonoBehaviour
    {
        [Tooltip("Xoay sprite theo hướng bay. Tắt với đạn tròn không có đầu.")]
        [SerializeField] bool faceDirection = true;

        [Tooltip("Tự huỷ sau ngần này giây dù chưa tới đích — lưới an toàn " +
                 "phòng khi tốc độ bị đặt bằng 0.")]
        [SerializeField] float maxLifetime = 5f;

        Unit target;

        /// <summary>Con ban ra. Giu de ghi cong vao bang chi so.</summary>
        Unit source;
        Vector3 start;
        Vector3 end;
        float damage;
        float flightTime;      // tổng thời gian bay, tính từ quãng đường / tốc độ
        float arcHeight;
        float elapsed;
        bool launched;

        /// <summary>
        /// Bắn đi. Gọi ngay sau Instantiate.
        /// </summary>
        /// <param name="arcHeightUnits">
        /// Đỉnh vòng cung cao hơn đường thẳng bao nhiêu đơn vị. 0 = bay thẳng.
        /// </param>
        public void Launch(Unit targetUnit, float damageAmount,
                           float speed, float arcHeightUnits, Unit shooter = null)
        {
            target = targetUnit;
            damage = damageAmount;
            source = shooter;
            arcHeight = Mathf.Max(0f, arcHeightUnits);

            start = transform.position;

            // Chốt điểm đến NGAY LÚC BẮN, không bám theo địch từng frame.
            // Lý do: toán vòng cung cần biết trước hai đầu mới tính được đỉnh.
            // Đổi lại, địch đang đi thì đạn rơi vào chỗ cũ.
            //
            // Trước đây biện minh bằng "tầm bắn ngắn ~1.6 đơn vị". Tầm giờ lên
            // tới 4.9 nên lý do đó hết đúng; cái giữ sai số nhỏ bây giờ là
            // THỜI GIAN BAY, không phải quãng đường. Tốc độ đạn đặt theo tầm
            // để mọi loại bay trong 0.2–0.45 s. Với MoveSpeed 1.35 thì địch
            // chỉ kịp nhích tối đa ~0.6 đơn vị, vẫn trong thân nó.
            //
            // Thêm tầm mà quên tăng tốc độ đạn là đạn bắt đầu bắn trượt.
            // HitPoint chu khong phai transform.position: goc linh o mat dat,
            // nham thang vao do thi dan bay la la duoi chan.
            end = targetUnit != null ? targetUnit.HitPoint : start;

            var distance = Vector3.Distance(start, end);
            flightTime = distance / Mathf.Max(0.01f, speed);

            // Khoảng cách 0 (hai con chồng lên nhau) thì nổ ngay ở frame sau,
            // không chia cho 0 và không treo mãi mãi.
            if (flightTime < 0.0001f) flightTime = 0.0001f;

            elapsed = 0f;
            launched = true;
            transform.position = start;
        }

        void Update()
        {
            if (!launched) return;

            elapsed += Time.deltaTime;

            if (elapsed >= maxLifetime)
            {
                Destroy(gameObject);
                return;
            }

            var t = Mathf.Clamp01(elapsed / flightTime);
            var previous = transform.position;
            transform.position = PointAt(t);

            if (faceDirection) FaceAlong(transform.position - previous);

            if (t >= 1f) Arrive();
        }

        /// <summary>
        /// Vị trí tại tỉ lệ t (0 = lúc bắn, 1 = tới nơi).
        ///
        /// Phần vòng cung: 4·t·(1−t) là một parabol bằng 0 ở hai đầu và bằng
        /// 1 đúng giữa đường. Nhân với arcHeight được độ vồng mong muốn.
        /// </summary>
        Vector3 PointAt(float t)
        {
            var p = Vector3.Lerp(start, end, t);
            if (arcHeight > 0f) p.y += arcHeight * 4f * t * (1f - t);
            return p;
        }

        void FaceAlong(Vector3 delta)
        {
            if (delta.sqrMagnitude < 1e-8f) return;
            var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        void Arrive()
        {
            // target có thể đã chết hoặc bị xoá trong lúc đạn đang bay.
            // So với null bằng toán tử của Unity (bắt cả MissingReference).
            if (target != null && target.IsAlive)
                target.TakeDamage(damage, source);

            Destroy(gameObject);
        }
    }
}
