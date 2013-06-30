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

namespace Byte.IntermediateModel.Builder
{
    public partial class ModelBuilder
    {
        private void PrevalidateModel()
        {
            if(_model == null)
            {
                throw new InvalidOperationException("No model exists.");
            }
        }

        public void SetAxisAlignment(AxisAlignment mode)
        {
            PrevalidateModel();

            _model.AxisAlignment = mode;
        }

        public void SetOpacityFix(bool state)
        {
            PrevalidateModel();

            _model.HasOpacityFix = state;
        }


        public void SetRandomUVs(bool state)
        {
            PrevalidateModel();

            _model.HasRandomUVs = state;
        }

        public void SetSeamless(bool state)
        {
            PrevalidateModel();

            _model.IsSeamless = state;
        }

        public void SetClumpTag(int tag)
        {
            if (_currentClump == null)
            {
                throw new InvalidOperationException("Tag is only valid inside the context of a clump.");
            }

            _currentClump.Tag = tag;
        }

        public void SetCollision(bool enabled)
        {
            if (_currentClump == null)
            {
                throw new InvalidOperationException("Collision is only valid inside the context of a clump.");
            }

            _currentClump.IsCollidable = enabled;
        }
    }
}
