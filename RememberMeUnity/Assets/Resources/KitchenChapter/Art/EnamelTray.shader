Shader "RememberMe/EnamelTray" {
 Properties { _MainTex ("Surface",2D)="white" {} }
 SubShader { Tags {"Queue"="Transparent" "RenderType"="Transparent"} Cull Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha
 Pass { CGPROGRAM
 #pragma vertex vert_img
 #pragma fragment frag
 #include "UnityCG.cginc"
 float grain(float2 p){return frac(sin(dot(p,float2(35.1,73.4)))*2418.32);}
 float rounded(float2 p,float2 h,float r){float2 q=abs(p)-h+r;return length(max(q,0))+min(max(q.x,q.y),0)-r;}
 fixed4 frag(v2f_img i):SV_Target {
   float2 u=i.uv; float2 p=(u-.5)*float2(2.15,1);
   float d=rounded(p,float2(1.01,.42),.16); float aa=max(fwidth(d),.005);
   float a=1-smoothstep(-aa,aa,d);
   float grit=grain(floor(u*float2(225,115)));
   float3 col=lerp(float3(.16,.26,.30),float3(.42,.55,.58),u.y);
   col+=.18*exp(-7500*pow(d+.041,2))-.13*exp(-2500*pow(d+.085,2))+.025*(grit-.5);
   for(int n=0;n<3;n++){
     float2 q=(p-float2(-.63+n*.62,.035))*float2(1,1.18)/(n==2?.31:.235);
     float rim=length(q)+.025*sin(atan2(q.y,q.x)*9+n);
     col*=1-.32*(1-smoothstep(.92,1.18,length(q+float2(-.05,.17))));
     float3 biscuit=lerp(float3(.52,.29,.12),float3(.88,.68,.40),1-smoothstep(.6,.99,rim));
     biscuit+=.065*(grit-.4)+.05*q.y;
     float2 dots=q*4.6; float k=grain(floor(dots)+n);
     float speck=1-smoothstep(.04,.16,length(frac(dots)-float2(.3+k*.3,.43)));
     biscuit*=1-.43*speck*step(.45,k);
     col=lerp(col,biscuit,1-smoothstep(.95,1.01,rim));
   }
   return fixed4(col,a);
 }
 ENDCG } } }
