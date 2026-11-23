Shader "RememberMe/WindingMotion"
{
    Properties
    {
        [PerRendererData] _MainTex ("Illustration", 2D) = "white" {}
        _CrankAngle ("Crank angle", Float) = 0
        _CylinderTravel ("Cylinder travel", Float) = 0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _CrankAngle, _CylinderTravel;

            struct Vertex
            {
                float4 position : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varying
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            Varying vert(Vertex input)
            {
                Varying result;
                result.position = UnityObjectToClipPos(input.position);
                result.color = input.color;
                result.uv = input.uv;
                return result;
            }

            // Top-left pixel coordinates correspond to the 1448 × 1086 approved illustration.
            float4 Artwork(float2 pixel)
            {
                return tex2D(_MainTex, float2(pixel.x / 1448, 1 - pixel.y / 1086));
            }

            // Separate the warm brass loop from the slate-blue bench through its colors.
            float CrankMask(float2 pixel, float3 color)
            {
                float boundary = smoothstep(200, 208, pixel.x) * (1 - smoothstep(309, 317, pixel.x))
                    * smoothstep(177, 184, pixel.y) * (1 - smoothstep(348, 353, pixel.y));
                return boundary * smoothstep(.003, .037, color.r - color.b);
            }

            float3 TurnCrank(float2 pixel, float3 original)
            {
                float2 center = float2(262, 266);
                float projectedHeight = cos(_CrankAngle);
                projectedHeight = (projectedHeight < 0 ? -1 : 1) * max(abs(projectedHeight), .10);
                float relativeY = (pixel.y - center.y) / projectedHeight;
                float2 source = float2(pixel.x - sin(_CrankAngle) * relativeY * .08, center.y + relativeY);
                float3 ring = Artwork(source).rgb;
                // Copy the adjacent part of the same bench slat wherever the ring moves aside.
                float3 bench = Artwork(pixel + float2(-108, 2)).rgb;
                float benchPatch = smoothstep(190, 203, pixel.x) * (1 - smoothstep(309, 327, pixel.x))
                    * smoothstep(176, 184, pixel.y) * (1 - smoothstep(348, 355, pixel.y));
                float shaft = smoothstep(303, 312, pixel.x) * (1 - smoothstep(11, 19, abs(pixel.y - center.y)));
                float3 background = lerp(original, bench, benchPatch * (1 - shaft));
                float3 turned = lerp(background, ring, CrankMask(source, ring) * (1 - shaft));
                return lerp(original, turned, smoothstep(.004, .080, abs(sin(_CrankAngle * .5))));
            }

            // Scroll the small surface details while retaining the fixed cylinder contour and light.
            float3 PinDetail(float2 pixel)
            {
                float3 softened = (Artwork(pixel - float2(8, 0)).rgb + Artwork(pixel + float2(8, 0)).rgb) * .5;
                return clamp(Artwork(pixel).rgb - softened, -.035, .065);
            }

            float3 TurnCylinder(float2 pixel, float3 original)
            {
                float x = pixel.x - 655;
                float top = 486 - x * .052;
                float bottom = 540 - x * .070;
                float height = (pixel.y - top) / (bottom - top);
                float mask = smoothstep(0, 14, x) * (1 - smoothstep(280, 295, x))
                    * smoothstep(0, .16, height) * (1 - smoothstep(.84, 1, height));
                float angular = acos(clamp(1 - 2 * height, -1, 1)) / UNITY_PI;
                float shifted = abs(frac((angular - _CylinderTravel) * .5 + .5) * 2 - 1);
                float shiftedY = top + (.5 - .5 * cos(shifted * UNITY_PI)) * (bottom - top);
                float3 details = PinDetail(float2(pixel.x, shiftedY)) - PinDetail(pixel);
                return saturate(original + mask * details * .72);
            }

            float4 frag(Varying input) : SV_Target
            {
                float2 pixel = float2(input.uv.x * 1448, (1 - input.uv.y) * 1086);
                float4 color = tex2D(_MainTex, input.uv);
                if (pixel.x > 186 && pixel.x < 330 && pixel.y > 176 && pixel.y < 355 && abs(sin(_CrankAngle * .5)) > .004)
                    color.rgb = TurnCrank(pixel, color.rgb);
                if (pixel.x > 655 && pixel.x < 950 && pixel.y > 460 && pixel.y < 550)
                    color.rgb = TurnCylinder(pixel, color.rgb);
                return color * input.color;
            }
            ENDCG
        }
    }
}
