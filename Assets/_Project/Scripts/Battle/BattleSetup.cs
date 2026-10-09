using System;
using System.Collections.Generic;
using Project.Data;
using UnityEngine;

namespace Project.Battle
{
    /// <summary>
    /// Sinh lính hai phe vào đúng lưới của phe đó.
    ///
    /// CHỈ sinh. Chuyện khi nào ra trận là của BattleFlow.
    ///
    /// Sinh trong Awake chứ không phải Start: BattleFlow đọc UnitRegistry ở
    /// Start của nó, mà Unity không bảo đảm thứ tự Start giữa hai object.
    /// Awake thì luôn chạy trước mọi Start.
    /// </summary>
    public sealed class BattleSetup : MonoBehaviour
    {
        [Serializable]
        public struct Entry
        {
            public UnitDefinition definition;
            [Min(1)] public int count;
        }

        [SerializeField] UnitSpawner spawner;
        [SerializeField] BattleBoard allyBoard;
        [SerializeField] BattleBoard enemyBoard;

        [Tooltip("Mỗi wave trôi qua thì địch đông thêm bấy nhiêu con. " +
                 "0 = wave nào cũng như nhau.")]
        [SerializeField] int extraEnemiesPerWave = 1;

        [Header("Khai lính mỗi phe. Cùng definition sẽ tự gộp vào một ô, " +
                "tối đa 3 con — luật ở BattleBoard.")]
        [SerializeField] Entry[] allies;
        [SerializeField] Entry[] enemies;

        [Tooltip("Để trống cũng chạy. Có thì mỗi wave mới sẽ sinh lại quân địch.")]
        [SerializeField] BattleFlow flow;

        void OnEnable()
        {
            if (flow == null) flow = FindFirstObjectByType<BattleFlow>();
            if (flow != null)
            {
                flow.PhaseChanged += OnPhase;
                flow.RunRestarted += SpawnBothSides;
            }
        }

        void OnDisable()
        {
            if (flow != null)
            {
                flow.PhaseChanged -= OnPhase;
                flow.RunRestarted -= SpawnBothSides;
            }
        }

        /// <summary>
        /// Sinh lại quân địch cho mỗi wave mới.
        ///
        /// Thiếu cái này thì wave 2 trở đi KHÔNG CÓ ĐỊCH: quân địch wave 1 đã
        /// chết hết và BattleSetup chỉ sinh một lần lúc Awake. Trận sau thắng
        /// tức thì, rồi lại về sân, rồi lại thắng tức thì — vòng lặp vô tận
        /// mà không có gì báo là sai.
        ///
        /// Wave sau đông hơn wave trước <see cref="extraEnemiesPerWave"/> con.
        /// </summary>
        void OnPhase(BattleFlow.Phase phase)
        {
            if (phase != BattleFlow.Phase.Prep) return;
            if (flow.Wave <= 1) return;          // wave 1 da sinh o Awake
            if (enemyBoard == null || spawner == null) return;
            SpawnEnemies(flow.Wave);
        }

        /// <summary>
        /// Quân địch của một wave: danh sách gốc, cộng thêm vài con nữa cho
        /// mỗi wave đã qua.
        ///
        /// Con thêm lấy LUÂN PHIÊN trong chính danh sách gốc, không nhân đôi
        /// con đầu — nhân đôi thì wave sau toàn một loại, mà cả ba con địch
        /// đánh khác nhau nên mất hết sự khác biệt.
        /// </summary>
        void SpawnEnemies(int wave)
        {
            SpawnSide(enemies, enemyBoard, UnitTeam.Enemy);

            var extra = Mathf.Max(0, extraEnemiesPerWave) * Mathf.Max(0, wave - 1);
            if (extra == 0 || enemies == null || enemies.Length == 0) return;

            var pick = new System.Collections.Generic.List<Entry>();
            for (var i = 0; i < extra; i++)
            {
                var src = enemies[i % enemies.Length];
                if (src.definition == null) continue;
                pick.Add(new Entry { definition = src.definition, count = 1 });
            }
            SpawnSide(pick.ToArray(), enemyBoard, UnitTeam.Enemy);
        }

        void Awake()
        {
            if (spawner == null || allyBoard == null || enemyBoard == null)
            {
                Debug.LogError("[BattleSetup] Thiếu Spawner / AllyBoard / " +
                               "EnemyBoard. Không sinh được lính.", this);
                enabled = false;
                return;
            }

            SpawnBothSides();
        }

        void SpawnBothSides()
        {
            SpawnSide(allies,  allyBoard,  UnitTeam.Ally);
            SpawnSide(enemies, enemyBoard, UnitTeam.Enemy);
        }

        void SpawnSide(Entry[] entries, BattleBoard board, UnitTeam team)
        {
            if (entries == null) return;

            foreach (var e in entries)
            {
                if (e.definition == null) continue;

                for (var i = 0; i < Mathf.Max(1, e.count); i++)
                {
                    if (!board.FindSpotFor(e.definition, out var row, out var col))
                    {
                        Debug.LogWarning(
                            $"[BattleSetup] Bàn {team} đã đầy, bỏ qua " +
                            $"'{e.definition.name}' thứ {i + 1}.", this);
                        break;
                    }

                    // Sinh ngay tại tâm ô. Place() sẽ xếp lại cho đúng đội hình
                    // sau khi biết trong ô có mấy con.
                    var unit = spawner.Spawn(e.definition, team,
                                             board.Grid.CellToWorld(row, col),
                                             board.transform);
                    if (unit == null) continue;

                    board.Place(unit, row, col);

                    // Quay mặt về phía địch ngay từ lúc xếp quân. UnitBrain
                    // lo việc này khi đánh, nhưng lúc chờ thì nó đang tắt —
                    // không có dòng này thì cả hai phe cùng nhìn sang phải.
                    unit.Visual?.SetFacing(team == UnitTeam.Ally ? 1f : -1f);

                    // Tắt não ngay. BattleFlow bật lại khi tới lượt ra trận.
                    var brain = unit.GetComponent<UnitBrain>();
                    if (brain != null) brain.enabled = false;
                }
            }
        }

    }
}
