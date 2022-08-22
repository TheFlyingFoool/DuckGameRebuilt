//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "Macros.fxh"

uniform global
{
	mat4 MatrixTransform;
};


@IFDEF_VS

layout(location = 0) in vec3 a_position;
layout(location = 1) in vec4 a_color;

out gl_PerVertex
{
  vec4 gl_Position;
};

layout(location = 0) out vec4 out_color;

void SpriteVertexShader()
{
	gl_Position = vec4(a_position, 1.0) * MatrixTransform;
	out_color = a_color;
}

@ENDIF_VS

@IFDEF_PS

layout(location = 0) in vec4 f_color;
layout(location = 0) out vec4 color;

void SpritePixelShader()	
{
	color = f_color;
}

@ENDIF_PS


TECHNIQUE( SpriteBatch, SpriteVertexShader, SpritePixelShader );