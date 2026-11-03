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
        public static void Recruit(Vector3 start, Vector3 finish, float seconds)
        {
            var fx = Create(new Color(1f,.9f,.35f), seconds); fx.origin = start; fx.end = finish; fx.mode = 1; fx.line.positionCount = 2; fx.presentation = true; fx.line.startWidth = .12f;
        }
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
            elapsed += presentation ? Time.unscaledDeltaTime : Time.deltaTime;
            if (elapsed >= duration || mode == 2 && (!follow || !follow.IsAlive || follow.Combat.Shield <= 0f)) { Destroy(gameObject); return; }
            var t = Mathf.Clamp01(elapsed / duration);
            if (mode == 2)
            {
                var sorting = follow.GetComponent<UnityEngine.Rendering.SortingGroup>();
                if (sorting) { line.sortingLayerID = sorting.sortingLayerID; line.sortingOrder = sorting.sortingOrder + 1; }
            }
            var color = tint; color.a = mode == 2 ? .8f : 1f - t * .7f;
            line.startColor = line.endColor = color;
            if (mode == 1)
            {
                var head = Vector3.Lerp(origin, end, t);
                var tail = Vector3.Lerp(origin, end, Mathf.Max(0f, t - .22f));
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
    }
}
