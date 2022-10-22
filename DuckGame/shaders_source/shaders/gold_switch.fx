#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;
uniform sampler2D gold;

uniform Block
{
	float width;
	float height;
	float xpos;
	float ypos;
};

layout(location = 1) in VertexToPixel frag_in;
layout(location = 0) out vec4 col;
void PixelShaderFunction()
{
	col = texture(sprite, frag_in.UV);
	vec4 goldCol = texture(gold, vec2((mod(frag_in.UV.x, width) / width) + (xpos * 0.01f), (mod(frag_in.UV.y, height) / height) + (ypos * 0.01f)));

	float border = 1.0f - floor(1.0 - (col.r * col.g * col.b));

	col.rgb = vec3(col.r + col.g + col.b) / 3;
	col.rgb = ((col.rgb) * ((goldCol.rgb * 2.3f))) * border;
	col *= frag_in.Color;
}
@ENDIF_PS

TECHNIQUE( Test, SpriteVertexShader, PixelShaderFunction )
