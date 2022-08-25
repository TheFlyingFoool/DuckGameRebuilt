struct VertexToPixel
{
    float4 Position     : POSITION;
    float4 Color        : COLOR0;
	float2 UV			: TEXCOORD0;
};

float4 PixelShaderFunctionSimple(VertexToPixel input) : COLOR0
{
	float4 color = input.Color * 0.1f;
	return color;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunctionSimple();
    }
}

