#include "SpriteVS_Switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;
uniform Block
{
	vec2 lightPos;
	float density;
	float weight;
	float decay;
	float exposure;
};

layout(location = 1) in  VertexToPixel frag_in;
layout(location = 0) out vec4 outCol;
void PixelShaderFunction()
{
	int NUM_SAMPLES = 12;

	vec2 deltaTexCoord = (frag_in.UV - lightPos);
	deltaTexCoord *= 1.0f / NUM_SAMPLES * density;
	vec3 color = vec3(texture(sprite, frag_in.UV));
	float illuminationDecay = 1.0f;

	vec2 sample_coords = frag_in.UV;
	for (int i = 0; i < NUM_SAMPLES; i++)
	{
		sample_coords -= deltaTexCoord;
		vec3 samp = vec3(texture(sprite, sample_coords));

		samp *= illuminationDecay * weight;
		color += samp;
		illuminationDecay *= decay;
	}

	outCol = vec4(color * exposure, 1);
}
@ENDIF_PS

TECHNIQUE( BasicTexture,SpriteVertexShader, PixelShaderFunction )