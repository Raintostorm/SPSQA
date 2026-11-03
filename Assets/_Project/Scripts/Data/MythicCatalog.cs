using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Data
{
    /// <summary>Approved whole-body poses, isolated with audited component masks. Cached once.</summary>
    public static class MythicCatalog
    {
        [Serializable] public class Run { public int x,y,w; }
        [Serializable] public class Pose { public string source; public int[] rect; public float[] pivot; public float ppu; public Run[] runs; }
        [Serializable] public class Entry { public string id,name; public Pose[] frames,empowered; public bool passive; }
        [Serializable] class Manifest { public Entry[] units; }
        [Serializable] class Effects { public Pose[] frames; }
        static readonly List<UnitDefinition> units = new();
        static readonly Dictionary<string,Texture2D> sources = new();
        static readonly Dictionary<string,Color32[]> sourcePixels = new();
        static readonly Dictionary<string,Sprite[]> effects = new();
        static bool prewarming;
        public static IReadOnlyList<UnitDefinition> All { get { Ensure(); return units; } }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() { units.Clear(); sources.Clear(); sourcePixels.Clear(); effects.Clear();prewarming=false; }
        static Texture2D Source(string id)
        {
            if(sources.TryGetValue(id,out var t))return t;
            var bytes=Resources.Load<TextAsset>("Mythics/"+id);
            if(!bytes)throw new InvalidOperationException("Missing Mythic art: "+id);
            t=new Texture2D(2,2,TextureFormat.RGBA32,false);if(!t.LoadImage(bytes.bytes))throw new InvalidOperationException("Cannot decode Mythic image "+id);t.filterMode=FilterMode.Bilinear;
            sources[id]=t;return t;
        }
        public static Sprite Cut(Pose p, string name)
        {
            var src=Source(p.source);var r=p.rect;
            if(r[0]<0||r[1]<0||r[0]+r[2]>src.width||r[1]+r[3]>src.height)throw new InvalidOperationException(name+" invalid rect ["+string.Join(",",r)+"] in "+p.source+" "+src.width+"x"+src.height);
            if(!sourcePixels.TryGetValue(p.source,out var allPixels))sourcePixels[p.source]=allPixels=src.GetPixels32();
            var pixels=new Color32[r[2]*r[3]];
            for(int y=0;y<r[3];y++)Array.Copy(allPixels,(src.height-r[1]-r[3]+y)*src.width+r[0],pixels,y*r[2],r[2]);
            if(p.runs!=null && p.runs.Length>0)
            {
                var mask=new bool[r[2]*r[3]];
                foreach(var run in p.runs)
                    for(int y=Math.Max(0,run.y-1);y<=Math.Min(r[3]-1,run.y+1);y++)
                    for(int x=Math.Max(0,run.x-1);x<Math.Min(r[2],run.x+run.w+1);x++)mask[(r[3]-1-y)*r[2]+x]=true;
                for(int i=0;i<pixels.Length;i++)if(!mask[i])pixels[i]=new Color32(0,0,0,0);
            }
            var tex=new Texture2D(r[2],r[3],TextureFormat.RGBA32,false);tex.name=name;tex.SetPixels32(pixels);tex.Apply(false,true);
            var pivot=p.pivot??new float[]{r[2]*.5f,r[3]*.5f};
            var sprite=Sprite.Create(tex,new Rect(0,0,r[2],r[3]),new Vector2(pivot[0]/r[2],1-pivot[1]/r[3]),p.ppu>0?p.ppu:r[2]);sprite.name=name;return sprite;
        }
        static UnitAnimationSet.Clip Clip(Pose[] poses,int offset,bool loop,string id,float duration)
        {
            if(poses==null || offset+4>poses.Length)return null;
            var c=new UnitAnimationSet.Clip{Frames=new Sprite[4],Durations=new float[4],Loop=loop,UseExplicitPlayback=true};
            for(int i=0;i<4;i++){c.Frames[i]=Cut(poses[offset+i],id+i);c.Durations[i]=duration;}
            return c;
        }
        static void Ensure()
        {
            if(units.Count>0)return;
            var manifest=JsonUtility.FromJson<Manifest>(Resources.Load<TextAsset>("Mythics/catalog").text);
            foreach(var e in manifest.units)
            {
                var d=ScriptableObject.CreateInstance<UnitDefinition>();d.name=d.UnitId="mythic_"+e.id;d.DisplayName=e.name;d.Rank=Rank.Mythic;
                d.Mythic=(MythicKind)Enum.Parse(typeof(MythicKind),e.id,true);
                d.Race=e.id=="elf"?Race.Elf:e.id=="undead"?Race.Dead:e.id=="bear"?Race.Monster:e.id=="robot"?Race.Machine:Race.Human;
                d.BaseHp=e.id=="robot"?20000:14000;d.BaseAttack=e.id=="bear"?2000:1600;d.Scale=.85f;d.MoveSpeed=1.7f;d.AttackCooldown=.7f;d.IsRanged=e.id=="elf"||e.id=="clock";d.AttackRange=d.IsRanged?3.2f:.55f;
                var a=ScriptableObject.CreateInstance<UnitAnimationSet>();a.name=d.UnitId+" animations";a.BakedMotion=true;a.ImpactStep=2;a.SkillImpactStep=2;
                a.Idle=Clip(e.frames,0,true,e.id+" idle",.22f);a.Walk=Clip(e.frames,4,true,e.id+" walk",.14f);a.Attack=Clip(e.frames,8,false,e.id+" attack",.13f);
                a.Skill=e.passive?null:Clip(e.frames,12,false,e.id+" skill",.16f);a.Dead=Clip(e.frames,e.passive?12:16,false,e.id+" dead",.18f);
                a.EmpoweredAttack=Clip(e.empowered,0,false,e.id+" charged",.13f);d.Animations=a;d.Portrait=a.Idle.Frames[0];units.Add(d);
            }
            ReleaseSources();
        }
        public static UnitDefinition Find(MythicKind kind){foreach(var d in All)if(d.Mythic==kind)return d;return null;}
        public static void PrewarmEffects()
        {
            prewarming=true;
            try{foreach(var key in new[]{"time","devour","heal","slash","aura","revive","buff","shield","electric","elf-bolt","clock-bolt","seal","relic"})Effect(key);}
            finally{prewarming=false;ReleaseSources();}
        }
        public static Sprite[] Effect(string key)
        {
            if(effects.TryGetValue(key,out var cached))return cached;
            if(key=="relic")
            {
                var source=Source("capacity-relic");var sprite=Cut(new Pose{source="capacity-relic",rect=new[]{0,0,source.width,source.height},ppu=source.width},key);effects[key]=new[]{sprite};if(!prewarming)ReleaseSources();return effects[key];
            }
            string[] combatKeys={"electric","elf-bolt","clock-bolt","seal"};int row=Array.IndexOf(combatKeys,key);
            if(row>=0)
            {
                var source=Source("combat-effects");int w=source.width/4,h=source.height/4;var result=new Sprite[4];
                for(int i=0;i<4;i++)result[i]=Cut(new Pose{source="combat-effects",rect=new[]{i*w,row*h,w,h},ppu=w},key+i);
                effects[key]=result;if(!prewarming)ReleaseSources();return result;
            }
            string[] names={"time","devour","heal","slash","aura","revive","buff","shield"};int index=Array.IndexOf(names,key);if(index<0)return Array.Empty<Sprite>();
            var frames=new Sprite[4];
            if(index<4)
            {
                var data=JsonUtility.FromJson<Effects>(Resources.Load<TextAsset>("Mythics/vfx-manifest").text);
                for(int i=0;i<4;i++)frames[i]=Cut(data.frames[index*4+i],key+i);
            }
            else
            {
                var src=Source("support-effects");int w=src.width/4,h=src.height/4;
                for(int i=0;i<4;i++)frames[i]=Cut(new Pose{source="support-effects",rect=new[]{i*w,(index-4)*h,w,h},ppu=w,pivot=new[]{w*.5f,h*.85f}},key+i);
            }
            effects[key]=frames;if(!prewarming)ReleaseSources();return frames;
        }
        static void ReleaseSources(){foreach(var t in sources.Values)UnityEngine.Object.Destroy(t);sources.Clear();sourcePixels.Clear();}
    }
}
