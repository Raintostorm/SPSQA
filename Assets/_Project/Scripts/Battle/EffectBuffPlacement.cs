using System.Collections.Generic;
namespace Project.Battle
{
    // Simulates promotion placements without changing the real board or input arrays.
    public static class EffectBuffPlacement
    {
        public static bool TryPlan<T>(T[,] occupied,int[,] occupancy,T[] incoming,int capacity,out int[] cells)
        {
            int rows=occupancy.GetLength(0),cols=occupancy.GetLength(1);
            var definitions=(T[,])occupied.Clone();var counts=(int[,])occupancy.Clone();
            var result=new int[incoming.Length];cells=System.Array.Empty<int>();
            for(int i=0;i<incoming.Length;i++)
            {
                bool found=false;
                for(int pass=0;pass<2&&!found;pass++)for(int r=0;r<rows&&!found;r++)for(int c=0;c<cols;c++)
                {
                    bool match=pass==0?counts[r,c]>0&&EqualityComparer<T>.Default.Equals(definitions[r,c],incoming[i])&&counts[r,c]<capacity:counts[r,c]==0;
                    if(!match)continue;
                    definitions[r,c]=incoming[i];counts[r,c]++;result[i]=r*cols+c;found=true;break;
                }
                if(!found)return false;
            }
            cells=result;return true;
        }
    }
}
