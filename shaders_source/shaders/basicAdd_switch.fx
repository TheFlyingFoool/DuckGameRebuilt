//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "Macros.fxh"
#include "SpriteVS_switch.fxh"

@IFDEF_PS

uniform sampler2D sprite;

uniform block
{
	vec3 add;
};

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 color;

void PixelShaderFunction()
{
	color = texture(sprite, input.UV) * input.Color;
	color.rgb += add;
	color.rgb *= color.a;
}

void PixelShaderFunctionSimple()
{
	color = input.Color;
	color.rgb += add;
}

@ENDIF_PS


TECHNIQUE( BasicTexture, SpriteVertexShader, PixelShaderFunction       );
TECHNIQUE( BasicSimple,  SpriteVertexShader, PixelShaderFunctionSimple );