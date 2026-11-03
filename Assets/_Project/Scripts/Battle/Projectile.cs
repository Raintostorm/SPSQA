using UnityEngine;
using Project.Visual;
namespace Project.Battle
{
    // Original target only. If its life ends, finish at that death position without damage/VFX.
    public sealed class Projectile:MonoBehaviour
    {
        [SerializeField] bool faceDirection=true;
        [SerializeField] float maxLifetime=5f;
        Unit target,source;Project.Data.UnitTeam targetTeam;
        Vector3 start,end;float damage,flightTime,arcHeight,elapsed;bool launched,lostTarget;
        BattleFlow flow;RangedImpactKind impactKind;
        public void Launch(Unit targetUnit,float damageAmount,float speed,float arcHeightUnits,Unit shooter=null)
        {
            Unsubscribe();target=targetUnit;source=shooter;lostTarget=!target||!target.IsAlive;
            if(target){targetTeam=target.Team;target.LifeEnded+=TargetDied;}
            flow=FindFirstObjectByType<BattleFlow>();damage=damageAmount;arcHeight=Mathf.Max(0,arcHeightUnits);
            start=transform.position;end=target?RangedShotStyle.Contact(target,start):start;
            flightTime=Mathf.Max(.0001f,Vector3.Distance(start,end)/Mathf.Max(.01f,speed));
            elapsed=0;launched=true;impactKind=RangedShotStyle.Kind(shooter?shooter.Definition:null);
            RangedImpact.Prewarm();
        }
        void TargetDied(Unit victim){if(lostTarget)return;end=victim.HitPoint;lostTarget=true;}
        bool ValidTarget=>!lostTarget&&target&&target.IsAlive&&target.Team==targetTeam&&target.Mythic.Deployed&&(!source||source.Team!=target.Team);
        void Update()=>Advance(Time.deltaTime);
        // Separate deterministic step makes trajectory/lifecycle regression tests possible.
        internal void Advance(float delta)
        {
            if(!launched)return;
            if(flow&&flow.Current!=BattleFlow.Phase.Marching&&flow.Current!=BattleFlow.Phase.Fighting){Remove();return;}
            if(ValidTarget)end=RangedShotStyle.Contact(target,transform.position);
            else lostTarget=true;
            elapsed+=delta;if(elapsed>=maxLifetime){Remove();return;}
            float t=Mathf.Clamp01(elapsed/flightTime);var previous=transform.position;
            transform.position=PointAt(t);
            if(faceDirection)FaceAlong(transform.position-previous);
            if(t>=1)Arrive();
        }
        Vector3 PointAt(float t)
        {
            var p=Vector3.Lerp(start,end,t);if(arcHeight>0)p.y+=arcHeight*4*t*(1-t);return p;
        }
        void FaceAlong(Vector3 delta)
        {if(delta.sqrMagnitude>.00000001f)transform.rotation=Quaternion.Euler(0,0,Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg);}
        void Arrive()
        {
            if(!launched)return;launched=false;
            if(ValidTarget)
            {
                // Snapshot contact BEFORE damage: lethal hits and shield depletion can move/change the victim.
                bool shield=RangedShotStyle.Shielded(target);var point=end;
                var body=target.GetComponent<FrameUnitVisual>()?.BodyRenderer;
                var group=target.GetComponent<UnityEngine.Rendering.SortingGroup>();
                int layer=group?group.sortingLayerID:body?body.sortingLayerID:0;
                int order=(group?group.sortingOrder:body?body.sortingOrder:150)+5;
                float width=body?body.bounds.size.x*.85f:.55f;
                target.TakeDamage(damage,source);
                RangedImpact.Play(shield?RangedImpactKind.Shield:impactKind,point,width,layer,order,flow);
            }
            Remove();
        }
        void Unsubscribe(){if(target)target.LifeEnded-=TargetDied;}
        void Remove(){launched=false;Unsubscribe();Destroy(gameObject);}
        void OnDestroy()=>Unsubscribe();
    }
}
