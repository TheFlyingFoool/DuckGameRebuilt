sampler sprite;
float fade;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	float4 col = tex2D(sprite, uv);
	col.rgb = max(floor(1.05f - (col.r + col.g + col.b)), 0) * col.a;
	col *= c;
	col *= fade;
	col.a *= 0.75f;
	return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
