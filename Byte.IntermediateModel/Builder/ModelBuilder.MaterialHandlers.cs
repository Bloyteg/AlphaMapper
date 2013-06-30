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

using System;
using Byte.IntermediateModel.Components;

namespace Byte.IntermediateModel.Builder
{
	public partial class ModelBuilder
	{
        private void PrevalidateMaterial()
        {
            if (_currentMaterial == null)
            {
                throw new InvalidOperationException("Critical error: No current material.");
            }
        }

        public void SetColor(float red, float green, float blue)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateColor(red, green, blue);
        }

        public void SetTexture(string texture, string mask, string bump)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateTexture(texture.Equals("null", StringComparison.InvariantCultureIgnoreCase) ? null : texture, mask, bump);
        }

        public void SetAmbient(float value)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateAmbient(value);
        }

        public void SetDiffuse(float value)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateDiffuse(value);
	    }

        public void SetSpecular(float value)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateSpecular(value);
	    }

	    public void SetSurface(float ambient, float diffuse, float specular)
	    {
	        PrevalidateMaterial();

	        _currentMaterial = _currentMaterial.UpdateSurface(ambient, diffuse, specular);
	    }

	    public void SetOpacity(float opacity)
	    {
	        PrevalidateMaterial();

	        _currentMaterial = _currentMaterial.UpdateOpacity(opacity);
	    }

	    public void SetTextureMode(TextureMode mode)
	    {
	        PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateTextureMode(mode);
	    }

        public void AddTextureMode(TextureMode mode)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.AddTextureMode(mode);
        }

        public void RemoveTextureMode(TextureMode mode)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.RemoveTextureMode(mode);
        }

        public void SetMaterialMode(MaterialMode mode)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateMaterialMode(mode);
	    }

        public void AddMaterialMode(MaterialMode mode)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.AddMaterialMode(mode);
        }

        public void RemoveMaterialMode(MaterialMode mode)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.RemoveMaterialMode(mode);
        }

        public void SetGeometrySampling(GeometrySampling sampling)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateGeometrySampling(sampling);
        }

        public void SetLightSampling(LightSampling sampling)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateLightSampling(sampling);
        }

        public void SetTextureAddressMode(TextureAddressMode mode)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateTextureAddressMode(mode);
        }

        public void SetTextureMipmapState(bool state)
        {
            PrevalidateMaterial();

            _currentMaterial = _currentMaterial.UpdateTextureMipmapState(state);
        }
	}
}
