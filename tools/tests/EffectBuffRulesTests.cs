using System;
using System.Linq;
using Project.Battle;
using Project.Data;
namespace Project.Data{public enum Rank{Common,Rare,Epic,Legendary,Mythic}}
class EffectBuffRulesTests
{
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static void Near(float value,float expected,string message)=>Check(Math.Abs(value-expected)<.00001,message);
    static void Main()
    {
        var burn=new BurnClock();burn.Apply(0);Check(!burn.TakeTick(.9f),"burn waits one second");burn.Apply(.8f);Check(burn.TakeTick(1)&&!burn.TakeTick(1),"refresh preserves due tick and no duplicate damage");burn.Apply(1.6f);Check(burn.TakeTick(2)&&burn.TakeTick(3)&&!burn.TakeTick(4),"duration refreshed without tick starvation");burn.Clear();Check(!burn.Active(0)&&!burn.TakeTick(100),"burn clears on death/run reset");burn.Apply(0);Check(burn.TakeTick(2.2f)&&burn.TakeTick(2.2f)&&!burn.TakeTick(2.2f),"fast game-time catches both ticks");
        var growth=new HarvestGrowth();growth.Kill(1);growth.Kill(1);Near(growth.Multiplier,1,"no same-wave growth");growth.NextWave();Near(growth.Multiplier,1.02f,"next-wave growth");for(int i=0;i<10000;i++)growth.Kill(2);growth.NextWave();Near(growth.Multiplier,201.02f,"growth has no stack cap");growth.Reset();Near(growth.Multiplier,1,"growth resets per run");
        Check(EffectBuffCatalog.All.Length==15,"fifteen definitions");
        Check(EffectBuffCatalog.All.Count(d=>d.Rank==Rank.Epic)==6&&EffectBuffCatalog.All.Count(d=>d.Rank==Rank.Legendary)==9,"distinct 6/9 pools");
        for(int seed=0;seed<100;seed++)foreach(var rank in new[]{Rank.Epic,Rank.Legendary})
        {var cards=EffectBuffCatalog.Draw(rank,new Random(seed),_=>true);Check(cards.Length==3&&cards.Select(d=>d.Kind).Distinct().Count()==3&&cards.All(d=>d.Rank==rank),"3 unique matching choices");}
        var filtered=EffectBuffCatalog.Draw(Rank.Legendary,new Random(1),k=>k!=EffectBuffKind.SecondWind&&k!=EffectBuffKind.GrandAscension);
        Check(filtered.Length==3&&filtered.All(d=>d.Kind!=EffectBuffKind.SecondWind&&d.Kind!=EffectBuffKind.GrandAscension),"eligibility filter");
        Check(EffectBuffMath.WaveCoins(1,5)==15&&EffectBuffMath.WaveCoins(2,15)==90,"wave-scaled dividend and buff levels");
        Check(EffectBuffCatalog.All.Where(d=>EffectBuffCatalog.Persistent(d.Kind)).Count()==10,"ten persistent buffs fit overflow HUD");
        Near(EffectBuffMath.HealFraction(1),.01f,"heal1");Near(EffectBuffMath.HealFraction(3),.03f,"heal3");
        Near(EffectBuffMath.Lifesteal(2),.3f,"lifesteal stack");Check(EffectBuffMath.Coins(3)==12,"coins stack");
        Near(EffectBuffMath.ExtraAttack(2),1,"extra 100%ATK");Near(EffectBuffMath.ShieldFraction(3),.6f,"shield stack");
        Near(EffectBuffMath.SlowMultiplier(1),.8f,"slow1");Near(EffectBuffMath.SlowMultiplier(8),.2f,"slow capped80%");
        Check(!EffectBuffMath.FourthHit(0)&&!EffectBuffMath.FourthHit(3)&&EffectBuffMath.FourthHit(4)&&EffectBuffMath.FourthHit(8),"hit cadence");
        Check(EffectBuffMath.RescueEligible(true,2,2,0,3),"rescue at defeat");
        Check(!EffectBuffMath.RescueEligible(false,2,2,0,3)&&!EffectBuffMath.RescueEligible(true,2,3,0,3)&&!EffectBuffMath.RescueEligible(true,2,2,1,3)&&!EffectBuffMath.RescueEligible(true,2,2,0,0),"rescue once/wave/all-dead enemies guards");
        Check(EffectBuffScaling.Describe(EffectBuffKind.GoldTile,3).Contains("12"),"UI value consistent");
        var occupied=new int[,]{{9,9,0}};var counts=new int[,]{{1,1,0}};
        Check(!EffectBuffPlacement.TryPlan(occupied,counts,new[]{1,2},3,out var impossible)&&impossible.Length==0,"split untouched stacks must not be collapsed by capacity estimate");
        Check(counts[0,2]==0&&occupied[0,2]==0,"failed plan leaves real board unchanged");
        Check(EffectBuffPlacement.TryPlan(occupied,counts,new[]{9,1,1},3,out var cells)&&cells.SequenceEqual(new[]{0,2,2}),"reuse matching cells before empty cells");
        Check(counts[0,0]==1&&counts[0,2]==0,"successful plan also leaves real board unchanged");
        for(int seed=0;seed<100;seed++)
        {
            var used=new System.Collections.Generic.HashSet<int>();var rng=new Random(seed);var candidates=new[]{1,2,5,6};
            Check(GroundBuffCells.TryChoose(candidates,used,rng,out int blood),"blood cell available");used.Add(blood);
            Check(GroundBuffCells.TryChoose(candidates,used,rng,out int gold)&&gold!=blood,"blood and gold cannot share cell");
            Check(candidates.Contains(blood)&&candidates.Contains(gold),"tiles stay on eligible grid cells");
        }
        Check(!GroundBuffCells.TryChoose(new[]{1},new System.Collections.Generic.HashSet<int>{1},new Random(1),out int none)&&none==-1,"no overlap fallback on full grid");
        Console.WriteLine("PASS: 6 Epic / 9 Legendary definitions, delayed unlimited harvest growth and wave-scaled coins, unique eligible 3-card draws, stack scaling, frost cap, hit cadence rescue guards and transactional promotion placement and distinct ground cells.");
    }
}
