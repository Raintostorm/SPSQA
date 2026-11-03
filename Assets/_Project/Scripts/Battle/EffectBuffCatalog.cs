using System;
using System.Linq;
using Project.Data;
namespace Project.Battle
{
    public enum EffectBuffKind { BloodTile,GoldTile,DoubleStrike,Bodyguard,FrostTouch,GrandAscension,Reinforcements,WeaponSupplies,WarChest,SecondWind,EmberTouch,ManaFlow,WaveDividend,MythicMight,SoulHarvest }
    public sealed class EffectBuffDefinition
    {
        public readonly EffectBuffKind Kind;public readonly Rank Rank;public readonly string Name,Description,Scope;
        public EffectBuffDefinition(EffectBuffKind kind,Rank rank,string name,string description,string scope){Kind=kind;Rank=rank;Name=name;Description=description;Scope=scope;}
    }
    public static class EffectBuffCatalog
    {
        public static readonly EffectBuffDefinition[] All={
            new(EffectBuffKind.BloodTile,Rank.Epic,"Blood Tile","Place allies here: heal 1% max HP/s and gain 15% lifesteal for the wave.","TILE ASSIGNMENT · Includes Mythics"),
            new(EffectBuffKind.GoldTile,Rank.Epic,"Gold Tile","Allies assigned here earn 4 coins per last hit this wave.","TILE ASSIGNMENT · Includes Mythics"),
            new(EffectBuffKind.DoubleStrike,Rank.Epic,"Double Strike","Every 4th basic hit: extra strike for 50% ATK.","ALL ALLIES · Includes Mythics"),
            new(EffectBuffKind.Bodyguard,Rank.Epic,"Bodyguard","Each wave: shield equal to 20% max HP.","ALL ALLIES · Includes Mythics"),
            new(EffectBuffKind.FrostTouch,Rank.Epic,"Frost Touch","Every 4th basic hit: slow move and attack speed 20% for 2s.","ALL ALLIES · Includes Mythics"),
            new(EffectBuffKind.GrandAscension,Rank.Legendary,"Grand Ascension","All ordinary allies advance 1 rarity. Stops at Legendary.","ORDINARY ALLIES · Mythics unchanged"),
            new(EffectBuffKind.Reinforcements,Rank.Legendary,"Legendary Reinforcements","Gain 3 random Legendary allies.","REINFORCEMENTS · 3 allies"),
            new(EffectBuffKind.WeaponSupplies,Rank.Legendary,"Weapon Supplies","Gain 2 random basic items. Equip or combine.","MYTHIC EQUIPMENT · Only Mythics equip"),
            new(EffectBuffKind.WarChest,Rank.Legendary,"War Chest","Gain 200 battle coins now.","BATTLE WALLET"),
            new(EffectBuffKind.SecondWind,Rank.Legendary,"Second Wind","Next wave, if your army falls: revive allies at full HP once.","ALL ALLIES · Includes Mythics"),
            new(EffectBuffKind.EmberTouch,Rank.Epic,"Ember Touch","Every 4th basic hit burns for 20% ATK/s for 2s.","ALL ALLIES"),
            new(EffectBuffKind.ManaFlow,Rank.Legendary,"Mana Flow","Receive 20% more mana from all sources.","MANA USERS"),
            new(EffectBuffKind.WaveDividend,Rank.Legendary,"Wave Dividend","After victory: gain 3 coins × wave number.","BATTLE WALLET"),
            new(EffectBuffKind.MythicMight,Rank.Legendary,"Mythic Might","Mythics gain 25% ATK.","MYTHICS"),
            new(EffectBuffKind.SoulHarvest,Rank.Legendary,"Soul Harvest","Each enemy defeated grants +1% ATK and HP from the next wave. Unlimited stacks.","ALL ALLIES")
        };
        public static bool Persistent(EffectBuffKind kind)=>(int)kind<5||(int)kind>=10;
        public static EffectBuffDefinition[] Draw(Rank rank,Random random,Func<EffectBuffKind,bool> allowed)
        {
            var pool=All.Where(d=>d.Rank==rank&&allowed(d.Kind)).ToList();
            for(int i=pool.Count-1;i>0;i--){int j=random.Next(i+1);(pool[i],pool[j])=(pool[j],pool[i]);}
            return pool.Take(3).ToArray();
        }
    }
}
