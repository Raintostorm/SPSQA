using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>
    /// Hai font của game (UI-005): Lilita One cho tiêu đề/nút/số/tên,
    /// Fredoka SemiBold cho câu mô tả và chữ nhỏ.
    ///
    /// Vì sao có máy quét: chữ được tạo ở ~30 file và trong scene, mỗi chỗ
    /// tự đặt font + FontStyle.Bold. Hai font này đã dày sẵn, Bold giả của
    /// Unity làm nét nhoè, nên mọi Text đều phải về Normal. Quét định kỳ
    /// thì chữ mới (popup, panel dựng muộn) cũng được sửa mà không phải
    /// sửa tay từng helper.
    /// </summary>
    public static class UiFonts
    {
        const string DisplayPath = "Fonts/LilitaOne-Regular";
        const string BodyPath = "Fonts/Fredoka-SemiBold";

        // Chữ từ cỡ này trở xuống luôn là chữ thân bài.
        const int BodyMaxSize = 24;
        // Ít nhất bấy nhiêu từ có chữ cái thì là một câu, không phải nhãn.
        const int SentenceWords = 4;

        static Font display, body, legacy;

        public static Font Display => display ? display : display = Load(DisplayPath);
        public static Font Body => body ? body : body = Load(BodyPath);

        static Font Load(string path)
        {
            var font = Resources.Load<Font>(path);
            if (font) return font;
            Debug.LogWarning("[UiFonts] Thiếu " + path + ", dùng font mặc định.");
            return legacy ? legacy : legacy = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        /// <summary>
        /// Câu (≥4 từ, cho xuống dòng) hoặc chữ nhỏ → Body; còn lại → Display.
        /// Quyết định một lần cho mỗi Text, để nhãn đổi nội dung không nhảy font.
        /// </summary>
        public static bool IsBody(Text t)
        {
            if (t.fontSize <= BodyMaxSize) return true;
            if (t.horizontalOverflow != HorizontalWrapMode.Wrap) return false;
            int words = 0;
            foreach (var word in t.text.Split(' ', '\n'))
                if (HasLetter(word) && ++words >= SentenceWords) return true;
            return false;
        }

        static bool HasLetter(string s)
        {
            foreach (var c in s) if (char.IsLetter(c)) return true;
            return false;
        }

        // Text → đã chọn Body chưa. Xoá mỗi lần nạp scene nên Text đã huỷ không đọng lại.
        static readonly Dictionary<Text, bool> roles = new();

        public static void Apply(Text t)
        {
            if (!t || string.IsNullOrEmpty(t.text)) return;
            if (!roles.TryGetValue(t, out var isBody)) roles[t] = isBody = IsBody(t);
            var font = isBody ? Body : Display;
            if (t.font != font) t.font = font;
            if (t.fontStyle != FontStyle.Normal) t.fontStyle = FontStyle.Normal;
        }

        public static void ApplyAll()
        {
            foreach (var t in Object.FindObjectsByType<Text>(FindObjectsInactive.Include))
                Apply(t);
        }

        // Static sống qua lần Play khi tắt Reload Domain — cùng bẫy với SaveSystem.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() { roles.Clear(); SceneManager.sceneLoaded -= OnSceneLoaded; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            var go = new GameObject("UiFonts") { hideFlags = HideFlags.HideInHierarchy };
            Object.DontDestroyOnLoad(go);
            go.AddComponent<Sweeper>();
            ApplyAll();
        }

        // sceneLoaded chạy sau Awake/OnEnable, trước Start: chữ trong scene
        // đổi font trước khi code nào chép font của nó sang chữ khác.
        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            roles.Clear();
            ApplyAll();
        }

        /// <summary>Quét lại mỗi 0.5s cho chữ dựng sau khi scene đã nạp.</summary>
        sealed class Sweeper : MonoBehaviour
        {
            const float Interval = .5f;
            float next;
            void Update()
            {
                if (Time.unscaledTime < next) return;
                next = Time.unscaledTime + Interval;
                ApplyAll();
            }
        }
    }
}
