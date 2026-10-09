using Project.Battle;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools
{
    // KHONG CON DUOC GOI TU BattleSceneSetup.
    //
    // 16 o bay gio ve san trong bg_prep.png, dung them sprite la hai lop
    // duong ke lech nhau. Giu file lai vi no van dung duoc cho mot anh nen
    // KHONG co o ve san — luc do goi tay GridBuilder.Build(board.Grid).
    //
    /// <summary>
    /// Dựng sprite ô lưới trong scene, lấy đúng toạ độ từ BattleGrid.
    ///
    /// Vì sao cần lệnh này thay vì đặt tay 16 ô: toạ độ ô do BattleGrid tính.
    /// Đặt tay thì sớm muộn số trong scene lệch khỏi số trong code, và lính
    /// sẽ snap vào chỗ không trùng với ô bạn nhìn thấy.
    ///
    /// Ô sinh ra là GameObject THẬT trong scene — sửa được, xoá được, thấy
    /// được khi không Play. Bấm lại thì nó dựng lại từ đầu.
    /// </summary>
    public static class GridBuilder
    {
        const string CellsName = "Cells";
        public const string CellSprite = "Assets/_Project/Art/UI/grid_cell.png";

        [MenuItem("SpinSquad/Dựng ô lưới cho BattleGrid đang chọn %#g")]
        static void BuildSelected()
        {
            var grid = Selection.activeGameObject != null
                     ? Selection.activeGameObject.GetComponent<BattleGrid>()
                     : null;

            if (grid == null)
            {
                EditorUtility.DisplayDialog("Chưa chọn lưới",
                    "Chọn GameObject có component BattleGrid rồi bấm lại.", "OK");
                return;
            }
            Build(grid);
        }

        /// <summary>Dựng lại toàn bộ ô cho một lưới. Gọi lại được nhiều lần.</summary>
        public static void Build(BattleGrid grid)
        {
            if (grid == null) return;

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(CellSprite);
            if (sprite == null)
            {
                Debug.LogError($"[GridBuilder] Không tìm thấy {CellSprite}");
                return;
            }

            // Xoá lứa cũ trước. Không xoá thì mỗi lần bấm lại chồng thêm 16 ô.
            var old = grid.transform.Find(CellsName);
            if (old != null) Undo.DestroyObjectImmediate(old.gameObject);

            var holder = new GameObject(CellsName);
            Undo.RegisterCreatedObjectUndo(holder, "Dựng ô lưới");
            holder.transform.SetParent(grid.transform, false);

            // Sprite 512px @ PPU 512 = 1 đơn vị. Co về đúng kích thước ô.
            // Hai trục riêng vì ô không còn vuông.
            var scale = new Vector2(grid.CellWidth, grid.CellHeight);

            for (var r = 0; r < grid.Rows; r++)
            for (var c = 0; c < grid.Cols; c++)
            {
                var go = new GameObject($"Cell_{r}_{c}");
                go.transform.SetParent(holder.transform, false);
                go.transform.position = grid.CellToWorld(r, c);
                go.transform.localScale = new Vector3(scale.x, scale.y, 1f);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = Project.Battle.BattleGrid.TileOrder;
            }

            EditorUtility.SetDirty(grid.gameObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                grid.gameObject.scene);

            Debug.Log($"[GridBuilder] Dựng {grid.Rows}×{grid.Cols} ô cho " +
                      $"'{grid.name}', ô {scale.x:0.00}×{scale.y:0.00} đơn vị.", grid);
        }
    }
}
