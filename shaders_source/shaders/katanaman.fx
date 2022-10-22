sampler sprite;
sampler edge;
float xpos;
float ypos;
float light1x;
float light1y;
float light2x;
float light2y;
float light3x;
float light3y;
float flipSub = 0.0f;
float3 fade = 1.0f;
float3 add = 0.0f;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	float worldSize = 100;
    float4 col = tex2D(sprite, uv);
	float4 edgeCol = tex2D(edge, uv);

	float2 pixelOffset = float2(((flipSub - uv.x) * flipSub) + (uv.x * (1.0f - flipSub)), uv.y) * 64;
	float2 pos = (float2(xpos, ypos) + pixelOffset);

	float4 lightingColor1 = float4(1.0f, 0.0f, 0.0f, 1.0f);
	float2 lightPos1 = float2(light1x, light1y);

	float4 lightingColor2 = float4(0.9f, 0.6f, 0.2f, 1.0f);
	float2 lightPos2 = float2(light2x, light2y);

	float4 lightingColor3 = float4(0, 0, 1.0f, 1.0f);
	float2 lightPos3 = float2(light3x, light3y);

	edgeCol.r *= 0.75f;

	float dist1 = 1.0f - min(length(pos - lightPos1) * 0.01f, 1.0f);
	float dist2 = 1.0f - min(length(pos - lightPos2) * 0.01f, 1.0f);
	float dist3 = 1.0f - min(length(pos - lightPos3) * 0.01f, 1.0f);

	//dist1 = 0;
	//dist2 = 0;
	//dist3 = 0;

	dist1 = min((dist1 + 0.15f) * (dist1 + 0.15f), 1);
	dist2 = min((dist2 + 0.15f) * (dist2 + 0.15f), 1);
	dist3 = min((dist3 + 0.15f) * (dist3 + 0.15f), 1);

	edgeCol.r *= max(max(dist1, dist2), dist3);

	float3 finalCol = (col.rgb * (1.0f - edgeCol.r)) + ((lightingColor1.rgb * dist1) * edgeCol.r) + ((lightingColor2.rgb * dist2) * edgeCol.r) + ((lightingColor3.rgb * dist3) * edgeCol.r);
	finalCol *= fade;
	finalCol += add;
	return float4(finalCol.r, finalCol.g, finalCol.b, col.a);
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
