sampler sprite;
sampler blur;
float screenWidth = 134;
float screenHeight = 86;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0, float4 position : TEXCOORD2) : COLOR0
{
    float4 col = tex2D(sprite, uv) * c;
	float4 blurVal = tex2D(blur, uv);
	col =  col * 0.7f + (blurVal * 0.6f);

	float widePixel = round(((position.x + 1) * 134) / 0.5) * 0.5;
	float tallPixel = round(((position.y + 1) * 86) / 0.5) * 0.5;

	col.rgb *= 0.9f + (abs(widePixel % 2) * 0.1f);
	col.rgb *= 0.9f + (abs(tallPixel % 2) * 0.1f);

	col.a *= 0.7f + (abs(widePixel % 2) * 0.3f);
	col.a *= 0.7f + (abs(tallPixel % 2) * 0.3f);

    return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
