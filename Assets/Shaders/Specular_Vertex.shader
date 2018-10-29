// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dev/Specular_Vertex"
{

    Properties
    {
        _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
        _Specular ("Specular", Color) = (1, 1, 1, 1)
        _Gloss ("Gloss", Range(5.0, 256.0)) = 20
        _MainTex("Texture", 2D) =  "white" {}
    }

    SubShader
    {

        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "LightMode" = "BasicLightMode" }
        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 color : COLOR;
            };

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                //Diffuse
                fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                // fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLight));
                fixed3 diffuse = _LightColor0.rgb * _Diffuse * (dot(worldNormal, worldLight)*0.5 + 0.5);
                //Specular
                fixed3 reflectDir = normalize(reflect(-worldLight, worldNormal));
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(reflectDir, viewDir)), _Gloss);

                o.color = unity_AmbientSky + diffuse + specular;
                return o;
            }

            fixed4 frag(v2f i): SV_TARGET{
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 v_col = fixed4(i.color, 1.0);
                return col * v_col;
            }
            ENDCG
        }
    }
}