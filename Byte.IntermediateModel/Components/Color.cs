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

namespace Byte.IntermediateModel.Components
{
    [DataContract]
    public struct Color : IEquatable<Color>
    {
        public Color(float r, float g, float b)
            : this()
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Gets or sets the R.
        /// </summary>
        /// <value>The R.</value>
        [DataMember]
        public float R { get; private set; }

        /// <summary>
        /// Gets or sets the G.
        /// </summary>
        /// <value>The G.</value>
        [DataMember]
        public float G { get; private set; }

        /// <summary>
        /// Gets or sets the B.
        /// </summary>
        /// <value>The B.</value>
        [DataMember]
        public float B { get; private set; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Color lhs, Color rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Color lhs, Color rhs)
        {
            return !(lhs.Equals(rhs));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Color other)
        {
            return (R == other.R) && (G == other.G) && (B == other.B);
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
            return obj is Color && Equals((Color)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ((byte) R.GetHashCode()*255) +
                   (((byte) G.GetHashCode()*255) << 8) +
                   (((byte) B.GetHashCode()*255) << 16);
        }
    }
}
