using Project.Battle;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>
    /// Đổi ảnh nền theo giai đoạn trận đấu.
    ///
    ///   CHUẨN BỊ   ảnh có vẽ sẵn 16 ô mỗi phe — người chơi xếp quân lên đó
    ///   RA TRẬN    ảnh con đường — lính rời ô, dàn hàng đánh nhau
    ///
    /// Vì sao đổi ảnh chứ không bật tắt mấy sprite ô: 16 ô là nét vẽ nằm
    /// TRONG ảnh, không phải object rời, nên không tắt từng cái được. Mà có
    /// tách rời được cũng không nên — ô vẽ liền vào nền thì ăn khớp với cỏ
    /// và đất xung quanh, dán sprite lên trên bao giờ cũng lộ.
    ///
    /// Đây là VIEW: nó chỉ nghe BattleFlow rồi đổi hình. Không quyết định gì.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BattleBackground : MonoBehaviour
    {
        [SerializeField] BattleFlow flow;
        [SerializeField] Sprite prepSprite;
        [SerializeField] Sprite battleSprite;

        [Tooltip("Tô tối nền để lính nổi lên. Đây là màu NHÂN — " +
                 "không sáng lên được, chỉ tối đi.")]
        [SerializeField] Color tint = new Color(0.90f, 0.92f, 0.96f);

        [Tooltip("Chiều cao một con lính, đơn vị thế giới. Chỉ dùng để in " +
                 "ra Console đối chiếu độ nét — không ảnh hưởng gì tới game.")]
        [SerializeField] float unitWorldHeight = 0.589f;

        SpriteRenderer sr;

        /// <summary>
        /// Kích thước ảnh khi cao đúng bằng khung nhìn.
        ///
        /// CHỈ phụ thuộc ortho — KHÔNG phụ thuộc tỉ lệ cửa sổ. Đó là điểm
        /// mấu chốt, không phải chuyện cho gọn.
        ///
        /// Bản trước phóng cho "phủ kín khung nhìn" nên hệ số phụ thuộc
        /// Camera.aspect. Hai chỗ đọc ra hai giá trị khác nhau:
        ///   - script Editor đọc lúc chưa Play, khi đó Camera.aspect có thể
        ///     là tỉ lệ của Scene view chứ không phải Game view
        ///   - để Game view ở "Free Aspect" thì nó còn đổi theo lúc kéo cửa sổ
        /// Kết quả: gốc lưới nướng theo một tỉ lệ, nền co theo tỉ lệ khác,
        /// và lính đứng chệch khỏi ô vẽ.
        ///
        /// Giải pháp: phủ kín một khung nhìn 16:9 CỐ ĐỊNH
        /// (<see cref="LetterboxCamera.DesignWidth"/>), và LetterboxCamera
        /// ép camera chỉ vẽ trong đúng khung đó. Cửa sổ lạ tỉ lệ thì có viền
        /// đen — đó là letterbox, không phải lỗi.
        ///
        /// Bản trước neo theo chiều cao mà KHÔNG khoá khung nhìn, nên cửa sổ
        /// rộng hơn 16:9 là hở hai bên lòi ra màu nền camera, nhìn như hỏng
        /// đồ hoạ.
        /// </summary>
        public static Vector2 FitDesign(Sprite sprite, float ortho)
        {
            if (sprite == null) return Vector2.zero;
            var b = sprite.bounds.size;
            // Phu kin khung nhin: ca BE NGANG co dinh lan chieu cao thuc te.
            // Lay he so lon hon trong hai — thieu cai nao la ho mep cai do.
            var viewH = ortho * 2f;
            var k = Mathf.Max(LetterboxCamera.DesignWidth / b.x, viewH / b.y);
            return new Vector2(k * b.x, k * b.y);
        }

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            if (flow == null) flow = FindFirstObjectByType<BattleFlow>();
        }

        void OnEnable()
        {
            if (flow == null) { Apply(BattleFlow.Phase.Prep); return; }
            flow.PhaseChanged += Apply;
            Apply(flow.Current);
        }

        void OnDisable()
        {
            if (flow != null) flow.PhaseChanged -= Apply;
        }

        void Apply(BattleFlow.Phase phase)
        {
            var want = phase == BattleFlow.Phase.Prep ? prepSprite : battleSprite;
            if (want == null) return;

            sr.sprite = want;
            sr.color = tint;

            // Tính lại cỡ mỗi lần đổi ảnh: hai ảnh khác tỉ lệ khung nên bề
            // ngang khác nhau. Đặt một lần lúc dựng scene là ảnh thứ hai sai cỡ.
            var cam = Camera.main;
            if (cam == null) return;
            var size = FitDesign(want, cam.orthographicSize);
            var b = want.bounds.size;
            transform.localScale = new Vector3(size.x / b.x, size.y / b.y, 1f);
            // Chieu cao con linh do tu prefab, de bao cao doi chieu duoc.
            LetterboxCamera.ReportResolution(cam.orthographicSize, unitWorldHeight, this);
        }
    }
}
