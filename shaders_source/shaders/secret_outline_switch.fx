#include "Macros.fxh"
#include "SpriteVS_switch.fxh"

@IFDEF_PS

uniform sampler2D sprite;
layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 color;
void SpritePixelShader()
{
	vec4 col = texture(sampler2D(sprite), input.UV);
	col.rgb = vec3(max(floor(1.2 - (col.r + col.g + col.b)), 0) * col.a);
	col *= input.Color;
	color = col;
}

@ENDIF_PS


TECHNIQUE(Test, SpriteVertexShader, SpritePixelShader);