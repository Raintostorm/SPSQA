using UnityEngine;
namespace Project.Battle
{
    // Bottom edge stays on the cell while the tile rises to its exact final bounds.
    public sealed class GroundTileRise:MonoBehaviour
    {
        Vector3 centre;Vector2 size;float elapsed;bool ready;
        const float Duration=.55f;
        public void Configure(Vector3 position,Vector2 dimensions)
        {centre=position;size=dimensions;ready=true;elapsed=0;Draw(0);}
        void OnEnable(){elapsed=0;if(ready)Draw(0);}
        void Update()
        {
            if(!ready||elapsed>=Duration)return;
            elapsed=Mathf.Min(Duration,elapsed+Time.unscaledDeltaTime);
            Draw(Project.UI.SharedSettings.EffectsEnabled?elapsed/Duration:1);
        }
        void Draw(float progress)
        {
            float ease=1-Mathf.Pow(1-Mathf.Clamp01(progress),3);
            float height=Mathf.Lerp(.06f,1,ease);
            transform.localScale=new Vector3(size.x,size.y*height,1);
            transform.position=centre+Vector3.down*(size.y*(1-height)*.5f);
        }
    }
}
