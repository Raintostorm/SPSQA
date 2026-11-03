using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    /// <summary>Decorative Home-only layers; no gameplay, input, or save state.</summary>
    public sealed class HomeMarchingBackground : MonoBehaviour
    {
        sealed class Layer
        {
            public RawImage[] Tiles = new RawImage[3];
            public float Top, Height, Width, Speed;
        }
        readonly System.Collections.Generic.List<Layer> layers = new System.Collections.Generic.List<Layer>();
        RectTransform rect;
        float distance;
        public bool Moving { get; set; } = true;

        public static HomeMarchingBackground Install(Transform parent)
        {
            var sky=Resources.Load<Texture2D>("HomeWalk/sky");
            var trees=Resources.Load<Texture2D>("HomeWalk/trees");
            var ground=Resources.Load<Texture2D>("HomeWalk/ground");
            if(!sky || !trees || !ground)return null; // Preserve old background on missing imports.
            var go=new GameObject("HomeMarchingBackground",typeof(RectTransform),typeof(RectMask2D));
            go.transform.SetParent(parent,false);go.transform.SetAsFirstSibling();
            var fx=go.AddComponent<HomeMarchingBackground>();
            fx.rect=go.GetComponent<RectTransform>();fx.rect.anchorMin=Vector2.zero;fx.rect.anchorMax=Vector2.one;fx.rect.offsetMin=fx.rect.offsetMax=Vector2.zero;
            var fill=new GameObject("SkyFill",typeof(RectTransform),typeof(Image)).GetComponent<Image>();fill.transform.SetParent(go.transform,false);
            var fillRect=fill.rectTransform;fillRect.anchorMin=Vector2.zero;fillRect.anchorMax=Vector2.one;fillRect.offsetMin=fillRect.offsetMax=Vector2.zero;
            fill.color=new Color(0,.68f,.95f);fill.raycastTarget=false;
            fx.Add(sky,0,530f/866,1590f/400,1.2f/400);
            var grass=new GameObject("GroundFill",typeof(RectTransform),typeof(Image)).GetComponent<Image>();grass.transform.SetParent(go.transform,false);
            grass.rectTransform.anchorMin=Vector2.zero;grass.rectTransform.anchorMax=new Vector2(1,1-490f/866);grass.rectTransform.offsetMin=grass.rectTransform.offsetMax=Vector2.zero;
            grass.color=new Color(.46f,.77f,.16f);grass.raycastTarget=false;
            fx.Add(ground,490f/866,310f/866,925f/400,22f/400);
            fx.Add(trees,365f/866,230f/866,690f/400,5.5f/400);
            var previous=parent.Find("Background");if(previous)previous.gameObject.SetActive(false);
            fx.Draw();return fx;
        }
        void Add(Texture2D texture,float top,float height,float width,float speed)
        {
            var layer=new Layer{Top=top,Height=height,Width=width,Speed=speed};
            for(int i=0;i<3;i++)
            {
                var image=new GameObject(texture.name+"Tile"+i,typeof(RectTransform),typeof(RawImage)).GetComponent<RawImage>();
                image.transform.SetParent(transform,false);image.texture=texture;image.raycastTarget=false;
                var r=image.rectTransform;r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);
                layer.Tiles[i]=image;
            }
            layers.Add(layer);
        }
        void Update()
        {
            if(Moving && Application.isFocused)distance+=Mathf.Min(Time.unscaledDeltaTime,.05f);
            Draw();
        }
        void Draw()
        {
            if(!rect)return;
            float width=rect.rect.width,height=rect.rect.height;
            foreach(var layer in layers)
            {
                float tileWidth=width*layer.Width, travel=distance*width*layer.Speed;
                if(tileWidth<=0)continue;
                int first=Mathf.FloorToInt(travel/tileWidth);
                for(int i=0;i<3;i++)
                {
                    int tile=first+i;
                    var image=layer.Tiles[i];image.uvRect=tile%2==0?new Rect(0,0,1,1):new Rect(1,0,-1,1);
                    image.rectTransform.anchoredPosition=new Vector2(tile*tileWidth-travel,-height*layer.Top);
                    image.rectTransform.sizeDelta=new Vector2(tileWidth+1,height*layer.Height);
                }
            }
        }
    }
}
