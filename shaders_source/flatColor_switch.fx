#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;
layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 outColor;
void PixelShaderFunction()
{
	vec4 lightColor = texture(sprite, input.UV);
	outColor = vec4(0.364, 0.709, 0.866, 1.0) * lightColor.a;
}
@ENDIF_PS

TECHNIQUE(BasicTexture, SpriteVertexShader, PixelShaderFunction)
