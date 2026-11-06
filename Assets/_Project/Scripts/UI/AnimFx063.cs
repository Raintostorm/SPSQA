using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Lo 063: chep dung nhip trong chat/check/063-timing.md. Chi trinh dien, khong cham, tu huy.
    static class Fx
    {
        public static Sprite Load(string p) => Resources.Load<Sprite>("Anim/" + p);
        public static float Smooth(float a, float b, float t) => Mathf.SmoothStep(0, 1, Mathf.InverseLerp(a, b, t));
        public static RectTransform Layer(Transform parent, Sprite sprite, Vector2 size, Color color)
        {
            if (!sprite) return null;
            var i = new GameObject(sprite.name, typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            i.transform.SetParent(parent, false); i.sprite = sprite; i.color = color; i.raycastTarget = false;
            i.rectTransform.anchorMin = i.rectTransform.anchorMax = i.rectTransform.pivot = Vector2.one * .5f;
            i.rectTransform.sizeDelta = size; return i.rectTransform;
        }
        public static void Alpha(RectTransform r, float a) { if (!r) return; var i = r.GetComponent<Image>(); var c = i.color; c.a = Mathf.Clamp01(a); i.color = c; }
        // Lop phu co canvas rieng tren root canvas cua target, dat tai mot diem the gioi.
        public static RectTransform Overlay(Component target, string name, Vector3 world, int order = 30)
        {
            var owner = target.GetComponentInParent<Canvas>(); if (!owner) return null;
            var go = new GameObject(name, typeof(RectTransform), typeof(Canvas));
            var r = (RectTransform)go.transform; r.SetParent(owner.rootCanvas.transform, false);
            r.anchorMin = r.anchorMax = r.pivot = Vector2.one * .5f; r.sizeDelta = Vector2.zero; r.position = world;
            var c = go.GetComponent<Canvas>(); c.overrideSorting = true; c.sortingOrder = owner.rootCanvas.sortingOrder + order;
            return r;
        }
    }

    // §1 Len cap quan — 0.88s: elip duoi chan, cot sang hat len, 8 sao ban ra roi roi, chevron nhay tren dau.
    public sealed class UnitLevelUp : MonoBehaviour
    {
        static readonly Color Gold = new Color(1f, .84f, .36f);
        const float Duration = .88f;
        RectTransform ground, pillar, chevron; readonly List<RectTransform> stars = new List<RectTransform>();
        readonly List<float> angles = new List<float>();
        float s, head, elapsed;

        // actor: Image nhan vat, pivot dat o chan (AllyUpgradeView.Draw lay pivot that cua frame).
        public static void Play(Image actor)
        {
            if (!actor || !actor.enabled) return;
            var at = actor.rectTransform;
            var root = Fx.Overlay(actor, "UnitLevelUp", at.position); if (!root) return;
            var fx = root.gameObject.AddComponent<UnitLevelUp>();
            // Doi toa do preview (than quan cao ~220px) sang co that cua quan tren man.
            float height = at.rect.height * (1 - at.pivot.y) * at.lossyScale.y / root.lossyScale.y;
            fx.s = Mathf.Max(.4f, height / 220f); fx.head = height;
            fx.pillar = Fx.Layer(root, Fx.Load("LevelUp/lu-pillar"), new Vector2(210, 420) * fx.s, Gold);
            if (fx.pillar) { fx.pillar.pivot = new Vector2(.5f, 0); fx.pillar.anchoredPosition = new Vector2(0, -10 * fx.s); }
            fx.ground = Fx.Layer(root, Fx.Load("LevelUp/lu-ground"), new Vector2(260, 87) * fx.s, Gold);
            if (fx.ground) fx.ground.SetAsFirstSibling();
            var big = Fx.Load("LevelUp/lu-star"); var small = Fx.Load("LevelUp/lu-star-small") ?? big;
            for (int i = 0; i < 8; i++)
            {
                var star = Fx.Layer(root, i % 2 == 0 ? small : big, Vector2.one * (i % 2 == 0 ? 22 : 32) * fx.s, Color.white);
                if (!star) break; fx.stars.Add(star); fx.angles.Add(Mathf.Lerp(-160, 160, i / 7f) * Mathf.Deg2Rad);
            }
            fx.chevron = Fx.Layer(root, Fx.Load("LevelUp/lu-chevron"), Vector2.one * 70 * fx.s, Color.white);
            fx.Tick(0);
        }

        void Update() { elapsed += Time.unscaledDeltaTime; Tick(elapsed); if (elapsed >= Duration) Destroy(gameObject); }

        void Tick(float t)
        {
            float fade = 1 - Fx.Smooth(.68f, .88f, t);
            Fx.Alpha(ground, Fx.Smooth(0, .10f, t) * fade);
            Fx.Alpha(pillar, .58f * Fx.Smooth(.06f, .20f, t) * fade);
            if (pillar) pillar.localScale = new Vector3(1, Mathf.Lerp(.3f, 1, Fx.Smooth(.06f, .20f, t)), 1);
            for (int i = 0; i < stars.Count; i++)
            {
                // Quy dao parabol: bay len, toa ngang, roi xuong (y Unity huong len nen dao dau).
                float u = Mathf.Clamp01((t - .16f - i * .012f) / .6f);
                float a = angles[i], vx = Mathf.Sin(a) * 190, vy = 500 + Mathf.Abs(Mathf.Cos(a)) * 100;
                stars[i].anchoredPosition = new Vector2(vx * u, 66 + vy * u - 700 * u * u) * s;
                stars[i].localEulerAngles = new Vector3(0, 0, 90 * u * .6f);
                Fx.Alpha(stars[i], u <= 0 ? 0 : (1 - u) * fade);
            }
            if (chevron)
            {
                float jump = Fx.Smooth(.34f, .54f, t);
                chevron.anchoredPosition = new Vector2(0, head + 30 * s + 35 * s * jump);
                Fx.Alpha(chevron, Fx.Smooth(.34f, .40f, t) * fade);
            }
        }
    }

    // §2 Relic — 0.72s: vet sang 30° quet trai->phai, cat theo dang relic; roi "ting" o goc tren phai.
    public sealed class RelicShine : MonoBehaviour
    {
        const float Duration = .72f;
        RectTransform stripe, glint; float width, elapsed;

        public static void Play(Image icon)
        {
            if (!icon || !icon.sprite) return;
            var r = icon.rectTransform;
            // Mat na = chinh hinh relic (Mask theo alpha), nen vet sang khong tran ra the/nen.
            var mask = new GameObject("ShineMask", typeof(RectTransform), typeof(Image), typeof(Mask)).GetComponent<RectTransform>();
            mask.SetParent(r, false); mask.anchorMin = Vector2.zero; mask.anchorMax = Vector2.one; mask.offsetMin = mask.offsetMax = Vector2.zero;
            var m = mask.GetComponent<Image>(); m.sprite = icon.sprite; m.preserveAspect = icon.preserveAspect; m.raycastTarget = false;
            mask.GetComponent<Mask>().showMaskGraphic = false;
            var fx = mask.gameObject.AddComponent<RelicShine>(); fx.width = r.rect.width;
            fx.stripe = Fx.Layer(mask, Fx.Load("Relic/shine-sweep"), new Vector2(r.rect.width * .5f, r.rect.height * 1.4f), new Color(1, 1, 1, .8f));
            if (fx.stripe) fx.stripe.localEulerAngles = new Vector3(0, 0, -30);
            fx.glint = Fx.Layer(r, Fx.Load("Relic/glint"), Vector2.one * 35, Color.white);
            if (fx.glint) fx.glint.anchoredPosition = new Vector2(r.rect.width * .38f, r.rect.height * .38f);
            fx.Tick(0);
        }

        void Update() { elapsed += Time.unscaledDeltaTime; Tick(elapsed); if (elapsed >= Duration) { if (glint) Destroy(glint.gameObject); Destroy(gameObject); } }

        void Tick(float t)
        {
            if (stripe) { stripe.anchoredPosition = new Vector2(Mathf.Lerp(-.6f, .6f, Fx.Smooth(0, .35f, t)) * width, 0); Fx.Alpha(stripe, t < .35f ? .8f : 0); }
            if (glint)
            {
                float u = Mathf.Clamp01((t - .30f) / .35f), wave = Mathf.Sin(u * Mathf.PI);
                glint.sizeDelta = Vector2.one * (35 + 30 * wave) * (width / 260f); Fx.Alpha(glint, wave);
                glint.localEulerAngles = new Vector3(0, 0, 45 * u);
            }
        }
    }

    // §3 Thang — 20 manh confetti roi suot ~1.8s. KHONG co sao danh gia: game chua co luat cham sao,
    // khong tu bia 3 sao cho moi van (063-timing.md da dan).
    public sealed class Confetti : MonoBehaviour
    {
        const float Duration = 1.8f;
        readonly List<RectTransform> bits = new List<RectTransform>(); readonly List<Vector3> motion = new List<Vector3>();
        float elapsed, top;

        public static void Play(RectTransform area, int count = 20)
        {
            if (!area) return;
            var root = Fx.Overlay(area, "Confetti", area.TransformPoint(area.rect.center), 25); if (!root) return;
            var fx = root.gameObject.AddComponent<Confetti>();
            float w = area.rect.width * area.lossyScale.x / root.lossyScale.x, h = area.rect.height * area.lossyScale.y / root.lossyScale.y;
            fx.top = h * .5f;
            var art = new Sprite[4]; for (int i = 0; i < 4; i++) art[i] = Fx.Load("Result/confetti-" + i);
            for (int i = 0; i < count; i++)
            {
                var bit = Fx.Layer(root, art[i % 4], Vector2.one * Random.Range(36f, 52f), new Color(1, 1, 1, .85f));
                if (!bit) break;
                bit.anchoredPosition = new Vector2(Random.Range(-.48f, .48f) * w, fx.top + Random.Range(0f, 400f));
                fx.bits.Add(bit); fx.motion.Add(new Vector3(Random.Range(300f, 412f), Random.Range(0, 6.28f), Random.Range(-240f, 240f)));
            }
        }

        void Update()
        {
            float dt = Time.unscaledDeltaTime; elapsed += dt;
            for (int i = 0; i < bits.Count; i++)
            {
                var m = motion[i]; var p = bits[i].anchoredPosition;
                p.y -= m.x * dt; p.x += Mathf.Sin(elapsed * 3 + m.y) * 28 * dt;
                bits[i].anchoredPosition = p; bits[i].localEulerAngles += new Vector3(0, 0, m.z * dt);
                Fx.Alpha(bits[i], .85f * (1 - Mathf.Max(0, elapsed - Duration + .4f) / .4f));
            }
            if (elapsed >= Duration) Destroy(gameObject);
        }
    }
}
