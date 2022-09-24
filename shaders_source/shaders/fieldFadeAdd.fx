uniform extern texture Sprite;
sampler sprite = sampler_state
{
    Texture = <Sprite>;
};

float3 fade = 1.0f;
float3 add = 0.0f;
struct VertexToPixel
{
    float4 Position     : POSITION;
    float4 Color        : COLOR0;
	float2 UV			: TEXCOORD0;
};

float4x4 WVP;
VertexToPixel SpriteVertexShader(	float4 position	: SV_Position,
										float4 color	: COLOR0,
										float2 texCoord	: TEXCOORD0 )
{
	VertexToPixel output;
	output.Position = mul(position, WVP);

	output.Color = color;
	output.UV = texCoord;
	return output;
}

float4 PixelShaderFunction(VertexToPixel input) : COLOR0
{
	float4 color = tex2D(sprite, input.UV) * input.Color;
	color.rgb *= fade;
	color.rgb += add;
	color.rgb *= color.a;
	return color;
}

technique BasicTexture
{
    pass Pass1
    {
		VertexShader = compile vs_2_0 SpriteVertexShader();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}