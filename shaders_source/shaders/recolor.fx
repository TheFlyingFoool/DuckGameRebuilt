sampler sprite;
float3 fcol;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
    float4 col = tex2D(sprite, uv);
	if((int)(col.r * 255) == 255 && (int)(col.r * 255) == 255 && (int)(col.r * 255) == 255)
		col.rgb = fcol.rgb;

	if((int)(col.r * 255) == 157)
		col.rgb = fcol.rgb * 0.7f;

	//col.rgb *= fcol;

    return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
