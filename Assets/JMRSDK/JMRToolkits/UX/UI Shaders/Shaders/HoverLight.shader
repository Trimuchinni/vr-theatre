Shader "JMRSDK/HoverLight"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
       
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

         _Hovercolor("HoverColor", Color) = (1,1,1,1)
        _Position("World Position",Vector) = (0,0,0,0)
        _Radius ("Radius", float) = 0.5
         _Softness ("Softness", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _Hovercolor;
        float _Radius;
        float _Softness;
        float4 _Position;
        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            float d = distance(_Position,IN.worldPos);
            half sum =saturate( (d-_Radius)/- _Softness);

           fixed4 lerpColor = lerp(c.rgba,_Hovercolor,sum);
           
            
              o.Albedo = lerpColor.rgb;
           
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
