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

module DataAST =
    type DataValue =
        | Int8 of string * int8
        | Int16 of string * int16
        | Int32 of string * int
        | Float32 of string * single
        | Float64 of string * double
        | StringValue of string * string
        | Int8List of string * int8 list
        | Int16List of string * int16 list
        | Int32List of string * int32 list
        | Float32List of string * single list
        | Float64List of string * double list
        | StringList of string * string list
        | InnerValues of string * InnerDataValue
        | ValueReference of DataObjectReference 
    and InnerDataValue =
        | InnerDataValueList of DataValue list
        | InnerDataArrayList of DataValue list list
    and NestedDataObject =
        | NestedReference of DataObjectReference
        | NestedData of DataObject
    and DataObjectReference = { Name : string option; UUID : System.Guid option }
    and DataObject = { Type : string; Identifier : DataObjectReference ; Data : DataValue list; Children : NestedDataObject list option }
