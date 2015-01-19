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
using System.Runtime.InteropServices;
using AlphaMapper.Renderer.InternalComponents;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using ShaderResourceView = SharpDX.Direct3D11.ShaderResourceView;
using DXTextureAddressMode = SharpDX.Direct3D11.TextureAddressMode;
using Device = SharpDX.Direct3D11.Device;
using TextureAddressMode = Bloyteg.AW.Model.RWX.Data.Components.TextureAddressMode;

namespace AlphaMapper.Renderer.Managers
{
    internal class DrawingManager : IDisposable
    {
        private enum DrawingState
        {
            Unknown,
            Standard,
            StandardFlat,
            StandardWirefame,
            Prelit,
            PrelitFlat,
            PrelitWireframe
        }

        private enum PrimitiveTopologyState
        {
            Unknown,
            TriangleList,
            LineList
        }

        private static readonly int MatrixStride = Marshal.SizeOf(typeof (Matrix));

        private readonly VertexLayouts _vertexLayouts;
        private readonly DeviceContext _context;
        private readonly Device _device;

        private readonly EffectTechnique _shadowMapStandard;
        private readonly EffectTechnique _shadowMapPrelit;
        private readonly EffectTechnique _standardSmooth;
        private readonly EffectTechnique _standardFlat;
        private readonly EffectTechnique _standardWireframe;
        private readonly EffectTechnique _prelitSmooth;
        private readonly EffectTechnique _prelitFlat;
        private readonly EffectTechnique _prelitWireframe;

        private readonly EffectMatrixVariable _worldMatrix;
        private readonly EffectMatrixVariable _shadowWorldMatrix;
        private readonly EffectMatrixVariable _postWorldMatrix;
        private readonly EffectMatrixVariable _viewMatrix;
        private readonly EffectMatrixVariable _projectionMatrix;
        private readonly EffectMatrixVariable _lightViewProjectionMatrix;
        private readonly EffectMatrixVariable _shadowLightViewProjectionMatrix;
        private readonly EffectVectorVariable _color;
        private readonly EffectScalarVariable _opacity;
        private readonly EffectScalarVariable _ambient;
        private readonly EffectScalarVariable _diffuse;
        private readonly EffectShaderResourceVariable _texture;
        private readonly EffectShaderResourceVariable _mask;
        private readonly EffectSamplerVariable _textureSampler;
        private readonly EffectScalarVariable _hasTexture;
        private readonly EffectScalarVariable _hasMask;
        private readonly EffectVariable _globalLight;
        private readonly EffectScalarVariable _isInstanced;
        private readonly EffectScalarVariable _isShadowInstanced;
        private readonly EffectScalarVariable _isColorTinted;
        private readonly EffectShaderResourceVariable _shadowMask;
        private readonly EffectScalarVariable _shadowHasMask;

        private EffectTechnique _currentTechnique;
        private DrawingState _currentDrawingState;
        private PrimitiveTopologyState _currentTopologyState;
        private TextureAddressMode _currentTextureAddressMode;

        public DrawingManager(DeviceManager deviceManager)
        {
            _device = deviceManager.Device;
            _context = deviceManager.Context;
            _vertexLayouts = deviceManager.VertexLayouts;

            var effect = deviceManager.Effect;

            _standardSmooth = effect.GetTechniqueByName("Standard|Smooth");
            _standardFlat = effect.GetTechniqueByName("Standard|Flat");
            _standardWireframe = effect.GetTechniqueByName("Standard|Wireframe");
            _prelitSmooth = effect.GetTechniqueByName("Prelit|Smooth");
            _prelitFlat = effect.GetTechniqueByName("Prelit|Flat");
            _prelitWireframe = effect.GetTechniqueByName("Prelit|Wireframe");

            _worldMatrix = effect.GetVariableByName("g_WorldMatrix").AsMatrix();
            _postWorldMatrix = effect.GetVariableByName("g_PostWorldMatrix").AsMatrix();
            _viewMatrix = effect.GetVariableByName("g_ViewMatrix").AsMatrix();
            _projectionMatrix = effect.GetVariableByName("g_ProjectionMatrix").AsMatrix();
            _color = effect.GetVariableByName("g_Color").AsVector();
            _opacity = effect.GetVariableByName("g_Opacity").AsScalar();
            _ambient = effect.GetVariableByName("g_Ambient").AsScalar();
            _diffuse = effect.GetVariableByName("g_Diffuse").AsScalar();
            _texture = effect.GetVariableByName("g_Texture").AsShaderResource();
            _mask = effect.GetVariableByName("g_Mask").AsShaderResource();
            _textureSampler = effect.GetVariableByName("g_TextureSampler").AsSampler();
            _hasTexture = effect.GetVariableByName("g_HasTexture").AsScalar();
            _hasMask = effect.GetVariableByName("g_HasMask").AsScalar();
            _isInstanced = effect.GetVariableByName("g_IsInstanced").AsScalar();
            _isColorTinted = effect.GetVariableByName("g_IsColorTinted").AsScalar();
            _globalLight = effect.GetVariableByName("g_GlobalLight");
            _lightViewProjectionMatrix = effect.GetVariableByName("g_LightViewProjectionMatrix").AsMatrix();

            //Set the states to Unknown initially.
            _currentDrawingState = DrawingState.Unknown;
            _currentTopologyState = PrimitiveTopologyState.Unknown;

            //Set the default of the g_PostWorldMatrix to Identity, so it isn't required to be set.
            _postWorldMatrix.SetMatrix(Matrix.Identity);

            //Setup shadow map technique.
            var shadowEffect = deviceManager.ShadowEffect;
            _shadowMapStandard = shadowEffect.GetTechniqueByName("ShadowMapTechnique|Standard");
            _shadowMapPrelit = shadowEffect.GetTechniqueByName("ShadowMapTechnique|Prelit");

            _isShadowInstanced = shadowEffect.GetVariableByName("g_IsInstanced").AsScalar();
            _shadowWorldMatrix = shadowEffect.GetVariableByName("g_WorldMatrix").AsMatrix();
            _shadowHasMask = shadowEffect.GetVariableByName("g_HasMask").AsScalar();
            _shadowMask = shadowEffect.GetVariableByName("g_Mask").AsShaderResource();
            _shadowLightViewProjectionMatrix = shadowEffect.GetVariableByName("g_LightViewProjectionMatrix").AsMatrix();

        }

        private void UpdateState(DrawingState drawingState, PrimitiveTopologyState topologyState)
        {
            if (drawingState != _currentDrawingState)
            {
                _currentDrawingState = drawingState;
                switch (_currentDrawingState)
                {
                    case DrawingState.Standard:
                        _context.InputAssembler.InputLayout = _vertexLayouts.StandardLayout;
                        _currentTechnique = _standardSmooth;
                        break;

                    case DrawingState.StandardFlat:
                        _context.InputAssembler.InputLayout = _vertexLayouts.StandardLayout;
                        _currentTechnique = _standardFlat;
                        break;

                    case DrawingState.StandardWirefame:
                        _context.InputAssembler.InputLayout = _vertexLayouts.StandardLayout;
                        _currentTechnique = _standardWireframe;
                        break;

                    case DrawingState.Prelit:
                        _context.InputAssembler.InputLayout = _vertexLayouts.PrelitLayout;
                        _currentTechnique = _prelitSmooth;
                        break;

                    case DrawingState.PrelitFlat:
                        _context.InputAssembler.InputLayout = _vertexLayouts.PrelitLayout;
                        _currentTechnique = _prelitFlat;
                        break;

                    case DrawingState.PrelitWireframe:
                        _context.InputAssembler.InputLayout = _vertexLayouts.PrelitLayout;
                        _currentTechnique = _prelitWireframe;
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        
            if(topologyState != _currentTopologyState)
            {
                _currentTopologyState = topologyState;
                switch (_currentTopologyState)
                {
                    case PrimitiveTopologyState.TriangleList:
                        _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                        break;
                    case PrimitiveTopologyState.LineList:
                        _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
                        break;
                }
            }
        }

        private void UpdateShadowState(bool isPrelit)
        {
            _currentDrawingState = DrawingState.Unknown;
            _currentTopologyState = PrimitiveTopologyState.Unknown;

            _context.InputAssembler.InputLayout = isPrelit ? _vertexLayouts.PrelitLayout : _vertexLayouts.StandardLayout;
            _currentTechnique = isPrelit ? _shadowMapPrelit : _shadowMapStandard;
            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }

        private static DrawingState GetDrawingState(bool isPrelit, bool isFlat, bool isWireframe)
        {
            if(isPrelit && isWireframe)
            {
                return DrawingState.PrelitWireframe;
            }

            if (isPrelit && isFlat)
            {
                return DrawingState.PrelitFlat;
            }

            if (isPrelit)
            {
                return DrawingState.Prelit;
            }

            if (isWireframe)
            {
                return DrawingState.StandardWirefame;
            }

            if (isFlat)
            {
                return DrawingState.StandardFlat;
            }

            return DrawingState.Standard;
        }

        private static PrimitiveTopologyState GetPrimitiveTopologyState(bool isWireframe)
        {
            return isWireframe ? PrimitiveTopologyState.LineList : PrimitiveTopologyState.TriangleList;
        }

        private static DXTextureAddressMode GetAddressMode(TextureAddressMode textureAddressMode)
        {
            switch (textureAddressMode)
            {
                case TextureAddressMode.Wrap:
                    return DXTextureAddressMode.Wrap;

                case TextureAddressMode.Clamp:
                    return DXTextureAddressMode.Clamp;

                case TextureAddressMode.Mirror:
                    return DXTextureAddressMode.Mirror;

                default:
                    throw new ArgumentException(@"TextureAddressMode is not recognized.", "textureAddressMode");
            }
        }

        public void Draw(IndexBuffer indexBuffer, bool isPrelit = false, bool isFlat = false, bool isWireframe = false)
        {
            UpdateState(GetDrawingState(isPrelit, isFlat, isWireframe), GetPrimitiveTopologyState(isWireframe));
            _context.InputAssembler.SetIndexBuffer(indexBuffer.Buffer, Format.R16_UInt, 0);
            _isInstanced.Set(false);

            for (int pass = 0; pass < _currentTechnique.Description.PassCount; ++pass)
            {
                _currentTechnique.GetPassByIndex(pass).Apply(_context);
                _context.DrawIndexed(indexBuffer.Count, 0, 0);
            }

        }

        public void DrawShadow(IndexBuffer indexBuffer, bool isPrelit = false)
        {
            UpdateShadowState(isPrelit);
            _context.InputAssembler.SetIndexBuffer(indexBuffer.Buffer, Format.R16_UInt, 0);
            _isShadowInstanced.Set(false);

            for (int pass = 0; pass < _currentTechnique.Description.PassCount; ++pass)
            {
                _currentTechnique.GetPassByIndex(pass).Apply(_context);
                _context.DrawIndexed(indexBuffer.Count, 0, 0);
            }
        }

        public void Draw(IndexBuffer faceGroup, int instanceCount, bool isPrelit = false, bool isFlat = false, bool isWireframe = false)
        {
            UpdateState(GetDrawingState(isPrelit, isFlat, isWireframe), GetPrimitiveTopologyState(isWireframe));
            _context.InputAssembler.SetIndexBuffer(faceGroup.Buffer, Format.R16_UInt, 0);
            _isInstanced.Set(true);

            for (int pass = 0; pass < _currentTechnique.Description.PassCount; ++pass)
            {
                _currentTechnique.GetPassByIndex(pass).Apply(_context);
                _context.DrawIndexedInstanced(faceGroup.Count, instanceCount, 0, 0, 0);
            }
        }

        public void DrawShadow(IndexBuffer faceGroup, int instanceCount, bool isPrelit = false)
        {
            UpdateShadowState(isPrelit);
            _context.InputAssembler.SetIndexBuffer(faceGroup.Buffer, Format.R16_UInt, 0);
            _isShadowInstanced.Set(true);

            for (int pass = 0; pass < _currentTechnique.Description.PassCount; ++pass)
            {
                _currentTechnique.GetPassByIndex(pass).Apply(_context);
                _context.DrawIndexedInstanced(faceGroup.Count, instanceCount, 0, 0, 0);
            }
        }

        public void SetVertexBuffer(DXBuffer vertexBuffer, bool isPrelit)
        {
            var stride = isPrelit ? PrelitVertex.Size : StandardVertex.Size;
            _context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, stride, 0));
        }

        public void SetVertexAndTransformBuffers(DXBuffer vertexBuffer, DXBuffer transformBuffer, bool isPrelit)
        {
            var stride = isPrelit ? PrelitVertex.Size : StandardVertex.Size;
            _context.InputAssembler.SetVertexBuffers(0,
                                                     new[]
                                                         {
                                                             new VertexBufferBinding(vertexBuffer, stride, 0),
                                                             new VertexBufferBinding(transformBuffer, MatrixStride, 0)
                                                         });
        }

        public void SetWorldMatrix(Matrix worldMatrix)
        {
            _shadowWorldMatrix.SetMatrix(worldMatrix);
            _worldMatrix.SetMatrix(worldMatrix);
        }

        public void SetPostWorldMatrix(Matrix postWorldMatrix)
        {
            _postWorldMatrix.SetMatrix(postWorldMatrix);
        }

        public void SetProjectionMatrix(Matrix projectionMatrix)
        {
            _projectionMatrix.SetMatrix(projectionMatrix);
        }

        public void SetViewMatrix(Matrix viewMatrix)
        {
            _viewMatrix.SetMatrix(viewMatrix);
        }

        public void SetOpacity(float opacity)
        {
            _opacity.Set(opacity);
        }

        public void SetAmbient(float ambient)
        {
            _ambient.Set(ambient);
        }

        public void SetDiffuse(float diffuse)
        {
            _diffuse.Set(diffuse);
        }

        public void SetColor(Color4 color)
        {
            _color.Set(color);
        }

        public void SetTexture(ShaderResourceView texture)
        {
            if(texture != null)
            {
                _hasTexture.Set(true);
                _texture.SetResource(texture);
            }
            else
            {
                _hasTexture.Set(false);
            }
        }

        public void SetMask(ShaderResourceView mask)
        {
            if(mask != null)
            {
                _hasMask.Set(true);
                _mask.SetResource(mask);
            }
            else
            {
                _hasMask.Set(false);
            }
        }

        public void SetShadowMask(ShaderResourceView mask)
        {
            if (mask != null)
            {
                _shadowHasMask.Set(true);
                _shadowMask.SetResource(mask);
            }
            else
            {
                _shadowHasMask.Set(false);
            } 
        }

        public void SetGlobalLight(Light light)
        {
            using (var dataStream = new DataStream(Marshal.SizeOf(typeof(Light)), true, true))
            {
                dataStream.Write(light);
                dataStream.Position = 0;
                _globalLight.SetRawValue(dataStream, (int)dataStream.Length);
            }

            var lightDirection = -new Vector3(light.Direction.X, light.Direction.Y, light.Direction.Z);
            var lightView = Matrix.LookAtRH(lightDirection, new Vector3(), new Vector3(0, 1, 0));
            var lightProjection = Matrix.OrthoRH(48.0f, 48.0f, -100.0f, 100.0f);
            var result = lightView * lightProjection;

            _shadowLightViewProjectionMatrix.SetMatrix(result);
            _lightViewProjectionMatrix.SetMatrix(result);
        }

        public void SetTextureAddressMode(TextureAddressMode textureAddressMode)
        {
            if(_currentTextureAddressMode == textureAddressMode)
            {
                return;
            }

            _currentTextureAddressMode = textureAddressMode;

            var samplerState = _textureSampler.GetSampler(0);
            var samplerStateDescription = samplerState.Description;
            var addressMode = GetAddressMode(textureAddressMode);

            samplerStateDescription.AddressU = addressMode;
            samplerStateDescription.AddressV = addressMode;

            var newSamplerState = new SamplerState(_device, samplerStateDescription);
            _textureSampler.SetSampler(0, newSamplerState);

            newSamplerState.Dispose();
            samplerState.Dispose();
        }

        public void SetColorTintState(bool enabled)
        {
            _isColorTinted.Set(enabled);
        }

        public void Dispose()
        {

        } 
    }
}
