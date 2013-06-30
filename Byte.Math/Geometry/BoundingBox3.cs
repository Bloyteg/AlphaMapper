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
using System.Linq;

namespace Byte.Math.Geometry
{
    /// <summary>
    /// Represents a 3D bounding box.
    /// </summary>
    public class BoundingBox3
    {
        public Vector3 Minimum { get; private set; }
        public Vector3 Maximum { get; private set; }

        public BoundingBox3()
        {
            Minimum = new Vector3();
            Maximum = new Vector3();
        }

        /// <summary>
        /// Computes from vertices.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        public void ComputeFromVertices(IEnumerable<Vector3> vertices)
        {
            Minimum = new Vector3(vertices.First().X,
                                  vertices.First().Y,
                                  vertices.First().Z);

            Maximum = new Vector3(vertices.First().X,
                                  vertices.First().Y,
                                  vertices.First().Z);

            foreach(var vertex in vertices)
            {
                if (vertex.X <= Minimum.X)
                    Minimum = Minimum.UpdateX(vertex.X);

                if (vertex.Y <= Minimum.Y)
                    Minimum = Minimum.UpdateY(vertex.Y);

                if (vertex.Z <= Minimum.Z)
                    Minimum = Minimum.UpdateZ(vertex.Z);

                if (vertex.X >= Maximum.X)
                    Maximum = Maximum.UpdateX(vertex.X);

                if (vertex.Y >= Maximum.Y)
                    Maximum = Maximum.UpdateY(vertex.Y);

                if (vertex.Z >= Maximum.Z)
                    Maximum = Maximum.UpdateZ(vertex.Z);
            }
        }
    }
}
