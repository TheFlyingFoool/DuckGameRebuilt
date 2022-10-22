//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D Texture;
uniform global
{
	vec3 fade;
	vec3 add;
};

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 color;

void SpritePixelShader()	
{
	color = texture(Texture, input.UV) * input.Color;
	color.rgb *= fade;
	color.rgb += add;
	color.rgb *= color.a;
}

void SimplePixelShader()	
{
	color = input.Color;
	color.rgb *= fade;
	color.rgb += add;
}
@ENDIF_PS

TECHNIQUE( BasicTexture, SpriteVertexShader, SpritePixelShader );
TECHNIQUE( BasicSimple, SpriteVertexShader, SimplePixelShader );