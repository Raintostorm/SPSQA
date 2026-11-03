#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Project.UI;
using Project.Meta;
using Project.Data;
using System.Reflection;

namespace Project.EditorTools
{
    public static class ResultScreenPreview
    {
        [MenuItem("SpinSquad/Diagnostics/Preview Victory (no rewards) %&v")]
        static void Victory()
        {
            if(!Application.isPlaying)return;
            var result=Object.FindFirstObjectByType<ResultPanel>();if(!result)return;
            var balance=typeof(ResultPanel).GetField("balance",BindingFlags.NonPublic|BindingFlags.Instance)?.GetValue(result) as GameBalance;
            int level=GameProgress.CurrentLevel;
            typeof(ResultPanel).GetMethod("BuildResult",BindingFlags.NonPublic|BindingFlags.Instance)?.Invoke(result,new object[]{true,balance?balance.GoldForLevel(level,false):0,balance?balance.KeysForLevel(level,false):0,GameProgress.GemsForLevel(level)});
        }
        // Layout-only preview: never awards rewards or changes saved progress.
        [MenuItem("SpinSquad/Diagnostics/Preview Result (no rewards) %&r")]
        static void Preview()
        {
            if(!Application.isPlaying)return;
            var result=Object.FindFirstObjectByType<ResultPanel>();
            if(!result)return;
            typeof(ResultPanel).GetMethod("BuildResult",BindingFlags.NonPublic|BindingFlags.Instance)?.Invoke(result,new object[]{false,0,0,0});
        }
    }
}
#endif
