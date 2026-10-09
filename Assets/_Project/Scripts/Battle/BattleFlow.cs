using System;
using System.Collections.Generic;
using Project.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Battle
{
    /// <summary>
    /// Chuyển giữa hai giai đoạn của một trận.
    ///
    ///   CHUAN BI  lưới 4×4 hiện, lính đứng trong ô, kéo thả được, não tắt
    ///   GIAO TRANH lưới ẩn, lính lần lượt ra con đường, não bật
    ///
    /// Vì sao tách lưới khỏi sàn đấu: lưới là chỗ XẾP, đường là chỗ ĐÁNH.
    /// Để lính đánh nhau ngay trên lưới thì chúng rời ô trong một frame và
    /// cái lưới thành vô nghĩa.
    ///
    /// Lính KHÔNG được sinh lại khi ra trận — vẫn là object cũ, chỉ dời chỗ
    /// và bật não. Nhờ vậy máu, trang bị và mọi thứ khác giữ nguyên.
    /// </summary>
    public sealed class BattleFlow : MonoBehaviour
    {
        /// <summary>
        /// CÔNG KHAI vì HUD cần đọc. Thêm Finished so với bản trước: không có
        /// nó thì trận đánh xong vẫn đứng mãi ở Fighting và không chỗ nào nói
        /// được ai thắng.
        /// </summary>
        public enum Phase { Prep, Marching, Fighting, Finished, LevelCleared, Defeated }

        [SerializeField] GameBalance balance;
        [SerializeField] BattleBoard allyBoard;
        [SerializeField] BattleBoard enemyBoard;
        [SerializeField] BattleLane lane;

        [Tooltip("Object chứa sprite ô lưới. Ẩn khi vào giao tranh.")]
        [SerializeField] GameObject allyCells;
        [SerializeField] GameObject enemyCells;

        [Tooltip("Để trống cũng được. Bị khoá khi ra trận.")]
        [SerializeField] UnitDragHandler dragHandler;

        /// <summary>Bắn MỖI LẦN đổi giai đoạn, kể cả lần vào Prep đầu tiên.</summary>
        public event Action<Phase> PhaseChanged;

        /// <summary>
        /// Bắn khi bắt đầu lại từ đầu, SAU khi quân cũ đã bị dọn sạch.
        /// RollService nghe để làm ví mới, BattleSetup nghe để sinh wave 1.
        /// </summary>
        public event Action RunRestarted;

        public Phase Current { get; private set; } = Phase.Prep;

        /// <summary>Phe thắng, chỉ có nghĩa khi Current == Finished.
        /// null = hoà (hai bên cùng sạch quân trong một frame).</summary>
        public UnitTeam? Winner { get; private set; }

        // Hàng đợi chứa Ô, không chứa từng con. Cả ô ra đường CÙNG MỘT LÚC
        // và giữ nguyên đội hình tam giác đã xếp — thả lẻ từng con thì người
        // chơi mất hẳn mối liên hệ giữa thứ mình xếp và thứ chạy ra.
        readonly Queue<List<Unit>> allyQueue = new Queue<List<Unit>>();
        readonly Queue<List<Unit>> enemyQueue = new Queue<List<Unit>>();

        /// <summary>
        /// Lính nào vốn đứng ô nào. Ghi lúc xuất quân, dùng lúc về sân.
        ///
        /// Không có cái này thì về sân là FindSpotFor xếp lại từ đầu, và mọi
        /// công người chơi kéo thả bị xoá sạch mỗi wave. Người chơi xếp đội
        /// hình là để nó ở yên đó.
        /// </summary>
        readonly Dictionary<Unit, Vector2Int> homeCell = new();
        int allySpawned, enemySpawned;
        float nextSpawn;

        float SpawnInterval => balance != null ? balance.WaveSpawnInterval : 0.7f;
        float PostBattlePause => balance != null ? balance.PostBattlePause : 1.5f;
        int WavesPerLevel => balance != null ? balance.WavesPerLevel : 3;

        /// <summary>Wave thứ mấy, đếm từ 1. Tăng khi quay lại chuẩn bị.</summary>
        public int Wave { get; private set; } = 1;

        float finishedTimer;

        void Start() => EnterPrep();

        void Update()
        {
            if (Current == Phase.Prep)
            {
                // Giữ phím Space song song với nút bấm — tiện lúc thử nhanh.
                var kb = Keyboard.current;
                if (kb != null && kb.spaceKey.wasPressedThisFrame) StartWave();
                return;
            }

            // LevelCleared va Defeated la trang thai DUNG YEN: cho nguoi choi
            // bam nut tren bang ket qua. Khong tick gi ca.
            if (Current == Phase.Marching) TickMarching();
            else if (Current == Phase.Fighting) TickFighting();
            else if (Current == Phase.Finished) TickFinished();
        }

        /// <summary>
        /// Một chỗ duy nhất đổi giai đoạn, nên event không thể quên bắn.
        /// Gán thẳng Current ở nơi khác là HUD đứng hình mà không báo lỗi.
        /// </summary>
        void SetPhase(Phase next)
        {
            if (Current == next) return;
            Current = next;
            PhaseChanged?.Invoke(next);
        }

        // ── chuẩn bị ──────────────────────────────────────────────────

        void EnterPrep()
        {
            Winner = null;
            Current = Phase.Prep;        // gán thẳng: đây là trạng thái đầu
            PhaseChanged?.Invoke(Phase.Prep);
            SetCells(true);
            if (dragHandler != null) dragHandler.enabled = true;

            foreach (var u in AllUnits())
            {
                var b = u.GetComponent<UnitBrain>();
                if (b != null) b.enabled = false;

                // Hồi đầy máu giữa hai wave. Không hồi thì lính sống sót
                // bước vào wave sau với máu thủng, và game thành vòng xoáy
                // chết — càng đánh càng yếu, không cách nào gỡ.
                //
                // Đây là QUYẾT ĐỊNH LUẬT CHƠI, Handoff chưa nói tới. Muốn
                // giữ máu qua wave thì xoá đúng dòng này.
                u.RestoreFull();
            }
            Debug.Log("[BattleFlow] Chuẩn bị. Xếp quân rồi bấm Space.", this);
        }

        // ── ra trận ───────────────────────────────────────────────────

        /// <summary>Gọi từ nút trên HUD, hoặc phím Space. Bỏ qua nếu không
        /// đang ở Prep — bấm hai lần không làm quân ra trận hai lượt.</summary>
        public void StartWave()
        {
            if (Current != Phase.Prep) return;
            SetPhase(Phase.Marching);
            SetCells(false);
            if (dragHandler != null) dragHandler.enabled = false;

            // Bu mau LAN NUA ngay truoc khi ra tran. EnterPrep da hoi day
            // roi, nhung buff mua trong luc chuan bi lam MaxHp tang sau do —
            // khong bu lai thi linh ra tran voi thanh mau thieu dung bang
            // phan buff vua mua.
            foreach (var u in AllUnits()) u.TopUpToMax();

            homeCell.Clear();
            Fill(allyQueue, allyBoard, UnitTeam.Ally, homeCell);
            Fill(enemyQueue, enemyBoard, UnitTeam.Enemy, homeCell);

            // Chỉ ẩn phần nhìn thấy. Không tắt GameObject vì Unit.OnDisable
            // sẽ rút quân khỏi Registry và làm sai việc đếm quân đang chờ.
            SetQueueVisible(allyQueue, false);
            SetQueueVisible(enemyQueue, false);
            allySpawned = enemySpawned = 0;
            nextSpawn = 0f;

            Debug.Log($"[BattleFlow] Ra trận — {allyQueue.Count} ô ally, " +
                      $"{enemyQueue.Count} ô enemy, mỗi {SpawnInterval:0.00}s một ô.\n" +
                      $"Thứ tự ally: {Describe(allyQueue, homeCell)}", this);
        }

        /// <summary>
        /// Xếp hàng ra trận theo CỘT: cột gần địch nhất đi trước.
        /// Đó là lý do người chơi phải nghĩ khi kéo thả — đặt ở đâu quyết định
        /// ai ra tuyến đầu.
        ///
        /// Mỗi phần tử là MỘT Ô. Ô trống bị bỏ qua, không chiếm một lượt chờ.
        /// </summary>
        static void Fill(Queue<List<Unit>> q, BattleBoard board, UnitTeam team,
                         Dictionary<Unit, Vector2Int> home)
        {
            q.Clear();
            if (board == null) return;

            var cols = board.Grid.Cols;
            for (var i = 0; i < cols; i++)
            {
                // Ally dung ben trai nen cot PHAI nhat gan dich nhat, va nguoc lai.
                var c = team == UnitTeam.Ally ? cols - 1 - i : i;
                for (var r = 0; r < board.Grid.Rows; r++)
                {
                    var alive = new List<Unit>();
                    foreach (var u in board.UnitsAt(r, c))
                    {
                        if (u == null || !u.IsAlive) continue;
                        alive.Add(u);
                        home[u] = new Vector2Int(r, c);   // nho cho de con ve
                    }
                    if (alive.Count > 0) q.Enqueue(alive);
                }
            }
        }

        /// <summary>
        /// Thứ tự xuất quân, dạng người đọc được. In ra để đối chiếu với
        /// cách mình đã xếp — không in thì "ai ra trước" là một lời hứa
        /// không ai kiểm được.
        /// </summary>
        static string Describe(Queue<List<Unit>> q, Dictionary<Unit, Vector2Int> home)
        {
            var sb = new System.Text.StringBuilder();
            var n = 1;
            foreach (var cell in q)
            {
                if (cell.Count == 0) continue;
                var at = home.TryGetValue(cell[0], out var h) ? $"({h.x},{h.y})" : "(?)";
                sb.Append($"{n++}. {at} {cell[0].Definition?.name} ×{cell.Count}   ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Bắt đầu lại từ wave 1: dọn sạch quân, ví mới, buff mới.
        ///
        /// Dọn rồi CHỜ MỘT KHUNG HÌNH mới sinh lại. Destroy của Unity chỉ
        /// thi hành vào cuối khung, nên sinh ngay thì quân mới và quân cũ
        /// cùng tồn tại một lúc: BattleBoard thấy ô đã đầy và từ chối, còn
        /// UnitRegistry thì đếm cả xác cũ.
        /// </summary>
        public void RestartRun()
        {
            foreach (var u in new List<Unit>(AllUnits()))
            {
                if (u == null) continue;
                BoardOf(u.Team)?.Remove(u);
                Destroy(u.gameObject);
            }
            homeCell.Clear();
            allyQueue.Clear();
            enemyQueue.Clear();
            Winner = null;
            Wave = 1;
            StartCoroutine(RestartNextFrame());
        }

        System.Collections.IEnumerator RestartNextFrame()
        {
            yield return null;               // cho Destroy thi hanh xong
            RunRestarted?.Invoke();
            EnterPrep();
            Debug.Log("[BattleFlow] Chơi lại từ wave 1.", this);
        }

        void TickMarching()
        {
            nextSpawn -= Time.deltaTime;
            if (nextSpawn > 0f) return;
            nextSpawn = SpawnInterval;

            var any = Release(allyQueue, UnitTeam.Ally, ref allySpawned)
                    | Release(enemyQueue, UnitTeam.Enemy, ref enemySpawned);

            if (!any && allyQueue.Count == 0 && enemyQueue.Count == 0)
            {
                SetPhase(Phase.Fighting);
                Debug.Log("[BattleFlow] Đã ra hết quân.", this);
            }
        }

        /// <summary>
        /// CHỈ kiểm khi đã ra hết quân. Kiểm sớm hơn là sai: lúc đang xuất
        /// quân, con đầu tiên của một phe có thể chết trước khi con thứ hai
        /// kịp ra, và phe đó bị tính là thua oan.
        /// </summary>
        void TickFighting()
        {
            var ally  = UnitRegistry.CountAlive(UnitTeam.Ally);
            var enemy = UnitRegistry.CountAlive(UnitTeam.Enemy);
            if (ally > 0 && enemy > 0) return;

            Winner = ally > 0 ? UnitTeam.Ally
                   : enemy > 0 ? UnitTeam.Enemy
                   : (UnitTeam?)null;
            SetPhase(Phase.Finished);
            finishedTimer = PostBattlePause;
            Debug.Log($"[BattleFlow] Hết trận wave {Wave} — {(Winner?.ToString() ?? "hoà")}. " +
                      $"Còn lại {ally} ally, {enemy} enemy.", this);
        }

        /// <summary>
        /// Nghỉ một nhịp cho người chơi đọc kết quả, rồi về lại sân chuẩn bị.
        ///
        /// Không về ngay: trận vừa kết thúc là lúc màn hình đang động nhất,
        /// cắt phụt sang màn khác thì người chơi không kịp thấy mình thắng.
        /// </summary>
        void TickFinished()
        {
            finishedTimer -= Time.deltaTime;
            if (finishedTimer > 0f) return;

            // Thua: phe ta sach quan. Het run, khong ve san nua.
            if (Winner == UnitTeam.Enemy)
            {
                SetPhase(Phase.Defeated);
                Debug.Log($"[BattleFlow] THUA ở wave {Wave}.", this);
                return;
            }

            // Thang du so wave cua man: clear level. Kiem TRUOC khi ve san, khong thi
            // no don dep roi vao wave 11 va man thang khong bao gio hien.
            if (Wave >= WavesPerLevel)
            {
                SetPhase(Phase.LevelCleared);
                Debug.Log($"[BattleFlow] QUA MÀN — hết {WavesPerLevel} wave.", this);
                return;
            }

            ReturnToBoard();
        }

        /// <summary>
        /// Dọn xác, đưa người sống về lưới, rồi sang wave kế.
        ///
        /// Lính sống KHÔNG bị sinh lại — vẫn là object cũ, chỉ dời chỗ. Nhờ
        /// vậy máu, trang bị, và mọi thứ gắn vào nó đều giữ nguyên, giống
        /// hệt lúc ra trận.
        /// </summary>
        void ReturnToBoard()
        {
            // Chép ra danh sách trước: vòng lặp dưới gọi board.Place, mà
            // Place đụng vào các danh sách mà AllUnits đang duyệt.
            var all = new List<Unit>();
            foreach (var u in AllUnits()) all.Add(u);

            var lost = 0;
            foreach (var u in all)
            {
                if (u == null) continue;

                // Xác ĐỊCH thì DỌN. Lính ta thì giữ.
                //
                // Hai phe hai luật, và đó là cố ý. Bàn cờ của người chơi là
                // tài sản — xoá xác ta thì mỗi wave mất vài con, wave 3 là
                // bàn trống và người chơi không hiểu quân mình đi đâu.
                //
                // Nhưng giữ xác ĐỊCH là một lỗi: BattleSetup sinh lại nguyên
                // danh sách địch mỗi wave, nên xác cũ ở lại là quân địch
                // CỘNG DỒN. Đo được: 3 → 7 → 12 → 18 con, trong khi luật là
                // 3 → 4 → 5. Không có gì báo, chỉ là wave 3 bỗng không thắng
                // nổi.
                if (u.Team == UnitTeam.Enemy && !u.IsAlive)
                {
                    BoardOf(u.Team)?.Remove(u);
                    Destroy(u.gameObject);
                    continue;
                }

                var board = BoardOf(u.Team);
                if (board == null) continue;

                // Về ĐÚNG ô cũ. Rơi vào FindSpotFor là mọi công kéo thả của
                // người chơi bị xoá sạch — phải kêu lên chứ không im lặng.
                int row, col;
                if (homeCell.TryGetValue(u, out var home))
                {
                    row = home.x; col = home.y;
                }
                else if (board.FindSpotFor(u.Definition, out row, out col))
                {
                    Debug.LogWarning($"[BattleFlow] Không nhớ ô cũ của " +
                                     $"'{u.name}', xếp tạm vào ({row},{col}).", this);
                }
                else
                {
                    Debug.LogWarning($"[BattleFlow] Không còn chỗ cho '{u.name}' " +
                                     $"khi về lưới.", this);
                    lost++;
                    continue;
                }

                u.transform.SetParent(board.transform, true);
                u.transform.position = board.Grid.CellToWorld(row, col);
                if (!board.Place(u, row, col))
                {
                    Debug.LogWarning($"[BattleFlow] Ô ({row},{col}) từ chối " +
                                     $"'{u.name}' khi về sân.", this);
                    lost++;
                    continue;
                }
                u.Visual?.SetFacing(u.Team == UnitTeam.Ally ? 1f : -1f);
            }

            homeCell.Clear();
            Wave++;
            allyQueue.Clear();
            enemyQueue.Clear();
            EnterPrep();
            Debug.Log($"[BattleFlow] Về sân, vào wave {Wave}." +
                      (lost > 0 ? $" Mất {lost} lính vì hết chỗ." : string.Empty), this);
        }

        /// <summary>Thả NGUYÊN một ô ra đường, giữ đội hình tam giác.</summary>
        bool Release(Queue<List<Unit>> q, UnitTeam team, ref int count)
        {
            if (q.Count == 0) return false;
            var cell = q.Dequeue();

            var board = BoardOf(team);
            var entry = lane != null ? lane.EntryPoint(team, count) : Vector3.zero;
            var grid = board != null ? board.Grid : null;

            // Đếm con còn sống TRƯỚC khi thả: đội hình 3 con khác đội hình 2
            // con, mà có thể vài con đã chết từ wave trước.
            var n = 0;
            foreach (var u in cell) if (u != null && u.IsAlive) n++;
            if (n == 0) return true;                    // o rong van tinh mot luot

            var slot = 0;
            foreach (var u in cell)
            {
                if (u == null || !u.IsAlive) continue;

                // Rut khoi luoi TRUOC khi doi cho — khong thi BattleBoard van
                // tuong no dang dung trong o va se keo no ve moi lan Reflow.
                board?.Remove(u);

                u.transform.SetParent(lane != null ? lane.transform : null, true);
                if (lane != null)
                {
                    var off = grid != null ? grid.FormationOffset(slot, n) : Vector2.zero;
                    u.transform.position = entry + new Vector3(off.x, off.y, 0f);
                }
                u.Visual?.SetFacing(team == UnitTeam.Ally ? 1f : -1f);
                u.Visual?.SetVisible(true);

                var brain = u.GetComponent<UnitBrain>();
                if (brain != null) brain.enabled = true;
                slot++;
            }

            count++;                                    // dem O, khong dem con
            return true;
        }

        // ── tiện ích ──────────────────────────────────────────────────

        static void SetQueueVisible(Queue<List<Unit>> queue, bool visible)
        {
            foreach (var cell in queue)
            foreach (var unit in cell)
                if (unit != null) unit.Visual?.SetVisible(visible);
        }

        BattleBoard BoardOf(UnitTeam t) => t == UnitTeam.Ally ? allyBoard : enemyBoard;

        void SetCells(bool on)
        {
            if (allyCells != null) allyCells.SetActive(on);
            if (enemyCells != null) enemyCells.SetActive(on);
        }

        IEnumerable<Unit> AllUnits()
        {
            foreach (var u in UnitRegistry.All)
                if (u != null) yield return u;
        }
    }
}
