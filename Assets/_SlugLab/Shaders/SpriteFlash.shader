Shader "SlugLab/2D Sprite Flash"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        _FlashAmount ("Flash Amount", Range(0,1)) = 0
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _DissolveScale ("Dissolve Scale", Float) = 14
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "SlugLabSpriteFlash"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _FlashColor;
            float _FlashAmount;
            float _DissolveAmount;
            float _DissolveScale;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.texcoord) * i.color;

                // Ruido barato y estable por píxel para el dissolve.
                float2 cell = floor(i.texcoord * _DissolveScale);
                float noise = frac(sin(dot(cell, float2(12.9898, 78.233))) * 43758.5453);
                clip(noise - _DissolveAmount);

                tex.rgb = lerp(tex.rgb, _FlashColor.rgb, saturate(_FlashAmount));
                return tex;
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
