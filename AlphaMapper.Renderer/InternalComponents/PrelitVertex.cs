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
    [StructLayout(LayoutKind.Sequential)]
    public struct PrelitVertex
    {
        private readonly Vector3 _position;
        private readonly Vector2 _uv;
        private readonly Vector3 _normal;
        private readonly Color3 _color;

        public static readonly int Size = Marshal.SizeOf(typeof(PrelitVertex));

        public PrelitVertex(Vector3 position, Vector2 uv, Color3 color, Vector3 normal)
            : this()
        {
            _position = position;
            _uv = uv;
            _normal = normal;
            _color = color;
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets the UV.
        /// </summary>
        public Vector2 UV
        {
            get { return _uv; }
        }

        public Color3 Color
        {
            get { return _color; }
        }

        /// <summary>
        /// Gets the normal.
        /// </summary>
        public Vector3 Normal
        {
            get { return _normal; }
        }
    }
}
