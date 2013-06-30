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
    [DataContract]
    public abstract class PrimitiveGeometry : IGeometry, ITransformable
    {
        private IList<Vertex> _vertices;
        private ICollection<Face> _faces;
        private int _materialId;

        protected const double TwoPi = 2 * System.Math.PI;

        protected PrimitiveGeometry()
        {
            Transform = new Matrix4();
        }

        /// <summary>
        /// Computes the geometry for the primitive type in a derived class.
        /// </summary>
        protected abstract void ComputeGeometry();

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <value>The vertices.</value>
        [IgnoreDataMember]
        public IList<Vertex> Vertices
        {
            get
            {
                if (_faces == null && _vertices == null)
                {
                    ComputeGeometry();
                }

                return _vertices;
            }

            protected set
            {
                _vertices = value;
            }
        }

        /// <summary>
        /// Gets the faces.
        /// </summary>
        /// <value>The faces.</value>
        [IgnoreDataMember]
        public ICollection<Face> Faces
        {
            get
            {
                if(_faces == null && _vertices == null)
                {
                    ComputeGeometry();
                }

                return _faces;
            }

            protected set
            {
                _faces = value;
            }
        }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        /// <value>The material.</value>
        [DataMember]
        public int MaterialId
        {
            get
            {
                return _materialId;
            }

            set
            {
                _materialId = value;
            
                if(_faces != null)
                {
                    foreach(var face in _faces)
                    {
                        face.MaterialId = _materialId;
                    }
                }
            }


        }

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>The transform.</value>
        [DataMember]
        public Matrix4 Transform
        {
            get; set;
        }
    }
}
