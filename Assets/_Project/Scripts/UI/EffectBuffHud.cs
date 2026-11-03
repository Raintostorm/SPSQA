using System.Linq;
using Project.Battle;
using UnityEngine;
using UnityEngine.UI;
namespace Project.UI
{
    public sealed class EffectBuffHud:MonoBehaviour
    {
        EffectBuffSystem effects;GameObject root,detailPanel,overflowPanel,dismiss;RectTransform page;
        readonly Text[] overflowLevels=new Text[6];readonly Image[] overflowIcons=new Image[6];readonly RectTransform[] overflowSlots=new RectTransform[6];
        Text overflowCount;bool expanded;
#if UNITY_EDITOR
        EffectBuffKind[] preview;
        public void PreviewCount(int count){preview=Enumerable.Range(0,Mathf.Clamp(count,0,10)).Select(i=>(EffectBuffKind)i).ToArray();nextRefresh=0;ClosePanels();}
#endif
        EffectBuffKind[] Buffs(){
#if UNITY_EDITOR
            if(preview!=null)return preview;
#endif
            return effects.Active.OrderBy(k=>(int)k).ToArray();
        }
        int BuffLevel(EffectBuffKind kind)=>Mathf.Max(1,effects.Level(kind));
        Text message,detail;readonly Text[] levels=new Text[5];readonly Image[] icons=new Image[5];readonly RectTransform[] slots=new RectTransform[5];
        float detailUntil,nextRefresh;EffectBuffKind? inspected;
        public void Bind(EffectBuffSystem source){effects=source;}
        void Start()
        {
            root=new GameObject("ActiveEffectBuffs",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
            var canvas=root.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=140;
            var scaler=root.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1125,2436);
            page=new GameObject("SafeArea",typeof(RectTransform)).GetComponent<RectTransform>();page.SetParent(root.transform,false);
            var outside=Rect(page,"Dismiss",0,0,0,0);outside.anchorMin=Vector2.zero;outside.anchorMax=Vector2.one;outside.offsetMin=outside.offsetMax=Vector2.zero;
            var outsideImage=outside.gameObject.AddComponent<Image>();outsideImage.color=Color.clear;
            outside.gameObject.AddComponent<Button>().onClick.AddListener(ClosePanels);dismiss=outside.gameObject;dismiss.SetActive(false);
            message=Label(page,0,-585,1000,70,30);
            for(int i=0;i<5;i++)
            {
                int index=i;var r=Rect(page,"ActiveBuff",0,-430,128,128);slots[i]=r;
                var frame=r.gameObject.AddComponent<Image>();BattleUiSkin.Skin(frame,BattleUiSkin.Navy);
                var button=r.gameObject.AddComponent<Button>();button.targetGraphic=frame;ButtonFamilySkin.States(button);button.onClick.AddListener(()=>{if(index==4&&Buffs().Length>5){expanded=!expanded;HideDetails();RefreshOverflow();}else Inspect(index);});
                var art=Rect(r,"Icon",0,0,102,92);art.anchorMin=art.anchorMax=art.pivot=new Vector2(.5f,.5f);art.anchoredPosition=new Vector2(0,12);
                icons[i]=art.gameObject.AddComponent<Image>();icons[i].preserveAspect=true;icons[i].raycastTarget=false;
                levels[i]=Label(r,0,-88,120,36,30);levels[i].rectTransform.anchorMin=levels[i].rectTransform.anchorMax=new Vector2(.5f,1);
            }
            overflowCount=Label(slots[4],0,-24,120,80,54);
            var drop=Rect(page,"MoreBuffs",150,-580,440,168);overflowPanel=drop.gameObject;
            var dropImage=drop.gameObject.AddComponent<Image>();BattleUiSkin.Skin(dropImage,BattleUiSkin.Navy);
            for(int i=0;i<6;i++)
            {
                int index=i+4;var r=Rect(drop,"ExtraBuff",(i%3-1)*140,-20-(i/3)*144,128,128);overflowSlots[i]=r;
                var frame=r.gameObject.AddComponent<Image>();BattleUiSkin.Skin(frame,BattleUiSkin.Navy);
                var b=r.gameObject.AddComponent<Button>();b.targetGraphic=frame;ButtonFamilySkin.States(b);b.onClick.AddListener(()=>Inspect(index));
                var art=Rect(r,"Icon",0,-6,102,88);overflowIcons[i]=art.gameObject.AddComponent<Image>();overflowIcons[i].preserveAspect=true;overflowIcons[i].raycastTarget=false;
                overflowLevels[i]=Label(r,0,-88,120,36,30);
            }
            overflowPanel.SetActive(false);
            var panel=Rect(page,"BuffDetails",0,-680,1000,245);detailPanel=panel.gameObject;
            var background=panel.gameObject.AddComponent<Image>();BattleUiSkin.Skin(background,BattleUiSkin.Navy);
            detail=Label(panel,-20,-36,850,195,34);
            var close=Rect(panel,"Close",445,-16,64,64);var closeImage=close.gameObject.AddComponent<Image>();BattleUiSkin.Skin(closeImage,BattleUiSkin.Navy);
            var closeButton=close.gameObject.AddComponent<Button>();closeButton.targetGraphic=closeImage;closeButton.onClick.AddListener(HideDetails);
            Label(close,0,0,60,60,42).text="×";detailPanel.SetActive(false);
        }
        static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)
        {
            var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);
            BattleUiSkin.Place(r,new Vector2(.5f,1),new Vector2(x,y),new Vector2(w,h));return r;
        }
        static Text Label(Transform parent,float x,float y,float w,float h,int size)
        {
            var r=Rect(parent,"Label",x,y,w,h);var t=r.gameObject.AddComponent<Text>();
            t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontStyle=FontStyle.Bold;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.raycastTarget=false;
            var o=t.gameObject.AddComponent<Outline>();o.effectDistance=new Vector2(2,-2);return t;
        }
        void ClosePanels(){expanded=false;if(overflowPanel)overflowPanel.SetActive(false);HideDetails();}
        void HideDetails(){inspected=null;if(detailPanel)detailPanel.SetActive(false);if(dismiss)dismiss.SetActive(expanded);}
        void RefreshOverflow()
        {
            var buffs=Buffs();if(buffs.Length<=5)expanded=false;
            overflowPanel.SetActive(expanded);dismiss.SetActive(expanded||inspected.HasValue);
            int count=Mathf.Max(0,buffs.Length-4);
            int columns=Mathf.Min(3,Mathf.Max(1,count));float width=columns*140+20;
            var rect=(RectTransform)overflowPanel.transform;rect.sizeDelta=new Vector2(width,count>3?312:168);
            // Align the dropdown's right edge under the +N tile.
            rect.anchoredPosition=new Vector2(360-width*.5f,-580);
            for(int i=0;i<6;i++)
            {
                overflowSlots[i].gameObject.SetActive(i<count);
                overflowSlots[i].anchoredPosition=new Vector2((i%columns-(columns-1)*.5f)*140,-20-(i/columns)*144);
                if(i>=count)continue;
                overflowIcons[i].sprite=EffectBuffArt.Get(buffs[i+4]);overflowLevels[i].text="Lv."+BuffLevel(buffs[i+4]);
            }
            if(detailPanel) ((RectTransform)detailPanel.transform).anchoredPosition=new Vector2(0,expanded?(count>3?-915:-770):-680);
        }
        void Inspect(int index)
        {
            if(!effects)return;var buffs=Buffs();if(index>=buffs.Length)return;
            if(inspected==buffs[index]&&detailPanel.activeSelf){HideDetails();return;}
            inspected=buffs[index];RefreshDetails();detailPanel.SetActive(true);detailUntil=Time.unscaledTime+8;dismiss.SetActive(true);
        }
        void RefreshDetails()
        {
            if(!inspected.HasValue)return;var kind=inspected.Value;
            var def=EffectBuffCatalog.All[(int)kind];detail.text=def.Name+" · Lv."+BuffLevel(kind)+"\n"+effects.Describe(kind);
        }
        void Update()
        {
            if(!effects||!page)return;var a=Screen.safeArea;
            page.anchorMin=new Vector2(a.xMin/Screen.width,a.yMin/Screen.height);page.anchorMax=new Vector2(a.xMax/Screen.width,a.yMax/Screen.height);page.offsetMin=page.offsetMax=Vector2.zero;
            if(inspected.HasValue&&Time.unscaledTime>detailUntil)HideDetails();
            if(Time.unscaledTime<nextRefresh)return;nextRefresh=Time.unscaledTime+.15f;
            var buffs=Buffs();int visible=Mathf.Min(5,buffs.Length);
            for(int i=0;i<icons.Length;i++)
            {
                slots[i].gameObject.SetActive(i<visible);
                if(i>=visible)continue;
                slots[i].anchoredPosition=new Vector2((i-(visible-1)*.5f)*148,-430);
                bool more=i==4&&buffs.Length>5;icons[i].enabled=!more;levels[i].enabled=!more;
                icons[i].sprite=EffectBuffArt.Get(buffs[i]);levels[i].text="Lv."+BuffLevel(buffs[i]);
                if(i==4){overflowCount.gameObject.SetActive(more);overflowCount.text="+"+(buffs.Length-4);BattleUiSkin.Skin(slots[i].GetComponent<Image>(),expanded?BattleUiSkin.Blue:BattleUiSkin.Navy);}
            }
            RefreshOverflow();
            message.text=(effects.PendingAllies+effects.PendingItems==0?"":$"Waiting: {effects.PendingAllies} allies · {effects.PendingItems} items")+(effects.RescueArmed?$"  Second Wind ready · Wave {effects.RescueWave}":"");
            if(inspected.HasValue){if(!buffs.Contains(inspected.Value))HideDetails();else RefreshDetails();}
        }
        void OnDestroy(){if(root)Destroy(root);}
    }
}
