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
using Byte.IntermediateModel.Components;
using DXBuffer = SlimDX.Direct3D11.Buffer;
using Matrix = SlimDX.Matrix;

namespace AlphaMapper.Renderer.Drawables
{
    public class MeshBatchDrawable : MeshDrawableBase
    {
        internal int InstanceCount { get; set; }
        internal DXBuffer TransformBuffer { get; set; }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public override void Draw()
        {
            if (VertexBuffer != null)
            {
                DrawingManager.SetVertexAndTransformBuffers(VertexBuffer, TransformBuffer, IsPrelit);
                DrawingManager.SetWorldMatrix(Matrix.Identity);

                foreach (TagGroup tagGroup in TagGroups)
                {
                    foreach(FaceGroup faceGroup in tagGroup.FaceGroups)
                    {
                        var isFlat = faceGroup.Material.LightSampling == LightSampling.Facet;
                        var isWireframe = faceGroup.Material.GeometrySampling == GeometrySampling.Wireframe;

                        SetDrawingStates(faceGroup, MaterialOverloads[tagGroup.Tag]);
                        DrawingManager.Draw(faceGroup.IndexBuffer, InstanceCount, IsPrelit, isFlat, isWireframe);
                    }
                }
            }

            //Draw children
            foreach (var child in Children)
            {
                child.Draw();
            }
        }

        /// <summary>
        /// Draws the shadow.
        /// </summary>
        public override void DrawShadow()
        {
            if (VertexBuffer != null)
            {
                DrawingManager.SetVertexAndTransformBuffers(VertexBuffer, TransformBuffer, IsPrelit);
                DrawingManager.SetWorldMatrix(Matrix.Identity);

                foreach (TagGroup tagGroup in TagGroups)
                {
                    foreach (FaceGroup faceGroup in tagGroup.FaceGroups)
                    {
                        SetShadowDrawingStates(faceGroup, MaterialOverloads[tagGroup.Tag]);
                        DrawingManager.DrawShadow(faceGroup.IndexBuffer, InstanceCount, IsPrelit);
                    }
                }
            }

            //Draw children
            foreach (var child in Children)
            {
                child.DrawShadow();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if(TransformBuffer != null)
            {
                TransformBuffer.Dispose();
            }

            foreach(var child in Children)
            {
                child.Dispose();
            }
        }
    }
}
