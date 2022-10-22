sampler sprite;
sampler sprite2;
float fade;
float dim;
float scrollX;
float scrollY;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
    float4 col = tex2D(sprite, uv) * (1.0f - fade);
	float4 col2 = tex2D(sprite2, ((uv * 0.75f) + float2(0.0f, 0.25f)) + float2(scrollX, scrollY));

	//float4 col2 = tex2D(sprite2, float2(((uv.x % width) / width) + col.r + col.g, ((uv.y % height) / height) + col.g + col.b - (col.r * 0.5)));
	//col.rgb = (col.r + col.g + col.b) / 3;
	//float invBurn = 1.0f - burn;
	//col = col * (float4(1 - ( ((1 - col2.r) * burn)),   1 - ( ((1 - col2.g) * burn)),   1 - ( ((1 - col2.b) * burn)), 1.0f));

    return (col + (col2 * fade)) * dim;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
