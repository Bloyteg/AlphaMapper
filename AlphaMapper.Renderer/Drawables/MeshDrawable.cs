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
using AlphaMapper.Renderer.Components;
using AlphaMapper.Renderer.InternalComponents;
using AlphaMapper.Renderer.Utility;
using Bloyteg.AW.Model.RWX.Data.Components;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using Matrix = SharpDX.Matrix;
using Vector3 = Bloyteg.AW.Math.Vector3;

namespace AlphaMapper.Renderer.Drawables
{
    public sealed class MeshDrawable : MeshDrawableBase, IRenderTransformable
    {
        private Vector3 _worldPosition = new Vector3();
        private Vector3 _worldRotation = new Vector3();
        private Vector3 _worldScale = new Vector3(1, 1, 1);
        private VectorPair _worldSkew = new VectorPair(new Vector3(), new Vector3());
        private SkewType _worldSkewType;

        internal MeshDrawable(DXBuffer vertexBuffer, IEnumerable<TagGroup> tagGroups)
        {
            WorldMatrix = Matrix.Identity;
            VertexBuffer = vertexBuffer;
            TagGroups.AddRange(tagGroups.Select(tagGroup => new WeakReference<TagGroup>(tagGroup)));
        }

        public MeshDrawable()
        {
            //Does nothing.    
        }

        public Vector3 WorldPosition
        {
            get { return _worldPosition; }
            set
            {
                _worldPosition = value;
                UpdateMatrix();

                foreach (var child in Children.OfType<MeshDrawable>())
                {
                    child.WorldPosition = value;
                }
            }
        }

        public Vector3 WorldRotation
        {
            get { return _worldRotation; }
            set
            {
                _worldRotation = value;
                UpdateMatrix();

                foreach (var child in Children.OfType<MeshDrawable>())
                {
                    child.WorldRotation = value;
                }
            }
        }

        public Vector3 WorldScale
        {
            get { return _worldScale; }
            set
            {
                _worldScale = value;
                UpdateMatrix();

                foreach (var child in Children.OfType<MeshDrawable>())
                {
                    child.WorldScale = value;
                }
            }
        }

        public VectorPair WorldSkew
        {
            get { return _worldSkew; }
            set
            {
                _worldSkew = value;
                UpdateMatrix();

                foreach (var child in Children.OfType<MeshDrawable>())
                {
                    child.WorldSkew = value;
                }
            }
        }

        public SkewType WorldSkewType
        {
            get { return _worldSkewType; }
            set
            {
                _worldSkewType = value;
                UpdateMatrix();

                foreach (var child in Children.OfType<MeshDrawable>())
                {
                    child.WorldSkewType = value;
                }
            }
        }

        internal Matrix WorldMatrix { get; private set; }

        public override void DrawShadow()
        {
            if (VertexBuffer != null)
            {
                DrawingManager.SetVertexBuffer(VertexBuffer, IsPrelit);
                DrawingManager.SetWorldMatrix(WorldMatrix);

                foreach (WeakReference<TagGroup> tagGroup in TagGroups)
                {
                    foreach (FaceGroup faceGroup in tagGroup.GetTarget().FaceGroups)
                    {
                        SetShadowDrawingStates(faceGroup, MaterialOverloads[tagGroup.GetTarget().Tag]);
                        DrawingManager.DrawShadow(faceGroup.IndexBuffer, IsPrelit);
                    }
                }
            }

            //Draw children
            foreach (var child in Children)
            {
                child.DrawShadow();
            }
        }

        public override void Draw()
        {
            if (VertexBuffer != null)
            {
                DrawingManager.SetVertexBuffer(VertexBuffer, IsPrelit);
                DrawingManager.SetWorldMatrix(WorldMatrix);

                foreach (WeakReference<TagGroup> tagGroup in TagGroups)
                {
                    foreach (FaceGroup faceGroup in tagGroup.GetTarget().FaceGroups)
                    {
                        var isFlat = faceGroup.Material.LightSampling == LightSampling.Facet;
                        var isWireframe = faceGroup.Material.GeometrySampling == GeometrySampling.Wireframe;

                        SetDrawingStates(faceGroup, MaterialOverloads[tagGroup.GetTarget().Tag]);
                        DrawingManager.Draw(faceGroup.IndexBuffer, IsPrelit, isFlat, isWireframe);
                    }
                }
            }

            foreach (var child in Children)
            {
                child.Draw();
            }
        }

        public override void Dispose()
        {
            //Does nothing right now.
        }

        protected override void UpdateMatrix()
        {
            WorldMatrix = ModelMatrix * this.GetTransformMatrix();
        }
    }
}
