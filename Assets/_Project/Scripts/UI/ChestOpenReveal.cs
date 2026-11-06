using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // ANIM-003: mo ruong relic. Chi trinh dien — TryOpenChest da tru chia, cap va luu truoc.
    // Ruong bat ra -> rung manh dan -> nap bat len, luong sang hat ra -> shard bay tung toe -> tan.
    // Mo nhieu chia (Open All) van chi mot chuoi, chi them shard. Cham bat ky = bo qua.
    public sealed class ChestOpenReveal : MonoBehaviour
    {
        // Mep duoi nap nam o hang 236/512 cua anh (do tren Anim/Chest/chest-lid.png):
        // dat tam xoay o day thi nap "gap" quanh ban le, khong co ve giua anh.
        const float LidHinge = 1 - 236f / 512f;
        static readonly Color Warm = new Color(1f, .86f, .45f);

        bool skip;

        public static IEnumerator Play(Transform parent, int keys)
        {
            var go = new GameObject("ChestOpenReveal", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var full = (RectTransform)go.transform; Stretch(full);
            var dim = go.GetComponent<Image>(); dim.color = new Color(.03f, .04f, .09f, 0); dim.raycastTarget = true;
            var canvas = go.AddComponent<Canvas>(); canvas.overrideSorting = true; canvas.sortingOrder = 460;
            go.AddComponent<GraphicRaycaster>();
            var view = go.AddComponent<ChestOpenReveal>();
            go.AddComponent<Button>().onClick.AddListener(() => view.skip = true);
            yield return view.Sequence(dim, keys);
            if (go) Destroy(go);
        }

        IEnumerator Sequence(Image dim, int keys)
        {
            System.Func<bool> skipped = () => skip;
            var body = Load("Chest/chest-body"); var lidArt = Load("Chest/chest-lid");
            if (!body || !lidArt) yield break; // thieu art: vao thang man ket qua

            var stage = Rect(transform, "Stage", Vector2.zero, Vector2.one * 640);
            var glow = Layer(stage, "Glow", Load("Common/glow"), new Vector2(0, 40), Vector2.one * 900, Warm);
            var ray = Layer(stage, "Ray", Load("Common/rarity-ray"), new Vector2(0, 60), Vector2.one * 1000, Warm);
            var chest = Rect(stage, "Chest", Vector2.zero, Vector2.one * 640);
            Layer(chest, "Body", body, Vector2.zero, Vector2.one * 640, Color.white);
            var lid = Layer(chest, "Lid", lidArt, Vector2.zero, Vector2.one * 640, Color.white);
            // Doi tam nap ve ban le ma khong lam nap xe dich.
            lid.pivot = new Vector2(.5f, LidHinge); lid.anchoredPosition = new Vector2(0, (LidHinge - .5f) * 640);
            var light = Layer(chest, "Light", Load("Common/chest-light"), new Vector2(0, (LidHinge - .5f) * 640 + 150), new Vector2(560, 420), Warm);
            if (ray) { Alpha(ray, 0); ray.gameObject.AddComponent<UiSpin>().DegreesPerSecond = 20; }
            if (glow) Alpha(glow, 0);
            // Luong sang nam SAU than ruong: chi lo ra o mieng ruong, khong de len mat truoc.
            if (light) { light.SetSiblingIndex(0); light.pivot = new Vector2(.5f, 0); light.anchoredPosition -= new Vector2(0, 210); light.localScale = new Vector3(1, 0, 1); }
            var open = Layer(chest, "Open", Load("Chest/chest-open"), Vector2.zero, Vector2.one * 640, Color.white);
            if (open) open.gameObject.SetActive(false);
            var flash = Layer(stage, "Flash", Load("Common/glow"), new Vector2(0, 60), Vector2.one * 1100, Color.white);
            if (flash) Alpha(flash, 0);

            // 1. Toi nen, ruong bat ra.
            yield return UiTween.Run(UiTween.Normal, t =>
            {
                var c = dim.color; c.a = .82f * t; dim.color = c;
                chest.localScale = Vector3.one * t;
            }, UiTween.OutBack, skipped);

            // 2. Rung manh dan, quang am phia sau tho len.
            yield return UiTween.Run(.7f, t =>
            {
                float power = t * t;
                chest.anchoredPosition = new Vector2(Mathf.Sin(t * 70) * 14 * power, Mathf.Abs(Mathf.Sin(t * 45)) * 8 * power);
                chest.localEulerAngles = new Vector3(0, 0, Mathf.Sin(t * 55) * 5 * power);
                if (glow) { Alpha(glow, .7f * power); glow.localScale = Vector3.one * (.7f + .3f * power); }
            }, UiTween.Linear, skipped);
            chest.anchoredPosition = Vector2.zero; chest.localEulerAngles = Vector3.zero;

            // 3. Nap bat len (nen theo chieu doc = lat ra sau), sang hat ra, ruong nay nhe.
            var shards = Shards(stage, Mathf.Clamp(keys * 3, 6, 24));
            // Nap KHONG bi nen dep nua (nhin nhu nap bep). Neu co anh ruong mo (Chest/chest-open)
            // thi loe trang che khoanh khac doi anh; chua co thi nap bat len va nghieng ra sau.
            yield return UiTween.Run(.22f, t =>
            {
                lid.anchoredPosition = new Vector2(0, (LidHinge - .5f) * 640 + 90 * t);
                lid.localEulerAngles = new Vector3(0, 0, 12 * t);
                if (flash) Alpha(flash, open ? Mathf.Sin(t * Mathf.PI) : 0);
                if (open && t >= .5f && !open.gameObject.activeSelf) { open.gameObject.SetActive(true); lid.gameObject.SetActive(false); chest.Find("Body").gameObject.SetActive(false); }
                if (light) light.localScale = Vector3.one * Mathf.Max(.001f, t);
                if (ray) Alpha(ray, .75f * t);
                chest.localScale = new Vector3(1 + .08f * (1 - t), 1 - .08f * (1 - t), 1);
            }, UiTween.OutBack, skipped);

            // 4. Shard bay vong cung len roi roi, mo dan.
            yield return UiTween.Run(.8f, t =>
            {
                foreach (var s in shards)
                {
                    var p = Vector2.Lerp(s.From, s.To, t); p.y += Mathf.Sin(t * Mathf.PI) * s.Arc;
                    s.Rect.anchoredPosition = p; s.Rect.localEulerAngles = new Vector3(0, 0, s.Spin * t);
                    s.Rect.localScale = Vector3.one * Mathf.Lerp(.4f, 1, Mathf.Min(1, t * 3));
                    Alpha(s.Rect, 1 - Mathf.Max(0, t - .6f) / .4f);
                }
            }, UiTween.OutCubic, skipped);
            foreach (var s in shards) if (s.Rect) Destroy(s.Rect.gameObject);

            // 5. Tan het, nhuong cho man ket qua (bo qua cung di qua buoc nay, rat nhanh).
            yield return UiTween.Run(skip ? .08f : UiTween.Normal, t =>
            {
                var c = dim.color; c.a = .82f * (1 - t); dim.color = c;
                stage.localScale = Vector3.one * (1 + .15f * t);
                foreach (var g in stage.GetComponentsInChildren<Image>()) { var k = g.color; k.a = Mathf.Min(k.a, 1 - t); g.color = k; }
            }, UiTween.Linear);
        }

        sealed class Shard { public RectTransform Rect; public Vector2 From, To; public float Arc, Spin; }

        static List<Shard> Shards(Transform parent, int count)
        {
            var list = new List<Shard>(); var art = Load("Chest/shard"); if (!art) return list;
            for (int i = 0; i < count; i++)
            {
                var r = Layer(parent, "Shard", art, new Vector2(0, 60), Vector2.one * Random.Range(60f, 96f), Color.white);
                float angle = Mathf.Lerp(20, 160, (i + Random.value) / count) * Mathf.Deg2Rad;
                float distance = Random.Range(320f, 520f);
                list.Add(new Shard { Rect = r, From = new Vector2(0, 60), To = new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance * .6f - 120),
                    Arc = Random.Range(160f, 320f), Spin = Random.Range(-540f, 540f) });
                r.localScale = Vector3.zero;
            }
            return list;
        }

        static Sprite Load(string path) => Resources.Load<Sprite>("Anim/" + path);
        static void Stretch(RectTransform r) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero; }
        static RectTransform Rect(Transform parent, string name, Vector2 position, Vector2 size)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>(); r.SetParent(parent, false);
            r.anchorMin = r.anchorMax = r.pivot = Vector2.one * .5f; r.anchoredPosition = position; r.sizeDelta = size; return r;
        }
        static RectTransform Layer(Transform parent, string name, Sprite sprite, Vector2 position, Vector2 size, Color color)
        {
            if (!sprite) return null;
            var r = Rect(parent, name, position, size); var image = r.gameObject.AddComponent<Image>();
            image.sprite = sprite; image.preserveAspect = true; image.raycastTarget = false; image.color = color; return r;
        }
        static void Alpha(RectTransform r, float a) { var i = r.GetComponent<Image>(); var c = i.color; c.a = a; i.color = c; }
    }
}
