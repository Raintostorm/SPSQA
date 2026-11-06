using System.Linq;
using Project.Battle;
using Project.Data;
using Project.Meta;
using UnityEngine;
using UnityEngine.UI;
namespace Project.UI
{
    public sealed partial class ChapterSelectScreen
    {
        ChapterOneSlimes monsterRoster;
        UnitDefinition[] Monsters()
        {
            if(monsterRoster==null)
            {
                var source=Resources.Load<AllySummon>("ChapterSelect/EnemyTemplates");
                if(source && source.Units!=null)monsterRoster=new ChapterOneSlimes(source.Units);
            }
            return monsterRoster!=null&&monsterRoster.Ready?monsterRoster.Entries:new UnitDefinition[0];
        }
        RectTransform Subpage(string title,UnityEngine.Events.UnityAction back)
        {
            if(notice){notice.SetActive(false);Destroy(notice);}
            var page=Panel(title,content,Vector2.zero,new Vector2(1050,2300),new Color(.035f,.09f,.15f));notice=page.gameObject;
            // Shared chrome (UI-005): one ribbon title, one back at top-left, no second Back at the bottom.
            ScreenChrome.Title(page,title,new Vector2(0,1050),760);
            ScreenChrome.Back(page,new Vector2(-455,1050),back);
            return page;
        }
        void CloseSubpage(){if(notice){notice.SetActive(false);Destroy(notice);notice=null;}}
        RectTransform Surface(Transform p,string name,float x,float y,float w,float h)
        {
            var r=Panel(name,p,new Vector2(x,y),new Vector2(w,h),Color.white);BattleUiSkin.Skin(r.GetComponent<Image>(),BattleUiSkin.Navy);return r;
        }
        void MonsterArt(Transform parent,UnitDefinition def,Vector2 pos,Vector2 size,bool silhouette=false)
        {
            var sprite=def?.Portrait;if(!sprite)return;
            var r=Rect("Monster",parent,pos,size);var im=r.gameObject.AddComponent<Image>();im.sprite=sprite;im.preserveAspect=true;im.raycastTarget=false;
            // Runtime slime sprites can have large transparent margins: fit visible sprite bounds.
            var bounds=sprite.bounds.size;
            if(bounds.x>0&&bounds.y>0)r.sizeDelta=new Vector2(size.x,Mathf.Min(size.y,size.x*bounds.y/bounds.x));
            im.color=silhouette?new Color(.015f,.025f,.045f):Color.white;
        }
        void RewardTile(Transform p,string icon,string amount,float x)
        {
            var tile=Surface(p,"Reward",x,330,300,280);Art(tile,icon,new Vector2(0,45),new Vector2(120,120));Label(tile,amount,new Vector2(0,-75),new Vector2(280,80),58);
        }
        void ShowRewards()
        {
            var page=Subpage("REWARDS",CloseSubpage);
            Label(page,$"CHAPTER 1-{selected}",new Vector2(0,905),new Vector2(900,70),44);
            Art(page,"ChapterSelect/banner",new Vector2(0,720),new Vector2(960,250));
            ScreenChrome.Title(page,"VICTORY REWARDS",new Vector2(0,535),620,ScreenChrome.Banner.Section);
            // A zero reward is not a reward: show only what this stage actually pays, centred.
            var paid=new[]{("HomeChrome/coin",gold.text),("HomeChrome/key",keys.text),("Summon/gem",gems.text)}.Where(r=>r.Item2!="0").ToArray();
            for(int i=0;i<paid.Length;i++)RewardTile(page,paid[i].Item1,paid[i].Item2,(i-(paid.Length-1)*.5f)*325);
            ScreenChrome.Title(page,"FIRST ENCOUNTERS",new Vector2(0,115),620,ScreenChrome.Banner.Section);
            Label(page,"One-time reward per monster type",new Vector2(0,30),new Vector2(960,50),30);
            var entries=Monsters();
            // Six compact rows fit the iPhone X safe area without an extra nested scroll.
            for(int i=0;i<entries.Length;i++)
            {
                var def=entries[i];bool seen=MonsterDiscovery.Seen(SaveSystem.Current,def.UnitId);
                var row=Surface(page,"Encounter",0,-75-i*140,960,125);
                MonsterArt(row,def,new Vector2(-350,0),new Vector2(170,95),!seen);
                Label(row,seen?def.DisplayName:"???",new Vector2(-60,0),new Vector2(410,95),34);
                Art(row,"Summon/gem",new Vector2(225,10),new Vector2(58,65));
                Label(row,"+"+MonsterDiscovery.Reward(def.UnitId),new Vector2(330,18),new Vector2(140,65),40);
                var state=Label(row,seen?"Received":"First encounter",new Vector2(300,-35),new Vector2(280,45),25);
                // Chu "Received" de lan vao chu khac khi luot nhanh; dau tick doc duoc
                // ma khong phai doc. Chua nhan thi de trong, khong dat huy hieu xam —
                // mot hang khong co gi da du nghia la "chua".
                if(seen)
                {
                    state.color=new Color(.55f,.88f,.55f);
                    Art(row,"Badges/check",new Vector2(430,0),new Vector2(56,56));
                }
            }
            Label(page,"Victory rewards after completion · Encounter rewards on discovery",new Vector2(0,-930),new Vector2(980,70),27);
        }
        void ShowCodex()
        {
            var page=Subpage("MONSTER CODEX",CloseSubpage);var entries=Monsters();
            Label(page,$"CHAPTER 1 · Discovered {entries.Count(d=>MonsterDiscovery.Seen(SaveSystem.Current,d.UnitId))} / {entries.Length}",new Vector2(0,920),new Vector2(980,75),38);
            Art(page,"ChapterSelect/banner",new Vector2(0,745),new Vector2(960,260));
            ScreenChrome.Title(page,"COMMON",new Vector2(0,571),380,ScreenChrome.Banner.Section);
            for(int i=0;i<entries.Length;i++)
            {
                var def=entries[i];bool seen=MonsterDiscovery.Seen(SaveSystem.Current,def.UnitId);bool wide=i>=4;
                float x=wide?0:(i%2==0?-245:245),y=wide?(i==4?-405:-780):365-i/2*355;
                if(wide)ScreenChrome.Title(page,i==4?"ELITE":"BOSS",new Vector2(0,y+175),380,ScreenChrome.Banner.Section);
                var button=Button(page,"",new Vector2(x,y),new Vector2(wide?960:465,wide?270:325),()=>ShowMonster(def));
                button.GetComponentInChildren<Text>().gameObject.SetActive(false);button.interactable=seen;BattleUiSkin.Skin(button.GetComponent<Image>(),BattleUiSkin.Navy);
                MonsterArt(button.transform,def,new Vector2(wide?-280:0,45),new Vector2(300,150),!seen);
                // The "?" sits on the silhouette, not beside the name on wide Elite/Boss cards.
                if(!seen)Label(button.transform,"?",new Vector2(wide?-280:0,wide?45:50),new Vector2(150,110),80);
                Label(button.transform,seen?def.DisplayName:"Undiscovered",new Vector2(wide?130:0,wide?45:-65),new Vector2(wide?560:445,65),34);
                var caption=Label(button.transform,seen?$"+{MonsterDiscovery.Reward(def.UnitId)} gems  ·  Received":"???",new Vector2(wide?130:0,wide?-40:-120),new Vector2(wide?560:445,65),29);
                if(seen)caption.color=new Color(.55f,.88f,.55f);
            }
        }
        void ShowMonster(UnitDefinition def,bool skill=false)
        {
            if(!def||!MonsterDiscovery.Seen(SaveSystem.Current,def.UnitId))return;
            var page=Subpage("MONSTER DETAILS",ShowCodex);
            Art(page,"ChapterSelect/banner",new Vector2(0,740),new Vector2(960,440));
            MonsterArt(page,def,new Vector2(0,680),new Vector2(620,260));
            Label(page,def.DisplayName,new Vector2(0,450),new Vector2(980,100),60);
            Label(page,$"{def.EnemyTier.ToString().ToUpper()}  ·  {(def.IsRanged?"RANGED":"MELEE")}",new Vector2(0,345),new Vector2(960,75),38);
            for(int i=0;i<2;i++){bool tab=i==1;var b=Button(page,tab?"Skill":"Stats",new Vector2((i-.5f)*480,230),new Vector2(465,100),()=>ShowMonster(def,tab));ScreenChrome.Action(b,skill==tab?ScreenChrome.Role.Secondary:ScreenChrome.Role.Cancel);}
            if(!skill)
            {
                float duration=def.Animations?.Attack?.Total??0;
                string[] names={"HP","ATK","ATK SPD","RANGE"};string[] icons={"hp","atk","speed","range"};
                string[] values={def.BaseHp.ToString("N0"),def.BaseAttack.ToString("N0"),(1f/Mathf.Max(.01f,duration+def.AttackCooldown)).ToString("0.00")+"/s",def.AttackRange.ToString("0.##")};
                for(int i=0;i<4;i++){var cell=Surface(page,names[i],(i%2-.5f)*480,70-i/2*180,465,160);Art(cell,"StatIcons/"+icons[i],new Vector2(-160,0),new Vector2(75,85));Label(cell,names[i],new Vector2(40,40),new Vector2(440,55),30);Label(cell,values[i],new Vector2(40,-30),new Vector2(440,80),52);}
                Label(page,"Base stats · Before battle modifiers",new Vector2(0,-265),new Vector2(960,65),29);
            }
            var description=Surface(page,"Ability",0,skill?-70:-490,960,skill?460:340);
            bool boss=def.EnemyTier==UnitDefinition.EnemyClass.Boss;
            Label(description,boss?"DOUBLE STRIKE":def.IsRanged?"RANGED ATTACK":"MELEE ATTACK",new Vector2(0,90),new Vector2(900,75),44);
            Label(description,boss?"Attacks twice.\nGains mana by attacking.\nCannot be revived.":def.IsRanged?"Fires a projectile at its target.":"Bites a nearby target.",new Vector2(0,-35),new Vector2(900,180),36);
            var reward=Surface(page,"DiscoveryReward",0,-815,960,180);Art(reward,"Summon/gem",new Vector2(-345,0),new Vector2(100,110));
            Label(reward,$"+{MonsterDiscovery.Reward(def.UnitId)} Gems · Received",new Vector2(75,30),new Vector2(720,80),44);
            Label(reward,"First encounter · Once only",new Vector2(75,-45),new Vector2(720,60),31);
        }
    }
}
