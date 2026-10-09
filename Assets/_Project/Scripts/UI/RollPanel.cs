using System.Collections;
using Project.Battle;
using Project.Gacha;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Sáu ô kết quả của một lượt roll.
    ///
    /// CHỈ vẽ lại những gì RollPayout đã quyết. Nó không đếm cụm, không biết
    /// luật "hoà thì cả hai cùng ăn" — cờ thắng đọc thẳng từ
    /// <see cref="RollPayout.CellWins"/>. Đếm lại ở đây là có hai bản luật.
    ///
    /// Có animation quay: sáu ô đảo icon rồi dừng LẦN LƯỢT từ trái sang.
    /// Dừng cùng lúc thì không có hồi hộp — cái hay của máy quay nằm ở chỗ
    /// ô cuối còn đang chạy mà năm ô kia đã lộ.
    /// </summary>
    public sealed class RollPanel : MonoBehaviour
    {
        [SerializeField] RollService roll;
        [SerializeField] BattleFlow flow;
        [SerializeField] CanvasGroup group;

        [Header("Do BattleHudBuilder gán")]
        [SerializeField] Image[] slots = new Image[6];
        [SerializeField] Image[] icons = new Image[6];
        [SerializeField] Text summary;

        [Header("Sprite")]
        [SerializeField] Sprite slotNormal;
        [SerializeField] Sprite slotWin;
        [SerializeField] Sprite coinIcon;
        [SerializeField] Sprite allyIcon;
        [SerializeField] Sprite buffIcon;

        [Header("Nhịp quay")]
        [Tooltip("Giây giữa hai lần đảo icon lúc đang quay.")]
        [SerializeField] float tick = 0.05f;

        [Tooltip("Giây giữ bảng trên màn sau khi chốt xong rồi tự mờ đi. " +
                 "Đặt 0 là không bao giờ tự tắt.")]
        [SerializeField] float holdSeconds = 5f;

        [Tooltip("Giây để mờ hẳn. Tắt phụt một cái thì mắt giật.")]
        [SerializeField] float fadeSeconds = 0.4f;

        [Tooltip("Giây giữa hai ô dừng. Ô đầu dừng sau ngần này.")]
        [SerializeField] float stagger = 0.13f;

        Coroutine spin;

        void OnEnable()
        {
            if (roll == null) roll = FindFirstObjectByType<RollService>();
            if (flow == null) flow = FindFirstObjectByType<BattleFlow>();
            if (roll != null) roll.Rolled += Show;
            if (flow != null) flow.PhaseChanged += OnPhase;
            SetVisible(false);
        }

        void OnDisable()
        {
            if (roll != null) roll.Rolled -= Show;
            if (flow != null) flow.PhaseChanged -= OnPhase;
        }

        // Ra tran thi cat bang di — no che mat con duong.
        void OnPhase(BattleFlow.Phase p)
        {
            if (p != BattleFlow.Phase.Prep) SetVisible(false);
        }

        void Show(RollPayout payout)
        {
            if (payout?.Cells == null) return;

            // Dang quay do ma bam roll tiep: cat nhip cu di. Khong cat thi
            // hai coroutine cung ghi vao mot o va ket qua nhap nhay.
            if (spin != null) StopCoroutine(spin);
            SetVisible(true);
            spin = StartCoroutine(SpinThenReveal(payout));
        }

        IEnumerator SpinThenReveal(RollPayout payout)
        {
            var n = Mathf.Min(slots.Length, payout.Cells.Length);
            var kinds = (RollCellKind[])System.Enum.GetValues(typeof(RollCellKind));

            // Dat lai: moi o ve khung thuong, icon sang ro, chua biet ket qua.
            for (var i = 0; i < n; i++)
            {
                if (slots[i] != null) slots[i].sprite = slotNormal;
                if (icons[i] != null) icons[i].color = Color.white;
            }
            if (summary != null) summary.text = string.Empty;

            // Dung THOI GIAN THAT: nguoi choi bam x3 de tua nhanh TRAN DANH,
            // khong phai de nut bam nhay loan len.
            var stopAt = new float[n];
            for (var i = 0; i < n; i++) stopAt[i] = Time.unscaledTime + stagger * (i + 1);

            var done = 0;
            while (done < n)
            {
                for (var i = done; i < n; i++)
                {
                    if (icons[i] == null) continue;
                    icons[i].sprite = IconFor(kinds[Random.Range(0, kinds.Length)]);
                }
                yield return new WaitForSecondsRealtime(tick);

                while (done < n && Time.unscaledTime >= stopAt[done])
                {
                    Settle(payout, done);
                    done++;
                }
            }

            if (summary != null) summary.text = payout.SummaryLine;

            // Doc xong roi thi cat di — bang nam giua luoi, de nguyen la
            // nguoi choi khong nhin thay o nao trong de keo quan vao.
            //
            // Van la THOI GIAN THAT: bam x3 de tua tran danh, khong phai de
            // bang ket qua bien mat truoc khi kip doc.
            if (holdSeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(holdSeconds);

                // Bam roll tiep khi dang mo dan: Show() goi StopCoroutine nen
                // vong nay chet ngay giua chung, roi SetVisible(true) keo alpha
                // ve 1. Khong can tu kiem tra gi them.
                var t = 0f;
                while (t < fadeSeconds)
                {
                    t += Time.unscaledDeltaTime;
                    if (group != null)
                        group.alpha = Mathf.Clamp01(1f - t / fadeSeconds);
                    yield return null;
                }
                SetVisible(false);
            }

            spin = null;
        }

        /// <summary>Chốt một ô về kết quả thật.</summary>
        void Settle(RollPayout payout, int i)
        {
            if (slots[i] != null)
                slots[i].sprite = payout.CellWins[i] ? slotWin : slotNormal;

            if (icons[i] == null) return;
            icons[i].sprite = IconFor(payout.Cells[i]);

            // Ô không thắng thì mờ đi. Sáu ô cùng độ đậm thì mắt phải tự đếm
            // xem loại nào nhiều hơn — mà đó đúng là việc màu sắc phải làm thay.
            icons[i].color = new Color(1f, 1f, 1f, payout.CellWins[i] ? 1f : 0.45f);
        }

        Sprite IconFor(RollCellKind kind) => kind switch
        {
            RollCellKind.Coin => coinIcon,
            RollCellKind.Ally => allyIcon,
            _                 => buffIcon,
        };

        void SetVisible(bool on)
        {
            if (group == null) return;
            group.alpha = on ? 1f : 0f;
            // Tat luon kha nang nhan chuot, khong thi bang vo hinh van nuot
            // cu bam vao luoi phia sau.
            group.blocksRaycasts = false;
            group.interactable = false;
        }
    }
}
