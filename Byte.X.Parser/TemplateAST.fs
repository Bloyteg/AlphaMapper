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

namespace Byte.X.Parser
module TemplateAST =
    type Type =
        | Word
        | DWord
        | Float
        | Double
        | Byte
        | String
    and Declaration =
        | Primitive of Type * string
        | Array of Declaration
        | TemplateRef of string * string
    and Restriction =
        | Open
        | Restricted of (string * System.Guid option) list
    and Template = string * (System.Guid option) * Declaration list * Restriction option

