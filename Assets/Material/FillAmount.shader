Shader "Custom/RadialFill"
{
    Properties
    {
    _MainTex ("Sprite Texture", 2D) = "white" {}
    _Fill ("Fill Amount", Range(0,1)) = 1.0
    _CenterX ("Center X", Float) = 0.5
    _CenterY ("Center Y", Float) = 0.5
    _FillColor ("Fill Color", Color) = (0.176, 0.263, 0.043, 1)  
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float _Fill;
            float _CenterX;
            float _CenterY;
            float4 _FillColor;  

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(_CenterX, _CenterY);
                float2 uv = i.uv - center;

                float angle = atan2(uv.y, uv.x) / (2.0 * 3.14159) + 0.5;
                if (angle > _Fill)
                    discard; 

                fixed4 texColor = tex2D(_MainTex, i.uv);
                return texColor * _FillColor;  
            }
            ENDCG
        }
    }
}
