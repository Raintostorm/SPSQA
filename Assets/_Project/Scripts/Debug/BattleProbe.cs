#if UNITY_EDITOR
using System.Collections;
using System.IO;
using System.Text;
using Project.Battle;
using UnityEngine;

namespace Project.DebugTools
{
    /// <summary>
    /// Ghi lại diễn biến trận ra file để đọc được từ bên ngoài Unity.
    ///
    /// Tồn tại vì một lý do rất đời: người ngồi ngoài Unity (hoặc một con AI)
    /// không bấm Play được và không nhìn được cửa sổ Game. Script này biến
    /// trận đấu thành hai thứ đọc được bằng mắt thường:
    ///
    ///   Logs/battle_probe.log      bang so theo thoi gian
    ///   Logs/frames/f_xx.png       anh cua so Game
    ///
    /// Tự gắn vào scene khi bấm Play, không cần kéo thả gì.
    /// Chỉ biên dịch trong Editor — không lọt vào bản build.
    /// </summary>
    public sealed class BattleProbe : MonoBehaviour
    {
        const float SampleInterval = 0.25f;
        const float ShotInterval   = 1.0f;
        const int   MaxShots       = 12;
        const float MaxSeconds     = 30f;

        static string LogPath    => Path.Combine(Directory.GetCurrentDirectory(), "Logs", "battle_probe.log");
        static string FramesPath => Path.Combine(Directory.GetCurrentDirectory(), "Logs", "frames");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoSpawn()
        {
            var go = new GameObject("~BattleProbe");
            go.hideFlags = HideFlags.DontSave;
            go.AddComponent<BattleProbe>();
            DontDestroyOnLoad(go);
        }

        readonly StringBuilder sb = new StringBuilder();
        float elapsed;
        int shots;

        IEnumerator Start()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
            Directory.CreateDirectory(FramesPath);
            foreach (var old in Directory.GetFiles(FramesPath, "f_*.png")) File.Delete(old);

            sb.AppendLine($"# scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}  " +
                          $"timeScale={Time.timeScale}");
            sb.AppendLine($"{"t",6} {"ten",-10} {"phe",-5} {"x",7} {"hp",7} {"song",4} {"dan",4}");

            var nextShot = 0f;

            while (elapsed < MaxSeconds)
            {
                Sample();

                if (elapsed >= nextShot && shots < MaxShots)
                {
                    nextShot += ShotInterval;
                    var file = Path.Combine(FramesPath, $"f_{shots:00}.png");
                    ScreenCapture.CaptureScreenshot(file);
                    shots++;
                }

                Flush();
                yield return new WaitForSeconds(SampleInterval);
                elapsed += SampleInterval;
            }

            sb.AppendLine($"# het {MaxSeconds}s, ngung ghi");
            Flush();
        }

        void Sample()
        {
            var units = UnitRegistry.All;
            var flying = FindObjectsByType<Projectile>(FindObjectsSortMode.None).Length;

            for (var i = 0; i < units.Count; i++)
            {
                var u = units[i];
                if (u == null) continue;
                sb.AppendLine($"{elapsed,6:F2} {u.name,-10} {u.Team,-5} " +
                              $"{u.transform.position.x,7:F2} {u.CurrentHp,7:F0} " +
                              $"{(u.IsAlive ? "x" : "-"),4} {(i == 0 ? flying.ToString() : ""),4}");
            }
        }

        void Flush()
        {
            if (sb.Length == 0) return;
            File.AppendAllText(LogPath, sb.ToString());
            sb.Clear();
        }

        void OnApplicationQuit() => Flush();
        void OnDisable() => Flush();
    }
}
#endif
