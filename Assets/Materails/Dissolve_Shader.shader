Shader "Unlit/Dissolve_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("_NoiseTex", 2D) = "white" {}

        _Color("_Color", Color) = (1,1,1,1)

        _HighlightColor("_HighlightColor", Color) = (1,1,1,1)

        _Threshold("_Threshold", Range(0,1)) = 0.1
        _HighlightThreshold("_HighlightThreshold", Range(0,1)) = 0.1



    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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

                float2 noiseuv : TESSFACTOR1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            fixed4 _HighlightColor;
            fixed4 _Color;

            float _Threshold;
            float _HighlightThreshold;


            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.noiseuv = TRANSFORM_TEX(v.uv,_NoiseTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                fixed4 noiseTex = tex2D(_NoiseTex,i.noiseuv);


                //高光宽度限制
                //1、当未开始溶解时，不显示高光
                //2、当溶解开始时，若溶解阈值小于高光阈值（高光宽度），则高光宽度随溶解阈值增大（避免溶解开始的瞬间出现大宽度高光）
                //3、当溶解阈值超过高光阈值，则设为高光阈值
                _HighlightThreshold = _Threshold ==0?0:_Threshold<_HighlightThreshold?_Threshold:_HighlightThreshold;

                fixed noiseDiff = pow(saturate(noiseTex.r - _Threshold )/_HighlightThreshold, 5);

                clip(noiseTex.r - _Threshold); //剔除像素
                col = lerp(col,_HighlightColor,saturate( 1 - noiseDiff));



                return col;
            }
            ENDCG
        }
    }
}
