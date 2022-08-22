#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;
uniform sampler2D sprite2;

layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 outColor;
void PixelShaderFunction()
{
	vec4 lightColor = texture(sprite, input.UV);
	vec4 baseColor = texture(sprite2, input.UV);

	outColor = texture(sprite2, input.UV) + lightColor;
}
@ENDIF_PS

TECHNIQUE(BasicTexture, SpriteVertexShader, PixelShaderFunction)
