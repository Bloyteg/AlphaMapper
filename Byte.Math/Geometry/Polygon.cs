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

namespace Byte.Math.Geometry
{
    public static class Polygon
    {
        /// <summary>
        /// Calculates the real area.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <returns></returns>
        private static double CalculateRealArea(IEnumerable<Vector2> vertices)
        {
            double area = 0;
            int count = vertices.Count();

            for(int index = 0; index < count; ++index)
            {
                int nextIndex = (index + 1) % count;
                area += vertices.ElementAt(index).X * vertices.ElementAt(nextIndex).Y;
                area -= vertices.ElementAt(index).Y * vertices.ElementAt(nextIndex).X;
            }

            return area/2;
        }

        public static bool IsClockwise(IEnumerable<Vector2> vertices)
        {
            return CalculateRealArea(vertices) < 0;
        }

        /// <summary>
        /// Calculates the area.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <returns></returns>
        public static double CalculateArea(IEnumerable<Vector2> vertices)
        {
            return System.Math.Abs(CalculateRealArea(vertices));
        }

        /// <summary>
        /// Calculates the area.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <returns></returns>
        public static double CalculateArea(IEnumerable<Vector3> vertices)
        {
            return CalculateArea(ProjectPointsTo2D(vertices));
        }

        /// <summary>
        /// Projects the points to 2D with the longest axis as the X component and
        /// the second longest axis as the Y component, discarding the shortest axis.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static IEnumerable<Vector2> ProjectPointsTo2D(IEnumerable<Vector3> points)
        {
            var boundingBox = new BoundingBox3();
            boundingBox.ComputeFromVertices(points);

            var delta = boundingBox.Maximum - boundingBox.Minimum;

            var axes = from Vector3.Axis axis in Enum.GetValues(typeof (Vector3.Axis))
                       let value = Vector3.GetComponent(axis, delta)
                       orderby value descending 
                       select axis;

            //Find the major and minor axes to project into 2D.
            Vector3.Axis majorAxis = axes.First();
            Vector3.Axis minorAxis = axes.Skip(1).First();

            return points.Select(vertex => new Vector2(Vector3.GetComponent(majorAxis, vertex),
                                                       Vector3.GetComponent(minorAxis, vertex)))
                .ToList();
        }
    }
}
