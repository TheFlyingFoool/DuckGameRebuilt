sampler sprite;
float intensity;

float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
    float4 col = tex2D(sprite, uv);

    // Define the green color
    float4 green = float4(0.0, 1.0, 0.0, 1.0);

    // Interpolate only the RGB components between the original color and green
    col.rgb = lerp(col.rgb, green.rgb, intensity);

    return col * col.a;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
