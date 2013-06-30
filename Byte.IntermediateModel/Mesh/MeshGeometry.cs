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
using Byte.IntermediateModel.Primitive;

namespace Byte.IntermediateModel.Mesh
{
    /// <summary>
    /// Represents a complex geometry.
    /// </summary>
    [KnownType(typeof(Block))]
    [KnownType(typeof(Cone))]
    [KnownType(typeof(Cylinder))]
    [KnownType(typeof(Disc))]
    [KnownType(typeof(Hemisphere))]
    [KnownType(typeof(Sphere))]
    [DataContract]
    public abstract class MeshGeometry : IGeometry
    {
        private List<Clump> _children = new List<Clump>();
        private List<Vertex> _vertices = new List<Vertex>();
        private List<Face> _faces = new List<Face>();
        private List<PrimitiveGeometry> _primitives = new List<PrimitiveGeometry>();
        private List<PrototypeInstance> _prototypeInstances = new List<PrototypeInstance>();

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        [DataMember]
        public ICollection<Clump> Children
        {
            get
            {
                return _children ?? (_children = new List<Clump>());
            }
        }

        /// <summary>
        /// Gets the primitives.
        /// </summary>
        /// <value>The primitives.</value>
        [DataMember]
        public ICollection<PrimitiveGeometry> Primitives
        {
            get
            {
                return _primitives ?? (_primitives = new List<PrimitiveGeometry>());
            }
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <value>The vertices.</value>
        [DataMember]
        public IList<Vertex> Vertices
        {
            get
            {
                return _vertices ?? (_vertices = new List<Vertex>());
            }
        }

        /// <summary>
        /// Gets the faces.
        /// </summary>
        /// <value>The faces.</value>
        [DataMember]
        public ICollection<Face> Faces
        {
            get
            {
                return _faces ?? (_faces = new List<Face>());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is prelit.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is prelit; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrelit { get; set; }

        /// <summary>
        /// Gets the prototype instances.
        /// </summary>
        /// <value>The prototype instances.</value>
        [DataMember]
        public IList<PrototypeInstance> PrototypeInstances
        {
            get { return _prototypeInstances ?? (_prototypeInstances = new List<PrototypeInstance>()); }
        }
    }
}
