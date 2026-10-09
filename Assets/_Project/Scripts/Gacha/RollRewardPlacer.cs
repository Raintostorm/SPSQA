using Project.Battle;
using Project.Data;
using UnityEngine;

namespace Project.Gacha
{
    /// <summary>
    /// Biến ô Ally thắng cụm thành con lính đứng trên lưới.
    ///
    /// Tách khỏi RollService là cố ý: RollService chỉ biết "được một lính
    /// thời Medieval biến thể 3", nó không biết lưới là gì, ô nào trống, hay
    /// prefab nào. Gộp vào thì mỗi lần đổi luật đặt quân lại phải sửa máy
    /// quay gacha.
    ///
    /// Tra cứu theo (Rank, Race) chứ KHÔNG parse tên asset: tên file là
    /// thứ người ta đổi mà không nghĩ, còn hai field kia là dữ liệu thật.
    /// </summary>
    public sealed class RollRewardPlacer : MonoBehaviour
    {
        [SerializeField] RollService roll;
        [SerializeField] BattleBoard allyBoard;
        [SerializeField] UnitSpawner spawner;

        [Tooltip("Cả 16 UnitDefinition. BattleSceneSetup nạp tự động.")]
        [SerializeField] UnitDefinition[] roster;

        [Tooltip("Dùng thay roster nếu có. Một nguồn cho cả gacha lẫn tiến hoá.")]
        [SerializeField] UnitPool pool;

        void OnEnable()
        {
            if (roll == null) roll = FindFirstObjectByType<RollService>();
            if (roll != null) roll.Rolled += Place;
        }

        void OnDisable()
        {
            if (roll != null) roll.Rolled -= Place;
        }

        void Place(RollPayout payout)
        {
            if (payout == null || allyBoard == null || spawner == null) return;

            foreach (var grant in payout.AllyGrants)
            {
                var def = Find(grant.Rank, grant.Race);
                if (def == null)
                {
                    Debug.LogWarning(
                        $"[RollRewardPlacer] Không có UnitDefinition nào ứng với " +
                        $"{grant.Race} bậc {grant.Rank}.", this);
                    continue;
                }

                if (!allyBoard.FindSpotFor(def, out var row, out var col))
                {
                    // Lưới đầy. Đúng ra phải hoàn tiền hoặc đổi thành coin —
                    // nhưng đó là luật chơi, chưa chốt. Tạm thời kêu to để
                    // không ai tưởng là mất lính không rõ lý do.
                    Debug.LogWarning(
                        $"[RollRewardPlacer] Lưới đầy, mất '{def.name}' roll được.", this);
                    continue;
                }

                // Sinh ở tâm ô; Place() xếp lại đúng đội hình sau khi biết
                // trong ô có mấy con. Giống hệt BattleSetup.
                var unit = spawner.Spawn(def, UnitTeam.Ally,
                                         allyBoard.Grid.CellToWorld(row, col),
                                         allyBoard.transform);
                if (unit == null) continue;

                allyBoard.Place(unit, row, col);
                unit.Visual?.SetFacing(1f);

                // Não phải TẮT lúc chuẩn bị. Lính sinh ra lúc đang xếp quân
                // mà não bật thì nó lao đi đánh ngay trên lưới.
                var brain = unit.GetComponent<UnitBrain>();
                if (brain != null) brain.enabled = false;

                Debug.Log($"[RollRewardPlacer] +{def.name} vào ô ({row},{col})", this);
            }
        }

        UnitDefinition Find(Rank rank, Race race)
        {
            if (pool != null) return pool.Find(rank, race);
            if (roster == null) return null;
            foreach (var d in roster)
                if (d != null && d.Rank == rank && d.Race == race) return d;
            return null;
        }
    }
}
