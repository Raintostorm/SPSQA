using System.Collections.Generic;
using Project.Battle;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Bảng xếp hạng trong trận: sát thương gây ra, chịu đòn, hồi phục.
    ///
    /// Mỗi lúc chỉ MỘT chỉ số. Ba cái cùng hiện thì ba thang số chồng lên
    /// nhau và không cái nào đọc được — bấm thẻ để đổi.
    ///
    /// Mỗi con lính một dòng riêng, KHÔNG gộp theo loại: ba con cùng tên mà
    /// gộp lại thì không biết con nào đang gánh trận.
    /// </summary>
    public sealed class StatPanel : MonoBehaviour
    {
        public enum Metric { Dealt, Taken, Healing }

        [System.Serializable]
        public sealed class Row
        {
            public GameObject Root;
            public Text Name;
            public Image Bar;
            public Text Value;
        }

        [SerializeField] CanvasGroup group;
        [SerializeField] BattleFlow flow;
        [SerializeField] Button[] tabs;
        [SerializeField] Text allyHeader;
        [SerializeField] Text enemyHeader;
        [SerializeField] Row[] allyRows;
        [SerializeField] Row[] enemyRows;

        [Tooltip("Giây giữa hai lần vẽ lại. Đánh nhau thì số đổi liên tục, " +
                 "vẽ mỗi khung hình là phí mà mắt cũng không theo kịp.")]
        [SerializeField] float refreshInterval = 0.25f;

        [SerializeField] Color allyBar = new Color(0.36f, 0.72f, 1f);
        [SerializeField] Color enemyBar = new Color(1f, 0.45f, 0.36f);

        static readonly string[] Titles = { "SÁT THƯƠNG", "CHỊU ĐÒN", "HỒI PHỤC" };

        Metric metric = Metric.Dealt;
        float timer;
        readonly List<Unit> buf = new List<Unit>(64);

        void Awake()
        {
            if (flow == null) flow = FindFirstObjectByType<BattleFlow>();
            for (var i = 0; i < (tabs?.Length ?? 0); i++)
            {
                var m = (Metric)i;
                if (tabs[i] != null) tabs[i].onClick.AddListener(() => Select(m));
            }
        }

        void OnEnable()
        {
            if (flow != null) flow.PhaseChanged += OnPhase;
            OnPhase(flow != null ? flow.Current : BattleFlow.Phase.Prep);
        }

        void OnDisable()
        {
            if (flow != null) flow.PhaseChanged -= OnPhase;
        }

        /// <summary>Chỉ hiện khi đang đánh. Lúc xếp quân thì chưa có số nào.</summary>
        void OnPhase(BattleFlow.Phase p)
        {
            var on = p == BattleFlow.Phase.Marching || p == BattleFlow.Phase.Fighting;
            if (group != null)
            {
                group.alpha = on ? 1f : 0f;
                group.blocksRaycasts = on;
            }
            if (on) Refresh();
        }

        void Update()
        {
            if (group == null || group.alpha <= 0f) return;
            timer -= Time.unscaledDeltaTime;
            if (timer > 0f) return;
            timer = refreshInterval;
            Refresh();
        }

        public void Select(Metric m)
        {
            metric = m;
            Highlight();
            Refresh();
        }

        void Highlight()
        {
            for (var i = 0; i < (tabs?.Length ?? 0); i++)
            {
                if (tabs[i] == null) continue;
                // Tham dang mo sang ro, hai cai kia toi han. Doi MAU chu khong
                // bat tat: tat han thi nguoi choi khong biet con the nao khac.
                var img = tabs[i].GetComponent<Image>();
                if (img != null)
                    img.color = (Metric)i == metric ? Color.white : new Color(0.45f, 0.45f, 0.45f);
            }
        }

        float Value(Unit u) => metric switch
        {
            Metric.Taken => u.DamageTaken,
            Metric.Healing => u.Healing,
            _ => u.DamageDealt,
        };

        void Refresh()
        {
            if (allyHeader != null) allyHeader.text = $"QUÂN TA — {Titles[(int)metric]}";
            if (enemyHeader != null) enemyHeader.text = $"ĐỊCH — {Titles[(int)metric]}";
            var best = Fill(allyRows, UnitTeam.Ally, 0f);
            Fill(enemyRows, UnitTeam.Enemy, best);
        }

        /// <summary>
        /// Xếp một phe vào các dòng. Trả về giá trị LỚN NHẤT gặp được.
        ///
        /// Thanh số của cả hai phe đo theo CÙNG một mốc — mốc là con cao nhất
        /// của phe ta. Mỗi phe một mốc riêng thì con địch yếu nhất cũng vẽ ra
        /// thanh đầy, nhìn như nó đang thắng.
        /// </summary>
        float Fill(Row[] rows, UnitTeam team, float scaleFrom)
        {
            if (rows == null || rows.Length == 0) return scaleFrom;

            buf.Clear();
            var all = UnitRegistry.All;
            for (var i = 0; i < all.Count; i++)
            {
                var u = all[i];
                if (u != null && u.Team == team && Value(u) > 0f) buf.Add(u);
            }
            buf.Sort((a, b) => Value(b).CompareTo(Value(a)));

            var top = buf.Count > 0 ? Value(buf[0]) : 0f;
            var scale = Mathf.Max(scaleFrom, top);

            for (var i = 0; i < rows.Length; i++)
            {
                var r = rows[i];
                if (r?.Root == null) continue;
                var has = i < buf.Count;
                r.Root.SetActive(has);
                if (!has) continue;

                var u = buf[i];
                var v = Value(u);
                var def = u.Definition;
                if (r.Name != null)
                    r.Name.text = $"{i + 1}.  {(def != null ? def.DisplayName : u.name)}";
                if (r.Value != null) r.Value.text = Mathf.RoundToInt(v).ToString();
                if (r.Bar != null)
                {
                    r.Bar.color = team == UnitTeam.Ally ? allyBar : enemyBar;
                    r.Bar.fillAmount = scale > 0f ? Mathf.Clamp01(v / scale) : 0f;
                }
            }
            return scale;
        }
    }
}
