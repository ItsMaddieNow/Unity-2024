// This shader fills the mesh shape with a color predefined in the code.
Shader "Custom/Lighting"
{
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    {
        _Shininess ("Shininess", Float) = 1.0
        _DiffuseColour ("Diffuse Colour", Color) = (0.5, 0, 0, 1)
        _SpecularColour ("Specular Colour", Color) = (0.5, 0, 0, 1)
    }

    // The SubShader block containing the Shader code.
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                float3 normalWS     : TEXCOORD0;
                float3 positionWS   : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float _Shininess;
                half4 _DiffuseColour;
                half4 _SpecularColour;
            CBUFFER_END
            // The vertex shader definition with properties defined in the Varyings
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // Returning the output.
                VertexNormalInputs normals = GetVertexNormalInputs(IN.positionOS);
                OUT.normalWS = normals.normalWS;
                VertexPositionInputs positions = GetVertexPositionInputs(IN.positionOS);
                OUT.positionWS = positions.positionWS;
                
                return OUT;
            }
            
            half DiffuseComponent(half3 lightDir, half3 surfaceNormal)
            {
                return saturate(dot(surfaceNormal, lightDir));
            }
            half SpecularComponent(half3 lightDir, half3 surfaceNormal, half3 viewDir, half smoothness)
            {
                half3 h = SafeNormalize(lightDir+viewDir);
                return pow(saturate(dot(surfaceNormal, h)), smoothness);
            }
            // The fragment shader definition.
            half4 frag(Varyings IN) : SV_Target
            {
                // Defining the color variable and returning it.
                half4 color = 0;
                Light light = GetMainLight();
                //color.rgb = IN.viewDir;
                //DiffuseComponent(light.direction, IN.normalWS);
                //return color;
                float3 viewDir = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                return _DiffuseColour*max(DiffuseComponent(light.direction, SafeNormalize(IN.normalWS)), 0)+_SpecularColour*max(SpecularComponent(light.direction, SafeNormalize(IN.normalWS), viewDir, _Shininess),0);
                //+SpecularComponent(light.direction, IN.normalWS, IN.viewDir, 1.0f)
            }
            ENDHLSL
        }
    }
}