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
using System.Runtime.Serialization;
using Byte.Math;
using Byte.Math.Geometry;

namespace Byte.IntermediateModel.Components
{
    [DataContract]
    public class Face
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class.
        /// Default constructor.
        /// </summary>
        public Face()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class.
        /// Used to construct a triangle face directly.
        /// </summary>
        /// <param name="index0">The index0.</param>
        /// <param name="index1">The index1.</param>
        /// <param name="index2">The index2.</param>
        public Face(int index0, int index1, int index2)
        {
            Indices = new[] {index0, index1, index2};

            Triangles = new List<Triangle>
                            {
                                new Triangle(index0, index1, index2)
                            };
        }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        /// <value>The material.</value>
        [DataMember]
        public int MaterialId { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        [DataMember]
        public int? Tag { get; set; }

        /// <summary>
        /// Gets or sets the vertices. Used for retrieving the original indices for wireframe mode.
        /// </summary>
        /// <value>The vertices.</value>
        [DataMember]
        public IEnumerable<int> Indices { get; set; }

        /// <summary>
        /// Gets or sets the triangles.
        /// </summary>
        /// <value>
        /// The triangles.
        /// </value>
        [DataMember]
        public ICollection<Triangle> Triangles { get; set; }
    }
}
