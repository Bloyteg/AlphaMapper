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
using AlphaMapper.Renderer.Components;
using SlimDX;

namespace AlphaMapper.Renderer.InternalComponents
{
    static class RenderTransformableExtensions
    {
        /// <summary>
        /// Gets the transform matrix.
        /// </summary>
        /// <param name="renderTransformable">The render transformable.</param>
        /// <returns></returns>
        internal static Matrix GetTransformMatrix(this IRenderTransformable renderTransformable)
        {
            Matrix scaleMatrix = Matrix.Scaling((float) renderTransformable.WorldScale.X,
                                                (float) renderTransformable.WorldScale.Y,
                                                (float) renderTransformable.WorldScale.Z);

            var rotationMatrix = Matrix.RotationX((float) (renderTransformable.WorldRotation.X*(Math.PI/180)))
                                 *Matrix.RotationZ((float) (renderTransformable.WorldRotation.Z*(Math.PI/180)))
                                 *Matrix.RotationY((float) (renderTransformable.WorldRotation.Y*(Math.PI/180)));

            //Matrix rotationMatrix = Matrix.RotationYawPitchRoll((float)(renderTransformable.WorldRotation.Y * (Math.PI / 180)),
            //                                                    (float)(renderTransformable.WorldRotation.X * (Math.PI / 180)),
            //                                                    (float)(renderTransformable.WorldRotation.Z * (Math.PI / 180)));

            Matrix positionMatrix = Matrix.Translation((float) renderTransformable.WorldPosition.X,
                                                       (float) renderTransformable.WorldPosition.Y,
                                                       (float) renderTransformable.WorldPosition.Z);

            Matrix skewMatrix = Matrix.Identity;

            switch (renderTransformable.WorldSkewType)
            {
                case SkewType.Shear:
                    {
                        const int skewLimit = 5;

                        skewMatrix.M11 = 1;
                        skewMatrix.M12 = Clamp(renderTransformable.WorldSkew.First.Z, -skewLimit, skewLimit);
                        skewMatrix.M13 = -Clamp(renderTransformable.WorldSkew.Second.Y, -skewLimit, skewLimit);
                        skewMatrix.M21 = -Clamp(renderTransformable.WorldSkew.Second.Z, -skewLimit, skewLimit);
                        skewMatrix.M22 = 1;
                        skewMatrix.M23 = Clamp(renderTransformable.WorldSkew.First.X, -skewLimit, skewLimit);
                        skewMatrix.M31 = Clamp(renderTransformable.WorldSkew.First.Y, -skewLimit, skewLimit);
                        skewMatrix.M32 = -Clamp(renderTransformable.WorldSkew.Second.X, -skewLimit, skewLimit);
                        skewMatrix.M33 = 1;
                        skewMatrix.M44 = 1;
                        break;
                    }
                case SkewType.Skew:
                    {
                        const double skewLimit = 89.0;

                        double x1 = Clamp(renderTransformable.WorldSkew.First.X, -skewLimit, skewLimit) * Math.PI/180;
                        double x2 = Clamp(renderTransformable.WorldSkew.Second.X, -skewLimit, skewLimit) * Math.PI / 180;
                        double y1 = Clamp(renderTransformable.WorldSkew.First.Y, -skewLimit, skewLimit) * Math.PI / 180;
                        double y2 = Clamp(renderTransformable.WorldSkew.Second.Y, -skewLimit, skewLimit) * Math.PI / 180;
                        double z1 = Clamp(renderTransformable.WorldSkew.First.Z, -skewLimit, skewLimit) * Math.PI / 180;
                        double z2 = Clamp(renderTransformable.WorldSkew.Second.Z, -skewLimit, skewLimit) * Math.PI / 180;

                        skewMatrix.M11 = (float) (Math.Cos(z1)*Math.Cos(y1));
                        skewMatrix.M12 = (float) Math.Sin(z1);
                        skewMatrix.M13 = (float) -Math.Sin(y2);

                        skewMatrix.M21 = (float) -Math.Sin(z2);
                        skewMatrix.M22 = (float) (Math.Cos(x1)*Math.Cos(z2));
                        skewMatrix.M23 = (float) Math.Sin(x1);

                        skewMatrix.M31 = (float) Math.Sin(y1);
                        skewMatrix.M32 = (float) -Math.Sin(x2);
                        skewMatrix.M33 = (float) (Math.Cos(y1)*Math.Cos(x2));
                        skewMatrix.M44 = 1;

                        break;
                    }
            }

            return skewMatrix*scaleMatrix*rotationMatrix*positionMatrix;
        }

        private static float Clamp(double input, double lower, double upper)
        {
            return (float)(input < lower ? lower : (input > upper ? upper : input));
        }
    }
}
