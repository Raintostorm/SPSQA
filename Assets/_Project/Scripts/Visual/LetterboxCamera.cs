using UnityEngine;

namespace Project.Visual
{
    /// <summary>
    /// Giữ chiều CAO khung nhìn cố định, và kẹp tỉ lệ vào khoảng
    /// [<see cref="WidestAspect"/>, <see cref="TallestAspect"/>].
    ///
    /// Vì sao cần: cả bàn cờ — gốc lưới, laneY, kích thước ô — đều tính từ
    /// kích thước ảnh nền đã phóng. Nếu hệ số phóng phụ thuộc tỉ lệ cửa sổ
    /// thì mỗi lần kéo cửa sổ là một bàn cờ khác, và script Editor (tính lúc
    /// chưa Play) với runtime sẽ ra hai kết quả khác nhau.
    ///
    /// Cách chặn: chiều CAO khung nhìn là ortho×2, một hằng số. Mọi toạ độ
    /// lưới tính từ chiều cao, không từ chiều rộng — nên kéo cửa sổ rộng ra
    /// chỉ làm lộ thêm phần trang trí hai bên, bàn cờ không nhúc nhích.
    ///
    /// Vì sao có HAI ngưỡng chứ không một:
    ///
    ///   rộng quá TallestAspect  — hết tranh để vẽ, phải cắt hai bên
    ///   hẹp quá WidestAspect   — khung chơi 16:9 bắt đầu bị xén, phải cắt trên dưới
    ///   ở giữa              — vẽ tràn cả cửa sổ, KHÔNG viền nào cả
    ///
    /// Bản trước khoá cứng đúng 16:9 nên màn 2.59:1 của người chơi luôn có
    /// hai dải trống, và tôi đã phải lấp chúng bằng cách lật gương rìa ảnh —
    /// trông rõ là ảnh nhân đôi. Nền 2.8:1 làm cả nhánh đó thành thừa.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [ExecuteAlways]
    public sealed class LetterboxCamera : MonoBehaviour
    {
        /// <summary>
        /// Bề ngang khung nhìn, đơn vị thế giới. ĐÂY là thứ cố định, không
        /// phải chiều cao.
        ///
        /// Màn ngang thì khoá chiều cao là đúng — màn rộng ra thì thấy thêm
        /// cảnh hai bên. Màn DỌC thì ngược: bề ngang quyết định ô to bao
        /// nhiêu, còn chiều cao thì máy dài máy ngắn mỗi máy một khác.
        ///
        /// Đổi số này là đổi cỡ MỌI THỨ trên sân cùng lúc.
        /// </summary>
        public const float DesignWidth = 5.5f;

        /// <summary>
        /// Máy RỘNG nhất ta chịu được: 16:9 dựng đứng. Rộng hơn thì cắt hai
        /// bên, không thì bàn cờ tràn ra ngoài.
        ///
        /// Màn dọc thì tỉ lệ = rộng ÷ cao, nên máy càng DÀI số càng NHỎ —
        /// "rộng nhất" lại là số LỚN nhất. Đặt tên theo hình dạng chứ không
        /// theo lớn bé, không thì chắc chắn có lúc dùng ngược.
        /// </summary>
        public const float WidestAspect = 9f / 16f;         // 0.5625

        /// <summary>
        /// Máy DÀI nhất ta có tranh để vẽ: ảnh nền 1242×2796. Dài hơn nữa
        /// là hở mép trên dưới. Đổi ảnh nền thì PHẢI đổi số này.
        /// </summary>
        public const float TallestAspect = 1242f / 2796f;   // 0.4442

        [Tooltip("Màu viền, chỉ thấy khi cửa sổ ra ngoài khoảng 1.78..2.80.")]
        // 0.07 do ra man la (18,28,18) — mat doc thang ra "den". Da do tren
        // anh chup that va dung dung con so do. Phai len quang 0.3.
        //
        // Gia tri nay chi an voi component MOI. Scene da luu thi Unity doc
        // so da luu, nen BattleSceneSetup.BarColor moi la cho quyet dinh.
        [SerializeField] Color barColor = new Color(0.30f, 0.40f, 0.25f);

        [Tooltip("Camera phụ tô màu viền. Để trống thì tự tạo.")]
        [SerializeField] Camera fill;

        Camera cam;
        int lastW, lastH;

        /// <summary>
        /// Camera phụ phủ TOÀN cửa sổ, nằm dưới, chỉ để tô màu.
        ///
        /// Vùng ngoài cam.rect không thuộc camera nào cả, nên Unity để
        /// nguyên bộ đệm — ra màu đen. Không có cách nào "đổi màu dải đen"
        /// trực tiếp; phải có một camera thật sự vẽ vào đó.
        ///
        /// clearFlags của camera chính đổi sang Depth: nếu nó vẫn xoá nền
        /// thì nó xoá đè lên màu vừa tô.
        /// </summary>
        void EnsureFill()
        {
            if (fill == null)
            {
                var t = transform.Find("LetterboxFill");
                fill = t != null ? t.GetComponent<Camera>() : null;
            }
            if (fill == null)
            {
                var go = new GameObject("LetterboxFill");
                go.transform.SetParent(transform, false);
                fill = go.AddComponent<Camera>();
            }
            fill.orthographic = true;
            fill.depth = cam.depth - 10f;        // ve TRUOC camera chinh
            fill.rect = new Rect(0f, 0f, 1f, 1f);
            fill.clearFlags = CameraClearFlags.SolidColor;
            fill.backgroundColor = barColor;
            fill.cullingMask = 0;                // khong ve vat the nao
            cam.clearFlags = CameraClearFlags.Depth;
        }

        void OnEnable()
        {
            reported = false;
            cam = GetComponent<Camera>();
            EnsureFill();
            lastW = lastH = 0;
            Apply();
        }

        void OnDisable()
        {
            // Trả lại toàn khung. Thiếu dòng này thì tắt component đi, camera
            // vẫn kẹt ở khung cũ và không chỗ nào giải thích vì sao.
            if (cam != null)
            {
                cam.rect = new Rect(0f, 0f, 1f, 1f);
                cam.clearFlags = CameraClearFlags.SolidColor;
            }
        }

        // Cua so doi kich thuoc thi khong co callback nao bao, nen phai do
        // moi khung hinh. Re: hai phep so sanh int.
        void Update()
        {
            if (Screen.width == lastW && Screen.height == lastH) return;
            Apply();
        }

        void Apply()
        {
            if (cam == null) return;
            lastW = Screen.width;
            lastH = Screen.height;
            if (lastW <= 0 || lastH <= 0) return;

            var window = (float)lastW / lastH;

            // Chieu cao khung nhin suy ra tu BE NGANG co dinh. Lam vay thi
            // o luoi to bang nhau tren moi may, con may dai thi chi thay
            // them canh tren duoi.
            // Kep vao khoang CO TRANH roi suy chieu cao khung nhin ra.
            var used = Mathf.Clamp(window, TallestAspect, WidestAspect);
            cam.orthographicSize = DesignWidth / used * 0.5f;

            if (window > WidestAspect)
            {
                // Rong hon khung choi: cat bot hai ben.
                var w = WidestAspect / window;
                cam.rect = new Rect((1f - w) * 0.5f, 0f, w, 1f);
            }
            else if (window < TallestAspect)
            {
                // Dai hon ca tam tranh: cat bot tren duoi.
                var h = window / TallestAspect;
                cam.rect = new Rect(0f, (1f - h) * 0.5f, 1f, h);
            }
            else
            {
                // Khoang giua — pho bien nhat. Ve tran ca cua so, khong vien.
                cam.rect = new Rect(0f, 0f, 1f, 1f);
            }
        }

        /// <summary>
        /// Báo kích thước THẬT của vùng đang vẽ, một lần mỗi phiên Play.
        ///
        /// Báo chứ không chỉ cảnh báo: khi có người nói "đồ hoạ xấu" thì
        /// đoán là vô ích, phải có con số. Art nhân vật cao 165px; vẽ ra
        /// 98px (ở 1920×1080) là đẹp, vẽ ra 37px thì không có cách nào cứu.
        ///
        /// Đây là vùng 16:9 sau khi cắt viền, không phải cả cửa sổ.
        /// </summary>
        public static void ReportResolution(float ortho, float unitHeight, Object ctx)
        {
            if (reported) return;
            reported = true;

            var h = Screen.height;
            var w = Screen.width;
            var window = (float)w / Mathf.Max(1, h);
            if (window > WidestAspect) w = Mathf.RoundToInt(h * WidestAspect);
            else if (window < TallestAspect) h = Mathf.RoundToInt(w / TallestAspect);

            // Khong viet cung chieu cao khung nhin: no la ortho*2, doi camera
            // la con so nay sai ma khong ai biet.
            var unitPx = unitHeight / (ortho * 2f) * h;
            // In ca rect va ortho: khi hinh "nhin sai" ma khong meo, thu
            // phan biet duoc ba kha nang — camera khong letterbox, ortho bi
            // doi, hay Screen tra ve so khac voi vung thuc su thay — chinh
            // la ba con so nay.
            var cam = Camera.main;
            var r = cam != null ? cam.rect : new Rect(0, 0, 1, 1);
            var msg = $"[LetterboxCamera] Screen {Screen.width}×{Screen.height} " +
                      $"(tỉ lệ {(float)Screen.width / Screen.height:0.000})  ·  " +
                      $"ortho {ortho:0.00}  ·  " +
                      $"cam.rect ({r.x:0.000}, {r.y:0.000}, {r.width:0.000}, {r.height:0.000})\n" +
                      $"   vùng 16:9 lẽ ra {w}×{h}, một con lính cao {unitPx:0} px.";

            if (h >= 1000)
                Debug.Log(msg + " Đủ nét.", ctx);
            else
                Debug.LogWarning(
                    msg + $" Cần ~1080 để nét. Phóng to cửa sổ Game — đưa " +
                    $"chuột lên tab Game rồi bấm Shift+Space — hoặc đổi " +
                    $"Free Aspect sang 1920×1080 để Unity dựng ở full rồi " +
                    $"thu xuống, thay vì dựng thẳng ở cỡ nhỏ.", ctx);
        }

        static bool reported;

    }
}
