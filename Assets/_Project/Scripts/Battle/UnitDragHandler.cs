using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Battle
{
    /// <summary>
    /// Kéo lính giữa các ô, lúc chuẩn bị.
    ///
    /// Kéo CẢ Ô chứ không kéo từng con. Lý do: một ô chỉ chứa lính cùng loại,
    /// nên nhóm trong ô là một đơn vị người chơi nghĩ tới — tách lẻ một con
    /// ra khỏi nhóm không có nghĩa gì trong luật của game này.
    ///
    /// Gắn lên GameObject có BattleBoard. Chỉ gắn cho lưới của PHE MÌNH —
    /// không gắn cho Grid_Enemy thì người chơi không xê dịch quân địch được,
    /// khỏi cần thêm luật kiểm tra.
    /// </summary>
    [RequireComponent(typeof(BattleBoard))]
    public sealed class UnitDragHandler : MonoBehaviour
    {
        [Tooltip("Lính đang kéo mờ đi bao nhiêu, để thấy ô bên dưới.")]
        [Range(0.2f, 1f)]
        [SerializeField] float dragAlpha = 0.6f;

        BattleBoard board;
        Camera cam;

        readonly List<Unit> carried = new List<Unit>();
        int fromRow, fromCol;
        Vector3 grabOffset;

        void Awake()
        {
            board = GetComponent<BattleBoard>();
            cam = Camera.main;
        }

        void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null || cam == null) return;

            if (mouse.leftButton.wasPressedThisFrame) TryGrab(PointerWorld(mouse));
            else if (carried.Count > 0)
            {
                if (mouse.leftButton.isPressed) Carry(PointerWorld(mouse));
                else Drop(PointerWorld(mouse));
            }
        }

        Vector3 PointerWorld(Mouse mouse)
        {
            var p = cam.ScreenToWorldPoint(mouse.position.ReadValue());
            p.z = 0f;
            return p;
        }

        // ── ba bước ───────────────────────────────────────────────────

        void TryGrab(Vector3 world)
        {
            if (!board.Grid.WorldToCell(world, out var r, out var c)) return;

            var list = board.UnitsAt(r, c);
            if (list.Count == 0) return;

            carried.Clear();
            carried.AddRange(list);
            fromRow = r; fromCol = c;

            // Nhớ lệch giữa con trỏ và tâm ô, để nhóm không nhảy giật vào
            // đúng con trỏ ngay khi bấm.
            grabOffset = board.Grid.CellToWorld(r, c) - world;

            SetAlpha(dragAlpha);
        }

        void Carry(Vector3 world)
        {
            var centre = world + grabOffset;
            for (var i = 0; i < carried.Count; i++)
            {
                if (carried[i] == null) continue;
                // Giữ nguyên đội hình trong lúc bay: lấy chỗ đứng ở ô (0,0)
                // rồi dời cả cụm theo con trỏ.
                var local = board.Grid.SlotToWorld(
                                0, 0, i, carried.Count, carried[i].Definition)
                          - board.Grid.CellToWorld(0, 0);
                carried[i].transform.position = centre + local;
            }
        }

        void Drop(Vector3 world)
        {
            var def = carried[0] != null ? carried[0].Definition : null;
            var ok = board.Grid.WorldToCell(world + grabOffset, out var r, out var c)
                  && (r != fromRow || c != fromCol)
                  && CanAcceptAll(r, c, def);

            var row = ok ? r : fromRow;
            var col = ok ? c : fromCol;

            // Rút hết khỏi ô cũ TRƯỚC khi đặt — nếu không thì con thứ hai
            // thấy ô đích đã có con thứ nhất và luật "cùng loại" tính nhầm.
            foreach (var u in carried) board.Remove(u);
            foreach (var u in carried) board.Place(u, row, col);

            SetAlpha(1f);
            carried.Clear();
        }

        /// <summary>Ô đích nhận được HẾT cả nhóm, chứ không phải chỉ con đầu.</summary>
        bool CanAcceptAll(int row, int col, Data.UnitDefinition def)
        {
            if (!board.Grid.IsInside(row, col) || def == null) return false;

            var there = board.UnitsAt(row, col);
            if (there.Count > 0 && there[0].Definition != def) return false;
            return there.Count + carried.Count <= board.Grid.MaxPerCell;
        }

        void SetAlpha(float a)
        {
            foreach (var u in carried)
            {
                if (u == null) continue;
                foreach (var sr in u.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    var col = sr.color;
                    col.a = a;
                    sr.color = col;
                }
            }
        }

        void OnDisable()
        {
            // Tắt giữa lúc đang kéo thì phải trả lính về chỗ cũ, không thì
            // chúng đứng lơ lửng ở nơi con trỏ dừng lại.
            if (carried.Count == 0) return;
            SetAlpha(1f);
            foreach (var u in carried) board.Place(u, fromRow, fromCol);
            carried.Clear();
        }
    }
}
