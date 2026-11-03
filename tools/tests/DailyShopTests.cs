using System;
using System.Linq;
using Project.Meta;
using Project.Data;
namespace UnityEngine
{
    public class Object{public static implicit operator bool(Object x)=>x!=null;}
    public class TextAsset:Object{public string text;}
    public static class Resources{public static T Load<T>(string path) where T:class=>null;}
    public static class JsonUtility{public static T FromJson<T>(string text)=>default;}
}
namespace Project.Data
{
    public enum Rank{Common,Rare,Epic,Legendary}
    public class UnitDefinition:UnityEngine.Object{public string UnitId;public Rank Rank;}
    public class TreasureItem{public string Id;public Rank Tier;}
    public class TreasureDatabase:UnityEngine.Object{public TreasureItem[] Items;public TreasureItem Find(string id)=>Items.FirstOrDefault(i=>i.Id==id);}
}
class DailyShopTests
{
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    static void Main()
    {
        var units=Enumerable.Range(0,20).Select(i=>new UnitDefinition{UnitId="u"+i,Rank=(Rank)(i%4)}).ToArray();
        var db=new TreasureDatabase{Items=Enumerable.Range(0,30).Select(i=>new TreasureItem{Id="r"+i,Tier=(Rank)(i%4)}).ToArray()};
        var save=new SaveData{Gems=10000};var prices=new ShopPrices();
        Check(DailyShop.Ensure(save,"2026-10-24",units,db,prices,new Random(42)),"first daily offers");
        Check(save.DailyShopOffers.Count==9&&save.DailyShopOffers.Count(o=>o.Ally)==5&&save.DailyShopOffers.Select(o=>o.Id).Distinct().Count()==9,"9 unique mixed offers");
        var first=save.DailyShopOffers;
        Check(!DailyShop.Ensure(save,"2026-10-24",units,db,prices,new Random(99))&&object.ReferenceEquals(first,save.DailyShopOffers),"reopen rerolled shop");
        var offer=first[0];int before=save.Gems;
        Check(DailyShop.Buy(save,0,units,db)&&save.Gems==before-offer.GemPrice&&offer.Sold,"atomic purchase");
        Check(!DailyShop.Buy(save,0,units,db)&&save.Gems==before-offer.GemPrice,"duplicate purchase");
        save.Gems=0;Check(!DailyShop.Buy(save,1,units,db)&&!first[1].Sold,"insufficient gems");
        save.Gems=10000;before=save.Gems;int gold=save.Gold;
        Check(DailyShop.Exchange(save,100,prices)&&save.Gems==before-100&&save.Gold==gold+10000,"gem gold exchange");
        Check(!DailyShop.Exchange(save,-100,prices)&&!DailyShop.Exchange(save,1,prices),"invalid exchange");
        Check(!DailyShop.Ensure(save,"2026-10-23",units,db,prices,new Random()),"clock rollback refreshed shop");
        Check(DailyShop.Ensure(save,"2026-10-25",units,db,prices,new Random(43))&&save.DailyShopOffers.All(o=>!o.Sold),"next day refresh");
        int ri=save.DailyShopOffers.FindIndex(o=>!o.Ally);var ro=save.DailyShopOffers[ri];save.Items.Add(new SaveData.OwnedItem{Id=ro.Id,Level=4,Shards=7});
        Check(DailyShop.Buy(save,ri,units,db)&&save.FindItem(ro.Id).Level==4&&save.FindItem(ro.Id).Shards==7+ro.Count,"relic auto upgraded");
        Check(DailyShop.Day(new DateTime(2026,10,24,16,59,0,DateTimeKind.Utc))=="2026-10-24"&&DailyShop.Day(new DateTime(2026,10,24,17,0,0,DateTimeKind.Utc))=="2026-10-25","UTC+7 reset");
        Console.WriteLine("PASS: 9 unique offers, daily persistence/reset, single purchase, atomic gems/shards, exchange, no auto upgrade, UTC+7 boundary.");
    }
}
