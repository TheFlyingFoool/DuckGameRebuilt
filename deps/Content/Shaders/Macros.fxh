//-----------------------------------------------------------------------------
// Macros.fxh
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#if __PSSL__
struct VSOutput
{
	float4 position		: S_POSITION;
	float4 color		: COLOR0;
    float2 uv			: TEXCOORD0;
	float4 outPos		: TEXCOORD2;
};

struct VSOutputPosition
{
	float4 position		: S_POSITION;
	float4 color		: COLOR0;
    float2 uv			: TEXCOORD0;
	float4 outPos		: TEXCOORD2;
};

struct VSOutputSimple
{
	float4 position		: S_POSITION;
	float4 color		: COLOR0;
};
#else
struct VSOutput
{
	float4 position		: POSITION;
	float4 color		: COLOR0;
    float2 uv			: TEXCOORD0;
	float4 outPos		: TEXCOORD2;
};

struct VSOutputPosition
{
	float4 position		: POSITION;
	float4 color		: COLOR0;
    float2 uv			: TEXCOORD0;
	float4 outPos		: TEXCOORD2;
};

struct VSOutputSimple
{
	float4 position		: POSITION;
	float4 color		: COLOR0;
};
#endif


#define VERTEXSHADER_DEF_POSITION 
	VSOutputPosition SpriteVertexShaderPosition(float4 position	: POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) \
	{ \
		VSOutputPosition output; \
		output.position = mul(position, MatrixTransform); \
		output.color = color; \
		output.uv = texCoord; \
		output.outPos = output.position; \
		return output; \
	}

#define VERTEXSHADER_DEF \
VSOutput SpriteVertexShader(float4 position	: POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0) \
{ \
	VSOutput output; \
	output.position = mul(position, MatrixTransform); \
	output.color = color; \
	output.uv = texCoord; \
	output.outPos = output.position; \
	return output; \
}

#define VERTEXSHADER_DEF_SIMPLE \
VSOutputSimple SpriteVertexShaderSimple(float4 position	: POSITION, float4 color : COLOR0) \
{ \
	VSOutputSimple output; \
	output.position = mul(position, MatrixTransform); \
	output.color = color; \
	return output; \
}


#if NSWITCH

// Macros for targetting Nintendo Switch

#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile vs_nx vsname (); PixelShader = compile ps_nx psname(); } }

#define DECLARE_TEXTURE(Name, index) \
    sampler2D Name

//#define DECLARE_CUBEMAP(Name, index) \
    //samplerCUBE Name : register(s##index);

#define SAMPLE_TEXTURE(Name, texCoord)  texture(Name, texCoord.xy)
//#define SAMPLE_CUBEMAP(Name, texCoord)  texCUBE(Name, texCoord)

#define UNROLL
#elif __PSSL__

// Macros for targetting PSSL for PlayStation 4.

#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile sce_vs_vs_orbis vsname(); PixelShader = compile sce_ps_orbis psname(); } }

#define BEGIN_CONSTANTS
#define MATRIX_CONSTANTS
#define END_CONSTANTS

#define _vs(r)
#define _ps(r)
#define _cb(r)

#define DECLARE_TEXTURE(Name, index) \
    Texture2D Name : register(t##index); \
    SamplerState Name##Sampler : register(s##index)

#define DECLARE_CUBEMAP(Name, index) \
    TextureCube Name : register(t##index); \
    SamplerState Name##Sampler : register(s##index)

#define SAMPLE_TEXTURE(Name, texCoord)  Name.Sample(Name##Sampler, texCoord)
#define SAMPLE_CUBEMAP(Name, texCoord)  Name.Sample(Name##Sampler, texCoord)

#define SV_Position	S_POSITION
#define SV_Target0	S_TARGET_OUTPUT0

#else


// Macros for targetting shader model 2.0 (DX9)

#define TECHNIQUE(name, vsname, psname ) \
	technique name { pass { VertexShader = compile vs_2_0 vsname (); PixelShader = compile ps_2_0 psname(); } }

#define TECHNIQUE_PS(name, psname ) \
	technique name { pass { PixelShader = compile ps_2_0 psname(); } }


#define BEGIN_CONSTANTS
#define MATRIX_CONSTANTS
#define END_CONSTANTS

#define _vs(r)  : register(vs, r)
#define _ps(r)  : register(ps, r)
#define _cb(r)

#define DECLARE_TEXTURE(Name, index) \
    sampler2D Name : register(s##index);

#define DECLARE_CUBEMAP(Name, index) \
    samplerCUBE Name : register(s##index);

#define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name, texCoord)
#define SAMPLE_CUBEMAP(Name, texCoord)  texCUBE(Name, texCoord)

#define UNROLL [unroll]

#endif