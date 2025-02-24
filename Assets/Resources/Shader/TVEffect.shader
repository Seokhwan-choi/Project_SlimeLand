Shader "Custom/TVEffect" 
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
            LOD 100

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;

                float random(float2 st)
                {
                    return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
                }

                float simplexNoise(float2 st)
                {
                    const float3 p = float3(0.06711056, 0.00583715, 52.9829189);
                    float3 i = floor(float3(st.xy, st.x * st.y) + dot(float3(st.xy, st.x * st.y), p.xyz));
                    float3 f = frac(float3(st.xy, st.x * st.y) + dot(float3(st.xy, st.x * st.y), p.xyz));
                    float3 u = f * f * (3.0 - 2.0 * f);

                    return lerp(lerp(lerp(random(i.xy), random(i.zy), u.x),
                                     lerp(random(i.xz), random(i.yz), u.x),
                                     u.y),
                                lerp(lerp(random(i.xy + 1.0), random(i.zy + 1.0), u.x),
                                     lerp(random(i.xz + 1.0), random(i.yz + 1.0), u.x),
                                     u.y),
                                u.z);
                }

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                    float noise = 0.0;
                    float time = _Time.y;

                    // 노이즈의 크기, 속도, 방향 등을 조정하여 TV처럼 지지직거리는 효과를 만듭니다.
                    noise += simplexNoise(uv * 20.0 + float2(time * 0.5, time)) * 0.05;
                    noise += simplexNoise(uv * 50.0 + float2(-time * 0.5, time * 0.5)) * 0.03;

                    // noise에 색상을 추가합니다.
                    float4 colorNoise = tex2D(_MainTex, uv + noise);
                    colorNoise = lerp(colorNoise, _Color, 0.5); // _Color는 쉐이더 프로퍼티에서 받아온 색상 값입니다.

                    return colorNoise;
                }
                ENDCG
            }
    }
}