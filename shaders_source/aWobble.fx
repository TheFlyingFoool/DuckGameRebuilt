sampler sprite;
sampler gold;
float width;
float height;

float xpos;
float ypos;
float time;
float glow;
float multi;
float4 tColor;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	float2 offset = float2(sin((time * 12) + (uv.y * 12)) * multi, sin((time * 12) + (uv.x * 12)) * multi) * glow;

    float4 col = tex2D(sprite, uv + offset) * c;
	float4 origCol = col;

	float4 goldCol = tex2D(gold, float2(((uv.x % width) / width) + (xpos * 0.01f), ((uv.y % height) / height) + (ypos * 0.01f))) * tColor;
	col = col / 3;
	col.rgb = (((goldCol.rgb + goldCol.rgb) + (col.rgb)) * col.a) / 2;

	return (col * glow) + origCol;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
