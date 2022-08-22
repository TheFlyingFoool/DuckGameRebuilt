//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D Sprite;
uniform block
{
	float screenCross;
};

layout(location = 1) in VertexToPixel IN;
layout(location = 0) out vec4 color;

void PixelShaderFunction()	
{
    vec4 cl = texture(Sprite, IN.UV) * IN.Color;
	float red = floor(1.0 - (cl.r * cl.g * cl.b));
	float blu = floor(fract((IN.Position.y + 1.0) * 20) + 0.1);	
	blu += floor(fract((IN.Position.x + 1.0) * 32) + 0.1);

	float spos = ((IN.Position.x + 1.0) / 2);
	float s = floor(spos + screenCross);

	color = (vec4(red * cl.a, blu* cl.a, (1.0 - (min(abs((1.0 - spos) - screenCross), 0.2) * 10)) * cl.a, cl.a) * s);
}

void PixelShaderFunctionSimple()
{
	color = IN.Color;
}

@ENDIF_PS

TECHNIQUE( BasicTexture, SpriteVertexShader, PixelShaderFunction       );
TECHNIQUE( BasicSimple,  SpriteVertexShader, PixelShaderFunctionSimple );
