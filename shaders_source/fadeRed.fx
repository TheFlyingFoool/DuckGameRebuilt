sampler sprite;

float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{    
	float4 col = tex2D(sprite, uv) * c;
	
	col.r = 1 * col.a;
	col.g = 0;
	col.b = 0;
    
    return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}