Shader "Custom/Water with surface"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

        _SufaceCutoff ("Surface Size", Range(0.0, 1.0)) = 0.99
        _SufaceBrightnessCutoff ("Surface Brightness Cutoff", Range(0, 1.0)) = 0.5
        _SurfaceColor("Surface color", Color) = (0, 0, 0, 0)
		_WaterTopIntencity ("Surface Intencity", Range(0, 1)) = 1
		_FGWaterTopIntencity ("FG Surface Intencity", Range(1, 2)) = 1.33

		_Overlay ("Overlay", Range(0, 1)) = 1

		_Effect ("Effect Val", Range(0, 1)) = 1
		_Color("Add Color", Color) = (1,1,1,1)

		// enhance and dehance
		_BlueEnhance ("Blue Val", Range(0, 2)) = 1
		_RedEnhance ("Red Val", Range(0, 2)) = 1
		_GreenEnhance ("Green Val", Range(0, 2)) = 1

		// warping
		_FrequencyX ("Frequency X", Range(0, 50)) = 0.1
		_AmplitudeX ("Amplitude X", Range(0, 2)) = 0.1
		_SpeedX ("Speed X", Range(0, 2)) = 0.1

		_FrequencyY ("Frequency Y", Range(0, 50)) = 0.1
		_AmplitudeY ("Amplitude Y", Range(0, 2)) = 0.1
		_SpeedY ("SpeedY", Range(0, 2)) = 0.1

		_ExpandSufaceLitPixelSize ("xPixelRelativeSize", Range(0, 0.02)) = 0.01
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

            // make fog work
            #pragma multi_compile_fog

			#include "UnityCG.cginc"
 

			sampler2D _GrabTexture;
			sampler2D _MainTex;
            float4 _MainTex_ST;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 uvtex : TEXCOORD1;
			};
 
			struct v2f
			{
				float4 uv : TEXCOORD0;
				float2 uvtex : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};
 
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = ComputeGrabScreenPos(o.vertex);
                o.uvtex = TRANSFORM_TEX(v.uv, _MainTex);
				return o;

                //UNITY_TRANSFER_FOG(o,o.vertex);
                //return o;
			}
 
            float _SufaceCutoff;
			float _SufaceBrightnessCutoff;
			float _WaterTopIntencity;
			float _FGWaterTopIntencity;
            fixed4 _SurfaceColor;

			float _Effect;
			float4 _Color;
			float _Overlay;

			// enhance
			float _BlueEnhance;
			float _RedEnhance;
			float _GreenEnhance;

			// warping
			half _FrequencyX;
            float _AmplitudeX;
            float _SpeedX;

            half _FrequencyY;
            float _AmplitudeY;
            float _SpeedY;
			float _AddedTime;

			float _ExpandSufaceLitPixelSize;
 
			fixed4 frag (v2f i) : SV_Target
			{
				

				float _myTime = _Time.y + _AddedTime;

                // warp
                i.uv.x += _AmplitudeX * sin((_myTime * _SpeedX + i.uv.y) * _FrequencyX)/20;
                i.uv.y += _AmplitudeY * sin((_myTime * _SpeedY + i.uv.y) * _FrequencyY)/20;

				fixed4 belowCol = tex2Dproj(_GrabTexture, i.uv);
				float belowStrength = belowCol.r + belowCol.g + belowCol.b;

				float4 uvPXOffset = float4(_ExpandSufaceLitPixelSize, 0, 0, 0);

				fixed4 belowColLeft = tex2Dproj(_GrabTexture, i.uv - uvPXOffset);
				float belowStrengthLeft = belowColLeft.r + belowColLeft.g + belowColLeft.b;

				fixed4 belowColRight = tex2Dproj(_GrabTexture, i.uv + uvPXOffset);
				float belowStrengthRight = belowColRight.r + belowColRight.g + belowColRight.b;

                if (i.uvtex.y > _SufaceCutoff) { 
					if (belowStrength > _SufaceBrightnessCutoff || belowStrengthLeft > _SufaceBrightnessCutoff || belowStrengthRight > _SufaceBrightnessCutoff) {
                		//i.uv.x += _AmplitudeX * 3 * sin((_myTime * _SpeedX + i.uv.y) * _FrequencyX)/20;
						//belowCol = tex2Dproj(_GrabTexture, i.uv);
						float4 surfaceAdd = _SurfaceColor * _FGWaterTopIntencity;// * belowStrength * 0.5; // * 0.5;
                    	return belowCol + surfaceAdd;
					}
					// NOTES FOR SURFACE
					// THE lighter the grabpass color is, the lighter the color should be,
					// the darker the grabpass color is, the darker it shold be.
					float4 surfaceAdd = _SurfaceColor * _WaterTopIntencity;// * belowStrength * 0.5; // * 0.5;
                    return belowCol + surfaceAdd;
                } 

				//return _Color;


				float4 color = _Color;//tex2D(_MainTex, i.uv);


                float4 warpedBelowcolor = tex2D(_GrabTexture, i.uv);

                //float4 inverse = (1,1,1,1);
                //float4 newColor = belowCol + (color * (inverse - belowCol));
                //newColor.a = color.a;




				warpedBelowcolor -= float4(1,1,1,0) * _Effect;
				color += warpedBelowcolor;
                color = lerp(color, _Color, _Overlay);

				color.b *= _BlueEnhance; 
				color.r *= _RedEnhance;
				color.g *= _GreenEnhance;

				return color;

			}
			ENDCG
		}
	}
}