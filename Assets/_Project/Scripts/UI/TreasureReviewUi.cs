using System;
using System.Collections;
using System.Linq;
using Project.Data;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public sealed partial class TreasureScreen
    {
        RectTransform reviewRoot, collection;
        GameObject reviewModal, resultsOverlay;
        Text walletGold, walletKeys, openLabel;
        Button openAll;
        int selectedRelic;
        readonly Color purple = new Color(.17f,.13f,.24f);
        readonly Color green = new Color(.08f,.72f,.38f);
        Font reviewFont;
        void Start()
        {
            rng=new System.Random(Environment.TickCount);
            BuildRelicReviewUi(); RefreshReviewUi();
        }
        RectTransform R(Transform parent,string name,float x,float y,float w,float h)
        {
            var rt=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
            rt.SetParent(parent,false);rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,1);
            rt.anchoredPosition=new Vector2(x,-y);rt.sizeDelta=new Vector2(w,h);return rt;
        }
        Image B(Transform p,string name,float x,float y,float w,float h,Color color)
        {
            var im=R(p,name,x,y,w,h).gameObject.AddComponent<Image>();im.color=color;
            if(name=="Panel" || name=="Stat" || name=="Reward")RelicUiSkin.Apply(im,3,color);
            if(name=="Gold wallet"||name=="Key wallet")BattleUiSkin.Skin(im,BattleUiSkin.Navy);
            if(name=="ShardTrack" || name=="ShardFill")RelicUiSkin.Apply(im,7,color);
            return im;
        }
        Text T(Transform p,string value,float x,float y,float w,float h,int size)
        {
            var t=R(p,"Text",x,y,w,h).gameObject.AddComponent<Text>();t.font=reviewFont;t.fontSize=size;t.fontStyle=FontStyle.Bold;
            t.text=value;t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.supportRichText=true;
            t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;
            var outline=t.gameObject.AddComponent<Outline>();outline.effectColor=new Color(0,0,0,.8f);outline.effectDistance=new Vector2(1.5f,-1.5f);
            return t;
        }
        Button Btn(Transform p,string name,float x,float y,float w,float h,Color color,Action action)
        {
            var im=B(p,name,x,y,w,h,color);var b=im.gameObject.AddComponent<Button>();b.targetGraphic=im;
            bool navigation=name=="Previous"||name=="Next";
            RelicUiSkin.Apply(im,navigation?(name=="Previous"?4:5):name=="Close"?2:name=="Back"?1:name=="OpenAll"||name=="UpgradeRelic"?0:3,navigation||name=="Close"||name=="Back"||name=="OpenAll"||name=="UpgradeRelic"?Color.white:color);
            // Role-based art (UI-006); OpenAll/UpgradeRelic keep their gold-state sprites below.
            if(name=="OpenAll"||name=="UpgradeRelic")ScreenChrome.Action(b,ScreenChrome.Role.Primary);
            else if(name=="Continue")ScreenChrome.Action(b,ScreenChrome.Role.Secondary,"continue");
            else if(name=="Back")ButtonFamilySkin.Apply(im,6);
            else if(name=="Close")ScreenChrome.Action(b,ScreenChrome.Role.Danger);
            // GoldStates would put the old approved sprite back over the role art, so roles own the states now.
            b.onClick.AddListener(()=>action());if(name!="OpenAll"&&name!="UpgradeRelic"&&name!="Continue"&&name!="Close")ButtonFamilySkin.States(b);return b;
        }
        Image Icon(Transform p,Sprite sprite,float x,float y,float size)
        {var im=B(p,"Icon",x,y,size,size,Color.white);im.sprite=sprite;im.preserveAspect=true;im.raycastTarget=false;return im;}
        void BuildRelicReviewUi()
        {
            reviewFont=UiFonts.Display;
            var bg=GetComponentsInChildren<Image>(true).FirstOrDefault(i=>i.name=="Background" || i.name=="Backdrop");
            Sprite background=bg?bg.sprite:null;
            foreach(Transform child in transform)child.gameObject.SetActive(false);
            reviewRoot=R(transform,"RelicsReviewUI",0,0,1125,2436);
            reviewRoot.anchorMin=Vector2.zero;reviewRoot.anchorMax=Vector2.one;reviewRoot.offsetMin=reviewRoot.offsetMax=Vector2.zero;
            B(reviewRoot,"Page",0,0,1125,2436,new Color(.12f,.15f,.20f));
            if(background && !Resources.Load<Sprite>("RelicVault/gallery")){var art=B(reviewRoot,"Vault",0,0,1125,2436,Color.white);art.sprite=background;art.raycastTarget=false;}
            // Shared chrome (UI-005): ribbon title + round back, pinned in this screen's top-edge layout.
            // Gallery replaces the old floating chest; title and wallets remain live UI.
            ScreenChrome.PinTop(ScreenChrome.Title(reviewRoot,"RELICS",Vector2.zero,600,ScreenChrome.Banner.Screen,120),0,118);
            ScreenChrome.PinTop(ScreenChrome.Back(reviewRoot,Vector2.zero,SceneRoutes.GoHome),-480,10);
            // Gold wallet shifted right so the round back button no longer covers its coin.
            B(reviewRoot,"Gold wallet",-170,23,330,85,new Color(.05f,.08f,.13f,.95f));
            B(reviewRoot,"Key wallet",355,23,260,85,new Color(.05f,.08f,.13f,.95f));
            walletGold=T(reviewRoot,"",-155,25,260,85,48);Icon(reviewRoot,ApprovedUiArt.Get("coin"),-300,35,65);
            Icon(reviewRoot,Resources.Load<Sprite>("HomeChrome/key"),250,35,55);
            walletKeys=T(reviewRoot,"",380,25,200,85,48);
            // Vom tuong (RelicVault/gallery) nam TRONG noi dung cuon, nen cuon cung relic thay vi dung yen
            // va cat ngang hang the o mep 520. Khung cuon vi the bat dau ngay duoi thanh vi.
            var viewport=R(reviewRoot,"RelicScroll",0,170,1125,2050);viewport.gameObject.AddComponent<RectMask2D>();
            var hit=viewport.gameObject.AddComponent<Image>();hit.color=new Color(0,0,0,.001f);
            var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.viewport=viewport;
            collection=R(viewport,"Content",0,0,1125,2000);scroll.content=collection;CollectionScrollHint.Install(scroll);scroll.scrollSensitivity=60;
            B(reviewRoot,"Footer",0,2220,1125,216,new Color(.10f,.13f,.18f));
            openAll=Btn(reviewRoot,"OpenAll",0,2250,1010,145,new Color(1,.76f,.08f),()=>StartCoroutine(OpenEveryKey()));
            openLabel=T(openAll.transform,"",0,0,980,145,54);
            // Mot hinh chia khoa duy nhat cho ca game: ban o thanh vi (HomeChrome/key).
            Icon(openAll.transform,Resources.Load<Sprite>("HomeChrome/key"),-120,90,40);
        }
        void RefreshReviewUi()
        {
            walletGold.text=GameProgress.Gold.ToString("N0");walletKeys.text=GameProgress.Keys.ToString("N0");
            openLabel.text="OPEN ALL\n<size=44>"+GameProgress.Keys.ToString("N0")+"</size>";
            openAll.interactable=!revealing && GameProgress.Keys>0;
            foreach(Transform child in collection){child.gameObject.SetActive(false);Destroy(child.gameObject);}
            if(database==null)return;
            float y=0;
            var gallery=Resources.Load<Sprite>("RelicVault/gallery");
            if(gallery){var art=B(collection,"Gallery",0,0,1125,358,Color.white);art.sprite=gallery;art.raycastTarget=false;y=358;}
            foreach(Rank tier in new[]{Rank.Common,Rank.Rare,Rank.Epic,Rank.Legendary})
            {
                float sectionTop=y;
                var wall=Resources.Load<Sprite>("RelicVault/"+tier.ToString().ToLowerInvariant());
                var section=B(collection,"VaultWall_"+tier,0,sectionTop,1125,100,Color.white);
                section.sprite=wall;section.type=Image.Type.Sliced;section.raycastTarget=false;
                ScreenChrome.PinTop(ScreenChrome.Title(collection,TierName(tier).ToUpperInvariant(),Vector2.zero,420,ScreenChrome.Banner.Section),0,y);y+=110;
                var entries=database.Items.Select((item,index)=>new{item,index}).Where(e=>e.item.Tier==tier).ToArray();
                for(int j=0;j<entries.Length;j++)
                {
                    var item=entries[j].item;int index=entries[j].index;var owned=SaveSystem.Current.FindItem(item.Id);
                    // Same roster tiles as Mercenaries (chat 006): icon window top, name + shards in the band.
                    float x=(j%3-1)*345,row=y+j/3*360;
                    var card=Btn(collection,item.Id,x,row,300,334,owned==null?new Color(.25f,.22f,.31f):TierColor(tier),()=>ShowRelicDetail(index));
                    var tile=Resources.Load<Sprite>("RosterTilesV1/tile-"+(owned==null?"locked":tier switch{Rank.Rare=>"rare",Rank.Epic=>"epic",Rank.Legendary=>"legendary",_=>"common"}));
                    if(tile){var face=card.GetComponent<Image>();face.sprite=tile;face.type=Image.Type.Simple;face.color=Color.white;}
                    var window=R(card.transform,"IconWindow",0,20,268,187);
                    var ic=SpriteFit.Place(window,item.Icon,new Vector2(268,187),.8f,.85f,false);if(owned==null)ic.color=new Color(.45f,.45f,.48f);
                    var nameLabel=T(card.transform,item.DisplayName,0,216,270,40,26);nameLabel.resizeTextForBestFit=true;nameLabel.resizeTextMinSize=16;nameLabel.resizeTextMaxSize=26;
                    if(owned==null)T(card.transform,"Not owned",0,260,260,36,26);
                    else
                    {
                        var lv=T(card.transform,"Lv."+owned.Level,-66,24,140,42,30);lv.alignment=TextAnchor.MiddleLeft;
                        bool max=owned.Level>=item.MaxLevel;int need=TreasureItem.ShardsToNext(owned.Level);
                        Progress(card.transform,0,260,260,36,max?1:(float)owned.Shards/need,max?"MAX":owned.Shards+" / "+need,28);
                        // Enough shards for the next level (gold is checked on the detail page).
                        if(!max&&owned.Shards>=need)Icon(card.transform,Resources.Load<Sprite>("Badges/upgrade"),112,4,62);
                    }
                }
                y+=Mathf.CeilToInt(entries.Length/3f)*360+15;
                section.rectTransform.sizeDelta=new Vector2(1125,y-sectionTop);
            }
            collection.sizeDelta=new Vector2(1125,y);
        }
        static Color TierColor(Rank tier)=>tier switch{Rank.Rare=>new Color(.07f,.43f,.66f),Rank.Epic=>new Color(.48f,.15f,.69f),Rank.Legendary=>new Color(.75f,.52f,.04f),_=>new Color(.43f,.36f,.30f)};
        void Progress(Transform p,float x,float y,float w,float h,float fill,string text,int size)
        {
            var track=B(p,"ShardTrack",x,y,w,h,new Color(.06f,.07f,.1f));
            var bar=B(track.transform,"ShardFill",0,0,w*Mathf.Clamp01(fill),h,green);
            bar.rectTransform.pivot=new Vector2(0,1);bar.rectTransform.anchoredPosition=new Vector2(-w/2,0);
            T(track.transform,text,0,0,w,h,size);
        }
        void ShowRelicDetail(int index)
        {
            if(reviewModal){reviewModal.SetActive(false);Destroy(reviewModal);}
            relicStats.Clear();
            selectedRelic=(index+database.Items.Length)%database.Items.Length;
            var item=database.Items[selectedRelic];var owned=SaveSystem.Current.FindItem(item.Id);
            bool max=owned!=null && owned.Level>=item.MaxLevel;
            var overlay=B(reviewRoot,"RelicDetails",0,0,1125,2436,new Color(0,0,0,.72f));reviewModal=overlay.gameObject;
            var modalCanvas=overlay.gameObject.AddComponent<Canvas>();modalCanvas.overrideSorting=true;modalCanvas.sortingOrder=400;
            overlay.gameObject.AddComponent<GraphicRaycaster>();
            // One stable frame: only the stats body scrolls, never the price or material bar.
            var safe=R(overlay.transform,"SafeArea",0,0,0,0);var area=Screen.safeArea;
            safe.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);
            safe.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);safe.offsetMin=safe.offsetMax=Vector2.zero;
            const float panelHeight=1650;
            var panel=B(safe,"DetailFrame",0,0,970,panelHeight,Color.white);BattleUiSkin.Skin(panel,BattleUiSkin.Navy);if(!relicIcon)UiPop.Play(panel);
            panel.rectTransform.anchorMin=panel.rectTransform.anchorMax=panel.rectTransform.pivot=Vector2.one*.5f;
            panel.rectTransform.anchoredPosition=Vector2.zero;
            Canvas.ForceUpdateCanvases();
            panel.rectTransform.localScale=Vector3.one*Mathf.Max(.1f,Mathf.Min(1,(safe.rect.width-30)/970,(safe.rect.height-40)/panelHeight));
            T(panel.transform,TierName(item.Tier).ToUpperInvariant(),0,25,650,65,40);
            var close=Btn(panel.transform,"Close",405,12,130,130,Color.white,()=>{Destroy(reviewModal);reviewModal=null;});
            ApprovedUiArt.Apply(close.GetComponent<Image>(),"close");
            relicIcon=Icon(panel.transform,item.Icon,0,100,260);
            T(panel.transform,item.DisplayName,0,365,820,90,52);
            relicLevel=T(panel.transform,owned==null?"Not owned":max?"Lv."+owned.Level+"  MAX":"Lv."+owned.Level+"  <color=#61ee80>+1</color>",0,455,800,80,54);
            int required=owned==null?1:TreasureItem.ShardsToNext(owned.Level);
            Progress(panel.transform,0,555,840,85,owned==null?0:max?1:(float)owned.Shards/required,max?"MAX":"Shards  "+(owned?.Shards??0)+" / "+required,46);
            var viewport=R(panel.transform,"StatViewport",0,685,860,530);
            viewport.gameObject.AddComponent<Image>().color=new Color(0,0,0,.001f);
            viewport.gameObject.AddComponent<RectMask2D>();
            var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.viewport=viewport;
            scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=65;
            float contentHeight=Mathf.Max(530,item.Stats.Length*285);
            var statContent=R(viewport,"StatContent",0,0,840,contentHeight);scroll.content=statContent;
            statContent.gameObject.AddComponent<Image>().color=new Color(0,0,0,.001f);
            CollectionScrollHint.Install(scroll);
            if(contentHeight>530)
            {
                var track=B(panel.transform,"StatScrollbar",440,685,12,530,new Color(.02f,.04f,.07f));
                var scrollbar=track.gameObject.AddComponent<Scrollbar>();scrollbar.direction=Scrollbar.Direction.BottomToTop;
                var handle=B(track.transform,"Handle",0,0,12,80,new Color(.4f,.58f,.75f));
                handle.rectTransform.anchorMin=Vector2.zero;handle.rectTransform.anchorMax=Vector2.one;handle.rectTransform.offsetMin=handle.rectTransform.offsetMax=Vector2.zero;
                scrollbar.handleRect=handle.rectTransform;scrollbar.targetGraphic=handle;scroll.verticalScrollbar=scrollbar;
            }
            float y=0;
            foreach(var entry in item.Stats)
            {
                if(y>0)B(statContent,"Divider",0,y-10,800,2,new Color(.18f,.3f,.43f));
                string statIcon=entry.Stat switch{TreasureStat.Atk=>"atk",TreasureStat.Hp=>"hp",TreasureStat.AtkSpeed=>"speed",TreasureStat.CritDamage=>"crit_damage",TreasureStat.CritChance=>"crit_chance",TreasureStat.LifeSteal=>"lifesteal",_=>"defense"};
                var art=R(statContent,"StatIcon",-350,y+60,85,85).gameObject.AddComponent<RawImage>();
                art.texture=entry.Stat==TreasureStat.MythicCapacity && item.Icon ? item.Icon.texture : ApprovedUiArt.StatTexture(statIcon);art.raycastTarget=false;
                T(statContent,(entry.Stat==TreasureStat.MythicCapacity?"Battle · ":"All allies · ")+StatName(entry.Stat),50,y+12,690,55,38);
                string suffix=entry.Stat==TreasureStat.MythicCapacity?" slots":entry.Stat==TreasureStat.Armor?" pts":"%";
                float delta=item.ValueAt(entry.Stat,(owned?.Level??1)+1)-item.ValueAt(entry.Stat,owned?.Level??1);
                relicStats.Add((T(statContent,"+"+item.ValueAt(entry.Stat,owned?.Level??1).ToString("0.#")+suffix,50,y+65,650,90,66),entry.Stat,suffix));
                string next=entry.Stat==TreasureStat.MythicCapacity?"<size=30>At Lv."+(((owned?.Level??1)/2+1)*2)+"</size>\n<color=#61ee80>+1 slot</color>":"<size=30>Next Level</size>\n<color=#61ee80>+"+delta.ToString("0.#")+suffix+"</color>";
                T(statContent,max?"MAX":owned==null?"Not owned":next,0,y+155,700,105,40);
                y+=285;
            }
            if(owned!=null && !max)
            {
                int price=TreasureRoll.UpgradeCost(item,owned.Level);
                var buy=Btn(panel.transform,"UpgradeRelic",0,1420,840,150,new Color(1,.76f,.08f),()=>{int before=owned.Level;if(TreasureRoll.Upgrade(item)){RefreshReviewUi();ShowRelicDetail(selectedRelic);RelicUpgraded(item,before);}});
                Icon(buy.transform,ApprovedUiArt.Get("coin"),-215,24,80);T(buy.transform,price.ToString("N0"),65,0,550,150,64);
                buy.interactable=owned.Shards>=required && GameProgress.Gold>=price;
                T(panel.transform,owned.Shards<required?"Not enough shards":GameProgress.Gold<price?"Not enough gold":"Ready to upgrade",0,1320,820,70,34);
            }
            else T(panel.transform,max?"MAX LEVEL":"Not owned",0,1390,840,100,44);
            Canvas.ForceUpdateCanvases();statContent.anchoredPosition=Vector2.zero;scroll.verticalNormalizedPosition=1;
            var previous=Btn(panel.transform,"Previous",-400,255,130,135,Color.white,()=>ShowRelicDetail(selectedRelic-1));
            var nextButton=Btn(panel.transform,"Next",400,255,130,135,Color.white,()=>ShowRelicDetail(selectedRelic+1));
            ApprovedUiArt.Apply(previous.GetComponent<Image>(),"previous");ApprovedUiArt.Apply(nextButton.GetComponent<Image>(),"next");
        }
        GameObject ShowRelicResults(System.Collections.Generic.List<TreasureRoll.Result> got,int opened,Action continued)
        {
            if(resultsOverlay){resultsOverlay.SetActive(false);Destroy(resultsOverlay);}
            var overlay=B(reviewRoot,"OpenResults",0,0,1125,2436,new Color(.075f,.09f,.17f));
            resultsOverlay=overlay.gameObject;
            var full=overlay.rectTransform;full.anchorMin=Vector2.zero;full.anchorMax=Vector2.one;full.offsetMin=full.offsetMax=Vector2.zero;
            var canvas=overlay.gameObject.AddComponent<Canvas>();canvas.overrideSorting=true;canvas.sortingOrder=450;
            overlay.gameObject.AddComponent<GraphicRaycaster>();
            var safe=R(overlay.transform,"SafeArea",0,0,0,0);var area=Screen.safeArea;
            safe.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);safe.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);safe.offsetMin=safe.offsetMax=Vector2.zero;
            var page=R(safe,"ResultsContent",0,0,1060,2250);page.anchorMin=page.anchorMax=page.pivot=Vector2.one*.5f;page.anchoredPosition=Vector2.zero;
            for(int i=0;i<15;i++)
            {
                var diamond=B(page,"Diamond",(i%3-1)*410+(i%2)*70,i/3*510,90,90,new Color(.6f,.5f,.9f,.045f));
                diamond.rectTransform.localRotation=Quaternion.Euler(0,0,45);diamond.raycastTarget=false;
            }
            var ribbon=Icon(page,ResultScreenArt.Get(0),0,0,1040);ribbon.rectTransform.sizeDelta=new Vector2(1040,300);
            T(ribbon.transform,"RELICS RECEIVED",0,45,910,180,72);
            T(page,opened.ToString("N0")+" keys opened",0,300,1000,80,42);
            var groups=got.Where(r=>r.Item!=null).GroupBy(r=>r.Item.Id).OrderByDescending(g=>g.First().Item.Tier).ThenBy(g=>g.First().Item.DisplayName).ToArray();
            long total=got.Sum(r=>(long)r.ShardsGranted);
            var summary=B(page,"Summary",0,415,1020,240,Color.white);BattleUiSkin.Skin(summary,BattleUiSkin.Navy);
            T(summary.transform,total.ToString("N0"),-250,15,470,130,106);T(summary.transform,"TOTAL SHARDS",-250,150,470,70,34);
            T(summary.transform,groups.Length.ToString("N0"),250,15,470,130,106);T(summary.transform,"relic types",250,150,470,70,34);
            var viewport=R(page,"RewardsScroll",0,690,1040,1350);viewport.gameObject.AddComponent<RectMask2D>();
            viewport.gameObject.AddComponent<Image>().color=new Color(0,0,0,.001f);
            var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.viewport=viewport;
            scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=65;
            var content=R(viewport,"Content",0,0,1020,Mathf.Max(1350,Mathf.CeilToInt(groups.Length/3f)*440));scroll.content=content;
            CollectionScrollHint.Install(scroll);
            for(int i=0;i<groups.Length;i++)
            {
                var group=groups[i];var item=group.First().Item;float x=(i%3-1)*340,y=i/3*440;
                var card=B(content,"Reward",x,y,320,420,TierColor(item.Tier));BattleUiSkin.Skin(card,BattleUiSkin.Neutral);card.color=TierColor(item.Tier);card.raycastTarget=false;
                var iconCrop=R(card.transform,"IconCrop",0,15,280,230);iconCrop.gameObject.AddComponent<RectMask2D>();
                var icon=Icon(iconCrop,item.Icon,0,0,220);
                RelicResultIconFit.Apply(icon);
                T(card.transform,item.DisplayName,0,250,300,90,34);
                long granted=group.Sum(r=>(long)r.ShardsGranted);
                T(card.transform,granted>0?"×"+granted.ToString("N0"):"MAX",0,335,300,75,64);
                if(group.Any(r=>r.IsNew))
                {
                    var badge=B(card.transform,"NewBadge",-95,15,105,55,Color.white);BattleUiSkin.Skin(badge,BattleUiSkin.Green);
                    T(badge.transform,"NEW",0,0,100,55,28);
                }
            }
            if(groups.Length==0)T(content,"No relics received",0,150,1000,100,42);
            var done=Btn(page,"Continue",0,2090,1020,145,Color.white,()=>{if(continued!=null)continued();else Destroy(overlay.gameObject);});
            T(done.transform,"CONTINUE",0,0,1000,145,64);
            done.onClick.AddListener(()=>done.interactable=false);
            Canvas.ForceUpdateCanvases();content.anchoredPosition=Vector2.zero;scroll.verticalNormalizedPosition=1;
            float fit=Mathf.Min(1,Mathf.Min((safe.rect.width-35)/1060,(safe.rect.height-40)/2250));page.localScale=Vector3.one*Mathf.Max(.1f,fit);
            return overlay.gameObject;
        }

        Image relicIcon; Text relicLevel;
        readonly System.Collections.Generic.List<(Text label,TreasureStat stat,string suffix)> relicStats=new System.Collections.Generic.List<(Text,TreasureStat,string)>();
        // ANIM-004 relic: icon sang len, Lv. nhay xanh, tung chi so chay tu muc cu len muc moi.
        void RelicUpgraded(TreasureItem item,int before)
        {
            RelicShine.Play(relicIcon);
            UiPulse.Play(relicLevel);
            foreach(var s in relicStats)StatCountUp.Play(s.label,item.ValueAt(s.stat,before),item.ValueAt(s.stat,before+1),"+",s.suffix);
        }
        IEnumerator OpenEveryKey()
        {
            if(revealing || database==null || balance==null)yield break;
            int count=GameProgress.Keys;if(count<=0)yield break;
            revealing=true;openAll.interactable=false;
            // Debit, grants and pity commit together; failure leaves keys untouched.
            if(!GameProgress.TryOpenChest(count,database,balance,rng,out var got)){revealing=false;RefreshReviewUi();yield break;}
            // Rewards granted and saved before presentation; no per-result animation for huge batches.
            RefreshReviewUi();
            // ANIM-003: ruong rung, bat nap, shard bay — mot chuoi du mo bao nhieu chia.
            yield return ChestOpenReveal.Play(reviewRoot,count);
            bool dismissed=false;
            var overlay=ShowRelicResults(got,count,()=>dismissed=true);
            while(!dismissed)yield return null;
            Destroy(overlay);revealing=false;RefreshReviewUi();
        }
    }
}
