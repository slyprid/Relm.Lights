#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler2D TextureSampler : register(s0)
{
	Texture = <Texture>;
};

Texture2D LightMap;
sampler LightMapSampler
{
	Texture = <LightMap>;
};

struct PixelShaderOutput
{
	float4 Position : SV_Position;
	float4 Color : COLOR0;
	float2 texCoord : TEXCOORD0;
};

float4 Main(PixelShaderOutput input) : COLOR0
{
	float4 mainColor = tex2D(TextureSampler, input.texCoord);
	float4 lightColor = tex2D(LightMapSampler, input.texCoord);
	return mainColor * lightColor;
}

technique LightMapDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL Main();
	}
};