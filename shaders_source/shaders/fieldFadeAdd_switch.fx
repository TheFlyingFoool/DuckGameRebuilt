#include "Macros.fxh"

@IFDEF_VS
uniform Block
{
	mat4 WVP;
};

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 color	 ;
layout(location = 2) in vec2 texCoord;

layout(location = 0) out gl_PerVertex {
	vec4 gl_Position;
};
layout(location = 1) out vec2 vs_out_uv   ;
layout(location = 2) out vec4 vs_out_color;

void SpriteVertexShader()
{
	gl_Position   = position * WVP;
	vs_out_color = color   ;
	vs_out_uv    = texCoord;
}
@ENDIF_VS

@IFDEF_PS
uniform sampler2D sprite;

uniform block
{
	vec3 fade;
	vec3 add;
};

layout(location = 1) in  vec2 fs_in_uv   ;
layout(location = 2) in  vec4 fs_in_color;
layout(location = 0) out vec4       color;
void PixelShaderFunction()
{
	color = texture(sprite, fs_in_uv) * fs_in_color;
	color.rgb *= fade;
	color.rgb += add;
	color.rgb *= color.a;
}
@ENDIF_PS

TECHNIQUE( BasicTexture, SpriteVertexShader, PixelShaderFunction );