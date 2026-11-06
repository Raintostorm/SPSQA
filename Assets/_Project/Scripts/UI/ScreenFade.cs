using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.UI
{
    // ANIM-006: moi lan vao man moi, man hien ra tu toi thay vi cat cung.
    // Chi lam phan "hien ra" (khong tre viec nap man), nen khong dung vao luong SceneRoutes.
    public sealed class ScreenFade : MonoBehaviour
    {
        const float Duration = .28f;
        Image cover; float elapsed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install() { SceneManager.sceneLoaded -= OnLoaded; SceneManager.sceneLoaded += OnLoaded; }

        static void OnLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single) return;
            var go = new GameObject("ScreenFade", typeof(Canvas), typeof(Image));
            var canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 999;
            var fade = go.AddComponent<ScreenFade>(); fade.cover = go.GetComponent<Image>();
            fade.cover.color = new Color(.03f, .04f, .09f, 1); fade.cover.raycastTarget = false;
        }

        void Update()
        {
            elapsed += Time.unscaledDeltaTime;
            var c = cover.color; c.a = 1 - UiTween.OutCubic(Mathf.Clamp01(elapsed / Duration)); cover.color = c;
            if (elapsed >= Duration) Destroy(gameObject);
        }
    }

    // ANIM-006: popup phong nhe 0.92 -> 1 va hien dan. Gan vao khung popup ngay khi dung.
    public sealed class UiPop : MonoBehaviour
    {
        CanvasGroup group; float elapsed;
        public static void Play(Component panel) { if (panel && !panel.GetComponent<UiPop>()) panel.gameObject.AddComponent<UiPop>(); }
        void Awake() { group = gameObject.AddComponent<CanvasGroup>(); Tick(0); }
        void Update() { elapsed += Time.unscaledDeltaTime; Tick(Mathf.Clamp01(elapsed / UiTween.Fast)); if (elapsed >= UiTween.Fast) { Tick(1); Destroy(group); Destroy(this); } }
        void Tick(float t) { transform.localScale = Vector3.one * Mathf.Lerp(.92f, 1, UiTween.OutBack(t)); group.alpha = t; }
    }
}
