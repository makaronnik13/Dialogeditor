// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1873,x:33229,y:32719,varname:node_1873,prsc:2|emission-1086-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Multiply,id:1086,x:32812,y:32818,cmnt:RGB,varname:node_1086,prsc:2|A-1791-RGB,B-5983-RGB,C-5376-RGB;n:type:ShaderForge.SFN_Color,id:5983,x:32551,y:32915,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:0.759;n:type:ShaderForge.SFN_VertexColor,id:5376,x:32551,y:33079,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:603,x:32812,y:32992,cmnt:A,varname:node_603,prsc:2|A-1791-A,B-5983-A,C-5376-A,D-160-A;n:type:ShaderForge.SFN_TexCoord,id:3260,x:31934,y:32822,varname:node_3260,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:160,x:32738,y:33171,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_160,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:06e4c8f553bbc6446938aeb62e15db8d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Rotator,id:9431,x:32346,y:32843,varname:node_9431,prsc:2|UVIN-6594-OUT;n:type:ShaderForge.SFN_Tex2d,id:1791,x:32522,y:32555,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_1791,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a86f6f703cbb32a4698c15547d64d364,ntxv:0,isnm:False|UVIN-9431-UVOUT;n:type:ShaderForge.SFN_Slider,id:4853,x:31682,y:33049,ptovrint:False,ptlb:node_4853,ptin:_node_4853,varname:node_4853,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8034188,max:2;n:type:ShaderForge.SFN_Multiply,id:7269,x:32108,y:32872,varname:node_7269,prsc:2|A-3260-UVOUT,B-4853-OUT;n:type:ShaderForge.SFN_Add,id:6594,x:32324,y:32970,varname:node_6594,prsc:2|A-7269-OUT,B-9996-OUT;n:type:ShaderForge.SFN_Slider,id:1146,x:31840,y:33177,ptovrint:False,ptlb:node_1146,ptin:_node_1146,varname:node_1146,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.1025641,max:1;n:type:ShaderForge.SFN_Slider,id:6506,x:31820,y:33243,ptovrint:False,ptlb:node_6506,ptin:_node_6506,varname:node_6506,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.1025641,max:1;n:type:ShaderForge.SFN_Append,id:9996,x:32158,y:33202,varname:node_9996,prsc:2|A-1146-OUT,B-6506-OUT;proporder:5983-160-1791-4853-1146-6506;pass:END;sub:END;*/

Shader "Shader Forge/ItemSelection" {
    Properties {
        _Color ("Color", Color) = (1,1,1,0.759)
        _Mask ("Mask", 2D) = "white" {}
        _MainTex ("MainTex", 2D) = "white" {}
        _node_4853 ("node_4853", Range(0, 2)) = 0.8034188
        _node_1146 ("node_1146", Range(-1, 1)) = 0.1025641
        _node_6506 ("node_6506", Range(-1, 1)) = 0.1025641
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _node_4853;
            uniform float _node_1146;
            uniform float _node_6506;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_6725 = _Time + _TimeEditor;
                float node_9431_ang = node_6725.g;
                float node_9431_spd = 1.0;
                float node_9431_cos = cos(node_9431_spd*node_9431_ang);
                float node_9431_sin = sin(node_9431_spd*node_9431_ang);
                float2 node_9431_piv = float2(0.5,0.5);
                float2 node_9431 = (mul(((i.uv0*_node_4853)+float2(_node_1146,_node_6506))-node_9431_piv,float2x2( node_9431_cos, -node_9431_sin, node_9431_sin, node_9431_cos))+node_9431_piv);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_9431, _MainTex));
                float3 emissive = (_MainTex_var.rgb*_Color.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                return fixed4(finalColor,(_MainTex_var.a*_Color.a*i.vertexColor.a*_Mask_var.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
