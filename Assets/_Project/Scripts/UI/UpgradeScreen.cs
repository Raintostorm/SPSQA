using Project.Data;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Màn Nâng cấp: nâng bằng vàng và mảnh riêng của từng nhân vật.
    ///
    /// Một dòng chứ không phải ô lưới: còn phải ghi cấp, giá, và nút nâng.
    /// Ô vuông không đủ chỗ cho ba thứ đó.
    /// </summary>
    public sealed class UpgradeScreen : MonoBehaviour
    {
        [Header("Do UpgradeSceneSetup gán")]
        [SerializeField] GameBalance balance;
        [SerializeField] Text goldLabel;
        [SerializeField] Button backButton;

        [Header("Bảng chi tiết")]
        [SerializeField] TreasureDatabase treasures;
        [SerializeField] CanvasGroup detailGroup;
        [SerializeField] Image detailPortrait;
        [SerializeField] Text detailTitle;
        [SerializeField] Text detailBody;
        [SerializeField] Button detailClose;
        [SerializeField] Button[] portraitButtons = new Button[20];

        [Header("20 dòng, cùng thứ tự với units")]
        [SerializeField] UnitDefinition[] units = new UnitDefinition[20];
        [SerializeField] Text[] nameLabels = new Text[20];
        [SerializeField] Text[] statLabels = new Text[20];
        [SerializeField] Button[] upgradeButtons = new Button[20];
        [SerializeField] Text[] costLabels = new Text[20];

        void Start()
        {
            // Keep serialized roster/balance references, replace the legacy row UI.
            if (AllyUpgradeView.Install(transform, units, balance, treasures)) return;
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(SceneRoutes.GoHome);
            }
            for (var i = 0; i < upgradeButtons.Length; i++)
            {
                if (upgradeButtons[i] == null) continue;
                var k = i;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => Buy(k));
            }
            for (var i = 0; i < portraitButtons.Length; i++)
            {
                if (portraitButtons[i] == null) continue;
                var k = i;
                if (i < units.Length && units[i] != null && units[i].Animations != null &&
                    units[i].Animations.Idle != null && units[i].Animations.Idle.IsValid)
                {
                    var portrait = portraitButtons[i].GetComponent<Image>();
                    if (portrait != null) portrait.sprite = units[i].Animations.Idle.FrameAt(0);
                }
                portraitButtons[i].onClick.RemoveAllListeners();
                portraitButtons[i].onClick.AddListener(() => ShowDetail(k));
            }
            if (detailClose != null)
            {
                detailClose.onClick.RemoveAllListeners();
                detailClose.onClick.AddListener(() => ShowPanel(false));
            }
            ShowPanel(false);
            Refresh();
        }

        /// <summary>
        /// Bảng chi tiết: tách rõ BA nguồn chỉ số thay vì gộp một con số.
        ///
        /// Gộp thì người chơi thấy "máu 1430" mà không biết nên nâng cấp
        /// nhân vật hay đi quay kho báu. Tách ra thì thấy ngay nguồn nào
        /// đang đóng góp ít.
        /// </summary>
        void ShowDetail(int i)
        {
            if (i < 0 || i >= units.Length || units[i] == null) return;
            var def = units[i];
            var level = SaveSystem.Current.FindUnit(def.UnitId)?.Level ?? 1;

            var lvMul = 1f + BonusAt(level);
            var tHp = treasures != null
                ? TreasureRoll.Total(TreasureStat.Hp, treasures) / 100f : 0f;
            var tAtk = treasures != null
                ? TreasureRoll.Total(TreasureStat.Atk, treasures) / 100f : 0f;

            if (detailPortrait != null && portraitButtons[i] != null)
            {
                var src = portraitButtons[i].GetComponent<Image>();
                if (src != null) detailPortrait.sprite = src.sprite;
            }
            if (detailTitle != null)
                detailTitle.text = $"{def.DisplayName}   ·   {def.Rank}   ·   Level {level}";

            if (detailBody != null)
            {
                string Line(string what, float basis, float lv, float tr) =>
                    $"{what}\n" +
                    $"   Base           {basis:N0}\n" +
                    $"   Upgrade    +{lv - 1f:P0}  →  {basis * lv:N0}\n" +
                    $"   Relics      +{tr:P0}  →  {basis * lv * (1f + tr):N0}\n" +
                    $"   TOTAL        {basis * lv * (1f + tr):N0}\n";

                detailBody.text =
                    Line("HP", def.BaseHp, lvMul, tHp) + "\n" +
                    Line("ATTACK", def.BaseAttack, lvMul, tAtk);
            }
            ShowPanel(true);
        }

        void ShowPanel(bool on)
        {
            if (detailGroup == null) return;
            detailGroup.alpha = on ? 1f : 0f;
            detailGroup.blocksRaycasts = on;
            detailGroup.interactable = on;
        }

        /// <summary>
        /// Giá lên cấp kế. Tăng theo cấp hiện tại chứ không cố định: giá
        /// phẳng thì cấp 1 và cấp 9 đắt như nhau, và chẳng có gì để cân.
        /// </summary>
        public int CostFor(int level) =>
            balance != null ? balance.UpgradeBaseCost * level : 500 * level;

        /// <summary>
        /// Mỗi cấp cộng bao nhiêu phần trăm máu và sát thương. Cùng quy tắc
        /// với vật phẩm: mỗi cấp +10% so với gốc.
        /// </summary>
        public float BonusAt(int level) => (level - 1) * 0.10f;

        void Buy(int i)
        {
            if (i < 0 || i >= units.Length || units[i] == null) return;
            if(!AllySummon.TryUpgrade(units[i].UnitId,balance))return;
            Refresh();
        }

        void Refresh()
        {
            if (goldLabel != null) goldLabel.text = $"{GameProgress.Gold:N0}";
            var save = SaveSystem.Current;
            var max = balance != null ? balance.UnitMaxLevel : 30;

            for (var i = 0; i < units.Length; i++)
            {
                if (units[i] == null) continue;
                var row = save.FindUnit(units[i].UnitId);
                var level = row?.Level ?? 1;
                var top = level >= max;

                if (nameLabels[i] != null)
                    nameLabels[i].text = units[i].DisplayName;

                // Dong ngoai CHI hien cap. Chi so day du nam o bang chi
                // tiet — nhet ca vao mot dong 1000px thi chu nho toi muc
                // khong doc duoc, ma van khong du cho.
                if (statLabels[i] != null)
                    statLabels[i].text = top ? "MAX" : $"Lv.{level}/{max} · Shards {row?.Shards ?? 0}/{AllySummon.ShardsFor(level)}";

                if (costLabels[i] != null)
                    costLabels[i].text = top ? "—" : $"{CostFor(level):N0}¢";

                if (upgradeButtons[i] != null)
                    upgradeButtons[i].interactable =
                        !top && GameProgress.Gold >= CostFor(level) && (row?.Shards ?? 0)>=AllySummon.ShardsFor(level);
            }
        }
    }
}
