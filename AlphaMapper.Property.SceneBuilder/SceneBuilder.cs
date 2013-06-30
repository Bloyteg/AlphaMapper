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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using AlphaMapper.Property.Model;
using AlphaMapper.Renderer;
using AlphaMapper.Renderer.Components;
using AlphaMapper.Renderer.Drawables;
using Byte.ActionCommandTranslator;
using Vector3 = Byte.Math.Vector3;

namespace AlphaMapper.Property.SceneBuilder
{
    public class SceneBuilder
    {
        private readonly RenderSystem _renderSystem;

        public SceneBuilder(RenderSystem renderSystem)
        {
            _renderSystem = renderSystem;
        }
    }
}
