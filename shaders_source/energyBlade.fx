sampler sprite;
sampler gold;
float width;
float height;

float xpos;
float ypos;
float time;
float glow;
float4 bladeColor;









float xoffset;
float yoffset;
float spritesizex;
float spritesizey;
float goldxoffset;
float goldyoffset;
float goldsizex;
float goldsizey;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	
	
    //float4 col = tex2D(sprite, uv);

    float2 realuv = (uv - float2(xoffset, yoffset)) / float2(spritesizex, spritesizey);
    realuv = frac(realuv + float2(xpos, ypos) * 0.01f);
    float2 golduv = realuv * float2(goldsizex, goldsizey) + float2(goldxoffset, goldyoffset);

    float4 goldCol = tex2D(sprite, golduv) * bladeColor;
	
    float2 offset = float2(sin((time * 12) + (realuv.y * 12)) * 0.05f * spritesizey, 0) * glow;
    float2 offset2 = float2(sin((time * 22) + (realuv.y * 18)) * 0.04f * spritesizey, offset.x * 0.5f);
    float2 off = float2(0.02f, 0.02f) * spritesizex;

    float4 col = tex2D(sprite, uv + offset) * c;
    return col;
	float4 origCol = col;
    float4 col2 = tex2D(sprite, uv + off + offset2);
    float4 col3 = tex2D(sprite, uv - off - offset2);
	//float4 goldCol = tex2D(gold, float2(((uv.x % width) / width) + (xpos * 0.01f), ((uv.y % height) / height) + (ypos * 0.01f))) * bladeColor;
	col = (col + col2 + col3) / 3;
	col.rgb = (((goldCol.rgb + goldCol.rgb) + (col.rgb)) * col.a) / 2;

	//return (col * glow) + (origCol * (1.0f - glow));
    return col;

}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
