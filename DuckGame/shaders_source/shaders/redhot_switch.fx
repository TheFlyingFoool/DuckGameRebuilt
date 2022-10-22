
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
	float intensity;
};

layout(location = 1) in  VertexToPixel input ;
layout(location = 0) out vec4 col;

void PixelShaderFunction()
{
	col = texture(sprite, input.UV);
	vec4 goldCol = texture(gold, vec2((mod(input.UV.x, width) / width) + (xpos * 0.01f),( mod(input.UV.y, height) / height) + (ypos * 0.01f)));

	float border = 1.0f - floor(1.0 - (col.r * col.g * col.b));

	vec4 newCol = col;
	newCol.rgb = vec3((newCol.r + newCol.g + newCol.b) / 3);
	newCol.rgb = ((newCol.rgb) * ((goldCol.rgb * 2.3f))) * border;
	newCol.r *= 1.1f;

	col = (col * (1.0f - intensity)) + (newCol * intensity);
	col *= input.Color;
}

@ENDIF_PS

technique Test
{
	pass Pass1
	{
		VertexShader = compile vs_nx SpriteVertexShader();
		PixelShader = compile ps_nx PixelShaderFunction();
	}
}
