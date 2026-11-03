#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
namespace Project.EditorTools
{
    // Editor baseline only. Not a device FPS benchmark. Samples actual running player loop.
    public static class CombatPerformanceSample
    {
        static ProfilerRecorder gc,main;static double end;static readonly List<long> allocations=new(),times=new();static float speed;static bool active;
        [MenuItem("SpinSquad/Diagnostics/Sample combat performance 10s")]
        static void Begin()
        {
            if(!Application.isPlaying||active)return;
            allocations.Clear();times.Clear();speed=Time.timeScale;
            gc=ProfilerRecorder.StartNew(ProfilerCategory.Memory,"GC Allocated In Frame");
            main=ProfilerRecorder.StartNew(ProfilerCategory.Internal,"Main Thread");
            end=EditorApplication.timeSinceStartup+10;active=true;EditorApplication.update+=Tick;
        }
        static void Tick()
        {
            if(!Application.isPlaying||EditorApplication.timeSinceStartup>=end){Finish();return;}
            if(gc.Valid)allocations.Add(gc.LastValue);if(main.Valid)times.Add(main.LastValue);
        }
        static void Finish()
        {
            EditorApplication.update-=Tick;active=false;
            var text="Editor diagnostic sample; timeScale="+speed+"; GC counter valid="+gc.Valid+"; main counter valid="+main.Valid+"\n"+Stats("GC bytes/frame",allocations,1)+"\n"+Stats("main ms/frame",times,1000000)+"\n";
            gc.Dispose();main.Dispose();Directory.CreateDirectory("artsrc/audits/performance");File.AppendAllText("artsrc/audits/performance/editor-baseline.txt",DateTime.Now+"\n"+text+"\n");Debug.Log("[CombatPerformance] "+text);
        }
        static string Stats(string name,List<long> data,double scale)
        {
            if(data.Count==0)return name+": unavailable";data.Sort();double sum=0;foreach(var v in data)sum+=v;
            return name+": n="+data.Count+" mean="+(sum/data.Count/scale).ToString("F2")+" p95="+(data[Math.Min(data.Count-1,(int)(data.Count*.95))]/scale).ToString("F2")+" max="+(data[data.Count-1]/scale).ToString("F2");
        }
    }
}
#endif
