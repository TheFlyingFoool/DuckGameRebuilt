sampler sprite;
sampler overlay;
float screenWidth = 185;
float screenHeight = 103;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0, float4 position : TEXCOORD2) : COLOR0
{
    float4 col = tex2D(sprite, uv) * c;
	float4 overColor = tex2D(overlay, uv);


	////float widePixel = round(((position.x) * screenWidth) / 0.5) * 0.5;
	////float tallPixel = round(((position.y) * screenHeight) / 0.5) * 0.5;

	//float widePixel = position.x * screenWidth;
	//float tallPixel = position.y * screenHeight;

	//col.rgb *= 0.5f + (abs(widePixel % 1) * 0.5f);
	//col.rgb *= 0.5f + (abs(tallPixel % 1) * 0.5f);

	////col.a *= 0.7f + (abs(widePixel % 1) * 0.3f);
	////col.a *= 0.7f + (abs(tallPixel % 1) * 0.3f);



	return ((col * 0.8f) * overColor) + ((col * overColor) * 0.25f);

	//col =  col * 0.7f;

	////float widePixel = round(((position.x) * screenWidth) / 0.5) * 0.5;
	////float tallPixel = round(((position.y) * screenHeight) / 0.5) * 0.5;

	//float widePixel = position.x * screenWidth;
	//float tallPixel = position.y * screenHeight;

	//col.rgb *= 0.5f + (abs(widePixel % 1) * 0.5f);
	//col.rgb *= 0.5f + (abs(tallPixel % 1) * 0.5f);

	////col.a *= 0.7f + (abs(widePixel % 1) * 0.3f);
	////col.a *= 0.7f + (abs(tallPixel % 1) * 0.3f);

 //   return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
