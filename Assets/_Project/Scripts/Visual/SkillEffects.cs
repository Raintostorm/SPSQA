using Project.Battle;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>Small world-space effects; use game time so pause freezes all VFX.</summary>
    public sealed class SkillEffects : MonoBehaviour
    {
        static Material material;
        LineRenderer line;
        Vector3 origin, end;
        Color tint;
        float duration, elapsed, radius;
        int mode;
        Unit follow;
        bool presentation;
        Renderer[] cachedRenderers;
        void Start(){cachedRenderers=GetComponentsInChildren<Renderer>();}
        void LateUpdate()
        {
            if(presentation)return;
            if(cachedRenderers!=null)foreach(var renderer in cachedRenderers)if(renderer)renderer.enabled=Project.UI.SharedSettings.EffectsEnabled;
        }
        SpriteRenderer rewardSprite;
        LineRenderer innerWave;
        Transform[] debris;

        public static void HumanShockwave(Vector3 feet, float area)
        {
            var fx = Create(new Color(1f, .76f, .24f), .8f);
            fx.origin = feet;
            fx.radius = area;
            fx.mode = 3;
            fx.line.positionCount = 65;
            fx.line.loop = true;
            fx.innerWave = new GameObject("Inner shockwave").AddComponent<LineRenderer>();
            fx.innerWave.transform.SetParent(fx.transform, false);
            fx.innerWave.sharedMaterial = material;
            fx.innerWave.useWorldSpace = true;
            fx.innerWave.loop = true;
            fx.innerWave.positionCount = 65;
            fx.innerWave.sortingOrder = fx.line.sortingOrder + 1;
            fx.debris = new Transform[16];
            for (var i = 0; i < fx.debris.Length; i++)
            {
                var rock = new GameObject("Earth fragment").AddComponent<LineRenderer>();
                rock.transform.SetParent(fx.transform, false);
                rock.sharedMaterial = material;
                rock.useWorldSpace = false;
                rock.loop = true;
                rock.positionCount = 4;
                rock.startWidth = rock.endWidth = .035f;
                rock.sortingOrder = fx.line.sortingOrder + 2;
                rock.SetPositions(new[] {new Vector3(-.055f,0), new Vector3(0,.075f),
                    new Vector3(.065f,.015f), new Vector3(.015f,-.04f)});
                fx.debris[i] = rock.transform;
            }
        }
        static SkillEffects Create(Color color, float seconds)
        {
            var fx = new GameObject("SkillEffect").AddComponent<SkillEffects>();
            fx.line = fx.gameObject.AddComponent<LineRenderer>();
            if (!material) material = new Material(Shader.Find("Sprites/Default"));
            fx.line.sharedMaterial = material;
            fx.line.useWorldSpace = true;
            fx.line.sortingOrder = 150;
            fx.line.startWidth = fx.line.endWidth = .055f;
            fx.tint = color;
            fx.duration = Mathf.Max(.05f, seconds);
            return fx;
        }
        public static void Ring(Vector3 point, float size, Color color, float seconds)
        {
            var fx = Create(color, seconds); fx.origin = point; fx.radius = size; fx.mode = 0; fx.line.positionCount = 49; fx.line.loop = true;
        }
        public static void Beam(Vector3 start, Vector3 finish, Color color, float seconds)
        {
            var fx = Create(color, seconds); fx.origin = start; fx.end = finish; fx.mode = 1; fx.line.positionCount = 2;
        }
        public static void Recruit(Vector3 start, Vector3 finish, float seconds, Color color)
        {
            var fx = Create(color, seconds); fx.origin = start; fx.end = finish; fx.mode = 1; fx.line.positionCount = 2; fx.presentation = true; fx.line.startWidth = .12f;
            fx.SetRewardSprite(0,.42f);
        }
        public static void RecruitArrival(Vector3 point,float size,Color color,float seconds)
        {
            var fx=Create(color,seconds);fx.origin=point;fx.radius=size;fx.mode=4;
            fx.line.enabled=false;fx.SetRewardSprite(2,size*2);
        }
        void SetRewardSprite(int index,float size)
        {
            var sprite=Resources.Load<Sprite>("RollRewards/recruit/frame_"+index);
            if(!sprite)return;
            rewardSprite=new GameObject("RecruitArt").AddComponent<SpriteRenderer>();
            rewardSprite.transform.SetParent(transform,false);rewardSprite.sprite=sprite;
            rewardSprite.sortingOrder=line.sortingOrder+1;rewardSprite.color=tint;
            rewardSprite.transform.localScale=Vector3.one*size;
        }
        Vector3 RecruitPoint(float t) => (1-t)*(1-t)*origin + 2*(1-t)*t*((origin+end)*.5f+Vector3.up*.7f)+t*t*end;
        public static void Shield(Unit unit, float seconds)
        {
            // Replace the previous bubble on refresh; do not leave stacked visuals.
            foreach (var old in FindObjectsByType<SkillEffects>(FindObjectsSortMode.None))
                if (old.mode == 2 && old.follow == unit) Destroy(old.gameObject);
            var fx = Create(new Color(.2f,.7f,1f), seconds); fx.follow = unit; fx.mode = 2; fx.line.positionCount = 49; fx.line.loop = true;
        }
        void Update()
        {
            if (presentation && Project.UI.SharedSettings.IsPaused) return;
            elapsed += Time.deltaTime;
            if (elapsed >= duration || mode == 2 && (!follow || !follow.IsAlive || follow.Combat.Shield <= 0f)) { Destroy(gameObject); return; }
            var t = Mathf.Clamp01(elapsed / duration);
            if(mode==4)
            {
                if(rewardSprite){rewardSprite.transform.position=origin;var c=tint;c.a=1-t;rewardSprite.color=c;}
                return;
            }
            if (mode == 3)
            {
                DrawShockwave(t);
                return;
            }
            if (mode == 2)
            {
                var sorting = follow.GetComponent<UnityEngine.Rendering.SortingGroup>();
                if (sorting) { line.sortingLayerID = sorting.sortingLayerID; line.sortingOrder = sorting.sortingOrder + 1; }
            }
            var color = tint; color.a = mode == 2 ? .8f : 1f - t * .7f;
            line.startColor = line.endColor = color;
            if (mode == 1)
            {
                var head = presentation ? RecruitPoint(t) : Vector3.Lerp(origin, end, t);
                var tail = presentation ? RecruitPoint(Mathf.Max(0,t-.14f)) : Vector3.Lerp(origin, end, Mathf.Max(0f, t - .22f));
                if(rewardSprite)
                {
                    rewardSprite.transform.position=head;rewardSprite.color=color;
                    var direction=head-tail;
                    // Source comet points diagonally down-right.
                    rewardSprite.transform.rotation=Quaternion.Euler(0,0,Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg+45f);
                }
                line.SetPosition(0, tail); line.SetPosition(1, head);
            }
            else
            {
                var centre = mode == 2 ? follow.HitPoint : origin;
                var r = mode == 2 ? .65f * Mathf.Abs(follow.transform.lossyScale.x) : radius * (.15f + .85f * t);
                for (var i = 0; i < 49; i++)
                {
                    var a = i * Mathf.PI * 2f / 48f;
                    line.SetPosition(i, centre + new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r * (mode == 2 ? 1f : .35f), 0));
                }
            }
        }

        void DrawShockwave(float t)
        {
            // All layers share the caster's feet. No detached ellipse centre.
            var expansion = 1f - Mathf.Pow(1f - t, 3f);
            var outer = radius * expansion;
            var glow = new Color(1f, .78f, .25f, Mathf.Pow(1f - t, 1.4f));
            line.startColor = line.endColor = glow;
            line.startWidth = line.endWidth = Mathf.Lerp(.13f, .025f, t);
            var white = new Color(1f, .96f, .72f, Mathf.Pow(1f - t, 2f));
            innerWave.startColor = innerWave.endColor = white;
            innerWave.startWidth = innerWave.endWidth = .055f * (1f - t);
            for (var i = 0; i < 65; i++)
            {
                var angle = i * Mathf.PI * 2f / 64f;
                var edge = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) * .35f, 0);
                line.SetPosition(i, origin + edge * outer);
                innerWave.SetPosition(i, origin + edge * outer * .82f);
            }
            for (var i = 0; i < debris.Length; i++)
            {
                var angle = i * Mathf.PI * 2f / debris.Length;
                var distance = outer * (.35f + .035f * (i % 5));
                var jump = Mathf.Sin(t * Mathf.PI) * (.18f + .06f * (i % 3));
                debris[i].position = origin + new Vector3(Mathf.Cos(angle) * distance,
                    Mathf.Sin(angle) * distance * .35f + jump, 0);
                debris[i].localRotation = Quaternion.Euler(0, 0, i * 47f + t * 160f);
                var rock = debris[i].GetComponent<LineRenderer>();
                var color = new Color(.68f, .44f, .2f, 1f - t);
                rock.startColor = rock.endColor = color;
                debris[i].localScale = Vector3.one * (1f - .5f * t);
            }
        }
    }
}
