sampler sprite;

struct VertexToPixel
{
    float4 Color        : COLOR0;
	float2 UV			: TEXCOORD0;
	float4 Position     :TEXCOORD2;
};

float scan = 0;
float secondScan = 0;
float center = 0.5;
float4 PixelShaderFunction(VertexToPixel IN) : COLOR0
{
	float screenCross = 1.0;
	float screenMul = 0.5;

	float4 color = tex2D(sprite, IN.UV);
	float4 texCol = color;


	float red = floor(1.0 - (color.r * color.g * color.b));
	float blu = floor(((((0.5 + 1.0) * 20) % 1) + 0.1));	
	blu += floor(((((0.5 + 1.0) * 32) % 1) + 0.1));

	float spos = ((0.5 + 1.0) / 2);
	float s = floor(spos + 0.5);


	color.b += ((max(1.0 - (min(abs(scan), 0.2) * 10), 0)) * 1.0) * color.a;

	color = (float4(red * color.a, blu * color.a, 0.0, color.a) * s) + (color * (1.0 - s));


	color *= min(max(0, (IN.Position.x - scan) * 10000), 1);


	color += (1.0 - min(max(0, (IN.Position.x - scan) * 10000), 1)) * (texCol * secondScan);

	//color *= IN.Position;


	return color * IN.Color;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
