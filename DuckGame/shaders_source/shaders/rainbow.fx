sampler sprite;
float offset;
float offset2;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	float shrink = (((sin(offset2 + (uv.x * 4)) + 1.0) / 2) * 0.25) + 0.75;
	uv.y *= shrink;
	uv.y -= (1 - shrink) * 0.5;
	uv.y += 0.11;

    float4 col = tex2D(sprite, uv + float2(0, (sin(offset + uv.x) * 0.05)));
	col = col * c;
    return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
