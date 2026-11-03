using UnityEngine;
namespace Project.Visual
{
    public static class StatusTint
    {
        public static Color Compose(float alpha,bool frozen,bool slowed,bool effectsEnabled,bool burned=false)
        {
            var tint=effectsEnabled&&frozen?new Color(.45f,.85f,1f):Color.white;
            if(effectsEnabled&&slowed){tint.r*=.65f;tint.g*=.85f;}
            if(effectsEnabled&&burned){tint.r=1f;tint.g*=.3f;tint.b*=.22f;}
            tint.a=alpha;
            return tint;
        }
    }
}
