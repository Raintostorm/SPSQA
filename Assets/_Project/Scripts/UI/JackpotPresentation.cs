using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>Visual celebration only. Payout is still committed once by RollService.</summary>
    public sealed class JackpotPresentation : MonoBehaviour
    {
        public const float Duration=2f;
        RectTransform banner;
        CanvasGroup group;
        RectTransform castle;
        Image[] bursts;
        Sprite[] frames;
        float elapsed;
        Project.Battle.BattleFlow flow;
        public static void Show(Component source)
        {
            var canvas=source.GetComponentInParent<Canvas>();if(!canvas)return;
            var go=new GameObject("JackpotCelebration",typeof(RectTransform),typeof(CanvasGroup));
            go.transform.SetParent(canvas.transform,false);
            var fx=go.AddComponent<JackpotPresentation>();fx.group=go.GetComponent<CanvasGroup>();
            fx.group.blocksRaycasts=false;fx.group.interactable=false;
            var root=go.GetComponent<RectTransform>();root.anchorMin=Vector2.zero;root.anchorMax=Vector2.one;root.offsetMin=root.offsetMax=Vector2.zero;
            fx.banner=new GameObject("JackpotBanner",typeof(RectTransform),typeof(Image)).GetComponent<RectTransform>();
            fx.banner.SetParent(root,false);BattleUiSkin.Place(fx.banner,Vector2.one*.5f,new Vector2(0,-140),new Vector2(920,220));
            // Anchor the approved celebration to the reels instead of screen centre.
            root.position=source.transform.position;
            BattleUiSkin.Place(fx.banner,Vector2.one*.5f,new Vector2(0,100),new Vector2(970,220));
            var ribbon=fx.banner.GetComponent<Image>();ribbon.sprite=Resources.Load<Sprite>("Jackpot/ribbon");ribbon.preserveAspect=true;ribbon.raycastTarget=false;ribbon.enabled=ribbon.sprite!=null;
            fx.frames=new Sprite[6];for(int i=0;i<6;i++)fx.frames[i]=Resources.Load<Sprite>("Jackpot/fireworks_"+i);
            fx.bursts=new Image[3];
            for(int i=0;i<3;i++)fx.bursts[i]=Art(root,"Fireworks",fx.frames[0],new Vector2((i-1)*330,260+i%2*110),Vector2.one*420);
            fx.castle=Art(root,"GoldenCastle",Resources.Load<Sprite>("Jackpot/castle"),new Vector2(0,280),new Vector2(360,310)).rectTransform;
            fx.banner.SetAsLastSibling();
            var label=new GameObject("JackpotTitle",typeof(RectTransform),typeof(Text),typeof(Outline)).GetComponent<Text>();
            label.transform.SetParent(fx.banner,false);label.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text="JACKPOT!";label.fontSize=82;label.fontStyle=FontStyle.Bold;label.alignment=TextAnchor.MiddleCenter;label.color=new Color(1,.96f,.77f);label.raycastTarget=false;
            label.GetComponent<Outline>().effectColor=new Color(.15f,.04f,.2f);label.GetComponent<Outline>().effectDistance=new Vector2(3,-3);
            BattleUiSkin.Place(label.rectTransform,Vector2.one*.5f,Vector2.zero,new Vector2(870,200));
            fx.flow=FindFirstObjectByType<Project.Battle.BattleFlow>();if(fx.flow)fx.flow.RunRestarted+=fx.Cancel;
            fx.Draw();
        }
        static Image Art(Transform parent,string name,Sprite sprite,Vector2 position,Vector2 size)
        {
            var image=new GameObject(name,typeof(RectTransform),typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(parent,false);image.sprite=sprite;image.preserveAspect=true;image.raycastTarget=false;image.enabled=sprite!=null;
            BattleUiSkin.Place(image.rectTransform,Vector2.one*.5f,position,size);return image;
        }
        void Cancel(){gameObject.SetActive(false);Destroy(gameObject);}
        void OnDestroy(){if(flow)flow.RunRestarted-=Cancel;}
        void Update(){if(SharedSettings.IsPaused)return;elapsed+=Time.deltaTime;if(elapsed>=Duration){Destroy(gameObject);return;}Draw();}
        void Draw()
        {
            var pop=Mathf.Clamp01(elapsed/.32f);banner.localScale=Vector3.one*Mathf.Lerp(.85f,1,1-Mathf.Pow(1-pop,3));
            group.alpha=Mathf.Clamp01(elapsed/.15f)*Mathf.Clamp01((Duration-elapsed)/.3f);
            float rise=1-Mathf.Pow(1-Mathf.Clamp01(elapsed/.32f),3);
            castle.anchoredPosition=new Vector2(0,280-(1-rise)*55);
            castle.localScale=Vector3.one*Mathf.Lerp(.8f,1,rise);
            for(int i=0;i<bursts.Length;i++)
            {
                int frame=Mathf.FloorToInt((elapsed-i*.12f)/.12f);
                bursts[i].enabled=frame>=0&&frame<6&&frames[Mathf.Clamp(frame,0,5)]!=null;
                if(frame>=0&&frame<6)bursts[i].sprite=frames[frame];
            }
        }
    }
}
