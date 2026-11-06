using UnityEngine;
namespace Project.Battle
{
    // Fixed seven-particle loop, no spawning or per-frame allocations. Entire effect stays below units.
    public sealed class BloodRainGroundVisual:MonoBehaviour
    {
        const int Count=7,Order=BattleGrid.TileOrder+103;
        const float Duration=1.65f;
        static Sprite drop;static Material lineMaterial;
        readonly SpriteRenderer[] droplets=new SpriteRenderer[Count];
        readonly LineRenderer[] ripples=new LineRenderer[Count];
        float clock;
        static float Noise(int n){float a=Mathf.Sin(n*91.733f+17.1f)*43758.5453f;return a-Mathf.Floor(a);}
        void Awake()
        {
            if(!lineMaterial)lineMaterial=new Material(Shader.Find("Sprites/Default"));
            if(!drop)drop=MakeDrop();
            for(int i=0;i<Count;i++)
            {
                var go=new GameObject("Blood drop");go.transform.SetParent(transform,false);
                droplets[i]=go.AddComponent<SpriteRenderer>();droplets[i].sprite=drop;droplets[i].sortingOrder=Order+1;
                var ring=new GameObject("Landing ripple");ring.transform.SetParent(transform,false);
                var lr=ring.AddComponent<LineRenderer>();lr.sharedMaterial=lineMaterial;lr.loop=true;lr.useWorldSpace=false;
                lr.positionCount=20;lr.startWidth=lr.endWidth=.009f;lr.sortingOrder=Order+1;
                for(int j=0;j<20;j++){float a=j*Mathf.PI*2/20;lr.SetPosition(j,new Vector3(Mathf.Cos(a),Mathf.Sin(a)*.45f,0));}
                ripples[i]=lr;
            }
            Draw();
        }
        // A tiny plump cel-shaded droplet, matching the continuous browser preview.
        static Sprite MakeDrop()
        {
            const int w=32,h=48;var mask=new bool[w*h];var pixels=new Color32[w*h];
            for(int y=0;y<h;y++)for(int x=0;x<w;x++)
            {
                float dx=x-15.5f,dy=y-15;
                mask[y*w+x]=(dx*dx+dy*dy<=100)||(y>=15&&y<=41&&Mathf.Abs(dx)<=(41-y)*.38f);
            }
            for(int y=0;y<h;y++)for(int x=0;x<w;x++)
            {
                if(!mask[y*w+x])continue;
                bool edge=false;for(int yy=-2;yy<=2;yy++)for(int xx=-2;xx<=2;xx++)
                {int sx=x+xx,sy=y+yy;if(sx<0||sx>=w||sy<0||sy>=h||!mask[sy*w+sx])edge=true;}
                pixels[y*w+x]=edge?new Color32(85,43,53,255):new Color32(213,76,83,255);
                if(!edge&&(x-12)*(x-12)+(y-19)*(y-19)<7)pixels[y*w+x]=new Color32(244,129,123,255);
            }
            var tex=new Texture2D(w,h,TextureFormat.RGBA32,false);tex.SetPixels32(pixels);tex.filterMode=FilterMode.Bilinear;tex.wrapMode=TextureWrapMode.Clamp;tex.Apply();
            return Sprite.Create(tex,new Rect(0,0,w,h),Vector2.one*.5f,100);
        }
        // Sua .cs luc dang Play: Unity tao moi mang readonly rong -> dung lai giot/vong thay vi bao loi moi khung.
        void Update(){if(!droplets[0]){foreach(Transform c in transform)Destroy(c.gameObject);Awake();}clock+=Time.deltaTime;Draw();}
        void Draw()
        {
            bool show=Project.UI.SharedSettings.EffectsEnabled;
            for(int i=0;i<Count;i++)
            {
                float full=clock/Duration+i/(float)Count;int cycle=Mathf.FloorToInt(full);float p=full-cycle;
                float x=(Noise(i*19+cycle*7)-.5f)*.80f,ground=(Noise(i*31+cycle*11+4)-.5f)*.70f;
                var sr=droplets[i];var lr=ripples[i];sr.enabled=show&&p<.66f;lr.enabled=show&&p>=.66f;
                if(p<.66f)
                {
                    float q=p/.66f,y=ground+.24f*(1-q*q);
                    sr.transform.localPosition=new Vector3(x,y,0);sr.transform.localScale=new Vector3(.13f,.13f*(1+.08f*q),1);
                    sr.color=new Color(1,1,1,Mathf.Min(1,q/.15f)*.82f);
                }
                else
                {
                    float q=(p-.66f)/.34f,r=.014f+q*.06f;
                    lr.transform.localPosition=new Vector3(x,ground,0);lr.transform.localScale=new Vector3(r,r,1);
                    lr.startColor=lr.endColor=new Color(.62f,.25f,.29f,(1-q)*(1-q)*.45f);
                }
            }
        }
    }
}
