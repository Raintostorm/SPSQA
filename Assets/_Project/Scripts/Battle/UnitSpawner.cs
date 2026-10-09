using Project.Data;
using Project.Visual;
using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Sinh lính từ UnitDefinition.
    ///
    /// Cả roster dùng CHUNG một prefab — khác nhau chỉ ở ba sprite và tên
    /// state tấn công. Nên thêm một lính mới = thêm một asset dữ liệu, không
    /// phải thêm prefab. Xem Handoff/09.
    ///
    /// Ba bước lắp, đúng thứ tự:
    ///   1. Instantiate prefab
    ///   2. Unit.Initialize   — máu, phe
    ///   3. mặc đồ + chọn clip tấn công
    /// Bước 3 phải sau bước 1 vì nó cần component trên bản vừa sinh.
    /// </summary>
    public sealed class UnitSpawner : MonoBehaviour
    {
        [Tooltip("Prefab dùng chung cho cả roster. Kéo Unit.prefab vào đây.")]
        [SerializeField] GameObject unitPrefab;

        public GameObject UnitPrefab => unitPrefab;

        /// <summary>Sinh một lính tại vị trí cho trước. Trả null nếu thiếu dữ liệu.</summary>
        public Unit Spawn(UnitDefinition def, UnitTeam team, Vector3 position,
                          Transform parent = null)
        {
            if (def == null)
            {
                Debug.LogError("[UnitSpawner] UnitDefinition = null.", this);
                return null;
            }

            // def.BattlePrefab là ngoại lệ hiếm — bộ xương riêng cho con đặc biệt.
            var prefab = def.BattlePrefab != null ? def.BattlePrefab : unitPrefab;
            if (prefab == null)
            {
                Debug.LogError(
                    $"[UnitSpawner] Chưa gán Unit Prefab, và '{def.name}' cũng " +
                    $"không có BattlePrefab riêng.", this);
                return null;
            }

            var go = Instantiate(prefab, position, Quaternion.identity, parent);
            go.name = string.IsNullOrEmpty(def.DisplayName) ? def.name : def.DisplayName;

            var unit = go.GetComponent<Unit>();
            if (unit == null)
            {
                Debug.LogError(
                    $"[UnitSpawner] Prefab '{prefab.name}' không có component Unit.", this);
                Destroy(go);
                return null;
            }

            unit.Initialize(def, team);
            Dress(go, def);
            return unit;
        }

        /// <summary>Lắp bộ khung hình của con lính vào prefab chung.</summary>
        static void Dress(GameObject go, UnitDefinition def)
        {
            var visual = go.GetComponent<Project.Visual.FrameUnitVisual>();
            if (visual != null)
            {
                if (def.Animations == null)
                    Debug.LogWarning(
                        $"[UnitSpawner] '{def.name}' chưa gán UnitAnimationSet — " +
                        $"lính sẽ vô hình. Chạy SpinSquad ▸ Nhập 20 nhân vật.", go);
                visual.Bind(def.Animations);
            }

            // Nhan vao scale san co cua prefab chu KHONG gan de: prefab co the
            // dang o mot ti le khac 1, gan thang la moi con doi co.
            if (def.Scale > 0f && !Mathf.Approximately(def.Scale, 1f))
                go.transform.localScale *= def.Scale;
        }
    }
}
