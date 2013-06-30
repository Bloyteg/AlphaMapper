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
open FParsec
open Byte.ActionCommand.Parser.ActionCommandAST

module internal ASTNodeBuilders = 

    (* AST Node builders *)
    let buildWithX ((x, y, z), parameters) =
        match y with
        | Some Y -> match z with
                    | Some Z -> (x, Y, Z, parameters)
                    | None -> (x, Y, 0.0, parameters)
        | None -> (x, 0.0, 0.0, parameters)

    let buildScale ((x, y, z), parameters) =
        match y with
        | Some Y -> match z with
                    | Some Z -> (x, Y, Z, parameters)
                    | None -> (x, Y, 1.0, parameters)
        | None -> (x, x, x, parameters)


    let buildWithY ((x, y, z), parameters) =
        match y with
        | Some Y -> match z with
                    | Some Z -> (x, Y, Z, parameters)
                    | None -> (x, Y, 0.0, parameters)
        | None -> (0.0, x, 0.0, parameters)

    let buildNumber6 ((x1, y1, z1, x2, y2, z2), parameters) =
        match y1 with
        | Some Y1 -> match z1 with
                     | Some Z1 -> match x2 with
                                  | Some X2 -> match y2 with
                                               | Some Y2 -> match z2 with
                                                            | Some Z2 -> (x1, Y1, Z1, X2, Y2, Z2, parameters)
                                                            | None -> (x1, Y1, Z1, X2, Y2, 0.0, parameters)
                                               | None -> (x1, Y1, Z1, X2, 0.0, 0.0, parameters)
                                  | None -> (x1, Y1, Z1, 0.0, 0.0, 0.0, parameters)
                     | None -> (x1, Y1, 0.0, 0.0, 0.0, 0.0, parameters)
        | None -> (x1, 0.0, 0.0, 0.0, 0.0, 0.0, parameters)
