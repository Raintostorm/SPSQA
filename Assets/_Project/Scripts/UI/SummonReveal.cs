using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // ANIM-002: hoat anh trieu hoi, chi la trinh dien — TryRoll da cap va luu thuong truoc do.
    // Cau sang -> no tia -> the bat ra mat sau -> lat. Luat: mau do hiem CHI hien SAU khi the lat,
    // nen cau va tia no luon trang/xanh trung tinh, khong lo truoc ket qua.
    // Cham bat ky dau = bo qua, nhay thang toi trang thai cuoi.
    public sealed class SummonReveal : MonoBehaviour
    {
        // 0 common, 1 rare, 2 epic, 3 legendary, 4 mythic stone.
        static readonly Color[] Tint =
        {
            new Color(.85f, .80f, .72f), new Color(.35f, .75f, 1f), new Color(.80f, .45f, 1f),
            new Color(1f, .80f, .25f), new Color(1f, .55f, .20f)
        };
        static readonly Color Neutral = new Color(.75f, .92f, 1f);

        List<RectTransform> cards; List<int> tiers; RectTransform stage;
        bool skip;
        GameObject skipLayer;

        public void Play(List<RectTransform> cards, List<int> tiers, RectTransform stage)
        {
            this.cards = cards; this.tiers = tiers; this.stage = stage;
            foreach (var card in cards)
            {
                // The duoc dung voi tam o mep tren; doi tam ve giua (giu nguyen cho dung) de
                // phong va lat quanh tam the, khong phai quanh mep tren.
                var shift = (new Vector2(.5f, .5f) - card.pivot) * card.sizeDelta;
                card.pivot = new Vector2(.5f, .5f); card.anchoredPosition += shift;
                card.localScale = Vector3.zero; // an ngay, khong loe mot khung
            }
            skipLayer = Layer(stage, "SkipTap", null, Vector2.zero, Vector2.zero).gameObject;
            var full = (RectTransform)skipLayer.transform;
            full.anchorMin = Vector2.zero; full.anchorMax = Vector2.one; full.offsetMin = full.offsetMax = Vector2.zero;
            var hit = skipLayer.GetComponent<Image>(); hit.color = new Color(0, 0, 0, 0); hit.raycastTarget = true;
            skipLayer.AddComponent<Button>().onClick.AddListener(() => skip = true);
            StartCoroutine(Sequence());
        }

        bool Skipped() => skip;

        IEnumerator Sequence()
        {
            // 1. Tu phep (lo 059): vong rune quay cham, hai vong xoay nguoc chieu, loi tho,
            //    vet sang bi hut vao tam, rung manh dan. Nhieu lop khac nhip = mem, khong sung.
            var rune = Layer(stage, "RevealRune", Load("Summon/rune-circle"), Vector2.zero, Vector2.one * 820);
            var swirlB = Layer(stage, "RevealSwirlB", Load("Summon/orb-swirl-b"), Vector2.zero, Vector2.one * 560);
            var swirlA = Layer(stage, "RevealSwirlA", Load("Summon/orb-swirl-a"), Vector2.zero, Vector2.one * 520);
            var glow = Layer(stage, "RevealGlow", Load("Common/glow"), Vector2.zero, Vector2.one * 900);
            var orb = Layer(stage, "RevealOrb", Load("Summon/orb-core"), Vector2.zero, Vector2.one * 420);
            foreach (var r in new[] { rune, swirlB, swirlA, glow }) if (r) r.GetComponent<Image>().color = Neutral;
            var streaks = Streaks(stage, 16);
            float charge = (rune || swirlA) ? 1.3f : UiTween.Slow; // thieu art moi: nhip ngan nhu cu
            float spinA = 0, spinB = 0, spinR = 0;
            yield return UiTween.Run(charge, t =>
            {
                float dt = Time.unscaledDeltaTime, speed = 1 + 3 * t * t;
                spinA += 90 * speed * dt; spinB -= 60 * speed * dt; spinR += 20 * dt;
                float grow = UiTween.OutBack(Mathf.Min(1, t * 2.5f));
                float breathe = 1 + .05f * Mathf.Sin(t * 18);
                float shake = Mathf.Max(0, t - .65f) / .35f;
                stage.anchoredPosition = new Vector2(Mathf.Sin(t * 90) * 10 * shake, Mathf.Cos(t * 75) * 6 * shake);
                if (rune) { rune.localScale = Vector3.one * Mathf.Lerp(.8f, 1, grow); rune.localEulerAngles = new Vector3(0, 0, spinR); Alpha(rune, .8f * Mathf.Min(1, t * 3)); }
                if (swirlA) { swirlA.localScale = Vector3.one * Mathf.Lerp(.3f, 1, grow) * breathe; swirlA.localEulerAngles = new Vector3(0, 0, -spinA); Alpha(swirlA, Mathf.Min(1, t * 3)); }
                if (swirlB) { swirlB.localScale = Vector3.one * Mathf.Lerp(.3f, 1, grow); swirlB.localEulerAngles = new Vector3(0, 0, -spinB); Alpha(swirlB, .8f * Mathf.Min(1, t * 3)); }
                if (orb) orb.localScale = Vector3.one * Mathf.Lerp(.5f, 1, grow) * breathe * (1 + .15f * shake);
                if (glow) { glow.localScale = Vector3.one * Mathf.Lerp(.4f, 1.1f, t); Alpha(glow, .4f + .6f * t); }
                foreach (var k in streaks)
                {
                    float u = Mathf.Clamp01((t - k.Delay) / .45f);
                    k.Rect.anchoredPosition = k.From * (1 - UiTween.InCubic(u));
                    Alpha(k.Rect, u <= 0 || u >= 1 ? 0 : Mathf.Sin(u * Mathf.PI));
                }
            }, UiTween.Linear, Skipped);
            stage.anchoredPosition = Vector2.zero;
            foreach (var k in streaks) Kill(k.Rect);

            // 2. Loe trang + no tia; moi lop tu tan.
            var flash = Layer(stage, "RevealFlash", Load("Summon/flash") ?? Load("Common/glow"), Vector2.zero, Vector2.one * 2400);
            var burst = Layer(stage, "RevealBurst", Load("Summon/burst-0"), Vector2.zero, Vector2.one * 1000);
            var frames = new Sprite[6]; for (int i = 0; i < 6; i++) frames[i] = Load("Summon/burst-" + i);
            if (burst) burst.GetComponent<Image>().color = Neutral;
            yield return UiTween.Run(.4f, t =>
            {
                if (burst) { var frame = frames[Mathf.Min(5, (int)(t * 6))]; if (frame) burst.GetComponent<Image>().sprite = frame; }
                if (flash) Alpha(flash, t < .2f ? t / .2f : 1 - (t - .2f) / .8f);
                foreach (var r in new[] { orb, glow, rune, swirlA, swirlB }) if (r) { Alpha(r, 1 - t); r.localScale *= 1 + .02f; }
            }, UiTween.Linear, Skipped);
            Kill(orb); Kill(glow); Kill(burst); Kill(rune); Kill(swirlA); Kill(swirlB); Kill(flash);

            // 3. Tung the bat ra roi lat. Mot luot trieu hoi thi cham hon cho co khoanh khac.
            float gap = cards.Count == 1 ? 0 : .1f;
            var running = new List<Coroutine>();
            for (int i = 0; i < cards.Count; i++)
            {
                running.Add(StartCoroutine(Card(cards[i], tiers[i])));
                yield return UiTween.Wait(gap, Skipped);
            }
            foreach (var c in running) yield return c;

            if (skipLayer) Destroy(skipLayer);
            Destroy(this);
        }

        IEnumerator Card(RectTransform card, int tier)
        {
            var back = Layer(card, "CardBack", Load("Summon/card-back"), Vector2.zero, Vector2.zero);
            if (back) { back.anchorMin = Vector2.zero; back.anchorMax = Vector2.one; back.offsetMin = back.offsetMax = Vector2.zero; back.GetComponent<Image>().preserveAspect = false; }

            yield return UiTween.Run(.22f, t => card.localScale = Vector3.one * t, UiTween.OutBack, Skipped);
            // The hiem dung lau hon truoc khi lat — chi la nhip, khong lo mau.
            yield return UiTween.Wait(tier >= 3 ? .35f : tier >= 2 ? .18f : .05f, Skipped);
            yield return UiTween.Run(.1f, t => card.localScale = new Vector3(1 - t, 1, 1), UiTween.InCubic, Skipped);
            Kill(back);
            if (tier >= 1) Ray(card, tier);
            yield return UiTween.Run(.14f, t => card.localScale = new Vector3(t, 1, 1), UiTween.OutBack, Skipped);
            card.localScale = Vector3.one;
        }

        // Tia sang nam SAU the: dat vao mot khung cung vi tri voi the, chen ngay truoc the.
        void Ray(RectTransform card, int tier)
        {
            var sprite = Load("Common/rarity-ray"); if (!sprite || !card.parent) return;
            var holder = new GameObject("RarityRay", typeof(RectTransform)).GetComponent<RectTransform>();
            holder.SetParent(card.parent, false);
            holder.anchorMin = card.anchorMin; holder.anchorMax = card.anchorMax; holder.pivot = card.pivot;
            holder.anchoredPosition = card.anchoredPosition; holder.sizeDelta = card.sizeDelta;
            holder.SetSiblingIndex(card.GetSiblingIndex());
            var ray = Layer(holder, "Ray", sprite, Vector2.zero, Vector2.one * (tier >= 3 ? 520 : 420));
            var image = ray.GetComponent<Image>(); var c = Tint[Mathf.Clamp(tier, 0, 4)]; c.a = tier >= 3 ? .9f : .6f; image.color = c;
            ray.gameObject.AddComponent<UiSpin>().DegreesPerSecond = tier >= 3 ? 40 : 25;
        }

        sealed class Streak { public RectTransform Rect; public Vector2 From; public float Delay; }

        // Vet sang tu xa bay vao tam, mui huong vao tam. Thieu art thi tra danh sach rong.
        static List<Streak> Streaks(RectTransform stage, int count)
        {
            var list = new List<Streak>(); var art = Load("Summon/streak"); if (!art) return list;
            for (int i = 0; i < count; i++)
            {
                float angle = (i + Random.value * .8f) / count * 360f;
                var from = (Vector2)(Quaternion.Euler(0, 0, angle) * Vector3.right) * Random.Range(520f, 700f);
                var r = Layer(stage, "Streak", art, from, new Vector2(Random.Range(180f, 260f), 48));
                r.localEulerAngles = new Vector3(0, 0, angle); r.GetComponent<Image>().preserveAspect = false;
                var image = r.GetComponent<Image>(); image.color = new Color(Neutral.r, Neutral.g, Neutral.b, 0);
                list.Add(new Streak { Rect = r, From = from, Delay = Random.Range(0f, .5f) });
            }
            return list;
        }

        static Sprite Load(string path) => Resources.Load<Sprite>("Anim/" + path);

        // Tao mot lop Image canh giua cha. sprite null -> tra null (thieu art thi bo qua buoc do),
        // tru lop cham-bo-qua la lop co y khong co anh.
        static RectTransform Layer(Transform parent, string name, Sprite sprite, Vector2 position, Vector2 size)
        {
            if (!sprite && name != "SkipTap") return null;
            var r = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            r.SetParent(parent, false);
            r.anchorMin = r.anchorMax = r.pivot = Vector2.one * .5f; r.anchoredPosition = position; r.sizeDelta = size;
            var image = r.GetComponent<Image>(); image.sprite = sprite; image.preserveAspect = true; image.raycastTarget = false;
            return r;
        }

        static void Alpha(RectTransform r, float a) { var i = r.GetComponent<Image>(); var c = i.color; c.a = a; i.color = c; }
        static void Kill(RectTransform r) { if (r) Destroy(r.gameObject); }
    }
}
