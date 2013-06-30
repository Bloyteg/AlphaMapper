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
using Byte.IntermediateModel.Mesh;
using Byte.Math;

namespace Byte.IntermediateModel.Builder
{
    public partial class ModelBuilder
    {
        #region clump

        public void BeginClump()
        {
            //If the model already has a root clump, error out;
            if(_model.MainClump != null)
            {
                throw new InvalidOperationException("Model is only allowed to have one root clump.");
            }

            _currentMeshGeometry = _currentClump = new Clump
                                                       {
                                                           Transform = _currentTransform.Copy(),
                                                       };

            //Clumps implicitly modify the transform and material stacks.
            PushAll();
            _currentTransform = new Matrix4();

            _currentPrelight = default(Color);
            _clumpStack.Push(_currentClump);
        }

        public void EndClump()
        {
            //Current clump doesn't exist.
            if(_currentClump == null)
            {
                throw new InvalidOperationException("Mismatched clumpend with no associated clumpbegin.");
            }

            //Compute normals
            _currentClump.ComputeNormals();

            _clumpStack.Pop();

            //Manage the clump stack.
            if (_clumpStack.Count > 0)
            {
                _clumpStack.Peek().Children.Add(_currentClump);
                _currentMeshGeometry = _currentClump = _clumpStack.Peek();
            }
            else if (_currentPrototype == null)
            {
                _model.MainClump = _currentClump;
                _currentMeshGeometry = _currentClump = null;
            }
            else
            {
                _currentPrototype.Children.Add(_currentClump);
                _currentMeshGeometry = _currentPrototype;
                _currentClump = null;
            }

            //Pop the stacks.
            PopAll();
        }
        #endregion

        #region prototype

        public void BeginPrototype(string name)
        {
            if(_currentPrototype != null)
            {
                throw new InvalidOperationException("Prototypes cannot be nested inside of other prototypes.");
            }

            _currentPrelight = default(Color);
            _currentMeshGeometry = _currentPrototype = new Prototype { Name = name };

            //Prototypes implicitly modify the transform and material stacks.
            PushAll();
        }


        public void EndPrototype()
        {
            if(_currentPrototype == null)
            {
                throw new InvalidOperationException("Mismatched protoend with no associated protobegin.");
            }

            //Finish the current prototype's definition and add it to the current model.
            _currentPrototype.ComputeNormals();
            _model.Prototypes.Add(_currentPrototype);
            _currentMeshGeometry = _currentPrototype = null;
            PopAll();
        }

        public void AddProtoInstance(string name)
        {
            //Handle there being no mesh to add to.
            if(_currentMeshGeometry == null)
            {
                throw new InvalidOperationException("This command is only valid inside a clump.");
            }

            var prototypeInstance = new PrototypeInstance
                                        {
                                            Name = name,
                                            Transform = _currentTransform.Copy()
                                        };

            _currentMeshGeometry.PrototypeInstances.Add(prototypeInstance);
        }

        public void AddProtoInstanceGeometry(string name)
        {
            //Handle there being no mesh to add to.
            if (_currentMeshGeometry == null)
            {
                throw new InvalidOperationException("This command is only valid inside a clump.");
            }

            var prototypeInstance = new PrototypeInstance
                                        {
                                            MaterialId = _model.AddMaterial(_currentMaterial),
                                            Name = name,
                                            Transform = _currentTransform.Copy()
                                        };

            _currentClump.PrototypeInstances.Add(prototypeInstance);
        }
        #endregion

        #region transform

        public void BeginTransform()
        {
            _transformStack.Push(_currentTransform.Copy());
        }

        public void EndTransform()
        {
            //Handles the case where a transformend has no corresponding transformbegin.
            if(_transformStack.Count == 0)
            {
                throw new InvalidOperationException("Mistmatched transformend with no corresponding transformbegin.");
            }

            _currentTransform = _transformStack.Pop();
        }
        #endregion

        #region material

        public void BeginMaterial()
        {
            _materialStack.Push(_currentMaterial);
        }

        public void EndMaterial()
        {
            //Handles the case where a materialend has no corresponding materialbegin.
            if (_transformStack.Count == 0)
            {
                throw new InvalidOperationException("Mistmatched materialend with no corresponding materialbegin.");
            }

            _currentMaterial = _materialStack.Pop();
        }
        #endregion

        #region helpers

        private void PushAll()
        {
            _transformStack.Push(_currentTransform.Copy());
            _materialStack.Push(_currentMaterial);
        }

        private void PopAll()
        {
            _currentTransform = _transformStack.Pop();
            _currentMaterial = _materialStack.Pop();
        }

        #endregion
    }
}
