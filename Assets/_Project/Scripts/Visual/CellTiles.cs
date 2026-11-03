using Project.Battle;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>
    /// 16 ô đất của một bàn, mỗi ô một SpriteRenderer.
    ///
    /// Vì sao là sprite rời chứ không vẽ sẵn vào ảnh nền: ô phải ĐỔI được
    /// khi người chơi bấm chọn. Ảnh tĩnh không làm được việc đó — muốn làm
    /// thì phải có 16 ảnh nền khác nhau.
    ///
    /// Toạ độ lấy từ BattleGrid chứ không gõ tay. Đó là lý do ô luôn khớp
    /// với chỗ lính đứng, kể cả sau này đổi cỡ lưới.
    /// </summary>
    [RequireComponent(typeof(BattleBoard))]
    public sealed class CellTiles : MonoBehaviour
    {
        const string Holder = "Cells";

        [SerializeField] Sprite normal;
        [SerializeField] Sprite selected;

        [Tooltip("Ô chiếm bao nhiêu phần một bước lưới. Phần còn lại là khe.")]
        [SerializeField, Range(0.5f, 1f)] float fill = 0.94f;

        [Tooltip("Dưới mọi bộ phận của lính, trên ảnh nền. " +
                 "BattleSceneSetup GHI ĐÈ số này — giá trị serialize trong " +
                 "scene thắng mặc định ở đây, nên sửa riêng chỗ này là vô ích.")]
        [SerializeField] int sortingOrder = Battle.BattleGrid.TileOrder;

        [Tooltip("Để trống thì tự tìm. Ô chỉ hiện ở giai đoạn chuẩn bị.")]
        [SerializeField] BattleFlow flow;

        SpriteRenderer[,] tiles;
        int selRow = -1, selCol = -1;
        Transform holder;

        BattleBoard board;
        bool[,] dragTargets;
        bool dragging, inPrep;
        float selectionUntil;
        BattleBoard Board => board != null ? board : (board = GetComponent<BattleBoard>());

        void Awake()
        {
            Collect();
            HideImmediately();
            if (flow == null) flow = FindFirstObjectByType<BattleFlow>();
        }

        void OnEnable()
        {
            // Ô đất là bàn XẾP QUÂN. Ra trận thì nền đổi sang bãi cỏ, mà ô
            // vẫn nằm đó thì lính đánh nhau trên một cái bàn cờ lơ lửng.
            if (flow == null) { SetShown(true); return; }
            flow.PhaseChanged += OnPhase;
            OnPhase(flow.Current);
        }

        void OnDisable()
        {
            if (flow != null) flow.PhaseChanged -= OnPhase;
        }

        void OnPhase(BattleFlow.Phase p)
        {
            var prep = p == BattleFlow.Phase.Prep;
            inPrep = prep;
            dragging = false;
            if (!prep) Deselect();      // khong de o sang mac ket qua tran danh
            SetShown(prep);
            HideImmediately();
        }

        void HideImmediately()
        {
            if (tiles == null) return;
            foreach (var sr in tiles) if (sr != null) sr.color = new Color(.45f, .75f, .72f, 0);
        }

        void Update()
        {
            if (tiles == null || !inPrep) return;
            if (!dragging && selectionUntil > 0 && Time.unscaledTime >= selectionUntil) Deselect();
            for (int r = 0; r < tiles.GetLength(0); r++)
            for (int c = 0; c < tiles.GetLength(1); c++)
            {
                var sr = tiles[r,c];
                if (sr == null) continue;
                bool active = r == selRow && c == selCol;
                float alpha = active ? .48f : dragging && dragTargets != null && dragTargets[r,c] ? .16f : 0;
                var color = active ? new Color(.55f, 1f, 1f) : new Color(.45f, .75f, .72f);
                color.a = Mathf.MoveTowards(sr.color.a, alpha, Time.unscaledDeltaTime * 3);
                sr.color = color;
            }
        }

        public void BeginDrag(Project.Data.UnitDefinition definition, int count, int fromRow, int fromCol)
        {
            dragging = true;
            selectionUntil = 0;
            var g = Board.Grid;
            dragTargets = new bool[g.Rows,g.Cols];
            for (int r = 0; r < g.Rows; r++)
            for (int c = 0; c < g.Cols; c++)
            {
                var units = Board.UnitsAt(r,c);
                dragTargets[r,c] = (r == fromRow && c == fromCol) ||
                    ((units.Count == 0 || units[0].Definition == definition) && units.Count + count <= g.MaxPerCell);
            }
        }

        public void HoverDrag(Vector3 world)
        {
            if (!dragging) return;
            if (Board.Grid.WorldToCell(world, out int r, out int c) && dragTargets[r,c]) Select(r,c);
            else Deselect();
        }

        public void EndDrag(int row, int col)
        {
            dragging = false;
            Select(row,col);
            selectionUntil = Time.unscaledTime + .65f;
        }

        public void CancelDrag()
        {
            dragging = false;
            Deselect();
        }

        void SetShown(bool on)
        {
            if (holder == null) holder = transform.Find(Holder);
            if (holder != null) holder.gameObject.SetActive(on);
        }

        /// <summary>
        /// Nhặt lại các ô đã dựng sẵn trong scene.
        ///
        /// KHÔNG dựng mới lúc chạy: script Editor dựng rồi, dựng lần nữa là
        /// hai lớp ô chồng nhau. Mà cũng không thể chỉ dựa vào thứ tự con —
        /// Unity không hứa hẹn gì về thứ tự đó — nên tra theo toạ độ.
        /// </summary>
        void Collect()
        {
            var g = Board.Grid;
            tiles = new SpriteRenderer[g.Rows, g.Cols];

            holder = transform.Find(Holder);
            if (holder == null) return;

            foreach (Transform t in holder)
            {
                var sr = t.GetComponent<SpriteRenderer>();
                if (sr == null) continue;
                if (!g.WorldToCell(t.position, out var r, out var c)) continue;
                tiles[r, c] = sr;
            }
        }

        /// <summary>Làm sáng một ô. Ô đang sáng trước đó tự tắt.</summary>
        public void Select(int row, int col)
        {
            selectionUntil = 0;
            Deselect();
            if (tiles == null || !Board.Grid.IsInside(row, col)) return;
            var sr = tiles[row, col];
            if (sr == null) return;
            sr.sprite = selected;
            selRow = row; selCol = col;
        }

        public void Deselect()
        {
            selectionUntil = 0;
            if (tiles == null || selRow < 0) return;
            var sr = tiles[selRow, selCol];
            if (sr != null) sr.sprite = normal;
            selRow = selCol = -1;
        }

        /// <summary>
        /// Dựng lại 16 ô. Xoá hết rồi dựng lại từ đầu, nên gọi lúc đang chạy
        /// là mất trạng thái đang chọn.
        ///
        /// KHÔNG rào trong #if UNITY_EDITOR dù chỉ Editor gọi: rào thì
        /// tools/csharp_check.sh không biên dịch nó, và một hàm không được
        /// biên dịch là một hàm không ai kiểm. Đã vấp — lỗi chỉ lộ ra lúc mở
        /// Unity. Cái giá là bản build thừa một hàm không ai gọi.
        /// </summary>
        public void Rebuild(Sprite tileNormal, Sprite tileSelected)
        {
            normal = tileNormal;
            selected = tileSelected;

            var old = transform.Find(Holder);
            if (old != null) DestroyImmediate(old.gameObject);
            holder = null;

            holder = new GameObject(Holder).transform;
            holder.SetParent(transform, false);

            var g = Board.Grid;
            // Một đơn vị thế giới = bao nhiêu pixel sprite. Dùng để quy ô về
            // đúng bề rộng ô lưới bất kể sprite xuất ra ở cỡ nào.
            var pw = normal.rect.width / normal.pixelsPerUnit;
            var ph = normal.rect.height / normal.pixelsPerUnit;

            for (var r = 0; r < g.Rows; r++)
            for (var c = 0; c < g.Cols; c++)
            {
                var go = new GameObject($"Cell_{r}_{c}");
                go.transform.SetParent(holder, false);
                go.transform.position = g.CellToWorld(r, c);
                go.transform.localScale = new Vector3(
                    g.CellWidth  * fill / pw,
                    g.CellHeight * fill / ph, 1f);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = normal;
                sr.sortingOrder = sortingOrder;
            }

            Collect();
        }
    }
}
