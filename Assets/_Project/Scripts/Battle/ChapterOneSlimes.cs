using System;
using System.Collections.Generic;
using System.Linq;
using Project.Data;
using UnityEngine;

namespace Project.Battle
{
    // Enemy-only runtime definitions. Never mutate ally definitions, saves or roster.
    public sealed class ChapterOneSlimes : IDisposable
    {
        [Serializable] sealed class Layout { public Pose[] frames; }
        [Serializable] sealed class Pose { public float x,y,w,h,pivotX,pivotY,bodyHeight; }
        readonly List<UnityEngine.Object> owned = new();
        readonly UnitDefinition[] common = new UnitDefinition[4];
        UnitDefinition elite, boss;
        public bool Ready { get; private set; }
        public UnitDefinition[] Entries => new[]{common[0],common[2],common[1],common[3],elite,boss};

        public ChapterOneSlimes(UnitDefinition[] templates)
        {
            var ranged = templates.FirstOrDefault(d => d && d.IsRanged);
            var melee = templates.Where(d => d && !d.IsRanged).ToArray();
            if (!ranged || melee.Length == 0) return;
            try
            {
                common[0] = Build("amber", "Amber Slime", melee[0]);
                common[1] = Build("green", "Green Slime", ranged);
                common[2] = Build("stone", "Stone Slime", melee[melee.Length-1]);
                common[3] = Build("violet", "Violet Slime", ranged);
                elite = Build("elite", "Bramble Slime", melee[0]);
                boss = Build("boss", "Slime Overlord", melee[melee.Length-1]);
                elite.EnemyTier = UnitDefinition.EnemyClass.Elite;
                boss.EnemyTier = UnitDefinition.EnemyClass.Boss;
                elite.BaseHp *= 1.75f; elite.BaseAttack *= 1.2f; elite.Scale *= 1.15f;
                boss.BaseHp *= 4f; boss.BaseAttack *= 1.5f; boss.Scale *= 1.5f;
                Ready = true;
            }
            catch (Exception ex) { Debug.LogWarning("[ChapterOneSlimes] Keeping original enemies: " + ex.Message); }
        }

        public UnitDefinition Pick(int slot, int wave, int finalWave, bool allowSpecial)
        {
            if (!Ready) return null;
            if (allowSpecial && slot == 0 && wave == finalWave) return boss;
            if (allowSpecial && slot == 0 && wave % 3 == 0) return elite;
            return common[(slot + Mathf.Max(0,wave-1)) % common.Length];
        }

        UnitDefinition Build(string id, string display, UnitDefinition template)
        {
            var def = UnityEngine.Object.Instantiate(template); owned.Add(def);
            def.name = def.UnitId = "enemy_slime_" + id; def.DisplayName = display;
            def.Race = Race.Monster; def.Rank = Rank.Common;
            var anim = ScriptableObject.CreateInstance<UnitAnimationSet>(); owned.Add(anim);
            anim.name = "Anim_" + def.UnitId; anim.BakedMotion = true; anim.SourceFacesLeft = true;
            bool green = id == "green";
            float pixelsPerUnit = 0;
            anim.Idle = Clip(id,"idle",green,ref pixelsPerUnit);
            anim.Walk = Clip(id,"walk",green,ref pixelsPerUnit);
            anim.Attack = Clip(id,"attack",green,ref pixelsPerUnit);
            anim.Dead = Clip(id,"dead",green,ref pixelsPerUnit);
            anim.ImpactStep = id == "violet" ? 2 : 3;
            if (id == "boss")
            {
                anim.Skill = anim.Attack;
                anim.SkillImpactStep = anim.ImpactStep;
            }
            def.Animations = anim; def.Portrait = anim.Idle.Frames[0];
            def.Scale = .65f; def.CellOffset = new Vector2(0,-.3f);
            if(def.IsRanged)
            {
                var sheet=Resources.Load<Texture2D>("ChapterOneSlimes/green_attack");
                var glob=Sprite.Create(sheet,new Rect(45,175,125,90),Vector2.one*.5f,450,0,SpriteMeshType.FullRect);
                owned.Add(glob);def.ProjectileSpriteOverride=glob;
                def.ProjectileTint=id=="violet"?new Color(.7f,.35f,1f):Color.white;
            }
            return def;
        }

        UnitAnimationSet.Clip Clip(string id, string state, bool green, ref float ppu)
        {
            bool bite = !green && id != "violet" && state == "attack";
            string path = "ChapterOneSlimes/" + id + (green ? "_" + state : bite ? "_bite" : "");
            var texture = Resources.Load<Texture2D>(path);
            if (!texture || !texture.isReadable) throw new InvalidOperationException(path + " missing or not readable");
            var data = Resources.Load<TextAsset>(path+"_layout");
            if(!data)throw new InvalidOperationException(path+" missing layout");
            var layout=JsonUtility.FromJson<Layout>(data.text);
            int count = green || bite ? 6 : 4;
            int row = state == "idle" ? 0 : state == "walk" ? 1 : state == "attack" ? 2 : 3;
            var frames = new Sprite[count];
            for (int i=0;i<count;i++)
            {
                var pose=layout.frames[green||bite?i:row*4+i];
                float cellHeight=(float)texture.height/(green||bite?2:4);
                if(ppu<=0) ppu=pose.bodyHeight/.72f/cellHeight;
                var sprite=Sprite.Create(texture,new Rect(pose.x,pose.y,pose.w,pose.h),new Vector2(pose.pivotX,pose.pivotY),ppu*cellHeight,0,SpriteMeshType.FullRect);
                sprite.name=id+"_"+state+"_"+i;owned.Add(sprite);frames[i]=sprite;
            }
            float[] times = bite ? new[]{.20f,.20f,.12f,.08f,.10f,.15f} : green
                ? state=="attack" ? new[]{.24f,.30f,.25f,.10f,.16f,.30f} : new[]{.16f,.16f,.18f,.16f,.16f,.18f}
                : state=="attack" ? new[]{.24f,.25f,.10f,.26f} : new[]{.20f,.18f,.18f,.20f};
            return new UnitAnimationSet.Clip {Frames=frames,Durations=times,Loop=state=="idle"||state=="walk",UseExplicitPlayback=true};
        }
        public void Dispose()
        {
            foreach(var o in owned) if(o)
            {
                if(Application.isPlaying)UnityEngine.Object.Destroy(o);
                else UnityEngine.Object.DestroyImmediate(o);
            }
            owned.Clear();
        }
    }
}
