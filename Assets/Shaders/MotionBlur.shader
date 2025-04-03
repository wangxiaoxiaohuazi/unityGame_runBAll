Shader "Custom/MotionBlur" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0,1)) = 0.5
    }

    SubShader {
        Tags { "RenderType"="Opaque" }

        CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float _BlurAmount;

        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        v2f vert(appdata_img v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord.xy;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target {
            // 创建动态模糊偏移
            float2 randomOffset = float2(
                (frac(sin(i.uv.x * 723.13) * 2354.17) - 0.5) * 0.02,
                (frac(sin(i.uv.y * 642.31) * 3241.29) - 0.5) * 0.02
            ) * _BlurAmount;

            // 采样周围像素
            fixed4 color = tex2D(_MainTex, i.uv);
            color += tex2D(_MainTex, i.uv + randomOffset * 0.4);
            color += tex2D(_MainTex, i.uv - randomOffset * 0.4);
            color /= 3;

            return color;
        }
        ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    FallBack Off
} 