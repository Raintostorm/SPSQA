using System;
using System.Linq;
using Project.Data;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Runtime uGUI counterpart of the approved stage demo. No baked UI text.
    public sealed class AllyUpgradeView : MonoBehaviour
    {
        static readonly string[] Factions = { "human", "elf", "dead", "monster", "machine" };
        static readonly Color[] RarityColors = {
            new Color(.42f,.38f,.35f), new Color(.03f,.49f,.69f),
            new Color(.50f,.12f,.73f), new Color(.88f,.62f,.08f), new Color(.85f,.32f,.08f) };
        UnitDefinition[] units;
        GameBalance balance;
        TreasureDatabase treasures;
        Texture2D[] stages;
        Font font;
        RectTransform page, host, stage;
        RawImage background;
        RectTransform rarityArrow,levelArrow;
        static RectTransform SortArrow(Transform parent)
        {
            var sprite=Resources.Load<Sprite>("Glyphs/sort");if(!sprite)return null;
            var image=new GameObject("SortArrow",typeof(RectTransform),typeof(Image)).GetComponent<Image>();image.transform.SetParent(parent,false);
            image.sprite=sprite;image.preserveAspect=true;image.raycastTarget=false;image.rectTransform.sizeDelta=new Vector2(56,56);image.rectTransform.anchoredPosition=new Vector2(150,0);
            return image.rectTransform;
        }
        Image actor, progress, upgradeGoldIcon;
        Text goldValue, gemValue, stoneValue, heading, title, badge, level, shards, sources, cost;
        readonly Text[] statValues = new Text[8];
        readonly Image[] statBoxes = new Image[8];
        GameObject mercenariesPanel, detailPanel, previous, next;
        RectTransform rosterContent;
        bool detail;
        bool rarityAscending=true, levelAscending;
        Text raritySort, levelSort;
        Button upgrade;
        Button[] cards;
        Text[] cardLevels, cardShards, cardReady;
        const float TileW=300, TileH=334, RowStep=350, ShardW=260;
        Image[] cardShardFill, cardReadyBadge;
        GameObject infoPopup;
        int selected, step, direction = 1;
        float timer, pixelsPerUnit;
        UnitAnimationSet.Clip clip;

        public static bool Install(Transform parent, UnitDefinition[] roster, GameBalance balance, TreasureDatabase treasures)
        {
            var textures = Factions.Select(r => Resources.Load<Texture2D>("AllyStages/" + r)).ToArray();
            if (textures.Any(t => !t) || roster == null || !roster.Any(u => u)) return false;
            foreach (Transform child in parent) child.gameObject.SetActive(false);
            var go = new GameObject("AlliesStageUI", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var view = go.AddComponent<AllyUpgradeView>();
            view.host = go.GetComponent<RectTransform>();
            view.host.anchorMin = Vector2.zero; view.host.anchorMax = Vector2.one;
            view.host.offsetMin = view.host.offsetMax = Vector2.zero;
            view.units = roster.Where(u => u).Concat(MythicCatalog.All).GroupBy(u=>u.UnitId).Select(g=>g.First()).OrderBy(u => u.Rank).ThenBy(u => u.Race).ToArray();
            view.balance = balance; view.treasures = treasures; view.stages = textures;
            view.font = UiFonts.Display;
            view.Build(); view.Select(0); return true;
        }
        RectTransform Rect(Transform parent, string name, float x, float y, float w, float h)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            r.SetParent(parent, false); r.anchorMin = r.anchorMax = r.pivot = new Vector2(.5f, 1);
            r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w,h); return r;
        }
        Image Box(Transform parent, string name, float x, float y, float w, float h, Color color)
        {
            var im = Rect(parent,name,x,y,w,h).gameObject.AddComponent<Image>(); im.color=color; return im;
        }
        Text Label(Transform parent,string text,float x,float y,float w,float h,int size,TextAnchor align=TextAnchor.MiddleCenter)
        {
            var t = Rect(parent,"Label",x,y,w,h).gameObject.AddComponent<Text>();
            t.text=text;t.font=font;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.color=Color.white;
            t.alignment=align;t.raycastTarget=false;
            var o=t.gameObject.AddComponent<Outline>();o.effectColor=new Color(0,0,0,.65f);o.effectDistance=new Vector2(1.5f,-1.5f);
            return t;
        }
        Button Button(Transform parent,string name,float x,float y,float w,float h,Sprite skin,Action click)
        {
            var im=Box(parent,name,x,y,w,h,Color.white);BattleUiSkin.Skin(im,skin);
            var b=im.gameObject.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(()=>click());
            var colors=b.colors;colors.disabledColor=new Color(.45f,.45f,.45f);b.colors=colors;ButtonFamilySkin.States(b);return b;
        }
        void Build()
        {
            var fill=Box(transform,"Backdrop",0,0,1,1,new Color(.025f,.055f,.095f));
            fill.rectTransform.anchorMin=Vector2.zero;fill.rectTransform.anchorMax=Vector2.one;
            fill.rectTransform.offsetMin=fill.rectTransform.offsetMax=Vector2.zero;
            page=Rect(transform,"FittedPage",0,0,1040,2200);page.anchorMin=page.anchorMax=page.pivot=Vector2.one*.5f;
            page.anchoredPosition=Vector2.zero;
            Box(page,"Page",0,0,1040,2200,new Color(.035f,.075f,.13f));
            // Shared chrome (UI-005): back top-left (wallets shift right), ribbon title. Rect() here is
            // top-edge based (wallets 20..110, stage from 230); ScreenChrome works from the page centre.
            // The ribbon gets the 115..225 gap, so it is 110 tall instead of 150 to clear the wallets.
            goldValue=Wallet(-255,"HomeChrome/coin");gemValue=Wallet(40,"Summon/gem");stoneValue=Wallet(335,"MythicStone");
            ScreenChrome.Back(page,new Vector2(-465,1100-65),()=>{if(detail)ShowDetail(false);else SceneRoutes.GoHome();});
            heading=ScreenChrome.Title(page,"MERCENARIES",new Vector2(0,1100-170),640,ScreenChrome.Banner.Screen,110);
            stage=Rect(page,"FactionStage",0,230,1040,693);
            stage.gameObject.AddComponent<RectMask2D>();
            background=Rect(stage,"Background",0,0,1040,693).gameObject.AddComponent<RawImage>();
            background.raycastTarget=false;
            var shadow=Box(stage,"Shadow",0,528,120,16,new Color(0,0,0,.18f));shadow.raycastTarget=false;
            actor=Rect(stage,"IdleAlly",0,465,240,240).gameObject.AddComponent<Image>();actor.raycastTarget=false;
            badge=Label(stage,"",0,18,950,65,30);
            // Nut tron chung (chat 012) thay cho o vuong xanh + chu "<" ">".
            var prev=ScreenChrome.Round(stage,"back","blue",Vector2.zero,120,()=>Select((selected+units.Length-1)%units.Length));
            // Man nay cang tu MEP TREN (Rect() lat dau y), con Round() cang tu giua,
            // nen phai neo lai bang PinTop neu khong nut se nhay len giua anh.
            ScreenChrome.PinTop(prev,-445,300);previous=prev.gameObject;
            var nxt=ScreenChrome.Round(stage,"next","blue",Vector2.zero,120,()=>Select((selected+1)%units.Length));
            ScreenChrome.PinTop(nxt,445,300);next=nxt.gameObject;
            title=Label(page,"",0,940,960,80,46);
            level=Label(page,"",-335,1030,300,60,34);
            var shardTrack=Box(page,"ShardTrack",0,1030,960,70,Color.white);BattleUiSkin.Skin(shardTrack,BattleUiSkin.Navy);
            progress=Box(page,"ShardProgress",0,1030,960,70,Color.white);BattleUiSkin.Skin(progress,BattleUiSkin.Green);
            progress.rectTransform.pivot=new Vector2(0,1);progress.rectTransform.anchoredPosition=new Vector2(-480,-1030);
            shards=Label(page,"",0,1030,920,70,40);
            var list=Rect(page,"MercenariesScreen",0,0,1040,2200);mercenariesPanel=list.gameObject;
            
            var byRarity=ScreenChrome.Action(Button(list,"SortRarity",-250,915,470,95,BattleUiSkin.Navy,()=>{rarityAscending=!rarityAscending;SortRoster(false);}),ScreenChrome.Role.Cancel);
            raritySort=Label(byRarity.transform,"Rarity",-30,0,400,95,38);rarityArrow=SortArrow(byRarity.transform);
            var byLevel=ScreenChrome.Action(Button(list,"SortLevel",250,915,470,95,BattleUiSkin.Navy,()=>{levelAscending=!levelAscending;SortRoster(true);}),ScreenChrome.Role.Cancel);
            levelSort=Label(byLevel.transform,"Level",-30,0,400,95,38);levelArrow=SortArrow(byLevel.transform);
            var details=Rect(page,"UpgradeScreen",0,0,1040,2200);detailPanel=details.gameObject;
            string[] names={"HP","MP","ATK","ATK SPD","Critical Chance","CRIT DMG","Damage Reduction","Range"};
            string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};
            for(int i=0;i<8;i++)statValues[i]=Stat(details,names[i],icons[i],i%2==0?-245:245,1115+i/2*145,i);
            sources=Label(details,"",-65,1705,760,110,32);
            var info=Button(details,"StatInfo",430,1705,120,120,BattleUiSkin.Info,ShowStatInfo);Label(info.transform,"i",0,0,120,120,44);
            upgrade=ScreenChrome.Action(Button(details,"Upgrade",0,1835,960,160,BattleUiSkin.Gold,Buy),ScreenChrome.Role.Primary);
            upgradeGoldIcon=Rect(upgrade.transform,"GoldIcon",-145,35,90,90).gameObject.AddComponent<Image>();
            upgradeGoldIcon.sprite=Resources.Load<Sprite>("HomeChrome/coin");
            upgradeGoldIcon.preserveAspect=true;upgradeGoldIcon.raycastTarget=false;
            cost=Label(upgrade.transform,"",70,30,350,100,64);
            // The bottom Back bar is gone, so the roster gets that space.
            var viewport=Rect(list,"RosterViewport",0,1035,1000,1135);
            Box(viewport,"ScrollHitArea",0,0,1000,1135,new Color(.025f,.055f,.095f));
            viewport.gameObject.AddComponent<RectMask2D>();
            int ordinary=units.Count(u=>u.Rank!=Rank.Mythic);int mythics=units.Length-ordinary;
            float mythicTop=Mathf.Ceil(ordinary/3f)*RowStep;
            var content=Rect(viewport,"Roster",0,0,1000,mythicTop+(mythics>0?100+Mathf.Ceil(mythics/3f)*RowStep:0));
            if(mythics>0)Label(content,"MYTHIC · Shared Mythic Stones",0,mythicTop,960,90,36);rosterContent=content;
            var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.viewport=viewport;scroll.content=content;
            CollectionScrollHint.Install(scroll);
            scroll.horizontal=false;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=60;
            cards=new Button[units.Length];cardLevels=new Text[units.Length];cardShards=new Text[units.Length];cardReady=new Text[units.Length];cardShardFill=new Image[units.Length];cardReadyBadge=new Image[units.Length];
            for(int i=0;i<units.Length;i++)
            {
                int k=i;int slot=i<ordinary?i:i-ordinary;float x=(slot%3-1)*326, y=i<ordinary?slot/3*RowStep+8:mythicTop+100+slot/3*RowStep+8;
                // Roster tile (chat 006, 357x398): navy portrait window top 6–62%, info band 63–92%.
                var card=Button(content,"Ally "+units[i].UnitId,x,y,TileW,TileH,BattleUiSkin.Dark,()=>{Select(k);ShowDetail(true);});cards[i]=card;
                var portraitCrop=Rect(card.transform,"PortraitCrop",0,20,268,187);portraitCrop.gameObject.AddComponent<RectMask2D>();
                var head=Resources.Load<Sprite>("ShopV2/Heads/"+units[i].UnitId);
                SpriteFit.Place(portraitCrop,head?head:units[i].Animations?.Idle?.FrameAt(0) ?? units[i].Portrait,new Vector2(268,187),1,1,true);
                cardLevels[i]=Label(card.transform,"",-66,24,140,42,30,TextAnchor.MiddleLeft);
                cardReadyBadge[i]=Rect(card.transform,"UpgradeReady",112,4,62,62).gameObject.AddComponent<Image>();
                cardReadyBadge[i].sprite=Resources.Load<Sprite>("Badges/upgrade");cardReadyBadge[i].preserveAspect=true;cardReadyBadge[i].raycastTarget=false;
                Label(card.transform,units[i].DisplayName,0,216,270,40,26).resizeTextForBestFit=true;
                Box(card.transform,"ShardTrack",0,260,ShardW,36,new Color(.025f,.055f,.095f));
                cardShardFill[i]=Box(card.transform,"ShardFill",-ShardW/2,260,ShardW,36,new Color(.07f,.65f,.40f));
                cardShardFill[i].rectTransform.pivot=new Vector2(0,1);
                cardShards[i]=Label(card.transform,"",0,260,ShardW,36,30);
                // Text fallback only when the badge art is missing.
                cardReady[i]=Label(card.transform,"",65,7,175,42,27);cardReady[i].color=new Color(.5f,1,.65f);
            }
            ShowDetail(false);
        }
        Text Wallet(float x,string icon)
        {
            var box=Box(page,"Wallet",x,20,270,90,Color.white);BattleUiSkin.Skin(box,BattleUiSkin.Navy);
            var art=Rect(box.transform,"CurrencyIcon",-95,5,75,75).gameObject.AddComponent<Image>();
            art.sprite=icon=="MythicStone"?MythicStoneArt.Icon:Resources.Load<Sprite>(icon);art.preserveAspect=true;art.raycastTarget=false;
            return Label(box.transform,"",30,0,190,90,42);
        }
        Text Stat(Transform parent,string name,string iconName,float x,float y,int index)
        {
            var box=Box(parent,name,x,y,470,140,new Color(.10f,.16f,.25f));statBoxes[index]=box;
            var icon=Rect(box.transform,"StatIcon "+iconName,-185,25,85,85).gameObject.AddComponent<RawImage>();
            icon.texture=ApprovedUiArt.StatTexture(iconName);icon.raycastTarget=false;
            if(!icon.texture){icon.enabled=false;Debug.LogWarning("[AllyUpgradeView] Missing stat icon: "+iconName);}
            Label(box.transform,name,50,8,340,45,29);
            var value=Label(box.transform,"",50,53,340,80,46);value.supportRichText=true;return value;
        }
        void ShowDetail(bool on)
        {
            detail=on;mercenariesPanel.SetActive(!on);detailPanel.SetActive(on);
            previous.SetActive(on);next.SetActive(on);heading.text=on?"UPGRADE":"MERCENARIES";
            float stageHeight=on?600:520;
            stage.sizeDelta=new Vector2(1040,stageHeight);
            background.rectTransform.sizeDelta=new Vector2(1040,stageHeight);
            background.uvRect=on?new UnityEngine.Rect(0,0,1,1):new UnityEngine.Rect(0,0,1,520f/693);
            actor.rectTransform.anchoredPosition=new Vector2(0,on?-465:-410);
            stage.Find("Shadow").GetComponent<RectTransform>().anchoredPosition=new Vector2(0,on?-453:-398);
            title.rectTransform.anchoredPosition=new Vector2(0,on?-850:-770);
            level.rectTransform.anchoredPosition=new Vector2(0,on?-945:-850);
            level.rectTransform.sizeDelta=new Vector2(900,65);level.fontSize=40;
            foreach(string name in new[]{"ShardTrack","ShardProgress"})
            {
                var track=page.Find(name);track.gameObject.SetActive(on);
                track.GetComponent<RectTransform>().anchoredPosition=new Vector2(name=="ShardProgress"?-480:0,-1030);
            }
            shards.gameObject.SetActive(on);
            shards.rectTransform.anchoredPosition=new Vector2(0,-1030);
            Refresh();
        }
        void SortRoster(bool byLevel)
        {
            var indices=Enumerable.Range(0,units.Length);
            var ordered=byLevel
                ?indices.OrderBy(i=>(levelAscending?1:-1)*(SaveSystem.Current.FindUnit(units[i].UnitId)?.Level??1)).ThenBy(i=>units[i].Rank)
                :indices.OrderBy(i=>(rarityAscending?1:-1)*(int)units[i].Rank).ThenBy(i=>units[i].Race);
            int slot=0;foreach(int i in ordered.Where(i=>units[i].Rank!=Rank.Mythic)){cards[i].GetComponent<RectTransform>().anchoredPosition=new Vector2((slot%3-1)*326,-(slot/3*RowStep+8));slot++;}
            float start=Mathf.Ceil(slot/3f)*RowStep+100;slot=0;foreach(int i in ordered.Where(i=>units[i].Rank==Rank.Mythic)){cards[i].GetComponent<RectTransform>().anchoredPosition=new Vector2((slot%3-1)*326,-(start+slot/3*320+8));slot++;}
            rosterContent.anchoredPosition=Vector2.zero;
            // Mui ten la sprite Glyphs/sort (mot mui len), xoay 180 do cho chieu xuong; thieu sprite thi quay ve ky tu.
            if(rarityArrow){rarityArrow.localEulerAngles=new Vector3(0,0,rarityAscending?0:180);raritySort.text="Rarity";}else raritySort.text="Rarity "+(rarityAscending?"↑":"↓");
            if(levelArrow){levelArrow.localEulerAngles=new Vector3(0,0,levelAscending?0:180);levelSort.text="Level";}else levelSort.text="Level "+(levelAscending?"↑":"↓");
        }
        void Select(int index)
        {
            selected=index;step=0;direction=1;timer=0;
            var def=units[index];clip=def.Animations?.Idle;
            var first=clip!=null&&clip.IsValid?clip.FrameAt(0):def.Portrait;
            pixelsPerUnit=first?240f/(first.rect.height/first.pixelsPerUnit):240f;
            background.texture=stages[Mathf.Clamp((int)def.Race,0,stages.Length-1)];Refresh();Draw();
        }
        void Refresh()
        {
            var def=units[selected];var save=SaveSystem.Current;var row=save.FindUnit(def.UnitId);
            int lv=row?.Level??1, count=AllySummon.UpgradeMaterials(def.UnitId), max=balance?balance.UnitMaxLevel:30;
            bool top=lv>=max;int need=AllySummon.ShardsFor(lv),gold=(balance?balance.UpgradeBaseCost:500)*lv;
            goldValue.text=save.Gold.ToString("N0");gemValue.text=save.Gems.ToString("N0");stoneValue.text=save.MythicStones.ToString("N0");
            title.text=def.DisplayName;badge.text=def.Rank==Rank.Mythic?$"Mythic  ·  {(def.IsRanged?"Ranged":"Melee")}":$"{def.Race}  ·  {def.Rank}  ·  {(def.IsRanged?"Ranged":"Melee")}";
            level.text=detail&&!top?$"Lv.{lv} → {lv+1}":$"Lv.{lv} / {max}";string material=def.Rank==Rank.Mythic?"Mythic Stone":"Shards";
            shards.text=top?$"MAX · {count} {material}":$"{material}  {count} / {need}";
            progress.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,960*(top?1:Mathf.Clamp01((float)count/need)));
            var current=AllyStatPreview.Read(def,lv,balance,treasures);
            var future=AllyStatPreview.Read(def,top?lv:lv+1,balance,treasures);
            float[] now={current.Hp,current.Mana,current.Attack,current.AttackSpeed,current.CritChance,current.CritDamage,current.DamageReduction,current.Range};
            float[] later={future.Hp,future.Mana,future.Attack,future.AttackSpeed,future.CritChance,future.CritDamage,future.DamageReduction,future.Range};
            string[] formats={"N0","N0","N0","0.##","0.#%","0.#%","0.#%","0.##"};
            for(int i=0;i<8;i++)
            {
                // Round displayed values before subtraction so the shown next value is exact.
                int decimals=i<3?0:i>=4&&i<=6?3:2;
                double roundedNow=Math.Round(now[i],decimals,MidpointRounding.AwayFromZero);
                double roundedLater=Math.Round(later[i],decimals,MidpointRounding.AwayFromZero);
                bool increases=!top&&roundedLater>roundedNow;
                statValues[i].text=roundedNow.ToString(formats[i])+(i==3?"/s":"")+
                    (increases?$"  <color=#70FFC2>+{(roundedLater-roundedNow).ToString(formats[i])}</color>":"");
                statBoxes[i].color=increases?new Color(.08f,.37f,.25f):new Color(.10f,.16f,.25f);
            }
            sources.text=top?"MAX LEVEL":"Next level gains shown in green";
            sources.text+=top?"":count<need?$"\nNot enough {material} · {count} / {need}":save.Gold<gold?"\nNot enough gold":"\nReady to upgrade";
            upgradeGoldIcon.gameObject.SetActive(!top);
            cost.rectTransform.anchoredPosition=new Vector2(top?0:70,-30);
            cost.rectTransform.sizeDelta=new Vector2(top?900:350,100);
            cost.text=top?"MAX LEVEL":$"{gold:N0}";
            upgrade.interactable=!top&&save.Gold>=gold&&count>=need;
            for(int i=0;i<cards.Length;i++)
            {
                var owned=save.FindUnit(units[i].UnitId);int cardLevel=owned?.Level??1,cardCount=AllySummon.UpgradeMaterials(units[i].UnitId);
                int cardNeed=AllySummon.ShardsFor(cardLevel);bool cardMax=cardLevel>=max;
                cardLevels[i].text=$"Lv.{cardLevel}";
                cardShards[i].text=cardMax?"MAX":$"{cardCount} / {cardNeed}";
                cardShardFill[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,ShardW*(cardMax?1:Mathf.Clamp01((float)cardCount/cardNeed)));
                bool ready=!cardMax&&cardCount>=cardNeed&&save.Gold>=(balance?balance.UpgradeBaseCost:500)*cardLevel;
                bool badgeArt=cardReadyBadge[i].sprite;cardReadyBadge[i].enabled=ready&&badgeArt;
                cardReady[i].text=ready&&!badgeArt?"READY":"";
                var color=RarityColors[(int)units[i].Rank];var cardImage=cards[i].GetComponent<Image>();
                var tile=Resources.Load<Sprite>("RosterTilesV1/tile-"+units[i].Rank switch{Rank.Rare=>"rare",Rank.Epic=>"epic",Rank.Legendary=>"legendary",Rank.Mythic=>"mythic",_=>"common"});
                if(tile){cardImage.sprite=tile;cardImage.type=Image.Type.Simple;cardImage.color=Color.white;}
                else{BattleUiSkin.Skin(cardImage,BattleUiSkin.Neutral);cardImage.color=color;}
                // Selection: a cyan Outline on a framed sprite draws an offset copy, so lift the tile instead.
                var outline=cards[i].GetComponent<Outline>();if(tile&&outline)outline.enabled=false;
                else if(!tile){outline??=cards[i].gameObject.AddComponent<Outline>();outline.effectColor=i==selected?new Color(.10f,.85f,1f):new Color(.015f,.035f,.06f);outline.effectDistance=new Vector2(4,-4);}
                cards[i].transform.localScale=Vector3.one*(tile&&i==selected?1.05f:1f);
            }
        }
        void ShowStatInfo()
        {
            if(infoPopup)return;
            var dim=Box(page,"StatInfoPopup",0,0,1040,2200,new Color(0,0,0,.8f));infoPopup=dim.gameObject;
            var panel=Box(dim.transform,"InfoPanel",0,650,920,680,Color.white);BattleUiSkin.Skin(panel,BattleUiSkin.Navy);UiPop.Play(panel);
            Label(panel.transform,"STAT DETAILS",0,45,840,90,48);
            Label(panel.transform,"Permanent stats include relics.\nTemporary battle buffs are excluded.\n\nATK SPD: attacks per second,\nincluding animation and cooldown.\nGreen + values show next-level gains.",0,165,820,350,36);
            var close=ScreenChrome.Action(Button(panel.transform,"Close",0,525,800,120,BattleUiSkin.Blue,()=>{Destroy(infoPopup);infoPopup=null;}),ScreenChrome.Role.Cancel);Label(close.transform,"Close",0,0,780,120,42);
        }
        void Buy(){if(AllySummon.TryUpgrade(units[selected].UnitId,balance)){Refresh();UnitLevelUp.Play(actor);UiPulse.Play(level);}}
        void Update()
        {
            if(page)
            {
                var safe=Screen.safeArea;
                host.anchorMin=new Vector2(safe.xMin/Screen.width,safe.yMin/Screen.height);
                host.anchorMax=new Vector2(safe.xMax/Screen.width,safe.yMax/Screen.height);
                host.offsetMin=host.offsetMax=Vector2.zero;
                page.localScale=Vector3.one*Mathf.Min(host.rect.width/1040,host.rect.height/2220);
            }
            if(clip==null||!clip.IsValid)return;
            timer+=Time.unscaledDeltaTime;
            int guard=0;
            while(timer>=clip.DurationAt(step)&&guard++<240)
            {
                timer-=clip.DurationAt(step);
                bool ping=clip.UseExplicitPlayback?clip.PingPong:true;
                if(ping&&clip.Count>1){step+=direction;if(step>=clip.Count){step=clip.Count-2;direction=-1;}else if(step<0){step=1;direction=1;}}
                else step=(step+1)%clip.Count;
            }
            Draw();
        }
        void Draw()
        {
            var sprite=clip!=null&&clip.IsValid?clip.FrameAt(step):units[selected].Portrait;
            actor.sprite=sprite;actor.enabled=sprite;
            if(!sprite)return;
            // Image uses each frame's actual pivot and PPU, not changing bounding-box centering.
            actor.rectTransform.pivot=new Vector2(sprite.pivot.x/sprite.rect.width,sprite.pivot.y/sprite.rect.height);
            actor.rectTransform.sizeDelta=sprite.rect.size/sprite.pixelsPerUnit*pixelsPerUnit;
        }
    }
}
