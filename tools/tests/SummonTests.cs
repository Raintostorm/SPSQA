// Isolated economy tests. Compile with production AllySummon.cs and SaveData.cs;
// these minimal stubs avoid booting Unity or touching the player's save.
using System;
using System.Linq;
using Project.Meta;
using Project.Data;
namespace UnityEngine {
    public class ScriptableObject {}
    public static class Mathf {
        public static int Max(int a,int b)=>Math.Max(a,b);
    }
}
namespace Project.Data {
    public enum Rank { Common,Rare,Epic,Legendary }
    public class UnitDefinition { public string UnitId; public Rank Rank; }
    public class GameBalance {
        public int[] TreasureWeights={70,23,6,1};
        public int LegendaryPity=80,UnitMaxLevel=30,UpgradeBaseCost=500;
        public Rank TenRollFloor=Rank.Epic;
    }
}
namespace Project.Meta {
    public static class MythicCatalog { public static UnitDefinition[] All=Array.Empty<UnitDefinition>(); }
    public static class SaveSystem {
        public static SaveData Current=new SaveData();
        public static void Save() {}
    }
}
static class SummonTests {
    sealed class StoneRandom:Random { public override double NextDouble()=>0; }
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static void Main() {
        var c=new AllySummon { Balance=new GameBalance(),Units=Enumerable.Range(0,20).Select(i=>new UnitDefinition { UnitId="unit"+i,Rank=(Rank)(i/5) }).ToArray() };
        var rng=new Random(481);
        SaveSystem.Current=new SaveData { Gems=9 };
        Check(!c.TryRoll(1,rng,out var result) && result.Count==0 && SaveSystem.Current.Gems==9,"insufficient gems");
        SaveSystem.Current=new SaveData { Gems=100 };
        Check(c.TryRoll(10,rng,out result) && result.Count==10,"ten results");
        Check(SaveSystem.Current.Gems==0 && SaveSystem.Current.Units.Sum(x=>x.Shards)==200 && result.All(x=>x.Shards==20),"20 shards per draw");
        Check(result.Any(x=>x.Unit.Rank>=Rank.Epic),"ten floor");
        Check(SaveSystem.Current.Units.All(x=>x.Level==1),"no auto upgrade");
        SaveSystem.Current.Gems=10;SaveSystem.Current.SummonsSinceLegendary=80;
        Check(c.TryRoll(1,rng,out result) && result[0].Unit.Rank==Rank.Legendary && SaveSystem.Current.SummonsSinceLegendary==0,"independent pity");
        Check(SaveSystem.Current.RollsSinceLegendary==0,"relic pity untouched");
        SaveSystem.Current=new SaveData{Gems=100,MythicStones=7};
        Check(c.TryRoll(1,new StoneRandom(),out result)&&result[0].MythicStone&&result[0].Shards==20,"stone outcome gives 20");
        Check(SaveSystem.Current.MythicStones==27&&SaveSystem.Current.Gems==90&&SaveSystem.Current.Units.Count==0,"stone wallet charge/grant no ally shards");
        SaveSystem.Current=new SaveData{Gems=100};
        Check(c.TryRoll(10,new StoneRandom(),out result)&&result.Count==10&&SaveSystem.Current.Gems==0,"stone ten-roll charge");
        Check(result.Count(r=>r.MythicStone)==9&&SaveSystem.Current.MythicStones==180,"multiple stones aggregate 20 each");
        Check(result.Last().Unit!=null&&result.Last().Unit.Rank>=Rank.Epic,"ten floor survives forced stone RNG");
        SaveSystem.Current=new SaveData{Gems=10,SummonsSinceLegendary=80};
        Check(c.TryRoll(1,new StoneRandom(),out result)&&!result[0].MythicStone&&result[0].Unit.Rank==Rank.Legendary,"legend pity precedes stone");
        SaveSystem.Current.Gems=10;c.Units=Array.Empty<UnitDefinition>();
        Check(!c.TryRoll(1,rng,out result) && SaveSystem.Current.Gems==10,"bad pool no debit");
        var row=new SaveData.UnitUpgrade { UnitId="test",Shards=1 };SaveSystem.Current.Units.Add(row);SaveSystem.Current.Gold=1000;
        Check(!AllySummon.TryUpgrade("test",c.Balance) && SaveSystem.Current.Gold==1000,"missing shards");
        row.Shards=2;SaveSystem.Current.Gold=499;
        Check(!AllySummon.TryUpgrade("test",c.Balance) && row.Shards==2,"missing gold");
        SaveSystem.Current.Gold=1000;
        Check(AllySummon.TryUpgrade("test",c.Balance) && row.Level==2 && row.Shards==0 && SaveSystem.Current.Gold==500,"upgrade atomic cost");
        row.Level=29;row.Shards=100;SaveSystem.Current.Gold=100000;
        Check(AllySummon.TryUpgrade("test",c.Balance) && row.Level==30,"level 29 can reach 30");
        row.Shards=100;SaveSystem.Current.Gold=100000;
        Check(!AllySummon.TryUpgrade("test",c.Balance) && row.Shards==100 && SaveSystem.Current.Gold==100000,"max level no debit");
        Console.WriteLine("PASS: 20 stones per outcome, repeated stone aggregation, pity/floor priority, summon cost, shards, x10 floor, independent pity, invalid pool, gold+shard upgrade and max-level guard.");
    }
}
