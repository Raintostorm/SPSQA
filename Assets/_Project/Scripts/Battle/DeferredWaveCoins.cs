using System.Collections.Generic;
namespace Project.Battle
{
    /// <summary>Resolved spells earn battle coins for the following wave. Claims are idempotent.</summary>
    public sealed class DeferredWaveCoins
    {
        readonly Dictionary<int,int> due=new();
        public void Earn(int wave,int coins)
        {if(wave<1||coins<=0)return;int next=wave+1;due.TryGetValue(next,out int amount);due[next]=amount+coins;}
        public int Claim(int wave)
        {
            int total=0;var paid=new List<int>();foreach(var row in due)if(row.Key<=wave){total+=row.Value;paid.Add(row.Key);}
            foreach(int key in paid)due.Remove(key);return total;
        }
        public void Reset()=>due.Clear();
    }
}
