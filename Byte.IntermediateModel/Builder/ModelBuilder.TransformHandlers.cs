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
using Byte.Math;

namespace Byte.IntermediateModel.Builder
{
	public partial class ModelBuilder
    {
        public void SetIdentityTransform()
        {
            _currentTransform = new Matrix4();
        }

        public void AddRotate(double x, double y, double z, double degrees)
        {
            _currentTransform = _currentTransform.Rotate(x, y, z, degrees);
        }

        public void AddScale(double x, double y, double z)
        {
            _currentTransform = _currentTransform.Scale(x, y, z);
        }

        public void AddTranslate(double x, double y, double z)
        {
            _currentTransform = _currentTransform.Translate(x, y, z);
        }

	    public void SetTransformMatrix(Matrix4 transform)
        {
            _currentTransform = transform;
        }
	}
}
