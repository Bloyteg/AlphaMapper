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
using System.Runtime.Serialization;
using Byte.IntermediateModel.Components;
using Byte.Math;

namespace Byte.IntermediateModel.Primitive
{
    [KnownType(typeof(Block))]
    [DataContract]
    public class Block : PrimitiveGeometry
    {
        protected override void ComputeGeometry()
        {
            var x = Width/2;
            var y = Height/2;
            var z = Depth/2;

            Vertices = new List<Vertex>
                           {
                               new Vertex {Position = new Vector3(-x, -y, z)},
                               new Vertex {Position = new Vector3(-x, y, z)},
                               new Vertex {Position = new Vector3(x, y, z)},
                               new Vertex {Position = new Vector3(x, -y, z)},
                               new Vertex {Position = new Vector3(-x, -y, -z)},
                               new Vertex {Position = new Vector3(-x, y, -z)},
                               new Vertex {Position = new Vector3(x, y, -z)},
                               new Vertex {Position = new Vector3(x, -y, -z)}
                           };

            Faces = new List<Face>
                        {
                            new Face
                                {
                                    MaterialId = MaterialId,
                                    Indices = new[] {3, 2, 1, 0},
                                    Triangles = new List<Triangle>
                                                    {
                                                        new Triangle(2, 1, 0),
                                                        new Triangle(3, 2, 0)
                                                    }
                                },
                            new Face
                                {
                                    MaterialId = MaterialId,
                                    Indices = new[] {4, 5, 6, 7},
                                    Triangles = new List<Triangle>
                                                    {
                                                        new Triangle(4, 5, 6),
                                                        new Triangle(4, 6, 7)
                                                    }

                                },
                            new Face
                                {
                                    MaterialId = MaterialId,
                                    Indices = new[] { 0, 1, 5, 4 },
                                    Triangles = new List<Triangle>
                                                    {
                                                        new Triangle(0, 1, 4),
                                                        new Triangle(5, 4, 1)
                                                    }
                                },
                            new Face
                                {
                                    MaterialId = MaterialId,
                                    Indices = new[] { 2, 3, 7, 6 },
                                    Triangles = new List<Triangle>
                                                    {
                                                        new Triangle(2, 3, 6),
                                                        new Triangle(7, 6, 3)
                                                    }
                                },
                            new Face
                                {
                                    MaterialId = MaterialId,
                                    Indices = new[] { 0 },
                                    Triangles = new List<Triangle>
                                                    {
                                                        new Triangle(0, 4, 3),
                                                        new Triangle(3, 4, 7)
                                                    }
                                },
                            new Face
                                {
                                    MaterialId = MaterialId,
                                    Indices = new[] { 1, 2, 6, 5 },
                                    Triangles = new List<Triangle>
                                                    {
                                                        new Triangle(1, 2, 5),
                                                        new Triangle(6, 5, 2)
                                                    }
                                }
                        };

            this.ComputeNormals();
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [DataMember]
        public double Width 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [DataMember]
        public double Height
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        /// <value>The depth.</value>
        [DataMember]
        public double Depth
        {
            get; 
            set; 
        }
    }
}
