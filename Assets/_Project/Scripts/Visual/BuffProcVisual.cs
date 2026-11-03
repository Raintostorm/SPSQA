using System.Collections.Generic;
using Project.Battle;
using UnityEngine;
namespace Project.Visual
{
    // Independent status lifetimes; one reused renderer per type and one mana orbit per unit.
    [DefaultExecutionOrder(120)]
    public sealed class BuffProcVisual:MonoBehaviour
    {
        sealed class Slot{public SpriteRenderer icon,frame;public float until;}
        readonly Dictionary<EffectBuffKind,Slot> slots=new();readonly List<EffectBuffKind> shown=new();
        Unit unit;FrameUnitVisual visual;SpriteRenderer orbit;SpriteRenderer[] bars;
        bool orbitActive;float orbitAge;
        static readonly EffectBuffKind[] kinds={EffectBuffKind.EmberTouch,EffectBuffKind.ManaFlow,EffectBuffKind.Bodyguard,EffectBuffKind.FrostTouch,EffectBuffKind.MythicMight,EffectBuffKind.SoulHarvest};
        void Awake(){unit=GetComponent<Unit>();}
        void Start(){visual=GetComponent<FrameUnitVisual>();var health=GetComponentInChildren<HealthBar>(true);bars=health?health.GetComponentsInChildren<SpriteRenderer>(true):System.Array.Empty<SpriteRenderer>();}
        public static void Play(Unit target,EffectBuffKind kind)
        {
            if(!target)return;
            var fx=target.GetComponent<BuffProcVisual>();if(!fx)fx=target.gameObject.AddComponent<BuffProcVisual>();
            fx.Ensure(kind).until=Time.time+(kind==EffectBuffKind.ManaFlow?1.8f:.8f);
            if(kind==EffectBuffKind.ManaFlow&&!fx.orbitActive&&Project.UI.SharedSettings.EffectsEnabled)
            {fx.orbitActive=true;fx.orbitAge=0;}
        }
        SpriteRenderer Renderer(string name)
        {var go=new GameObject(name);go.transform.SetParent(transform,false);return go.AddComponent<SpriteRenderer>();}
        Slot Ensure(EffectBuffKind kind)
        {
            if(slots.TryGetValue(kind,out var slot))return slot;
            slot=new Slot{icon=Renderer("Status "+kind),frame=Renderer("Status frame "+kind)};
            slot.frame.sprite=Project.UI.BattleUiSkin.Navy;slot.frame.drawMode=SpriteDrawMode.Sliced;slot.icon.enabled=slot.frame.enabled=false;slots.Add(kind,slot);return slot;
        }
        void Scale(Transform node,float width,Sprite sprite)
        {
            if(!sprite)return;float factor=width/Mathf.Max(sprite.bounds.size.x,sprite.bounds.size.y);
            var scale=transform.lossyScale;node.localScale=new Vector3(factor/(Mathf.Abs(scale.x)<.001f?.001f:scale.x),factor/(Mathf.Abs(scale.y)<.001f?.001f:scale.y),1);
        }
        bool Persistent(EffectBuffKind kind)=>kind switch
        {
            EffectBuffKind.EmberTouch=>unit.Effects&&unit.Effects.Burning,
            EffectBuffKind.Bodyguard=>unit.Effects&&unit.Effects.Shield>0||unit.Combat&&unit.Combat.Shield>0||unit.Mythic&&unit.Mythic.Shield>0,
            EffectBuffKind.FrostTouch=>unit.Effects&&unit.Effects.Slowed,
            _=>false
        };
        void LateUpdate()
        {
            if(!unit)return;
            bool alive=unit.IsAlive&&unit.Mythic&&unit.Mythic.Deployed;
            bool visible=alive&&Project.UI.SharedSettings.EffectsEnabled;
            shown.Clear();
            foreach(var kind in kinds)
            {
                bool on=alive&&(Persistent(kind)||slots.TryGetValue(kind,out var existing)&&Time.time<existing.until);
                if(on){Ensure(kind);shown.Add(kind);}
            }
            foreach(var slot in slots.Values)slot.icon.enabled=slot.frame.enabled=false;
            if(!alive){Clear();return;}
            var body=visual?visual.BodyRenderer:null;var bounds=body?body.bounds:new Bounds(unit.HitPoint,Vector3.one*.6f);
            float top=bounds.max.y;
            if(bars!=null)foreach(var bar in bars)if(bar&&bar.enabled)top=Mathf.Max(top,bar.bounds.max.y);
            float size=Mathf.Clamp(bounds.size.y*.22f,.12f,.22f),gap=size*1.2f;
            for(int i=0;i<shown.Count;i++)
            {
                var kind=shown[i];var slot=slots[kind];int row=i/4,start=row*4,count=Mathf.Min(4,shown.Count-start);
                var point=new Vector3(bounds.center.x+(i-start-(count-1)*.5f)*gap,top+size*.8f+row*gap,transform.position.z);
                slot.icon.sprite=kind==EffectBuffKind.EmberTouch?BuffStatusArt.Flame(Time.time):kind==EffectBuffKind.ManaFlow?BuffStatusArt.Mana(Time.time):Project.UI.EffectBuffArt.Get(kind);
                slot.icon.transform.position=slot.frame.transform.position=point;
                slot.icon.sortingLayerID=slot.frame.sortingLayerID=body?body.sortingLayerID:0;
                slot.icon.sortingOrder=(body?body.sortingOrder:150)+6;slot.frame.sortingOrder=slot.icon.sortingOrder-1;
                Scale(slot.icon.transform,size,slot.icon.sprite);
                var parentScale=transform.lossyScale;slot.frame.transform.localScale=new Vector3(1/(Mathf.Abs(parentScale.x)<.001f?.001f:parentScale.x),1/(Mathf.Abs(parentScale.y)<.001f?.001f:parentScale.y),1);slot.frame.size=Vector2.one*(size*1.2f);
                slot.icon.enabled=slot.frame.enabled=visible;
            }
            if(!orbitActive){if(orbit)orbit.enabled=false;return;}
            orbitAge+=Time.deltaTime;float p=orbitAge/1.4f;
            if(p>=1){orbitActive=false;if(orbit)orbit.enabled=false;return;}
            if(!orbit){orbit=Renderer("Mana orbit");orbit.sprite=BuffStatusArt.Get("mana-orb");}
            float angle=p*Mathf.PI*2-Mathf.PI,radius=p<.78f?1:Mathf.Clamp01((1-p)/.22f);
            var center=new Vector3(bounds.center.x,bounds.min.y+bounds.size.y*.45f,transform.position.z);
            orbit.transform.position=center+new Vector3(Mathf.Cos(angle)*Mathf.Clamp(bounds.size.x*.85f,.28f,.65f)*radius,Mathf.Sin(angle)*bounds.size.y*.23f*radius,0);
            orbit.sortingLayerID=body?body.sortingLayerID:0;orbit.sortingOrder=(body?body.sortingOrder:150)+(Mathf.Sin(angle)>=0?2:-1);
            Scale(orbit.transform,Mathf.Clamp(bounds.size.y*.3f,.16f,.28f),orbit.sprite);
            orbit.transform.rotation=Quaternion.Euler(0,0,-angle*Mathf.Rad2Deg-35);
            orbit.color=new Color(1,1,1,Mathf.Min(1,(1-p)*7));orbit.enabled=visible;
        }
        public void Clear(){orbitActive=false;foreach(var slot in slots.Values){slot.until=0;slot.icon.enabled=slot.frame.enabled=false;}if(orbit)orbit.enabled=false;}
        void OnDisable(){Clear();}
    }
}
