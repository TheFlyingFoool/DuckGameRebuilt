sampler sprite;
sampler gold;
float width;
float height;

float xpos;
float ypos;
float intensity;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
    float4 col = tex2D(sprite, uv);
	float4 goldCol = tex2D(gold, float2(((uv.x % width) / width) + (xpos * 0.01f),( (uv.y % height) / height) + (ypos * 0.01f)));

	float border = 1.0f - floor(1.0 - (col.r * col.g * col.b));


	float4 newCol = col;
	newCol.rgb = (newCol.r + newCol.g + newCol.b) / 3;
	newCol.rgb = ((newCol.rgb) * ((goldCol.rgb * 2.3f))) * border;
	newCol.r *= 1.1f;

	col = (col * (1.0f - intensity)) + (newCol * intensity);
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
