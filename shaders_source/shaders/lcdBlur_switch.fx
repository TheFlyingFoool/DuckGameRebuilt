//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 color;

void SpritePixelShader()	
{
	float blurSize = 0.012;

	vec4 sum = vec4(0.0, 0.0, 0.0, 0.0);
	sum += texture(sprite, vec2(input.UV.x - blurSize, input.UV.y)) * 0.125;
	sum += texture(sprite, vec2(input.UV.x + blurSize, input.UV.y)) * 0.125;
	sum += texture(sprite, vec2(input.UV.x - blurSize, input.UV.y - blurSize)) * 0.125;
	sum += texture(sprite, vec2(input.UV.x + blurSize, input.UV.y - blurSize)) * 0.125;
	sum += texture(sprite, vec2(input.UV.x + blurSize, input.UV.y + blurSize)) * 0.125;
	sum += texture(sprite, vec2(input.UV.x - blurSize, input.UV.y + blurSize)) * 0.125;
	sum += texture(sprite, vec2(input.UV.x, input.UV.y - blurSize)) * 0.125;
	sum += texture(sprite, vec2(input.UV.x, input.UV.y + blurSize)) * 0.125;
	color = sum;
}
@ENDIF_PS

TECHNIQUE( SpriteBatch, SpriteVertexShader, SpritePixelShader );