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

namespace Byte.FSharp.Helpers

module Null = 
    let toNull<'T when 'T : not struct> : 'T option -> 'T =
        function
        | None -> Operators.Unchecked.defaultof<'T>
        | Some value -> value

    let toNullable<'T when 'T : (new : unit -> 'T)
                      and 'T : struct
                      and 'T :> System.ValueType> : 'T option -> System.Nullable<'T> =
        function
        | None -> new System.Nullable<'T>()
        | Some value -> new System.Nullable<'T>(value)

    let (|Null|NotNull|) input =
        if System.Object.ReferenceEquals(input, null) then
            Null
        else
            NotNull(input)

    let (|Nullable|NotNullable|) (input : System.Nullable<'T>) =
        if input.HasValue then
            NotNullable
        else
            Nullable

    let inline (@??) (input : ^a option) (defaultValue : ^a) : ^a =
        match input with
        | Some(value) -> value
        | None -> defaultValue
