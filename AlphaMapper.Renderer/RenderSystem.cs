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
using System.Windows.Forms;
using AlphaMapper.Renderer.Components;
using AlphaMapper.Renderer.Drawables;
using AlphaMapper.Renderer.InternalComponents;
using AlphaMapper.Renderer.Managers;
using MrByte.RWX.Model;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Resource = SharpDX.Direct3D11.Resource;

namespace AlphaMapper.Renderer
{
    public class RenderSystem : IDisposable
    {
        public event Action DrawFrame;
        public event Action DrawShadowFrame;

        private readonly DeviceManager _deviceManager;
        private readonly DrawingManager _drawingManager;
        private readonly ResourceManager _resourceManager;

        private bool _disposed;

        public RenderSystem(Control targetControl, string filePath)
        {
            _deviceManager = new DeviceManager(targetControl);
            _drawingManager = new DrawingManager(_deviceManager);
            _resourceManager = new ResourceManager(_deviceManager.Device, filePath);
        }

        ~RenderSystem()
        {
            if(!_disposed)
            {
                Dispose();
            }
        }

        public void AddModel(string modelName, Model model)
        {
            _resourceManager.AddModel(modelName, model);
        }

        public MeshDrawable GetMeshDrawable(string modelName, ISet<MaterialOverload> materialOverloads = null)
        {
            var mesh = _resourceManager.LoadMesh(modelName, materialOverloads);
            mesh.DrawingManager = _drawingManager;
            SetChildDrawingManager(mesh, _drawingManager);

            return mesh;
        }

        public MeshBatchDrawable GetMeshBatchDrawable(string modelName, IEnumerable<MeshBatchInstanceTransform> instanceTransforms, ISet<MaterialOverload> materialOverloads = null)
        {
            var mesh = _resourceManager.LoadMeshBatch(modelName, instanceTransforms, materialOverloads);
            mesh.DrawingManager = _drawingManager;
            SetChildDrawingManager(mesh, _drawingManager);
            return mesh;
        }

        private static void SetChildDrawingManager(MeshDrawableBase mesh, DrawingManager drawingManager)
        {
            foreach (MeshDrawableBase child in mesh.Children)
            {
                child.DrawingManager = drawingManager;
                SetChildDrawingManager(child, drawingManager);
            }
        }

        public void SetCamera(Camera camera)
        {
            var matrix = Matrix.LookAtRH(camera.Eye.ToDXVector3(), camera.Target.ToDXVector3(), camera.Up.ToDXVector3());
            _drawingManager.SetViewMatrix(matrix);
        }

        public void SetOrthographicProjection(float width, float height, float zNear, float zFar)
        {
            var matrix = Matrix.OrthoRH(width, height, zNear, zFar);
            _drawingManager.SetProjectionMatrix(matrix);
        }

        public void SetPerspectiveProjection(float fieldOfView, float aspect, float zNear, float zFar)
        {
            var projection = Matrix.PerspectiveFovRH(fieldOfView, aspect, zNear, zFar);
            _drawingManager.SetProjectionMatrix(projection);
        }

        public void SetGlobalLight(Components.Light light)
        {
            _drawingManager.SetGlobalLight(light.ToInternalLight());
        }

        public void SetPostWorldTransform(MrByte.Math.Matrix4 matrix)
        {
            _drawingManager.SetPostWorldMatrix(Matrix.Transpose(matrix.ToDXMatrix()));
        }

        public void DrawSingleFrame()
        {
            if (DrawShadowFrame != null)
            {
                _deviceManager.StartShadowMapRender();
                DrawShadowFrame();
            }

            if (DrawFrame != null)
            {
                _deviceManager.StartSceneRender();
                DrawFrame();
            }

            _deviceManager.SwapChain.Present(0, PresentFlags.None);
        }

        public void DrawContinuousFrames(Form mainWindow)
        {
           // MessagePump.Run(mainWindow, DrawSingleFrame);
        }

        public void SaveFrameToFile(string fileName)
        {
            using (var texture = Resource.FromSwapChain<Texture2D>(_deviceManager.SwapChain, 0))
            using (var outTexture = new Texture2D(_deviceManager.Device,
                                                  new Texture2DDescription
                                                      {
                                                          Width = texture.Description.Width,
                                                          Height = texture.Description.Height,
                                                          MipLevels = 1,
                                                          ArraySize = 1,
                                                          Format = Format.R8G8B8A8_UNorm,
                                                          SampleDescription = new SampleDescription(1, 0),
                                                          Usage = ResourceUsage.Default,
                                                          BindFlags = BindFlags.RenderTarget,
                                                          CpuAccessFlags = CpuAccessFlags.None,
                                                          OptionFlags = texture.Description.OptionFlags
                                                      }))
            {
                _deviceManager.Context.ResolveSubresource(texture, 0, outTexture, 0, outTexture.Description.Format);

                Resource.ToFile(_deviceManager.Context, outTexture, ImageFileFormat.Jpg, fileName);
            }
        }

        public void FreeResources()
        {
            _resourceManager.FreeResources();
        }

        public void Dispose()
        {
            _deviceManager.Dispose();
            _resourceManager.Dispose();
            _drawingManager.Dispose();

            _disposed = true;
        }
    }
}
