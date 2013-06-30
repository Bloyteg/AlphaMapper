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
    [KnownType(typeof(Cylinder))]
    [DataContract]
    public class Cylinder : PrimitiveGeometry
    {
        protected override void ComputeGeometry()
        {      
            var step = TwoPi/Sides;

            Vertices = new List<Vertex>();
            for(double angle = 0; angle < TwoPi; angle += step)
            { 
                var x = System.Math.Cos(angle);
                var z = System.Math.Sin(angle);

                var topVertex = new Vector3(TopRadius*x, Height, TopRadius*z);
                var bottomVertex = new Vector3(BottomRadius*x, 0, BottomRadius*z);

                Vertices.Add(new Vertex { Position = topVertex});
                Vertices.Add(new Vertex { Position = bottomVertex});
            }

            Faces = new List<Face>();
            var sign = (System.Math.Sign(TopRadius) < 0 || System.Math.Sign(BottomRadius) < 0) ? 0 : 1;

            for (int index = 0; index < Vertices.Count; index+= 2)
            {
                var indices = new[]
                                  {
                                      (index + 3)%Vertices.Count,
                                      (index + 2)%Vertices.Count,
                                      (index + 1)%Vertices.Count,
                                      index,
                                  };


                Faces.Add(new Face
                              {
                                  Indices = new[] {indices[0], indices[2], indices[3], indices[1]},
                                  MaterialId = MaterialId,
                                  Triangles = sign == 1
                                                  ? new List<Triangle>
                                                        {
                                                            new Triangle(indices[2], indices[1], indices[0]),
                                                            new Triangle(indices[1], indices[2], indices[3])
                                                        }
                                                  : new List<Triangle>
                                                        {
                                                            new Triangle(indices[0], indices[1], indices[2]),
                                                            new Triangle(indices[3], indices[2], indices[1])
                                                        }
                              });
            }

            this.ComputeNormals();
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [DataMember]
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the bottom radius.
        /// </summary>
        /// <value>The bottom radius.</value>
        [DataMember]
        public double BottomRadius { get; set; }

        /// <summary>
        /// Gets or sets the top radius.
        /// </summary>
        /// <value>The top radius.</value>
        [DataMember]
        public double TopRadius { get; set; }

        /// <summary>
        /// Gets or sets the sides.
        /// </summary>
        /// <value>The sides.</value>
        [DataMember]
        public int Sides { get; set; }
    }
}
