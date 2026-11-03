namespace Project.Battle
{
    // Refresh duration without postponing scheduled damage ticks.
    public sealed class BurnClock
    {
        float until=float.NegativeInfinity,next=float.PositiveInfinity;
        public bool Active(float now)=>now<until;
        public void Apply(float now){if(!Active(now))next=now+1;until=now+2;}
        public bool TakeTick(float now){if(next>now||next>until)return false;next+=1;return true;}
        public void Clear(){until=float.NegativeInfinity;next=float.PositiveInfinity;}
    }
}
