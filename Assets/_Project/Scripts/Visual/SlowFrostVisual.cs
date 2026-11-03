using UnityEngine;
using Project.Battle;
namespace Project.Visual
{
    // Reused renderers per unit; no object creation during slow refresh/ticking.
    [DefaultExecutionOrder(100)]
    public sealed class SlowFrostVisual:MonoBehaviour
    {
        static Material material;static readonly int SlowId=Shader.PropertyToID("_SlowAmount"),BurnId=Shader.PropertyToID("_BurnAmount");
        Unit unit;SpriteRenderer body;MaterialPropertyBlock block;LineRenderer ring;
        readonly LineRenderer[] sparks=new LineRenderer[4];float clock;
        void Start(){unit=GetComponent<Unit>();body=GetComponent<FrameUnitVisual>()?.BodyRenderer;block=new MaterialPropertyBlock();}
        LineRenderer Line(string label,int points)
        {
            if(!material)material=new Material(Shader.Find("Sprites/Default"));
            var go=new GameObject(label);go.transform.SetParent(transform,false);
            var line=go.AddComponent<LineRenderer>();line.sharedMaterial=material;line.useWorldSpace=true;line.loop=true;line.positionCount=points;line.enabled=false;return line;
        }
        void SetTint(bool on)
        {
            if(!body||!body.sharedMaterial||!body.sharedMaterial.HasProperty(SlowId))return;
            if(block==null)block=new MaterialPropertyBlock();
            body.GetPropertyBlock(block);block.SetFloat(SlowId,on?1:0);block.SetFloat(BurnId,unit&&unit.Effects&&unit.Effects.Burning&&Project.UI.SharedSettings.EffectsEnabled?1:0);body.SetPropertyBlock(block);
        }
        void LateUpdate()
        {
            // Play-mode script reload can preserve the renderer while clearing non-serialized caches.
            if(!unit)unit=GetComponent<Unit>();
            if(!body)body=GetComponent<FrameUnitVisual>()?.BodyRenderer;
            if(block==null)block=new MaterialPropertyBlock();
            bool on=unit&&unit.Effects&&unit.Effects.Slowed&&Project.UI.SharedSettings.EffectsEnabled;
            SetTint(on);
            if(!on){if(ring)ring.enabled=false;foreach(var s in sparks)if(s)s.enabled=false;clock=0;return;}
            if(!ring){ring=Line("Frost pulse",32);for(int i=0;i<4;i++)sparks[i]=Line("Frost sparkle",4);}
            clock+=Time.deltaTime;float p=clock%1.35f/1.35f;
            var bounds=body?body.bounds:new Bounds(transform.position,Vector3.one);
            var feet=new Vector3(bounds.center.x,bounds.min.y+.025f,transform.position.z);
            float width=Mathf.Clamp(bounds.size.x*.55f,.16f,.65f),radius=width*(.85f+.15f*p);
            int order=body?body.sortingOrder:150;
            ring.enabled=true;ring.sortingOrder=order-1;ring.startWidth=ring.endWidth=.025f*(1-.5f*p);
            ring.startColor=ring.endColor=new Color(.55f,.94f,1,.7f*(1-p));
            for(int i=0;i<32;i++){float a=i*Mathf.PI*2/32;ring.SetPosition(i,feet+new Vector3(Mathf.Cos(a)*radius,Mathf.Sin(a)*radius*.23f,0));}
            for(int i=0;i<4;i++)
            {
                float q=(clock/1.4f+i*.25f)%1,alpha=Mathf.Sin(q*Mathf.PI)*.85f;
                var s=sparks[i];s.enabled=true;s.sortingOrder=order+1;s.startWidth=s.endWidth=.017f;
                s.startColor=s.endColor=new Color(.7f,.98f,1,alpha);
                var centre=feet+new Vector3(Mathf.Cos(i*2.4f)*width,q*.25f,0);float r=.025f;
                s.SetPosition(0,centre+Vector3.up*r);s.SetPosition(1,centre+Vector3.right*r);s.SetPosition(2,centre+Vector3.down*r);s.SetPosition(3,centre+Vector3.left*r);
            }
        }
        void OnDisable(){if(block!=null&&body){body.GetPropertyBlock(block);block.SetFloat(SlowId,0);block.SetFloat(BurnId,0);body.SetPropertyBlock(block);}if(ring)ring.enabled=false;foreach(var s in sparks)if(s)s.enabled=false;}
    }
}
