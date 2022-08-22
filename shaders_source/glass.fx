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

float width;
float height;
float scale;

struct VertexToPixel
{
    float4 Position     : POSITION;
    float4 Color        : COLOR0;
	float2 UV			: TEXCOORD0;
	float4 POS			: TEXCOORD1;
};

VertexToPixel VertexShaderFunction(float4 pos : POSITION, float2 uv : TEXCOORD0, float4 color : COLOR)
{
	VertexToPixel output;

	output.Position = mul(pos, World);
	output.Position = mul(output.Position, View);
	output.Position = mul(output.Position, Projection);
	float4 trans = mul(output.Position, View);
	trans = mul(trans, Projection);

	output.Color = color;
	output.UV = uv;
	output.POS = output.Position;
	
    return output;
}

float4 PixelShaderFunction(VertexToPixel input) : COLOR0
{
	float2 pos = input.POS.xy / input.POS.w;
	float wTrans = ((width* scale) * 4.0f);



	pos.x = (pos.x + 1.0f) / 2.0f;
	pos.y = 1.0f - ((pos.y + 1.0f) / 2.0f);

	//pos /= input.POS.w;

    //float2 vNormalizedScreenPos = pos /= float2(1280.0f, 700.0f);
    //float2 vTexCoord = vNormalizedScreenPos + (0.5f / float2(1280.0f, 700.0f));  


	float4 col = tex2D(sprite, input.UV);
    /*float4 sCol = tex2D(screen, pos - float2(((scale * (width * 2.0f)) * input.UV.x), 0.0f));
	float4 sCol2 = tex2D(screen, pos + float2(((scale * (width * 2.0f)) * (1.0f - input.UV.x)), 0.0f));

	sCol *= (1.0f - input.UV.x);
	sCol2 *= input.UV.x;*/

	float v = (4.0f * 16.0f) / 128.0f;


	float uv = (input.UV.y - v) * 8.0f;



	//float4 sCol = tex2D(screen, pos - float2(0.0f,((scale * (height)) * uv)));
	float4 sCol = tex2D(screen, pos - float2(0.0f, (scale * height) * uv) - float2(0.0f,(scale * (height)) * uv) - float2(0.0f,(scale * (height)) * uv)  - float2(0.0f,(scale * (height)) * uv));
	//float4 sCol2 = tex2D(screen, pos + float2(0.0f,((scale * (height * 2.0f)) * (1.0f - uv))));

	sCol += (uv * 2.0f);
	//sCol2 *= input.uv;

	sCol.r += col.r * 0.0001f;

	if(sCol.r > 1.0f)
		sCol.r = 1.0f;
		if(sCol.g > 1.0f)
		sCol.g = 1.0f;
			if(sCol.b > 1.0f)
		sCol.b = 1.0f;
	
	//col.a == fun.a;
	//if(col.a < 0.5f)
	//	clip();
	//col.xyz = ((sCol + sCol2) * 0.5f).xyz;


	col.xyz *= sCol.xyz;

    return col * input.Color;
}

/*float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float4 col = float4(1.0f, 0.0f, 0.0f, 1.0f);//tex2D(sprite, input.UV);
    return col;
}*/

technique Test
{
    pass Pass1
    {
		VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
