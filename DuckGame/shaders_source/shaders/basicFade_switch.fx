//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "Macros.fxh"
#include "SpriteVS_switch.fxh"

@IFDEF_PS

uniform sampler2D Texture;

uniform block
{
	vec3 fade;
};

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 color;

void SpritePixelShader()	
{
	color = texture(sampler2D(Texture), input.UV) * input.Color;
	color.rgb *= fade;
}

void SimplePixelShader()	
{
	color = input.Color;
	color.rgb *= fade;
}

@ENDIF_PS


TECHNIQUE( BasicTexture, SpriteVertexShader, SpritePixelShader );
TECHNIQUE( BasicSimple, SpriteVertexShader, SimplePixelShader );