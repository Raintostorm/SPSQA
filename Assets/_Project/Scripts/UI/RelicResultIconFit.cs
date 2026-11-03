using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    // Offline alpha bounds of existing icon files. Fit visible art, not transparent padding.
    static class RelicResultIconFit
    {
        [Serializable] sealed class Bounds { public string Name; public float X,Y,W,H,Width,Height; }
        [Serializable] sealed class Catalog { public Bounds[] Entries; }
        static Dictionary<string,Bounds> entries;
        public static void Apply(Image image)
        {
            if(!image||!image.sprite)return;
            if(entries==null)
            {
                entries=new Dictionary<string,Bounds>();var data=Resources.Load<TextAsset>("RelicResults/icon-bounds");
                if(data){var catalog=JsonUtility.FromJson<Catalog>(data.text);if(catalog?.Entries!=null)foreach(var b in catalog.Entries)entries[b.Name]=b;}
            }
            var rect=image.rectTransform;rect.anchorMin=rect.anchorMax=rect.pivot=Vector2.one*.5f;
            rect.anchoredPosition=Vector2.zero;rect.sizeDelta=new Vector2(260,220);
            if(!entries.TryGetValue(image.sprite.name,out var bounds)||bounds.W<=0||bounds.H<=0)return;
            float scale=Mathf.Min(260/bounds.W,220/bounds.H);
            rect.sizeDelta=new Vector2(bounds.Width*scale,bounds.Height*scale);
            rect.anchoredPosition=new Vector2((bounds.Width/2-bounds.X-bounds.W/2)*scale,(bounds.Y+bounds.H/2-bounds.Height/2)*scale);
        }
    }
}
