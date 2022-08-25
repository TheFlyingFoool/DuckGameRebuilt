sampler sprite;
float wave;

float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	float breadth = 1.0f;
	float focalLength = 0.045f;

	float ypos = uv.y * 240.0f;
	float scaleX = (breadth + (ypos * focalLength));


	float dist = uv.x;
	dist -= 0.5f;


	uv.x = (scaleX) * dist;

	//uv.x -= (uv.y * 0.1f) * (scaleX - 10.0f);

	//uv.y *= 3;

    float4 color = tex2D(sprite, uv);


    return color;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
