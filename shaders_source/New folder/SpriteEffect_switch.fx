//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "Macros.fxh"


uniform sampler2D Texture;

#include "SpriteVS_switch.fxh"

@IFDEF_PS
layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 color;
void SpritePixelShader()	
{
	color = texture(Texture, input.UV) * input.Color;
}
@ENDIF_PS

TECHNIQUE( SpriteBatch, SpriteVertexShader, SpritePixelShader );