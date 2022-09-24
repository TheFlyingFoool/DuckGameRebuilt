//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "SpriteVS_Switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;
uniform sampler2D sprite2;

uniform global
{
	float fade;
	float dim;
	float scrollX;
	float scrollY;
};

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 color;

void SpritePixelShader()	
{
	vec4 col = texture(sprite, input.UV) * (1.0f - fade);	
	vec4 col2 = texture(sprite2, ((input.UV * 0.75f) + vec2(0.0f, 0.25f)) + vec2(scrollX, scrollY));
    color = (col + (col2 * fade)) * dim;
}

@ENDIF_PS

TECHNIQUE( SpriteBatch, SpriteVertexShader, SpritePixelShader );