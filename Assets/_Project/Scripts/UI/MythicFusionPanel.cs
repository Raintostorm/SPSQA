using System.Linq;
using Project.Battle;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;
namespace Project.UI
{
    /// <summary>Preparation-only recipe browser. Uses the same live definitions as combat.</summary>
    public sealed class MythicFusionPanel:MonoBehaviour
    {
        BattleFlow flow;BattleBoard board;UnitSpawner spawner;GameObject modal;Button opener;Font font;Text counter;
        RectTransform readyTray,ownedTray;Button itemButton;UnitDefinition recipeSelection;UnitDefinition selectedDetail;BattleItemInventory inventory;int itemRevision=-1;bool itemOpen,recipeItems;int itemRecipe;float nextPoll;int signature=int.MinValue;float listPosition=0;
        public static void Install(BattleHud hud)
        {if(!hud.GetComponent<MythicFusionPanel>())hud.gameObject.AddComponent<MythicFusionPanel>();}
        void Start()
        {
            inventory=GetComponent<BattleItemInventory>();if(!inventory)inventory=gameObject.AddComponent<BattleItemInventory>();
            flow=FindFirstObjectByType<BattleFlow>();spawner=FindFirstObjectByType<UnitSpawner>();
            var boards=FindObjectsByType<BattleBoard>(FindObjectsSortMode.None);board=boards.FirstOrDefault(b=>b.name.ToLowerInvariant().Contains("ally"));
            font=UiFonts.Display;
            opener=Button(transform,"Mythic",new Vector2(-400,150),new Vector2(220,180),Open);opener.GetComponent<RectTransform>().anchorMin=opener.GetComponent<RectTransform>().anchorMax=new Vector2(.5f,0);
            counter=opener.GetComponentInChildren<Text>();counter.text="";BattleChrome.Button(opener,0);
            itemButton=Button(transform,"Item",new Vector2(400,150),new Vector2(220,180),OpenItems);itemButton.GetComponent<RectTransform>().anchorMin=itemButton.GetComponent<RectTransform>().anchorMax=new Vector2(.5f,0);
            itemButton.GetComponentInChildren<Text>().text="";BattleChrome.Button(itemButton,2);
        }
        void Update()
        {
            if(itemOpen&&modal&&inventory&&itemRevision!=inventory.Revision)OpenItems();
            if(!opener)return;bool prep=flow&&flow.Current==BattleFlow.Phase.Prep;opener.gameObject.SetActive(prep);itemButton.gameObject.SetActive(prep);if(ownedTray){ownedTray.gameObject.SetActive(prep);ownedTray.anchoredPosition=new Vector2(0,itemOpen&&modal?870:650);}
            if(!prep){if(readyTray)readyTray.gameObject.SetActive(false);if(modal){Destroy(modal);selectedDetail=null;}signature=int.MinValue;return;}
            if(Time.unscaledTime<nextPoll)return;nextPoll=Time.unscaledTime+.25f;
            int state=BoardSignature();if(state==signature)return;signature=state;
            RefreshReady();RefreshOwned();if(modal&&!itemOpen){if(selectedDetail)ShowDetail(selectedDetail);else Open();}
        }
        int BoardSignature()
        {
            unchecked{int hash=MythicFusion.Limit*31+MythicFusion.Count+(inventory?inventory.Revision*997:0);
                if(board)for(int r=0;r<board.Grid.Rows;r++)for(int c=0;c<board.Grid.Cols;c++)foreach(var u in board.UnitsAt(r,c))if(u)hash=hash*31+u.GetInstanceID()+(u.IsAlive?1:0);
                foreach(float value in new[]{RunBuffs.HpPct,RunBuffs.DmgPct,RunBuffs.AtkSpeedPct,RunBuffs.CritChance,RunBuffs.ArmorPoints,RunBuffs.CritMultiplier})hash=hash*31+value.GetHashCode();return hash;}
        }
        void RefreshReady()
        {
            if(readyTray){readyTray.gameObject.SetActive(false);Destroy(readyTray.gameObject);}
            var ready=MythicCatalog.All.Where(d=>MythicFusion.MatchIngredients(board,d,out _,out _)).ToArray();
            if(ready.Length==0)return;
            readyTray=Rect(transform,"Ready Mythics",new Vector2(-105,-850),new Vector2(175,510));readyTray.anchorMin=readyTray.anchorMax=new Vector2(1,1);
            readyTray.gameObject.AddComponent<RectMask2D>();var scroll=readyTray.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.vertical=true;scroll.viewport=readyTray;
            var content=Rect(readyTray,"Summon controls",Vector2.zero,new Vector2(175,Mathf.Max(510,ready.Length*170)));content.anchorMin=content.anchorMax=content.pivot=new Vector2(.5f,1);content.anchoredPosition=Vector2.zero;scroll.content=content;
            for(int i=0;i<ready.Length;i++)
            {
                var output=ready[i];bool canFuse=MythicFusion.CanFuse(board,output,out _,out var reason);
                var card=Button(content,"",new Vector2(0,-85-i*170),new Vector2(165,155),()=>{if(MythicFusion.Fuse(board,output,spawner))signature=int.MinValue;});card.GetComponent<RectTransform>().anchorMin=card.GetComponent<RectTransform>().anchorMax=new Vector2(.5f,1);card.interactable=canFuse;
                BattleUiSkin.Skin(card.GetComponent<Image>(),BattleUiSkin.Gold);
                var portrait=Rect(card.transform,"Portrait",new Vector2(0,20),new Vector2(105,105)).gameObject.AddComponent<Image>();portrait.sprite=output.Portrait;portrait.preserveAspect=true;portrait.raycastTarget=false;
                Label(card.transform,canFuse?"Summon!":"Limit reached",new Vector2(0,-53),new Vector2(155,40),27);
            }
            if(modal)modal.transform.SetAsLastSibling();
        }
        RectTransform Rect(Transform p,string n,Vector2 pos,Vector2 size)
        {var r=new GameObject(n,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(p,false);r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.sizeDelta=size;r.anchoredPosition=pos;return r;}
        Text Label(Transform p,string text,Vector2 pos,Vector2 size,int fontsize)
        {var t=Rect(p,text,pos,size).gameObject.AddComponent<Text>();t.font=font;t.text=text;t.fontSize=fontsize;t.fontStyle=FontStyle.Bold;t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;return t;}
        Button Button(Transform p,string title,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction action)
        {var r=Rect(p,title,pos,size);var im=r.gameObject.AddComponent<Image>();BattleUiSkin.Skin(im,BattleUiSkin.Navy);var b=r.gameObject.AddComponent<Button>();ButtonFamilySkin.States(b);b.onClick.AddListener(action);Label(r,title,Vector2.zero,size,32);return b;}
        UnitDefinition Ingredient(string id)
        {var pool=FindFirstObjectByType<Project.Gacha.UnitPool>();if(pool)foreach(Rank rank in new[]{Rank.Common,Rank.Rare,Rank.Epic,Rank.Legendary})foreach(var d in pool.Of(rank))if(d.UnitId==id)return d;return null;}
        int Available(string id){int n=0;for(int r=0;r<board.Grid.Rows;r++)for(int c=0;c<board.Grid.Cols;c++)foreach(var u in board.UnitsAt(r,c))if(u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Definition.UnitId==id)n++;return n;}
        void IngredientCards(Transform row,MythicFusion.Recipe recipe)
        {
            for(int i=0;i<recipe.ingredients.Length;i++)
            {
                string id=recipe.ingredients[i];var def=Ingredient(id);
                int occurrence=recipe.ingredients.Take(i+1).Count(x=>x==id);
                bool ready=Available(id)>=occurrence;
                var chip=Rect(row,"Ingredient "+i,new Vector2((i-1)*325,0),new Vector2(290,350));
                BattleUiSkin.Skin(chip.gameObject.AddComponent<Image>(),BattleUiSkin.Dark);
                var crop=Rect(chip,"Portrait crop",new Vector2(0,65),new Vector2(250,155));crop.gameObject.AddComponent<RectMask2D>();
                var portrait=Rect(crop,"Portrait",new Vector2(0,-20),new Vector2(215,230)).gameObject.AddComponent<Image>();
                portrait.sprite=def?(def.Portrait?def.Portrait:def.Animations?.Idle?.FrameAt(0)):null;portrait.preserveAspect=true;portrait.raycastTarget=false;
                var name=Label(chip,def?def.DisplayName:id,new Vector2(0,-43),new Vector2(278,76),34);name.resizeTextForBestFit=true;name.resizeTextMinSize=28;name.resizeTextMaxSize=34;
                Label(chip,def?def.Rank.ToString():"",new Vector2(0,-102),new Vector2(260,42),29).color=def&&def.Rank==Rank.Epic?new Color(.8f,.5f,1):new Color(.7f,.78f,.86f);
                Label(chip,ready?"1/1":"0/1",new Vector2(0,-145),new Vector2(260,50),42).color=ready?new Color(.4f,1,.65f):new Color(1,.55f,.5f);
                if(i<2)Label(row,"+",new Vector2((i-.5f)*325,15),new Vector2(35,65),42);
            }
        }

        static string SkillSummary(MythicKind kind)=>kind switch
        {
            MythicKind.Guardian=>"Melee · Aura damage + self-heal",
            MythicKind.Undead=>"Melee · Resurrection passive",
            MythicKind.Elf=>"Ranged · Team ATK + attack speed",
            MythicKind.Bear=>"Melee · Empowered chain lightning",
            MythicKind.Robot=>"Melee · Nearby ally shields",
            MythicKind.Clock=>"Ranged · Stop enemies for 5s",
            MythicKind.Mimic=>"Melee · Devour + next-wave coins",
            _=>"Melee · Dash through enemies"
        };
        void RefreshOwned()
        {
            if(ownedTray){ownedTray.gameObject.SetActive(false);Destroy(ownedTray.gameObject);}
            ownedTray=Rect(transform,"Owned Mythics",new Vector2(0,650),new Vector2(1000,175));ownedTray.anchorMin=ownedTray.anchorMax=new Vector2(.5f,0);
            var units=UnitRegistry.All.Where(u=>u&&u.Team==UnitTeam.Ally&&u.Definition&&u.Definition.Rank==Rank.Mythic).ToArray();
            // Deliberately leave this reserved area empty until a Mythic is summoned.
            for(int i=0;i<units.Length;i++)
            {var def=units[i].Definition;var slot=Button(ownedTray,"",new Vector2((i-(units.Length-1)*.5f)*155,0),new Vector2(140,150),()=>ShowDetail(def));var art=Rect(slot.transform,"Mythic portrait",new Vector2(0,10),new Vector2(110,110)).gameObject.AddComponent<Image>();art.sprite=def.Portrait;art.preserveAspect=true;art.raycastTarget=false;
                var gear=units[i].Equipment;for(int g=0;g<gear.Types.Count;g++){var badge=Rect(slot.transform,"Equipped item",new Vector2(-45+(g%3)*45,-48-(g/3)*32),new Vector2(42,42)).gameObject.AddComponent<Image>();badge.sprite=BattleItemRecipes.Icon(gear.Types[g]);badge.preserveAspect=true;badge.raycastTarget=false;}
            }
        }
        void OpenItems()
        {
            itemOpen=true;
            if(modal){modal.SetActive(false);Destroy(modal);}selectedDetail=null;
            itemRevision=inventory?inventory.Revision:0;
            var root=Rect(transform,"Item inventory",new Vector2(0,610),new Vector2(1040,350));root.anchorMin=root.anchorMax=new Vector2(.5f,0);modal=root.gameObject;BattleChrome.Frame(root.gameObject.AddComponent<Image>());
            BattleChrome.Draw(root,2,new Vector2(-460,135),new Vector2(48,48));
            Label(root,"ITEMS",new Vector2(-325,135),new Vector2(180,60),32);
            Label(root,"Drag onto a Mythic to equip",new Vector2(60,135),new Vector2(570,60),27);
            var close=Button(root,"",new Vector2(450,135),new Vector2(115,115),()=>{itemOpen=false;Destroy(modal);});close.GetComponent<Image>().color=Color.clear;BattleChrome.Draw(close.transform,4,Vector2.zero,new Vector2(100,100));
            var available=inventory?inventory.Items.Where(item=>!item.EquippedTo).ToArray():System.Array.Empty<BattleItemInventory.Item>();
            if(available.Length==0)Label(root,"No items yet · Earn items from waves",new Vector2(0,235),new Vector2(980,65),32);
            for(int i=0;i<10;i++)
            {
                var cell=Rect(root,"Inventory slot "+i,new Vector2(-400+i%5*200,55-i/5*135),new Vector2(180,120));BattleChrome.Cell(cell.gameObject.AddComponent<Image>());
                if(i>=available.Length)continue;
                var item=available[i];ItemRecipeIcon(cell,item.Type,new Vector2(0,7),new Vector2(122,110));Label(cell,"1",new Vector2(62,-38),new Vector2(35,35),25);
                var drag=cell.gameObject.AddComponent<BattleItemDrag>();drag.Inventory=inventory;drag.Id=item.InstanceId;drag.Changed=OpenItems;drag.Inspect=()=>{foreach(Transform sibling in root)if(sibling.name.StartsWith("Inventory slot"))BattleChrome.Cell(sibling.GetComponent<Image>());BattleUiSkin.Skin(cell.GetComponent<Image>(),BattleUiSkin.Blue);ShowItemEffect(root,item.Type);};
            }

        }
        void ShowItemEffect(RectTransform parent,int type)
        {
            var old=parent.Find("Item effect");if(old){old.gameObject.SetActive(false);Destroy(old.gameObject);}
            var tip=Rect(parent,"Item effect",new Vector2(0,280),new Vector2(1040,185));var background=tip.gameObject.AddComponent<Image>();BattleChrome.Frame(background);background.raycastTarget=false;
            ItemRecipeIcon(tip,type,new Vector2(-435,0),new Vector2(100,100));
            Label(tip,BattleItemRecipes.Items[type].Name,new Vector2(50,55),new Vector2(810,55),36);
            Label(tip,BattleItemRecipes.Effect(type),new Vector2(50,-30),new Vector2(810,105),30).color=new Color(.6f,1f,.75f);
        }
        void Open()
        {
            itemOpen=false;
            selectedDetail=null;
            if(!board||!flow||flow.Current!=BattleFlow.Phase.Prep)return;if(modal){modal.SetActive(false);Destroy(modal);}
            if(!recipeSelection)recipeSelection=MythicCatalog.All.First();
            var blocker=Rect(transform,"Recipe overlay",Vector2.zero,Vector2.zero);blocker.anchorMin=Vector2.zero;blocker.anchorMax=Vector2.one;blocker.offsetMin=blocker.offsetMax=Vector2.zero;blocker.gameObject.AddComponent<Image>().color=new Color(0,0,0,.08f);modal=blocker.gameObject;
            var root=Rect(blocker,"Recipe sheet",new Vector2(0,recipeItems?770:960),new Vector2(1080,recipeItems?880:1260));root.anchorMin=root.anchorMax=new Vector2(.5f,0);BattleChrome.Frame(root.gameObject.AddComponent<Image>());
            // Flat inner surface separates information from clickable actions.
            var surface=Rect(root,"Navy surface",Vector2.zero,root.sizeDelta-new Vector2(40,40));surface.gameObject.AddComponent<Image>().color=new Color32(14,37,58,255);
            float headerY=recipeItems?365:550;
            var tabAllies=Button(root,"Mythic Allies",new Vector2(-300,headerY),new Vector2(400,100),()=>{recipeItems=false;Open();});
            var tabItems=Button(root,"Item",new Vector2(145,headerY),new Vector2(400,100),()=>{recipeItems=true;Open();});
            foreach(var tab in new[]{tabAllies,tabItems})
            {
                BattleUiSkin.Skin(tab.GetComponent<Image>(),BattleUiSkin.Dark);
                var label=tab.GetComponentInChildren<Text>();label.fontSize=38;label.rectTransform.anchoredPosition=new Vector2(30,0);label.rectTransform.sizeDelta=new Vector2(325,90);
                var icon=Rect(tab.transform,"Tab icon",new Vector2(-155,0),new Vector2(60,60)).gameObject.AddComponent<Image>();icon.sprite=ApprovedUiArt.Get(tab==tabAllies?"recipe":"battle");icon.preserveAspect=true;icon.raycastTarget=false;
            }
            var active=recipeItems?tabItems:tabAllies;
            Rect(active.transform,"Active underline",new Vector2(0,-47),new Vector2(350,5)).gameObject.AddComponent<Image>().color=new Color(.05f,.85f,1);
            var close=Button(root,"",new Vector2(440,headerY),new Vector2(125,125),()=>Destroy(modal));ApprovedUiArt.Apply(close.GetComponent<Image>(),"close");
            if(recipeItems){DrawItemRecipe(root);return;}
            var def=recipeSelection;
            var selectedPortrait=Rect(root,"Selected Mythic",new Vector2(-365,340),new Vector2(230,220)).gameObject.AddComponent<Image>();selectedPortrait.sprite=def.Portrait;selectedPortrait.preserveAspect=true;selectedPortrait.raycastTarget=false;
            var title=Label(root,def.DisplayName,new Vector2(105,395),new Vector2(650,90),48);title.resizeTextForBestFit=true;title.resizeTextMinSize=38;title.resizeTextMaxSize=48;
            Label(root,SkillSummary(def.Mythic),new Vector2(105,305),new Vector2(650,90),32).color=new Color(.72f,.83f,.94f);
            var recipe=MythicFusion.For(def);if(recipe?.ingredients?.Length==3){var holder=Rect(root,"Ingredients",new Vector2(0,40),new Vector2(990,350));IngredientCards(holder,recipe);}
            bool can=MythicFusion.CanFuse(board,def,out _,out var reason);
            var summon=Button(root,"Summon",new Vector2(0,-210),new Vector2(940,115),()=>{if(MythicFusion.Fuse(board,def,spawner)){signature=int.MinValue;Open();}});
            ScreenChrome.Action(summon,ScreenChrome.Role.Primary);summon.interactable=can;summon.GetComponentInChildren<Text>().fontSize=48;
            Label(root,can?"Ready to summon":reason,new Vector2(0,-300),new Vector2(980,55),34).color=can?new Color(.4f,1,.65f):new Color(1,.55f,.5f);
            Label(root,"Mythic "+MythicFusion.Count+" / "+MythicFusion.Limit,new Vector2(0,-370),new Vector2(800,50),32);
            var viewport=Rect(root,"Mythic list",new Vector2(0,-505),new Vector2(1000,200));viewport.gameObject.AddComponent<RectMask2D>();viewport.gameObject.AddComponent<Image>().color=new Color(0,0,0,.02f);
            var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.horizontal=true;scroll.vertical=false;scroll.viewport=viewport;scroll.movementType=ScrollRect.MovementType.Clamped;
            float savedPosition=listPosition;
            var all=MythicCatalog.All.ToArray();var content=Rect(viewport,"Portraits",Vector2.zero,new Vector2(all.Length*180,190));content.anchorMin=content.anchorMax=content.pivot=new Vector2(0,.5f);scroll.content=content;
            for(int i=0;i<all.Length;i++)
            {var choice=all[i];var button=Button(content,"",new Vector2(90+i*180,0),new Vector2(165,180),()=>{recipeSelection=choice;Open();});button.GetComponent<RectTransform>().anchorMin=button.GetComponent<RectTransform>().anchorMax=new Vector2(0,.5f);BattleUiSkin.Skin(button.GetComponent<Image>(),choice==def?BattleUiSkin.Blue:BattleUiSkin.Dark);
                var art=Rect(button.transform,"Portrait",new Vector2(0,20),new Vector2(115,115)).gameObject.AddComponent<Image>();art.sprite=choice.Portrait;art.preserveAspect=true;art.raycastTarget=false;
                int have=0;var needed=MythicFusion.For(choice);if(needed!=null)foreach(var group in needed.ingredients.GroupBy(x=>x))have+=Mathf.Min(group.Count(),Available(group.Key));
                Label(button.transform,have+" / 3",new Vector2(0,-62),new Vector2(155,45),32).color=have==3?new Color(.4f,1,.65f):new Color(1,.65f,.6f);
            }
            var next=Button(root,"›",new Vector2(485,-505),new Vector2(65,100),()=>scroll.horizontalNormalizedPosition=Mathf.Clamp01(scroll.horizontalNormalizedPosition+.35f));next.GetComponentInChildren<Text>().fontSize=60;var nextGlyph=Resources.Load<Sprite>("Glyphs/next");if(nextGlyph){next.GetComponentInChildren<Text>().enabled=false;var g=new GameObject("Glyph",typeof(RectTransform),typeof(Image)).GetComponent<Image>();g.transform.SetParent(next.transform,false);g.sprite=nextGlyph;g.preserveAspect=true;g.raycastTarget=false;g.rectTransform.sizeDelta=new Vector2(60,60);}
            Canvas.ForceUpdateCanvases();scroll.horizontalNormalizedPosition=savedPosition;next.interactable=savedPosition<.999f;
            scroll.onValueChanged.AddListener(v=>{listPosition=v.x;next.interactable=v.x<.999f;});
        }
        void ItemRecipeIcon(Transform parent,int index,Vector2 position,Vector2 size)
        {
            var image=Rect(parent,BattleItemRecipes.Items[index].Name,position,size).gameObject.AddComponent<Image>();
            image.sprite=BattleItemRecipes.Icon(index);image.preserveAspect=true;image.raycastTarget=false;
        }
        void DrawItemRecipe(RectTransform root)
        {
            var recipe=BattleItemRecipes.Recipes[itemRecipe];
            Label(root,BattleItemRecipes.Items[recipe[2]].Name,new Vector2(-30,255),new Vector2(850,65),42);
            for(int i=0;i<3;i++)
            {
                float x=(i-1)*340;var cell=Rect(root,"Recipe item",new Vector2(x,100),new Vector2(265,230));BattleUiSkin.Skin(cell.gameObject.AddComponent<Image>(),i==2?BattleUiSkin.Blue:BattleUiSkin.Navy);
                ItemRecipeIcon(cell,recipe[i],new Vector2(0,35),new Vector2(160,160));
                Label(cell,BattleItemRecipes.Items[recipe[i]].Name,new Vector2(0,-78),new Vector2(255,65),27);
            }
            Label(root,"+",new Vector2(-170,100),new Vector2(55,65),46);Label(root,"=",new Vector2(170,100),new Vector2(55,65),46);
            var effect=BattleItemRecipes.Effect(recipe[2]).Split('\n');
            if(effect.Length>1){Label(root,"On kill",new Vector2(0,-25),new Vector2(950,50),32).color=new Color(1,.82f,.4f);Label(root,effect[1].Replace("On kill: ",""),new Vector2(0,-85),new Vector2(950,75),32);Label(root,effect[0],new Vector2(0,-165),new Vector2(970,65),29).color=new Color(.6f,1,.78f);}
            else Label(root,effect[0],new Vector2(0,-80),new Vector2(950,130),36).color=new Color(.6f,1,.78f);
            for(int i=0;i<3;i++)
            {int selected=i;var button=Button(root,"",new Vector2((i-1)*320,-325),new Vector2(300,170),()=>{itemRecipe=selected;Open();});BattleUiSkin.Skin(button.GetComponent<Image>(),i==itemRecipe?BattleUiSkin.Blue:BattleUiSkin.Navy);
                ItemRecipeIcon(button.transform,BattleItemRecipes.Recipes[i][2],new Vector2(0,30),new Vector2(110,110));Label(button.transform,BattleItemRecipes.Items[BattleItemRecipes.Recipes[i][2]].Name,new Vector2(0,-56),new Vector2(285,45),28);
            }
            Label(root,"Mythics only",new Vector2(0,-210),new Vector2(950,45),27).color=new Color(.65f,.8f,.88f);
        }

        void ShowDetail(UnitDefinition def)
        {
            var unit=UnitRegistry.All.FirstOrDefault(u=>u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Definition==def);
            if(!unit)return;
            itemOpen=false;selectedDetail=null;
            if(modal){modal.SetActive(false);Destroy(modal);}
            BattleUnitInfo.Show(unit);
        }
    }
}
