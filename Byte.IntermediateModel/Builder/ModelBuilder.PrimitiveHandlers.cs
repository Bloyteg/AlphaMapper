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
using Byte.IntermediateModel.Primitive;

namespace Byte.IntermediateModel.Builder
{
    public partial class ModelBuilder
    {
        private void PrevalidatePrimitive()
        {
            if (_currentMeshGeometry == null)
            {
                throw  new InvalidOperationException("Can only use primitives inside clumps or prototypes.");
            }
        }

        public void AddBlock(double height, double width, double depth)
        {
            PrevalidatePrimitive();

            _currentMeshGeometry.Primitives.Add(new Block
                                                    {
                                                        Width = width,
                                                        Height = height,
                                                        Depth = depth,
                                                        MaterialId = _model.AddMaterial(_currentMaterial),
                                                        Transform = _currentTransform
                                                    });
        }

        public void AddCone(double radius, double height, int sides)
        {
            PrevalidatePrimitive();

            _currentMeshGeometry.Primitives.Add(new Cone
                                                    {
                                                        Radius = radius,
                                                        Height = height,
                                                        Sides = sides,
                                                        MaterialId = _model.AddMaterial(_currentMaterial),
                                                        Transform = _currentTransform
                                                    });
        }

        public void AddCylinder(double height, double bottomRadius, double topRadius, int sides)
        {
            PrevalidatePrimitive();

            _currentMeshGeometry.Primitives.Add(new Cylinder
                                                    {
                                                        Height = height,
                                                        TopRadius = topRadius,
                                                        BottomRadius = bottomRadius,
                                                        Sides = sides,
                                                        MaterialId = _model.AddMaterial(_currentMaterial),
                                                        Transform = _currentTransform
                                                    });
        }

        public void AddDisc(double offset, double radius, int sides)
        {
            PrevalidatePrimitive();

            _currentMeshGeometry.Primitives.Add(new Disc
                                                    {
                                                        Offset = offset,
                                                        Radius = radius,
                                                        Sides = sides,
                                                        MaterialId = _model.AddMaterial(_currentMaterial),
                                                        Transform = _currentTransform
                                                    });
        }

        public void AddHemisphere(double radius, int density)
        {
            PrevalidatePrimitive();

            _currentMeshGeometry.Primitives.Add(new Hemisphere
                                                    {
                                                        Radius = radius,
                                                        Density = density,
                                                        MaterialId = _model.AddMaterial(_currentMaterial),
                                                        Transform = _currentTransform
                                                    });
        }

        public void AddSphere(double radius, int density)
        {
            PrevalidatePrimitive();

            _currentMeshGeometry.Primitives.Add(new Sphere
                                                    {
                                                        Radius = radius,
                                                        Density = density,
                                                        MaterialId = _model.AddMaterial(_currentMaterial),
                                                        Transform = _currentTransform
                                                    });
        }
    }
}
