#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;

uniform global2
{
	float offset;
	float offset2;
};


layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 outColor;
void PixelShaderFunction()
{
	vec2 uv = input.UV;
	float shrink = (((sin(offset2 + (uv.x * 4)) + 1.0) / 2) * 0.25) + 0.75;
	uv.y *= shrink;
	uv.y -= (1 - shrink) * 0.5;
	uv.y += 0.11;

    vec4 col = texture(sprite, uv + vec2(0, (sin(offset + uv.x) * 0.05)));
	col = col * input.Color;
    outColor = col;
}
@ENDIF_PS

TECHNIQUE(BasicTexture, SpriteVertexShader, PixelShaderFunction)

