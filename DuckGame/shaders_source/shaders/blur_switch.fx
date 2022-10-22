#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;
uniform Block
{
	vec3 fade;
	vec3 add;
};


layout(location = 1) in VertexToPixel input;
layout(location = 0) out vec4 sum;
void PixelShaderFunction()
{
	float blurSize = 0.002f;

	vec4 sum = vec4(0.0, 0.0, 0.0, 0.0);
	sum += texture(sprite, vec2(input.UV.x, input.UV.y - 4.0*blurSize)) * 0.05;
	sum += texture(sprite, vec2(input.UV.x, input.UV.y - 3.0*blurSize)) * 0.09;
	sum += texture(sprite, vec2(input.UV.x, input.UV.y - 2.0*blurSize)) * 0.12;
	sum += texture(sprite, vec2(input.UV.x, input.UV.y - blurSize)) * 0.15;
	sum += texture(sprite, vec2(input.UV.x, input.UV.y)) * 0.16;
	sum += texture(sprite, vec2(input.UV.x, input.UV.y + blurSize)) * 0.15;
	sum += texture(sprite, vec2(input.UV.x, input.UV.y + 2.0*blurSize)) * 0.12;
	sum += texture(sprite, vec2(input.UV.x, input.UV.y + 3.0*blurSize)) * 0.09;
	sum += texture(sprite, vec2(input.UV.x, input.UV.y + 4.0*blurSize)) * 0.05;

	sum.rgb *= fade;
	sum.rgb += add;
}
@ENDIF_PS

TECHNIQUE( Test, SpriteVertexShader, PixelShaderFunction )