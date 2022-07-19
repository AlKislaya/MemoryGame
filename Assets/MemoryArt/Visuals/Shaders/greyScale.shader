Shader "My/GrayShader" 
{
    Properties {
         [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
         _Alpha("Alpha", Range(0,1)) = 1
     }
 
     SubShader {
         Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
        LOD 110

         Pass {
         CGPROGRAM
#pragma vertex vert  
#pragma fragment frag
# include "UnityCG.cginc"

         sampler2D _MainTex;
         float _Alpha;

struct v2f
{
    float4 pos : SV_POSITION;
             fixed4 color: COLOR;
             float4 texcoord : TEXCOORD0;
         };

v2f vert(appdata_full v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.color = v.color;
    o.texcoord = v.texcoord;
    return o;
}

fixed4 frag(v2f i) : COLOR {
                fixed4 c = tex2D(_MainTex, i.texcoord);
                c.rgb = (c.r + c.g + c.b) / 6 + 0.5f;//3;
                c.a = _Alpha * c.a;

                return c;
         }
         ENDCG
         }
     }
 }