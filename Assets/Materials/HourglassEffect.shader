Shader "Custom/URP/HourglassEffect"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _Radius ("Sphere Radius", Range(0.01, 2.0)) = 0.5
        //_AnimationTime ("Animation Time", Float) = 0.0
        _DropAmount ("Sphere Drop Amount", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Radius;
                //float _AnimationTime;
                float _DropAmount;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 rayOriginOS : TEXCOORD1;
                float3 rayDirOS : TEXCOORD2;
                float3 centerWS : TEXCOORD3;
                float2 uv : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionWS = positionWS;
                output.positionCS = TransformWorldToHClip(positionWS);

                float3 cameraPositionOS = TransformWorldToObject(GetCameraPositionWS());
                output.rayOriginOS = cameraPositionOS;
                output.rayDirOS = normalize(input.positionOS.xyz - cameraPositionOS);

                output.centerWS = TransformObjectToWorld(float3(0, 0, 0));

                output.uv = input.uv;

                return output;
            }

            float sdSphere(float3 p, float3 center, float radius)
            {
                return length(p - center) - radius;
            }

            float smin(float a, float b, float k)
            {
                float h = max(k - abs(a - b), 0.0) / k;
                return min(a, b) - h * h * k * 0.25;
            }

            float mapHourglass(float3 p, float time)
            {
                float drop = time * _DropAmount;
                float3 center1 = float3(0, drop, 0);
                float3 center2 = float3(0, -drop, 0);

                float scale1 = 1.0 - time * 0.5;
                float scale2 = 0.5 + time * 0.5;

                float d1 = sdSphere(p, center1, _Radius * scale1);
                float d2 = sdSphere(p, center2, _Radius * scale2);

                return smin(d1, d2, 0.3);
            }

            float3 calcNormal(float3 p, float time)
            {
                float2 e = float2(0.001, 0.0);
                return normalize(float3(
                    mapHourglass(p + e.xyy, time) - mapHourglass(p - e.xyy, time),
                    mapHourglass(p + e.yxy, time) - mapHourglass(p - e.yxy, time),
                    mapHourglass(p + e.yyx, time) - mapHourglass(p - e.yyx, time)
                ));
            }

            bool raymarch(float3 ro, float3 rd, float time, out float t, out float3 normal)
            {
                const int MAX_STEPS = 64;
                const float EPSILON = 0.001;
                t = 0.0;

                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 pos = ro + rd * t;
                    float dist = mapHourglass(pos, time);

                    if (dist < EPSILON)
                    {
                        normal = calcNormal(pos, time);
                        return true;
                    }
                    
                    t += dist;
                    if (t > 10.0)
                        break;
                }
                return false;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float t;
                float3 normalOS;

                float animTime = _Time.y % 1.0;

                if (!raymarch(input.rayOriginOS, input.rayDirOS, animTime, t, normalOS))
                {
                    discard;
                }

                float3 hitPointOS = input.rayOriginOS + input.rayDirOS * t;
                float3 hitPointWS = TransformObjectToWorld(hitPointOS);
                float3 normalWS = TransformObjectToWorldNormal(normalOS);

                float3 viewDir = normalize(GetCameraPositionWS() - hitPointWS);
                float3 lightDir = _MainLightPosition.xyz;
                if (length(lightDir) < 0.01)
                {
                    lightDir = normalize(float3(1, 1, 1));
                }

                float NdotL = saturate(dot(normalWS, lightDir));
                float3 finalColor = _BaseColor.rgb * NdotL;

                return float4(finalColor, _BaseColor.a);
            }
            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}