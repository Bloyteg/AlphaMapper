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

using MrByte.RWX.Model.Components;
using MrByte.Math;
using SharpDX;
using Color = MrByte.RWX.Model.Components.Color;
using Vector2 = SharpDX.Vector2;
using Vector3 = SharpDX.Vector3;

namespace AlphaMapper.Renderer.InternalComponents
{
    internal static class HelperExtensions
    {
        public static Vector3 ToDXVector3(this MrByte.Math.Vector3 vector)
        {
            if (vector == null)
            {
                return Vector3.Zero;
            }

            return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }

        public static Vector2 ToDXVector2(this MrByte.Math.Vector2 vector)
        {
            if (vector == null)
            {
                return Vector2.Zero;
            }

            return new Vector2((float)vector.X, (float)vector.Y);
        }

        public static Vector2 ToDXVector2(this UV vector)
        {
            if (vector == null)
            {
                return Vector2.Zero;
            }

            return new Vector2(vector.U, vector.V);
        }

        public static Matrix ToDXMatrix(this Matrix4 matrix)
        {
            if(matrix == null)
            {
                return Matrix.Identity;
            }

            return new Matrix
                       {
                           M11 = (float) matrix[0, 0],
                           M12 = (float) matrix[0, 1],
                           M13 = (float) matrix[0, 2],
                           M14 = (float) matrix[0, 3],
                           M21 = (float) matrix[1, 0],
                           M22 = (float) matrix[1, 1],
                           M23 = (float) matrix[1, 2],
                           M24 = (float) matrix[1, 3],
                           M31 = (float) matrix[2, 0],
                           M32 = (float) matrix[2, 1],
                           M33 = (float) matrix[2, 2],
                           M34 = (float) matrix[2, 3],
                           M41 = (float) matrix[3, 0],
                           M42 = (float) matrix[3, 1],
                           M43 = (float) matrix[3, 2],
                           M44 = (float) matrix[3, 3],
                       };
        }

        public static Color4 ToDXColor4(this Color color)
        {
            return new Color4(color.ToDXColor3(), 1);
        }

        public static Color3 ToDXColor3(this Color color)
        {
            return new Color3(color.R, color.G, color.B);
        }
    }
}
