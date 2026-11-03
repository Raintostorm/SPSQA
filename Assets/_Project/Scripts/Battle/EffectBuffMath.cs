using System;
namespace Project.Battle
{
    // Shared by gameplay, descriptions and isolated combat-rule tests.
    public static class EffectBuffMath
    {
        public static int WaveCoins(int level,int wave)=>checked(Math.Max(0,level)*Math.Max(0,wave)*3);
        public static float HealFraction(int level)=>Math.Max(0,level)*.01f;
        public static float Lifesteal(int level)=>Math.Max(0,level)*.15f;
        public static int Coins(int level)=>checked(Math.Max(0,level)*4);
        public static float ExtraAttack(int level)=>Math.Max(0,level)*.5f;
        public static float ShieldFraction(int level)=>Math.Max(0,level)*.2f;
        public static float SlowMultiplier(int level)=>Math.Max(.2f,1-Math.Max(0,level)*.2f);
        public static bool FourthHit(int hits)=>hits>0&&hits%4==0;
        public static bool RescueEligible(bool armed,int armedWave,int wave,int aliveAllies,int aliveEnemies)=>armed&&armedWave==wave&&aliveAllies==0&&aliveEnemies>0;
    }
}
