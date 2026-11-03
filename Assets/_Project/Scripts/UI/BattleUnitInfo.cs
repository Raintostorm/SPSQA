using System.Linq;
using Project.Battle;
using Project.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>One live inspector for ordinary allies, Legendary and Mythic units.</summary>
    public sealed class BattleUnitInfo : MonoBehaviour
    {
        static BattleUnitInfo current;
        Unit unit; BattleFlow flow; Font font;
        readonly Text[] numbers=new Text[8];
        Text manaLabel; Image manaFill; float nextRefresh; int equipmentCount=-1; RectTransform equipment;
        public static void Show(Unit target)
        {
            if(!target||!target.Definition)return;
            if(current){current.gameObject.SetActive(false);Destroy(current.gameObject);}
            current=new GameObject("Battle unit information").AddComponent<BattleUnitInfo>();
            current.unit=target;current.Build();
        }
        RectTransform Rect(Transform parent,string name,Vector2 position,Vector2 size)
        {
            var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);
            r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.anchoredPosition=position;r.sizeDelta=size;return r;
        }
        RectTransform Box(Transform parent,string name,Vector2 position,Vector2 size)
        {var r=Rect(parent,name,position,size);BattleUiSkin.Skin(r.gameObject.AddComponent<Image>(),BattleUiSkin.Navy);return r;}
        Text Label(Transform p,string value,Vector2 pos,Vector2 size,int fontSize)
        {
            var t=Rect(p,value,pos,size).gameObject.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=fontSize;t.fontStyle=FontStyle.Bold;
            t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;
        }
        Image Icon(Transform p,Sprite sprite,Vector2 position,Vector2 size)
        {var image=Rect(p,"Icon",position,size).gameObject.AddComponent<Image>();image.sprite=sprite;image.preserveAspect=true;image.raycastTarget=false;return image;}
        void Build()
        {
            font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");flow=FindFirstObjectByType<BattleFlow>();
            var canvas=gameObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=500;
            var scaler=gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1125,2436);
            gameObject.AddComponent<GraphicRaycaster>();
            var dim=Rect(transform,"Input blocker",Vector2.zero,Vector2.zero);dim.anchorMin=Vector2.zero;dim.anchorMax=Vector2.one;dim.offsetMin=dim.offsetMax=Vector2.zero;
            dim.gameObject.AddComponent<Image>().color=new Color(0,0,0,.65f);
            var safe=Rect(dim,"Safe area",Vector2.zero,Vector2.zero);var area=Screen.safeArea;
            safe.anchorMin=new Vector2(area.xMin/Screen.width,area.yMin/Screen.height);safe.anchorMax=new Vector2(area.xMax/Screen.width,area.yMax/Screen.height);safe.offsetMin=safe.offsetMax=Vector2.zero;
            var panel=Box(safe,"Unit information",Vector2.zero,new Vector2(1050,2220));
            Canvas.ForceUpdateCanvases();float fit=Mathf.Min(1f,Mathf.Min(safe.rect.width/1090f,safe.rect.height/2260f));panel.localScale=Vector3.one*fit;
            var def=unit.Definition;
            bool mythic=def.Rank==Rank.Mythic;
            bool usesMana=unit.Combat&&unit.Combat.UsesMana;
            // One outer frame, flat information surfaces: only actions look clickable.
            var surface=Rect(panel,"Flat navy surface",Vector2.zero,new Vector2(1010,2170));
            surface.gameObject.AddComponent<Image>().color=new Color32(17,38,60,255);
            Label(panel,def.DisplayName,new Vector2(-35,1000),new Vector2(800,95),60);
            var badges=Label(panel,(mythic?"MYTHIC":def.Rank.ToString())+"   ·   "+(def.IsRanged?"Ranged":"Melee"),new Vector2(0,910),new Vector2(900,65),36);
            badges.color=new Color(1,.83f,.52f);
            var close=Rect(panel,"Close",new Vector2(440,1010),new Vector2(125,125));ApprovedUiArt.Apply(close.gameObject.AddComponent<Image>(),"close");close.gameObject.AddComponent<Button>().onClick.AddListener(Close);
            float stageHeight=mythic||usesMana?400:530;
            var stage=Rect(panel,"Idle preview",new Vector2(0,845-stageHeight/2),new Vector2(970,stageHeight));stage.gameObject.AddComponent<RectMask2D>();
            var background=Rect(stage,"Faction background",Vector2.zero,stage.sizeDelta).gameObject.AddComponent<RawImage>();
            background.texture=Resources.Load<Texture2D>("AllyStages/"+def.Race.ToString().ToLowerInvariant());background.raycastTarget=false;
            if(background.texture)
            {
                float fraction=(970f/stageHeight)/((float)background.texture.width/background.texture.height);
                background.uvRect=fraction<1?new UnityEngine.Rect((1-fraction)/2,0,fraction,1):new UnityEngine.Rect(0,(1-1/fraction)/2,1,1/fraction);
            }
            var actor=Icon(stage,null,new Vector2(0,-stageHeight*.34f),Vector2.one);
            actor.gameObject.AddComponent<MythicIdleImage>().Bind(actor,def.Animations?.Idle,300);
            if(usesMana)
            {
                var meter=Box(panel,"Mana",new Vector2(0,405),new Vector2(950,55));
                manaFill=Box(meter,"Fill",Vector2.zero,new Vector2(940,45)).GetComponent<Image>();BattleUiSkin.Skin(manaFill,BattleUiSkin.Blue);manaFill.rectTransform.pivot=new Vector2(0,.5f);manaFill.rectTransform.anchoredPosition=new Vector2(-470,0);manaFill.raycastTarget=false;
                manaLabel=Label(meter,"",Vector2.zero,new Vector2(900,55),30);
            }
            string[] names={"HP","MAX MP","ATK","ATK SPD","CRIT CHANCE","CRIT DMG","REDUCTION","RANGE"};
            string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};
            float statTop=mythic||usesMana?310:220;
            for(int row=0;row<=4;row++)Divider(panel,new Vector2(0,statTop+77-row*140),new Vector2(920,3));
            for(int i=0;i<8;i++)
            {
                var cell=Rect(panel,names[i],new Vector2(i%2==0?-242:242,statTop-i/2*140),new Vector2(465,130));
                var icon=Rect(cell,"Stat icon",new Vector2(-160,0),new Vector2(94,94)).gameObject.AddComponent<RawImage>();icon.texture=ApprovedUiArt.StatTexture(icons[i]);icon.raycastTarget=false;
                Label(cell,names[i],new Vector2(60,32),new Vector2(325,46),30).color=new Color(.7f,.79f,.9f);
                numbers[i]=Label(cell,"",new Vector2(60,-24),new Vector2(325,68),52);
            }
            float skillY=mythic?-435:-545;
            var skill=Box(panel,"Skill",new Vector2(0,skillY),new Vector2(950,330));
            BattleUiSkin.Skin(skill.GetComponent<Image>(),BattleUiSkin.Dark);
            Label(skill,SkillName(def),new Vector2(0,112),new Vector2(900,65),44).color=new Color(1,.83f,.35f);
            var description=Label(skill,SkillDescription(def),new Vector2(0,-30),new Vector2(870,215),36);description.resizeTextForBestFit=true;description.resizeTextMinSize=29;description.resizeTextMaxSize=36;
            if(mythic)
            {
                equipment=Rect(panel,"Equipment",new Vector2(0,-795),new Vector2(950,300));
                Divider(panel,new Vector2(0,-620),new Vector2(920,3));
            }
            else
            {
                Divider(panel,new Vector2(0,-785),new Vector2(920,3));
                Label(panel,"Only Mythics can equip items",new Vector2(0,-850),new Vector2(900,75),34).color=new Color(.65f,.75f,.86f);
            }
            var back=Box(panel,"Back",new Vector2(0,-1010),new Vector2(900,120));var button=back.gameObject.AddComponent<Button>();button.onClick.AddListener(Close);ButtonFamilySkin.States(button);
            Label(back,"‹   Back",Vector2.zero,new Vector2(860,110),48);
            Refresh();
        }
        void Divider(Transform parent,Vector2 position,Vector2 size)
        {var line=Rect(parent,"Divider",position,size).gameObject.AddComponent<Image>();line.color=new Color(.3f,.43f,.58f,.5f);line.raycastTarget=false;}
        void Update()
        {
            if(!unit||!unit.IsAlive||flow&&flow.Current!=BattleFlow.Phase.Prep){Close();return;}
            if(Time.unscaledTime<nextRefresh)return;nextRefresh=Time.unscaledTime+.1f;Refresh();
        }
        void Refresh()
        {
            var def=unit.Definition;bool mana=unit.Combat&&unit.Combat.UsesMana;
            var brain=unit.GetComponent<UnitBrain>();
            string[] values={unit.MaxHp.ToString("N0"),mana?"100":"0",unit.AttackPower.ToString("N0"),(brain?brain.AttacksPerSecond:0).ToString("0.##")+"/s",(RunBuffs.Crit(unit.Team)*100).ToString("0.#")+"%",(unit.Team==UnitTeam.Ally?RunBuffs.CritMultiplier*100:200).ToString("0.#")+"%",(100-RunBuffs.DamageAfterArmor(100,unit.Team)).ToString("0.#")+"%",(brain?brain.EffectiveAttackRange:def.AttackRange).ToString("0.##")};
            for(int i=0;i<8;i++)numbers[i].text=values[i];
            if(manaLabel)manaLabel.text=mana?"MP  "+unit.Combat.Mana.ToString("0")+" / 100":def.Mythic==MythicKind.Undead?"PASSIVE · NO MANA":"NO MANA SKILL";
            if(manaFill)manaFill.rectTransform.sizeDelta=new Vector2(mana?940*Mathf.Clamp01(unit.Combat.Mana/100):0,60);
            if(!equipment)return;
            int count=unit.Equipment?.Types.Count??0;if(equipmentCount==count)return;equipmentCount=count;
            foreach(Transform child in equipment)Destroy(child.gameObject);
            Label(equipment,"EQUIPPED ITEMS",new Vector2(0,120),new Vector2(900,55),34);
            if(def.Rank!=Rank.Mythic){Label(equipment,"Only Mythics can equip items",Vector2.zero,new Vector2(850,140),34);return;}
            if(count==0){Label(equipment,"No items equipped\nDrag an item onto this Mythic",Vector2.zero,new Vector2(850,160),34);return;}
            var view=Rect(equipment,"Item list",new Vector2(0,-35),new Vector2(900,210));view.gameObject.AddComponent<RectMask2D>();view.gameObject.AddComponent<Image>().color=new Color(0,0,0,.01f);
            var scroll=view.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.viewport=view;scroll.scrollSensitivity=45;
            var content=Rect(view,"Content",Vector2.zero,new Vector2(880,Mathf.Max(210,count*150)));content.anchorMin=content.anchorMax=content.pivot=new Vector2(.5f,1);scroll.content=content;
            for(int i=0;i<count;i++)
            {
                int type=unit.Equipment.Types[i];float y=-75-i*150;
                var itemIcon=Icon(content,BattleItemRecipes.Icon(type),new Vector2(-350,y),new Vector2(110,110));itemIcon.rectTransform.anchorMin=itemIcon.rectTransform.anchorMax=new Vector2(.5f,1);
                var name=Label(content,BattleItemRecipes.Items[type].Name,new Vector2(75,y+38),new Vector2(670,48),34);name.rectTransform.anchorMin=name.rectTransform.anchorMax=new Vector2(.5f,1);
                var effect=Label(content,BattleItemRecipes.Effect(type),new Vector2(75,y-26),new Vector2(670,90),29);effect.color=new Color(.6f,1,.78f);effect.rectTransform.anchorMin=effect.rectTransform.anchorMax=new Vector2(.5f,1);
            }
        }
        static string SkillName(UnitDefinition def)=>def.Mythic!=MythicKind.None?MythicPresentation.SkillName(def.Mythic):def.Rank==Rank.Legendary?"Legendary Skill":"Basic Attack";
        static string SkillDescription(UnitDefinition def)
        {
            if(def.Mythic==MythicKind.Guardian)return "20% ATK damage each second\nHeal 50% of aura damage dealt\n15 MP/s · No mana gain while active\nMove and attack while aura is active";
            if(def.Mythic!=MythicKind.None)return MythicPresentation.Description(def.Mythic);
            if(def.Rank!=Rank.Legendary)return def.IsRanged?"Attacks enemies from range.":"Attacks enemies in melee range.";
            return def.Race switch{Race.Human=>"Area shockwave\nDeals 200% ATK and knocks enemies up.",Race.Elf=>"Frost rain\nDeals 200% ATK and freezes enemies for 2s.",Race.Machine=>"Protect melee allies\nShield: 50% of each ally's max HP for 8s.",Race.Monster=>"Team roar\nAll allies gain 30% ATK and attack speed for 5s.",_=>"Summon 3 skeletons\nEach has one third of the caster's HP and ATK."};
        }
        void Close(){gameObject.SetActive(false);Destroy(gameObject);}
        void OnDestroy(){if(current==this)current=null;}
    }
}
