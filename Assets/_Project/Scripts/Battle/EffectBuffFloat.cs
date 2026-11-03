using UnityEngine;
namespace Project.Battle
{
    public sealed class EffectBuffFloat:MonoBehaviour
    {
        float life;
        void Update(){if(Project.UI.SharedSettings.IsPaused)return;life+=Time.deltaTime;transform.position+=Vector3.up*Time.deltaTime*.4f;if(life>.8f)Destroy(gameObject);}
    }
}
