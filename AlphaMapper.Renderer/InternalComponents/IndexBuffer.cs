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
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace AlphaMapper.Renderer.InternalComponents
{
    internal class IndexBuffer
    {
        public IndexBuffer(Device device, ushort[] indices)
        {
            if(device == null)
            {
                throw new ArgumentNullException("device");    
            }

            if(indices == null)
            {
                throw new ArgumentNullException("indices");
            }

            using(var dataStream = new DataStream(sizeof(UInt16)*indices.Length, true, true))
            {
                dataStream.WriteRange(indices);
                dataStream.Position = 0;

                Buffer = new Buffer(device,
                                    dataStream,
                                    (int) dataStream.Length,
                                    ResourceUsage.Immutable,
                                    BindFlags.IndexBuffer,
                                    CpuAccessFlags.None,
                                    ResourceOptionFlags.None,
                                    0);
            }

            Count = indices.Length;
        }

        public Buffer Buffer { get; private set; }
        public int Count { get; private set; }
    }
}
