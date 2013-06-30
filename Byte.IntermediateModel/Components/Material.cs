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
using System.Runtime.Serialization;
using Byte.Utility;

namespace Byte.IntermediateModel.Components
{
    [DataContract]
    public enum GeometrySampling
    {
        [EnumMember]
        Solid,
        [EnumMember]
        Wireframe,
        [EnumMember]
        Pointcloud
    }

    [DataContract]
    public enum LightSampling
    {
        [EnumMember]
        Facet,
        [EnumMember]
        Vertex
    }

    [DataContract]
    [Flags]
    public enum TextureMode
    {
        [EnumMember]
        Null = 0x00,
        [EnumMember]
        Lit = 0x01,
        [EnumMember]
        Foreshorten = 0x02,
        [EnumMember]
        Filter = 0x04
    }

    [DataContract]
    public enum TextureAddressMode
    {
        [EnumMember]
        Wrap,
        [EnumMember]
        Mirror,
        [EnumMember]
        Clamp
    }

    [DataContract]
    public enum MaterialMode
    {
        [EnumMember]
        Null,
        [EnumMember]
        Double
    }

    [DataContract]
    public class Material : IEquatable<Material>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        public Material()
        {
            Opacity = 1;
            TextureMode = TextureMode.Lit;
            Color = new Color();
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [DataMember]
        public Color Color { get; private set; }

        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>The opacity.</value>
        [DataMember]
        public float Opacity { get; private set; }

        /// <summary>
        /// Gets or sets the ambient.
        /// </summary>
        /// <value>The ambient.</value>
        [DataMember]
        public float Ambient { get; private set; }

        /// <summary>
        /// Gets or sets the diffuse.
        /// </summary>
        /// <value>The diffuse.</value>
        [DataMember]
        public float Diffuse { get; private set; }

        /// <summary>
        /// Gets the specular.
        /// </summary>
        [DataMember]
        public float Specular { get; private set; }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>The texture.</value>
        [DataMember]
        public string Texture { get; private set; }

        /// <summary>
        /// Gets or sets the mask.
        /// </summary>
        /// <value>The mask.</value>
        [DataMember]
        public string Mask { get; private set; }

        /// <summary>
        /// Gets or sets the bump.
        /// </summary>
        /// <value>
        /// The bump.
        /// </value>
        [DataMember]
        public string Bump { get; private set; }

        /// <summary>
        /// Gets or sets the texture mode.
        /// </summary>
        /// <value>The texture mode.</value>
        [DataMember]
        public TextureMode TextureMode { get; private set; }

        /// <summary>
        /// Gets or sets the texture address mode.
        /// </summary>
        /// <value>The texture address mode.</value>
        [DataMember]
        public TextureAddressMode TextureAddressMode { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [texture mipmap state].
        /// </summary>
        /// <value><c>true</c> if [texture mipmap state]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool TextureMipmapState { get; private set; }

        /// <summary>
        /// Gets or sets the light sampling.
        /// </summary>
        /// <value>The light sampling.</value>
        [DataMember]
        public LightSampling LightSampling { get; private set; }

        /// <summary>
        /// Gets or sets the geometry sampling.
        /// </summary>
        /// <value>The geometry sampling.</value>
        [DataMember]
        public GeometrySampling GeometrySampling { get; private set; }

        /// <summary>
        /// Gets or sets the material mode.
        /// </summary>
        /// <value>The material mode.</value>
        [DataMember]
        public MaterialMode MaterialMode { get; private set; }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        private Material Copy()
        {
            return new Material
            {
                Color = new Color(Color.R, Color.G, Color.B),
                Opacity = Opacity,
                Ambient = Ambient,
                Diffuse = Diffuse,
                Texture = Texture,
                Mask = Mask,
                TextureMode = TextureMode,
                TextureAddressMode = TextureAddressMode,
                TextureMipmapState = TextureMipmapState,
                GeometrySampling = GeometrySampling,
                LightSampling = LightSampling,
                MaterialMode = MaterialMode
            };
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Material other)
        {
            return this.Equals(other, () => Color == other.Color &&
                                            Opacity == other.Opacity &&
                                            Ambient == other.Ambient &&
                                            Diffuse == other.Diffuse &&
                                            Texture == other.Texture &&
                                            Mask == other.Mask &&
                                            TextureMode == other.TextureMode &&
                                            TextureAddressMode == other.TextureAddressMode &&
                                            TextureMipmapState == other.TextureMipmapState &&
                                            GeometrySampling == other.GeometrySampling &&
                                            LightSampling == other.LightSampling &&
                                            MaterialMode == other.MaterialMode);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Material);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Ambient.GetHashCode() * Diffuse.GetHashCode() + (Texture ?? string.Empty).GetHashCode();
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Material lhs, Material rhs)
        {
            return lhs.IsEqualTo(rhs);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Material lhs, Material rhs)
        {
            return !lhs.IsEqualTo(rhs);
        }

        /// <summary>
        /// Updates the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="mask">The mask.</param>
        /// <param name="bump">The bump.</param>
        /// <returns></returns>
        public Material UpdateTexture(string texture, string mask, string bump)
        {
            var copy = Copy();
            copy.Texture = texture;
            copy.Mask = mask;
            copy.Bump = bump;
            return copy;
        }

        /// <summary>
        /// Updates the color.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public Material UpdateColor(float r, float g, float b)
        {
            var copy = Copy();
            copy.Color = new Color(r, g, b);
            return copy;
        }

        /// <summary>
        /// Updates the surface.
        /// </summary>
        /// <param name="ambient">The ambient.</param>
        /// <param name="diffuse">The diffuse.</param>
        /// <param name="specular">The specular.</param>
        /// <returns></returns>
        public Material UpdateSurface(float ambient, float diffuse, float specular)
        {
            var copy = Copy();
            copy.Ambient = ambient;
            copy.Diffuse = diffuse;
            copy.Specular = specular;
            return copy;
        }

        /// <summary>
        /// Updates the ambient.
        /// </summary>
        /// <param name="ambient">The ambient.</param>
        /// <returns></returns>
        public Material UpdateAmbient(float ambient)
        {
            var copy = Copy();
            copy.Ambient = ambient;
            return copy;
        }

        /// <summary>
        /// Updates the diffuse.
        /// </summary>
        /// <param name="diffuse">The diffuse.</param>
        /// <returns></returns>
        public Material UpdateDiffuse(float diffuse)
        {
            var copy = Copy();
            copy.Diffuse = diffuse;
            return copy;
        }

        /// <summary>
        /// Updates the specular.
        /// </summary>
        /// <param name="specular">The specular.</param>
        /// <returns></returns>
        public Material UpdateSpecular(float specular)
        {
            var copy = Copy();
            copy.Specular = specular;
            return copy;
        }

        /// <summary>
        /// Updates the opacity.
        /// </summary>
        /// <param name="opacity">The opacity.</param>
        /// <returns></returns>
        public Material UpdateOpacity(float opacity)
        {
            var copy = Copy();
            copy.Opacity = opacity;
            return copy;
        }

        /// <summary>
        /// Updates the texture mode.
        /// </summary>
        /// <param name="textureMode">The texture mode.</param>
        /// <returns></returns>
        public Material UpdateTextureMode(TextureMode textureMode)
        {
            var copy = Copy();
            copy.TextureMode = textureMode;
            return copy;
        }

        /// <summary>
        /// Adds the texture mode.
        /// </summary>
        /// <param name="textureMode">The texture mode.</param>
        /// <returns></returns>
        public Material AddTextureMode(TextureMode textureMode)
        {
            var copy = Copy();
            copy.TextureMode |= textureMode;
            return copy;
        }

        /// <summary>
        /// Removes the texture mode.
        /// </summary>
        /// <param name="textureMode">The texture mode.</param>
        /// <returns></returns>
        public Material RemoveTextureMode(TextureMode textureMode)
        {
            var copy = Copy();
            copy.TextureMode &= ~textureMode;
            return copy;
        }


        /// <summary>
        /// Updates the material mode.
        /// </summary>
        /// <param name="materialMode">The material mode.</param>
        /// <returns></returns>
        public Material UpdateMaterialMode(MaterialMode materialMode)
        {
            var copy = Copy();
            copy.MaterialMode = materialMode;
            return copy;
        }

        /// <summary>
        /// Adds the material mode.
        /// </summary>
        /// <param name="materialMode">The material mode.</param>
        /// <returns></returns>
        public Material AddMaterialMode(MaterialMode materialMode)
        {
            var copy = Copy();
            copy.MaterialMode |= materialMode;
            return copy;
        }

        /// <summary>
        /// Removes the material mode.
        /// </summary>
        /// <param name="materialMode">The material mode.</param>
        /// <returns></returns>
        public Material RemoveMaterialMode(MaterialMode materialMode)
        {
            var copy = Copy();
            copy.MaterialMode &= ~materialMode;
            return copy;
        }

        /// <summary>
        /// Updates the geometry sampling.
        /// </summary>
        /// <param name="sampling">The sampling.</param>
        /// <returns></returns>
        public Material UpdateGeometrySampling(GeometrySampling sampling)
        {
            var copy = Copy();
            copy.GeometrySampling = sampling;
            return copy;
        }

        /// <summary>
        /// Updates the light sampling.
        /// </summary>
        /// <param name="sampling">The sampling.</param>
        /// <returns></returns>
        public Material UpdateLightSampling(LightSampling sampling)
        {
            var copy = Copy();
            copy.LightSampling = sampling;
            return copy;
        }

        /// <summary>
        /// Updates the texture address mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public Material UpdateTextureAddressMode(TextureAddressMode mode)
        {
            var copy = Copy();
            copy.TextureAddressMode = mode;
            return copy;
        }

        /// <summary>
        /// Updates the state of the texture mipmap.
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <returns></returns>
        public Material UpdateTextureMipmapState(bool state)
        {
            var copy = Copy();
            copy.TextureMipmapState = state;
            return copy;
        }
    }
}
