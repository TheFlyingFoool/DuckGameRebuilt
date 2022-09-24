#include "SpriteVS_switch.fxh"

@IFDEF_PS
uniform sampler2D sprite;
uniform Block
{
	float scan;
	float secondScan;
};;

layout(location = 1) in VertexToPixel IN;
layout(location = 0) out vec4 colorOut;

void PixelShaderFunction()
{
	float screenCross = 1.0;
	float screenMul = 0.5;
	vec4  color = texture(sprite, IN.UV);
	vec4  texCol = color;

	float red = floor(1.0 - (color.r * color.g * color.b));

	color = vec4(red, 0.0, 0.0, 1.0) * (color.a * min(max(0, (IN.Position.x - scan) * 10000), 1));
	color += (1.0 - min(max(0, (IN.Position.x - scan) * 10000), 1)) * (texCol * secondScan);

	//color *= IN.Position;

	colorOut = color * IN.Color;
}
@ENDIF_PS

TECHNIQUE( Test, SpriteVertexShader, PixelShaderFunction )
