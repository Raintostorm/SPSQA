// Isolated regression harness: compiles the actual UnitBrain.cs against minimal engine doubles.
using System;
using System.Reflection;
using Project.Battle;
class UnitBrainTargetLockTests
{
 static void Set(object o,string n,object v)=>o.GetType().GetField(n,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(o,v);
 static object Get(object o,string n)=>o.GetType().GetField(n,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(o);
 static void Call(object o,string n)=>o.GetType().GetMethod(n,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(o,null);
 static void Check(bool b,string s){if(!b)throw new Exception(s);}
 static void Main()
 {
  var brain=new UnitBrain();var self=new Unit();var a=new Unit{Team=Project.Data.UnitTeam.Enemy};var b=new Unit{Team=Project.Data.UnitTeam.Enemy};
  self.Definition=new Project.Data.UnitDefinition();var visual=new FakeVisual();
  Set(brain,"self",self);Set(brain,"def",self.Definition);Set(brain,"visual",visual);Set(brain,"target",a);
  UnitRegistry.Nearest=b;UnitRegistry.Queries=0;Call(brain,"EnsureTarget");
  Check(ReferenceEquals(Get(brain,"target"),a)&&UnitRegistry.Queries==0,"Do not switch a living target while approaching");
  Call(brain,"Fight");Check(visual.Attacks==1,"Begin attack");a.IsAlive=false;
  Call(brain,"Update");Check(UnitRegistry.Queries==0&&visual.Faces==0&&visual.Moves==1,"Death during swing must not retarget, turn or walk");
  // Even if a future caller changes target, contact must remain bound to the original victim.
  Set(brain,"target",b);visual.Impact();Check(a.Hits==0&&b.Hits==0,"Dead victim cannot redirect damage to another enemy");
  Set(brain,"target",a);visual.Finished();Call(brain,"Update");
  Check(ReferenceEquals(Get(brain,"target"),b)&&UnitRegistry.Queries==1,"Retarget after animation finishes");
  Set(brain,"cooldownTimer",0f);Call(brain,"Fight");visual.Impact();Check(b.Hits==1,"Living locked victim receives one contact");
  // Enemy dies to an aura while we are still approaching: preserve its last point.
  visual.Finished();Set(brain,"target",a);a.IsAlive=true;a.transform.position=new UnityEngine.Vector3(2,0,0);
  Call(brain,"EnsureTarget");a.IsAlive=false;int scans=UnitRegistry.Queries;int swings=visual.Attacks;
  Call(brain,"Update");Check(UnitRegistry.Queries==scans&&brain.transform.position.x>0,"Continue to dead focus instead of acquiring another enemy");
  brain.transform.position=new UnityEngine.Vector3(2,0,0);Set(brain,"cooldownTimer",0f);Call(brain,"Update");
  Check(visual.Attacks==swings+1,"Perform final swing at dead focus position");int hits=b.Hits;visual.Impact();Check(b.Hits==hits,"Dead focus final swing causes no collateral damage");
  visual.Finished();Call(brain,"Update");Check(ReferenceEquals(Get(brain,"target"),b),"Acquire next only after final swing");
  Console.WriteLine("PASS: stable chase target, finish swing after victim death, no redirected damage, then retarget; also approach a victim killed by a skill and finish a harmless swing.");
 }
 class FakeVisual:Project.Visual.IUnitVisual
 {
  public int Attacks,Faces,Moves;public Action Impact,Finished;
  public void PlayAttack(Action a,Action b){Attacks++;Impact=a;Finished=b;}
  public void SetMoving(bool b){Moves++;}public void SetFacing(float f){Faces++;}public void SetAttackSpeedMultiplier(float f){}
 }
}
namespace UnityEngine
{
 public class Object{public static implicit operator bool(Object o)=>o!=null;}
 public class MonoBehaviour:Object{public Transform transform=new Transform();public string name="test";public bool enabled=true;public T GetComponent<T>()=>default(T);protected GameObject Instantiate(GameObject o,Vector3 p,Quaternion q)=>new GameObject();protected void Destroy(GameObject o){}}
 public class Transform{public Vector3 position,localScale;}
 public class GameObject:Object{public string name;public Transform transform=new Transform();public GameObject(string n=""){name=n;}public T AddComponent<T>() where T:new()=>new T();public T GetComponent<T>()=>default(T);public T[] GetComponentsInChildren<T>()=>Array.Empty<T>();}
 public class SpriteRenderer:Object{public int sortingOrder;public bool enabled;public Object sprite;public Color color;public Transform transform=new Transform();}
 public struct Color{}
 public struct Quaternion{public static Quaternion identity=>new Quaternion();}
 public struct Vector3{public float x,y,z;public Vector3(float x,float y,float z=0){this.x=x;this.y=y;this.z=z;}public static Vector3 one=>new Vector3(1,1,1);public static Vector3 operator *(Vector3 a,float f)=>new Vector3(a.x*f,a.y*f,a.z*f);public static float Distance(Vector3 a,Vector3 b)=>(float)Math.Sqrt((a.x-b.x)*(a.x-b.x)+(a.y-b.y)*(a.y-b.y));}
 public static class Mathf{public static float Max(float a,float b)=>Math.Max(a,b);public static float Min(float a,float b)=>Math.Min(a,b);public static float Abs(float a)=>Math.Abs(a);public static float MoveTowards(float a,float b,float d)=>Math.Abs(b-a)<=d?b:a+Math.Sign(b-a)*d;}
 public static class Time{public static float deltaTime=.016f;}
 public static class Random{public static float value=>1;}
 public static class Debug{public static void LogWarning(string s,Object o=null){}public static void LogError(string s,Object o=null){}}
 public class SerializeField:Attribute{}public class TooltipAttribute:Attribute{public TooltipAttribute(string s){}}public class HeaderAttribute:Attribute{public HeaderAttribute(string s){}}public class RequireComponent:Attribute{public RequireComponent(Type t){}}
}
namespace Project.Data
{
 public enum UnitTeam{Ally,Enemy}public enum MythicKind{Clock}
 public class UnitDefinition:UnityEngine.Object{public string name="unit";public bool IsRanged;public float AttackRange=.55f,AttackCooldown=.7f,MoveSpeed=1,ProjectileSpeed,ProjectileArcHeight;public UnityEngine.GameObject ProjectilePrefab;public UnityEngine.Object ProjectileSpriteOverride;public UnityEngine.Color ProjectileTint;public Animations Animations;}
 public class Animations{public Clip Attack;}public class Clip{public float Total=.52f;}
}
namespace Project.Visual
{
 public interface IUnitVisual{void PlayAttack(Action a,Action b);void SetMoving(bool b);void SetFacing(float f);void SetAttackSpeedMultiplier(float f);}
 public class MythicProjectileArt{public void Bind(string s){}}
}
namespace Project.Battle
{
 public class Unit:UnityEngine.MonoBehaviour{public bool IsAlive=true;public Project.Data.UnitTeam Team;public Project.Data.UnitDefinition Definition;public Combat Combat=new Combat();public Mythic Mythic=new Mythic();public Equipment Equipment=new Equipment();public EffectState Effects=new EffectState();public float AttackPower=10;public UnityEngine.Vector3 HitPoint=>transform.position;public int Hits;public event Action<Unit> Died;public void TakeDamage(float d,Unit u){Hits++;}}
 public class Combat:UnityEngine.Object{public bool Stunned,Casting;public float SpeedMultiplier=1;public bool TryCast(Unit u)=>false;}
 public class Mythic:UnityEngine.Object{public bool Casting,Charged,Active;public bool Deployed=true;public float SpeedMultiplier=1;public Project.Data.MythicKind Kind;public void AfterBasicAttack(Unit u,bool b){}}
 public class EffectState{public float SpeedMultiplier=1;}
 public class Equipment:UnityEngine.Object{public float SpeedBonus;}
 public static class RunBuffs{public static float Speed(Project.Data.UnitTeam t)=>0;public static float Crit(Project.Data.UnitTeam t)=>0;public static float CritMultiplier=>2;}
 public static class UnitRegistry{public static Unit Nearest;public static int Queries;public static Unit FindNearestEnemy(UnityEngine.Vector3 p,Project.Data.UnitTeam t){Queries++;return Nearest;}}
 public class Projectile:UnityEngine.Object{public void Launch(Unit u,float d,float s,float a,Unit owner){}}
}

namespace Project.Battle { public static class RangedShotStyle { public static UnityEngine.Vector3 Origin(Unit unit,UnityEngine.Transform anchor)=>anchor!=null?anchor.position:unit.HitPoint; } }

namespace Project.Battle { public static class MeleeAttackEffect { public static void Play(Unit unit,bool empowered){} } }
