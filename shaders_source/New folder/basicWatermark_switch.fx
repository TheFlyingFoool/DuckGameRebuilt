//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform block
{
	float fade;
};

uniform sampler2D Texture;

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 color;
void SpritePixelShader()	
{
	vec4 col = texture(Texture, input.UV) * input.Color;
	col.rgb *= fade * 0.5;
	color = col;
}
@ENDIF_PS

TECHNIQUE( SpriteBatch, SpriteVertexShader, SpritePixelShader );