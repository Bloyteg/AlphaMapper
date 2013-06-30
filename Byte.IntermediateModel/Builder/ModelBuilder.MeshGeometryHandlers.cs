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
using System.Linq;
using Byte.IntermediateModel.Components;
using Byte.Math;
using Byte.Math.Geometry;

namespace Byte.IntermediateModel.Builder
{
    public partial class ModelBuilder
    {
        private void PrevalidateGeometry()
        {
            //Handle there being no mesh to add to.
            if (_currentMeshGeometry == null)
            {
                throw new InvalidOperationException("This command is only valid inside a prototype or clump.");
            }
        }

        public void AddPolygon(int count, IEnumerable<int> indices, int? tag)
        {
            PrevalidateGeometry();

            //Error out on polygons not matching, for the sake of being consistent with AW and other RWX loaders.
            if(indices.Count() != count)
            {
                throw new InvalidOperationException("Polygon vertex count mismatch.");
            }

            _currentMeshGeometry.Faces.Add(new Face
                                               {
                                                   Indices = indices.ToList(),
                                                   MaterialId = _model.AddMaterial(_currentMaterial),
                                                   Tag = tag,
                                                   Triangles = ProcessIndicesToTriangle(indices).ToList()
                                               });

            if(_currentMaterial.MaterialMode == MaterialMode.Double)
            {
                _currentMeshGeometry.Faces.Add(new Face
                {
                    Indices = indices.Reverse().ToList(),
                    MaterialId = _model.AddMaterial(_currentMaterial),
                    Tag = tag,
                    Triangles = ProcessIndicesToTriangle(indices.Reverse()).ToList()
                });
            }
        }


        public void AddQuad(int index0, int index1, int index2, int index3, int? tag)
        {
            PrevalidateGeometry();

            var indicesAsArray = new[]
                              {
                                  index0,
                                  index1,
                                  index2,
                                  index3
                              };

            _currentMeshGeometry.Faces.Add(new Face
            {
                Indices = indicesAsArray,
                MaterialId = _model.AddMaterial(_currentMaterial),
                Tag = tag,
                Triangles = new List<Triangle>
                                {
                                    new Triangle(indicesAsArray[0], indicesAsArray[1], indicesAsArray[2]),
                                    new Triangle(indicesAsArray[0], indicesAsArray[2], indicesAsArray[3])
                                }
            });

            if (_currentMaterial.MaterialMode == MaterialMode.Double)
            {
                _currentMeshGeometry.Faces.Add(new Face
                {
                    Indices = indicesAsArray.Reverse().ToList(),
                    MaterialId = _model.AddMaterial(_currentMaterial),
                    Tag = tag,
                    Triangles = new List<Triangle>
                                {
                                    new Triangle(indicesAsArray[2], indicesAsArray[1], indicesAsArray[0]),
                                    new Triangle(indicesAsArray[3], indicesAsArray[2], indicesAsArray[0])
                                }
                });
            }
        }

        public void AddTriangle(int index0, int index1, int index2, int? tag)
        {
            PrevalidateGeometry();

            var triangle = new Face(index0, index1, index2)
                               {
                                   MaterialId = _model.AddMaterial(_currentMaterial),
                                   Tag = tag
                               };
           
            _currentMeshGeometry.Faces.Add(triangle);

            if(_currentMaterial.MaterialMode == MaterialMode.Double)
            {
                var backTriangle = new Face(index0, index1, index2)
                {
                    MaterialId = _model.AddMaterial(_currentMaterial),
                    Tag = tag
                };

                _currentMeshGeometry.Faces.Add(backTriangle);
            }
        }

        public void AddVertex(Tuple<double, double, double> position, Tuple<float, float> uv, Tuple<float, float, float> prelight)
        {
            PrevalidateGeometry();

            var vertex = new Vertex
            {
                Position = new Vector3(position.Item1, position.Item2, position.Item3)
            };

            vertex.Position *= _currentTransform;

            //Handle the UV
            if (uv != null)
            {
                vertex.UV = new UV(uv.Item1, uv.Item2);
            }

            //Handle prelight
            if(prelight != null)
            {
                _currentMeshGeometry.IsPrelit = true;
                _currentPrelight = new Color(prelight.Item1, prelight.Item2, prelight.Item3);
            }

            vertex.Prelight = _currentPrelight;

            _currentMeshGeometry.Vertices.Add(vertex);
        }

        /// <summary>
        /// Processes the indices to triangle.
        /// </summary>
        /// <param name="indices">The indices.</param>
        private IEnumerable<Triangle> ProcessIndicesToTriangle(IEnumerable<int> indices)
        {
            var vertices = from index in indices select _currentMeshGeometry.Vertices[index].Position;
            var triangulator = new Triangulator(vertices);

            int index0 = 0;
            int index1 = 0;
            int index2 = 0;

            foreach (var triangle in triangulator.Triangles)
            {
                for (int index = 0; index < _currentMeshGeometry.Vertices.Count; index++)
                {
                    var vertex = _currentMeshGeometry.Vertices[index];
                    if (ReferenceEquals(vertex.Position, triangle.Item1))
                    {
                        index0 = index;
                    }
                    else if (ReferenceEquals(vertex.Position, triangle.Item2))
                    {
                        index1 = index;
                    }
                    else if (ReferenceEquals(vertex.Position, triangle.Item3))
                    {
                        index2 = index;
                    }
                }

                yield return new Triangle(index0, index1, index2);
            }
        }
    }
}
