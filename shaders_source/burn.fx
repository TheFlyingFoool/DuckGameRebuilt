sampler sprite;
sampler sprite2;
float width;
float height;
float burn;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
    float4 col = tex2D(sprite, uv);
	//float4 col2 = tex2D(sprite2, float2((uv.x * 1.8f) + col.r + col.g, (uv.y * 1.7f) + col.g + col.b - (col.r * 0.5)));

	float4 col2 = tex2D(sprite2, float2(((uv.x % width) / width) + col.r + col.g, ((uv.y % height) / height) + col.g + col.b - (col.r * 0.5)));
	//col.rgb = (col.r + col.g + col.b) / 3;
	float invBurn = 1.0f - burn;
	col = col * (float4(1 - ( ((1 - col2.r) * burn)),   1 - ( ((1 - col2.g) * burn)),   1 - ( ((1 - col2.b) * burn)), 1.0f));

    return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
