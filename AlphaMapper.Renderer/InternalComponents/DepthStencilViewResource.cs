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
using SharpDX.Direct3D11;

namespace AlphaMapper.Renderer.InternalComponents
{
    class DepthStencilViewResource : IDisposable
    {
        private readonly Texture2D _texture;
        private readonly DepthStencilView _depthStencilView;
        private readonly ShaderResourceView _shaderResourceView;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTargetResource"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="description">The description.</param>
        /// <param name="depthStencilViewDescription">The depth stencil view description.</param>
        /// <param name="shaderResourceViewDescription">The shader resource view description.</param>
        public DepthStencilViewResource(Device device, Texture2DDescription description, DepthStencilViewDescription depthStencilViewDescription, ShaderResourceViewDescription shaderResourceViewDescription)
        {
            _texture = new Texture2D(device, description);
            _depthStencilView = new DepthStencilView(device, _texture, depthStencilViewDescription);
            _shaderResourceView = new ShaderResourceView(device, _texture, shaderResourceViewDescription);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _depthStencilView.Dispose();
            _shaderResourceView.Dispose();
            _texture.Dispose();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="AlphaMapper.Renderer.InternalComponents.RenderTargetResource"/> to <see cref="SharpDX.Direct3D11.Texture2D"/>.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Texture2D(DepthStencilViewResource resource)
        {
            return resource._texture;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="AlphaMapper.Renderer.InternalComponents.RenderTargetResource"/> to <see cref="SharpDX.Direct3D11.RenderTargetView"/>.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator DepthStencilView(DepthStencilViewResource resource)
        {
            return resource._depthStencilView;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="AlphaMapper.Renderer.InternalComponents.RenderTargetResource"/> to <see cref="SharpDX.Direct3D11.ShaderResourceView"/>.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ShaderResourceView(DepthStencilViewResource resource)
        {
            return resource._shaderResourceView;
        }
    }
}
