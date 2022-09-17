sampler sprite;
sampler gold;
float width;
float height;

float xpos;
float ypos;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
    float4 col = tex2D(sprite, uv);
	float4 goldCol = tex2D(gold, float2(((uv.x % width) / width) + (xpos * 0.01f),( (uv.y % height) / height) + (ypos * 0.01f)));

	float border = 1.0f - floor(1.0 - (col.r * col.g * col.b));

	col.rgb = (col.r + col.g + col.b) / 3;
	col.rgb = ((col.rgb) * ((goldCol.rgb * 2.3f))) * border;
	col *= c;
	return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
