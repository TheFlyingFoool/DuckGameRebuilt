uniform extern texture Sprite;
sampler sprite = sampler_state
{
    Texture = <Sprite>;
};

float3 fade = 1.0f;
float3 add = 0.0f;
struct VertexToPixel
{
    float4 Position     : POSITION;
    float4 Color        : COLOR0;
	float2 UV			: TEXCOORD0;
};

float4 PixelShaderFunction(VertexToPixel input) : COLOR0
{
	float4 color = tex2D(sprite, input.UV) * input.Color;
	color.rgb *= fade;
	color.rgb += add;
	color.rgb *= color.a;
	return color;
}

float4 PixelShaderFunctionSimple(VertexToPixel input) : COLOR0
{
	float4 color = input.Color;
	color.rgb *= fade;
	color.rgb += add;
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
