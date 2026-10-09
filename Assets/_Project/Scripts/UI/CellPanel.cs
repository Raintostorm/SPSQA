using Project.Battle;
using Project.Data;
using Project.Gacha;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Bảng thông tin một ô: chỉ số lính, và với phe ta thì thêm nút Bán /
    /// Tiến hoá.
    ///
    /// Nó HỎI, logic TRẢ LỜI — đúng tầng mà Handoff/09 chốt. Bảng không tự
    /// xét "ô này có 3 con giống nhau không", nó gọi
    /// <see cref="EvolutionSystem.CanEvolve"/> rồi hiện hay ẩn nút theo câu
    /// trả lời. Tự xét là có hai bản luật.
    ///
    /// Không có nút Đóng: bấm ra chỗ trống là đóng, CellInspector lo.
    /// </summary>
    public sealed class CellPanel : MonoBehaviour
    {
        [SerializeField] GameBalance balance;
        [SerializeField] UnitPool pool;
        [SerializeField] UnitSpawner spawner;
        [SerializeField] RollService roll;

        [Header("Do BattleHudBuilder gán")]
        [SerializeField] CanvasGroup group;
        [SerializeField] Text title;
        [SerializeField] Text stats;
        [SerializeField] Button sellButton;
        [SerializeField] Text sellLabel;
        [SerializeField] Button evolveButton;

        BattleBoard board;
        Project.Visual.CellTiles lit;
        int row, col;
        System.Random rng;

        void Awake()
        {
            rng = new System.Random();
            if (sellButton != null) sellButton.onClick.AddListener(DoSell);
            if (evolveButton != null) evolveButton.onClick.AddListener(DoEvolve);
            Close();
        }

        public void Open(BattleBoard b, int r, int c)
        {
            // Tat o dang sang o BAN CU truoc khi doi ban. Bam o phe ta roi
            // bam sang o phe dich ma khong co dong nay thi ca hai cung sang.
            Highlight(null);
            board = b; row = r; col = c;
            Refresh();
        }

        public void Close()
        {
            Highlight(null);
            board = null;
            Show(false);
        }

        /// <summary>
        /// Lam sang o dang xem, tat o sang truoc do.
        ///
        /// Dat o CellPanel chu khong o CellInspector: Close() duoc goi tu
        /// bon cho — luc khoi dong, luc bam ra cho trong, luc o rong sau khi
        /// ban, va tu nut dong cua chinh bang. Noi o CellInspector thi ba
        /// duong con lai de o sang mac ket.
        /// </summary>
        void Highlight(BattleBoard b)
        {
            if (lit != null) { lit.Deselect(); lit = null; }
            if (b == null) return;
            lit = b.GetComponent<Project.Visual.CellTiles>();
            if (lit != null) lit.Select(row, col);
        }

        void Refresh()
        {
            var units = board?.UnitsAt(row, col);
            if (units == null || units.Count == 0) { Close(); return; }

            // Bat sang o DAY chu khong o Open(): Refresh() moi la cho biet
            // chac o nay co linh. Bat o Open() thi bam vao o rong cung sang
            // mot nhip roi moi tat.
            Highlight(board);

            var def = units[0].Definition;
            var ally = units[0].Team == UnitTeam.Ally;

            if (title != null)
                title.text = def != null
                    ? $"{def.DisplayName}  ×{units.Count}"
                    : $"? ×{units.Count}";

            // Doc chi so tu UNIT chu khong tu UnitDefinition: Unit da nhan
            // buff vao roi, con definition thi chua. Hien so goc la noi doi
            // dung bang cach cu — xem RunBuffs.
            if (stats != null)
            {
                var u = units[0];
                stats.text = def == null ? string.Empty :
                    $"{def.Race} · {def.Rank}   ·   {(def.IsRanged ? "đánh xa" : "cận chiến")}\n" +
                    $"MÁU {u.MaxHp:0}      SÁT THƯƠNG {u.AttackPower:0}\n" +
                    $"TẦM {def.AttackRange:0.0}   NHỊP {def.AttackCooldown:0.00}s";
            }

            // Phe địch: chỉ xem, không bán không tiến hoá.
            if (sellButton != null) sellButton.gameObject.SetActive(ally);
            if (evolveButton != null) evolveButton.gameObject.SetActive(ally);

            if (ally)
            {
                var price = SellPrice(units);
                if (sellLabel != null) sellLabel.text = $"BÁN  +{price}¢";
                if (evolveButton != null)
                    evolveButton.interactable =
                        EvolutionSystem.CanEvolve(board, row, col, pool, out _, out _);
            }

            Show(true);
        }

        int SellPrice(System.Collections.Generic.IReadOnlyList<Unit> units)
        {
            var total = 0;
            var table = balance != null ? balance.SellPriceByRank : null;
            if (table == null) return 0;
            foreach (var u in units)
            {
                if (u?.Definition == null) continue;
                var i = (int)u.Definition.Rank;
                if (i < table.Length) total += table[i];
            }
            return total;
        }

        void DoSell()
        {
            if (board == null) return;
            var coins = EvolutionSystem.Sell(board, row, col, balance);
            roll?.Run?.AddCoins(coins);
            Debug.Log($"[CellPanel] Bán ô ({row},{col}) được {coins}¢", this);
            Close();
        }

        void DoEvolve()
        {
            if (board == null) return;
            var unit = EvolutionSystem.Evolve(board, row, col, pool, spawner, rng);
            if (unit == null) return;
            Debug.Log($"[CellPanel] Ô ({row},{col}) tiến hoá → {unit.name}", this);
            Refresh();          // ve lai: gio o chi con 1 con, nut doi theo
        }

        void Show(bool on)
        {
            if (group == null) return;
            group.alpha = on ? 1f : 0f;
            group.blocksRaycasts = on;
            group.interactable = on;
        }
    }
}
