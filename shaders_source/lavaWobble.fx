sampler sprite;


float uvL;
float uvR;
float uvT;
float uvB;
float gL;
float gR;
float gT;
float gB;

float time;
float glow;
float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
	//ignore how overly complicated this HLSL code please, or dont -NiK0
	bool inRect = uv.x >= uvL && uv.x <= uvR && uv.y <= uvT && uv.y >= uvB;
	if (inRect)
	{
		float uvWidth = uvR - uvL;
		float uvHeight = uvB - uvT;

		float gameX = (uv.x - uvL) / uvWidth * (gR - gL) + gL;
		float gameY = (uv.y - uvT) / uvHeight * (gB - gT) + gT;
	
		float2 fUv = float2((gameX - gL) / (gR - gL), (gameY - gT) / (gB - gT));
	
		float center = (gT + gB) / 2.0; // Calculate the center of the range
		float distanceFromCenter = abs(gameY - center); // Calculate the distance from the center
		float maxDistance = (gB - gT) / 2.0; // Calculate the maximum distance from the center

		float xMultiplier = saturate(1.0 - smoothstep(0.0, 1.0, distanceFromCenter / maxDistance));
		
		center = (gL + gR) / 2.0; // Calculate the center of the range
		distanceFromCenter = abs(gameX - center); // Calculate the distance from the center
		maxDistance = (gR - gL) / 2.0; // Calculate the maximum distance from the center

		float yMultiplier = saturate(1.0 - smoothstep(0.0, 1.0, distanceFromCenter / maxDistance));
		
		float2 offset = float2((sin((time * 12) + (fUv.y * 12)) * 0.05f) * xMultiplier * uvWidth * yMultiplier, (cos((time * 8) + (fUv.x * 10)) * 0.05f) * xMultiplier * uvHeight * yMultiplier) * glow;

		float4 col = tex2D(sprite, uv + offset) * c;

		float4 origCol = col;

		return (col * glow) + (origCol * (1.0f - glow));
	}
	return tex2D(sprite, uv) * c;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
