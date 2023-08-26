sampler sprite;

float uvL;
float uvR;
float uvT;
float uvB;
float gL;
float gR;
float gT;
float gB;
float mult;
float time;

float4 PixelShaderFunction(float2 uv: TEXCOORD0, float4 c: COLOR0) : COLOR0
{
    if (uv.x < uvL || uv.x > uvR || uv.y > uvT || uv.y < uvB) return tex2D(sprite, uv) * c;
        
    float uvWidth = uvR - uvL;
    float uvHeight = uvB - uvT;

    float gameX = (uv.x - uvL) / uvWidth * (gR - gL) + gL;
    float gameY = (uv.y - uvT) / uvHeight * (gB - gT) + gT;
    
    float2 boundsUv = float2((gameX - gL) / (gR - gL), (gameY - gT) / (gB - gT));
    
    float roundness = boundsUv.y > 0.35 ? 0.3 : 0.;
    float strength = 1. - smoothstep(0., 0.3, length(max(abs(boundsUv - float2(0.5, 0.35)) - float2(0.35, 0.35) + roundness, 0.)) - roundness);


    float xOff = sin(time + gameX * 0.3 + cos(gameY * 0.3) * 2.) * 1e-2;
    float yOff = cos(-time + gameY * 0.3 + sin(gameX * 0.3) * 2.) * 1e-2;

    float perPixel = (uvWidth / (gR - gL)) * 100.;

    uv += (float2(xOff, yOff) * strength * perPixel) * mult;

    return tex2D(sprite, uv) * c;
}

technique Test
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}