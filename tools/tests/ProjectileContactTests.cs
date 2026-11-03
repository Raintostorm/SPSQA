using System;
using Project.Battle;
using Project.Visual;
using UnityEngine;
class ProjectileContactTests
{
 static void Check(bool ok,string s){if(!ok)throw new Exception(s);}
 static void Near(float a,float b){Check(Math.Abs(a-b)<.001f,$"{a} != {b}");}
 static Unit Enemy(float x=10,float y=0)=>new Unit{Team=Project.Data.UnitTeam.Enemy,transform=new Transform{position=new Vector3(x,y,0)}};
 static Projectile Shot(Unit victim,Unit source=null){var p=new Projectile();p.Launch(victim,20,10,0,source??new Unit());return p;}
 static void Main()
 {
  ObjectForTest();
  var enemy=Enemy();var shot=Shot(enemy);shot.Advance(.2f);enemy.transform.position=new Vector3(10,4,0);shot.Advance(.8f);
  Check(enemy.Hits==1&&RangedImpact.Count==1,"moving target gets exactly one damage and effect");Near(shot.transform.position.y,4);
  shot.Advance(1);Check(enemy.Hits==1&&RangedImpact.Count==1,"arrival is idempotent");
  RangedImpact.Reset();enemy=Enemy(10,2);shot=Shot(enemy);shot.Advance(.2f);enemy.EndLife(true);enemy.transform.position=new Vector3(40,50,0);shot.Advance(1);
  Check(enemy.Hits==0&&RangedImpact.Count==0,"old projectile cannot hit revived life");Near(shot.transform.position.x,10);Near(shot.transform.position.y,2);
  RangedImpact.Reset();enemy=Enemy();shot=Shot(enemy);enemy.EndLife(false);shot.Advance(1.2f);Check(enemy.Hits==0&&RangedImpact.Count==0,"dead target gets no damage or impact");
  enemy=Enemy();enemy.Shield=true;shot=Shot(enemy);shot.Advance(1.1f);Check(enemy.Hits==1&&RangedImpact.Kind==RangedImpactKind.Shield,"shield contact uses shield impact even if shield depleted");Near(RangedImpact.Point.x,9.75f);
  RangedImpact.Reset();enemy=Enemy();enemy.Lethal=true;shot=Shot(enemy);shot.Advance(1.1f);Check(enemy.Hits==1&&RangedImpact.Count==1,"valid lethal contact still produces one effect");
  RangedImpact.Reset();var shooter=new Unit();enemy=Enemy();shot=Shot(enemy,shooter);shooter.IsAlive=false;shot.Advance(1.1f);Check(enemy.Hits==1,"shooter death does not cancel its shot");
  RangedImpact.Reset();enemy=Enemy();shot=Shot(enemy);enemy.Team=Project.Data.UnitTeam.Ally;shot.Advance(1.1f);Check(enemy.Hits==0&&RangedImpact.Count==0,"no damage after allegiance change");
  UnityEngine.Object.Flow=new BattleFlow{Current=BattleFlow.Phase.Prep};enemy=Enemy();shot=Shot(enemy);shot.Advance(2);Check(enemy.Hits==0&&shot.gameObject.Destroyed,"no projectile leaks into Prep");
  Console.WriteLine("PASS: actual Projectile follows moving original target; single contact; death endpoint; instant revival protection; shield impact; lethal hit; shooter death; allegiance and phase cleanup.");
 }
 static void ObjectForTest(){UnityEngine.Object.Flow=null;RangedImpact.Reset();}
}
namespace UnityEngine
{
 [AttributeUsage(AttributeTargets.Field)]public class SerializeField:Attribute{}
 public class Object{public static Project.Battle.BattleFlow Flow;public static implicit operator bool(Object x)=>x!=null&&!x.Destroyed;public bool Destroyed;public static T FindFirstObjectByType<T>()where T:Object=>Flow as T;public static void Destroy(Object x){if(x!=null)x.Destroyed=true;}}
 public class GameObject:Object{}
 public class MonoBehaviour:Object{public Transform transform=new Transform();public GameObject gameObject=new GameObject();public T GetComponent<T>()where T:class=>null;}
 public class Transform:Object{public Vector3 position;public Quaternion rotation;}
 public struct Quaternion{public static Quaternion Euler(float x,float y,float z)=>new Quaternion();}
 public struct Vector3{public float x,y,z;public Vector3(float x,float y,float z){this.x=x;this.y=y;this.z=z;}public float sqrMagnitude=>x*x+y*y+z*z;public static Vector3 operator+(Vector3 a,Vector3 b)=>new Vector3(a.x+b.x,a.y+b.y,a.z+b.z);public static Vector3 operator-(Vector3 a,Vector3 b)=>new Vector3(a.x-b.x,a.y-b.y,a.z-b.z);public static Vector3 operator*(Vector3 a,float b)=>new Vector3(a.x*b,a.y*b,a.z*b);public static Vector3 Lerp(Vector3 a,Vector3 b,float t)=>a+(b-a)*t;public static float Distance(Vector3 a,Vector3 b)=>(float)Math.Sqrt((a-b).sqrMagnitude);}
 public struct Bounds{public Vector3 size;}
 public class SpriteRenderer:Object{public int sortingLayerID,sortingOrder;public Bounds bounds;}
 public static class Time{public static float deltaTime=.01f;}
 public static class Mathf{public const float Rad2Deg=57.29578f;public static float Max(float a,float b)=>Math.Max(a,b);public static float Clamp01(float x)=>Math.Clamp(x,0,1);public static float Atan2(float a,float b)=>(float)Math.Atan2(a,b);}
}
namespace UnityEngine.Rendering{public class SortingGroup:UnityEngine.Object{public int sortingLayerID,sortingOrder;}}
namespace Project.Data{public enum UnitTeam{Ally,Enemy}public class UnitDefinition{}}
namespace Project.Battle
{
 public class BattleFlow:UnityEngine.Object{public enum Phase{Prep,Marching,Fighting}public Phase Current;}
 public class Mythic{public bool Deployed=true;}
 public class Unit:UnityEngine.MonoBehaviour{public bool IsAlive=true,Shield,Lethal;public int Hits;public Project.Data.UnitTeam Team;public Mythic Mythic=new Mythic();public Project.Data.UnitDefinition Definition=new Project.Data.UnitDefinition();public UnityEngine.Vector3 HitPoint=>transform.position;public event Action<Unit> LifeEnded;public void EndLife(bool revive){LifeEnded?.Invoke(this);IsAlive=revive;}public void TakeDamage(float d,Unit s){Hits++;Shield=false;if(Lethal)EndLife(false);}}
 public static class RangedShotStyle{public static RangedImpactKind Kind(Project.Data.UnitDefinition d)=>RangedImpactKind.Arrow;public static bool Shielded(Unit u)=>u.Shield;public static UnityEngine.Vector3 Contact(Unit u,UnityEngine.Vector3 from)=>u.HitPoint+(u.Shield?new UnityEngine.Vector3(-.25f,0,0):new UnityEngine.Vector3());}
}
namespace Project.Visual
{
 public class FrameUnitVisual{public UnityEngine.SpriteRenderer BodyRenderer;}
 public enum RangedImpactKind{None,Arrow,Shield}
 public static class RangedImpact{public static int Count;public static RangedImpactKind Kind;public static UnityEngine.Vector3 Point;public static void Reset(){Count=0;}public static void Prewarm(){}public static void Play(RangedImpactKind k,UnityEngine.Vector3 p,float w,int l,int o,Project.Battle.BattleFlow f){Count++;Kind=k;Point=p;}}
}
