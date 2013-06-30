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

namespace Byte.Math
{
    [DataContract]
    public class Vector2 : IEquatable<Vector2>
    {
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        [DataMember]
        public double X { get; private set; }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        [DataMember]
        public double Y { get; private set; }


        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2()
        {
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public double Length
        {
            get { return System.Math.Sqrt(X*X + Y*Y); }
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2 operator /(Vector2 lhs, double rhs)
        {
            if (lhs == null)
                return null;

            return new Vector2(lhs.X / rhs, lhs.Y / rhs);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2 operator *(Vector2 lhs, double rhs)
        {
            if (lhs == null)
                return null;

            return new Vector2(lhs.X * rhs, lhs.Y * rhs);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2 operator *(double lhs, Vector2 rhs)
        {
            return (rhs * lhs);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            if (lhs == null || rhs == null)
                return null;

            return new Vector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            if (lhs == null || rhs == null)
                return null;

            return new Vector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        /// <summary>
        /// Produces a dot product of two vectors.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns></returns>
        public static double Dot(Vector2 lhs, Vector2 rhs)
        {
            if (lhs == null || rhs == null)
                return double.NaN;

            return lhs.X * rhs.X + lhs.Y * rhs.Y;
        }

        /// <summary>
        /// Produces a cross product of two vectors.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <returns></returns>
        public static Vector2 Orthogonal(Vector2 lhs)
        {
            if (lhs == null)
                return null;

            return new Vector2(lhs.Y, -lhs.X);
        }

        public static double Determinant(Vector2 lhs, Vector2 rhs)
        {
            return (lhs.X * rhs.Y - lhs.Y * rhs.X);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("<{0}, {1}>", X, Y);
        }

        public void Normalize()
        {
            double length = Length;
            X /= length;
            Y /= length;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Vector2 other)
        {
            return this.Equals(other, () => (X == other.X) && (Y == other.Y));
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
            return Equals(obj as Vector2);
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
                return X.GetHashCode() + (Y.GetHashCode() << 16);
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
        public static bool operator ==(Vector2 lhs, Vector2 rhs)
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
        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return !lhs.IsEqualTo(rhs);
        }
    }
}
