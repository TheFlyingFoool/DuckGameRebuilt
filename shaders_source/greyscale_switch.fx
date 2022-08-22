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

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 color;

void SpritePixelShader()	
{
	vec4 col = texture(sampler2D(Texture), input.UV);
		
	float newCol = 	(col.r + col.g + col.b) / 3;
	col.r = newCol;
	col.g = newCol;
	col.b = newCol;
	color = col;
}

@ENDIF_PS


TECHNIQUE( Test, SpriteVertexShader, SpritePixelShader );