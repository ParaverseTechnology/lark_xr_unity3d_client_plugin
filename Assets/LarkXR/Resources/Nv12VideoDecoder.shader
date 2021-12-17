Shader "Custom/Nv12VideoDecoder"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _UVTex("Texture", 2D) = "white" {}
        _EYE("Render Eye, 0=left, 1=right, 2=both", Int) = 0
        _FLIP_Y("flip uv-y; 0=not flip; 1=flip", Int) = 0
    }
        SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            int _EYE;
            int _FLIP_Y;

            // float4x4 _TextureScale;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.vertex = mul(UNITY_MATRIX_VP, mul( mul( UNITY_MATRIX_M, _TextureScale ), v.vertex));
                // o.vertex = 0.1f * o.vertex;
                // o.uv = v.uv;
                // o.vertex = v.vertex;

                // left eye
                if (_EYE == 0) {
                    o.uv.x = v.uv.x / 2;
                }
                // right eye
                else if (_EYE == 1) {
                    o.uv.x = 0.5 + v.uv.x / 2;
                }
                // render both eye
                else {
                    o.uv.x = v.uv.x;
                }
                if (_FLIP_Y == 0) {
                    o.uv.y = v.uv.y;
                } else {
                    o.uv.y = (1 - v.uv.y);
                }
                // o.uv = mul(_TextureScale, float4(o.uv, 0, 1)).xy;
                return o;
            }

            static const float3x3 YUVtoRGBCoeffMatrix =
            {
                1.164383f, 1.164383f, 1.164383f,
                0.000000f, -0.391762f, 2.017232f,
                1.596027f, -0.812968f, 0.000000f
            };

            sampler2D _MainTex;
            sampler2D _UVTex;

            float3 ConvertYUVtoRGB(float3 yuv) 
            {
                // Derived from https://msdn.microsoft.com/en-us/library/windows/desktop/dd206750(v=vs.85).aspx
                // Section: Converting 8-bit YUV to RGB888
                // These values are calculated from (16 / 255) and (128 / 255)
                yuv -= float3(0.062745f, 0.501960f, 0.501960f);
                yuv = mul(yuv, YUVtoRGBCoeffMatrix);
                return saturate(yuv);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float y = tex2D(_MainTex, i.uv);
                float2 uv = tex2D(_UVTex, i.uv);
                float3 YUV = float3(y, uv.r, uv.g);
                float3 RGB = ConvertYUVtoRGB(YUV);
                return float4(RGB.r, RGB.g, RGB.b, 1);
                // return float4(RGB.r, 0, 0, 1);
            }
            ENDCG
        }
    }
}