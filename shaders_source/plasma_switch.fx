//-----------------------------------------------------------------------------
// SpriteEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#include "Macros.fxh"


uniform sampler2D sprite;
uniform sampler2D sprite2;
uniform sampler2D sprite3;
uniform global
{
	mat4 MatrixTransform;
	
	float offset;
	float offset2;
	float scroll;
	float scroll2;
	float gradientOffset;
	float gradientOffset2;
	vec4 color1;
	vec4 color2;
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
	gl_Position = vec4(a_position, 1.0) * MatrixTransform;
	out_color = a_color;
	out_texCoord = a_texCoord;
	out_position = gl_Position;
}

@ENDIF_VS

@IFDEF_PS

layout(location = 0) in vec4 f_color;
layout(location = 1) in vec2 f_texCoord;
layout(location = 2) in vec2 f_position;
layout(location = 0) out vec4 color;

void SpritePixelShader()	
{
	vec2 texcoord = f_texCoord * 2;
	
    vec4 col = texture(sampler2D(sprite), texcoord + vec2(((sin((texcoord.y + scroll) * 40) * 0.05) * offset) -scroll2, -scroll));
	vec4 col2 = texture(sampler2D(sprite3), texcoord + vec2(((sin((( texcoord.y) - scroll2) * 40) * 0.05) * offset2) +scroll, scroll2));
	vec4 gradient = texture(sampler2D(sprite2), texcoord + vec2(0, gradientOffset)) * color2;
	vec4 gradient2 = texture(sampler2D(sprite2), texcoord + vec2(0, gradientOffset2)) * color1;

	col *= col2;

	col *= vec4(gradient2.r, gradient2.g, gradient2.b, gradient2.a);
	float inv = 1.0 - col.a;
	col += vec4(gradient.r * inv, gradient.g * inv, gradient.b * inv, inv);
	color = col;
}

@ENDIF_PS


TECHNIQUE( SpriteBatch, SpriteVertexShader, SpritePixelShader );