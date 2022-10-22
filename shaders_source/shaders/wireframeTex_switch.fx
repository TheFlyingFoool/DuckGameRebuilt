#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;

uniform Block
{
	float screenCross    ;
	float scanMul        ;
};

layout(location = 1) in VertexToPixel IN;
layout(location = 0) out vec4 color;
void PixelShaderFunction()
{
	vec4 cl = texture(sprite, IN.UV) * IN.Color;
	float red = floor(1.0 - (cl.r * cl.g * cl.b));
	float blu = floor(fract((IN.Position.y + 1.0) * 20) + 0.1);	
	blu += floor(fract((IN.Position.x + 1.0) * 32) + 0.1);

	float spos = ((IN.Position.x + 1.0) / 2);
	float s = floor(spos + screenCross);

	cl.b += ((max(1.0 - (min(abs((1.0 - spos) - screenCross), 0.2) * 10), 0)) * scanMul) * cl.a;

	color = (vec4(red * cl.a, blu * cl.a, 0.0, cl.a) * s) + (cl * (1.0 - s));
}

void PixelShaderFunctionSimple()
{
	color = IN.Color;
}
@ENDIF_PS

TECHNIQUE( BasicTexture, SpriteVertexShader, PixelShaderFunction );
TECHNIQUE( BasicSimple,  SpriteVertexShader, PixelShaderFunction );
