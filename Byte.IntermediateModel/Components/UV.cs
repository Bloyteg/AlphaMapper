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

using System.Runtime.Serialization;

namespace Byte.IntermediateModel.Components
{
    [DataContract]
    public class UV
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UV"/> class.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <param name="v">The v.</param>
        public UV(float u, float v)
        {
            U = u;
            V = v;
        }

        /// <summary>
        /// Gets or sets the U.
        /// </summary>
        /// <value>The U.</value>
        [DataMember]
        public float U { get; private set; }

        /// <summary>
        /// Gets or sets the V.
        /// </summary>
        /// <value>The V.</value>
        [DataMember]
        public float V { get; private set; }
    }
}
