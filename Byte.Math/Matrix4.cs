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

namespace Byte.Math
{
    [DataContract]
    public class Matrix4 : ICloneable
    {
        private double[,] _transformMatrix = new double[,]
                                                {
                                                    {1, 0, 0, 0},
                                                    {0, 1, 0, 0},
                                                    {0, 0, 1, 0},
                                                    {0, 0, 0, 1}
                                                };

        [DataMember(Name="Matrix")]
        private byte[] AsBytes
        {
            get
            { 
                var outArray = new byte[_transformMatrix.Length*sizeof (double)];
                int width = _transformMatrix.GetUpperBound(0);
                int height = _transformMatrix.GetUpperBound(1);

                int currentOffset = 0;
                for(int row = 0; row < width; ++row)
                {
                    for(int column = 0; column < height; ++column)
                    {
                        BitConverter.GetBytes(_transformMatrix[row, column]).CopyTo(outArray, currentOffset);
                        currentOffset += sizeof (double);
                    }
                }

                return outArray;
            }

            set
            {
                if(_transformMatrix == null)
                {
                    _transformMatrix = new double[4,4];
                }

                int width = _transformMatrix.GetUpperBound(0);
                int height = _transformMatrix.GetUpperBound(1);

                int currentOffset = 0;
                for (int row = 0; row < width; ++row)
                {
                    for (int column = 0; column < height; ++column)
                    {
                        _transformMatrix[row, column] = BitConverter.ToDouble(value, currentOffset);
                        currentOffset += sizeof(double);
                    }
                }

                _transformMatrix[3, 0] = 0;
                _transformMatrix[3, 1] = 0;
                _transformMatrix[3, 2] = 0;
                _transformMatrix[3, 3] = 1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix4"/> class.
        /// </summary>
        /// <param name="transformMatrix">The transform matrix.</param>
        public Matrix4(double[,] transformMatrix)
        {
            _transformMatrix = transformMatrix.Clone() as double[,];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix4"/> class.
        /// </summary>
        public Matrix4() { }

        /// <summary>
        /// Gets or sets the <see cref="System.Single"/> with the specified row.
        /// </summary>
        /// <value></value>
        public double this[int row, int column]
        {
            get
            {
                return _transformMatrix[row, column];
            }

            set
            {
                if(column != 3)
                {
                    _transformMatrix[row, column] = value;
                }
            }
        }

        /// <summary>
        /// Translates the matrix.
        /// </summary>
        /// <param name="xOffset">The x offset.</param>
        /// <param name="yOffset">The y offset.</param>
        /// <param name="zOffset">The z offset.</param>
        public Matrix4 Translate(double xOffset, double yOffset, double zOffset)
        {
            var translateMatrix = new [,]
                                      {
                                          {1, 0, 0, 0},
                                          {0, 1, 0, 0},
                                          {0, 0, 1, 0},
                                          {xOffset, yOffset, zOffset, 1}
                                      };

            return new Matrix4
                       {
                           _transformMatrix = Multiply(translateMatrix, _transformMatrix)
                       };
        }

        /// <summary>
        /// Rotates the matrix.
        /// </summary>
        /// <param name="xFactor">The x factor.</param>
        /// <param name="yFactor">The y factor.</param>
        /// <param name="zFactor">The z factor.</param>
        /// <param name="degrees">The degrees.</param>
        public Matrix4 Rotate(double xFactor, double yFactor, double zFactor, double degrees)
        {
            var rads = (degrees*System.Math.PI/180);
            var s = System.Math.Sin(rads);
            var c = System.Math.Cos(rads);
            var t = 1 - c;

            var rotateMatrix = new [,]
                                   {
                                       {(t*(xFactor*xFactor) + c), t*xFactor*yFactor + s*zFactor, t*xFactor*zFactor - s*yFactor, 0},
                                       {t*xFactor*yFactor - s*zFactor, (t*(yFactor*yFactor) + c), t*yFactor*zFactor + s*xFactor, 0},
                                       {t*xFactor*yFactor + s*yFactor, t*yFactor*zFactor - s*xFactor, (t*(zFactor*zFactor) + c), 0},
                                       {0, 0, 0, 1}
                                   };

            return new Matrix4
                       {
                           _transformMatrix = Multiply(rotateMatrix, _transformMatrix)
                       };
        }

        /// <summary>
        /// Scales the matrix.
        /// </summary>
        /// <param name="xFactor">The x factor.</param>
        /// <param name="yFactor">The y factor.</param>
        /// <param name="zFactor">The z factor.</param>
        public Matrix4 Scale(double xFactor, double yFactor, double zFactor)
        {
            var scaleMatrix = new[,]
                                 {
                                     {xFactor, 0, 0, 0},
                                     {0, yFactor, 0, 0},
                                     {0, 0, zFactor, 0},
                                     {0, 0, 0, 1}
                                 };

            return new Matrix4
                       {
                            _transformMatrix = Multiply(scaleMatrix, _transformMatrix)
                       };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new Matrix4(_transformMatrix);
        }

        /// <summary>
        /// Multiplies the specified matrix1.
        /// </summary>
        /// <param name="matrix1">The matrix1.</param>
        /// <param name="matrix2">The matrix2.</param>
        /// <returns></returns>
        private static double[,] Multiply(double[,] matrix1, double[,] matrix2)
        {
            //Make sure the matrices are the appropriate size.
            if (matrix1.GetLength(1) == matrix2.GetLength(0))
            {
                var result = new double[matrix1.GetLength(0), matrix2.GetLength(1)];
                for (int row = 0; row < result.GetLength(0); row++)
                {
                    for (int column = 0; column < result.GetLength(1); column++)
                    {
                        result[row, column] = 0;
                        for (int k = 0; k < matrix1.GetLength(1); k++)
                        {
                            result[row, column] = result[row, column] + matrix1[row, k]*matrix2[k, column];
                        }
                    }
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix4 operator *(Matrix4 lhs, Matrix4 rhs)
        {
            return new Matrix4
                       {
                           _transformMatrix = Multiply(lhs._transformMatrix, rhs._transformMatrix)
                       };
        }
        
    }
}
