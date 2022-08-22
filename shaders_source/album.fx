sampler sprite;
sampler page;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	float4 pageOffset = tex2D(page, float2(uv.x, 0));
    float4 col = tex2D(sprite, uv + float2(0, pageOffset.r * 0.5f));
	//col.rgb = (col.r + col.g + col.b) / 3;
    return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
