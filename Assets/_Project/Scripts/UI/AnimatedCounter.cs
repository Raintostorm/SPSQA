using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>Increases count in green, hold, then restore the original label color.</summary>
    public sealed class AnimatedCounter : MonoBehaviour
    {
        Text label;
        Color normal;
        float current, from, target, elapsed;
        bool initialized, percent, active;
        string prefix;
        // Vi tien (vang, chia, gem): lan dau chi hien so; lan sau so TANG thi chay xanh len roi ve mau cu.
        // Nho so da hien theo key, song qua cac lan doi man.
        static readonly System.Collections.Generic.Dictionary<string, int> shown = new System.Collections.Generic.Dictionary<string, int>();
        public static void Wallet(Text text, int value, string key)
        {
            if (!text) return;
            if (shown.TryGetValue(key, out var last) && value > last && !text.GetComponent<AnimatedCounter>()) Set(text, last, "");
            Set(text, value, "", false, .3f);
            shown[key] = value;
        }
        public static void Set(Text text, float value, string prefix, bool percent = false, float delay = 0f)
        {
            if (!text) return;
            var counter = text.GetComponent<AnimatedCounter>();
            if (!counter) counter = text.gameObject.AddComponent<AnimatedCounter>();
            counter.Configure(value, prefix, percent, delay);
        }
        void Configure(float value, string lead, bool pct, float delay)
        {
            if (!label) { label = GetComponent<Text>(); normal = label.color; }
            prefix = lead; percent = pct;
            if (initialized && Mathf.Approximately(value, target)) return;
            from = current; target = value; elapsed = -Mathf.Max(0,delay);
            active = initialized && value > current;
            initialized = true;
            if (!active) { current = target; label.color = normal; }
            Draw();
        }
        void Update()
        {
            if (!active || SharedSettings.IsPaused) return;
            elapsed += Time.unscaledDeltaTime; // khong nhanh theo nut x2/x3
            if (elapsed < 0f) return;
            current = Mathf.Lerp(from, target, Mathf.SmoothStep(0f, 1f, elapsed / .65f));
            label.color = elapsed < 1f ? new Color(.2f,1f,.35f) : Color.Lerp(new Color(.2f,1f,.35f), normal, (elapsed - 1f) / .25f);
            Draw();
            if (elapsed >= 1.25f) { active = false; current = target; label.color = normal; Draw(); }
        }
        void Draw() { label.text = prefix + (percent ? current.ToString("P0") : Mathf.RoundToInt(current).ToString("N0")); }
    }
}
