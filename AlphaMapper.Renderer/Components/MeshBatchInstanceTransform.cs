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

using AlphaMapper.Renderer.InternalComponents;
using SharpDX;
using Vector3 = Byte.Math.Vector3;

namespace AlphaMapper.Renderer.Components
{
    public class MeshBatchInstanceTransform : IRenderTransformable
    {
        private Vector3 _worldPosition = new Vector3();
        private Vector3 _worldRotation = new Vector3();
        private Vector3 _worldScale = new Vector3(1, 1, 1);
        private VectorPair _worldSkew = new VectorPair(new Vector3(), new Vector3());
        private SkewType _worldSkewType;

        /// <summary>
        /// Gets or sets the world position.
        /// </summary>
        /// <value>
        /// The world position.
        /// </value>
        public Vector3 WorldPosition
        {
            get { return _worldPosition; }
            set
            {
                _worldPosition = value;
                UpdateMatrix();
            }
        }

        /// <summary>
        /// Gets or sets the world rotation.
        /// </summary>
        /// <value>
        /// The world rotation.
        /// </value>
        public Vector3 WorldRotation
        {
            get { return _worldRotation; }
            set
            {
                _worldRotation = value;
                UpdateMatrix();
            }
        }

        /// <summary>
        /// Gets or sets the world scale.
        /// </summary>
        /// <value>
        /// The world scale.
        /// </value>
        public Vector3 WorldScale
        {
            get { return _worldScale; }
            set
            {
                _worldScale = value;
                UpdateMatrix();
            }
        }

        /// <summary>
        /// Gets or sets the world skew.
        /// </summary>
        /// <value>
        /// The world skew.
        /// </value>
        public VectorPair WorldSkew
        {
            get { return _worldSkew; }
            set
            {
                _worldSkew = value;
                UpdateMatrix();
            }
        }

        /// <summary>
        /// Gets or sets the type of the skew.
        /// </summary>
        /// <value>
        /// The type of the skew.
        /// </value>
        public SkewType WorldSkewType
        {
            get { return _worldSkewType; }
            set
            {
                _worldSkewType = value;
                UpdateMatrix();
            }
        }

        /// <summary>
        /// Gets the world matrix.
        /// </summary>
        internal Matrix WorldMatrix { get; private set; }

        /// <summary>
        /// Updates the matrix.
        /// </summary>
        private void UpdateMatrix()
        {
            WorldMatrix = this.GetTransformMatrix();
        }
    }
}
