uniform extern texture Sprite;
sampler sprite = sampler_state
{
    Texture = <Sprite>;
};

uniform extern texture Screen;
sampler screen = sampler_state
{
    Texture = <Screen>;
};

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexToPixel
{
    float4 Position     : POSITION;
    float4 Color        : COLOR0;
	float2 UV			: TEXCOORD0;
	float2 POS			: TEXCOORD1;
};

VertexToPixel VertexShaderFunction(float4 pos : POSITION, float2 uv : TEXCOORD0, float4 color : COLOR)
{
	VertexToPixel output;
	output.Position = mul(pos, World);
	output.Position = mul(output.Position, View);
	output.Position = mul(output.Position, Projection);
	output.Color = color;
	output.UV = uv;
	output.POS = float2(output.Position.x, output.Position.y);
	
    return output;
}

float4 PixelShaderFunction(VertexToPixel input) : COLOR0
{
    float4 col = tex2D(sprite, input.UV);
    return col;
}

technique Test
{
    pass Pass1
    {
		VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
