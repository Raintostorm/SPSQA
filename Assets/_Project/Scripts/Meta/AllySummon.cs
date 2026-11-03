using System;
using System.Collections.Generic;
using System.Linq;
using Project.Data;
using UnityEngine;

namespace Project.Meta
{
    // Explicit runtime references keep the full roster available in player builds.
    public sealed class AllySummon : ScriptableObject
    {
        public GameBalance Balance;
        public UnitDefinition[] Units;
        public static int ShardsFor(int level) => level + 1;
        public struct Result { public UnitDefinition Unit; public int Shards; public bool MythicStone; }
        public static bool IsMythic(string id)=>MythicCatalog.All.Any(u=>u.UnitId==id);
        public static int UpgradeMaterials(string id)=>IsMythic(id)?SaveSystem.Current.MythicStones:(SaveSystem.Current.FindUnit(id)?.Shards??0);
        // Always rarer than the configured ordinary Legendary chance.
        public static double StoneChance(GameBalance balance)
        {var w=balance?.TreasureWeights;if(w==null||w.Length!=4)return 0;double total=w.Sum(x=>Math.Max(0,x));return total>0?Math.Min(.002,Math.Max(0,w[3])/total*.2):0;}

        public static bool TryUpgrade(string unitId, GameBalance balance)
        {
            var save=SaveSystem.Current;
            bool mythic=IsMythic(unitId);
            var row=save.FindUnit(unitId);
            int level=row?.Level??1;
            if((row==null&&!mythic) || level>=(balance!=null?balance.UnitMaxLevel:30))return false;
            int gold=(balance!=null?balance.UpgradeBaseCost:500)*level;
            int shards=ShardsFor(level);
            if(save.Gold<gold || UpgradeMaterials(unitId)<shards)return false;
            if(row==null){row=new SaveData.UnitUpgrade{UnitId=unitId};save.Units.Add(row);}
            save.Gold-=gold;if(mythic)save.MythicStones-=shards;else row.Shards-=shards;row.Level++;
            SaveSystem.Save();return true;
        }

        public bool TryRoll(int count, System.Random rng, out List<Result> results)
        {
            results = new List<Result>();
            if ((count != 1 && count != 10) || Balance == null || rng == null) return false;
            var pools = new List<UnitDefinition>[4];
            for (int i=0;i<4;i++) pools[i]=new List<UnitDefinition>();
            if (Units != null) foreach(var unit in Units)
                if(unit != null && !string.IsNullOrEmpty(unit.UnitId) && (int)unit.Rank>=0 && (int)unit.Rank<4)
                    pools[(int)unit.Rank].Add(unit);
            // Validate all pools before charging; a missing asset must never eat gems.
            foreach(var pool in pools) if(pool.Count==0) return false;
            var weights=Balance.TreasureWeights;
            if(weights==null || weights.Length!=4) return false;
            int total=0;foreach(var weight in weights) total+=Mathf.Max(0,weight);
            if(total<=0 || (int)Balance.TenRollFloor<0 || (int)Balance.TenRollFloor>3) return false;
            var save=SaveSystem.Current;
            if(save.Gems<count*10) return false;
            var pity=save.SummonsSinceLegendary;
            bool floor=count==10;
            for(int i=0;i<count;i++)
            {
                int tier=0;
                bool guaranteedLegend=Balance.LegendaryPity>0 && pity>=Balance.LegendaryPity;
                bool guaranteedFloor=floor && i==count-1;
                if(!guaranteedLegend&&!guaranteedFloor&&rng.NextDouble()<StoneChance(Balance))
                {results.Add(new Result{MythicStone=true,Shards=20});pity++;continue;}
                if(Balance.LegendaryPity>0 && pity>=Balance.LegendaryPity) tier=3;
                else
                {
                    int pick=rng.Next(total);
                    for(int t=0;t<4;t++){pick-=Mathf.Max(0,weights[t]);if(pick<0){tier=t;break;}}
                }
                if(floor && i==count-1) tier=Mathf.Max(tier,(int)Balance.TenRollFloor);
                if(tier>=(int)Balance.TenRollFloor) floor=false;
                pity=tier==3?0:pity+1;
                results.Add(new Result { Unit=pools[tier][rng.Next(pools[tier].Count)], Shards=20 });
            }
            save.Gems-=count*10;
            save.SummonsSinceLegendary=pity;
            foreach(var result in results)
            {
                if(result.MythicStone){save.MythicStones+=result.Shards;continue;}
                var row=save.FindUnit(result.Unit.UnitId);
                if(row==null){row=new SaveData.UnitUpgrade { UnitId=result.Unit.UnitId };save.Units.Add(row);}
                row.Shards+=result.Shards;
            }
            SaveSystem.Save();
            return true;
        }
    }
}
