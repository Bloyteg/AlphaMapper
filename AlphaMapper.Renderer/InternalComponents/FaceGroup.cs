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
using SharpDX.Direct3D11;
using Byte.Utility;

namespace AlphaMapper.Renderer.InternalComponents
{
    internal class FaceGroup : IDisposable
    {
        public IndexBuffer IndexBuffer { get; set; }
        public Material Material { get; set; }
        public Byte.Utility.WeakReference<ShaderResourceView> Texture { get; set; }
        public Byte.Utility.WeakReference<ShaderResourceView> Mask { get; set; }

        public void Dispose()
        {
            if(IndexBuffer != null)
            {
                IndexBuffer.Buffer.Dispose();
            }
        }
    }
}
