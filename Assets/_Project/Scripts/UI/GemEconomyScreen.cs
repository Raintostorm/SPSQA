using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Project.Meta;

namespace Project.UI
{
    /// <summary>No-payment test gem shop and ally shard Summon.</summary>
    public sealed class GemEconomyScreen : MonoBehaviour
    {
        Font font;
        Text wallet,feedback;
        int shown=-1;
        bool shop;
        AllySummon catalog;
        Text stoneBalance, pityLabel;
        RectTransform safeArea, summonLayout;
        readonly System.Random rng=new System.Random();
        public static void Show(Transform parent,bool shop)
        {
            var old=parent.Find("GemEconomyScreen");if(old){old.SetAsLastSibling();return;}
            var go=new GameObject("GemEconomyScreen",typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var r=go.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
            go.GetComponent<Image>().color=new Color(.1f,.13f,.22f);
            var view=go.AddComponent<GemEconomyScreen>();view.shop=shop;view.font=UiFonts.Display;view.Build();
        }
        void Update(){if(!shop && shown!=GameProgress.Gems){shown=GameProgress.Gems;if(wallet)wallet.text=shown.ToString("N0");}}
        void LateUpdate()
        {
            if(stoneBalance)stoneBalance.text=SaveSystem.Current.MythicStones.ToString("N0");
            // UI-009 / Q2: hien bo dem bao hiem, KHONG hien ti le (GAME_RULES: khong dua rates len man chinh).
            if(pityLabel&&catalog&&catalog.Balance&&catalog.Balance.LegendaryPity>0)
            {
                int left=Mathf.Max(1,catalog.Balance.LegendaryPity-SaveSystem.Current.SummonsSinceLegendary);
                pityLabel.text=left==1?"<color=#F5B728>Next summon: Legendary guaranteed!</color>":$"Legendary guaranteed in <color=#F5B728>{left}</color> summons";
            }
            if(!summonLayout)return;
            var area=Screen.safeArea;safeArea.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);safeArea.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);
            summonLayout.localScale=Vector3.one*Mathf.Min(safeArea.rect.width/1125f,safeArea.rect.height/2436f);
        }
        void Build()
        {
            if(!shop){BuildSummon();return;}
            gameObject.AddComponent<DailyShopScreen>();
        }
        void BuildLegacyShop()
        {
            var content=Rect("SafeArea",transform,Vector2.zero,Vector2.zero);
            var area=Screen.safeArea;content.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);content.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);content.offsetMin=content.offsetMax=Vector2.zero;
            Label(content,shop?"SHOP":"SUMMON",new Vector2(0,750),new Vector2(1000,130),86);
            var header=Rect("Wallet",content,new Vector2(230,940),new Vector2(400,100));BattleUiSkin.Skin(header.gameObject.AddComponent<Image>(),BattleUiSkin.Navy);
            Art(header,"gem",new Vector2(-140,0),new Vector2(125,125));wallet=Label(header,"",new Vector2(40,0),new Vector2(270,90),48);
            feedback=Label(content,"",new Vector2(0,-610),new Vector2(1000,130),36);
            if(shop)
            {
                Label(content,"TEST SHOP · NO PAYMENT",new Vector2(0,550),new Vector2(1000,100),44);
                int[] packs={100,500,1000};
                for(int i=0;i<packs.Length;i++)
                {
                    int amount=packs[i];float y=260-i*260;
                    var card=Rect("GemPack",content,new Vector2(0,y),new Vector2(940,220));BattleUiSkin.Skin(card.gameObject.AddComponent<Image>(),BattleUiSkin.Dark);
                    Art(card,"gem",new Vector2(-330,0),new Vector2(170,170));
                    Label(card,amount.ToString("N0")+" Gems",new Vector2(-60,0),new Vector2(330,100),48);
                    Button(card,"FREE",new Vector2(300,0),new Vector2(260,120),()=>{GameProgress.AddGems(amount);feedback.text=$"+{amount:N0} Gems · No payment charged";});
                }
            }
            Button(content,"Back",new Vector2(0,-950),new Vector2(500,130),()=>Destroy(gameObject));
        }
        void BuildSummon()
        {
            catalog=Resources.Load<AllySummon>("Summon/Catalog");
            var backdrop=Rect("WitchTentBackground",transform,Vector2.zero,Vector2.zero);
            backdrop.anchorMin=Vector2.zero;backdrop.anchorMax=Vector2.one;backdrop.offsetMin=backdrop.offsetMax=Vector2.zero;
            var picture=backdrop.gameObject.AddComponent<Image>();picture.sprite=Resources.Load<Sprite>("Summon/witch-tent");picture.raycastTarget=false;
            if(picture.sprite){var fit=backdrop.gameObject.AddComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.EnvelopeParent;fit.aspectRatio=picture.sprite.rect.width/picture.sprite.rect.height;}
            safeArea=Rect("SafeArea",transform,Vector2.zero,Vector2.zero);safeArea.anchorMin=Vector2.zero;safeArea.anchorMax=Vector2.one;safeArea.offsetMin=safeArea.offsetMax=Vector2.zero;
            summonLayout=Rect("SummonLayout",safeArea,Vector2.zero,new Vector2(1125,2436));
            // Shared chrome (UI-005): ribbon instead of the full-width box that hid the tent; back top-left.
            ScreenChrome.Title(summonLayout,"SUMMON",new Vector2(0,860),760);
            ScreenChrome.Back(summonLayout,new Vector2(-487,1025),()=>Destroy(gameObject));
            var header=Rect("Wallet",summonLayout,new Vector2(300,1025),new Vector2(360,96));BattleUiSkin.Skin(header.gameObject.AddComponent<Image>(),BattleUiSkin.Navy);
            Art(header,"gem",new Vector2(-135,0),new Vector2(110,110));wallet=Label(header,"",new Vector2(40,0),new Vector2(270,90),52);
            var stoneWallet=Rect("MythicStoneWallet",summonLayout,new Vector2(-150,1025),new Vector2(360,96));BattleUiSkin.Skin(stoneWallet.gameObject.AddComponent<Image>(),BattleUiSkin.Navy);
            var stoneIcon=Rect("MythicStone",stoneWallet,new Vector2(-125,0),new Vector2(85,85)).gameObject.AddComponent<Image>();stoneIcon.sprite=MythicStoneArt.Icon;stoneIcon.preserveAspect=true;stoneIcon.raycastTarget=false;
            stoneBalance=Label(stoneWallet,"",new Vector2(45,0),new Vector2(260,90),44);
            var caption=Label(summonLayout,"Ally shards & Mythic Stones",new Vector2(0,-660),new Vector2(1000,80),44);Outline(caption,3);
            SummonButton(-260,"Summon 1x · 10",1);
            SummonButton(260,"Summon 10x · 100",10);
            feedback=Label(summonLayout,"",new Vector2(0,-1000),new Vector2(1000,130),34);Outline(feedback,2);
            pityLabel=Label(summonLayout,"",new Vector2(0,-1100),new Vector2(1000,60),34);pityLabel.supportRichText=true;Outline(pityLabel,2);
            LateUpdate();
        }
        void SummonButton(float x,string text,int count)
        {
            var button=Button(summonLayout,"",new Vector2(x,-830),new Vector2(490,190),()=>Summon(count));
            // Art SummonKit (003) ve san qua cau + tia sang + de ru-bang nen chong nhieu lop,
            // user 2026-11-04 thay roi. Dung than nut 002 + icon 010 cho bang voi cac nut khac.
            ScreenChrome.Action(button,count>1?ScreenChrome.Role.Primary:ScreenChrome.Role.Secondary);
            var orb=Rect("Orb",button,new Vector2(-170,0),new Vector2(120,120));
            var orbImage=orb.gameObject.AddComponent<Image>();orbImage.raycastTarget=false;orbImage.preserveAspect=true;
            orbImage.sprite=Resources.Load<Sprite>("ButtonIcons/"+(count>1?"summon-10":"summon-1"));
            orbImage.enabled=orbImage.sprite;
            var label=button.GetComponentInChildren<Text>();label.text="Summon ×"+count;label.rectTransform.anchoredPosition=new Vector2(80,32);label.rectTransform.sizeDelta=new Vector2(300,70);label.fontSize=44;
            Outline(label,3);
            var pill=Rect("Price",button,new Vector2(80,-42),new Vector2(220,70));var pillImage=pill.gameObject.AddComponent<Image>();pillImage.raycastTarget=false;
            pillImage.sprite=Resources.Load<Sprite>("SummonKit/price-pill");pillImage.enabled=pillImage.sprite;
            if(pillImage.sprite){pillImage.type=Image.Type.Sliced;pillImage.pixelsPerUnitMultiplier=pillImage.sprite.rect.height/70f;}
            Art(pill,"gem",new Vector2(-62,0),new Vector2(48,52));
            Label(pill,(count*10).ToString("N0"),new Vector2(22,0),new Vector2(140,60),36);
        }
        static void Outline(Text text,float width)
        {var outline=text.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.02f,.025f,.04f);outline.effectDistance=new Vector2(width,-width);}
        void Summon(int count)
        {
            if(GetComponentInChildren<SummonResultsView>())return;
            if(!catalog){feedback.text="Summon data unavailable. No gems spent.";return;}
            if(GameProgress.Gems<count*10){feedback.text="Not enough gems.";return;}
            if(!catalog.TryRoll(count,rng,out var results)){feedback.text="Summon data invalid. No gems spent.";return;}
            SummonResultsView.Show(transform,results,count);
            int totalShards=results.Where(r=>!r.MythicStone).Sum(r=>r.Shards);
            int totalStones=results.Where(r=>r.MythicStone).Sum(r=>r.Shards);
            feedback.text=$"Received {totalShards} ally shards · {totalStones} Mythic Stones.";
        }
        RectTransform Rect(string name,Transform parent,Vector2 pos,Vector2 size)
        {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);BattleUiSkin.Place(r,Vector2.one*.5f,pos,size);return r;}
        void Art(Transform parent,string name,Vector2 pos,Vector2 size)
        {var im=Rect(name,parent,pos,size).gameObject.AddComponent<Image>();im.sprite=Resources.Load<Sprite>("Summon/"+name);im.preserveAspect=true;im.raycastTarget=false;im.enabled=im.sprite!=null;}
        Text Label(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize)
        {var t=Rect("Label",parent,pos,size).gameObject.AddComponent<Text>();t.text=value;t.font=font;t.fontSize=fontSize;t.fontStyle=FontStyle.Bold;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.raycastTarget=false;return t;}
        RectTransform Button(Transform parent,string value,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction action)
        {var r=Rect(value,parent,pos,size);r.gameObject.AddComponent<Image>();var b=r.gameObject.AddComponent<Button>();b.targetGraphic=r.GetComponent<Image>();b.onClick.AddListener(action);ScreenChrome.Action(b,ScreenChrome.Role.Secondary);Label(r,value,Vector2.zero,size-Vector2.one*12,42);return r;}
    }
}
