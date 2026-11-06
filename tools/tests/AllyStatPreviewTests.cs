// Isolated production stat-model tests; does not read or modify player saves.
using System;
using System.Collections.Generic;
using Project.Data;
using Project.Meta;
namespace Project.Data
{
    public enum TreasureStat {Atk,Hp,AtkSpeed,CritDamage,CritChance,LifeSteal,Armor}
    public class UnitDefinition
    {
        public float BaseHp=180,BaseAttack=27,AttackCooldown=.7f,AttackRange=.9f;
        public bool IsRanged;public Rank Rank;public MythicKind Mythic;
        public AnimationSet Animations=new AnimationSet();
    }
    public class AnimationSet {public Clip Attack=new Clip();}
    public class Clip {public float Total=.3f;}
    public class GameBalance {public float CritMultiplier=2;}
    public class TreasureDatabase {}
}
namespace Project.Meta { public static class TreasureRoll {public static float Total(TreasureStat stat,TreasureDatabase db)=>0;} }
namespace Project.Battle
{
    public static class UnitUpgrades {public const float PerLevel=.1f;}
    public static class LegendaryCombat {public const float MaxMana=100;}
}
static class AllyStatPreviewTests
{
    static void Near(float actual,float expected,string what)
    {if(Math.Abs(actual-expected)>.001f)throw new Exception(what+": "+actual+" != "+expected);}
    static void Main()
    {
        var def=new UnitDefinition();
        var totals=new Dictionary<TreasureStat,float> {
            [TreasureStat.Hp]=22,[TreasureStat.Atk]=50,[TreasureStat.AtkSpeed]=25,
            [TreasureStat.CritDamage]=60,[TreasureStat.CritChance]=12,[TreasureStat.Armor]=60 };
        float Total(TreasureStat stat)=>totals.TryGetValue(stat,out var value)?value:0;
        var now=AllyStatPreview.Calculate(def,1,2,Total);
        var next=AllyStatPreview.Calculate(def,2,2,Total);
        Near(now.Hp,219.6f,"HP incl relics");Near(next.Hp,241.56f,"next HP incl relics");
        Near(next.Hp-now.Hp,21.96f,"HP delta");Near(now.Attack,40.5f,"ATK incl relics");
        Near(next.Attack-now.Attack,4.05f,"ATK delta");
        Near(now.Mana,0,"nonlegend MP");Near(now.AttackSpeed,1.25f,"attack clip plus cooldown");
        Near(now.CritChance,.12f,"crit chance");Near(now.CritDamage,3.2f,"crit multiplier stacking");
        Near(now.DamageReduction,.375f,"armor conversion");Near(now.Range,.55f,"actual melee clamp");
        Near(now.AttackSpeed,next.AttackSpeed,"level does not improve speed");
        Near(now.CritChance,next.CritChance,"level does not improve crit");
        def.Rank=Rank.Legendary;def.IsRanged=true;def.AttackRange=3.2f;
        var legend=AllyStatPreview.Calculate(def,30,2,Total);
        Near(legend.Mana,100,"legend MP");Near(legend.Range,3.2f,"ranged range");
        Near(legend.Hp,856.44f,"level 30 HP");
        var noRelics=AllyStatPreview.Read(def,1,null,null);
        Near(noRelics.CritChance,0,"no invented base crit");Near(noRelics.CritDamage,2,"base crit damage");
        // Mana is a role rule: Undead Mythic has a passive, every other Mythic uses mana.
        def.Rank=Rank.Mythic;
        foreach(MythicKind kind in Enum.GetValues(typeof(MythicKind)))
        {
            if(kind==MythicKind.None)continue;
            def.Mythic=kind;
            var mythic=AllyStatPreview.Calculate(def,30,2,Total);
            Near(mythic.Mana,kind==MythicKind.Undead?0:100,"Mythic mana: "+kind);
            Near(mythic.Hp,legend.Hp,"Mythic permanent HP uses same level/relic model: "+kind);
        }
        def.Rank=Rank.Legendary;def.Mythic=MythicKind.Undead;
        Near(AllyStatPreview.Calculate(def,1,2,Total).Mana,100,"Rank determines Legendary mana despite stale Mythic identity");
        Console.WriteLine("PASS: all 8 stats, relic stacking, next-level deltas, ranges, attack cycle, Legendary and all Mythic mana roles.");
    }
}
