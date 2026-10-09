using Project.Battle;
using Project.Data;
using Project.Gacha;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.UI
{
    /// <summary>
    /// Bấm vào một ô có lính → mở bảng thông tin.
    ///
    /// Phân biệt BẤM với KÉO bằng quãng đường con trỏ đi được giữa lúc nhấn
    /// và lúc nhả. UnitDragHandler cũng đang nghe cùng một nút chuột; không
    /// phân biệt thì mỗi lần kéo thả xong lại bật bảng lên.
    ///
    /// Chỉ sống ở màn chuẩn bị. Đang đánh mà mở bảng ra xem chỉ số thì vô
    /// nghĩa — và nút Bán, Tiến hoá lúc đó càng không được bấm.
    /// </summary>
    public sealed class CellInspector : MonoBehaviour
    {
        [SerializeField] BattleFlow flow;
        [SerializeField] BattleBoard allyBoard;
        [SerializeField] BattleBoard enemyBoard;
        [SerializeField] CellPanel panel;

        [Tooltip("Con trỏ xê dịch quá ngần này pixel thì tính là KÉO, không phải bấm.")]
        [SerializeField] float clickSlop = 8f;

        Camera cam;
        Vector2 pressAt;
        bool pressed;

        void Awake() => cam = Camera.main;

        void Update()
        {
            if (flow != null && flow.Current != BattleFlow.Phase.Prep) return;

            var mouse = Mouse.current;
            if (mouse == null || cam == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                pressAt = mouse.position.ReadValue();
                pressed = true;
                return;
            }
            if (!mouse.leftButton.wasReleasedThisFrame || !pressed) return;
            pressed = false;

            if (Vector2.Distance(pressAt, mouse.position.ReadValue()) > clickSlop) return;
            Inspect(ToWorld(mouse.position.ReadValue()));
        }

        Vector3 ToWorld(Vector2 screen)
        {
            var p = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
            p.z = 0f;
            return p;
        }

        void Inspect(Vector3 world)
        {
            if (Hit(allyBoard, world) || Hit(enemyBoard, world)) return;
            panel?.Close();          // bam ra cho trong thi dong bang
        }

        bool Hit(BattleBoard board, Vector3 world)
        {
            if (board == null) return false;
            if (!board.Grid.WorldToCell(world, out var row, out var col)) return false;
            if (board.CountAt(row, col) == 0) return false;
            panel?.Open(board, row, col);
            return true;
        }
    }
}
