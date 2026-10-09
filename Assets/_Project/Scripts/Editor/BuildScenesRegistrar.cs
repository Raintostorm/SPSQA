using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools
{
    /// <summary>
    /// Đăng ký mọi scene trong _Project/Scenes vào Build Settings.
    ///
    /// SceneManager.LoadScene chỉ thấy scene đã nằm trong danh sách đó.
    /// Thiếu một cái thì nút bấm vào rơi vào hư không — và lỗi hiện ra lúc
    /// CHẠY, không phải lúc biên dịch.
    ///
    /// Home luôn đứng đầu: Unity mở scene index 0 khi chạy bản build.
    /// </summary>
    public static class BuildScenesRegistrar
    {
        const string Dir = "Assets/_Project/Scenes";
        static readonly string[] Order = { "Home", "CharacterLab", "Treasure", "Upgrade" };

        [MenuItem("SpinSquad/Đăng ký scene vào Build Settings")]
        public static void Run()
        {
            var found = new List<string>();
            foreach (var name in Order)
            {
                var path = $"{Dir}/{name}.unity";
                if (File.Exists(path)) found.Add(path);
                else Debug.LogWarning($"[BuildScenesRegistrar] Chưa có {path}.");
            }

            // Scene nao co trong thu muc ma khong nam trong Order thi van
            // them vao cuoi — quen mot cai la no im lang khong load duoc.
            foreach (var guid in AssetDatabase.FindAssets("t:Scene", new[] { Dir }))
            {
                var p = AssetDatabase.GUIDToAssetPath(guid);
                if (!found.Contains(p)) found.Add(p);
            }

            EditorBuildSettings.scenes = found
                .ConvertAll(p => new EditorBuildSettingsScene(p, true))
                .ToArray();

            Debug.Log($"[BuildScenesRegistrar] {found.Count} scene: " +
                      string.Join(", ", found.ConvertAll(Path.GetFileNameWithoutExtension)));
        }
    }
}
