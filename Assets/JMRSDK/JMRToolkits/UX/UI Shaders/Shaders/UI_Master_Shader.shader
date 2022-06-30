     
Shader "UI/UI_Master_Shader"
{
 
    Properties
    {
     [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [HideInInspector]  _Color ("Base Tint", Color) = (1,1,1,1)
        [Header(Mask)]
        _MaskColor ("Mask Color", Color) = (1,1,1,0)
        _MaskTex ("Mask", 2D) = "white" {}
        _InvertMask ("InvertMask", Range(0,1)) = 0.0
        _BlendAmount ("BlendMask", Range(0,1)) = 0.0

        [Header(Circle Animation)]
        _CircleColor ("CircleColor", Color) = (1,1,1,0)
        _Position("Position",Vector) = (0,0,0,0)
        _Radius("Radius", Range(0,5)) = 0
        _Softness("Softness",Range(0,10)) = 1
        _Alpha(" Alpha", Range(0,1)) = 0.5
        _ResizeCircle("Resize Circle X&Y",Vector) = (0,0,0,0)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

     
    }
 
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
       
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
 
           
            fixed4 _ResizeCircle;
            fixed4 _Position;
            fixed4 _Color;
          //  fixed4 _MaskColor;
            fixed4 _CircleColor;
            float4 _ClipRect;
            fixed4 _TextureSampleAdd;
            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _SecMaskTex;
            float _test;
            float _Radius;
            float _InvertMask;
            float _BlendAmount;
            float _Softness;
 
            float _Circles;
            float _Alpha;

           
            struct vertexIn
            {
                float4 vertex : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
 
            
            struct vertexOut
            {
                float4 vertex : SV_POSITION;
                fixed4 color    : COLOR;
                float4 texcoord01 : TEXCOORD0;
                float4 texcoord02 : TEXCOORD1;
                float4 texcoord03 : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _MaskColor)
            UNITY_INSTANCING_BUFFER_END(Props)
            vertexOut vert(vertexIn v)
            {
                vertexOut OUT;

                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord01.xy = v.texcoord;
                OUT.texcoord02.xy = v.texcoord1;
                OUT.texcoord03.xy = v.texcoord2;
                OUT.color = v.color * _Color;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, OUT);

                return OUT;
            }
     
         
          
         fixed4 frag(vertexOut IN) : SV_Target
         {
             
                fixed4 _mainTexture = tex2D(_MainTex, IN.texcoord01.xy)  ;
                fixed4 _maskTexture;
                fixed4 _topMaskTextue = tex2D(_MaskTex,1- IN.texcoord01.xy);
                fixed4 _downMaskTextue = tex2D(_MaskTex, IN.texcoord01.xy);
                _maskTexture = lerp( tex2D(_MaskTex, IN.texcoord01.xy), tex2D(_MaskTex,1- IN.texcoord01.xy),_InvertMask);
               
                float x = IN.texcoord01.x * _ResizeCircle.x;
                float y =  IN.texcoord01.y* _ResizeCircle.y;

                fixed4 _mainTextureColor = _mainTexture * IN.color; 
                UNITY_SETUP_INSTANCE_ID(IN);
                fixed4 _maskTextureColor = _maskTexture * UNITY_ACCESS_INSTANCED_PROP(Props, _MaskColor); 
                fixed4 firstOverlay = _mainTexture; 
                firstOverlay.rgb = lerp(_mainTextureColor, _maskTextureColor, ( _maskTextureColor.a *_BlendAmount)* _MaskColor.a).rgb;
                fixed4 returnTexture = firstOverlay;
   
                float dist = sqrt(pow((0.5 + _Position.x )- x, 2) 
                + pow((0.5+_Position.y ) - y, 2));
                 half sum = saturate((dist-_Radius)/-_Softness);

                returnTexture.rgb = lerp(firstOverlay, _CircleColor,(_CircleColor.a*_Alpha) * sum  ).rgb;
                returnTexture.a = _mainTextureColor.a;
                return returnTexture;
         }
                ENDCG
        }
    }
}
 
 
 