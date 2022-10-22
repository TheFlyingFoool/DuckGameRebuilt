sampler sprite;
float3 fade = 1.0f;
float3 add = 0.0f;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	float blurSize = 0.002f;

    /*float4 col = (tex2D(sprite, uv) + 
		tex2D(sprite, float2(uv.x - blurSize, uv.y + blurSize)) +
		tex2D(sprite, float2(uv.x + blurSize, uv.y - blurSize)) +
		tex2D(sprite, float2(uv.x - blurSize, uv.y - blurSize)) +
		tex2D(sprite, float2(uv.x + blurSize, uv.y + blurSize))) / 5;
		*/

	float4 sum = float4(0.0, 0.0, 0.0, 0.0);
	sum += tex2D(sprite, float2(uv.x, uv.y - 4.0*blurSize)) * 0.05;
	sum += tex2D(sprite, float2(uv.x, uv.y - 3.0*blurSize)) * 0.09;
	sum += tex2D(sprite, float2(uv.x, uv.y - 2.0*blurSize)) * 0.12;
	sum += tex2D(sprite, float2(uv.x, uv.y - blurSize)) * 0.15;
	sum += tex2D(sprite, float2(uv.x, uv.y)) * 0.16;
	sum += tex2D(sprite, float2(uv.x, uv.y + blurSize)) * 0.15;
	sum += tex2D(sprite, float2(uv.x, uv.y + 2.0*blurSize)) * 0.12;
	sum += tex2D(sprite, float2(uv.x, uv.y + 3.0*blurSize)) * 0.09;
	sum += tex2D(sprite, float2(uv.x, uv.y + 4.0*blurSize)) * 0.05;

	sum.rgb *= fade;
	sum.rgb += add;

    return sum;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
