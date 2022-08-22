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
	output.color = color;
	output.texCoord = texCoord;
	output.outPos = output.position;
	return output;
}

technique BasicTexture
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 SpriteVertexShader();
    }
}