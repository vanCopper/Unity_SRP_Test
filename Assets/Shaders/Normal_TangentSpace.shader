Shader "Dev/Normal_TangentSpace"
{
   Properties
   {
       _Color ("Color Tint", Color) = (1, 1, 1, 1)
       _Specular ("Specular", Color) = (1, 1, 1, 1)
       _Gloss ("Gloss", Range(5.0, 256.0)) =  20
       _MainTex("Texture", 2D) = "white" {}
       _BumpMap ("Normal Map", 2D) = "bump" {}
       _BumpScale ("Bump Scale", Float) = 1.0
   }

   SubShader
   {
       Tags { "RenderType"="Opaque" "Queue"="Geometry" "LightMode"="BasicLightMode" }

       Pass
       {
           CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
           
           #include "UnityCG.cginc"
           #include "Lighting.cginc"

           struct a2v
           {
               float4 vertex : POSITION;
               float4 texcoord :  TEXCOORD0;
               float3 normal : NORMAL;
               float4 tangent : TANGENT;
           };

           struct v2f
           {
               float4 pos : SV_POSITION;
               // xy 存储 _MainTex纹理坐标 zw 存储 _BumpMap纹理坐标
               float4 uv : TEXCOORD0;
               float3 lightDir : TEXCOORD1;
               float3 viewDir : TEXCOORD2;
           };

           fixed4 _Color;
           fixed4 _Specular;
           float _Gloss;

           sampler2D _MainTex;
           float4 _MainTex_ST;

           sampler2D _BumpMap;
           float4 _BumpMap_ST;

           float _BumpScale;

           v2f vert(a2v v)
           {
               v2f o;
               o.pos = UnityObjectToClipPos(v.vertex);
               o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
               o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

               TANGENT_SPACE_ROTATION;
               o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
               o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;
               return o;
           }

           fixed4 frag(v2f i) : SV_TARGET
           {
               fixed3 tangentLightDir = normalize(i.lightDir);
               fixed3 tangentViewDir = normalize(i.viewDir);

               fixed4 packedNormal = tex2D(_BumpMap, i.uv.zw);
               fixed3 tangentNormal;
               tangentNormal = UnpackNormal(packedNormal);
			   tangentNormal.xy *= _BumpScale;
			   tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

               fixed3 albedo =  tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;

               fixed3 ambient = unity_AmbientSky * albedo;
               //Diffuse
               fixed3 diffuse = _LightColor0.rgb * albedo * (dot(tangentNormal, tangentLightDir) * 0.5 + 0.5);

               //Specular
               fixed3 hDir = normalize(tangentLightDir + tangentViewDir);
               fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(tangentNormal, hDir)), _Gloss);

              
               return fixed4(ambient + diffuse + specular, 1.0);
           }

           ENDCG
       }
   }

}