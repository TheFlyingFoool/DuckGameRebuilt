uniform extern texture Sprite;
sampler sprite = sampler_state
{
    Texture = <Sprite>;
};

float fade = 1.0f;
struct VertexToPixel
{
    float4 Position     : POSITION;
    float4 Color        : COLOR0;
	float2 UV			: TEXCOORD0;
};

float4 PixelShaderFunction(VertexToPixel input) : COLOR0
{
	float4 color = tex2D(sprite, input.UV) * input.Color;
	color.rgb *= fade * 0.5f;
	return color;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
