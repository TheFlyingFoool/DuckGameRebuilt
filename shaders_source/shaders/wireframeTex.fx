uniform extern texture Sprite;
sampler sprite = sampler_state
{
    Texture = <Sprite>;
};

struct VertexToPixel
{
    float4 Color        : COLOR0;
	float2 UV			: TEXCOORD0;
	float4 Position     :TEXCOORD2;
};

float screenCross = 0.5f;
float scanMul = 1.0f;
float4 PixelShaderFunction(VertexToPixel IN) : COLOR0
{
	float4 color = tex2D(sprite, IN.UV) * IN.Color;
	float red = floor(1.0 - (color.r * color.g * color.b));
	float blu = floor(((((IN.Position.y + 1.0) * 20) % 1) + 0.1));	
	blu += floor(((((IN.Position.x + 1.0) * 32) % 1) + 0.1));

	float spos = ((IN.Position.x + 1.0) / 2);
	float s = floor(spos + screenCross);

	color.b += ((max(1.0 - (min(abs((1.0 - spos) - screenCross), 0.2) * 10), 0)) * scanMul) * color.a;

	color = (float4(red * color.a, blu * color.a, 0.0, color.a) * s) + (color * (1.0 - s));
	return color;
}

float4 PixelShaderFunctionSimple(VertexToPixel input) : COLOR0
{
	float4 color = input.Color;
	return color;
}

technique BasicTexture
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

technique BasicSimple
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunctionSimple();
    }
}

