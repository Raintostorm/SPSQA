// Sprite thuong, cong mot nuoc son phu de nhay sang luc bi danh.
//
// Vi sao can shader rieng: SpriteRenderer.color la phep NHAN. Mau trang
// (1,1,1,1) nhan voi texture ra dung texture — khong sang len duoc. Muon
// sang phai LERP mau pixel ve phia mau son, va do la viec cua shader.
//
// Dung nguyen bo khung cua Sprites/Default, chi thay ham fragment.
// Vertex van la SpriteVert do UnitySprites.cginc cung cap — dat ten khac
// la shader khong bien dich, va Unity to hong canh sen chu khong bao loi.
Shader "SpinSquad/SpriteFlash"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        _FlashAmount ("Flash Amount", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"             = "Transparent"
            "IgnoreProjector"   = "True"
            "RenderType"        = "Transparent"
            "PreviewType"       = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex   SpriteVert
            #pragma fragment FlashFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            fixed4 _FlashColor;
            float  _FlashAmount;

            fixed4 FlashFrag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

                // Lerp TRUOC khi nhan alpha. Lam nguoc lai la nhan alpha hai
                // lan, nuoc son bi toi di thay vi sang len.
                c.rgb = lerp(c.rgb, _FlashColor.rgb, _FlashAmount);

                c.rgb *= c.a;     // sprite Unity dung premultiplied alpha
                return c;
            }
            ENDCG
        }
    }
}
