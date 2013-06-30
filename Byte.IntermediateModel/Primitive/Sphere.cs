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
    [KnownType(typeof(Sphere))]
    [DataContract]
    public class Sphere : PrimitiveGeometry
    {
        private const double Pi = System.Math.PI;

        protected override void ComputeGeometry()
        {
            var longitudeStep = Pi / (Density);
            var lattitudeStep = Pi / (2*Density);

            var longitudeDivisions = 2*Density;
            var lattitudeDivisions = 2*Density;

            Vertices = new List<Vertex>();

            //Generate the main portions of the sphere
            for (var phiStep = 1; phiStep < lattitudeDivisions; ++phiStep)
            {
                var phi = phiStep*lattitudeStep;

                for (var thetaStep = 0; thetaStep < longitudeDivisions; ++thetaStep)
                {
                    var theta = thetaStep*longitudeStep;
                    var x = System.Math.Sin(phi) * System.Math.Cos(theta);
                    var y = System.Math.Cos(phi);
                    var z = System.Math.Sin(phi) * System.Math.Sin(theta);

                    var vertex = new Vector3(Radius * x, Radius * y, Radius * z);
                    Vertices.Add(new Vertex { Position = vertex });
                }
            }

            //Generate the poles.
            Vertices.Add(new Vertex { Position = new Vector3(0, Radius, 0) });
            Vertices.Add(new Vertex { Position = new Vector3(0, -Radius, 0) });

            //Generate the faces.
            Faces = new List<Face>();

            var vertexIndices = Vertices.Select((vertex, index) => index).ToList();
            var verticesToTake = 2*Density;
            var ringsToTake = 2*Density - 2;

            for(int ringIndex = 0; ringIndex < ringsToTake; ++ringIndex)
            {
                var ringIndices = Enumerable.Zip(vertexIndices.Skip(ringIndex*verticesToTake).Take(verticesToTake),
                                                 vertexIndices.Skip((ringIndex + 1)*verticesToTake).Take(verticesToTake),
                                                 (first, second) => new {Lower = first, Upper = second})
                                  .ToList();

                for(int index = 0; index < ringIndices.Count; ++index)
                {
                    var firstSet = ringIndices[index];
                    var secondSet = ringIndices[(index + 1) % ringIndices.Count];

                    Faces.Add(new Face(firstSet.Lower, secondSet.Lower, firstSet.Upper) { MaterialId = MaterialId});
                    Faces.Add(new Face(secondSet.Lower, secondSet.Upper, firstSet.Upper) { MaterialId = MaterialId });
                }
            }

            //Add the top and bottom end cap faces.
            var topIndex = Vertices.Count - 2;
            var bottomIndex = Vertices.Count - 1;
            var topRing = vertexIndices.Take(verticesToTake).ToList();
            var bottomRing = vertexIndices.Skip(ringsToTake*verticesToTake).Take(verticesToTake).ToList();

            for (int index = 0; index < topRing.Count; ++index)
            {
                var firstTopIndex = topRing[index];
                var secondTopIndex = topRing[(index + 1)%topRing.Count];
                Faces.Add(new Face(firstTopIndex, topIndex, secondTopIndex) { MaterialId = MaterialId });

                var firstBottomIndex = bottomRing[index];
                var secondBottomIndex = bottomRing[(index + 1) % topRing.Count];
                Faces.Add(new Face(firstBottomIndex, secondBottomIndex, bottomIndex) { MaterialId = MaterialId });
            }

            //Compute normals
            this.ComputeNormals();
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the density.
        /// </summary>
        /// <value>
        /// The density.
        /// </value>
        public int Density { get; set; }
    }
}
