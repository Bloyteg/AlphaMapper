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
using Byte.Math;

namespace Byte.IntermediateModel.Components
{
    [DataContract]
    public class Triangle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class.
        /// </summary>
        /// <param name="index0">The index0.</param>
        /// <param name="index1">The index1.</param>
        /// <param name="index2">The index2.</param>
        public Triangle(int index0, int index1, int index2)
        {
            Indices = new []
                          {
                              index0,
                              index1,
                              index2
                          };
        }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        /// <value>The normal.</value>
        [DataMember]
        public Vector3 Normal { get; set; }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        [DataMember]
        public int[] Indices { get; private set; }
    }
}
