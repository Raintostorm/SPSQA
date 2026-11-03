#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Project.Data;
using Project.Meta;
using Project.UI;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools
{
    public static class RelicResultsPreview
    {
        [MenuItem("SpinSquad/Diagnostics/Open Relics for UI review %&t")]
        static void OpenRelics(){if(Application.isPlaying)SceneRoutes.GoTreasure();}

        [MenuItem("SpinSquad/Diagnostics/Preview Relic Results (no grants) %&o")]
        static void Standard()=>Preview(false);
        [MenuItem("SpinSquad/Diagnostics/Preview Long Relic Results (no grants) %&l")]
        static void Long()=>Preview(true);
        static void Preview(bool many)
        {
            if(!Application.isPlaying)return;
            var screen=UnityEngine.Object.FindFirstObjectByType<TreasureScreen>();if(!screen)return;
            const BindingFlags flags=BindingFlags.NonPublic|BindingFlags.Instance;
            var db=typeof(TreasureScreen).GetField("database",flags)?.GetValue(screen) as TreasureDatabase;
            if(!db||db.Items==null)return;
            var results=new List<TreasureRoll.Result>();int[] counts={8,5,3,2,2,2,1,1,1};
            for(int i=0;i<Math.Min(db.Items.Length,many?db.Items.Length:9);i++)
                for(int j=0;j<(many?1:counts[i]);j++)
                    results.Add(new TreasureRoll.Result{Item=db.Items[i],ShardsGranted=many&&i==0?0:1,IsNew=(i==6||i==8)&&j==0});
            // Fixtures go directly to the renderer. No Roll, SpendKeys, Save or grant calls.
            typeof(TreasureScreen).GetMethod("ShowRelicResults",flags)?.Invoke(screen,new object[]{results,results.Count,(Action)null});
        }
    }
}
#endif
