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
float4 PixelShaderFunction(VertexToPixel input) : COLOR0
{
	float4 color = tex2D(sprite, input.UV);

	float red = floor(1.0 - (color.r * color.g * color.b)) * color.a;
	float blu = floor((((((input.Position.y + 1.0) * 40) % 5) / 5.0) * 1.08));
	blu += floor((((((input.Position.x + 1.0) * 70) % 5) / 5.0) * 1.08));
	float s = floor(((input.Position.x + 1.0) / 2.0) + screenCross);
	
	color = (float4(red, blu * color.a, 0.0, color.a) * s) + (float4(0,0,0,0) * (1.0 - s));

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

