sampler sprite;
sampler gold;
float width;
float height;

float xpos;
float ypos;


float2 sasize; //= float2(4409, 4408);


float xoffset;
float yoffset;
float spritesizex;
float spritesizey;

// these are not / width or height ill come back to clean this up for sure mabye
float goldxoffset;
float goldyoffset;
float goldsizex;
float goldsizey;

float4 sampleAtlasWrap(float2 uv, float2 TexOffset, float2 TexSize)
{
    float2 scale = TexSize / sasize;
    TexOffset /= sasize;
    //uv = (uv + 0.002) % 0.998;
    //uv = frac(uv - 0.001);
   // uv = clamp(uv, float2(0.002, 0.002),);
    uv.y = clamp(uv.y, 0.001f, 0.999f);
    uv.x = clamp(uv.x, 0.001f, 0.999f);
    uv = frac(uv);
    uv *= scale.yx;
    uv += TexOffset;
   // uv.y = clamp(uv.y, 0.0f, spritesizey + yoffset);
   // uv.x = clamp(uv.x, 0.0f, spritesizex + xoffset);
    return tex2D(sprite, uv);
}
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
    float2 realuv = (uv - float2(xoffset, yoffset)) / float2(spritesizex, spritesizey);
    // float4 col = sampleAtlasWrap(realuv + offset, float2(xoffset * sasize.x, yoffset * sasize.y), float2(spritesizey * sasize.y, spritesizex * sasize.x)) * c; // float2(xoffset * sasize.x, yoffset * sasize.y),
    float4 col = sampleAtlasWrap(realuv, float2(xoffset * sasize.x, yoffset * sasize.y), float2(spritesizey * sasize.y, spritesizex * sasize.x)); // float2(xoffset * sasize.x, yoffset * sasize.y),
    //float4 col = tex2D(sprite, uv);
    
    float4 goldCol = sampleAtlasWrap(float2(((uv.x % width) / width) + (xpos * 0.01f), ((uv.y % height) / height) + (ypos * 0.01f)), float2(goldxoffset, goldyoffset), float2(goldsizey, goldsizex));
	//float4 goldCol = tex2D(gold, float2(((uv.x % width) / width) + (xpos * 0.01f),( (uv.y % height) / height) + (ypos * 0.01f)));

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
