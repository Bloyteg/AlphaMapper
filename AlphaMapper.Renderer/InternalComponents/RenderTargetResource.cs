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
using SlimDX.Direct3D11;

namespace AlphaMapper.Renderer.InternalComponents
{
    class RenderTargetResource : IDisposable
    {
        private readonly Texture2D _texture;
        private readonly RenderTargetView _renderTargetView;
        private readonly ShaderResourceView _shaderResourceView;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetResource"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="description">The description.</param>
        public RenderTargetResource(Device device, Texture2DDescription description)
        {
            _texture = new Texture2D(device, description);
            _renderTargetView = new RenderTargetView(device, _texture);
            _shaderResourceView = new ShaderResourceView(device, _texture);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _renderTargetView.Dispose();
            _shaderResourceView.Dispose();
            _texture.Dispose();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="AlphaMapper.Renderer.InternalComponents.RenderTargetResource"/> to <see cref="SlimDX.Direct3D11.Texture2D"/>.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Texture2D(RenderTargetResource resource)
        {
            return resource._texture;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="AlphaMapper.Renderer.InternalComponents.RenderTargetResource"/> to <see cref="SlimDX.Direct3D11.RenderTargetView"/>.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator RenderTargetView(RenderTargetResource resource)
        {
            return resource._renderTargetView;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="AlphaMapper.Renderer.InternalComponents.RenderTargetResource"/> to <see cref="SlimDX.Direct3D11.ShaderResourceView"/>.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ShaderResourceView(RenderTargetResource resource)
        {
            return resource._shaderResourceView;
        }
    }
}
