Shader "RememberMe/Memory Wash"
{
    Properties
    {
        [PerRendererData] _MainTex ("Room illustration", 2D) = "white" {}
        _Decay ("Memory loss", Range(0, 1)) = 0
        _Path ("Warm path", Range(0, 1)) = 0
        _Room ("Room index", Float) = 0
        _ClockTime ("Clock time", Float) = 0
        _ClockMissing ("Hide minute hand", Float) = 0
        _DoorFlicker ("Fading threshold", Range(0, 1)) = 0
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
            float _Room;
            float _ClockTime;
            float _ClockMissing;
            float _DoorFlicker;
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
            // Painting corrections operate in pixels of the Kitchen and Parlor source illustrations.
            float3 artColor(float2 pixel)
            {
                return tex2D(_MainTex, float2(pixel.x / 1448, 1 - pixel.y / 1086)).rgb;
            }
            float clockHand(float2 p, float2 tip, float width)
            {
                float2 projected = tip * saturate(dot(p, tip) / dot(tip, tip));
                return 1 - smoothstep(width, width + .8, length(p - projected));
            }
            float3 kitchenChanges(float3 color, float2 p)
            {
                float2 face = p - float2(838, 144);
                float distance = length(face);
                if (distance < 19)
                {
                    float faceLight = saturate((face.x - face.y + 17) / 34);
                    float3 paper = lerp(artColor(float2(836, 157)), artColor(float2(842, 151)), faceLight);
                    color = lerp(paper, color, smoothstep(16, 19, distance));
                    float angle = _ClockTime * .22 + 1.04;
                    float2 minute = 14 * float2(sin(angle), -cos(angle));
                    float hourMask = clockHand(face, float2(-8, -6.4), 1);
                    float minuteMask = clockHand(face, minute, .72) * (1 - _ClockMissing);
                    color = lerp(color, artColor(float2(819, 182)), max(hourMask, minuteMask));
                }
                if (_DoorFlicker > .001)
                {
                    float threshold = smoothstep(653, 662, p.x) * (1 - smoothstep(783, 794, p.x));
                    threshold *= smoothstep(841, 864, p.y);
                    color = lerp(color, artColor(float2(610, 955)), threshold * _DoorFlicker);
                }
                return color;
            }
            float3 parlorChanges(float3 color, float2 p)
            {
                float2 table = (p - float2(977, 366)) / float2(43, 24);
                float mask = 1 - smoothstep(.94, 1.05, length(table));
                if (mask <= 0) return color;
                float3 mid = artColor(float2(1019, 367));
                float3 tone = lerp(mid * .78, artColor(float2(980, 340)), saturate(.68 - table.y * .3));
                float grain = sin(p.x * .26 + sin(p.y * .11) + sin(p.x * .37) * .65);
                return lerp(color, tone * (1 + grain * .032), mask);
            }
            fixed4 frag(Pixel input):SV_Target
            {
                fixed4 color = tex2D(_MainTex,input.uv);
                float2 artPixel = float2(input.uv.x, 1 - input.uv.y) * float2(1448, 1086);
                if (_Room < .5) color.rgb = kitchenChanges(color.rgb, artPixel);
                else if (_Room > 1.5 && _Room < 2.5) color.rgb = parlorChanges(color.rgb, artPixel);
                float loss = wash(input.uv);
                if (_Decay > .5 && loss > .16)
                {
                    float2 blur = float2(.0014, .0018) * _Decay;
                    float3 surround = .25 * (tex2D(_MainTex, input.uv + float2(blur.x, 0)).rgb +
                        tex2D(_MainTex, input.uv - float2(blur.x, 0)).rgb +
                        tex2D(_MainTex, input.uv + float2(0, blur.y)).rgb +
                        tex2D(_MainTex, input.uv - float2(0, blur.y)).rgb);
                    color.rgb = lerp(color.rgb, surround, loss * .25);
                }
                float gray = dot(color.rgb,float3(.2126,.7152,.0722));
                float3 cold = lerp(float3(.046,.055,.075),gray * float3(.54,.64,.77),.72);
                color.rgb = lerp(color.rgb,cold,loss);
                return color;
            }
            ENDCG
        }
    }
}
