using Project.Battle;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace Project.Visual
{
    // Runs after animation: constrain the complete rendered sprite, not its pivot.
    [DefaultExecutionOrder(10000)]
    public sealed class BattleGroundConstraint : MonoBehaviour
    {
        Unit unit;
        UnitBrain brain;
        SpriteRenderer body, background;
        BattleFlow flow;
        SortingGroup sorting;
        bool deployed;

        // Fit the WHOLE deployment triangle before individual LateUpdates.
        // Clamping each member at the screen edge collapses their offsets.
        public static void FitDeployment(List<Unit> units, float rowFraction)
        {
            var bg=FindFirstObjectByType<BattleBackground>();
            var cam=Camera.main;
            if(!bg || !cam)return;
            bool found=false;
            Bounds group=new Bounds();
            foreach(var member in units)
            {
                if(!member || !member.IsAlive)continue;
                var renderer=member.GetComponent<FrameUnitVisual>()?.BodyRenderer;
                if(!renderer || !renderer.sprite)continue;
                if(!found){group=renderer.bounds;found=true;}
                else group.Encapsulate(renderer.bounds);
            }
            if(!found)return;
            var field=bg.GetComponent<SpriteRenderer>().bounds;
            // Additional breathing room for pose changes and sword swings.
            float bottom=field.max.y-field.size.y*.625f+.18f;
            float top=field.max.y-field.size.y*.365f-.18f;
            float half=cam.orthographicSize*cam.aspect;
            float left=cam.transform.position.x-half+.14f;
            float right=cam.transform.position.x+half-.14f;
            float centreY=Mathf.Lerp(bottom+group.extents.y,
                top-group.extents.y,Mathf.Clamp01(rowFraction));
            float centreX=Mathf.Clamp(group.center.x,
                left+group.extents.x,right-group.extents.x);
            var shift=new Vector3(centreX-group.center.x,centreY-group.center.y,0);
            foreach(var member in units)
                if(member && member.IsAlive)member.transform.position+=shift;
        }

        void Start()
        {
            unit=GetComponent<Unit>();brain=GetComponent<UnitBrain>();
            body=GetComponent<FrameUnitVisual>()?.BodyRenderer;
            sorting=GetComponent<SortingGroup>();
            flow=FindFirstObjectByType<BattleFlow>();
            var bg=FindFirstObjectByType<BattleBackground>();
            if(bg)background=bg.GetComponent<SpriteRenderer>();
        }

        void LateUpdate()
        {
            if(!flow || !body || !background || !unit)return;
            if(flow.Current==BattleFlow.Phase.Prep){deployed=false;return;}
            if(brain && brain.enabled)deployed=true;
            if(!deployed)return; // Queued troops remain hidden until released.
            if(unit.IsAlive)unit.Visual?.SetVisible(true);
            if(!body.sprite)return;
            var field=background.bounds;
            // Road v3: top-down fractions; inset from bushes at both edges.
            float bottom=field.max.y-field.size.y*.625f+.08f;
            float top=field.max.y-field.size.y*.365f-.08f;
            var bounds=body.bounds;
            if(bounds.size.y>top-bottom)
            {
                // Defensive fallback for an oversized imported pose; never crop the head.
                transform.localScale*=((top-bottom)/bounds.size.y)*.98f;
                bounds=body.bounds;
            }
            float dy=bounds.max.y>top?top-bounds.max.y:0;
            if(bounds.min.y+dy<bottom)dy=bottom-bounds.min.y;
            float dx=0;
            var cam=Camera.main;
            if(cam)
            {
                float half=cam.orthographicSize*cam.aspect;
                float left=cam.transform.position.x-half+.04f,right=cam.transform.position.x+half-.04f;
                if(bounds.min.x<left)dx=left-bounds.min.x;
                if(bounds.max.x+dx>right)dx=right-bounds.max.x;
            }
            transform.position+=new Vector3(dx,dy,0);
            // SortingGroup controls the whole unit, including health bars and
            // effects. Child renderer order only controls layers inside it.
            if(sorting)
            {
                sorting.sortingLayerID=background.sortingLayerID;
                sorting.sortingOrder=Mathf.Max(background.sortingOrder+10,
                    Mathf.RoundToInt(-transform.position.y*100f));
            }
        }
    }
}
