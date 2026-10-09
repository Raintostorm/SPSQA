using Project.Data;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>
    /// Con số sát thương bay lên rồi tan.
    ///
    /// Tự dựng lúc chạy, KHÔNG cần prefab và KHÔNG cần sửa scene: file .unity
    /// do Unity giữ trong bộ nhớ nên không ghi được từ ngoài (xem Handoff/11
    /// mục E). Một hàm tĩnh dựng lấy GameObject của chính nó là cách duy nhất
    /// thêm được thứ này mà không bắt ai phải kéo thả gì.
    ///
    /// Dùng TextMesh chứ không dùng Canvas: Canvas cho mỗi con số là rất đắt
    /// khi mười mấy con đánh nhau, và TextMesh ăn chung hệ Sorting Layer với
    /// nhân vật nên không phải lo thứ tự vẽ giữa hai hệ — cùng lý do HealthBar
    /// dùng SpriteRenderer.
    ///
    /// KHÔNG gắn vào con lính: lính có localScale riêng 0.45–0.85, gắn vào là
    /// con số to nhỏ theo từng nhân vật. Và lính chết thì số cũng biến mất
    /// giữa chừng, đúng lúc người chơi muốn đọc nhất.
    /// </summary>
    public sealed class DamagePopup : MonoBehaviour
    {
        // Cao bao nhieu don vi the gioi. Man iPhone X cao 11.909 don vi tren
        // 2436px, nen 0.22 ra khoang 45px — doc duoc ma khong che mat lính.
        const float WorldHeight = 0.22f;

        // FontSize lon roi thu nho bang CharacterSize: font nho phong to la
        // vo rang cua. TextMesh cao xap xi fontSize * characterSize / 10.
        const int FontSize = 64;

        const float RiseDistance = 0.55f;
        const float LifeSeconds = 0.75f;

        // Tren moi thu. Linh chay tu -600 den +600 (SortingOrderFor = -y*100),
        // nen 5000 la chac chan tren dau, khong phu thuoc linh dung o dau.
        const int SortOrder = 5000;

        static readonly Color AllyHurt  = new Color(1f, 0.42f, 0.38f);
        static readonly Color EnemyHurt = new Color(1f, 0.93f, 0.45f);

        TextMesh text;
        Vector3 from;
        float born;

        /// <summary>
        /// Hiện số máu ĐÃ MẤT THẬT tại <paramref name="worldPos"/>.
        ///
        /// <paramref name="victim"/> chỉ dùng để chọn màu: phe ta bị đánh thì
        /// đỏ, địch bị đánh thì vàng. Nhìn một cái là biết ai đang ăn đòn mà
        /// không phải đọc chữ.
        /// </summary>
        public static void Show(Vector3 worldPos, float amount, UnitTeam victim)
        {
            if (amount <= 0f) return;

            var go = new GameObject("DamagePopup");
            go.transform.position = worldPos;

            var tm = go.AddComponent<TextMesh>();
            tm.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            tm.fontSize = FontSize;
            tm.characterSize = WorldHeight * 10f / FontSize;
            tm.fontStyle = FontStyle.Bold;
            tm.anchor = TextAnchor.LowerCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = victim == UnitTeam.Ally ? AllyHurt : EnemyHurt;
            tm.text = Mathf.RoundToInt(amount).ToString();

            // TextMesh ve qua MeshRenderer, va material cua font phai lay TU
            // font. Thieu dong nay thi chu ra mot khoi hong tit.
            var mr = go.GetComponent<MeshRenderer>();
            mr.material = tm.font.material;
            mr.sortingOrder = SortOrder;

            var popup = go.AddComponent<DamagePopup>();
            popup.text = tm;
            popup.from = worldPos;
            popup.born = Time.time;
        }

        void Update()
        {
            // Time.time chu khong Time.deltaTime cong don: nut x2 doi
            // Time.timeScale, ma deltaTime da nhan timeScale roi — cong don
            // thi so bay nhanh gap doi luc tang toc. Doc dong ho mot lan thi
            // ca hai che do deu dung 0.75 giay cua the gioi trong game.
            var t = (Time.time - born) / LifeSeconds;
            if (t >= 1f)
            {
                Destroy(gameObject);
                return;
            }

            // Bay nhanh luc dau roi cham dan — giong nem len chu khong giong
            // thang may. 1-(1-t)^2 la duong cong giam toc re nhat.
            var ease = 1f - (1f - t) * (1f - t);
            transform.position = from + Vector3.up * (RiseDistance * ease);

            // Chi mo di o NUA SAU. Mo ngay tu dau thi con so dam nhat lai la
            // luc no vua hien, dung luc can doc.
            var a = t < 0.5f ? 1f : 1f - (t - 0.5f) * 2f;
            var c = text.color;
            text.color = new Color(c.r, c.g, c.b, a);
        }
    }
}
