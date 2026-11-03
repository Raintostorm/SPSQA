using Project.Battle;
using Project.Data;
using System.Collections;
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
        public bool IsDelivering { get; private set; }

        void OnEnable()
        {
            if (roll == null) roll = FindFirstObjectByType<RollService>();
            if (roll != null) roll.RewardReady += Place;
        }

        void OnDisable()
        {
            if (roll != null) roll.RewardReady -= Place;
            StopAllCoroutines();
            IsDelivering = false;
        }

        void Place(RollPayout payout)
        {
            if (payout == null || allyBoard == null || spawner == null) return;
            StartCoroutine(Deliver(payout));
        }

        IEnumerator Deliver(RollPayout payout)
        {
            IsDelivering = true;

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
                var occupied = allyBoard.CountAt(row,col);
                var destination = allyBoard.Grid.SlotToWorld(row,col,occupied,occupied+1,def);
                var camera = Camera.main;
                if (camera)
                {
                    var source = camera.ScreenToWorldPoint(new Vector3(Screen.width * .5f, Screen.height * .22f, Mathf.Abs(camera.transform.position.z - destination.z)));
                    var panel = FindFirstObjectByType<Project.UI.RollPanel>();
                    if (panel) source = panel.RewardOrigin(camera, destination.z);
                    Project.Visual.SkillEffects.Recruit(source, destination, .55f,
                        Project.Visual.PrepUnitRing.ColorForRank(def.Rank));
                    float elapsed = 0f;
                    while (elapsed < .55f)
                    {
                        if (!Project.UI.SharedSettings.IsPaused) elapsed += Time.deltaTime;
                        yield return null;
                    }
                }
                if (!allyBoard || !spawner) break;
                var unit = spawner.Spawn(def, UnitTeam.Ally,
                                         destination,
                                         allyBoard.transform);
                if (unit == null) continue;
                BattleContribution.Recruited++;

                allyBoard.Place(unit, row, col);
                unit.Visual?.SetFacing(1f);

                // Não phải TẮT lúc chuẩn bị. Lính sinh ra lúc đang xếp quân
                // mà não bật thì nó lao đi đánh ngay trên lưới.
                var brain = unit.GetComponent<UnitBrain>();
                if (brain != null) brain.enabled = false;

                Debug.Log($"[RollRewardPlacer] +{def.name} vào ô ({row},{col})", this);
                Project.Visual.SkillEffects.RecruitArrival(unit.transform.position, .7f,
                    Project.Visual.PrepUnitRing.ColorForRank(def.Rank), .5f);
            }
            IsDelivering = false;
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
