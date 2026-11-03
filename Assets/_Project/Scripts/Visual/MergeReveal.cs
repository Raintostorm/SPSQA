using Project.Battle;
using UnityEngine;

namespace Project.Visual
{
    public sealed class MergeReveal : MonoBehaviour
    {
        SpriteRenderer body;
        Vector3 size;
        float elapsed;
        public static void Play(Unit unit)
        {
            var frame=unit.GetComponent<FrameUnitVisual>();if(!frame||!frame.BodyRenderer)return;
            var fx=unit.gameObject.AddComponent<MergeReveal>();fx.body=frame.BodyRenderer;
            fx.size=fx.body.transform.localScale;
            SkillEffects.RecruitArrival(unit.transform.position,.8f,PrepUnitRing.ColorForRank(unit.Definition.Rank),.55f);
            fx.Draw(0);
        }
        void Update()
        {
            if(Project.UI.SharedSettings.IsPaused)return;
            elapsed+=Time.deltaTime;Draw(Mathf.Clamp01(elapsed/.22f));
            if(elapsed>=.22f)Destroy(this);
        }
        void Draw(float t)
        {
            if(!body)return;
            body.transform.localScale=size*Mathf.Lerp(.65f,1,Mathf.SmoothStep(0,1,t));
            var c=body.color;c.a=t;body.color=c;
        }
        void OnDestroy(){if(body){body.transform.localScale=size;var c=body.color;c.a=1;body.color=c;}}
    }
}
