using UnityEngine;
namespace Project.Battle
{
    // Per-instance combat state, never stored on shared definition assets.
    [DefaultExecutionOrder(50)]
    public sealed class EffectBuffUnit:MonoBehaviour
    {
        LineRenderer shieldArt;static Material material;
        Unit unit;int hits,shieldWave=-1;float shield,slowUntil,slowAmount;
        Unit burnSource;float burnDamage;readonly BurnClock burn=new();
        public bool Burning=>unit&&unit.IsAlive&&unit.Mythic&&unit.Mythic.Deployed&&EffectBuffSystem.Current&&EffectBuffSystem.Current.CombatActive&&burn.Active(Time.time);
        public void Burn(Unit source,float damage){burnSource=source;burnDamage=damage;burn.Apply(Time.time);}
        void Update()
        {
            if(!unit.IsAlive||!EffectBuffSystem.Current||!EffectBuffSystem.Current.CombatActive){burn.Clear();return;}
            while(unit.IsAlive&&burn.TakeTick(Time.time))unit.TakeDamage(burnDamage,burnSource,false);
        }
        public float Shield=>shield;
        public bool Slowed=>enabled&&unit&&unit.IsAlive&&unit.Mythic&&unit.Mythic.Deployed&&Time.time<slowUntil;
        public float SpeedMultiplier=>Time.time<slowUntil?Mathf.Max(.2f,1-slowAmount):1f;
        void Awake(){unit=GetComponent<Unit>();if(!GetComponent<Project.Visual.SlowFrostVisual>())gameObject.AddComponent<Project.Visual.SlowFrostVisual>();if(!GetComponent<Project.Visual.BuffProcVisual>())gameObject.AddComponent<Project.Visual.BuffProcVisual>();}
        void OnEnable(){if(unit)unit.Died+=Died;}
        void OnDisable(){if(unit)unit.Died-=Died;burn.Clear();GetComponent<Project.Visual.BuffProcVisual>()?.Clear();}
        void Died(Unit dead){burn.Clear();slowUntil=0;shield=0;GetComponent<Project.Visual.BuffProcVisual>()?.Clear();}
        public void ResetRun(){burn.Clear();burnSource=null;hits=0;shieldWave=-1;shield=0;slowUntil=0;GetComponent<Project.Visual.BuffProcVisual>()?.Clear();}
        public void Slow(float amount){slowAmount=amount;slowUntil=Time.time+2;}
        public void WaveShield(int wave)
        {if(shieldWave==wave||!unit.IsAlive)return;shieldWave=wave;shield=unit.MaxHp*EffectBuffMath.ShieldFraction(EffectBuffSystem.Current?EffectBuffSystem.Current.Level(EffectBuffKind.Bodyguard):0);Project.Visual.SkillEffects.Ring(unit.transform.position,.4f,Color.cyan,1f);}
        public float Absorb(float damage){float used=Mathf.Min(shield,damage);shield-=used;return damage-used;}
        void LateUpdate()
        {
            bool visible=unit&&unit.IsAlive&&unit.Mythic&&unit.Mythic.Deployed&&Project.UI.SharedSettings.EffectsEnabled;
            if(visible&&shield>0&&!shieldArt)
            {
                var go=new GameObject("Bodyguard Shield");go.transform.SetParent(transform,false);shieldArt=go.AddComponent<LineRenderer>();
                if(!material)material=new Material(Shader.Find("Sprites/Default"));shieldArt.sharedMaterial=material;shieldArt.useWorldSpace=false;shieldArt.loop=true;shieldArt.positionCount=32;shieldArt.startWidth=shieldArt.endWidth=.035f;
                shieldArt.startColor=shieldArt.endColor=new Color(.2f,.8f,1,.7f);
                for(int i=0;i<32;i++){float a=i*Mathf.PI*2/32;shieldArt.SetPosition(i,new Vector3(Mathf.Cos(a)*.3f,Mathf.Sin(a)*.4f+.35f,0));}
            }
            if(shieldArt)shieldArt.enabled=visible&&shield>0;
        }
        public void BasicHit(Unit victim)
        {
            var effects=EffectBuffSystem.Current;if(!effects||!unit||unit.Team!=Data.UnitTeam.Ally)return;
            hits++;
            if(!EffectBuffMath.FourthHit(hits))return;
            if(effects.Has(EffectBuffKind.EmberTouch)&&victim&&victim.IsAlive)victim.Effects?.Burn(unit,unit.AttackPower*.2f*effects.Level(EffectBuffKind.EmberTouch));
            if(effects.Has(EffectBuffKind.FrostTouch)&&victim&&victim.IsAlive)victim.Effects?.Slow(.2f*effects.Level(EffectBuffKind.FrostTouch));
            if(effects.Has(EffectBuffKind.DoubleStrike)&&victim&&victim.IsAlive)
            {victim.TakeDamage(unit.AttackPower*EffectBuffMath.ExtraAttack(effects.Level(EffectBuffKind.DoubleStrike)),unit,false);effects.StrikeVisual(unit,victim);}
        }
    }
}
