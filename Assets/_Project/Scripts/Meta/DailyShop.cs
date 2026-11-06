using System;
using System.Collections.Generic;
using System.Linq;
using Project.Data;
using UnityEngine;
namespace Project.Meta
{
    [Serializable] public sealed class ShopPrices
    {
        public int GoldPerGem=100;
        public int AllyShards=20,RelicShards=5;
        public int[] AllyGemPrices={15,30,75,150};
        public int[] RelicGemPrices={10,25,60,120};
        public int[] ExchangeGemCosts={100,500,1000};
        // Doi gems ra chia khoa ruong. Keys chi roi tu phan thuong man choi, nen nguoi choi
        // het chia la ket. Gia mac dinh 5 gems/chia; sua trong Resources/Shop/prices.json.
        public int GemsPerKey=5;
        public int[] KeyExchangeCounts={10,50,100};
        public static ShopPrices Load()
        {
            var text=Resources.Load<TextAsset>("Shop/prices");
            return text?JsonUtility.FromJson<ShopPrices>(text.text):new ShopPrices();
        }
    }
    public static class DailyShop
    {
        // Daily reset at midnight UTC+7; no paid/manual refresh.
        public static string Day(DateTime utc)=>utc.ToUniversalTime().AddHours(7).ToString("yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture);
        public static bool Ensure(SaveData save,string day,UnitDefinition[] units,TreasureDatabase relics,ShopPrices prices,System.Random rng)
        {
            if(save.DailyShopDay==day && save.DailyShopOffers!=null && save.DailyShopOffers.Count==9)return false;
            // Do not rewind the shop when the device clock moves backwards.
            if(!string.IsNullOrEmpty(save.DailyShopDay) && string.CompareOrdinal(day,save.DailyShopDay)<0 && save.DailyShopOffers?.Count==9)return false;
            if(units==null||relics==null||prices==null||rng==null)return false;
            var allies=units.Where(u=>u && !string.IsNullOrEmpty(u.UnitId)).GroupBy(u=>u.UnitId).Select(g=>g.First()).ToList();
            var items=relics.Items.Where(i=>i!=null && !string.IsNullOrEmpty(i.Id)).GroupBy(i=>i.Id).Select(g=>g.First()).ToList();
            if(allies.Count<5||items.Count<4)return false;
            var offers=new List<SaveData.ShopOffer>();
            for(int i=0;i<5;i++){int index=rng.Next(allies.Count);var u=allies[index];allies.RemoveAt(index);int tier=(int)u.Rank;if(tier<0||tier>=prices.AllyGemPrices.Length)return false;offers.Add(new SaveData.ShopOffer{Id=u.UnitId,Ally=true,Count=prices.AllyShards,GemPrice=prices.AllyGemPrices[tier]});}
            for(int i=0;i<4;i++){int index=rng.Next(items.Count);var item=items[index];items.RemoveAt(index);int tier=(int)item.Tier;if(tier<0||tier>=prices.RelicGemPrices.Length)return false;offers.Add(new SaveData.ShopOffer{Id=item.Id,Count=prices.RelicShards,GemPrice=prices.RelicGemPrices[tier]});}
            for(int i=offers.Count-1;i>0;i--){int j=rng.Next(i+1);var temp=offers[i];offers[i]=offers[j];offers[j]=temp;}
            save.DailyShopDay=day;save.DailyShopOffers=offers;return true;
        }
        public static bool Buy(SaveData save,int slot,UnitDefinition[] units,TreasureDatabase relics)
        {
            if(save.DailyShopOffers==null||slot<0||slot>=save.DailyShopOffers.Count)return false;
            var o=save.DailyShopOffers[slot];
            if(o==null||o.Sold||o.Count<=0||o.GemPrice<=0||save.Gems<o.GemPrice)return false;
            if(o.Ally)
            {
                if(units==null||!units.Any(u=>u && u.UnitId==o.Id))return false;
                var row=save.FindUnit(o.Id);
                if(row!=null && row.Shards>int.MaxValue-o.Count)return false;
                if(row==null){row=new SaveData.UnitUpgrade{UnitId=o.Id};save.Units.Add(row);}
                row.Shards+=o.Count;
            }
            else
            {
                var item=relics?.Find(o.Id);if(item==null)return false;
                var row=save.FindItem(o.Id);
                if(row!=null && row.Shards>int.MaxValue-o.Count)return false;
                // First acquisition unlocks Lv.1, just like chest rewards; never upgrades an owned relic.
                if(row==null){row=new SaveData.OwnedItem{Id=o.Id};save.Items.Add(row);}
                row.Shards+=o.Count;
            }
            save.Gems-=o.GemPrice;o.Sold=true;return true;
        }
        public static bool ExchangeKeys(SaveData save,int keys,ShopPrices prices)
        {
            if(prices==null||prices.KeyExchangeCounts==null||!prices.KeyExchangeCounts.Contains(keys)||keys<=0||prices.GemsPerKey<=0)return false;
            long gems=(long)keys*prices.GemsPerKey;
            if(gems>int.MaxValue||save.Gems<gems)return false;
            if(keys>int.MaxValue-save.TreasureKeys)return false;
            save.Gems-=(int)gems;save.TreasureKeys+=keys;return true;
        }
        public static bool Exchange(SaveData save,int gems,ShopPrices prices)
        {
            if(prices==null||prices.ExchangeGemCosts==null||!prices.ExchangeGemCosts.Contains(gems)||gems<=0||prices.GoldPerGem<=0||save.Gems<gems)return false;
            long gold=(long)gems*prices.GoldPerGem;if(gold>int.MaxValue-save.Gold)return false;
            save.Gems-=gems;save.Gold+=(int)gold;return true;
        }
    }
}
