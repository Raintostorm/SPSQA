using System;
using System.Linq;
using Project.Data;
using Project.Meta;
namespace UnityEngine
{
    public class Sprite{}
    public static class Mathf{public static int Max(int a,int b)=>Math.Max(a,b);public static int Clamp(int a,int b,int c)=>Math.Min(c,Math.Max(b,a));}
}
namespace Project.Data
{
    public enum Rank{Common,Rare,Epic,Legendary}
    public class GameBalance{public Rank TenRollFloor=Rank.Epic;public int LegendaryPity=80;public int[] TreasureWeights={70,23,6,1};}
    public class TreasureDatabase{public TreasureItem Item;public System.Collections.Generic.List<TreasureItem> OfTier(Rank tier)=>new System.Collections.Generic.List<TreasureItem>{Item};public TreasureItem Find(string id)=>Item;}
}
namespace Project.Meta{public static class SaveSystem{public static SaveData Current=new SaveData();public static void Save(){}}}
class RelicUpgradeTests
{
    static void Assert(bool ok,string message){if(!ok)throw new Exception(message);}
    static void Main()
    {
        var item=new TreasureItem{Id="torch",Tier=Rank.Common,MaxLevel=10};var db=new TreasureDatabase{Item=item};var save=SaveSystem.Current;
        save.Gold=1000;
        var result=TreasureRoll.Roll(10,db,new GameBalance(),new Random(42));
        var owned=save.FindItem(item.Id);
        Assert(owned.Level==1 && owned.Shards==10 && result.All(r=>!r.LeveledUp && r.ShardsGranted==1),"Roll auto-upgraded");
        Assert(TreasureRoll.TryUpgrade(save,item)&&save.Gold==0&&owned.Shards==8&&owned.Level==2,"Manual upgrade transaction");
        Assert(!TreasureRoll.TryUpgrade(save,item)&&owned.Shards==8,"Insufficient gold changed shards");
        save.Gold=10000;owned.Shards=0;
        Assert(!TreasureRoll.TryUpgrade(save,item)&&save.Gold==10000,"Insufficient shards changed gold");
        owned.Level=10;owned.Shards=999;
        Assert(!TreasureRoll.TryUpgrade(save,item)&&save.Gold==10000,"Max upgraded");
        var maxRoll=TreasureRoll.Roll(1,db,new GameBalance(),new Random(1));
        Assert(owned.Shards==999 && maxRoll[0].ShardsGranted==0,"Max roll changed shards or misreported grant");
        save.Items.Clear();
        var first=TreasureRoll.Roll(1,db,new GameBalance(),new Random(1));
        Assert(first[0].IsNew && first[0].ShardsGranted==1 && save.FindItem(item.Id).Shards==1,"First roll must unlock and grant 1 shard");
        var duplicate=TreasureRoll.Roll(1,db,new GameBalance(),new Random(2));
        Assert(!duplicate[0].IsNew && duplicate[0].ShardsGranted==1 && save.FindItem(item.Id).Shards==2,"Duplicate roll must grant 1 shard");
        Console.WriteLine("PASS: 1 shard per new/duplicate relic roll, accurate grant totals, unchanged MAX behavior; rolls only grant shards after unlock; manual gold+shard spend; insufficient resources; max level.");
    }
}
