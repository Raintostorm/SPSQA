using Project.Battle;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>Animation-following silhouette outline and two distinct active-buff badges.</summary>
    public sealed class MonsterBuffVisual : MonoBehaviour
    {
        Unit owner;
        SpriteRenderer body;
        SpriteRenderer[] outline;
        Transform badges;
        MaterialPropertyBlock tint;
        float until;

        public void Refresh(float expires) { until = expires; }
        void Awake()
        {
            owner=GetComponent<Unit>();
            var visual=GetComponent<FrameUnitVisual>();
            if(visual)body=visual.BodyRenderer;
            if(!body)return;
            tint=new MaterialPropertyBlock();
            tint.SetFloat("_FlashAmount",1);
            tint.SetColor("_FlashColor",new Color(1,.08f,.035f));
            outline=new SpriteRenderer[8];
            for(int i=0;i<outline.Length;i++)
            {
                var image=new GameObject("RoarOutline").AddComponent<SpriteRenderer>();
                image.transform.SetParent(body.transform,false);
                image.sharedMaterial=body.sharedMaterial;image.SetPropertyBlock(tint);
                outline[i]=image;
            }
            badges=new GameObject("AttackAndSpeedBuffs").transform;
            // World-sized badges should not shrink/mirror with the unit sprite.
            MakeBadge("Attack",Resources.Load<Sprite>("SharedUI/battle"),-.22f);
            MakeBadge("AttackSpeed",Resources.Load<Sprite>("SharedUI/buff_speed"),.22f);
            SetVisible(false);
        }
        void MakeBadge(string name,Sprite sprite,float x)
        {
            var image=new GameObject(name).AddComponent<SpriteRenderer>();image.transform.SetParent(badges,false);
            image.sprite=sprite;image.transform.localPosition=new Vector3(x,0,0);
            if(sprite)image.transform.localScale=Vector3.one*(.32f/Mathf.Max(.01f,sprite.bounds.size.y));
            var arrow=new GameObject("Increase").AddComponent<TextMesh>();arrow.transform.SetParent(badges,false);
            arrow.text="+";arrow.fontSize=40;arrow.characterSize=.06f;arrow.anchor=TextAnchor.MiddleCenter;arrow.color=new Color(1,.3f,.1f);
            arrow.transform.localPosition=new Vector3(x+.14f,.13f,0);
        }
        void SetVisible(bool active)
        {
            if(outline!=null)foreach(var image in outline)if(image)image.enabled=active;
            if(badges)badges.gameObject.SetActive(active);
        }
        void LateUpdate()
        {
            bool active=owner && owner.IsAlive && body && body.enabled && Time.time<until && owner.Combat.AttackMultiplier>1f;
            SetVisible(active);if(!active)return;
            for(int i=0;i<outline.Length;i++)
            {
                var image=outline[i];image.sprite=body.sprite;image.flipX=body.flipX;image.flipY=body.flipY;
                image.sortingLayerID=body.sortingLayerID;image.sortingOrder=body.sortingOrder-1;
                float angle=i*Mathf.PI*.25f;
                var offset=new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0)*.022f;
                image.transform.position=body.transform.position+offset;
                image.color=new Color(1,1,1,body.color.a*(.8f+.2f*Mathf.Sin(Time.time*7)));
            }
            badges.position=new Vector3(body.bounds.center.x,body.bounds.max.y+.3f,body.transform.position.z);
            foreach(var renderer in badges.GetComponentsInChildren<Renderer>())
            {renderer.sortingLayerID=body.sortingLayerID;renderer.sortingOrder=body.sortingOrder+8;}
        }
        void OnDisable(){SetVisible(false);}
        void OnDestroy()
        {
            if(badges)Destroy(badges.gameObject);
            if(outline!=null)foreach(var image in outline)if(image)Destroy(image.gameObject);
        }
    }
}
