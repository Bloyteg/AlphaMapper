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

using AlphaMapper.Renderer.InternalComponents;
using AlphaMapper.Renderer.Utility;
using Bloyteg.AW.Model.RWX.Data.Components;
using Bloyteg.AW.Math;

namespace AlphaMapper.Renderer.Components
{
    public class Light
    {
        public Light(Vector3 direction, Color ambient, Color diffuse)
        {
            Direction = direction;
            Ambient = ambient;
            Diffuse = diffuse;
        }

        public Vector3 Direction { get; private set; }
        public Color Ambient { get; private set; }
        public Color Diffuse { get; private set; }

        internal InternalComponents.Light ToInternalLight()
        {
            return new InternalComponents.Light(Direction.ToDXVector3(), Ambient.ToDXColor4(), Diffuse.ToDXColor4());
        }
    }
}
