using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Project.EditorTools
{
    /// <summary>
    /// Chạy builder theo yêu cầu đặt trong một file, thay vì đợi người bấm menu.
    ///
    /// Vì sao phải chạy BÊN TRONG Editor: file .unity do Editor giữ một bản
    /// trong bộ nhớ. Ghi thẳng vào file từ ngoài thì lần Cmd+S kế tiếp đè mất,
    /// và đè âm thầm. Còn code chạy trong chính Editor đó thì nó SỬA bản trong
    /// bộ nhớ rồi lưu — đúng y việc mà menu vẫn làm.
    ///
    /// Cách dùng, từ ngoài (shell, công cụ, script):
    ///     echo "all" &gt; Temp/spinsquad_request.txt
    ///     ... đợi ~1-2 giây ...
    ///     cat Temp/spinsquad_result.txt
    ///
    /// Yêu cầu hợp lệ: roster | scene | home | all
    ///
    /// Vì sao để trong Temp/ chứ không trong Assets/: file trong Assets làm
    /// Unity import lại, import lại làm recompile, recompile làm agent khởi
    /// động lại — một vòng lặp tự nuôi.
    /// </summary>
    [InitializeOnLoad]
    public static class SpinSquadAgent
    {
        const string RequestPath = "Temp/spinsquad_request.txt";
        const string ResultPath  = "Temp/spinsquad_result.txt";

        // Không dò mỗi frame: Editor chạy update hàng trăm lần một giây, mà
        // File.Exists là một lần chạm đĩa. Nửa giây là đủ nhanh cho người
        // đang đợi, và gần như không tốn gì.
        const double PollSeconds = 0.5;

        const string AttemptPath = "Temp/spinsquad_attempt.txt";

        // Moi yeu cau duoc nhieu nhat 3 vong. Mot vong co the mat vi nap lai
        // domain; ba vong ma chua xong thi la hong that, khong phai dang doi.
        const int MaxAttempts = 3;

        static double nextPoll;

        // Da goi Refresh cho yeu cau dang xu ly chua. Nap lai domain xoa
        // bien nay ve false — dung y muon: sau khi nap lai thi code da moi,
        // Refresh lan nua la khong tot kem gi.
        static bool refreshed;

        static int Attempts()
        {
            try { return int.TryParse(File.ReadAllText(AttemptPath), out var n) ? n : 0; }
            catch (IOException) { return 0; }
        }

        static void WriteAttempts(int n)
        {
            try { File.WriteAllText(AttemptPath, n.ToString()); }
            catch (IOException) { }
        }

        static void Clear()
        {
            try { File.Delete(RequestPath); }  catch (IOException) { }
            try { File.Delete(AttemptPath); }  catch (IOException) { }
        }

        static SpinSquadAgent() => EditorApplication.update += Tick;

        static void Tick()
        {
            if (EditorApplication.timeSinceStartup < nextPoll) return;
            nextPoll = EditorApplication.timeSinceStartup + PollSeconds;

            // Đang biên dịch hay đang import thì ĐỢI, không phải từ chối:
            // builder gọi vào code có thể chưa nạp xong. Yêu cầu nằm nguyên
            // đó, vòng sau xử lý.
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;

            if (!File.Exists(RequestPath)) return;

            string request;
            try { request = File.ReadAllText(RequestPath).Trim().ToLowerInvariant(); }
            catch (IOException) { return; }   // bên kia còn đang ghi dở, thử lại sau

            // KHONG xoa yeu cau o day — xoa khi xong han. Ly do o ngay duoi:
            // AssetDatabase.Refresh() co the lam nap lai domain, giet ham nay
            // giua chung. Yeu cau con nguyen thi vong sau chay tiep.
            var tries = Attempts() + 1;
            WriteAttempts(tries);
            if (tries > MaxAttempts)
            {
                Clear();
                Finish(new List<string>(), false,
                    $"thu {MaxAttempts} lan van khong xong — dung lai de khoi lap vo han");
                return;
            }

            // Nap lai code MOI nhat truoc khi chay. Thieu buoc nay thi sua
            // builder xong goi agent van chay ban cu — va chay im lang, ket
            // qua trong nhu da sua ma thuc ra chua.
            //
            // PHAI tra ve ngay sau Refresh, khong duoc chay tiep. Refresh chi
            // XEP HANG viec import va bien dich; Unity nap lai domain o mot
            // nhip sau, khong phai ngay tai day. Chay tiep la chay bang code
            // CU — da dinh dung loi do: gui 'home-reset' ma agent tra loi
            // "khong hieu", vi lenh do chi co trong ban vua sua.
            //
            // Tra ve thi yeu cau con nguyen. Vong sau: dang bien dich thi
            // chot o tren giu lai; bien dich xong, domain nap lai, 'refreshed'
            // ve false, Refresh lan hai khong con gi de lam, va di tiep.
            if (!refreshed)
            {
                refreshed = true;
                AssetDatabase.Refresh();
                return;
            }

            Handle(request);
        }

        /// <summary>
        /// Scene mà yêu cầu này BẮT BUỘC phải chạy trong đó. null = không
        /// đụng scene nào (ví dụ 'roster' chỉ ghi file .asset).
        /// </summary>
        static string SceneFor(string request) => request switch
        {
            "scene" or "all" => "Assets/_Project/Scenes/CharacterLab.unity",
            "home" or "home-reset" => "Assets/_Project/Scenes/Home.unity",
            "treasure" => "Assets/_Project/Scenes/Treasure.unity",
            "upgrade" => "Assets/_Project/Scenes/Upgrade.unity",
            _ => null,
        };

        static void Handle(string request)
        {
            var force = request.EndsWith(" force");
            if (force) request = request.Substring(0, request.Length - 6).Trim();

            var log = new List<string>();
            void Say(string s) { log.Add(s); }

            Say($"yeu cau: '{request}'{(force ? " (force)" : "")}");

            // ── chốt 1: Play mode ─────────────────────────────────────
            // Sửa scene lúc đang Play thì mọi thay đổi bốc hơi khi thoát Play.
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Finish(log, false, "DANG PLAY MODE. Thoat Play roi gui lai.");
                return;
            }

            // ── chốt 0: ĐÚNG SCENE ────────────────────────────────────
            // Builder dựng vào scene ĐANG MỞ, nó không tự kiểm. Lần chạy
            // 14:37 hôm nay đổ nguyên scene trận — Grid_Ally, HUD, Lane,
            // Flow — vào Home.unity, vì lúc đó Home đang mở. Hai scene
            // trộn làm một, và không có gì kêu lên.
            //
            // Nên agent MỞ scene đúng trước, chứ không trông vào việc người
            // dùng tình cờ đang mở cái nào.
            var want = SceneFor(request);
            if (want != null &&
                !EditorSceneManager.GetActiveScene().path.EndsWith(want))
            {
                if (EditorSceneManager.GetActiveScene().isDirty && !force)
                {
                    Finish(log, false,
                        $"Can mo '{want}' nhung scene hien tai chua luu. " +
                        $"Luu (Cmd+S) roi gui lai.");
                    return;
                }
                Say($"mo scene '{want}'");
                EditorSceneManager.OpenScene(want);
            }

            var scene = EditorSceneManager.GetActiveScene();

            // ── chốt 2: scene có sửa tay chưa lưu ─────────────────────
            // Đây là chốt quan trọng nhất. Người dùng vừa kéo một con lính
            // mà chưa Cmd+S, mình chạy builder là công đó mất sạch và mất
            // âm thầm. DỪNG, không tự quyết. 'force' chỉ dùng khi người
            // dùng đã nói rõ là bỏ được.
            var touchesScene = request is "scene" or "home" or "all"
                                        or "home-reset" or "treasure" or "upgrade";
            if (touchesScene && scene.isDirty && !force)
            {
                Finish(log, false,
                    $"Scene '{scene.name}' dang co thay doi CHUA LUU. Dung lai de " +
                    $"khong de mat. Luu (Cmd+S) roi gui lai, hoac gui '{request} force' " +
                    $"neu chap nhan bo phan chua luu do.");
                return;
            }

            // Gom cả Debug.Log của builder vào kết quả — không thì bên ngoài
            // chỉ biết "xong" mà không biết nó làm gì, và "xong" không chứng
            // minh là ĐÚNG.
            void OnLog(string msg, string stack, LogType type)
            {
                if (type is LogType.Error or LogType.Exception)      Say($"  [LOI] {msg}");
                else if (type == LogType.Warning)                    Say($"  [canh bao] {msg}");
                else if (msg.StartsWith("["))                        Say($"  {msg}");
            }

            Application.logMessageReceived += OnLog;
            try
            {
                switch (request)
                {
                    case "roster": UnitRosterBuilder.Run();      break;
                    case "treasure-db": TreasureDatabaseBuilder.Run(); break;
                    case "scenes": BuildScenesRegistrar.Run();   break;
                    case "scene":  BattleSceneSetup.SetupPortrait(); break;
                    case "home":   HomeSceneSetup.Build();       break;
                    case "treasure": TreasureSceneSetup.Build();  break;
                    case "upgrade": UpgradeSceneSetup.Build();    break;

                    // Don sach roi dung lai tu dau. Dung de cuu Home.unity
                    // sau khi scene tran bi do nham vao. An toan vi toan bo
                    // noi dung trang chu do HomeSceneSetup sinh ra — khong
                    // co gi dat tay trong do de mat.
                    case "home-reset":
                        var roots = scene.GetRootGameObjects();
                        Say($"xoa {roots.Length} object goc");
                        foreach (var go in roots) UnityEngine.Object.DestroyImmediate(go);
                        HomeSceneSetup.Build();
                        break;
                    case "all":
                        UnitRosterBuilder.Run();
                        BattleSceneSetup.SetupPortrait();
                        break;
                    default:
                        Finish(log, false,
                            $"Khong hieu '{request}'. Hop le: roster | treasure-db | scenes | scene | home | home-reset | treasure | upgrade | all");
                        return;
                }

                if (touchesScene)
                {
                    var saved = EditorSceneManager.SaveOpenScenes();
                    Say(saved ? $"da luu scene '{scene.name}'"
                              : $"LUU THAT BAI cho scene '{scene.name}'");
                    if (!saved) { Finish(log, false, "khong luu duoc scene"); return; }
                }
                else
                {
                    AssetDatabase.SaveAssets();
                    Say("da luu asset");
                }

                Finish(log, true, "xong");
            }
            catch (Exception e)
            {
                Finish(log, false, $"NGOAI LE: {e.GetType().Name}: {e.Message}");
            }
            finally
            {
                Application.logMessageReceived -= OnLog;
            }
        }

        static void Finish(List<string> log, bool ok, string summary)
        {
            Clear();
            refreshed = false;
            log.Add((ok ? "OK: " : "THAT BAI: ") + summary);
            var text = $"{DateTime.Now:HH:mm:ss}\n{string.Join("\n", log)}\n";
            try { File.WriteAllText(ResultPath, text); }
            catch (IOException) { /* khong ghi duoc thi thoi, van log ra Console */ }
            Debug.Log($"[SpinSquadAgent]\n{text}");
        }
    }
}
