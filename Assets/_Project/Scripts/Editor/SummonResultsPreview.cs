#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Project.Meta;
using Project.UI;
using UnityEditor;
using UnityEngine;
namespace Project.EditorTools
{
    public static class SummonResultsPreview
    {
        [MenuItem("SpinSquad/Diagnostics/Preview Summon Results (no grants) %&s")]
        static void Preview()
        {
            if(!Application.isPlaying)return;
            var canvas=Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None).FirstOrDefault(c=>c.isRootCanvas);var catalog=Resources.Load<AllySummon>("Summon/Catalog");
            if(!canvas || !catalog)return;
            string[] ids={"machine_epic","human_rare","elf_rare","human_common","elf_common","dead_common"};int[] amounts={20,20,20,60,40,40};
            var results=new List<AllySummon.Result>();
            for(int i=0;i<ids.Length;i++){var unit=catalog.Units.FirstOrDefault(u=>u && u.UnitId==ids[i]);if(unit)results.Add(new AllySummon.Result{Unit=unit,Shards=amounts[i]});}
            // Render only; never spend, grant or save during a UI preview.
            SummonResultsView.Show(canvas.transform,results,10);
        }
    }
}
#endif
