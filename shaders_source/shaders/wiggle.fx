sampler sprite;

float xpos;
float ypos;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
    float4 col = tex2D(sprite, uv + float2(0, sin((uv.x * 12) + (xpos * 0.32f)) * 0.07f));
	return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
