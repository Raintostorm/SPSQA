namespace Project.Battle
{
    // Run-only growth: kills never alter stats during their current wave.
    public sealed class HarvestGrowth
    {
        public double ActivePercent { get; private set; }
        public double PendingPercent { get; private set; }
        public void Kill(int level){if(level>0)PendingPercent+=level;}
        public void NextWave(){ActivePercent+=PendingPercent;PendingPercent=0;}
        public void Reset(){ActivePercent=PendingPercent=0;}
        public float Multiplier=>(float)(1+ActivePercent*.01);
    }
}
