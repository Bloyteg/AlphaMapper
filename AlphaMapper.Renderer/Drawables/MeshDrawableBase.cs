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
using AlphaMapper.Renderer.InternalComponents;
using AlphaMapper.Renderer.Utility;
using SharpDX;
using DXBuffer = SharpDX.Direct3D11.Buffer;

namespace AlphaMapper.Renderer.Drawables
{
    public abstract class MeshDrawableBase : Drawable
    {
        private Matrix _modelMatrix;
        private WeakReference<DXBuffer> _vertexBuffer;
        private readonly List<Drawable> _children = new List<Drawable>();
        private readonly List<WeakReference<TagGroup>> _tagGroups = new List<WeakReference<TagGroup>>();
        private readonly Dictionary<Tag, TagGroupMaterialOverload> _materialOverloads = new Dictionary<Tag, TagGroupMaterialOverload>();

        internal IEnumerable<Drawable> Children
        {
            get { return _children; }
        }

        internal IDictionary<Tag, TagGroupMaterialOverload> MaterialOverloads
        {
            get { return _materialOverloads; }
        }

        internal bool IsPrelit { get; set; }

        internal int? Tag { get; set; }

        internal Matrix ModelMatrix
        {
            get { return _modelMatrix; }
            set
            {
                _modelMatrix = value;
                UpdateMatrix();
            }
        }

        internal DXBuffer VertexBuffer
        {
            get { return _vertexBuffer.GetTarget(); }
            set { _vertexBuffer = new WeakReference<DXBuffer>(value); }
        }

        internal List<WeakReference<TagGroup>> TagGroups
        {
            get
            {
                return _tagGroups;
            }
        }

        internal void AddChild(MeshDrawableBase child)
        {
            _children.Add(child);
        }

        internal void SetDrawingStates(FaceGroup faceGroup, TagGroupMaterialOverload materialOverload)
        {
            var color = faceGroup.Material.Color;

            DrawingManager.SetColor((materialOverload.Color ?? color).ToDXColor4());
            DrawingManager.SetColorTintState(materialOverload.Color != null && materialOverload.IsColorTint);
            DrawingManager.SetOpacity(materialOverload.Opacity ?? faceGroup.Material.Opacity);
            DrawingManager.SetAmbient(faceGroup.Material.Ambient);
            DrawingManager.SetDiffuse(faceGroup.Material.Diffuse);
            DrawingManager.SetTextureAddressMode(faceGroup.Material.TextureAddressMode);

            if (materialOverload.IsColorTint || materialOverload.Color == null)
            {
                DrawingManager.SetTexture(materialOverload.Texture.GetTarget() ?? faceGroup.Texture.GetTarget());
                DrawingManager.SetMask(materialOverload.Mask.GetTarget() ?? faceGroup.Mask.GetTarget());
            }
            else
            {
                DrawingManager.SetTexture(null);
                DrawingManager.SetMask(null);
            }
        }

        /// <summary>
        /// Sets the shadow drawing states.
        /// </summary>
        /// <param name="faceGroup">The face group.</param>
        /// <param name="materialOverload">The material overload.</param>
        internal void SetShadowDrawingStates(FaceGroup faceGroup, TagGroupMaterialOverload materialOverload)
        {
            if (materialOverload.IsColorTint || materialOverload.Color == null)
            {
                DrawingManager.SetShadowMask(materialOverload.Mask.GetTarget() ?? faceGroup.Mask.GetTarget());
            }
            else
            {
                DrawingManager.SetShadowMask(null);
            }
        }

        /// <summary>
        /// Updates the matrix.
        /// </summary>
        protected virtual void UpdateMatrix()
        {
            //Empty
        }

        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns></returns>
        public MeshDrawable Copy()
        {
            return (MeshDrawable)MemberwiseClone();
        }
    }
}
