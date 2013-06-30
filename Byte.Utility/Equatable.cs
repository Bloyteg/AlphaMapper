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

namespace Byte.Utility
{
    public static class Equatable
    {
       public static bool Equals<T>(this T self, T other, Func<bool> equalityCheck)
           where T : IEquatable<T>
       {
           if (ReferenceEquals(other, null))
           {
               return false;
           }

           if (ReferenceEquals(other, self))
           {
               return true;
           }

           return equalityCheck();
       }

       public static bool IsEqualTo<T>(this T self, T other)
           where T : IEquatable<T>
       {
           return ReferenceEquals(self, null) ? ReferenceEquals(other, null) : self.Equals(other);
       }
    }
}
