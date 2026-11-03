using System;
using Project.Gacha;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>Presentation only: never grants coins, buffs or units.</summary>
    public sealed class RollRewardArt : MonoBehaviour
    {
        Image art;
        Sprite[] frames;
        RectTransform target, space;
        Vector2 start;
        float elapsed, delay, seconds;
        bool flash;
        bool coinAmount;
        bool buffAmount;
        Text amountLabel;
        int fixedFrame = -1;
        Color tint = Color.white;
        Project.Battle.BattleFlow flow;
        void OnEnable()
        {
            flow=FindFirstObjectByType<Project.Battle.BattleFlow>();
            if(flow)flow.RunRestarted+=Cancel;
        }
        void OnDisable(){if(flow)flow.RunRestarted-=Cancel;}
        void Cancel(){gameObject.SetActive(false);Destroy(gameObject);}
        public static void Flash(Image slot)
        {
            if (!slot) return;
            var fx=Create(slot.rectTransform,"slot-flash");
            fx.flash=true;fx.seconds=.3f;
            fx.art.rectTransform.anchorMin=Vector2.zero;fx.art.rectTransform.anchorMax=Vector2.one;
            fx.art.rectTransform.offsetMin=Vector2.one*-10;fx.art.rectTransform.offsetMax=Vector2.one*10;
        }
        static RollRewardArt Create(RectTransform parent,string sheet)
        {
            var go=new GameObject("RollRewardArt "+sheet,typeof(RectTransform),typeof(Image));
            go.transform.SetParent(parent,false);
            var fx=go.AddComponent<RollRewardArt>();fx.art=go.GetComponent<Image>();
            fx.art.raycastTarget=false;fx.art.preserveAspect=true;
            fx.frames=Resources.LoadAll<Sprite>("RollRewards/"+sheet);
            Array.Sort(fx.frames,(a,b)=>string.CompareOrdinal(a.name,b.name));
            if(fx.frames.Length>0)fx.art.sprite=fx.frames[0];
            return fx;
        }
        public static void Present(RollPayout payout,Vector3 sourceScreen)
        {
            var hud=FindFirstObjectByType<BattleHud>();if(!hud)return;
            if(payout.CoinAwarded>0)
                ShowCoinAmount(hud.RewardTarget(-1),payout.CoinAwarded);
            foreach(var buff in payout.BuffAdds)
            {
                var name = buff.Stat switch {
                    BuffStatKind.HpPct => "HP", BuffStatKind.DmgPct => "Attack",
                    BuffStatKind.AtkSpeedPct => "Attack Speed", _ => "Crit Chance" };
                Fly(hud.RewardTarget((int)buff.Stat),sourceScreen,"buff",(int)buff.Stat,0,
                    name + " +" + (buff.Value*100f).ToString("0.#") + "%");
            }
        }
        static void ShowCoinAmount(RectTransform destination,int amount)
        {
            if(!destination)return;
            var canvas=destination.GetComponentInParent<Canvas>();if(!canvas)return;
            var parent=canvas.transform as RectTransform;
            var fx=Create(parent,"coin");fx.space=parent;fx.target=destination;
            fx.coinAmount=true;fx.seconds=.7f;
            fx.start=(Vector2)parent.InverseTransformPoint(destination.position)+Vector2.up*110;
            var rt=fx.art.rectTransform;rt.anchorMin=rt.anchorMax=Vector2.one*.5f;
            rt.sizeDelta=Vector2.one*55;rt.anchoredPosition=fx.start;
            fx.amountLabel=new GameObject("CoinAmount",typeof(RectTransform),typeof(Text)).GetComponent<Text>();
            fx.amountLabel.transform.SetParent(rt,false);
            var source=destination.GetComponent<Text>();
            fx.amountLabel.font=source?source.font:Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            fx.amountLabel.fontSize=38;fx.amountLabel.fontStyle=FontStyle.Bold;fx.amountLabel.color=new Color(1,.86f,.25f);
            fx.amountLabel.alignment=TextAnchor.MiddleLeft;fx.amountLabel.text="+"+amount;fx.amountLabel.raycastTarget=false;
            BattleUiSkin.Place(fx.amountLabel.rectTransform,Vector2.one*.5f,new Vector2(32,0),new Vector2(180,65));
        }
        static void Fly(RectTransform destination,Vector3 source,string sheet,int frame,float delay,string caption=null)
        {
            if(!destination)return;
            var canvas=destination.GetComponentInParent<Canvas>();if(!canvas)return;
            var parent=canvas.transform as RectTransform;
            var fx=Create(parent,sheet);fx.space=parent;fx.target=destination;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,source,
                canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera,out fx.start);
            fx.fixedFrame=frame;fx.delay=delay;fx.seconds=.55f;
            fx.art.rectTransform.anchorMin=fx.art.rectTransform.anchorMax=Vector2.one*.5f;
            fx.art.rectTransform.sizeDelta=Vector2.one*(sheet=="coin"?70:95);
            fx.art.rectTransform.anchoredPosition=fx.start;fx.art.enabled=false;
            if(caption!=null)
            {
                fx.buffAmount=true;fx.seconds=.85f;
                // Reveal the actual resolved buff above the reel before delivery.
                fx.start+=Vector2.up*150;
                fx.amountLabel=new GameObject("BuffAmount",typeof(RectTransform),typeof(Text),typeof(Outline)).GetComponent<Text>();
                fx.amountLabel.transform.SetParent(fx.art.transform,false);
                var label=destination.GetComponent<Text>();
                fx.amountLabel.font=label?label.font:Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                fx.amountLabel.fontSize=38;fx.amountLabel.fontStyle=FontStyle.Bold;
                fx.amountLabel.alignment=TextAnchor.MiddleLeft;fx.amountLabel.text=caption;
                fx.amountLabel.color=new Color(.3f,1,.45f);fx.amountLabel.raycastTarget=false;
                fx.amountLabel.GetComponent<Outline>().effectColor=Color.black;
                BattleUiSkin.Place(fx.amountLabel.rectTransform,Vector2.one*.5f,new Vector2(48,0),new Vector2(420,85));
            }
        }
        void Update()
        {
            if(SharedSettings.IsPaused)return;
            elapsed+=Time.deltaTime;
            if(frames.Length==0||elapsed>=delay+seconds){Destroy(gameObject);return;}
            if(elapsed<delay)return;
            art.enabled=true;float t=Mathf.Clamp01((elapsed-delay)/seconds);
            if(coinAmount)
            {
                if(!target){Destroy(gameObject);return;}
                var end=(Vector2)space.InverseTransformPoint(target.position);
                var move=Mathf.SmoothStep(0,1,Mathf.Clamp01((elapsed-.25f)/.45f));
                art.rectTransform.anchoredPosition=Vector2.Lerp(start,end,move);
                art.sprite=frames[(int)(elapsed*18)%frames.Length];
                var alpha=1-Mathf.Clamp01((move-.8f)/.2f);
                art.color=new Color(1,1,1,alpha);amountLabel.color=new Color(1,.86f,.25f,alpha);
                return;
            }
            art.sprite=frames[Mathf.Clamp(fixedFrame>=0?fixedFrame:flash?(int)(t*6):(int)(elapsed*18)%6,0,frames.Length-1)];
            if(!flash)
            {
                if(!target){Destroy(gameObject);return;}
                Vector2 end=space.InverseTransformPoint(target.position);
                var control=(start+end)*.5f+Vector2.up*140;
                var travel=buffAmount?Mathf.SmoothStep(0,1,Mathf.Clamp01((elapsed-.35f)/.5f)):t;
                art.rectTransform.anchoredPosition=(1-travel)*(1-travel)*start+2*(1-travel)*travel*control+travel*travel*end;
                if(buffAmount&&amountLabel)
                    amountLabel.color=new Color(.3f,1,.45f,1-Mathf.Clamp01((travel-.15f)/.45f));
            }
            art.color=new Color(tint.r,tint.g,tint.b,t>.85f?(1-t)/.15f:1);
        }
    }
}
