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
using DXBuffer = SharpDX.Direct3D11.Buffer;
using DXMatrix = SharpDX.Matrix;

namespace AlphaMapper.Renderer.InternalComponents
{
    class MeshCache : IDisposable
    {
        private readonly DXBuffer _vertexBuffer;
        private readonly List<MeshCache> _children = new List<MeshCache>();
        private readonly List<MeshCacheReference> _prototypeReferences = new List<MeshCacheReference>();
        private readonly IEnumerable<TagGroup> _tagGroups;
        private readonly DXMatrix _localMatrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshCache"/> class.
        /// </summary>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        /// <param name="tagGroups">The tag groups.</param>
        /// <param name="localMatrix">The local matrix.</param>
        /// <param name="isPrelit">if set to <c>true</c> [is prelit].</param>
        /// <param name="tag">The tag.</param>
        public MeshCache(DXBuffer vertexBuffer, IEnumerable<TagGroup> tagGroups, DXMatrix localMatrix, bool isPrelit, int? tag)
        {
            _vertexBuffer = vertexBuffer;
            _tagGroups = tagGroups;
            _localMatrix = localMatrix;
            IsPrelit = isPrelit;
            Tag = tag;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshCache"/> class.
        /// </summary>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        /// <param name="tagGroups">The tag groups.</param>
        /// <param name="localMatrix">The local matrix.</param>
        public MeshCache(DXBuffer vertexBuffer, IEnumerable<TagGroup> tagGroups, DXMatrix localMatrix)
        {
            _vertexBuffer = vertexBuffer;
            _tagGroups = tagGroups;
            IsPrelit = false;
            _localMatrix = localMatrix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshCache"/> class.
        /// </summary>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        /// <param name="tagGroups">The tag groups.</param>
        public MeshCache(DXBuffer vertexBuffer, IEnumerable<TagGroup> tagGroups)
        {
            _vertexBuffer = vertexBuffer;
            _tagGroups = tagGroups;
            IsPrelit = false;
            _localMatrix = DXMatrix.Identity;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public IList<MeshCache> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Gets the prototype references.
        /// </summary>
        public IList<MeshCacheReference> PrototypeReferences
        {
            get { return _prototypeReferences; }
        }

        /// <summary>
        /// Gets the vertex buffer.
        /// </summary>
        public DXBuffer VertexBuffer
        {
            get { return _vertexBuffer; }
        }

        /// <summary>
        /// Gets the tag groups.
        /// </summary>
        public IEnumerable<TagGroup> TagGroups
        {
            get { return _tagGroups; }
        }

        /// <summary>
        /// Gets the local matrix.
        /// </summary>
        public DXMatrix LocalMatrix
        {
            get { return _localMatrix; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is prelit.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is prelit; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrelit { get; private set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public int? Tag { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if(_vertexBuffer != null)
                _vertexBuffer.Dispose();

            //Clean up index buffers
            foreach (var faceGroup in TagGroups.SelectMany(tagGroup => tagGroup.FaceGroups).Where(faceGroup => faceGroup.IndexBuffer != null))
            {
                faceGroup.Dispose();
            }

            //Clean up children
            foreach(var child in _children)
            {
                child.Dispose();
            }
        }
    }
}
