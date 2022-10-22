sampler sprite;
sampler sprite2;
sampler sprite3;
float offset;
float offset2;
float scroll;
float scroll2;
float gradientOffset;
float gradientOffset2;
float4 color1;
float4 color2;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	uv *= 2;
    float4 col = tex2D(sprite, uv + float2(((sin((uv.y + scroll) * 40) * 0.05f) * offset) -scroll2, -scroll));
	float4 col2 = tex2D(sprite3, uv + float2(((sin((( uv.y) - scroll2) * 40) * 0.05f) * offset2) +scroll, scroll2));
	float4 gradient = tex2D(sprite2, uv + float2(0, gradientOffset)) * color2;
	float4 gradient2 = tex2D(sprite2, uv + float2(0, gradientOffset2)) * color1;

	col *= col2;

	col *= float4(gradient2.r, gradient2.g, gradient2.b, gradient2.a);
	float inv = 1.0f - col.a;
	col += float4(gradient.r * inv, gradient.g * inv, gradient.b * inv, inv);
    return col;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
