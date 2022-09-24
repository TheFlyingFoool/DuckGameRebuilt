uniform sampler2D sprite;

struct VertexToPixel
{
	vec4 Color;
	vec2 UV;
	vec4 Position;
};

float screenCross = 0.5f;

layout(location = 1) in  VertexToPixel IN      ;
layout(location = 0) out vec4          colorOut;

void PixelShaderFunction()
{
	vec4 color = texture(sprite, IN.UV) * IN.Color;
	float red = floor(1.0 - (color.r * color.g * color.b));
	float blu = floor(fract((IN.Position.y + 1.0) * 20) + 0.1);	
	blu      += floor(fract((IN.Position.x + 1.0) * 32) + 0.1);

	float spos = (IN.Position.x + 1.0) / 2;
	float s = floor(spos + screenCross);

	colorOut = vec4(red * color.a, blu* color.a, 0.0, color.a) * s;
}

void PixelShaderFunctionSimple()
{
	colorOut = IN.Color;
}

technique BasicTexture
{
	pass Pass1
	{
		PixelShader = compile ps_nx PixelShaderFunction();
	}
}

technique BasicSimple
{
	pass Pass1
	{
		PixelShader = compile ps_nx PixelShaderFunctionSimple();
	}
}

