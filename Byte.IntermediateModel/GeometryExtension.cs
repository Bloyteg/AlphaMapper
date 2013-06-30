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
using System.Linq;
using Byte.IntermediateModel.Components;
using Byte.Math;

namespace Byte.IntermediateModel
{
    public static class GeometryExtension
    {
        /// <summary>
        /// Computes the normals.
        /// </summary>
        public static void ComputeNormals(this IGeometry geometry)
        {
            //Compute face normals
            foreach (Triangle triangle in geometry.Faces.SelectMany(face => face.Triangles))
            {
                triangle.Normal = new Vector3();

                var vertices = (from vertex in triangle.Indices select geometry.Vertices[vertex]).ToArray();

                Vector3 u = vertices[1].Position - vertices[0].Position;
                Vector3 v = vertices[2].Position - vertices[0].Position;

                triangle.Normal = Vector3.Cross(u, v).Normalize();
            }

            //Compute vertex normals;
            var trianglesByVertex = from face in geometry.Faces
                                    from triangle in face.Triangles
                                    from index in triangle.Indices
                                    group triangle by index;

            foreach(var vertexWithTriangles in trianglesByVertex)
            {
                int localVertex = vertexWithTriangles.Key;

                var normals = from triangle in vertexWithTriangles
                              let currentIndex = Array.IndexOf(triangle.Indices, localVertex)
                              let previousIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : triangle.Indices.Length - 1
                              let nextIndex = (currentIndex + 1) % triangle.Indices.Length
                              select new
                              {
                                  Vec1 = (geometry.Vertices[triangle.Indices[previousIndex]].Position - geometry.Vertices[triangle.Indices[currentIndex]].Position),
                                  Vec2 = (geometry.Vertices[triangle.Indices[nextIndex]].Position - geometry.Vertices[triangle.Indices[currentIndex]].Position),
                                  triangle.Normal,
                              };

                geometry
                    .Vertices[localVertex]
                    .Normal = normals.Aggregate(Tuple.Create(new Vector3(), 0.0),
                                                (aggregate, faceNormal) =>
                                                    {
                                                        var angle = System.Math.Acos(Vector3.Dot(faceNormal.Vec1.Normalize(),
                                                                                                 faceNormal.Vec2.Normalize()));

                                                        return Tuple.Create(aggregate.Item1 + faceNormal.Normal*angle,
                                                                            aggregate.Item2 + angle);
                                                    },
                                                resultTuple => (resultTuple.Item1/resultTuple.Item2).Normalize());
            }
        }
    }
}
