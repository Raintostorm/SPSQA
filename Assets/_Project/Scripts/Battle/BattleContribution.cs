using System.Collections.Generic;
using Project.Data;
using Project.Gacha;
using UnityEngine;

namespace Project.Battle
{
    // Run-wide ledger, including allies later killed, sold or merged.
    public static class BattleContribution
    {
        public sealed class Entry { public UnitDefinition Definition; public float Dealt,Taken; }
        static readonly Dictionary<int,Entry> rows=new Dictionary<int,Entry>();
        public static IEnumerable<Entry> Rows=>rows.Values;
        public static int Rolls,Recruited,Merges,SkillCasts,Summons;
        public static readonly int[] Rarity=new int[4];
        public static void Reset(){rows.Clear();Rolls=Recruited=Merges=SkillCasts=Summons=0;System.Array.Clear(Rarity,0,4);}
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize()=>Reset();
        public static void Damage(Unit unit,float amount,bool taken)
        {
            if(!unit || unit.Team!=UnitTeam.Ally || !unit.Definition || amount<=0)return;
            int id=unit.GetInstanceID();
            if(!rows.TryGetValue(id,out var row)){row=new Entry { Definition=unit.Definition };rows.Add(id,row);}
            if(taken)row.Taken+=amount;else row.Dealt+=amount;
        }
        public static void Roll(RollPayout payout){Rolls++;foreach(var grant in payout.AllyGrants)Rarity[(int)grant.Rank]++;}
    }
}
