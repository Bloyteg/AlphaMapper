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
    public class VertexLayouts : IDisposable
    {
        internal VertexLayouts(InputLayout standardLayout, InputLayout prelitLayout)
        {
            StandardLayout = standardLayout;
            PrelitLayout = prelitLayout;
        }

        /// <summary>
        /// Gets the prelit layout.
        /// </summary>
        public InputLayout PrelitLayout
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the standard layout.
        /// </summary>
        public InputLayout StandardLayout
        {
            get;
            private set;
        }

        public void Dispose()
        {
            StandardLayout.Dispose();
            PrelitLayout.Dispose();
        }
    }
}
