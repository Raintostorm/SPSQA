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
        Button infoButton;
        bool showInfo;
        RectTransform floating;
        GameObject infoCard;

        void Awake()
        {
            rng = new System.Random();
            if (sellButton != null) sellButton.onClick.AddListener(DoSell);
            if (evolveButton != null) evolveButton.onClick.AddListener(DoEvolve);
            BuildCompactActions();
            Close();
        }

        void BuildCompactActions()
        {
            floating = group != null ? group.GetComponent<RectTransform>() : GetComponent<RectTransform>();
            if (!floating) return;
            foreach (var image in floating.GetComponentsInChildren<Image>(true))
                if (image.transform == floating ||
                    (title && image.transform == title.transform.parent))
                { image.color = Color.clear; image.raycastTarget = false; }
            BattleUiSkin.Place(floating, Vector2.one * .5f, Vector2.zero, new Vector2(420,400));
            if (sellButton)
            {
                BattleUiSkin.Place(sellButton.GetComponent<RectTransform>(),Vector2.one*.5f,new Vector2(0,130),new Vector2(270,100));
                BattleUiSkin.Button(sellButton,BattleUiSkin.Red,"+0");ScreenChrome.Action(sellButton,ScreenChrome.Role.Danger);
                BattleUiSkin.Icon(sellButton,"HomeChrome/coin");
            }
            if (evolveButton)
            {
                BattleUiSkin.Place(evolveButton.GetComponent<RectTransform>(),Vector2.one*.5f,new Vector2(0,-120),new Vector2(270,100));
                BattleUiSkin.Button(evolveButton,BattleUiSkin.Orange,"Merge");ScreenChrome.Action(evolveButton,ScreenChrome.Role.Primary);
                BattleUiSkin.Icon(evolveButton,"upgrade");
            }
            var go = new GameObject("UnitInfoButton",typeof(RectTransform),typeof(Image),typeof(Button));
            go.transform.SetParent(floating,false);infoButton=go.GetComponent<Button>();
            BattleUiSkin.Place(go.GetComponent<RectTransform>(),Vector2.one*.5f,new Vector2(195,0),new Vector2(90,90));
            ApprovedUiArt.Apply(go.GetComponent<Image>(),"info");
            var label=new GameObject("Label",typeof(RectTransform),typeof(Text)).GetComponent<Text>();
            label.transform.SetParent(go.transform,false);label.font=title ? title.font : UiFonts.Display;
            label.fontSize=48;label.text="";label.alignment=TextAnchor.MiddleCenter;label.color=Color.white;label.raycastTarget=false;
            BattleUiSkin.Place(label.rectTransform,Vector2.one*.5f,Vector2.zero,new Vector2(80,80));
            infoButton.onClick.AddListener(()=>{var units=board?.UnitsAt(row,col);if(units!=null&&units.Count>0)BattleUnitInfo.Show(units[0]);});
            infoCard=new GameObject("UnitInfoCard",typeof(RectTransform),typeof(Image));
            infoCard.transform.SetParent(floating,false);
            BattleUiSkin.Place(infoCard.GetComponent<RectTransform>(),Vector2.one*.5f,new Vector2(0,255),new Vector2(570,230));
            BattleUiSkin.Skin(infoCard.GetComponent<Image>(),BattleUiSkin.Navy);
            infoCard.GetComponent<Image>().raycastTarget=false;
            if(title){title.transform.SetParent(infoCard.transform,false);BattleUiSkin.Place(title.rectTransform,Vector2.one*.5f,new Vector2(0,65),new Vector2(530,60));}
            if(stats){stats.transform.SetParent(infoCard.transform,false);BattleUiSkin.Place(stats.rectTransform,Vector2.one*.5f,new Vector2(0,-30),new Vector2(530,120));}
        }
        void SetInfoVisible()
        {if(infoCard)infoCard.SetActive(showInfo);if(title)title.gameObject.SetActive(showInfo);if(stats)stats.gameObject.SetActive(showInfo);}
        void LateUpdate()
        {
            if (!board || !floating || !group || group.alpha == 0) return;
            var camera=Camera.main;var parent=floating.parent as RectTransform;
            if (!camera || !parent) return;
            var units=board.UnitsAt(row,col);
            var centre=Vector3.zero;int count=0;
            foreach(var unit in units)if(unit){centre+=unit.transform.position;count++;}
            if(count==0){Close();return;}
            centre/=count;centre+=Vector3.up*.2f;
            var screen=camera.WorldToScreenPoint(centre);
            var canvas=floating.GetComponentInParent<Canvas>();
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,screen,
                canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null,out point);
            point.x=Mathf.Clamp(point.x,parent.rect.xMin+145,parent.rect.xMax-145);
            point.y=Mathf.Clamp(point.y,parent.rect.yMin+250,parent.rect.yMax-360);
            if(infoButton)infoButton.GetComponent<RectTransform>().anchoredPosition=
                new Vector2(point.x+245>parent.rect.xMax?-195:195,0);
            if(infoCard)infoCard.GetComponent<RectTransform>().anchoredPosition=
                new Vector2(Mathf.Clamp(point.x,parent.rect.xMin+290,parent.rect.xMax-290)-point.x,255);
            floating.anchoredPosition=point;
        }

        public void Open(BattleBoard b, int r, int c)
        {
            // Tat o dang sang o BAN CU truoc khi doi ban. Bam o phe ta roi
            // bam sang o phe dich ma khong co dong nay thi ca hai cung sang.
            Highlight(null);
            board = b; row = r; col = c;
            showInfo=false;SetInfoVisible();
            Refresh();
        }

        public void Close()
        {
            Project.Visual.PrepUnitRing.Select(null);
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
            Project.Visual.PrepUnitRing.Select(units);

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
                    (def.Mythic!=MythicKind.None ? $"Mythic · {(def.IsRanged ? "Ranged" : "Melee")}\n" : $"{def.Race} · {def.Rank} · {(def.IsRanged ? "Ranged" : "Melee")}\n") +
                    $"HP {u.MaxHp:0}      ATTACK {u.AttackPower:0}\n" +
                    $"RANGE {def.AttackRange:0.0}   ATK SPD {u.GetComponent<UnitBrain>().AttacksPerSecond:0.##}/s";
                int gearCount=u.Equipment.Types.Count;
                foreach(int type in u.Equipment.Types)
                    stats.text+="\n<color=#99FFBF>"+BattleItemRecipes.Items[type].Name+" · "+BattleItemRecipes.Effect(type)+"</color>";
                if(infoCard)infoCard.GetComponent<RectTransform>().sizeDelta=new Vector2(570,230+gearCount*65);
                stats.rectTransform.sizeDelta=new Vector2(530,120+gearCount*65);
                if(title)title.rectTransform.anchoredPosition=new Vector2(0,65+gearCount*32.5f);
            }

            // Phe địch: chỉ xem, không bán không tiến hoá.
            if (sellButton != null) sellButton.gameObject.SetActive(ally);
            if (evolveButton != null) evolveButton.gameObject.SetActive(ally &&
                EvolutionSystem.CanEvolve(board, row, col, pool, out _, out _));

            if (ally)
            {
                var price = SellPrice(units);
                if (sellLabel != null) sellLabel.text = $"+{price}";
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
