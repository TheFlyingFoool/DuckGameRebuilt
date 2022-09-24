sampler sprite;
sampler gold;
float width;
float height;
//float widthinc;
float frameWidth;

float xpos;
float ypos;
float amount;
float yoff;


float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	float2 newv = (float2((uv.x % (width * 2)) / (width * 2), (uv.y % (height * 2)) / (height * 2))) + float2(yoff * 0.1f, yoff);
	newv.x = round(newv.x * frameWidth) / frameWidth;
	newv.y = round(newv.y * frameWidth) / frameWidth;

	float4 offset = tex2D(gold, newv);
	//float4 col = tex2D(sprite, uv + float2(((offset.r * ((xpos % 1.0f) * 0.01f)) * 0.2f), ((offset.b * ((xpos % 1.0f) * 0.01f)) * 0.1f)));

	float2 realOffset = float2(((offset.r * 0.3f) * width) - ((offset.b * 0.3f) * width), 0) * amount;

	//realOffset.x = round(realOffset.x * frameWidth) / frameWidth;

	float4 col = tex2D(sprite, uv + realOffset);

	return col;




    /*float4 col = tex2D(sprite, uv);
	float4 goldCol = tex2D(gold, float2(((uv.x % width) / width) + (xpos * 0.01f),( (uv.y % height) / height) + (ypos * 0.01f)));

	float border = 1.0f - floor(1.0 - (col.r * col.g * col.b));

	col.rgb = (col.r + col.g + col.b) / 3;
	col.rgb = ((col.rgb) * ((goldCol.rgb * 2.3f))) * border;
	col *= c;
	return col;*/
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
