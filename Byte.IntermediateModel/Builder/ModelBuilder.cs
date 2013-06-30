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
using Byte.IntermediateModel.Components;
using Byte.IntermediateModel.Mesh;
using Byte.Math;

namespace Byte.IntermediateModel.Builder
{
    public partial class ModelBuilder
    {
        //References to the current geometry data.
        private Model _model = new Model();
        private MeshGeometry _currentMeshGeometry;
        private Clump _currentClump;
        private Prototype _currentPrototype;
        private Color _currentPrelight;
        private Matrix4 _currentTransform = new Matrix4();
        private Material _currentMaterial = new Material();

        //References to the different stacks.
        private readonly Stack<Clump> _clumpStack = new Stack<Clump>();
        private readonly Stack<Material> _materialStack = new Stack<Material>();
        private readonly Stack<Matrix4> _transformStack = new Stack<Matrix4>();

        public Model ToModel()
        {
            return _model;
        }

        public void Reset()
        {
            _currentMeshGeometry = null;
            _currentClump = null;
            _currentPrototype = null;
            _currentPrelight = default(Color);
            _currentTransform = new Matrix4();
            _currentMaterial = new Material();

            _clumpStack.Clear();
            _materialStack.Clear();
            _transformStack.Clear();

            _model = new Model();
        }
    }
}
