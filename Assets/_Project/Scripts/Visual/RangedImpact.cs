using System.Collections.Generic;
using Project.Battle;
using UnityEngine;
namespace Project.Visual
{
    public enum RangedImpactKind{None,Arrow,Machine,Magic,Spirit,Web,Shield,Elf,Clock,GreenSlime,VioletSlime,Slash,Pierce,Smash,Saw,DarkSlash,Claw,Sting,Heavy,Fire,Bite,Royal,Leaf,Skeleton}
    // Small non-looping contact art; no gameplay callbacks. Shared cached sprites and bounded pool.
    public sealed class RangedImpact:MonoBehaviour
    {
        static readonly Dictionary<RangedImpactKind,Sprite[]> clips=new();static readonly List<RangedImpact> pool=new();
        SpriteRenderer art;Sprite[] frames;BattleFlow flow;float age,duration;bool playing;
        const int Limit=64;const float Duration=.24f;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset(){clips.Clear();pool.Clear();}
        public static void Prewarm()
        {
            if(clips.Count>0)return;
            Load("RangedImpacts/arrow-shield",2,new[]{RangedImpactKind.Arrow,RangedImpactKind.Shield});
            Load("RangedImpacts/families",4,new[]{RangedImpactKind.Machine,RangedImpactKind.Magic,RangedImpactKind.Spirit,RangedImpactKind.Web});
            Load("RangedImpacts/remaining",4,new[]{RangedImpactKind.Elf,RangedImpactKind.Clock,RangedImpactKind.GreenSlime,RangedImpactKind.VioletSlime});
            Load("RangedImpacts/melee",13,new[]{RangedImpactKind.Slash,RangedImpactKind.Pierce,RangedImpactKind.Smash,RangedImpactKind.Saw,RangedImpactKind.DarkSlash,RangedImpactKind.Claw,RangedImpactKind.Sting,RangedImpactKind.Heavy,RangedImpactKind.Fire,RangedImpactKind.Bite,RangedImpactKind.Royal,RangedImpactKind.Leaf,RangedImpactKind.Skeleton});
        }
        static void Load(string path,int rows,RangedImpactKind[] kinds)
        {
            var file=Resources.Load<TextAsset>(path);if(!file)return;
            var tex=new Texture2D(2,2,TextureFormat.RGBA32,false);tex.LoadImage(file.bytes);tex.filterMode=FilterMode.Bilinear;tex.wrapMode=TextureWrapMode.Clamp;
            float w=tex.width/4f,h=tex.height/(float)rows;
            for(int row=0;row<rows;row++){var clip=new Sprite[4];for(int col=0;col<4;col++)clip[col]=Sprite.Create(tex,new Rect(col*w,(rows-1-row)*h,w,h),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);clips[kinds[row]]=clip;}
        }
        public static void Play(RangedImpactKind kind,Vector3 point,float width,int sortingLayer,int sortingOrder,BattleFlow battle,float seconds=Duration,bool mirror=false)
        {
            if(kind==RangedImpactKind.None||!Project.UI.SharedSettings.EffectsEnabled)return;
            Prewarm();if(!clips.TryGetValue(kind,out var clip))return;
            RangedImpact effect=null;
            for(int i=pool.Count-1;i>=0;i--){if(!pool[i]){pool.RemoveAt(i);continue;}if(!pool[i].playing)effect=pool[i];}
            if(!effect&&pool.Count<Limit){effect=new GameObject("Ranged impact").AddComponent<RangedImpact>();effect.art=effect.gameObject.AddComponent<SpriteRenderer>();pool.Add(effect);}
            if(!effect){effect=pool[0];foreach(var item in pool)if(item.age>effect.age)effect=item;}
            effect.frames=clip;effect.age=0;effect.duration=Mathf.Max(.05f,seconds);effect.playing=true;effect.flow=battle;effect.transform.position=point;
            effect.transform.localScale=Vector3.one*Mathf.Clamp(width,.25f,seconds>.3f?2.6f:1.1f)/clip[0].bounds.size.x;
            if(mirror){var scale=effect.transform.localScale;scale.x=-scale.x;effect.transform.localScale=scale;}
            effect.art.sortingLayerID=sortingLayer;effect.art.sortingOrder=sortingOrder;effect.art.sprite=clip[0];effect.art.color=Color.white;effect.gameObject.SetActive(true);
        }
        void Update()
        {
            if(!playing)return;
            if(!Project.UI.SharedSettings.EffectsEnabled||flow&&flow.Current!=BattleFlow.Phase.Marching&&flow.Current!=BattleFlow.Phase.Fighting){Hide();return;}
            age+=Time.deltaTime;if(age>=duration){Hide();return;}
            float progress=age/duration;
            int frame=duration>.3f?(progress<.12f?0:progress<.52f?1:progress<.8f?2:3):Mathf.Min(3,(int)(progress*4));
            art.sprite=frames[frame];
            if(duration>.3f)art.color=new Color(1,1,1,progress>.8f?(1-progress)/.2f:1);
        }
        void Hide(){playing=false;gameObject.SetActive(false);}
        void OnDestroy(){pool.Remove(this);}
    }
}
