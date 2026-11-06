using System;
using System.Linq;
using Project.Data;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;
namespace Project.UI
{
    public sealed class DailyShopScreen:MonoBehaviour
    {
        RectTransform safe,layout,content;
        Font font;ShopPrices prices;AllySummon allies;TreasureDatabase relics;
        Text gems,gold,message,countdown;string day;int lastGems,lastGold;bool busy;
        float messageUntil;
        readonly Color dark=new Color(.17f,.13f,.24f);
        void Start()
        {
            font=UiFonts.Display;prices=ShopPrices.Load();allies=Resources.Load<AllySummon>("Summon/Catalog");relics=Resources.Load<TreasureDatabase>("Shop/Relics");
            if(!relics)relics=Resources.FindObjectsOfTypeAll<TreasureDatabase>().FirstOrDefault();
            safe=Rect(transform,"SafeArea",0,0,0,0);safe.anchorMin=Vector2.zero;safe.anchorMax=Vector2.one;safe.offsetMin=safe.offsetMax=Vector2.zero;
            layout=Rect(safe,"ShopLayout",0,0,1125,2436);
            Box(layout,"Background",0,0,1125,2436,dark);
            var header=Rect(layout,"MerchantHeader",0,968,1125,500).gameObject.AddComponent<RawImage>();header.texture=Resources.Load<Texture2D>("ShopV2/header");header.uvRect=new Rect(.125f,0,.75f,1);header.raycastTarget=false;
            // Shared chrome (UI-005): back top-left (wallets shift right), ribbon title on the counter,
            // no bottom bar — it only held Back and a single "Shop" tab.
            ScreenChrome.Back(layout,new Vector2(-487,1145),()=>Destroy(gameObject));
            var goldWallet=Card(layout,"GoldWallet",-120,1145,360,100,Color.white);BattleUiSkin.Skin(goldWallet.GetComponent<Image>(),BattleUiSkin.Navy);
            var gemWallet=Card(layout,"GemWallet",330,1145,360,100,Color.white);BattleUiSkin.Skin(gemWallet.GetComponent<Image>(),BattleUiSkin.Navy);
            Picture(layout,Resources.Load<Sprite>("HomeChrome/coin"),-255,1145,80);gold=Text(layout,"",-90,1145,240,90,52);
            Picture(layout,Resources.Load<Sprite>("Summon/gem"),195,1145,80);gems=Text(layout,"",360,1145,240,90,52);
            var viewport=Rect(layout,"ShopScroll",0,-280,1060,1820);viewport.gameObject.AddComponent<RectMask2D>();viewport.gameObject.AddComponent<Image>().color=new Color(0,0,0,.001f);
            var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.vertical=true;scroll.viewport=viewport;
            scroll.scrollSensitivity=80f;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.inertia=true;scroll.decelerationRate=.135f;
            content=Rect(viewport,"Products",0,0,1060,2600);content.anchorMin=content.anchorMax=content.pivot=new Vector2(.5f,1);content.anchoredPosition=Vector2.zero;scroll.content=content;CollectionScrollHint.Install(scroll);
            // A hit surface also allows dragging in the gaps between products.
            content.gameObject.AddComponent<Image>().color=new Color(0,0,0,.001f);
            ScreenChrome.Title(layout,"SHOP",new Vector2(0,715),620);
            message=Text(layout,"",0,-1150,1000,70,32);
            Refresh();LateUpdate();
        }
        void LateUpdate()
        {
            if(!layout)return;var area=Screen.safeArea;
            safe.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);safe.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);
            layout.localScale=Vector3.one*Mathf.Min(safe.rect.width/1125,safe.rect.height/2436);
        }
        void Update()
        {
            if(!layout)return;
            if(message && message.text.Length>0){if(messageUntil==0)messageUntil=Time.unscaledTime+4;else if(Time.unscaledTime>=messageUntil){message.text="";messageUntil=0;}}
            if(day!=DailyShop.Day(DateTime.UtcNow)){Refresh();return;}
            if(lastGems!=GameProgress.Gems||lastGold!=GameProgress.Gold)Wallet();
            if(countdown){var local=DateTime.UtcNow.AddHours(7);var remaining=local.Date.AddDays(1)-local;countdown.text=$"{(int)remaining.TotalHours:00}h {remaining.Minutes:00}m {remaining.Seconds:00}s";}
        }
        void Wallet(){lastGems=GameProgress.Gems;lastGold=GameProgress.Gold;gems.text=lastGems.ToString("N0");gold.text=lastGold.ToString("N0");}
        void Refresh()
        {
            day=DailyShop.Day(DateTime.UtcNow);
            if(DailyShop.Ensure(SaveSystem.Current,day,allies?allies.Units:null,relics,prices,new System.Random()))SaveSystem.Save();
            foreach(Transform child in content){child.gameObject.SetActive(false);Destroy(child.gameObject);}Wallet();
            Section("Gold",-1850);
            for(int i=0;i<prices.ExchangeGemCosts.Length;i++)
            {
                int cost=prices.ExchangeGemCosts[i];float x=(i-1)*345;
                var card=Product(content,"GoldPack",x,-2090,325,350);
                // Pack art grows with the pack (chat 004): small stack → pouch → chest.
                var pile=Resources.Load<Sprite>("ShopPacks/gold-"+PackSize(i));if(pile)Picture(card,pile,0,5,190);else Texture(card,"Shop/gold",0,25,150);
                Text(card,(cost*prices.GoldPerGem).ToString("N0"),0,125,310,65,44);
                var buy=PriceButton(card,"Exchange",0,-132,300,84,()=>Confirm("Gold",cost*prices.GoldPerGem,cost,null,null,false,()=>DailyShop.Exchange(SaveSystem.Current,cost,prices)));
                ScreenChrome.Action(buy,ScreenChrome.Role.Primary);GemPrice(buy,cost);
            }
            Section("Daily Shop",-55);
            countdown=Text(content,"",0,-130,1000,55,36);
            var offers=SaveSystem.Current.DailyShopOffers;
            if(offers==null||offers.Count!=9)Text(content,"Shop data unavailable. Nothing charged.",0,-700,1000,120,38);
            else for(int i=0;i<9;i++)
            {
                int slot=i;var offer=offers[i];UnitDefinition unit=null;TreasureItem item=null;
                if(offer.Ally)unit=allies?allies.Units.FirstOrDefault(u=>u && u.UnitId==offer.Id):null;
                else item=relics?.Find(offer.Id);
                bool valid=unit||item!=null;Rank rank=unit?unit.Rank:item?.Tier??Rank.Common;
                string name=unit?unit.DisplayName:item?.DisplayName??"Unavailable";
                // Same rarity frames as Summon Results (chat 003), 302x441 aspect; price button under the card.
                float rowY=-390-i/3*535;
                var card=Frame(content,"Offer"+i,(i%3-1)*345,rowY,rank);
                Sprite portrait=unit?unit.Portrait:item?.Icon;
                if(!portrait && unit && unit.Animations?.Idle?.IsValid==true)portrait=unit.Animations.Idle.FrameAt(0);
                if(unit) AllyPortrait(card,unit,portrait);
                else{var window=Rect(card,"IconWindow",0,52,250,289);SpriteFit.Place(window,portrait,new Vector2(250,289),.8f,.72f,false);}
                // Long names ("Chipped Wooden Cup") shrink to fit the plate instead of being cut.
                var plate=Text(card,name,0,-144,250,62,28);plate.resizeTextForBestFit=true;plate.resizeTextMinSize=16;plate.resizeTextMaxSize=26;
                // Count badge (shard icon + ×N) replaces the unexplained corner icon.
                var badge=Box(card,"Count",72,204,150,54,Color.white);badge.raycastTarget=false;badge.sprite=Resources.Load<Sprite>("SummonKit/price-pill");
                if(badge.sprite){badge.type=Image.Type.Sliced;badge.pixelsPerUnitMultiplier=badge.sprite.rect.height/54f;}
                Texture(badge.transform,"Shop/fragment",-45,0,38);Text(badge.transform,"×"+offer.Count,15,0,100,50,30);
                var buy=PriceButton(content,"BuyShards",(i%3-1)*345,rowY-262,300,84,()=>Confirm(name,offer.Count,offer.GemPrice,unit,item?.Icon,true,()=>DailyShop.Buy(SaveSystem.Current,slot,allies?allies.Units:null,relics)));
                ScreenChrome.Action(buy,ScreenChrome.Role.Primary);
                if(offer.Sold)
                {
                    // Sold: grey button AND a dimmed card, so it no longer reads as buyable.
                    var im=buy.GetComponent<Image>();im.sprite=Resources.Load<Sprite>("ButtonsV2/disabled")??im.sprite;
                    foreach(var g in card.GetComponentsInChildren<Graphic>())g.color*=new Color(.55f,.55f,.6f,1);
                    Text(buy,"SOLD OUT",0,0,260,65,30);
                }
                else GemPrice(buy,offer.GemPrice);
                buy.GetComponent<Button>().interactable=!offer.Sold&&valid;

            }
            Section("Keys",-2350);
            for(int i=0;i<prices.KeyExchangeCounts.Length;i++)
            {
                int count=prices.KeyExchangeCounts[i];int cost=count*prices.GemsPerKey;float x=(i-1)*345;
                var card=Product(content,"KeyPack",x,-2590,325,350);
                var pile=Resources.Load<Sprite>("ShopPacks/keys-"+PackSize(i))??Resources.Load<Sprite>("HomeChrome/key");
                if(pile)Picture(card,pile,0,5,190);
                Text(card,count.ToString("N0"),0,125,310,65,44);
                var buy=PriceButton(card,"ExchangeKeys",0,-132,300,84,()=>Confirm("Keys",count,cost,null,pile,false,()=>DailyShop.ExchangeKeys(SaveSystem.Current,count,prices)));
                ScreenChrome.Action(buy,ScreenChrome.Role.Primary);GemPrice(buy,cost);
            }
            Section("Gems",-2850);
            int[] packs={100,500,1000};
            for(int i=0;i<3;i++)
            {
                int amount=packs[i];
                var pack=Product(content,"FreeTestGems",(i-1)*345,-3090,325,350);
                Text(pack,amount.ToString("N0"),0,125,310,65,44);
                GemBundle(pack,i);
                var buy=PriceButton(pack,"ClaimGems",0,-132,300,84,()=>{GameProgress.AddGems(amount);message.text="+"+amount+" gems · Test only";Wallet();});
                ScreenChrome.Action(buy,ScreenChrome.Role.Secondary);Text(buy,"FREE",0,0,280,65,38);
            }
            Text(content,"Test only · No payment",0,-3320,1000,65,32);
            content.sizeDelta=new Vector2(1060,3390);
        }
        void Confirm(string product,int quantity,int gemCost,UnitDefinition unit,Sprite relicIcon,bool daily,Func<bool> transaction)
        {
            if(busy)return;busy=true;
            string presentedDay=day;bool submitted=false;
            var dim=Box(layout,"PurchaseConfirm",0,0,1125,2436,new Color(0,0,0,.78f));
            var panel=Card(dim.transform,"Confirmation",0,0,980,1120,Color.white);
            BattleUiSkin.Skin(panel.GetComponent<Image>(),BattleUiSkin.Navy);
            Text(panel,"CONFIRM PURCHASE",0,455,910,110,54);
            var tile=Card(panel,"ProductPortrait",-285,230,265,265,unit?Tier(unit.Rank):new Color(.24f,.20f,.32f));
            if(unit)AllyPortrait(tile,unit,unit.Portrait,0);
            else if(relicIcon)Picture(tile,relicIcon,0,0,210);
            else Texture(tile,"Shop/gold",0,0,220);
            Text(panel,product,150,305,550,135,46);
            Text(panel,"×"+quantity.ToString("N0"),150,200,550,100,76);
            var kind=Text(panel,unit?"Ally shards":daily?"Relic shards":"Gold",150,115,550,70,36);kind.color=new Color(.68f,.8f,.92f);
            var costRow=Card(panel,"Cost",0,-20,860,130,Color.white);BattleUiSkin.Skin(costRow.GetComponent<Image>(),BattleUiSkin.Navy);
            Text(costRow,"Cost",-245,0,240,100,42);
            Picture(costRow,Resources.Load<Sprite>("Summon/gem"),-15,0,85);Text(costRow,gemCost.ToString("N0"),170,0,280,100,62);
            Box(panel,"Divider",0,-115,850,3,new Color(.2f,.34f,.48f));
            Text(panel,"Gems now",-235,-170,420,65,34);Text(panel,"After purchase",235,-170,420,65,34);
            int balance=GameProgress.Gems;bool affordable=balance>=gemCost;
            Picture(panel,Resources.Load<Sprite>("Summon/gem"),-365,-255,65);Text(panel,balance.ToString("N0"),-170,-255,300,100,56);
            Picture(panel,Resources.Load<Sprite>("Summon/gem"),105,-255,65);
            var remaining=Text(panel,affordable?(balance-gemCost).ToString("N0"):"Not enough",300,-255,310,100,affordable?56:38);
            if(!affordable)remaining.color=new Color(1,.5f,.5f);
            var yes=Button(panel,"Confirm",225,-400,415,140,0,()=>
            {
                if(submitted)return;submitted=true;
                // All displayed balances are previews; transaction revalidates live state.
                if(presentedDay!=DailyShop.Day(DateTime.UtcNow))message.text="Shop refreshed. Please choose again.";
                else if(transaction()){SaveSystem.Save();message.text="Purchase complete";}
                else message.text="Not enough gems, sold out or unavailable. Nothing charged.";
                dim.gameObject.SetActive(false);Destroy(dim.gameObject);busy=false;Refresh();
            });
            Picture(yes,Resources.Load<Sprite>("Summon/gem"),-105,0,85);Text(yes,gemCost.ToString("N0"),65,0,230,110,56);
            yes.GetComponent<Button>().interactable=affordable;
            var no=Button(panel,"Cancel",-225,-400,415,140,1,()=>{dim.gameObject.SetActive(false);Destroy(dim.gameObject);busy=false;});
            Text(no,"Cancel",0,0,390,110,48);
            Text(panel,daily?"One purchase today":"Gold exchange",0,-515,850,60,31);
        }
        // Content is top-pivoted. Its children MUST also anchor at the top,
        // otherwise their negative Y starts halfway down the full content height.
        RectTransform Rect(Transform p,string name,float x,float y,float w,float h){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(p,false);r.anchorMin=r.anchorMax=p==content?new Vector2(.5f,1):Vector2.one*.5f;r.pivot=Vector2.one*.5f;r.anchoredPosition=new Vector2(x,y);r.sizeDelta=new Vector2(w,h);return r;}
        // Section headers use the shared navy plaque; re-anchor to the top like every other product row.
        void Section(string label,float y){var plaque=(RectTransform)ScreenChrome.Title(content,label,Vector2.zero,620,ScreenChrome.Banner.Section).transform.parent;plaque.anchorMin=plaque.anchorMax=new Vector2(.5f,1);plaque.anchoredPosition=new Vector2(0,y);}
        // Packs use the navy surface of the rest of the game instead of the cream card.
        RectTransform Product(Transform p,string name,float x,float y,float w,float h){var im=Box(p,name,x,y,w,h,Color.white);BattleUiSkin.Skin(im,BattleUiSkin.Navy);return im.rectTransform;}
        RectTransform Frame(Transform p,string name,float x,float y,Rank rank)
        {
            // Flat v2 frames (chat 003-v2): navy window top 5–71%, dark name band 72–94%.
            var im=Box(p,name,x,y,290,438,Color.white);im.raycastTarget=false;
            im.sprite=Resources.Load<Sprite>("RewardCardsV2/card-"+rank switch{Rank.Rare=>"rare",Rank.Epic=>"epic",Rank.Legendary=>"legendary",_=>"common"});
            if(!im.sprite)RelicUiSkin.Apply(im,3,Tier(rank));
            return im.rectTransform;
        }
        RectTransform PriceButton(Transform p,string name,float x,float y,float w,float h,Action action){var im=Box(p,name,x,y,w,h,new Color(1,1,1,.001f));var b=im.gameObject.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(()=>action());ButtonFamilySkin.States(b);return im.rectTransform;}
        Image Box(Transform p,string n,float x,float y,float w,float h,Color c){var im=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();im.color=c;return im;}
        RectTransform Card(Transform p,string n,float x,float y,float w,float h,Color c){var im=Box(p,n,x,y,w,h,c);RelicUiSkin.Apply(im,3,c);return im.rectTransform;}
        RectTransform Button(Transform p,string n,float x,float y,float w,float h,int skin,Action action){var im=Box(p,n,x,y,w,h,Color.white);var b=im.gameObject.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(()=>action());ScreenChrome.Action(b,skin==0?ScreenChrome.Role.Primary:ScreenChrome.Role.Cancel);return im.rectTransform;}
        Text Text(Transform p,string value,float x,float y,float w,float h,int size){var t=Rect(p,"Label",x,y,w,h).gameObject.AddComponent<Text>();t.font=font;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.text=value;t.raycastTarget=false;var o=t.gameObject.AddComponent<Outline>();o.effectDistance=new Vector2(1.5f,-1.5f);return t;}
        // Shop-only head crops preserve the original battle sprites and animations.
        void AllyPortrait(Transform card,UnitDefinition unit,Sprite fallback,float windowY=52)
        {
            var head=Resources.Load<Sprite>("ShopV2/Heads/"+unit.UnitId);
            // Heads are cut flat at the shoulders: sit them on the bottom of the frame's portrait window.
            var window=Rect(card,"PortraitWindow",0,windowY,250,289);window.gameObject.AddComponent<RectMask2D>();
            // Heads v3 are uniform square crops (tools/build_uniform_heads.py): fill the window, sit on its floor.
            SpriteFit.Place(window,head?head:fallback,new Vector2(250,289),1f,1f,true);
        }
        static string PackSize(int tier)=>tier==0?"small":tier==1?"medium":"large";
        void GemBundle(Transform card,int tier)
        {
            var pile=Resources.Load<Sprite>("ShopPacks/gems-"+PackSize(tier));
            if(pile){Picture(card,pile,0,5,190);return;}
            var gem=Resources.Load<Sprite>("Summon/gem");
            if(tier==0){Picture(card,gem,0,12,140);return;}
            // A shared crystal silhouette keeps the pack art consistent with the wallet.
            int count=tier==1?3:6;
            for(int j=0;j<count;j++)
            {
                float x=tier==1?(j-1)*54:(j%3-1)*54;
                float y=tier==1?(j==1?28:-4):(j<3?42:-18);
                Picture(card,gem,x,y,100);
            }
        }
        void Picture(Transform p,Sprite sprite,float x,float y,float size){if(!sprite)return;var im=Box(p,"Icon",x,y,size,size,Color.white);im.sprite=sprite;im.preserveAspect=true;im.raycastTarget=false;}
        void Texture(Transform p,string path,float x,float y,float size){var t=Resources.Load<Texture2D>(path);if(!t)return;var im=Rect(p,"Art",x,y,size,size).gameObject.AddComponent<RawImage>();im.texture=t;im.raycastTarget=false;}
        void GemPrice(Transform p,int cost){Picture(p,Resources.Load<Sprite>("Summon/gem"),-85,0,48);Text(p,cost.ToString("N0"),35,0,170,65,36);}
        static Color Tier(Rank tier)=>tier switch{Rank.Rare=>new Color(.06f,.40f,.62f),Rank.Epic=>new Color(.45f,.14f,.63f),Rank.Legendary=>new Color(.65f,.43f,.06f),_=>new Color(.40f,.33f,.29f)};
    }
}
