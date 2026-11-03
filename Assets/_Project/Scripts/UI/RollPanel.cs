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
        public bool IsSpinning { get; private set; }
        RollPayout shownPayout;
        public Vector3 RewardOrigin(Camera camera, float worldZ)
        {
            Transform source = transform;
            if (shownPayout != null)
                for (var i = 0; i < shownPayout.Cells.Length && i < icons.Length; i++)
                    if (shownPayout.Cells[i] == RollCellKind.Ally && icons[i]) { source = icons[i].transform; break; }
            var canvas = GetComponentInParent<Canvas>();
            var screen = RectTransformUtility.WorldToScreenPoint(canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null, source.position);
            return camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(camera.transform.position.z - worldZ)));
        }
        readonly Image[] next = new Image[6];

        void Start()
        {
            // Results sit below owned Mythics and above the gold/dock controls.
            var root=(RectTransform)transform;
            BattleUiSkin.Place(root,new Vector2(.5f,0),new Vector2(0,380),new Vector2(970,150));
            root.pivot=new Vector2(.5f,0);
            for(int i=0;i<slots.Length;i++)
            {
                if(!slots[i] || !icons[i])continue;
                BattleUiSkin.Place(slots[i].rectTransform,new Vector2(0,1),new Vector2(i*164,0),new Vector2(150,140));
                BattleUiSkin.Skin(slots[i],BattleUiSkin.Navy);
                var viewport=new GameObject("ReelViewport",typeof(RectTransform),typeof(RectMask2D));
                viewport.transform.SetParent(slots[i].transform,false);
                BattleUiSkin.Place(viewport.GetComponent<RectTransform>(),Vector2.one*.5f,Vector2.zero,new Vector2(122,112));
                icons[i].transform.SetParent(viewport.transform,false);
                BattleUiSkin.Place(icons[i].rectTransform,Vector2.one*.5f,Vector2.zero,new Vector2(94,94));
                icons[i].preserveAspect=true;
                var extra=new GameObject("Next",typeof(RectTransform),typeof(Image));extra.transform.SetParent(viewport.transform,false);
                next[i]=extra.GetComponent<Image>();next[i].preserveAspect=true;next[i].raycastTarget=false;
                BattleUiSkin.Place(next[i].rectTransform,Vector2.one*.5f,new Vector2(0,112),new Vector2(94,94));
            }
            // Slot icons and winning frames already communicate the outcome.
            if(summary)summary.transform.parent.gameObject.SetActive(false);
        }

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
            Cancel();
            if (roll != null) roll.Rolled -= Show;
            if (flow != null) flow.PhaseChanged -= OnPhase;
        }

        // Ra tran thi cat bang di — no che mat con duong.
        void OnPhase(BattleFlow.Phase p)
        {
            if (p != BattleFlow.Phase.Prep) Cancel();
        }

        void Cancel(){if(spin!=null)StopCoroutine(spin);spin=null;IsSpinning=false;if(roll)roll.FlushPresentation();SetVisible(false);}

        void Show(RollPayout payout)
        {
            if (payout?.Cells == null) return;
            shownPayout = payout;

            // Dang quay do ma bam roll tiep: cat nhip cu di. Khong cat thi
            // hai coroutine cung ghi vao mot o va ket qua nhap nhay.
            if (spin != null) StopCoroutine(spin);
            SetVisible(true);
            spin = StartCoroutine(SpinThenReveal(payout));
        }

        IEnumerator SpinThenReveal(RollPayout payout)
        {
            IsSpinning=true;
            var n = Mathf.Min(slots.Length, payout.Cells.Length);
            var kinds = (RollCellKind[])System.Enum.GetValues(typeof(RollCellKind));

            // Dat lai: moi o ve khung thuong, icon sang ro, chua biet ket qua.
            for (var i = 0; i < n; i++)
            {
                if (slots[i] != null) BattleUiSkin.Skin(slots[i],BattleUiSkin.Navy);
                if (icons[i] != null) icons[i].color = Color.white;
            }
            if (summary != null) summary.text = string.Empty;

            // Reel and delivery follow the same x1/x2/x3 game clock.
            var stopAt = new float[n];
            for (var i = 0; i < n; i++) stopAt[i] = 1.1f + .17f * i;
            float elapsed = 0;

            var done = 0;
            while (done < n)
            {
                if(SharedSettings.IsPaused){yield return null;continue;}
                elapsed+=Time.deltaTime;
                for (var i = done; i < n; i++)
                {
                    if (icons[i] == null) continue;
                    float t=Mathf.Clamp01(elapsed/stopAt[i]);
                    float position=(1-Mathf.Pow(1-t,3))*12;
                    int step=Mathf.FloorToInt(position);
                    float offset=(position-step)*112;
                    icons[i].sprite=IconFor(kinds[(step+i)%kinds.Length]);
                    icons[i].rectTransform.anchoredPosition=new Vector2(0,-offset);
                    if(next[i]){next[i].sprite=step>=11?IconFor(payout.Cells[i]):IconFor(kinds[(step+i+1)%kinds.Length]);next[i].color=Color.white;next[i].rectTransform.anchoredPosition=new Vector2(0,112-offset);}
                }

                while (done < n && elapsed >= stopAt[done])
                {
                    Settle(payout, done);
                    done++;
                }
                yield return null;
            }

            if(payout.Jackpot)
            {
                JackpotPresentation.Show(this);
                yield return WaitPresentation(JackpotPresentation.Duration);
            }
            roll.CommitPayout(payout);
            RollRewardArt.Present(payout,RectTransformUtility.WorldToScreenPoint(null,transform.position));
            var placer = FindFirstObjectByType<Project.Gacha.RollRewardPlacer>();
            while (placer && placer.IsDelivering) yield return null;
            // Let coin/buff arrivals finish before unlocking the next action.
            yield return WaitPresentation(.85f);
            roll.FinishPresentation();
            IsSpinning=false;

            if (summary != null) summary.text = payout.SummaryLine;

            // Doc xong roi thi cat di — bang nam giua luoi, de nguyen la
            // nguoi choi khong nhin thay o nao trong de keo quan vao.
            //
            // Result hold/fade follows the selected game speed too.
            if (holdSeconds > 0f)
            {
                yield return WaitPresentation(holdSeconds);

                // Bam roll tiep khi dang mo dan: Show() goi StopCoroutine nen
                // vong nay chet ngay giua chung, roi SetVisible(true) keo alpha
                // ve 1. Khong can tu kiem tra gi them.
                var t = 0f;
                while (t < fadeSeconds)
                {
                    if (SharedSettings.IsPaused) { yield return null; continue; }
                    t += Time.deltaTime;
                    if (group != null)
                        group.alpha = Mathf.Clamp01(1f - t / fadeSeconds);
                    yield return null;
                }
                SetVisible(false);
            }

            spin = null;
        }

        System.Collections.IEnumerator WaitPresentation(float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                yield return null;
                if (!SharedSettings.IsPaused) elapsed += Time.deltaTime;
            }
        }

        /// <summary>Chốt một ô về kết quả thật.</summary>
        void Settle(RollPayout payout, int i)
        {
            if(payout.CellWins[i] && slots[i])RollRewardArt.Flash(slots[i]);
            if (slots[i] != null)
                BattleUiSkin.Skin(slots[i],payout.CellWins[i] ? BattleUiSkin.Orange : BattleUiSkin.Navy);

            if (icons[i] == null) return;
            icons[i].sprite = IconFor(payout.Cells[i]);
            icons[i].rectTransform.anchoredPosition=Vector2.zero;
            if(next[i])next[i].color=Color.clear;

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
