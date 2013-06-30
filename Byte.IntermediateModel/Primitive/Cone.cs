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
using System.Runtime.Serialization;
using System.Text;
using Byte.IntermediateModel.Components;
using Byte.Math;

namespace Byte.IntermediateModel.Primitive
{
    [KnownType(typeof(Cone))]
    [DataContract]
    public class Cone : PrimitiveGeometry
    {
        protected override void ComputeGeometry()
        {
            var step = TwoPi / Sides;

            Vertices = new List<Vertex>
                           {
                               new Vertex {Position = new Vector3(0, Height, 0)}
                           };

            //Add the top vertex.

            //Add the ring vertices around the bottom.
            for (double angle = 0; angle < TwoPi; angle += step)
            {
                var x = System.Math.Cos(angle);
                var z = System.Math.Sin(angle);

                var vertex = new Vector3(Radius * x, 0, Radius * z);

                Vertices.Add(new Vertex { Position = vertex });
            }

            Faces = new List<Face>();
            var sign = System.Math.Sign(Radius);

            for (int index = 0; index < Vertices.Count - 2; index++)
            {
                if (sign < 0)
                {
                    Faces.Add(new Face(0,
                                       (index + 1)%Vertices.Count,
                                       (index + 2)%Vertices.Count)
                                  {
                                      MaterialId = MaterialId
                                  });
                }
                else
                {
                    Faces.Add(new Face((index + 2)%Vertices.Count,
                                       (index + 1)%Vertices.Count,
                                       0)
                                  {
                                      MaterialId = MaterialId
                                  });
                }
            }

            if(sign < 0)
            {
                Faces.Add(new Face(0, Vertices.Count - 1, 1)
                              {
                                  MaterialId = MaterialId
                              });
            }
            else
            {
                Faces.Add(new Face(0, 1, Vertices.Count - 1)
                {
                    MaterialId = MaterialId
                });
            }

            this.ComputeNormals();
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        [DataMember]
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [DataMember]
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the sides.
        /// </summary>
        /// <value>The sides.</value>
        [DataMember]
        public int Sides { get; set; }
    }
}
