using System.Collections.Generic;
using System.Globalization;
using Project.Data;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>Legible combat numbers, cached font/material and a bounded reusable pool.</summary>
    public sealed class DamagePopup : MonoBehaviour
    {
        const float WorldHeight=.30f, RiseDistance=.48f, LifeSeconds=.9f;
        const int FontSize=96, SortOrder=5000, MaxPopups=64;
        static readonly Color AllyHurt=new Color(1f,.52f,.46f);
        static readonly Color EnemyHurt=new Color(1f,.96f,.72f);
        static readonly Color SkillDamage=new Color(.80f,.48f,1f);
        static readonly Vector2[] StrokeOffsets={new Vector2(-1,-1),new Vector2(-1,1),new Vector2(1,-1),new Vector2(1,1)};
        static readonly List<DamagePopup> pool=new List<DamagePopup>();
        static Font damageFont;static int lane;
        TextMesh text;readonly TextMesh[] stroke=new TextMesh[4];
        Vector3 from;float elapsed;bool playing,requiresNumbers;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetPool(){pool.Clear();damageFont=null;lane=0;Font.textureRebuilt-=FontRebuilt;Font.textureRebuilt+=FontRebuilt;}

        // UI Text now shares this font (UiFonts). When UI requests new glyphs Unity rebuilds the
        // font texture, and live TextMesh UVs point at the old layout — the numbers vanish. Re-request.
        static void FontRebuilt(Font font)
        {
            if(font!=damageFont)return;
            foreach(var popup in pool)if(popup&&popup.playing)popup.Remesh();
        }
        void Remesh()
        {
            var value=text.text;text.text="";text.text=value;
            foreach(var outline in stroke){outline.text="";outline.text=value;}
        }

        public static void Show(Vector3 worldPos,float amount,UnitTeam victim,bool skill=false)
        {
            ShowAmount(worldPos,amount,skill?SkillDamage:(victim==UnitTeam.Ally?AllyHurt:EnemyHurt),"","",true);
        }
        public static void ShowHealing(Vector3 worldPos,float amount)=>ShowAmount(worldPos,amount,new Color(.25f,1f,.4f),"+","",true);
        public static void ShowHealingFull(Vector3 worldPos,float attempted)=>ShowAmount(worldPos,attempted,new Color(.25f,1f,.4f),"+","",false);
        public static void ShowCoins(Vector3 worldPos,int coins)=>ShowAmount(worldPos,coins,new Color(1f,.84f,.15f),"+"," coin",false);
        static void ShowAmount(Vector3 worldPos,float amount,Color color,string prefix,string suffix,bool numbersOnly)
        {
            if(amount<=0||numbersOnly&&!Project.UI.SharedSettings.DamageNumbersEnabled)return;
            DamagePopup popup=null;
            for(int i=pool.Count-1;i>=0;i--)
            {
                if(!pool[i]){pool.RemoveAt(i);continue;}
                if(!pool[i].playing)popup=pool[i];
            }
            if(!popup&&pool.Count<MaxPopups)
            {
                var go=new GameObject("DamagePopup");popup=go.AddComponent<DamagePopup>();popup.Build();pool.Add(popup);
            }
            if(!popup)
            {
                // Recycle the oldest visual under a large AoE burst; never allocate unboundedly.
                popup=pool[0];foreach(var candidate in pool)if(candidate.elapsed>popup.elapsed)popup=candidate;
            }
            popup.requiresNumbers=numbersOnly;popup.Play(worldPos,amount,color,prefix,suffix);
        }
        void Build()
        {
            if(!damageFont)damageFont=Resources.Load<Font>("Fonts/LilitaOne-Regular")??Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            for(int i=0;i<stroke.Length;i++)
            {
                var go=new GameObject("Outline");go.transform.SetParent(transform,false);
                go.transform.localPosition=new Vector3(StrokeOffsets[i].x*.012f,StrokeOffsets[i].y*.012f,0);
                stroke[i]=Make(go,SortOrder-1);
            }
            text=Make(gameObject,SortOrder);
        }
        TextMesh Make(GameObject go,int sorting)
        {
            var tm=go.AddComponent<TextMesh>();tm.font=damageFont;tm.fontSize=FontSize;
            tm.characterSize=WorldHeight*10f/FontSize;tm.fontStyle=FontStyle.Normal;
            tm.anchor=TextAnchor.LowerCenter;tm.alignment=TextAlignment.Center;
            var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=damageFont.material;renderer.sortingOrder=sorting;
            return tm;
        }
        void Play(Vector3 position,float amount,Color color,string prefix,string suffix)
        {
            playing=true;elapsed=0;
            int slot=lane++%5;
            from=position+new Vector3((slot-2)*.065f,.10f+(slot%2)*.07f,0);
            transform.position=from;transform.localScale=Vector3.one;
            string value=System.Math.Max(1d,System.Math.Round(amount,System.MidpointRounding.AwayFromZero)).ToString("N0",CultureInfo.InvariantCulture);
            value=prefix+value+suffix;text.text=value;text.color=color;
            foreach(var outline in stroke){outline.text=value;outline.color=new Color(.025f,.045f,.07f,1);}
            gameObject.SetActive(true);
        }
        void Update()
        {
            if(requiresNumbers&&!Project.UI.SharedSettings.DamageNumbersEnabled){Hide();return;}
            if(Project.UI.SharedSettings.IsPaused||Time.timeScale<=0)return;
            // Presentation uses real seconds, so x2/x3 combat still leaves numbers readable.
            elapsed+=Time.unscaledDeltaTime;float t=elapsed/LifeSeconds;
            if(t>=1){Hide();return;}
            float ease=1-(1-t)*(1-t);transform.position=from+Vector3.up*(RiseDistance*ease);
            float pop=1+.14f*Mathf.Max(0,1-t/.18f);transform.localScale=Vector3.one*pop;
            float alpha=t<.7f?1:Mathf.Clamp01((1-t)/.3f);
            var color=text.color;color.a=alpha;text.color=color;
            foreach(var outline in stroke){var c=outline.color;c.a=alpha;outline.color=c;}
        }
        void Hide(){playing=false;gameObject.SetActive(false);}
        void OnDestroy(){pool.Remove(this);}
    }
}
