using System.Collections.Generic;
using Project.Data;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Màn Kho báu: quay rương, xem 30 ô vật phẩm.
    ///
    /// Ô vật phẩm dựng SẴN đủ 30 lúc build scene, không sinh lúc chạy. Món
    /// chưa có thì hiện khung xám và icon mờ — ẩn hẳn thì người chơi không
    /// biết còn gì phía trước, mà biết còn gì phía trước chính là lý do họ
    /// quay tiếp.
    /// </summary>
    public sealed class TreasureScreen : MonoBehaviour
    {
        [Header("Do TreasureSceneSetup gán")]
        [SerializeField] TreasureDatabase database;
        [SerializeField] GameBalance balance;
        [SerializeField] Text keyLabel;
        [SerializeField] Text resultLabel;
        [SerializeField] Image chestImage;
        [SerializeField] Sprite chestClosed;
        [SerializeField] Sprite chestOpen;
        [SerializeField] Button backButton;

        [Header("Ba nút quay")]
        [SerializeField] Button[] rollButtons = new Button[3];
        [SerializeField] int[] rollCounts = { 1, 10, 100 };

        [Header("Bảng chi tiết")]
        [SerializeField] CanvasGroup detailGroup;
        [SerializeField] Image detailIcon;
        [SerializeField] Text detailTitle;
        [SerializeField] Text detailBody;
        [SerializeField] Button detailClose;

        [Header("Lưới 30 ô — cùng thứ tự với database")]
        [SerializeField] Button[] slotButtons = new Button[30];
        [SerializeField] Image[] slotFrames = new Image[30];
        [SerializeField] Image[] slotIcons = new Image[30];
        [SerializeField] Text[] slotLevels = new Text[30];

        [Header("Khung theo bậc: khoá, Common, Rare, Epic, Legendary")]
        [SerializeField] Sprite[] frameByTier = new Sprite[5];

        System.Random rng;

        void Start()
        {
            // Hat giong theo dong ho: moi lan vao man la mot day ket qua khac.
            // Co dinh hat giong thi quay lai game van ra dung thu tu cu.
            rng = new System.Random(System.Environment.TickCount);

            for (var i = 0; i < rollButtons.Length; i++)
            {
                if (rollButtons[i] == null) continue;
                var n = i < rollCounts.Length ? rollCounts[i] : 1;
                rollButtons[i].onClick.RemoveAllListeners();
                rollButtons[i].onClick.AddListener(() => DoRoll(n));
            }
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(SceneRoutes.GoHome);
            }
            for (var i = 0; i < slotButtons.Length; i++)
            {
                if (slotButtons[i] == null) continue;
                var k = i;
                slotButtons[i].onClick.RemoveAllListeners();
                slotButtons[i].onClick.AddListener(() => ShowDetail(k));
            }
            if (detailClose != null)
            {
                detailClose.onClick.RemoveAllListeners();
                detailClose.onClick.AddListener(() => ShowPanel(false));
            }
            ShowPanel(false);
            Refresh();
        }

        void DoRoll(int count)
        {
            if (database == null || balance == null) return;

            if (!GameProgress.SpendKeys(count))
            {
                if (resultLabel != null)
                    resultLabel.text = $"Thiếu chìa — cần {count}, có {GameProgress.Keys}";
                return;
            }

            var got = TreasureRoll.Roll(count, database, balance, rng);
            Report(got);
            Refresh();
        }

        void Report(List<TreasureRoll.Result> got)
        {
            if (resultLabel == null) return;
            if (got.Count == 0) { resultLabel.text = "Không ra gì."; return; }

            // Mot luot x100 khong the liet ke 100 dong. Dem theo bac, roi
            // goi ten rieng thu HIEM nhat — do la thu nguoi choi muon biet.
            var byTier = new int[4];
            TreasureRoll.Result best = default;
            foreach (var r in got)
            {
                if (r.Item == null) continue;
                byTier[(int)r.Item.Tier]++;
                if (best.Item == null || r.Item.Tier > best.Item.Tier) best = r;
            }

            var line = $"Common {byTier[0]} · Rare {byTier[1]} · " +
                       $"Epic {byTier[2]} · Legend {byTier[3]}";
            if (best.Item != null)
                line += $"\n{best.Item.DisplayName} — " +
                        (best.IsNew ? "MỚI!" : $"cấp {best.NewLevel}");
            resultLabel.text = line;

            if (chestImage != null && chestOpen != null)
                chestImage.sprite = chestOpen;
        }

        /// <summary>
        /// Bảng chi tiết một món. Hiện CẢ món chưa có — người chơi phải biết
        /// mình đang quay để lấy cái gì, không thì mọi món chưa mở đều như nhau.
        /// </summary>
        void ShowDetail(int index)
        {
            if (database == null || index < 0 || index >= database.Items.Length) return;
            var item = database.Items[index];
            var owned = SaveSystem.Current.FindItem(item.Id);
            var level = owned?.Level ?? 0;

            if (detailIcon != null)
            {
                detailIcon.sprite = item.Icon;
                detailIcon.color = owned != null ? Color.white
                                                 : new Color(0.3f, 0.3f, 0.33f, 0.9f);
            }
            if (detailTitle != null)
                detailTitle.text = owned == null
                    ? $"{item.DisplayName}  ·  {TierName(item.Tier)}  ·  CHƯA CÓ"
                    : $"{item.DisplayName}  ·  {TierName(item.Tier)}  ·  " +
                      (level >= item.MaxLevel ? "TỐI ĐA" : $"Cấp {level}/{item.MaxLevel}");

            if (detailBody != null)
            {
                var sb = new System.Text.StringBuilder();
                foreach (var e in item.Stats)
                {
                    // Chua co thi hien gia tri CAP 1 — do la thu se nhan duoc.
                    var now = item.ValueAt(e.Stat, Mathf.Max(1, level));
                    sb.Append($"{StatName(e.Stat)}  +{now:0.#}{Unit(e.Stat)}");
                    if (level > 0 && level < item.MaxLevel)
                        sb.Append($"   →  +{item.ValueAt(e.Stat, level + 1):0.#}{Unit(e.Stat)}");
                    sb.Append('\n');
                }
                if (owned != null && level < item.MaxLevel)
                    sb.Append($"\nMảnh: {owned.Shards}/{TreasureItem.ShardsToNext(level)}");
                else if (owned == null)
                    sb.Append("\nQuay rương để nhận.");
                detailBody.text = sb.ToString();
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

        static string TierName(Rank r) => r switch
        {
            Rank.Rare => "Rare", Rank.Epic => "Epic",
            Rank.Legendary => "Legendary", _ => "Common",
        };

        static string StatName(TreasureStat s) => s switch
        {
            TreasureStat.Atk => "Sát thương",
            TreasureStat.Hp => "Máu",
            TreasureStat.AtkSpeed => "Tốc đánh",
            TreasureStat.CritDamage => "ST chí mạng",
            TreasureStat.CritChance => "Chí mạng",
            TreasureStat.LifeSteal => "Hút máu",
            _ => "Giáp",
        };

        /// <summary>Hai thang đơn vị khác nhau — ghi sai là người chơi hiểu sai.</summary>
        static string Unit(TreasureStat s) =>
            s is TreasureStat.CritChance or TreasureStat.LifeSteal or TreasureStat.Armor
                ? " điểm" : "%";

        void Refresh()
        {
            if (keyLabel != null) keyLabel.text = $"{GameProgress.Keys:N0}";
            if (database == null) return;

            var save = SaveSystem.Current;
            var n = Mathf.Min(database.Items.Length, slotFrames.Length);
            for (var i = 0; i < n; i++)
            {
                var item = database.Items[i];
                var owned = save.FindItem(item.Id);
                var have = owned != null;

                if (slotFrames[i] != null && frameByTier.Length >= 5)
                    slotFrames[i].sprite = have ? frameByTier[(int)item.Tier + 1]
                                                : frameByTier[0];

                if (slotIcons[i] != null)
                {
                    slotIcons[i].sprite = item.Icon;
                    // Chua co thi lam TOI di chu khong an: van thay hinh gi,
                    // van biet con thieu mon nao.
                    slotIcons[i].color = have ? Color.white
                                              : new Color(0.25f, 0.25f, 0.28f, 0.85f);
                }

                if (slotLevels[i] != null)
                    slotLevels[i].text = have
                        ? (owned.Level >= item.MaxLevel ? "MAX" : $"Lv{owned.Level}")
                        : string.Empty;
            }
        }
    }
}
