using System;
using System.Collections.Generic;
namespace Project.Battle
{
    public static class GroundBuffCells
    {
        public static bool TryChoose(IReadOnlyList<int> eligible,ISet<int> occupied,Random random,out int chosen)
        {
            chosen=-1;int free=0;
            foreach(int cell in eligible)if(!occupied.Contains(cell))free++;
            if(free==0)return false;
            int index=random.Next(free);
            foreach(int cell in eligible)if(!occupied.Contains(cell)&&index--==0){chosen=cell;return true;}
            return false;
        }
    }
}
