Shader "Unlit/ButtonDefault"
{
    Properties
    {
        _HealthTex ("Texture", 2D) = "white" {}
        _CurrentHealthPercent("CurrentHealthPercent",Range(0,1)) = 1
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
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"
            sampler2D _HealthTex;
            float _CurrentHealthPercent;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // model space uv
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //fixed4 output = tex2D(_HealthTex, i.uv);
                fixed4 output = tex2D(_HealthTex, float2((_CurrentHealthPercent+0.01)*0.9,1));
                output = output - (i.uv.x > _CurrentHealthPercent);
                float4 flash = _Time.y % 2;
                output = output + (_CurrentHealthPercent < 0.2) * flash * (output > 0);
                return output;
            }
            ENDCG
        }
    }
}
