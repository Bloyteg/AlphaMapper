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
using Byte.Math;

namespace Byte.IntermediateModel.Components
{
    [DataContract]
    public class Vertex
    {
        private Vector3 _vector = new Vector3();

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [DataMember]
        public Vector3 Position
        {
            get { return _vector; }
            set { _vector = value; }
        }

        /// <summary>
        /// Gets or sets the UV.
        /// </summary>
        /// <value>The UV.</value>
        [DataMember]
        public UV UV { get; set; }

        /// <summary>
        /// Gets or sets the prelight.
        /// </summary>
        /// <value>The prelight.</value>
        [DataMember]
        public Color Prelight { get; set; }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        /// <value>The normal.</value>
        [DataMember]
        public Vector3 Normal { get; set; }
    }
}
