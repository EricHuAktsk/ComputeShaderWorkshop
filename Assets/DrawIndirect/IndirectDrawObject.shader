Shader "MyShader/IndirectDrawObject"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" { }
    }
    SubShader
    {

        Pass
        {

            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma target 4.5

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct MeshData
            {
                float3 Position;
            };

            StructuredBuffer<MeshData> meshDataBuffer;
            float _WorldSize;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv_MainTex : TEXCOORD0;
                float3 color : TEXCOORD1;
            };

            void rotate2D(inout float2 v, float r)
            {
                float s, c;
                sincos(r, s, c);
                v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
            }

            void Unity_RotateAboutAxis_Radians_float(float3 In, float3 Axis, float Rotation, out float3 Out)
            {
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;

                Axis = normalize(Axis);
                float3x3 rot_mat = {
                    one_minus_c * Axis.x * Axis.x + c, one_minus_c * Axis.x * Axis.y - Axis.z * s, one_minus_c * Axis.z * Axis.x + Axis.y * s,
                    one_minus_c * Axis.x * Axis.y + Axis.z * s, one_minus_c * Axis.y * Axis.y + c, one_minus_c * Axis.y * Axis.z - Axis.x * s,
                    one_minus_c * Axis.z * Axis.x - Axis.y * s, one_minus_c * Axis.y * Axis.z + Axis.x * s, one_minus_c * Axis.z * Axis.z + c
                };
                Out = mul(rot_mat, In);
            }

            float random(in float2 st)
            {
                return frac(sin(dot(st.xy,
                float2(12.9898, 78.233)))
                * 43758.5453123);
            }

            v2f vert(appdata_full v, uint instanceID : SV_InstanceID)
            {
                float3 data = meshDataBuffer[instanceID].Position;

                float3 localPosition = v.vertex.xyz;
                Unity_RotateAboutAxis_Radians_float(localPosition, float3(random(data.xy), random(data.xz), random(data.xz)), _Time.x * 15, localPosition);
                float3 worldPosition = data.xyz + localPosition;
                float3 worldNormal = v.normal;

                float3 ndotl = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz)) + 0.5 * 0.5;
                float3 color = ndotl * float3(saturate(abs(worldPosition.x / _WorldSize)), saturate(abs(worldPosition.y / _WorldSize)), saturate(abs(worldPosition.z / _WorldSize)));

                v2f o;
                o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                o.uv_MainTex = v.texcoord;
                o.color = color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //fixed4 albedo = tex2D(_MainTex, i.uv_MainTex);
                //float3 lighting = i.diffuse * shadow + i.ambient;
                return fixed4(i.color, 1);
            }

            ENDCG

        }
    }
}