#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Project.EditorTools
{
    public static class MythicCapture
    {
        [MenuItem("SpinSquad/Diagnostics/Capture Mythic UI")]
        static void Capture()
        {
            if(!Application.isPlaying)return;
            Directory.CreateDirectory("artsrc/units/mythic-integration");
            ScreenCapture.CaptureScreenshot("artsrc/units/mythic-integration/game-ui.png");
        }
    }
}
#endif
