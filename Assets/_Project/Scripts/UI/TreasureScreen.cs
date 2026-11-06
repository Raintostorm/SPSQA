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
    public sealed partial class TreasureScreen : MonoBehaviour
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
        bool revealing;

        void LegacyStart()
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
            if (revealing || database == null || balance == null) return;

            // Debit, grants and pity commit together; failure leaves keys untouched.
            if (!GameProgress.TryOpenChest(count, database, balance, rng, out var got))
            {
                if (resultLabel != null)
                    resultLabel.text = GameProgress.Keys < count
                        ? $"Not enough keys — need {count}, have {GameProgress.Keys}"
                        : "Could not open chest — keys were not spent";
                return;
            }
            // Saved before the presentation; closing a screen cannot lose rewards.
            StartCoroutine(RevealChest(got));
        }

        System.Collections.IEnumerator RevealChest(List<TreasureRoll.Result> got)
        {
            revealing=true;
            foreach(var button in rollButtons)if(button)button.interactable=false;
            if(chestImage && chestClosed)chestImage.sprite=chestClosed;
            var chest=chestImage ? chestImage.rectTransform : null;
            var scale=chest ? chest.localScale : Vector3.one;
            float time=0;
            while(time<.65f)
            {
                time+=Time.unscaledDeltaTime;
                if(chest){chest.localRotation=Quaternion.Euler(0,0,Mathf.Sin(time*55)*7);chest.localScale=scale*(1+.08f*Mathf.Sin(time*20));}
                yield return null;
            }
            if(chest){chest.localRotation=Quaternion.identity;chest.localScale=scale;}
            Report(got);Refresh();
            yield return new WaitForSecondsRealtime(.2f);
            var groups=new Dictionary<TreasureItem,List<TreasureRoll.Result>>();
            foreach(var reward in got){if(reward.Item==null)continue;if(!groups.ContainsKey(reward.Item))groups[reward.Item]=new List<TreasureRoll.Result>();groups[reward.Item].Add(reward);}
            var canvas=GetComponentInParent<Canvas>();
            var overlay=new GameObject("ChestRewards",typeof(RectTransform),typeof(Image));
            overlay.transform.SetParent(canvas ? canvas.transform : transform,false);
            var root=overlay.GetComponent<RectTransform>();root.anchorMin=Vector2.zero;root.anchorMax=Vector2.one;root.offsetMin=root.offsetMax=Vector2.zero;
            overlay.GetComponent<Image>().color=new Color(.035f,.05f,.1f,.97f);
            RewardText(root,"RELICS RECEIVED",new Vector2(0,830),new Vector2(980,100),52);
            var close=new GameObject("Continue",typeof(RectTransform),typeof(Image),typeof(Button));close.transform.SetParent(root,false);
            BattleUiSkin.Place(close.GetComponent<RectTransform>(),Vector2.one*.5f,new Vector2(0,-870),new Vector2(600,110));
            ScreenChrome.Action(close.GetComponent<Button>(),ScreenChrome.Role.Secondary);
            RewardText(close.transform,"Continue",Vector2.zero,new Vector2(560,100),44);
            bool dismissed=false;
            close.GetComponent<Button>().onClick.AddListener(()=>dismissed=true);
            close.SetActive(false);
            int index=0;
            foreach(var entry in groups)
            {
                var card=new GameObject("RelicCard",typeof(RectTransform),typeof(Image),typeof(CanvasGroup));card.transform.SetParent(root,false);
                var rect=card.GetComponent<RectTransform>();
                BattleUiSkin.Place(rect,Vector2.one*.5f,new Vector2((index%5-2)*195,650-index/5*240),new Vector2(180,220));
                var tier=(int)entry.Key.Tier;
                card.GetComponent<Image>().color=tier==3?new Color(.55f,.36f,.06f):tier==2?new Color(.35f,.15f,.5f):tier==1?new Color(.08f,.28f,.5f):new Color(.22f,.25f,.3f);
                var icon=new GameObject("Icon",typeof(RectTransform),typeof(Image));icon.transform.SetParent(rect,false);
                BattleUiSkin.Place(icon.GetComponent<RectTransform>(),Vector2.one*.5f,new Vector2(0,28),new Vector2(105,105));
                var image=icon.GetComponent<Image>();image.sprite=entry.Key.Icon;image.enabled=image.sprite!=null;image.preserveAspect=true;image.raycastTarget=false;
                bool isNew=entry.Value.Exists(r=>r.IsNew);
                RewardText(rect,entry.Key.DisplayName,new Vector2(0,-48),new Vector2(170,60),23);
                RewardText(rect,(isNew?"NEW · ":"")+"×"+entry.Value.Count,new Vector2(0,-88),new Vector2(170,35),25);
                float pop=0;while(pop<.09f){pop+=Time.unscaledDeltaTime;rect.localScale=Vector3.one*Mathf.Lerp(.75f,1,Mathf.Clamp01(pop/.09f));yield return null;}rect.localScale=Vector3.one;
                index++;
            }
            close.SetActive(true);
            while(!dismissed)yield return null;
            Destroy(overlay);revealing=false;
            foreach(var button in rollButtons)if(button)button.interactable=true;
        }

        Text RewardText(Transform parent,string value,Vector2 position,Vector2 size,int fontSize)
        {
            var text=new GameObject("Label",typeof(RectTransform),typeof(Text)).GetComponent<Text>();text.transform.SetParent(parent,false);
            BattleUiSkin.Place(text.rectTransform,Vector2.one*.5f,position,size);text.text=value;text.font=UiFonts.Display;text.fontSize=fontSize;text.alignment=TextAnchor.MiddleCenter;text.color=Color.white;text.raycastTarget=false;return text;
        }

        void Report(List<TreasureRoll.Result> got)
        {
            if (resultLabel == null) return;
            if (got.Count == 0) { resultLabel.text = "No items received."; return; }

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
                        (best.IsNew ? "NEW!" : $"level {best.NewLevel}");
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
                    ? $"{item.DisplayName}  ·  {TierName(item.Tier)}  ·  NOT OWNED"
                    : $"{item.DisplayName}  ·  {TierName(item.Tier)}  ·  " +
                      (level >= item.MaxLevel ? "MAX" : $"Level {level}/{item.MaxLevel}");

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
                    sb.Append($"\nShards: {owned.Shards}/{TreasureItem.ShardsToNext(level)}");
                else if (owned == null)
                    sb.Append("\nOpen chests to obtain.");
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
            TreasureStat.MythicCapacity => "Mythic limit · +1 every 2 levels",
            TreasureStat.Atk => "Attack",
            TreasureStat.Hp => "HP",
            TreasureStat.AtkSpeed => "Attack Speed",
            TreasureStat.CritDamage => "Crit Damage",
            TreasureStat.CritChance => "Crit Chance",
            TreasureStat.LifeSteal => "Lifesteal",
            _ => "Armor",
        };

        /// <summary>Hai thang đơn vị khác nhau — ghi sai là người chơi hiểu sai.</summary>
        static string Unit(TreasureStat s) =>
            s is TreasureStat.CritChance or TreasureStat.LifeSteal or TreasureStat.Armor
                ? " points" : "%";

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
