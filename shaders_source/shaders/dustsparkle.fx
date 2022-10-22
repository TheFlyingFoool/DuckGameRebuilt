sampler sprite;
sampler sprite2;
float2 topLeft;
float2 size;
float fade;
float4x4 viewMatrix;
float4x4 projMatrix;

struct VSOutput
{
	float4 position		: SV_Position;
	float4 color		    : COLOR0;
    float2 texCoord		: TEXCOORD0;
	float4 outPos		    : TEXCOORD2;
};

VSOutput SpriteVertexShader(	float4 position	: SV_Position,
								float4 color	: COLOR0,
								float2 texCoord	: TEXCOORD0)
{
	VSOutput output;
    output.position = position;
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projMatrix);

	output.color = color;
	output.texCoord = texCoord;
	output.outPos = position;
	return output;
}

struct VertexToPixel
{
    float4 Color        : COLOR0;
	float2 UV			: TEXCOORD0;
	float4 Position     :TEXCOORD2;
};

float4 PixelShaderFunction(VertexToPixel IN) : COLOR0
{
    float4 col = tex2D(sprite, IN.UV);
	float4 cone = tex2D(sprite2, float2((IN.Position.x - topLeft.x) / size.x, (IN.Position.y - topLeft.y) / size.y));
	cone.a = cone.r;

	return (col * 0.0001f) + (cone * IN.Color) * fade;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
		VertexShader = compile vs_2_0 SpriteVertexShader();
    }
}
