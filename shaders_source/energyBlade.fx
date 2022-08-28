sampler sprite;
float width;
float height;

float xpos;
float ypos;
float time;
float4 bladeColor;
float glow;
float2 sasize;//= float2(4409, 4408);


float xoffset;
float yoffset;
float spritesizex;
float spritesizey;

// these are not / width or height ill come back to clean this up for sure mabye
float goldxoffset;
float goldyoffset;
float goldsizex;
float goldsizey;

//float4 sampleAtlasWrap(float2 uv, float2 TexOffset, float2 TexSize)
//{
//    float2 scale = TexSize / sasize;
//    TexOffset /= sasize;
//    uv = frac(uv);
//    uv *= scale.yx;
//    uv += TexOffset;
//    return tex2D(sprite, uv);
//}
float4 sampleAtlasWrap(float2 uv, float2 TexOffset, float2 TexSize)
{
    float2 scale = TexSize / sasize;
    TexOffset /= sasize;
    //uv = (uv + 0.002) % 0.998;
    //uv = frac(uv - 0.001);
   // uv = clamp(uv, float2(0.002, 0.002),);
    //uv.y = clamp(uv.y, 0.001f, 0.999f);
    //uv.x = clamp(uv.x, 0.001f, 0.999f);
    uv = frac(uv);
    uv *= scale.yx;
    uv += TexOffset;
   // uv.y = clamp(uv.y, 0.0f, spritesizey + yoffset);
   // uv.x = clamp(uv.x, 0.0f, spritesizex + xoffset);
    return tex2D(sprite, uv);
}

float4 PixelShaderFunction(float2 uv : TEXCOORD0, float4 c : COLOR0) : COLOR0 //(out float4 fragColor, in float2 fragCoord)
{

    float2 realuv = (uv - float2(xoffset, yoffset)) / float2(spritesizex, spritesizey);
    float4 bladeColor2 = bladeColor; //  float2(spritesizey * sasize.y, spritesizex * sasize.x);
    float2 offset = float2(sin((time * 12.0f) + (realuv.y * 12.0f)) * 0.05f, 0) * glow;
    float2 offset2 = float2(sin((time * 22.0f) + (realuv.y * 18.0f)) * 0.04f, offset.x * 0.5f);
    float4 col = sampleAtlasWrap(realuv + offset, float2(xoffset * sasize.x, yoffset * sasize.y), float2(spritesizey * sasize.y, spritesizex * sasize.x)) * c; // float2(xoffset * sasize.x, yoffset * sasize.y),
    float4 origCol = col;
    float4 col2 = sampleAtlasWrap(realuv + float2(0.02f, 0.02f) + offset2, float2(xoffset * sasize.x, yoffset * sasize.y), float2(spritesizey * sasize.y, spritesizex * sasize.x));
    float4 col3 = sampleAtlasWrap(realuv - float2(0.02f, 0.02f) - offset2, float2(xoffset * sasize.x, yoffset * sasize.y), float2(spritesizey * sasize.y, spritesizex * sasize.x));
    float2 goldUv = float2((((realuv.x % width)) / width) + (xpos * 0.01f), (((realuv.y % width)) / height) + (ypos * 0.01f));
    float4 goldCol = sampleAtlasWrap(goldUv, float2(goldxoffset, goldyoffset), float2(goldsizey, goldsizex)) * bladeColor2;
    col = (col + col2 + col3) / 3.0f;
    col.rgb = (((goldCol.rgb + goldCol.rgb) + (col.rgb)) * col.a) / 2.0f;
    return (col * glow) + (origCol * (1.0f - glow));;
    
}
technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

