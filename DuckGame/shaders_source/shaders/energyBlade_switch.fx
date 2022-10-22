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
	float time;
	float glow;
	vec4 bladeColor;
};

layout(location = 1) in VertexToPixel frag_in;
layout(location = 0) out vec4 colr; 
void PixelShaderFunction()
{
	vec2 offset = vec2(sin((time * 12.0) + (frag_in.UV.y * 12.0)) * 0.05, 0.0) * glow;
	vec2 offset2 = vec2(sin((time * 22.0) + (frag_in.UV.y * 18.0)) * 0.04, offset.x * 0.5);

    vec4 col = texture(sprite, frag_in.UV + offset) * frag_in.Color;
	vec4 origCol = col;

	vec4 col2 = texture(sprite, frag_in.UV + vec2(0.02, 0.02) + offset2);
	vec4 col3 = texture(sprite, frag_in.UV - vec2(0.02, 0.02) - offset2);
	vec4 goldCol = texture(gold, vec2((mod(frag_in.UV.x, width) / width) + (xpos * 0.01), (mod(frag_in.UV.y, height) / height) + (ypos * 0.01))) * bladeColor;
	col = (col + col2 + col3) / 3.0;
	col.rgb = (((goldCol.rgb + goldCol.rgb) + (col.rgb)) * col.a) / 2.0;

	colr = (col * glow) + (origCol * (1.0 - glow));
}
@ENDIF_PS

TECHNIQUE(Test, SpriteVertexShader, PixelShaderFunction)