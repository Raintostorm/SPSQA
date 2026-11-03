using System.Collections.Generic;
using Project.Battle;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Read-only live ranking. One spawned ally per row; share uses the whole ally total.
    public sealed class StatPanel : MonoBehaviour
    {
        public enum Metric { Dealt, Taken, Healing }
        [System.Serializable] public sealed class Row
        {
            public GameObject Root;
            public Text Name;
            public Image Bar;
            public Text Value;
            [System.NonSerialized] public Image Portrait;
        }
        // Keep builder/scene serialization compatible; runtime presentation is rebuilt below.
        [SerializeField] CanvasGroup group;
        [SerializeField] BattleFlow flow;
        [SerializeField] Button[] tabs;
        [SerializeField] Text allyHeader,enemyHeader;
        [SerializeField] Row[] allyRows,enemyRows;
        [SerializeField] float refreshInterval=.25f;
        [SerializeField] Color allyBar=new Color(.05f,.8f,1),enemyBar=new Color(1,.35f,.4f);
        sealed class LiveRow { public RectTransform Root; public Image Portrait,Bar; public Text Rank,Name,Value,Share; }
        readonly List<LiveRow> rows=new List<LiveRow>();
        readonly List<Unit> units=new List<Unit>();
        readonly List<Button> metricButtons=new List<Button>();
        RectTransform content;
        Text totalLabel,healthLabel;
        Font font;
        Metric metric;
        float timer;
        static readonly string[] Titles={"Damage Dealt","Damage Taken"};
        void Awake(){if(!flow)flow=FindFirstObjectByType<BattleFlow>();}
        void OnEnable(){if(flow)flow.PhaseChanged+=OnPhase;OnPhase(flow?flow.Current:BattleFlow.Phase.Prep);}
        void OnDisable(){if(flow)flow.PhaseChanged-=OnPhase;}
        void Start()
        {
            font=allyHeader?allyHeader.font:GetComponentInChildren<Text>(true).font;
            foreach(Transform child in transform)child.gameObject.SetActive(false);
            BattleUiSkin.Place((RectTransform)transform,new Vector2(.5f,0),new Vector2(0,40),new Vector2(1060,620));
            var body=Box(transform,"LiveRanking",Vector2.zero,new Vector2(1060,620),BattleUiSkin.Navy);
            for(int i=0;i<2;i++)
            {
                int selected=i;
                var r=Box(body,"Metric",new Vector2(i==0?-260:260,250),new Vector2(505,100),BattleUiSkin.Navy);
                r.GetComponent<Image>().raycastTarget=true;
                var button=r.gameObject.AddComponent<Button>();ButtonFamilySkin.States(button);button.onClick.AddListener(()=>Select((Metric)selected));
                Text(r,Titles[i],Vector2.zero,new Vector2(490,85),34);metricButtons.Add(button);
            }
            healthLabel=Text(body,"",new Vector2(-160,166),new Vector2(680,55),27);healthLabel.alignment=TextAnchor.MiddleLeft;
            totalLabel=Text(body,"",new Vector2(340,166),new Vector2(330,55),30);totalLabel.alignment=TextAnchor.MiddleRight;
            var viewport=Rect(body,"Viewport",new Vector2(0,-83),new Vector2(1010,420));viewport.gameObject.AddComponent<RectMask2D>();
            var hit=viewport.gameObject.AddComponent<Image>();hit.color=new Color(0,0,0,.01f);
            var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.viewport=viewport;scroll.horizontal=false;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=40;
            content=Rect(viewport,"Rows",Vector2.zero,new Vector2(1000,420));content.anchorMin=content.anchorMax=content.pivot=new Vector2(.5f,1);scroll.content=content;
            Highlight();Refresh();OnPhase(flow?flow.Current:BattleFlow.Phase.Prep);
        }
        void OnPhase(BattleFlow.Phase phase)
        {
            bool show=phase==BattleFlow.Phase.Marching||phase==BattleFlow.Phase.Fighting;
            if(group){group.alpha=show?1:0;group.blocksRaycasts=show;}
            if(show)Refresh();
        }
        void Update()
        {
            if(!content||group&&group.alpha<=0)return;
            timer-=Time.unscaledDeltaTime;if(timer>0)return;timer=refreshInterval;Refresh();
        }
        public void Select(Metric value){if(value==Metric.Healing)return;metric=value;Highlight();Refresh();}
        void Highlight(){for(int i=0;i<metricButtons.Count;i++)BattleUiSkin.Skin(metricButtons[i].GetComponent<Image>(),i==(int)metric?BattleUiSkin.Blue:BattleUiSkin.Dark);}
        float Value(Unit u)=>metric==Metric.Taken?u.DamageTaken:u.DamageDealt;
        void Refresh()
        {
            if(!content)return;
            units.Clear();float allyHp=0,enemyHp=0,total=0;
            foreach(var u in UnitRegistry.All)if(u)
            {
                if(u.Team==UnitTeam.Ally){units.Add(u);allyHp+=Mathf.Max(0,u.CurrentHp);total+=Mathf.Max(0,Value(u));}
                else enemyHp+=Mathf.Max(0,u.CurrentHp);
            }
            units.Sort((a,b)=>{int c=Value(b).CompareTo(Value(a));return c!=0?c:a.GetInstanceID().CompareTo(b.GetInstanceID());});
            healthLabel.text=$"HP  Allies {Mathf.CeilToInt(allyHp):N0} · Enemies {Mathf.CeilToInt(enemyHp):N0}";
            totalLabel.text=$"Total {total:N0}";
            while(rows.Count<units.Count)rows.Add(CreateRow());
            content.sizeDelta=new Vector2(1000,Mathf.Max(420,units.Count*140));
            for(int i=0;i<rows.Count;i++)
            {
                var row=rows[i];row.Root.gameObject.SetActive(i<units.Count);if(i>=units.Count)continue;
                var unit=units[i];float value=Mathf.Max(0,Value(unit)),share=total>0?value/total:0;
                row.Root.anchoredPosition=new Vector2(0,-70-i*140);
                row.Rank.text=(i+1).ToString();row.Name.text=unit.Definition?unit.Definition.DisplayName:unit.name;
                row.Portrait.sprite=unit.Definition?(unit.Definition.Portrait?unit.Definition.Portrait:unit.Definition.Animations?.Idle?.FrameAt(0)):null;row.Portrait.enabled=row.Portrait.sprite;
                row.Value.text=value.ToString("N0");row.Share.text=(share*100).ToString("0.0")+"%";
                row.Bar.fillAmount=share;row.Bar.color=metric==Metric.Taken?enemyBar:allyBar;
            }
        }
        LiveRow CreateRow()
        {
            var row=new LiveRow();row.Root=Box(content,"AllyRow",Vector2.zero,new Vector2(1000,130),BattleUiSkin.Dark);row.Root.anchorMin=row.Root.anchorMax=new Vector2(.5f,1);
            row.Rank=Text(row.Root,"",new Vector2(-460,0),new Vector2(50,70),30);
            row.Portrait=Rect(row.Root,"Portrait",new Vector2(-375,0),new Vector2(100,115)).gameObject.AddComponent<Image>();row.Portrait.preserveAspect=true;row.Portrait.raycastTarget=false;
            row.Name=Text(row.Root,"",new Vector2(80,34),new Vector2(750,48),32);row.Name.alignment=TextAnchor.MiddleLeft;
            row.Value=Text(row.Root,"",new Vector2(-210,-16),new Vector2(170,50),38);row.Value.color=new Color(1,.86f,.3f);
            row.Share=Text(row.Root,"",new Vector2(400,-16),new Vector2(170,50),32);
            var track=Box(row.Root,"Track",new Vector2(85,-49),new Vector2(750,16),BattleUiSkin.Dark);track.GetComponent<Image>().color=new Color(.25f,.34f,.45f);
            row.Bar=Rect(track,"Fill",Vector2.zero,new Vector2(750,16)).gameObject.AddComponent<Image>();row.Bar.sprite=BattleUiSkin.Neutral;row.Bar.type=Image.Type.Filled;row.Bar.fillMethod=Image.FillMethod.Horizontal;row.Bar.raycastTarget=false;
            return row;
        }
        RectTransform Rect(Transform parent,string name,Vector2 pos,Vector2 size)
        {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);BattleUiSkin.Place(r,Vector2.one*.5f,pos,size);return r;}
        RectTransform Box(Transform parent,string name,Vector2 pos,Vector2 size,Sprite sprite)
        {var r=Rect(parent,name,pos,size);var image=r.gameObject.AddComponent<Image>();BattleUiSkin.Skin(image,sprite);image.raycastTarget=false;return r;}
        Text Text(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize)
        {var t=Rect(parent,"Label",pos,size).gameObject.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=fontSize;t.fontStyle=FontStyle.Bold;t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;return t;}
    }
}
