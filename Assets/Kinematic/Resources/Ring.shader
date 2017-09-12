// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:6,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:4,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32823,y:32721,varname:node_3138,prsc:2|emission-9630-OUT,alpha-2281-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32234,y:32773,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:8583,x:32257,y:33019,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_8583,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6118-UVOUT;n:type:ShaderForge.SFN_Multiply,id:9630,x:32512,y:32825,varname:node_9630,prsc:2|A-7241-RGB,B-8583-RGB;n:type:ShaderForge.SFN_Multiply,id:2281,x:32512,y:32936,varname:node_2281,prsc:2|A-7241-A,B-8583-A;n:type:ShaderForge.SFN_Rotator,id:6118,x:32090,y:32998,varname:node_6118,prsc:2|UVIN-928-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:928,x:31922,y:32843,varname:node_928,prsc:2,uv:0;proporder:7241-8583;pass:END;sub:END;*/

Shader "Shader Forge/Ring" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _MainTex ("MainTex", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_3981 = _Time + _TimeEditor;
                float node_6118_ang = node_3981.g;
                float node_6118_spd = 1.0;
                float node_6118_cos = cos(node_6118_spd*node_6118_ang);
                float node_6118_sin = sin(node_6118_spd*node_6118_ang);
                float2 node_6118_piv = float2(0.5,0.5);
                float2 node_6118 = (mul(i.uv0-node_6118_piv,float2x2( node_6118_cos, -node_6118_sin, node_6118_sin, node_6118_cos))+node_6118_piv);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_6118, _MainTex));
                float3 emissive = (_Color.rgb*_MainTex_var.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,(_Color.a*_MainTex_var.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
