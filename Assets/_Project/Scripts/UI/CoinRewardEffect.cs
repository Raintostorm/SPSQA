using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>Non-blocking coin rain drawn above the counter in its own canvas.</summary>
    public sealed class CoinRewardEffect : MonoBehaviour
    {
        RectTransform[] coins;
        Vector2 destination;
        float elapsed;
        public static void Show(Text label)
        {
            var sprite = Resources.Load<Sprite>("HomeChrome/coin");
            var owner = label.GetComponentInParent<Canvas>();
            if (!sprite || !owner) return;
            var go = new GameObject("CoinRewardEffect", typeof(RectTransform), typeof(Canvas));
            go.transform.SetParent(owner.transform, false);
            var rt = go.GetComponent<RectTransform>(); rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;
            var canvas = go.GetComponent<Canvas>(); canvas.overrideSorting = true; canvas.sortingOrder = owner.sortingOrder + 20;
            var fx = go.AddComponent<CoinRewardEffect>();
            fx.destination = rt.InverseTransformPoint(label.transform.position);
            fx.coins = new RectTransform[8];
            for (var i = 0; i < fx.coins.Length; i++)
            {
                var image = new GameObject("Coin", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
                image.transform.SetParent(go.transform, false); image.sprite = sprite; image.preserveAspect = true; image.raycastTarget = false;
                image.rectTransform.sizeDelta = new Vector2(38,38);
                fx.coins[i] = image.rectTransform;
            }
        }
        void Update()
        {
            if (SharedSettings.IsPaused) return;
            elapsed += Time.unscaledDeltaTime;
            for (var i = 0; i < coins.Length; i++)
            {
                var t = Mathf.Clamp01((elapsed - i * .04f) / .6f);
                var start = destination + new Vector2((i - 3.5f) * 25f, 170f + i * 9f);
                coins[i].anchoredPosition = Vector2.Lerp(start, destination, t * t);
                coins[i].localRotation = Quaternion.Euler(0,0,t*160f);
                coins[i].gameObject.SetActive(t < 1f);
            }
            if (elapsed > 1f) Destroy(gameObject);
        }
    }
}
