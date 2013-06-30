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
    public class Triangulator
    {
        private readonly IList<Tuple<Vector3, Vector2>> _vertexPairs;
        private readonly bool _isClockwise;

        private readonly List<int> _reflexVertices = new List<int>();
        private readonly List<int> _convexVertices = new List<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangulator"/> class.
        /// </summary>
        /// <param name="points">The points in order in which they appear in the polygon.</param>
        public Triangulator(IEnumerable<Vector3> points)
        {
            var pointsList = points.ToList();//.Distinct();
            var points2D = Polygon.ProjectPointsTo2D(pointsList).ToList();
            _isClockwise = Polygon.IsClockwise(points2D);
            var pairs = pointsList.Zip(points2D, Tuple.Create);
            _vertexPairs = (_isClockwise ? pairs.Reverse() : pairs).ToList();
        }

        private void SetupVertices()
        {
            _reflexVertices.Clear();
            _convexVertices.Clear();

            foreach(var index in _vertexPairs.Select((pair, index) => index))
            {
                if(IsConvex(index))
                {
                    _convexVertices.Add(index);
                }
                else
                {
                    _reflexVertices.Add(index);
                }
            }
        }

        /// <summary>
        /// Gets the triangles.
        /// </summary>
        /// <value>The triangles.</value>
        public IEnumerable<Tuple<Vector3, Vector3, Vector3>> Triangles
        {
            get
            {
                SetupVertices();

                int previousCount = 0;

                //Triangulate any faces if there's more than 4 vertices.
                while(_vertexPairs.Count > 4)
                {
                    SetupVertices();

                    //This is in the event that the polygon has caused an infinite loop.
                    //If so, throw an exception.
                    if(_vertexPairs.Count == previousCount)
                    {
                        throw new InvalidOperationException("The given polygon cannot be triangulated.");
                    }

                    previousCount = _vertexPairs.Count;

                    foreach (var index in _convexVertices.Where(IsEar))
                    {
                        if (!_isClockwise)
                        {
                            yield return Tuple.Create(_vertexPairs[PreviousIndex(index)].Item1,
                                                      _vertexPairs[index].Item1,
                                                      _vertexPairs[NextIndex(index)].Item1);
                        }
                        else
                        {
                            yield return Tuple.Create(_vertexPairs[NextIndex(index)].Item1,
                                                      _vertexPairs[index].Item1,
                                                      _vertexPairs[PreviousIndex(index)].Item1);
                        }

                        _vertexPairs.RemoveAt(index);
                        break;
                    }
                }

                //Split the four remaining vertices into two faces.
                if(_vertexPairs.Count == 4)
                {
                    if(!_isClockwise)
                    {
                        yield return Tuple.Create(_vertexPairs[0].Item1, _vertexPairs[1].Item1, _vertexPairs[2].Item1);
                        yield return Tuple.Create(_vertexPairs[0].Item1, _vertexPairs[2].Item1, _vertexPairs[3].Item1);
                    }
                    else
                    {
                        yield return Tuple.Create(_vertexPairs[2].Item1, _vertexPairs[1].Item1, _vertexPairs[0].Item1);
                        yield return Tuple.Create(_vertexPairs[3].Item1, _vertexPairs[2].Item1, _vertexPairs[0].Item1); 
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified index is convex.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// 	<c>true</c> if the specified index is convex; otherwise, <c>false</c>.
        /// </returns>
        private bool IsConvex(int index)
        {
            var testPoint = _vertexPairs[index].Item2;

            if (IsLine(index))
            {
                return false;
            }

            int result = ToLine(testPoint, PreviousIndex(index), NextIndex(index));
            return result >= 0;
        }

        private bool IsLine(int index)
        {
            var u = _vertexPairs[index].Item1 - _vertexPairs[PreviousIndex(index)].Item1;
            var v = _vertexPairs[index].Item1 - _vertexPairs[NextIndex(index)].Item1;
            var test = Vector3.Cross(u, v).Normalize();

            return double.IsNaN(test.X);
        }

        /// <summary>
        /// Determines whether the specified index is ear.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// 	<c>true</c> if the specified index is ear; otherwise, <c>false</c>.
        /// </returns>
        private bool IsEar(int index)
        {
            //Check if there's any reflex vertices left, if not then there's an ear.
            if(_reflexVertices.Count == 0)
            {
                return true;
            }

            //Search the reflex vertices and test if any are in the triangle.
            int previousIndex = PreviousIndex(index);
            int nextIndex = NextIndex(index);

            return _reflexVertices
                        .Where(testIndex => testIndex != previousIndex && testIndex != index && testIndex != nextIndex)
                        .All(testIndex => ToTriangle(_vertexPairs[testIndex].Item2, previousIndex, index, nextIndex) >= 0);
        }

        #region helper methods
        /// <summary>
        /// Retrieves the index prior to the index passed in.  Wraps around to Count - 1 if the previous index is out of range.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int PreviousIndex(int index)
        {
            var previousIndex = index - 1;
            return (previousIndex >= 0 ? previousIndex : _vertexPairs.Count - 1);
        }

        /// <summary>
        /// Retrieves the index following the index passed in.  Wraps around to 0 if the next index is out of range.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int NextIndex(int index)
        {
            return (index + 1)%_vertexPairs.Count;
        }

        /// <summary>
        /// Determines where the test point falls in relation to the points at the specified indices.
        ///   1 is on the right of the line
        ///  -1 is on the left of the line
        ///   0 is on the line
        /// </summary>
        /// <param name="test"></param>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        private int ToLine(Vector2 test, int v0, int v1)
        {
            bool positive = Sort(ref v0, ref v1);

            var det = Vector2.Determinant((test - _vertexPairs[v0].Item2) * 10, (_vertexPairs[v1].Item2 - test) * 10);

            if(!positive)
            {
                det = -det;
            }

            const double eps = 0.00025;
            var detEpsLower = det - eps;
            var detEpsUpper = det + eps;

            return ((detEpsUpper > 0 || detEpsLower > 0) ? 1 : ((detEpsUpper < 0 || detEpsLower < 0) ? -1 : 0));
        }


        /// <summary>
        /// Tests whether or not if the given test point is inside the triangle given by the vertices.
        /// </summary>
        /// <returns>
        /// 1 if outside the triangle.
        /// 0 if on the edge of the triangle.
        /// -1 if inside the triangle.
        /// </returns>
        private int ToTriangle(Vector2 test, int v0, int v1, int v2)
        {
            int sign0 = ToLine(test, v1, v2);
            if(sign0 > 0)
            {
                return 1;
            }

            int sign1 = ToLine(test, v0, v2);
            if(sign1 < 0)
            {
                return 1;
            }

            int sign2 = ToLine(test, v0, v1);
            if(sign2 > 0)
            {
                return 1;
            }

            return ((sign0 != 0 && sign1 != 0 && sign2 != 0) ? -1 : 0);
        }

        /// <summary>
        /// Sorts the given indices, returns true if they have a positive orientation.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        private static bool Sort(ref int v0, ref int v1)
        {
            if (v0 < v1)
            {
                return true;
            }

            int temp = v0;
            v0 = v1;
            v1 = temp;
            return false;
        }
        #endregion
    }
}
