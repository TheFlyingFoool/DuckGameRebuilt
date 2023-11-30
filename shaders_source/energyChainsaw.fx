sampler sprite;
sampler gold;
float width;
float height;

float xpos;
float ypos;
float time;
float glow;
float4 bladeColor;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	float2 offset = float2(0, sin((time * 12) + (uv.x * 12)) * 0.05f) * glow;
	float2 offset2 = float2(offset.y * 0.5f, sin((time * 22) + (uv.x * 18)) * 0.04f);


    float4 col = tex2D(sprite, uv + offset) * c;
	float4 origCol = col;

	float4 col2 = tex2D(sprite, uv + float2(0.02f, 0.02f) + offset2);
	float4 col3 = tex2D(sprite, uv - float2(0.02f, 0.02f) - offset2);
	float4 goldCol = tex2D(gold, float2(((uv.y % width) / width) + (ypos * 0.01f), ((uv.x % height) / height) + (xpos * 0.01f))) * bladeColor;
	col = (col + col2 + col3) / 3;
	col.rgb = (((goldCol.rgb + goldCol.rgb) + (col.rgb)) * col.a) / 2;

	return (col * glow) + (origCol * (1.0f - glow));
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
