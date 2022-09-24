#include "SpriteVS_switch.fxh"

@IFDEF_PS

uniform sampler2D sprite;
uniform sampler2D page;

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 outCol;
void PixelShaderFunction()
{
	vec4 pageOffset =  texture(sampler2D(page),   vec2(input.UV.x, 0));
    vec4 outCol     =  texture(sampler2D(sprite), input.UV + vec2(0, pageOffset.r * 0.5f));
}
@ENDIF_PS

TECHNIQUE(SpriteBatch, SpriteVertexShader, PixelShaderFunction)
