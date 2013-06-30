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
using Byte.IntermediateModel.Components;
using Byte.Math;

namespace Byte.IntermediateModel.Mesh
{
    [DataContract]
    public class Prototype : MeshGeometry
    {
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public class PrototypeInstance : ITransformable
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the material.
        /// If null then this has no parent material.
        /// </summary>
        /// <value>
        /// The material.
        /// </value>
        [DataMember]
        public int MaterialId { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [DataMember]
        public Matrix4 Transform { get; set; }
    }
}
