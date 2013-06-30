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
using System.Windows.Forms;
using AlphaMapper.Renderer.InternalComponents;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;

namespace AlphaMapper.Renderer.Managers
{
    internal class DeviceManager : IDisposable
    {
        #region Fields

        private readonly Control _targetControl;
        private Texture2DDescription _depthStencilDescription;

        private const string PositionFieldName = "_position";
        private const string UVFieldName = "_uv";
        private const string NormalFieldName = "_normal";
        private const string ColorFieldName = "_color";
        private const string PositionElementName = "POSITION";
        private const string UVElementName = "TEXCOORD";
        private const string NormalElementName = "NORMAL";
        private const string ColorElementName = "COLOR";
        private const string TransformName = "TRANSFORM";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceManager"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public DeviceManager(Control target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            _targetControl = target;
            _targetControl.Resize += OnTargetControlResize;
            InitializeRendererComponents();
        }

        #endregion

        #region Properties

        public DeviceContext Context { get; private set; }
        public Device Device { get; private set; }
        public Effect Effect { get; private set; }
        public Effect ShadowEffect { get; private set; }
        public VertexLayouts VertexLayouts { get; private set; }

        //Shadow map rendering
        public DepthStencilViewResource ShadowMapDepthStencilResource { get; private set; }

        //Forward rendering pass
        public DepthStencilView DepthStencilView { get; private set; }
        public RenderTargetView RenderTargetView { get; private set; }
        public SwapChain SwapChain { get; private set; }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            RenderTargetView.Dispose();
            DepthStencilView.Dispose();
            ShadowMapDepthStencilResource.Dispose();
            SwapChain.Dispose();
            VertexLayouts.Dispose();
            Effect.Dispose();
            ShadowEffect.Dispose();
            Context.Dispose();
            Device.Dispose();
        }

        #endregion

        #region Events

        public event Action<Control> RenderTargetResize;

        #endregion

        /// <summary>
        /// Called when target control resized.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTargetControlResize(object sender, EventArgs e)
        {
            RenderTargetView.Dispose();
            DepthStencilView.Dispose();

            SwapChain.ResizeBuffers(2,
                                    _targetControl.ClientSize.Width,
                                    _targetControl.ClientSize.Height,
                                    Format.R8G8B8A8_UNorm,
                                    SwapChainFlags.AllowModeSwitch);

            using (var resource = Resource.FromSwapChain<Texture2D>(SwapChain, 0))
            {
                RenderTargetView = new RenderTargetView(Device, resource);
            }

            SetupDepthStencilView(Device);

            if (RenderTargetResize != null)
            {
                RenderTargetResize(_targetControl);
            }
        }

        /// <summary>
        /// Initializes the renderer components.
        /// </summary>
        private void InitializeRendererComponents()
        {
            Device = GetDevice();
            Context = Device.ImmediateContext;
            SetupRenderTarget(Device);
            Effect = GetEffect(Device, Resources.effect);
            ShadowEffect = GetEffect(Device, Resources.effect_shadow);
            VertexLayouts = GetVertexLayouts(Device, Effect);

            using (var factory = SwapChain.GetParent<Factory>())
            {
                factory.SetWindowAssociation(_targetControl.Handle, WindowAssociationFlags.IgnoreAltEnter);
            }
        }

        /// <summary>
        /// Gets the device.
        /// </summary>_targetControl
        /// <returns></returns>
        private Device GetDevice()
        {
            Device device;

            var description = new SwapChainDescription
                                  {
                                      BufferCount = 2,
                                      Usage = Usage.RenderTargetOutput,
                                      OutputHandle = _targetControl.Handle,
                                      IsWindowed = true,
                                      ModeDescription = new ModeDescription(_targetControl.ClientSize.Width,
                                                                            _targetControl.ClientSize.Height,
                                                                            new Rational(60, 1),
                                                                            Format.R8G8B8A8_UNorm),
                                      SampleDescription = new SampleDescription(8, 0),
                                      Flags = SwapChainFlags.AllowModeSwitch,
                                      SwapEffect = SwapEffect.Discard
                                  };

            SwapChain swapChain;
            Device.CreateWithSwapChain(DriverType.Hardware,
                                       DeviceCreationFlags.None,
                                       new[] { FeatureLevel.Level_11_0 }, 
                                       description, 
                                       out device,
                                       out swapChain);
            SwapChain = swapChain;

            return device;
        }

        /// <summary>
        /// Gets the effect.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="effectBytes">The effect bytes.</param>
        /// <returns></returns>
        private static Effect GetEffect(Device device, byte[] effectBytes)
        {
            Effect effect;

            using (var dataStream = new DataStream(effectBytes.Length, true, true))
            {
                dataStream.WriteRange(effectBytes);
                dataStream.Position = 0;

                using (var bytecode = new ShaderBytecode(dataStream))
                {
                    effect = new Effect(device, bytecode);
                }
            }

            return effect;
        }

        /// <summary>
        /// Gets the vertex layouts.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="effect">The effect.</param>
        /// <returns></returns>
        private static VertexLayouts GetVertexLayouts(Device device, Effect effect)
        {
            return new VertexLayouts(GetStandardVertexLayout(device, effect),
                                     GetPrelitVertexLayout(device, effect));
        }

        /// <summary>
        /// Gets the standard vertex layout.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="effect">The effect.</param>
        /// <returns></returns>
        private static InputLayout GetStandardVertexLayout(Device device, Effect effect)
        {
            var type = typeof(StandardVertex);
            var positionOffset = Marshal.OffsetOf(type, PositionFieldName).ToInt32();
            var uvOffset = Marshal.OffsetOf(type, UVFieldName).ToInt32();
            var normalOffset = Marshal.OffsetOf(type, NormalFieldName).ToInt32();

            var elements = new[]
                               {
                                   new InputElement(PositionElementName, 0, Format.R32G32B32_Float, positionOffset, 0, InputClassification.PerVertexData, 0),
                                   new InputElement(UVElementName, 0, Format.R32G32_Float, uvOffset, 0, InputClassification.PerVertexData, 0),
                                   new InputElement(NormalElementName, 0, Format.R32G32B32_Float, normalOffset, 0, InputClassification.PerVertexData, 0),
                                   new InputElement(TransformName, 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                                   new InputElement(TransformName, 1, Format.R32G32B32A32_Float, 16, 1, InputClassification.PerInstanceData, 1),
                                   new InputElement(TransformName, 2, Format.R32G32B32A32_Float, 32, 1, InputClassification.PerInstanceData, 1),
                                   new InputElement(TransformName, 3, Format.R32G32B32A32_Float, 48, 1, InputClassification.PerInstanceData, 1),
                               };

            return BuildLayout(effect, device, Resources.StandardSmoothTechnique, elements);
        }

        /// <summary>
        /// Gets the prelit vertex layout.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="effect">The effect.</param>
        /// <returns></returns>
        private static InputLayout GetPrelitVertexLayout(Device device, Effect effect)
        {
            var type = typeof(PrelitVertex);
            var positionOffset = Marshal.OffsetOf(type, PositionFieldName).ToInt32();
            var uvOffset = Marshal.OffsetOf(type, UVFieldName).ToInt32();
            var normalOffset = Marshal.OffsetOf(type, NormalFieldName).ToInt32();
            var colorOffset = Marshal.OffsetOf(type, ColorFieldName).ToInt32();

            var elements = new[]
                               {
                                   new InputElement(PositionElementName, 0, Format.R32G32B32_Float, positionOffset, 0, InputClassification.PerVertexData, 0),
                                   new InputElement(UVElementName, 0, Format.R32G32_Float, uvOffset, 0, InputClassification.PerVertexData, 0),
                                   new InputElement(NormalElementName, 0, Format.R32G32B32_Float, normalOffset, 0, InputClassification.PerVertexData, 0),
                                   new InputElement(ColorElementName, 0, Format.R32G32B32_Float, colorOffset, 0, InputClassification.PerVertexData, 0),
                                   new InputElement(TransformName, 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                                   new InputElement(TransformName, 1, Format.R32G32B32A32_Float, 16, 1, InputClassification.PerInstanceData, 1),
                                   new InputElement(TransformName, 2, Format.R32G32B32A32_Float, 32, 1, InputClassification.PerInstanceData, 1),
                                   new InputElement(TransformName, 3, Format.R32G32B32A32_Float, 48, 1, InputClassification.PerInstanceData, 1),
                               };

            return BuildLayout(effect, device, Resources.PrelitSmoothTechnique, elements);
        }

        /// <summary>
        /// Builds the layout.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <param name="device">The device.</param>
        /// <param name="techniqueName">Name of the technique.</param>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        private static InputLayout BuildLayout(Effect effect, Device device, string techniqueName, InputElement[] elements)
        {
            EffectTechnique technique = effect.GetTechniqueByName(techniqueName);
            var passDescription = technique.GetPassByIndex(0).Description;
            return new InputLayout(device, passDescription.Signature, elements);
        }

        /// <summary>
        /// Sets up the render target.
        /// </summary>
        /// <param name="device">The device.</param>
        private void SetupRenderTarget(Device device)
        {
            //Create a render target.
            SetupRenderTargetView(device);

            //Create a depth stencil view.
            SetupDepthStencilView(device);

            //Create a depth stenvil view for shadow mapping.
            SetupShadowMapDepthStencilView(device);
        }

        /// <summary>
        /// Starts the shadow map render.
        /// </summary>
        public void StartShadowMapRender()
        {
            Effect.GetVariableByName("g_ShadowMap").AsResource().SetResource(null);

            //Set output merger stage.
            Context.OutputMerger.SetTargets(ShadowMapDepthStencilResource, (RenderTargetView)null);

            //Setup the view port.
            var viewport = new Viewport(0.0f, 0.0f, 4096f, 4096f);
            Context.Rasterizer.SetViewports(viewport);

            //Clear views.
            Context.ClearDepthStencilView(ShadowMapDepthStencilResource, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        /// <summary>
        /// Starts the scene render.
        /// </summary>
        public void StartSceneRender()
        {
            Context.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);

            Effect.GetVariableByName("g_ShadowMap").AsResource().SetResource(ShadowMapDepthStencilResource);

            //Setup the view port.
            var viewport = new Viewport(0.0f, 0.0f, _targetControl.ClientSize.Width, _targetControl.ClientSize.Height);
            Context.Rasterizer.SetViewports(viewport);

            //Clear views.
            Context.ClearRenderTargetView(RenderTargetView, new Color4(0, 0, 0));
            Context.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        /// <summary>
        /// Setups the render target view.
        /// </summary>
        /// <param name="device">The device.</param>
        private void SetupRenderTargetView(Device device)
        {
            using (var resource = Resource.FromSwapChain<Texture2D>(SwapChain, 0))
            {
                RenderTargetView = new RenderTargetView(device, resource);
            }
        }

        /// <summary>
        /// Setups the depth stencil view.
        /// </summary>
        /// <param name="device">The device.</param>
        private void SetupDepthStencilView(Device device)
        {
            if(DepthStencilView != null)
            {
                DepthStencilView.Dispose();
            }

            _depthStencilDescription = new Texture2DDescription
                                           {
                                               Width = _targetControl.ClientSize.Width,
                                               Height = _targetControl.ClientSize.Height,
                                               MipLevels = 1,
                                               ArraySize = 1,
                                               Format = Format.D32_Float,
                                               SampleDescription = new SampleDescription(8, 0),
                                               Usage = ResourceUsage.Default,
                                               BindFlags = BindFlags.DepthStencil,
                                               CpuAccessFlags = CpuAccessFlags.None,
                                               OptionFlags = ResourceOptionFlags.None
                                           };

            //Setup a depth stenvil view.
            using (var depthStencil = new Texture2D(device, _depthStencilDescription))
            {
                DepthStencilView = new DepthStencilView(device, depthStencil);
            }
        }

        /// <summary>
        /// Setups the shadow map depth stencil view.
        /// </summary>
        /// <param name="device">The device.</param>
        private void SetupShadowMapDepthStencilView(Device device)
        {
            const int bounds = 4096;

            var textureDescription = new Texture2DDescription
            {
                Width = bounds,
                Height = bounds,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.R32_Typeless,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            var shaderResourceViewDescription = new ShaderResourceViewDescription
                                                {
                                                    Format = Format.R32_Float,
                                                    Dimension = ShaderResourceViewDimension.Texture2D,
                                                    MipLevels = textureDescription.MipLevels,
                                                    MostDetailedMip = 0
                                                };

            var depthStencilViewDescription = new DepthStencilViewDescription
                                                  {
                                                      Format = Format.D32_Float,
                                                      Dimension = DepthStencilViewDimension.Texture2D,
                                                      MipSlice = 0
                                                  };

            //Setup a depth stencill resource.
            ShadowMapDepthStencilResource = new DepthStencilViewResource(device, textureDescription,
                                                                         depthStencilViewDescription,
                                                                         shaderResourceViewDescription);
        }
    }
}
