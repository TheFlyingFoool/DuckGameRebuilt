#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;
uniform sampler2D overlay;

uniform block
{
	float screenWidth;
	float screenHeight;
};

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 outCol;
void PixelShaderFunction()
{
	vec4 col       = texture(sprite,  input.UV) * input.Color;
	vec4 overColor = texture(overlay, input.UV);

	outCol = ((col * 0.8f) * overColor) + ((col * overColor) * 0.25f);
}
@ENDIF_PS

TECHNIQUE(Test, SpriteVertexShader, PixelShaderFunction)
