//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "Macros.fxh"

#include "SpriteVS_switch.fxh"

uniform sampler2D Texture;

uniform block
{
	vec3 fcol;
};

@IFDEF_PS

layout(location = 1) in VertexToPixel IN;
layout(location = 0) out vec4 color;

void SpritePixelShader()	
{
	vec4 col = texture(Texture, IN.UV);
		
	if (col.r == col.g && col.g == col.b)
	{
		col.rgb *= fcol;
	}
		
	color = col;
}

@ENDIF_PS

TECHNIQUE( SpriteBatch, SpriteVertexShader, SpritePixelShader );