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

namespace AlphaMapper.Property.Model
{
    public class ObjectBase
    {
        public virtual int Id { get; set; }
        public DateTime BuildDate { get; set; }
        public int PositionY { get; set; }
        public int CellX { get; set; }
        public int CellZ { get; set; }
        public int OffsetX { get; set; }
        public int OffsetZ { get; set; }
        public int Yaw { get; set; }
        public int Tilt { get; set; }
        public int Roll { get; set; }
        public int Owner { get; set; }
    }
}
