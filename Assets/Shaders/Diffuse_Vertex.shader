
Shader "Dev/Diffuse_Vertex" 
{
	Properties
	{
        // Shader properties
		_Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        
	}
	SubShader
	{
        // Shader code
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "LightMode"="BasicLightMode" }
        // Tags { "LightMode"="ForwardBase" }
		Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Lighting.cginc"
            #include "UnityCG.cginc"
           
           // 漫反射属性
            fixed4 _Diffuse;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct a2v {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed3 color : COLOR; 
            };

            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLight));
                
                o.color = unity_AmbientSky +  diffuse;
                
                return o;
            };

            fixed4 frag(v2f i): SV_TARGET{
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 v_col = fixed4(i.color, 1.0);
                return col * v_col;
            };

            ENDCG
		}
	} 
}
