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

namespace Byte.ActionCommand.Parser
module ActionCommandAST =
    type Timer =
        | NamedTimer of string * float * int option
        | VRTTimer of int option * int option * int option

    type Command = 
        | Move of (float * float option * float option) * (float option * float option * string option * bool option * bool option * bool option * unit option * unit option * unit option) * unit option
        | Rotate of (float * float option * float option) * (float option * float option * string option * bool option * bool option * bool option * unit option) * unit option
        | Scale of (float * float option * float option) * (float option * float option * string option * bool option * bool option * bool option * unit option) * unit option
        | Shear of (float * float option * float option * float option * float option * float option) * string option * unit option
        | Skew of (float * float option * float option * float option * float option * float option) * string option * unit option
        | Texture of string * (string option * int option * string option) * unit option
        | Color of (string option * string option * unit option) * unit option
        | Picture of (System.Uri option * int option * bool option * string option) * unit option
        | Animate of int option * bool option * (string option * string * int * int * int * int list) * unit option
        | Opacity of float * string option * unit option
        | Visible of (bool * string option * int option) * unit option
        | Astop of string option * unit option
        | Name of string
        | IgnoredCommand of string
        | InvalidCommand of string

    type Trigger = Create of Command list
                 | Bump of Command list
                 | Activate of Command list
                 | Adone of Command list
                 | Sdone of Command list
                 | Collide of Command list
                 | At of Timer * Command list
                 | EnterZone of string * Command list
                 | ExitZone of string * Command list
                 | InvalidInput of string
