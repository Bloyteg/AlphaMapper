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
    public class Vector3 : IEquatable<Vector3>
    {
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3()
        {
        }

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

        /// <summary>
        /// Gets or sets the Z.
        /// </summary>
        /// <value>The Z.</value>
        [DataMember]
        public double Z { get; private set; }


        public double Length
        {
            get { return System.Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator *(Vector3 lhs, Matrix4 rhs)
        {
            if (rhs == null || lhs == null)
            {
                return lhs;
            }

            var vector = new Vector3(System.Math.Round((lhs.X * rhs[0, 0]) + (lhs.Y * rhs[1, 0]) + (lhs.Z * rhs[2, 0]) + (1 * rhs[3, 0]), 10), System.Math.Round((lhs.X * rhs[0, 1]) + (lhs.Y * rhs[1, 1]) + (lhs.Z * rhs[2, 1]) + (1 * rhs[3, 1]), 10), System.Math.Round((lhs.X * rhs[0, 2]) + (lhs.Y * rhs[1, 2]) + (lhs.Z * rhs[2, 2]) + (1 * rhs[3, 2]), 10));

            return vector;
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator /(Vector3 lhs, double rhs)
        {
            if (lhs == null)
                return null;

            return new Vector3(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator *(Vector3 lhs, double rhs)
        {
            if (lhs == null)
                return null;

            return new Vector3(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator *(double lhs, Vector3 rhs)
        {
            return (rhs * lhs);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
        {
            if (lhs == null || rhs == null)
                return null;

            return new Vector3(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
        {
            if (lhs == null || rhs == null)
                return null;

            return new Vector3(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        /// <summary>
        /// Produces a dot product of two vectors.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns></returns>
        public static double Dot(Vector3 lhs, Vector3 rhs)
        {
            if (lhs == null || rhs == null)
                return double.NaN;

            return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
        }

        /// <summary>
        /// Produces a cross product of two vectors.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns></returns>
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            if (lhs == null || rhs == null)
                return null;

            return new Vector3((lhs.Y * rhs.Z) - (lhs.Z * rhs.Y), (lhs.Z * rhs.X) - (lhs.X * rhs.Z), (lhs.X * rhs.Y) - (lhs.Y * rhs.X));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("<{0}, {1}, {2}>", X, Y, Z);
        }

        /// <summary>
        /// Normalizes this instance.
        /// </summary>
        public Vector3 Normalize()
        {
            double length = Length;
            return new Vector3(X/length, Y/length, Z/length);
        }

        public Vector3 UpdateX(double x)
        {
            var copy = Copy();
            copy.X = x;
            return copy;
        }

        public Vector3 UpdateY(double y)
        {
            var copy = Copy();
            copy.Y = y;
            return copy;
        }

        public Vector3 UpdateZ(double z)
        {
            var copy = Copy();
            copy.Z = z;
            return copy; 
        }

        private Vector3 Copy()
        {
            return new Vector3(X, Y, Z);
        }

        internal enum Axis
        {
            X,
            Y,
            Z
        }

        /// <summary>
        /// Gets the component.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal static double GetComponent(Axis axis, Vector3 point)
        {
            switch(axis)
            {
                case Axis.X:
                    return point.X;

                case Axis.Y:
                    return point.Y;

                case Axis.Z:
                    return point.Z;

                default:
                    return double.NaN;
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Vector3 other)
        {
            return this.Equals(other, () => (X == other.X) && (Y == other.Y) && (Z == other.Z));
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
            return Equals(obj as Vector3);
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
                return X.GetHashCode() + (Y.GetHashCode() << 8) + (Z.GetHashCode() << 16);
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
        public static bool operator ==(Vector3 lhs, Vector3 rhs)
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
        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !lhs.IsEqualTo(rhs);
        }
    }
}
