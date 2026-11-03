using Project.Battle;
using Project.Data;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>
    /// Thanh máu nổi trên đầu lính.
    ///
    /// Dùng SpriteRenderer chứ không dùng Canvas: một Canvas cho mỗi lính là
    /// rất đắt, và sprite thì ăn cùng hệ Sorting Layer với nhân vật nên không
    /// phải lo thứ tự vẽ giữa hai hệ.
    ///
    /// Co lại bằng localScale.x, nên sprite phần đầy BẮT BUỘC có pivot ở mép
    /// trái (0, 0.5). Pivot giữa thì thanh co về hai phía, trông như bị bóp.
    /// </summary>
    public sealed class HealthBar : MonoBehaviour
    {
        [SerializeField] Unit unit;
        [SerializeField] SpriteRenderer fill;
        [SerializeField] SpriteRenderer background;

        [Header("Màu theo lượng máu còn lại")]
        [SerializeField] Color allyColor  = new Color(0.35f, 0.85f, 0.35f);
        [SerializeField] Color enemyColor = new Color(0.90f, 0.35f, 0.30f);


        float fullWidth;
        UnitTeam? shownTeam;        // phe ma mau hien tai dang the hien
        UnitBrain brain;
        SpriteRenderer manaFill, manaBackground;

        void Awake()
        {
            if (unit == null) unit = GetComponentInParent<Unit>();
            if(unit)brain=unit.GetComponent<UnitBrain>();
            if (unit == null || fill == null)
            {
                Debug.LogWarning($"[HealthBar] '{name}' thiếu tham chiếu Unit " +
                                 $"hoặc Fill. Thanh máu sẽ không chạy.", this);
                enabled = false;
                return;
            }

            // Nhớ bề ngang lúc đầu rồi nhân theo tỉ lệ — KHÔNG gán scale = tỉ lệ.
            // Gán thẳng thì mọi chỉnh tay về bề ngang trong Inspector bị xoá sạch.
            fullWidth = fill.transform.localScale.x;
            manaBackground = Instantiate(background != null ? background : fill, transform);
            manaBackground.name = "ManaBackground";
            // Background has a centred pivot; fill has a left-edge pivot.
            // Keep each one's HP anchor instead of placing both at the left edge.
            manaBackground.transform.localPosition = (background != null ? background.transform.localPosition : fill.transform.localPosition) + Vector3.down * .13f;
            var bs = fill.transform.localScale; bs.y *= .6f;
            manaBackground.transform.localScale = bs;
            manaBackground.color = new Color(.06f,.12f,.23f);
            manaFill = Instantiate(fill, transform);
            manaFill.name = "ManaFill";
            manaFill.transform.localPosition = fill.transform.localPosition + Vector3.down * .13f;
            manaFill.transform.localScale = bs;
            manaFill.color = new Color(.12f,.6f,1f);
            manaFill.sortingOrder = manaBackground.sortingOrder + 1;
            manaFill.enabled = manaBackground.enabled = false;
        }

        void OnEnable()
        {
            if (unit != null) unit.Died += OnUnitDied;
        }

        void OnDisable()
        {
            if (unit != null) unit.Died -= OnUnitDied;
        }

        void LateUpdate()
        {
            // Phe chi biet SAU khi Unit.Initialize chay, ma Awake cua thanh mau
            // chay TRUOC do (Instantiate goi Awake ngay). Nen doi mau o day,
            // va chi doi khi khac — khong gan mau moi frame.
            if (shownTeam != unit.Team) { shownTeam = unit.Team; Recolor(); }

            var max = unit.MaxHp;
            if (max <= 0f) return;

            var ratio = Mathf.Clamp01(unit.CurrentHp / max);

            var s = fill.transform.localScale;
            s.x = fullWidth * ratio;
            fill.transform.localScale = s;

            // Lật mặt lính làm scale.x của GỐC thành âm, thanh máu bị lật
            // theo và chạy ngược. Bù lại bằng cách lấy dấu của cha:
            //   cha -1  ->  mình -|x|  ->  tích = +|x|   (thẳng)
            //   cha +1  ->  mình +|x|  ->  tích = +|x|
            var parent = transform.parent;
            if (parent != null)
            {
                var sign = Mathf.Sign(parent.lossyScale.x);
                var l = transform.localScale;
                var want = Mathf.Abs(l.x) * sign;
                if (!Mathf.Approximately(l.x, want))
                {
                    l.x = want;
                    transform.localScale = l;
                }
            }

            // Health is visible only while wounded; mana remains independent.
            var active = unit.IsAlive && brain != null && brain.enabled;
            var visible = active && unit.CurrentHp < max;
            var showMana = active && unit.Combat && unit.Combat.UsesMana;
            manaFill.enabled = manaBackground.enabled = showMana;
            var ms = manaFill.transform.localScale;
            ms.x = fullWidth * (unit.Combat ? unit.Combat.Mana / LegendaryCombat.MaxMana : 0f);
            manaFill.transform.localScale = ms;
            if (fill.enabled != visible)
            {
                fill.enabled = visible;
                if (background != null) background.enabled = visible;
            }
        }

        void Recolor()
        {
            fill.color = unit.Team == UnitTeam.Enemy ? enemyColor : allyColor;
        }

        void OnUnitDied(Unit _)
        {
            fill.enabled = false;
            if (manaFill) manaFill.enabled = manaBackground.enabled = false;
            if (background != null) background.enabled = false;
        }

        /// <summary>
        /// Hiện lại thanh máu. Gọi khi lính hồi sinh sang wave mới.
        ///
        /// Không có hàm này thì con từng chết sẽ không bao giờ có thanh máu
        /// nữa, dù nó vẫn đánh nhau bình thường.
        /// </summary>
        public void Restore()
        {
            bool visible = unit && unit.IsAlive && unit.CurrentHp < unit.MaxHp && brain && brain.enabled;
            fill.enabled = visible;
            if (background != null) background.enabled = visible;
        }
    }
}
