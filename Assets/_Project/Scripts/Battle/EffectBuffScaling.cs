using System;
namespace Project.Battle
{
    public static class EffectBuffScaling
    {
        public static string Describe(EffectBuffKind kind,int level)=>kind switch
        {
            EffectBuffKind.BloodTile=>$"Place allies here: heal {level}% max HP/s and gain {15*level}% lifesteal this wave.",
            EffectBuffKind.GoldTile=>$"Allies assigned here earn {4*level} battle coins per last hit this wave.",
            EffectBuffKind.DoubleStrike=>$"Every 4th basic hit: extra strike for {50*level}% ATK.",
            EffectBuffKind.Bodyguard=>$"Each wave: shield equal to {20*level}% max HP.",
            EffectBuffKind.FrostTouch=>$"Every 4th basic hit: slow move and attack speed {Math.Min(80,20*level)}% for 2s.",
            EffectBuffKind.EmberTouch=>$"Every 4th basic hit burns for {20*level}% ATK/s for 2s.",
            EffectBuffKind.ManaFlow=>$"Receive {20*level}% more mana. Mana locks still apply.",
            EffectBuffKind.WaveDividend=>$"After victory: gain {3*level} coins × wave number.",
            EffectBuffKind.MythicMight=>$"Mythics gain {25*level}% ATK.",
            EffectBuffKind.SoulHarvest=>$"Each enemy defeated grants +{level}% ATK and HP from next wave. Unlimited stacks.",
            _=>""
        };
    }
}
