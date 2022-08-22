#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;
uniform sampler2D edge;

uniform Block
{
	float xpos;
	float ypos;
	float light1x;
	float light1y;
	float light2x;
	float light2y;
	float light3x;
	float light3y;
	float flipSub;
	vec3 fade;
	vec3 add;
};

layout(location = 1) in VertexToPixel frag_in;
layout(location = 0) out vec4 colr;
void PixelShaderFunction()
{
	vec4 col = texture(sprite, frag_in.UV);
	vec4 edgeCol = texture(edge, frag_in.UV);


	vec2 pixelOffset = vec2(((flipSub - frag_in.UV.x) * flipSub) + (frag_in.UV.x * (1.0 - flipSub)), frag_in.UV.y) * 64.0;
	vec2 pos = (vec2(xpos, ypos) + pixelOffset);

	vec4 lightingColor1 = vec4(1.0, 0.0, 0.0, 1.0);
	vec2 lightPos1 = vec2(light1x, light1y);

	vec4 lightingColor2 = vec4(0.9, 0.6, 0.2, 1.0);
	vec2 lightPos2 = vec2(light2x, light2y);

	vec4 lightingColor3 = vec4(0.0, 0.0, 1.0, 1.0);
	vec2 lightPos3 = vec2(light3x, light3y);

	float dist1 = 1.0 - min(length(pos - lightPos1) * 0.01, 1.0);
	float dist2 = 1.0 - min(length(pos - lightPos2) * 0.01, 1.0);
	float dist3 = 1.0 - min(length(pos - lightPos3) * 0.01, 1.0);
	dist1 = min((dist1 + 0.15) * (dist1 + 0.15), 1.0);
	dist2 = min((dist2 + 0.15) * (dist2 + 0.15), 1.0);
	dist3 = min((dist3 + 0.15) * (dist3 + 0.15), 1.0);
	edgeCol.r *= max(max(dist1, dist2), dist3);

	vec3 finalCol = (col.rgb * (1.0 - edgeCol.r)) + ((lightingColor1.rgb * dist1) * edgeCol.r) + ((lightingColor2.rgb * dist2) * edgeCol.r) + ((lightingColor3.rgb * dist3) * edgeCol.r);
	finalCol *= fade;
	finalCol += add;
	colr = vec4(finalCol.r, finalCol.g, finalCol.b, col.a);
}
@ENDIF_PS

TECHNIQUE(Test, SpriteVertexShader, PixelShaderFunction)
