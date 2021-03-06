// Copyright 2013 Joshua R. Rodgers
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================

#include "effect_light.fx"

//Light
Light g_GlobalLight;

float4x4 g_LightViewProjectionMatrix;

//Constant buffers
cbuffer cbPerObject
{
    float4x4 g_WorldMatrix;
	bool g_IsInstanced = false;
};

cbuffer cbPerMove
{
	float4x4 g_ViewMatrix;
};

cbuffer cbPerResize
{
	float4x4 g_PostWorldMatrix;
	float4x4 g_ProjectionMatrix;
};

cbuffer cbPerMaterial
{
    float4 g_Color;
	float g_Ambient;
	float g_Diffuse;
	float g_Opacity;
	bool g_HasTexture;
	bool g_HasMask;
	bool g_IsColorTinted;
};

//Texture sampler
SamplerState g_TextureSampler
{
	Filter = ANISOTROPIC;
	AddressU = WRAP;
	AddressV = WRAP;
};

//Texture and mask
Texture2D g_Texture;
Texture2D g_Mask;
