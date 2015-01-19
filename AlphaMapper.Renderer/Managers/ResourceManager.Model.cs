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
using System.Runtime.InteropServices;
using AlphaMapper.Renderer.Components;
using AlphaMapper.Renderer.Drawables;
using AlphaMapper.Renderer.InternalComponents;
using AlphaMapper.Renderer.Utility;
using Bloyteg.AW.Model.RWX.Data;
using Bloyteg.AW.Model.RWX.Data.Components;
using Bloyteg.AW.Model.RWX.Data.Mesh;
using Bloyteg.AW.Model.RWX.Data.Primitive;
using SharpDX;
using SharpDX.Direct3D11;
using DXBuffer = SharpDX.Direct3D11.Buffer;

namespace AlphaMapper.Renderer.Managers
{
    internal partial class ResourceManager
    {
        private readonly Stack<Matrix> _matrixStack = new Stack<Matrix>();
        private Matrix _currentMatrix = Matrix.Identity;

        private MeshCache GetMeshCacheFromModel(string name, Model model)
        {
            //Construct the cache of the model's prototypes.
            if(!_cachedMeshPrototypes.ContainsKey(name))
            {
                _cachedMeshPrototypes.Add(name, new Dictionary<string, MeshCache>());
            }

            if (model.Prototypes != null)
            {
                foreach (var prototype in model.Prototypes)
                {
                    _cachedMeshPrototypes[name].Add(prototype.Name, GetMeshCacheFromMeshGeometry(model, prototype));
                }
            }

            //Construct a cache of the model itself.
            return GetMeshCacheFromMeshGeometry(model, model.MainClump);
        }

        private TMesh BuildMeshFromMeshCache<TMesh>(string name, MeshCache cachedMesh)
            where TMesh : MeshDrawableBase, new()
        {
            _matrixStack.Push(_currentMatrix);
            _currentMatrix *= cachedMesh.LocalMatrix;

            var mesh = new TMesh
                           {
                               ModelMatrix = _currentMatrix,
                               IsPrelit = cachedMesh.IsPrelit,
                               Tag = cachedMesh.Tag,
                               VertexBuffer = cachedMesh.VertexBuffer
                           };

            mesh.TagGroups.AddRange(from tagGroup in cachedMesh.TagGroups
                                    select new WeakReference<TagGroup>(tagGroup));

            //Add child clumps.
            foreach (var childMesh in cachedMesh.Children.Select(child => BuildMeshFromMeshCache<TMesh>(name, child)))
            {
                mesh.AddChild(childMesh);
            }

            //Add prototype references.
            foreach (var prototypeReference in cachedMesh.PrototypeReferences)
            {
                mesh.AddChild(BuildPrototypeFromMeshCacheReference<TMesh>(name, prototypeReference));
            }

            _currentMatrix = _matrixStack.Pop();

            return mesh;
        }

        private TMesh BuildPrototypeFromMeshCacheReference<TMesh>(string name, MeshCacheReference meshCacheReference)
            where TMesh : MeshDrawableBase, new()
        {
            _matrixStack.Push(_currentMatrix);
            _currentMatrix = _currentMatrix*meshCacheReference.LocalMatrix;

            var cachedPrototype = _cachedMeshPrototypes[name][meshCacheReference.Name];

            var prototypeMesh = new TMesh
                                    {
                                        VertexBuffer = cachedPrototype.VertexBuffer,
                                        ModelMatrix = _currentMatrix,
                                        IsPrelit = cachedPrototype.IsPrelit
                                    };

            prototypeMesh.TagGroups.AddRange(from tagGroup in cachedPrototype.TagGroups
                                             select new WeakReference<TagGroup>(tagGroup));

            //Add each of the clumps to the prototype.
            foreach (var childMesh in cachedPrototype.Children)
            {
                prototypeMesh.AddChild(BuildMeshFromMeshCache<TMesh>(name, childMesh));
            }

            //Add each of the sub-prototypes to the prototype.
            foreach(var cacheReference in cachedPrototype.PrototypeReferences)
            {
                prototypeMesh.AddChild(BuildPrototypeFromMeshCacheReference<TMesh>(name, cacheReference));
            }

            _currentMatrix = _matrixStack.Pop();

            return prototypeMesh;
        }

        private MeshCache GetMeshCacheFromMeshGeometry(Model model, MeshGeometry geometry)
        {
            //Load vertex buffer.
            DXBuffer vertexBuffer = GetVertexBuffer(geometry);

            //Group index buffers by tag and material ID.
            IEnumerable<TagGroup> tagGroups = BuildGeometryTagGroups(model, geometry);

            //Build mesh cache
            MeshCache meshCache;

            var clump = geometry as Clump;
            if (clump != null)
            {
                meshCache = new MeshCache(vertexBuffer,
                                          tagGroups,
                                          clump.Transform.ToDXMatrix(),
                                          clump.IsPrelit,
                                          clump.Tag);
            }
            else
            {
                meshCache = new MeshCache(vertexBuffer, tagGroups);   
            }

            //Add appropriate references to prototypes.
            if (geometry.PrototypeInstances != null)
            {
                foreach (var prototypeInstance in geometry.PrototypeInstances)
                {
                    meshCache.PrototypeReferences.Add(new MeshCacheReference(prototypeInstance.Name,
                                                                             prototypeInstance.Transform.ToDXMatrix()));
                }
            }

            //Build children.
            if (geometry.Children != null)
            {
                foreach (var meshGeometry in geometry.Children)
                {
                    meshCache.Children.Add(GetMeshCacheFromMeshGeometry(model, meshGeometry));
                }
            }

            //Build prims.
            if (geometry.Primitives != null)
            {
                foreach (var primitiveGeometry in geometry.Primitives)
                {
                    meshCache.Children.Add(GetMeshCacheFromPrimitiveGeometry(model, primitiveGeometry));
                }
            }

            return meshCache;
        }

        private MeshCache GetMeshCacheFromPrimitiveGeometry(Model model, PrimitiveGeometry geometry)
        {
            //Load vertex buffer.
            DXBuffer vertexBuffer = GetVertexBuffer(geometry);

            //Group index buffers by tag and material ID.
            IEnumerable<TagGroup> tagGroups = BuildGeometryTagGroups(model, geometry);

            //Build mesh cache
            return new MeshCache(vertexBuffer, tagGroups, geometry.Transform.ToDXMatrix());
        }

        private DXBuffer GetVertexBuffer(IGeometry meshGeometry)
        {
            if (meshGeometry.Vertices.Count == 0)
            {
                return null;
            }

            var isPrelit = IsPrelit(meshGeometry);
            var vertexSize = isPrelit ? PrelitVertex.Size : StandardVertex.Size;

            using (var stream = new DataStream(meshGeometry.Vertices.Count*vertexSize, true, true))
            {
                if (isPrelit)
                {
                    stream.WriteRange(meshGeometry.Vertices.Select(vertex => new PrelitVertex(vertex.Position.ToDXVector3(),
                                                                                              vertex.UV.ToDXVector2(),
                                                                                              vertex.Prelight.ToDXColor3(),
                                                                                              vertex.Normal.ToDXVector3())).ToArray());
                }
                else
                {
                    stream.WriteRange(meshGeometry.Vertices.Select(vertex => new StandardVertex(vertex.Position.ToDXVector3(),
                                                                                                vertex.UV.ToDXVector2(),
                                                                                                vertex.Normal.ToDXVector3())).ToArray()); 
                }

                stream.Position = 0;
                return new DXBuffer(_device,
                                    stream,
                                    (int) stream.Length,
                                    ResourceUsage.Default,
                                    BindFlags.VertexBuffer,
                                    CpuAccessFlags.None,
                                    ResourceOptionFlags.None,
                                    vertexSize);
            }

        }

        private static bool IsPrelit(IGeometry meshGeometry)
        {
            return meshGeometry is MeshGeometry && ((MeshGeometry)meshGeometry).IsPrelit;
        }

        private IEnumerable<TagGroup> BuildGeometryTagGroups(Model model, IGeometry geometry)
        {
            return (from face in geometry.Faces
                    group face by face.Tag
                    into tagGroups
                    select new TagGroup
                               {
                                   Tag = tagGroups.Key,
                                   FaceGroups = (from face in tagGroups
                                                 group face by face.MaterialId
                                                 into faceGroup
                                                 let material = model.Materials[faceGroup.Key]
                                                 let isWireframe = material.GeometrySampling == GeometrySampling.Wireframe
                                                 let texture = isWireframe ? null : GetTexture(material.Texture, false)
                                                 let mask = isWireframe ? null : GetTexture(material.Mask, true)
                                                 let indices = isWireframe
                                                                   ? (from faces in faceGroup
                                                                      from index in ProcessIndices(faces.Indices)
                                                                      select index)
                                                                   : (from faces in faceGroup
                                                                      from triangle in faces.Triangles
                                                                      from index in triangle.Indices
                                                                      select (ushort) index)
                                                 select new FaceGroup
                                                            {
                                                                IndexBuffer = new IndexBuffer(_device, indices.ToArray()),
                                                                Material = material,
                                                                Texture = new WeakReference<ShaderResourceView>(texture),
                                                                Mask = new WeakReference<ShaderResourceView>(mask)
                                                            }).ToList(),
                               }).ToList();
        }

        private static IEnumerable<ushort> ProcessIndices(IEnumerable<int> indices)
        {
            var indexList = indices.ToList();

            var count = indexList.Count;
            
            for(int index = 0; index < count; ++index)
            {
                yield return (ushort) indexList[index];
                yield return (ushort) indexList[(index + 1) % count];
            }
        }

        private void BuildTransformBuffers(MeshBatchDrawable meshBatch, IEnumerable<MeshBatchInstanceTransform> instanceTransforms)
        {
            int matrixSize = Marshal.SizeOf(typeof (Matrix));
            var transforms = instanceTransforms.ToList();

            int instanceCount = transforms.Count;

            using (var stream = new DataStream(matrixSize * instanceCount, true, true))
            {
                var matrices = (from transform in transforms
                                select Matrix.Transpose(meshBatch.ModelMatrix*transform.WorldMatrix)).ToArray();

                stream.WriteRange(matrices);
                stream.Position = 0;

                var matrixBuffer = new DXBuffer(_device,
                                                stream,
                                                (int) stream.Length,
                                                ResourceUsage.Immutable,
                                                BindFlags.VertexBuffer,
                                                CpuAccessFlags.None,
                                                ResourceOptionFlags.None,
                                                matrixSize);

                meshBatch.TransformBuffer = matrixBuffer;
                meshBatch.InstanceCount = instanceCount;
            }

            foreach(var child in meshBatch.Children.OfType<MeshBatchDrawable>())
            {
                BuildTransformBuffers(child, transforms);
            }
        }

        private void BuildMaterialOverloads(MeshDrawableBase mesh, IEnumerable<MaterialOverload> materialOverloads)
        {
            var overloads = materialOverloads == null ? new List<MaterialOverload>() : materialOverloads.ToList();

            foreach (WeakReference<TagGroup> tagGroup in mesh.TagGroups)
            {
                mesh.MaterialOverloads.Add(tagGroup.GetTarget().Tag, BuildTagGroupMaterialOverloads(tagGroup.GetTarget().Tag.Value, overloads));
            }

            foreach(var child in mesh.Children.OfType<MeshDrawableBase>())
            {
                BuildMaterialOverloads(child, overloads);
            }
        }

        private TagGroupMaterialOverload BuildTagGroupMaterialOverloads(int? tag, IEnumerable<MaterialOverload> materialOverloads)
        {
            var overloads = from materialOverload in (materialOverloads ?? Enumerable.Empty<MaterialOverload>())
                            where materialOverload.Tag == null || materialOverload.Tag == tag
                            orderby materialOverload.Tag descending
                            select materialOverload;

            var result = new TagGroupMaterialOverload();

            if (overloads.Any())
            {
                var overload = overloads.FirstOrDefault() ?? overloads.Last();

                result.Texture = new WeakReference<ShaderResourceView>(GetTexture(overload.Texture, false));
                result.Mask = new WeakReference<ShaderResourceView>(GetTexture(overload.Mask, true));
                result.Opacity = overload.Opacity;
                result.Color = overload.Color;
                result.IsColorTint = overload.IsColorTint;
            }

            return result;
        }
    }
}
