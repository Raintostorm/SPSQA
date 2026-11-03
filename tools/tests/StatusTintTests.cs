using System;
using Project.Visual;
namespace UnityEngine
{
    public struct Color
    {
        public float r,g,b,a;
        public Color(float r,float g,float b,float a=1){this.r=r;this.g=g;this.b=b;this.a=a;}
        public static Color white=>new Color(1,1,1);
    }
}
class StatusTintTests
{
    static void Near(float x,float y){if(Math.Abs(x-y)>.00001)throw new Exception($"Expected {y}, got {x}");}
    static void Main()
    {
        var c=StatusTint.Compose(1,false,true,true);
        for(int i=0;i<10000;i++)c=StatusTint.Compose(c.a,false,true,true);
        Near(c.r,.65f);Near(c.g,.85f);Near(c.b,1);
        c=StatusTint.Compose(.37f,true,true,true);
        Near(c.r,.45f*.65f);Near(c.g,.85f*.85f);Near(c.b,1);Near(c.a,.37f);
        c=StatusTint.Compose(c.a,true,false,true);Near(c.r,.45f);Near(c.g,.85f);Near(c.a,.37f);
        c=StatusTint.Compose(c.a,false,false,true);Near(c.r,1);Near(c.g,1);Near(c.a,.37f);
        c=StatusTint.Compose(0,false,true,true);Near(c.a,0);
        c=StatusTint.Compose(1,false,false,true);Near(c.r,1);Near(c.g,1);Near(c.a,1);
        c=StatusTint.Compose(.5f,true,true,false);Near(c.r,1);Near(c.g,1);Near(c.b,1);Near(c.a,.5f);
        c=StatusTint.Compose(.4f,false,false,true,true);Near(c.r,1);Near(c.g,.3f);Near(c.b,.22f);Near(c.a,.4f);
        c=StatusTint.Compose(.4f,false,false,false,true);Near(c.r,1);Near(c.g,1);Near(c.b,1);Near(c.a,.4f);
        Console.WriteLine("PASS: stable slow over 10000 frames; freeze+slow composition; expiration/reset restores RGB; death/reveal alpha preserved; Effects off removes tint.");
    }
}
