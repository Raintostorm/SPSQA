using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // ANIM-004 (ban 2, theo gop y user 06/11): nhe va gon. Bo mui ten to va vong xoay —
    // chi con quang vang tho mot nhip, mot vong sang mo no ra va vai hat lap lanh bay len.
    // Khong chan cham, bam nang cap lien tuc van duoc.
    public sealed class LevelUpBurst : MonoBehaviour
    {
        static readonly Color Gold = new Color(1f, .86f, .4f);
        RectTransform glow, ring; readonly List<RectTransform> sparks = new List<RectTransform>();
        readonly List<Vector2> drift = new List<Vector2>();
        Sprite[] frames = new Sprite[4];
        float elapsed;
        const float Duration = .75f;

        public static void Play(RectTransform target, float size = 360)
        {
            if (!target) return;
            var owner = target.GetComponentInParent<Canvas>(); if (!owner) return;
            var go = new GameObject("LevelUpBurst", typeof(RectTransform), typeof(Canvas));
            var root = (RectTransform)go.transform; root.SetParent(owner.rootCanvas.transform, false);
            root.anchorMin = root.anchorMax = root.pivot = Vector2.one * .5f; root.sizeDelta = Vector2.one * size;
            root.position = target.TransformPoint(target.rect.center);
            var canvas = go.GetComponent<Canvas>(); canvas.overrideSorting = true; canvas.sortingOrder = owner.rootCanvas.sortingOrder + 30;
            var fx = go.AddComponent<LevelUpBurst>();
            for (int i = 0; i < 4; i++) fx.frames[i] = Resources.Load<Sprite>("Anim/LevelUp/levelup-flash-" + i);
            fx.glow = Layer(root, Resources.Load<Sprite>("Anim/Common/glow"), size * 1.3f, Gold);
            fx.ring = Layer(root, fx.frames[0], size, Gold);
            var spark = Resources.Load<Sprite>("Anim/Common/sparkle");
            for (int i = 0; spark && i < 7; i++)
            {
                fx.sparks.Add(Layer(root, spark, Random.Range(26f, 44f), Color.white));
                fx.drift.Add(new Vector2(Random.Range(-.45f, .45f) * size, Random.Range(.35f, .6f) * size));
            }
            fx.Tick(0);
        }

        void Update()
        {
            elapsed += Time.unscaledDeltaTime;
            Tick(Mathf.Clamp01(elapsed / Duration));
            if (elapsed >= Duration) Destroy(gameObject);
        }

        void Tick(float t)
        {
            float pulse = Mathf.Sin(t * Mathf.PI);
            if (glow) { glow.localScale = Vector3.one * Mathf.Lerp(.7f, 1.05f, pulse); Alpha(glow, .55f * pulse); }
            if (ring)
            {
                var frame = frames[Mathf.Min(3, (int)(t * 4))]; if (frame) ring.GetComponent<Image>().sprite = frame;
                ring.localScale = Vector3.one * Mathf.Lerp(.6f, 1.15f, UiTween.OutCubic(t));
                Alpha(ring, .7f * (1 - t));
            }
            for (int i = 0; i < sparks.Count; i++)
            {
                float u = Mathf.Clamp01(t * 1.2f - i * .04f);
                sparks[i].anchoredPosition = new Vector2(drift[i].x * .5f, -drift[i].y * .3f) + drift[i] * UiTween.OutCubic(u) * .7f;
                sparks[i].localEulerAngles = new Vector3(0, 0, 180 * u);
                Alpha(sparks[i], Mathf.Sin(u * Mathf.PI));
            }
        }

        static RectTransform Layer(Transform parent, Sprite sprite, float size, Color color)
        {
            if (!sprite) return null;
            var image = new GameObject(sprite.name, typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(parent, false); image.sprite = sprite; image.color = color;
            image.preserveAspect = true; image.raycastTarget = false; image.rectTransform.sizeDelta = Vector2.one * size;
            return image.rectTransform;
        }
        static void Alpha(RectTransform r, float a) { var i = r.GetComponent<Image>(); var c = i.color; c.a = Mathf.Clamp01(a); i.color = c; }
    }

    // Nhay chu: phong 1 -> 1.25 -> 1 va xanh roi tro ve mau cu (Lv. sau khi nang cap).
    public sealed class UiPulse : MonoBehaviour
    {
        static readonly Color Green = new Color(.38f, .93f, .5f);
        Text label; Color normal; float elapsed;
        public static void Play(Text text)
        {
            if (!text) return;
            var old = text.GetComponent<UiPulse>(); if (old) { old.elapsed = 0; return; }
            var p = text.gameObject.AddComponent<UiPulse>(); p.label = text; p.normal = text.color;
        }
        void Update()
        {
            elapsed += Time.unscaledDeltaTime; float t = Mathf.Clamp01(elapsed / .8f);
            transform.localScale = Vector3.one * (1 + .25f * Mathf.Sin(Mathf.Min(1, t * 2.5f) * Mathf.PI));
            label.color = Color.Lerp(Green, normal, Mathf.Max(0, t - .5f) / .5f);
            if (t >= 1) { transform.localScale = Vector3.one; label.color = normal; Destroy(this); }
        }
    }

    // Chi so relic chay tu gia tri cu len gia tri moi, xanh roi ve mau cu.
    public sealed class StatCountUp : MonoBehaviour
    {
        static readonly Color Green = new Color(.38f, .93f, .5f);
        Text label; Color normal; float from, to, elapsed, delay; string prefix, suffix, format;
        public static void Play(Text text, float from, float to, string prefix, string suffix, string format = "0.#", float delay = 0)
        {
            if (!text || Mathf.Approximately(from, to)) return;
            var c = text.gameObject.AddComponent<StatCountUp>();
            c.label = text; c.normal = text.color; c.from = from; c.to = to; c.prefix = prefix; c.suffix = suffix; c.format = format; c.delay = delay; c.Draw(from);
        }
        void Update()
        {
            if (delay > 0) { delay -= Time.unscaledDeltaTime; return; }
            elapsed += Time.unscaledDeltaTime; float t = Mathf.Clamp01((elapsed - .25f) / .6f);
            Draw(Mathf.Lerp(from, to, Mathf.SmoothStep(0, 1, t)));
            label.color = elapsed < 1f ? Green : Color.Lerp(Green, normal, (elapsed - 1f) / .3f);
            if (elapsed >= 1.3f) { Draw(to); label.color = normal; Destroy(this); }
        }
        void Draw(float v) => label.text = prefix + v.ToString(format) + suffix;
    }
}
