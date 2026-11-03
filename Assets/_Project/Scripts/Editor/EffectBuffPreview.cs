#if UNITY_EDITOR
using System;
using Project.Battle;
using Project.Data;
using Project.UI;
using UnityEditor;
using UnityEngine;
namespace Project.EditorTools
{
    public static class EffectBuffPreview
    {
        [MenuItem("SpinSquad/Diagnostics/Preview Epic Effect Buffs (no grants)")]
        static void Epic()=>Show(Rank.Epic);
        [MenuItem("SpinSquad/Diagnostics/Preview Legendary Effect Buffs (no grants)")]
        static void Legend()=>Show(Rank.Legendary);
        [MenuItem("SpinSquad/Diagnostics/Test Ground Buff Tiles (current run only)")]
        static void GroundTiles()
        {
            if(!EditorApplication.isPlaying)return;
            var system=UnityEngine.Object.FindFirstObjectByType<EffectBuffSystem>();
            if(!system){Debug.LogWarning("Open battle in Play Mode first.");return;}
            system.PreviewGroundTiles();
        }
        [MenuItem("SpinSquad/Diagnostics/Capture Ground Buff Tiles")]
        static void CaptureGround()
        {
            if(!EditorApplication.isPlaying)return;
            System.IO.Directory.CreateDirectory("artsrc/ui/ground-buffs-v1");
            ScreenCapture.CaptureScreenshot("artsrc/ui/ground-buffs-v1/unity-play.png");
        }
        [MenuItem("SpinSquad/Diagnostics/Preview Buff HUD 6 (no grants)")]
        static void Hud6()=>Hud(6);
        [MenuItem("SpinSquad/Diagnostics/Preview Buff HUD 10 (no grants)")]
        static void Hud10()=>Hud(10);
        static void Hud(int count)
        {
            if(!EditorApplication.isPlaying)return;
            var hud=UnityEngine.Object.FindFirstObjectByType<EffectBuffHud>();
            if(hud)hud.PreviewCount(count);else Debug.LogWarning("Open battle first.");
        }
        [MenuItem("SpinSquad/Diagnostics/Capture Buff Overflow HUD")]
        static void CaptureOverflow()
        {
            if(!EditorApplication.isPlaying)return;
            System.IO.Directory.CreateDirectory("artsrc/ui/buff-overflow-v1");
            ScreenCapture.CaptureScreenshot("artsrc/ui/buff-overflow-v1/unity-play.png");
        }
        static void Show(Rank rank)
        {
            if(!EditorApplication.isPlaying){Debug.LogWarning("Enter Play Mode first. Preview does not modify saves or grant rewards.");return;}
            if(UnityEngine.Object.FindFirstObjectByType<EffectBuffChoiceView>())return;
            EffectBuffChoiceView.Show(EffectBuffCatalog.Draw(rank,new System.Random(11),_=>true),kind=>Debug.Log("[EffectBuffPreview] Selected "+kind+" (no grants)."));
        }
    }
}
#endif
