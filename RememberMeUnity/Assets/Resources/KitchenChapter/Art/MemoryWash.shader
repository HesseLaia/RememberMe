Shader "RememberMe/Memory Wash"
{
    Properties
    {
        [PerRendererData] _MainTex ("Room illustration", 2D) = "white" {}
        _Decay ("Memory loss", Range(0, 1)) = 0
        _Path ("Warm path", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="False" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float _Decay;
            float _Path;
            struct Vertex { float4 pos:POSITION; float2 uv:TEXCOORD0; };
            struct Pixel { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };
            Pixel vert(Vertex input)
            {
                Pixel output;
                output.pos = UnityObjectToClipPos(input.pos);
                output.uv = input.uv;
                return output;
            }
            float wash(float2 uv)
            {
                float grain = frac(sin(dot(floor(uv * float2(365,277)), float2(12.9898,78.233))) * 43758.5453);
                float edge = max(abs(uv.x - .5) * 2, abs(uv.y - .5) * 1.36);
                float ripples = .055 * sin(uv.y * 91 + uv.x * 31) + .045 * sin(uv.x * 151 - uv.y * 33);
                float loss = smoothstep(.85 - _Decay * .66, 1.03 - _Decay * .54, edge + ripples + (grain - .5) * .17);
                float central = 1 - smoothstep(.035,.13,abs(uv.x - .5));
                return saturate(loss * _Decay + _Decay * .19) * (1 - central * _Path * .82);
            }
            fixed4 frag(Pixel input):SV_Target
            {
                fixed4 color = tex2D(_MainTex,input.uv);
                float loss = wash(input.uv);
                float gray = dot(color.rgb,float3(.2126,.7152,.0722));
                float3 cold = lerp(float3(.046,.055,.075),gray * float3(.54,.64,.77),.72);
                color.rgb = lerp(color.rgb,cold,loss);
                return color;
            }
            ENDCG
        }
    }
}
