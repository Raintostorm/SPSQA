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
            elapsed += Time.deltaTime;
            if (elapsed < 0f) return;
            current = Mathf.Lerp(from, target, Mathf.SmoothStep(0f, 1f, elapsed / .65f));
            label.color = elapsed < 1f ? new Color(.2f,1f,.35f) : Color.Lerp(new Color(.2f,1f,.35f), normal, (elapsed - 1f) / .25f);
            Draw();
            if (elapsed >= 1.25f) { active = false; current = target; label.color = normal; Draw(); }
        }
        void Draw() { label.text = prefix + (percent ? current.ToString("P0") : Mathf.RoundToInt(current).ToString()); }
    }
}
