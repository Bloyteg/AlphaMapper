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
using System.IO;
using System.Linq;
using AlphaMapper.Renderer.Components;
using AlphaMapper.Renderer.Drawables;
using AlphaMapper.Renderer.InternalComponents;
using MrByte.RWX.Model;
using SharpDX.Direct3D11;

namespace AlphaMapper.Renderer.Managers
{
    internal partial class ResourceManager : IDisposable
    {
        private readonly Device _device;
        private readonly Dictionary<string, ShaderResourceView> _textures = new Dictionary<string, ShaderResourceView>();
        private readonly Dictionary<string, Model> _models = new Dictionary<string, Model>();
        private readonly Dictionary<string, MeshCache> _cachedMeshes = new Dictionary<string, MeshCache>();
        private readonly Dictionary<string, Dictionary<string, MeshCache>> _cachedMeshPrototypes = new Dictionary<string, Dictionary<string, MeshCache>>();
        private readonly string _filePath = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManager"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="filePath">The file path.</param>
        public ResourceManager(Device device, string filePath)
        {
            _device = device;
            _filePath = filePath;
        }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <param name="textureName">Name of the texture.</param>
        /// <param name="isMask">if set to <c>true</c> [is mask].</param>
        /// <returns></returns>
        public ShaderResourceView GetTexture(string textureName, bool isMask)
        {
            if (string.IsNullOrWhiteSpace(textureName))
            {
                return null;
            }

            if (_textures.ContainsKey(textureName))
            {
                return _textures[textureName];
            }

            string fileExtension = Path.GetExtension(textureName);
            string fileName = _filePath + textureName;

            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                fileName += isMask ? ".bmp" : ".jpg";
            }

            if (File.Exists(fileName))
            {
                _textures.Add(textureName, ShaderResourceView.FromFile(_device, fileName));
                return _textures[textureName];
            }

            return null;
        }

        public void AddModel(string name, Model model)
        {
            _models.Add(name, model);
        }

        public MeshDrawable LoadMesh(string name, IEnumerable<MaterialOverload> materialOverloads)
        {
            Model model = _models[name];

            if (!_cachedMeshes.ContainsKey(name))
            {
                _cachedMeshes.Add(name, GetMeshCacheFromModel(name, model));
            }

            var mesh = BuildMeshFromMeshCache<MeshDrawable>(name, _cachedMeshes[name]);
            BuildMaterialOverloads(mesh, materialOverloads);
            return mesh;
        }


        public MeshBatchDrawable LoadMeshBatch(string name,
                                               IEnumerable<MeshBatchInstanceTransform> instanceTransforms,
                                               IEnumerable<MaterialOverload> materialOverloads)
        {
            Model model = _models[name];

            if (!_cachedMeshes.ContainsKey(name))
            {
                _cachedMeshes.Add(name, GetMeshCacheFromModel(name, model));
            }

            var meshBatch = BuildMeshFromMeshCache<MeshBatchDrawable>(name, _cachedMeshes[name]);
            BuildTransformBuffers(meshBatch, instanceTransforms);
            BuildMaterialOverloads(meshBatch, materialOverloads);
            return meshBatch;
        }

        public void FreeResources()
        {
            //Free up all of the textures.
            foreach (var texture in _textures)
            {
                texture.Value.Dispose();
            }

            //Free up all of the cached meshes
            foreach (var mesh in _cachedMeshes)
            {
                mesh.Value.Dispose();
            }

            foreach (var prototype in _cachedMeshPrototypes.SelectMany(prototypeCache => prototypeCache.Value))
            {
                prototype.Value.Dispose();
            }

            _textures.Clear();
            _cachedMeshes.Clear();
            _cachedMeshPrototypes.Clear();
            _models.Clear();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            FreeResources();
        }
    }
}
