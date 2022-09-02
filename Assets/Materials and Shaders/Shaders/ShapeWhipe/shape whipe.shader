// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/shape whipe"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _WhipeTex ("Whipe Texture", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(0.0, 1.0)) = 0.5
        _Color("Example color", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
                
                /*
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv1 = v.uv;

                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
                    o.uv1.y = 1 - 0.uv1.y;
                #endif
                */
            }

            float _Cutoff;
            fixed4 _Color;
            sampler2D _WhipeTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 transit = tex2D(_WhipeTex, i.uv);

                if (transit.b < _Cutoff) {
                    return _Color;
                } 

                // sample the texture
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
