//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "SpriteVS_switch.fxh"


@IFDEF_PS
uniform sampler2D sprite;
uniform sampler2D blur;

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 color;

void SpritePixelShader()
{
	float screenWidth = 134;
	float screenHeight = 86;

	vec4 col = texture(sprite, input.UV) * input.Color;
	vec4 blurVal = texture(blur, input.UV);
	col =  col * 0.7 + (blurVal * 0.6);

	float widePixel = round(((input.Position.x + 1) * 134) / 0.5) * 0.5;
	float tallPixel = round(((input.Position.y + 1) * 86) / 0.5) * 0.5;

	col.rgb *= 0.9 + (abs(mod(widePixel, 2)) * 0.1);
	col.rgb *= 0.9 + (abs(mod(tallPixel, 2)) * 0.1);

	col.a *= 0.7 + (abs(mod(widePixel, 2)) * 0.3);
	col.a *= 0.7 + (abs(mod(tallPixel, 2)) * 0.3);
	color = col;
}

@ENDIF_PS


TECHNIQUE( SpriteBatch, SpriteVertexShader, SpritePixelShader );