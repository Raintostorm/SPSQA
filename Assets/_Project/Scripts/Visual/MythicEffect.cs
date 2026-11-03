using Project.Battle;
using Project.Data;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>Independent VFX lifetime: never cancels gameplay, follows game time and effect settings.</summary>
    public sealed class MythicEffect : MonoBehaviour
    {
        Sprite[] frames;SpriteRenderer art;Unit owner;BattleFlow flow;Vector3 origin;float life,elapsed,width;bool loop,follow;string effectKey;
        public static MythicEffect Play(string key,Unit caster,Vector3 point,float size,float seconds,bool looping=false,bool attached=false)
        {
            if(!Project.UI.SharedSettings.EffectsEnabled)return null;
            if(key=="shield"||key=="buff")foreach(var old in FindObjectsByType<MythicEffect>(FindObjectsSortMode.None))if(old.effectKey==key && old.owner==caster){old.gameObject.SetActive(false);Destroy(old.gameObject);}
            var frames=MythicCatalog.Effect(key);if(frames.Length==0)return null;
            var fx=new GameObject("Mythic VFX "+key).AddComponent<MythicEffect>();fx.effectKey=key;fx.frames=frames;fx.owner=caster;fx.origin=point;fx.width=size;fx.life=Mathf.Max(.05f,seconds);fx.loop=looping;fx.follow=attached;
            fx.art=fx.gameObject.AddComponent<SpriteRenderer>();fx.flow=FindFirstObjectByType<BattleFlow>();if(fx.flow)fx.flow.PhaseChanged+=fx.Phase;fx.Draw();return fx;
        }
        void Phase(BattleFlow.Phase p){if(p!=BattleFlow.Phase.Fighting&&p!=BattleFlow.Phase.Marching)Destroy(gameObject);}
        void OnDestroy(){if(flow)flow.PhaseChanged-=Phase;}
        void Update(){elapsed+=Time.deltaTime;if(elapsed>=life || follow&&(!owner||!owner.IsAlive) || effectKey=="shield"&&owner&&owner.Mythic.Shield<=0){Destroy(gameObject);return;}Draw();}
        void Draw()
        {
            art.enabled=Project.UI.SharedSettings.EffectsEnabled;
            art.sprite=frames[loop?(int)(elapsed/.16f)%frames.Length:Mathf.Min(frames.Length-1,(int)(elapsed/life*frames.Length))];
            transform.position=follow?owner.transform.position:origin;
            float spriteWidth=art.sprite.bounds.size.x;transform.localScale=Vector3.one*(width/Mathf.Max(.01f,spriteWidth));
            art.sortingOrder=145;art.color=new Color(1,1,1,Mathf.Min(effectKey=="buff"?.65f:1,(life-elapsed)/.15f));
        }
        public static void Lightning(Vector3 a,Vector3 b)
        {
            if(!Project.UI.SharedSettings.EffectsEnabled)return;
            var go=new GameObject("Chain lightning");var fx=go.AddComponent<LightningArc>();fx.Initialize(a,b);
        }
    }
    public sealed class LightningArc:MonoBehaviour
    {
        static Material material;LineRenderer outer,core;float age;
        public void Initialize(Vector3 a,Vector3 b)
        {
            if(!material)material=new Material(Shader.Find("Sprites/Default"));
            outer=Line("Blue outline",new Color(.08f,.4f,1f),.14f,151);core=Line("White core",new Color(.8f,1,1),.055f,152);
            var points=new Vector3[9];var normal=Vector3.Cross((b-a).normalized,Vector3.forward);
            for(int i=0;i<9;i++)points[i]=Vector3.Lerp(a,b,i/8f)+(i==0||i==8?Vector3.zero:normal*(i%2==0?.12f:-.12f));
            outer.SetPositions(points);core.SetPositions(points);
        }
        LineRenderer Line(string n,Color c,float w,int order){var l=new GameObject(n).AddComponent<LineRenderer>();l.transform.SetParent(transform);l.sharedMaterial=material;l.positionCount=9;l.startWidth=l.endWidth=w;l.startColor=l.endColor=c;l.sortingOrder=order;return l;}
        void Update(){age+=Time.deltaTime;outer.enabled=core.enabled=Project.UI.SharedSettings.EffectsEnabled;if(age>=.24f)Destroy(gameObject);}
    }
}

namespace Project.Visual
{
    public sealed class RevivalSeal:UnityEngine.MonoBehaviour
    {
        UnityEngine.SpriteRenderer art;Project.Battle.Unit unit;
        void Start(){unit=GetComponent<Project.Battle.Unit>();art=new UnityEngine.GameObject("Revived once seal").AddComponent<UnityEngine.SpriteRenderer>();art.transform.SetParent(transform,false);art.sprite=Project.Data.MythicCatalog.Effect("seal")[0];art.sortingOrder=153;}
        void LateUpdate(){if(!art)return;art.enabled=unit.IsAlive&&Project.UI.SharedSettings.EffectsEnabled;art.transform.position=unit.HitPoint+UnityEngine.Vector3.up*.5f;art.transform.localScale=UnityEngine.Vector3.one*(.5f/UnityEngine.Mathf.Max(.01f,art.sprite.bounds.size.x));}
    }
}

namespace Project.Visual
{
    public sealed class MythicProjectileArt:UnityEngine.MonoBehaviour
    {
        UnityEngine.SpriteRenderer art;UnityEngine.Sprite[] frames;float age;
        public void Bind(string key){art=GetComponent<UnityEngine.SpriteRenderer>();frames=Project.Data.MythicCatalog.Effect(key);art.sprite=frames[0];}
        void Update(){age+=UnityEngine.Time.deltaTime;art.sprite=frames[(int)(age/.08f)%frames.Length];art.enabled=true;}
    }
}
