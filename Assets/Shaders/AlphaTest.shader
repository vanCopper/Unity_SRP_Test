// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Dev/AlphaTest"
{
    Properties{
        _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
    }

    SubShader{

        Tags { "RenderType"="Transparent" "Queue"="AlphaTest" "LightMode"="BasicLightMode" }
        Pass{

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "UnityCG.cginc"

            fixed4 _Diffuse;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;

            struct a2v{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldNormal = normalize(mul(v.normal,  (float3x3)unity_WorldToObject));
                return o;
            };

            fixed4 frag(v2f i):SV_TARGET{
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(i.worldNormal, worldLight));
                fixed3 color = unity_AmbientSky + diffuse;

                fixed4 v_color = tex2D(_MainTex, i.uv);
                clip (v_color.a - _Cutoff);
                fixed4 f_color = fixed4(color, 1.0);
                return v_color * f_color;
            };

            ENDCG

        }        
    }
}