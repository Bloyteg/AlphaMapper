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
using Bloyteg.AW.Model.RWX.Data.Components;

namespace AlphaMapper.Renderer.Components
{
    public class MaterialOverload : IEquatable<MaterialOverload>
    {
        public float? Opacity { get; set; }
        public Color? Color { get; set; }
        public bool IsColorTint { get; set; }
        public string Texture { get; set; }
        public string Mask { get; set; }
        public int? Tag { get; set; }

        public bool Equals(MaterialOverload other)
        {
            return other != null && other.Tag == Tag;
        }

        public override bool Equals(object other)
        {
            return Equals(other as MaterialOverload);
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode();
        }
    }
}
