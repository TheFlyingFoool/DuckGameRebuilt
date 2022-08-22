//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// Armature Edit:
//	 This used to have a dummy MatrixTransform member to avoid a warning, about
//   a terrible hack to work around some monogame issues. The warning has been silenced.
//			
//-----------------------------------------------------------------------------

#include "Macros.fxh"


uniform sampler2D sprite;
uniform sampler2D sprite2;

uniform vsglobal
{
	mat4 viewMatrix;
	mat4 projMatrix;
};

uniform global
{
	vec2 topLeft;
	vec2 size;
	float fade;
};



@IFDEF_VS

layout(location = 0) in vec3 a_position;
layout(location = 1) in vec4 a_color;
layout(location = 2) in vec2 a_texCoord;

out gl_PerVertex
{
  vec4 gl_Position;
};

layout(location = 0) out vec4 out_color;
layout(location = 1) out vec2 out_texCoord;
layout(location = 2) out vec4 out_position;

void SpriteVertexShader()
{
	gl_Position = vec4(a_position, 1.0) * viewMatrix;
	gl_Position = gl_Position * projMatrix;
	out_color = a_color;
	out_texCoord = a_texCoord;
	out_position = vec4(a_position, 1.0);
}

@ENDIF_VS

@IFDEF_PS

layout(location = 0) in vec4 f_color;
layout(location = 1) in vec2 f_texCoord;
layout(location = 2) in vec2 f_position;
layout(location = 0) out vec4 color;

void SpritePixelShader()	
{
    vec4 col = texture(sampler2D(sprite), f_texCoord);
	vec4 cone = texture(sampler2D(sprite2), vec2((f_position.x - topLeft.x) / size.x, (f_position.y - topLeft.y) / size.y));
	cone.a = cone.r;

	color = (col * 0.0001) + (cone * f_color) * fade;
}

@ENDIF_PS


TECHNIQUE( SpriteBatch, SpriteVertexShader, SpritePixelShader );