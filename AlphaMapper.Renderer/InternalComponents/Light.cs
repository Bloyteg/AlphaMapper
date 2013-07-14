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

using System.Runtime.InteropServices;
using SharpDX;

namespace AlphaMapper.Renderer.InternalComponents
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Light
    {
        [FieldOffset(0)]
        private readonly Vector3 _direction;
        [FieldOffset(16)]
        private readonly Color4 _ambient;
        [FieldOffset(32)]
        private readonly Color4 _diffuse;

        public Light(Vector3 direction, Color4 ambient, Color4 diffuse) : this()
        {
            _direction = direction;
            _ambient = ambient;
            _diffuse = diffuse;
        }

        public Vector3 Direction
        {
            get { return _direction; }
        }

        public Color4 Ambient
        {
            get { return _ambient; }
        }

        public Color4 Diffuse
        {
            get { return _diffuse; }
        }
    }
}
