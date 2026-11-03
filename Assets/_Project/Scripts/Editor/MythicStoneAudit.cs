#if UNITY_EDITOR
using System;
using System.Linq;
using Project.Data;
using Project.Meta;
using UnityEditor;
using UnityEngine;
namespace Project.EditorTools
{
    public static class MythicStoneAudit
    {
        sealed class StoneRandom:System.Random { public override double NextDouble()=>0;public override int Next(int max)=>0; }
        [MenuItem("SpinSquad/Audit Mythic Stone")]
        static void Run()
        {
            var save=SaveSystem.Current;string snapshot=JsonUtility.ToJson(save);int passed=0;
            void Check(bool ok,string name){if(!ok)throw new Exception(name);passed++;}
            try
            {
                var catalog=Resources.Load<AllySummon>("Summon/Catalog");var rng=new StoneRandom();
                save.Gems=1000;save.Gold=100000;save.MythicStones=0;save.SummonsSinceLegendary=0;
                Check(catalog.TryRoll(1,rng,out var rewards)&&rewards[0].MythicStone&&save.MythicStones==20&&save.Gems==990,"Stone grant and charge");
                save.SummonsSinceLegendary=catalog.Balance.LegendaryPity;
                Check(catalog.TryRoll(1,rng,out rewards)&&rewards[0].Unit.Rank==Rank.Legendary,"Legendary pity preserved");
                save.SummonsSinceLegendary=0;
                Check(catalog.TryRoll(10,rng,out rewards)&&rewards.Count==10&&rewards.Any(r=>r.Unit&&(int)r.Unit.Rank>=(int)catalog.Balance.TenRollFloor),"Ten roll floor preserved");
                var mythics=MythicCatalog.All.Take(2).ToArray();foreach(var d in mythics){var row=save.FindUnit(d.UnitId);if(row==null){row=new SaveData.UnitUpgrade{UnitId=d.UnitId};save.Units.Add(row);}row.Level=1;}
                save.MythicStones=4;int gold=save.Gold;
                Check(AllySummon.TryUpgrade(mythics[0].UnitId,catalog.Balance)&&AllySummon.TryUpgrade(mythics[1].UnitId,catalog.Balance)&&save.MythicStones==0&&save.Gold==gold-catalog.Balance.UpgradeBaseCost*2,"Shared stones, independent levels, gold charged");
                gold=save.Gold;Check(!AllySummon.TryUpgrade(mythics[0].UnitId,catalog.Balance)&&save.Gold==gold,"Insufficient stones no charge");
                save.FindUnit(mythics[0].UnitId).Level=30;save.MythicStones=100;
                Check(!AllySummon.TryUpgrade(mythics[0].UnitId,catalog.Balance)&&save.MythicStones==100,"Max level cap");
                Debug.Log("[MythicStoneAudit] PASS "+passed+"/6");
            }
            finally{JsonUtility.FromJsonOverwrite(snapshot,save);SaveSystem.Save();}
        }
    }
}
#endif
