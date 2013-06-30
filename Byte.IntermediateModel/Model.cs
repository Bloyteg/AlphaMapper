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
using System.Runtime.Serialization;
using Byte.IntermediateModel.Components;
using Byte.IntermediateModel.Mesh;

namespace Byte.IntermediateModel
{
    [DataContract]
    public enum AxisAlignment
    {
        [EnumMember]
        None,
        [EnumMember]
        ZOrientX,
        [EnumMember]
        ZOrientY,
        [EnumMember]
        XYZ
    }

    [DataContract]
    public class Model
    {
        private List<Prototype> _prototypes = new List<Prototype>();

        [DataMember(Name="Materials")]
        private readonly List<Material> _materials = new List<Material>();

        /// <summary>
        /// Gets the prototypes.
        /// </summary>
        /// <value>The prototypes.</value>
        [DataMember]
        public IList<Prototype> Prototypes
        {
            get { return _prototypes ?? (_prototypes = new List<Prototype>()); }
        }

        /// <summary>
        /// Gets or sets the main clump.
        /// </summary>
        /// <value>The main clump.</value>
        [DataMember(Name="Clump")]
        public Clump MainClump
        {
            get; set;
        }

        public bool HasOpacityFix { get; set; }

        public bool HasRandomUVs { get; set; }

        public bool IsSeamless { get; set; }

        public AxisAlignment AxisAlignment { get; set; }

        /// <summary>
        /// Adds the material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns>The ID of material.</returns>
        public int AddMaterial(Material material)
        {
            foreach (var currentMaterial in _materials
                .Select((currentMaterial, index) => new { Material = currentMaterial, Index = index })
                .Where(currentMaterial => currentMaterial.Material.GetHashCode() == material.GetHashCode() && currentMaterial.Material == material))
            {
                return currentMaterial.Index;
            }

            _materials.Add(material);
            return _materials.Count - 1;
        }

        /// <summary>
        /// Gets the material.
        /// </summary>
        /// <param name="materialId">The material id.</param>
        /// <returns></returns>
        public Material GetMaterial(int materialId)
        {
            if (!HasMaterial(materialId))
            {
                throw new ArgumentOutOfRangeException("materialId", "The given material id does not exist.");
            }

            return _materials[materialId];
        }

        /// <summary>
        /// Tries to get the material.
        /// </summary>
        /// <param name="materialId">The material id.</param>
        /// <param name="material">The material.</param>
        /// <returns></returns>
        public bool TryGetMaterial(int materialId, ref Material material)
        {
            if (!HasMaterial(materialId))
            {
                return false;
            }

            material = _materials[materialId];
            return true;
        }

        /// <summary>
        /// Determines whether the specified material id has material.
        /// </summary>
        /// <param name="materialId">The material id.</param>
        /// <returns>
        ///   <c>true</c> if the specified material id has material; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMaterial(int materialId)
        {
            return (materialId >= 0) && (materialId < _materials.Count);
        }

        /// <summary>
        /// Loads from XML file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static Model LoadFromXmlFile(string filePath)
        {
            var serializer = new DataContractSerializer(typeof(Model));

            using (var file = File.OpenRead(filePath))
            {
                var result = serializer.ReadObject(file);
                return result as Model;
            }
        }

        /// <summary>
        /// Loads from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static Model LoadFromStream(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(Model));
            var result = serializer.ReadObject(stream);
            return result as Model;
        }

        /// <summary>
        /// Saves to XML file.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="filePath">The file path.</param>
        public static void SaveToXmlFile(Model model, string filePath)
        {
            var serializer = new DataContractSerializer(typeof (Model));

            using (var file = new FileStream(filePath, FileMode.Create))
            {
                serializer.WriteObject(file, model);
            }
        }

        /// <summary>
        /// Saves to stream.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        public static void SaveToStream(Model model, Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(Model));

            serializer.WriteObject(stream, model);
        }
    }
}
