using System;
using Project.Battle;
using Project.Data;

namespace Project.Meta
{
    // Permanent pre-battle stats only. Never reads leftover buffs from a previous run.
    public readonly struct AllyStatPreview
    {
        public readonly float Hp, Mana, Attack, AttackSpeed, CritChance, CritDamage, DamageReduction, Range;
        AllyStatPreview(float hp,float mana,float attack,float speed,float crit,float critDamage,float reduction,float range)
        { Hp=hp;Mana=mana;Attack=attack;AttackSpeed=speed;CritChance=crit;CritDamage=critDamage;DamageReduction=reduction;Range=range; }

        public static AllyStatPreview Read(UnitDefinition def,int level,GameBalance balance,TreasureDatabase treasures)
        {
            float Total(TreasureStat stat) => treasures != null ? TreasureRoll.Total(stat,treasures) : 0;
            return Calculate(def,level,balance != null ? balance.CritMultiplier : 2f,Total);
        }
        public static AllyStatPreview Calculate(UnitDefinition def,int level,float baseCritMultiplier,Func<TreasureStat,float> total)
        {
            float mult=1f+(Math.Max(1,level)-1)*UnitUpgrades.PerLevel;
            float armor=Math.Max(0,total(TreasureStat.Armor));
            // UnitBrain rests AFTER its attack animation; inverse cooldown alone overstates attacks/s.
            float animation=def.Animations?.Attack?.Total??0;
            float cycle=Math.Max(.01f,animation+def.AttackCooldown);
            return new AllyStatPreview(
                def.BaseHp*mult*(1+total(TreasureStat.Hp)/100f),
                def.Rank==Rank.Mythic?(def.Mythic==MythicKind.Undead?0:100):def.Rank==Rank.Legendary?LegendaryCombat.MaxMana:0,
                def.BaseAttack*mult*(1+total(TreasureStat.Atk)/100f),
                (1+Math.Max(0,total(TreasureStat.AtkSpeed))/100f)/cycle,
                Math.Max(0,Math.Min(1,total(TreasureStat.CritChance)/100f)),
                baseCritMultiplier*(1+total(TreasureStat.CritDamage)/100f),
                armor/(armor+100f),
                def.IsRanged?def.AttackRange:Math.Min(def.AttackRange,.55f));
        }
    }
}
