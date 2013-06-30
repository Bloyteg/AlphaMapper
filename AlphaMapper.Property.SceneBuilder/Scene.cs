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

using System.Collections.Generic;
using AlphaMapper.Renderer.Drawables;

namespace AlphaMapper.Property.SceneBuilder
{
    public class Scene
    {
        private readonly IEnumerable<Drawable> _drawables;
 
        internal Scene(IEnumerable<Drawable> drawables)
        {
            _drawables = drawables;
        }

        public void Draw()
        {
            foreach(var drawable in _drawables)
            {
                drawable.Draw();
            }
        }

        public void DrawShadow()
        {
            foreach (var drawable in _drawables)
            {
                drawable.DrawShadow();
            }
        }
    }
}
