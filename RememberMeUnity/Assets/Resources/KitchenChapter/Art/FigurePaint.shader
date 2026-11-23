Shader "RememberMe/Figure Paint" {
Properties { _MainTex ("Paint",2D)="white" {} }
SubShader {
Tags {"Queue"="Transparent" "RenderType"="Transparent"}
Cull Back ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
sampler2D _MainTex;
struct Output {float4 pos:SV_POSITION; float2 uv:TEXCOORD0; float3 normal:TEXCOORD1;};
Output vert(appdata_base v) {
    Output o; o.pos=UnityObjectToClipPos(v.vertex); o.uv=v.texcoord;
    o.normal=UnityObjectToWorldNormal(v.normal); return o;
}
fixed4 frag(Output i):SV_Target {
    fixed4 c=tex2D(_MainTex,i.uv);
    float lamp=dot(normalize(i.normal),normalize(float3(-.58,.48,-.70)));
    c.rgb*=.90+.15*saturate(lamp); return c;
}
ENDCG
}
}}
