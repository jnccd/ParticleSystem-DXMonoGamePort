Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float BlurWeights[15];
float2 i_texsize = float2(1/1920.0, 1/1080.0);
bool horz = false;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = float4(0,0,0,0);

	for (int i = 0; i < 15; i++)
	{
		int j = i-7;
		if (BlurWeights[i] == 0)
			BlurWeights[i] = pow(0.9, j*j);

		//float2 coords = float2(input.TextureCoordinates.x + ((float)i - 7) * InvertedTexSize, input.TextureCoordinates.y);
		float2 coords = 0;
		if (horz)
			coords = float2(input.TextureCoordinates.x + i_texsize.x * j, input.TextureCoordinates.y);
		else
			coords = float2(input.TextureCoordinates.x, input.TextureCoordinates.y + i_texsize.y * j);
		color += tex2D(SpriteTextureSampler, coords) * BlurWeights[i];
	}
	color.rgb /= 1.8f;

	//color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	//color += tex2D(SpriteTextureSampler, float2(input.TextureCoordinates.x - 1/1920.0, input.TextureCoordinates.y)) * 0.5;
	//color += tex2D(SpriteTextureSampler, float2(input.TextureCoordinates.x + 1/1920.0, input.TextureCoordinates.y)) * 0.5;

	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile ps_4_0_level_9_3 MainPS();
	}
};