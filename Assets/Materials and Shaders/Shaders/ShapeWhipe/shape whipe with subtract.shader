Shader "Custom/Color Effects - Subtract Below (grabpass)"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

        _WhipeTex ("Whipe Texture", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(0.0, 1.0)) = 0.5
        _CutoffColor("Cutoff color", Color) = (0, 0, 0, 0)

		_Overlay ("Overlay", Range(0, 1)) = 1

		_Effect ("Effect Val", Range(0, 1)) = 1
		_Color("Add Color", Color) = (1,1,1,1)
	}

	SubShader
	{
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
 
 
		GrabPass
		{
			"_GrabTexture"
		}
 
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
			#include "UnityCG.cginc"
 
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
 
			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
 
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = ComputeGrabScreenPos(o.vertex);
				return o;
			}
 
			sampler2D _GrabTexture;

			sampler2D _MainTex;
			float _Effect;
			float4 _Color;
			float _Overlay;

            float _Cutoff;
            fixed4 _CutoffColor;
            sampler2D _WhipeTex;
 
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 transit = tex2D(_WhipeTex, i.uv);

                if (transit.b < _Cutoff) {
                    return _CutoffColor;
                } 

				fixed4 belowCol = tex2Dproj(_GrabTexture, i.uv);

				float4 color = _Color;//tex2D(_MainTex, i.uv);
				
				belowCol -= float4(1,1,1,0) * _Effect;

				color += belowCol;


                color = lerp(color, _Color, _Overlay);

				//color.a = floor(color.a);
				//color.a -= 1;

				return color;
				/*
                fixed4 transit = tex2D(_WhipeTex, i.uv);

                if (transit.b < _Cutoff) {
                    return _CutoffColor;
                } 

				fixed4 col = tex2Dproj(_GrabTexture, i.uv);

				float4 texColor = tex2D(_MainTex, i.uv);
				col.a = floor(texColor.a);

				//float4 white = (1,1,1,1);
				col -= _Color * _Effect * col.a;
                col = lerp(col, _Color, _Overlay);

                col *= 1 - _Darken;
                col.a = 1;
				
				return col;
				*/
			}
			ENDCG
		}
	}
}