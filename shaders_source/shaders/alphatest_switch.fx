#include "SpriteVS_Switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;

layout(location = 1) in  VertexToPixel frag_in;
layout(location = 0) out vec4 color;
void PixelShaderFunction()
{
	vec4 col = texture(sprite, frag_in.UV);
	if (col.a < 0.5) discard;

	color = col;
}
@ENDIF_PS

TECHNIQUE(Test, SpriteVertexShader, PixelShaderFunction)
