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

open Byte.X.Parser.HelperParser
open Byte.X.Parser.TemplateAST
open Byte.X.Parser.DataAST
open FParsec

type TemplateEnvironment = { Parser : Parser<DataValue list, unit>; Restriction : Restriction option }

module TemplatedParserBuilder =
    let dataRef = (between (pchar '{') (pchar '}') nameUuidPair) |>> fun (name, uuid) -> ValueReference { Name = name; UUID = uuid }

    let buildDeclarationParser (environment : Ref<Map<string, TemplateEnvironment>>) declaration =
        match declaration with
        | Primitive(dataType, name) ->
            match dataType with
            | Byte -> pint8 .>> semicolon |>> fun value -> Int8(name, value)
            | Word -> pint16 .>> semicolon |>> fun value -> Int16(name, value)
            | DWord -> pint32 .>> semicolon |>> fun value -> Int32(name, value)
            | Float -> pfloat .>> semicolon |>> fun value -> Float32(name, single value)
            | Double -> pfloat .>> semicolon |>> fun value -> Float64(name, value)
            | String -> quotedString |>> fun value -> StringValue(name, value)
        | Array(arrayDeclaration) ->
            match arrayDeclaration with
            | Primitive(dataType, name) ->
                match dataType with
                | Byte -> (sepBy pint8 (pchar ',' .>> ws)) .>> semicolon
                                |>> fun value -> Int8List(name, value) 
                | Word -> (sepBy pint16 (pchar ',' .>> ws)) .>> semicolon
                                |>> fun value -> Int16List(name, value) 
                | DWord -> (sepBy pint32 (pchar ',' .>> ws)) .>> semicolon
                                |>> fun value -> Int32List(name, value) 
                | Float -> (sepBy pfloat (pchar ',' .>> ws)) .>> semicolon
                                |>> fun value -> Float32List(name, List.map (fun f -> single f) value) 
                | Double -> (sepBy pfloat (pchar ',' .>> ws)) .>> semicolon
                                |>> fun value -> Float64List(name, value)
                | String -> (sepBy quotedString (pchar ',' .>> ws)) .>> semicolon
                                |>> fun value -> StringList(name, value)
            | TemplateRef(template, name) -> let templateParser = (Map.find template !environment)
                                             (((sepBy ((dataRef |>> (fun value -> [value])) <|> templateParser.Parser) (pchar ',' .>> ws)) .>> semicolon)
                                                |>> fun value -> InnerValues(name, InnerDataArrayList(value)))
            | _ -> failwith "This array type is unsupported."
        | TemplateRef(template, name) -> let templateParser = (Map.find template !environment)
                                         dataRef <|> 
                                         (templateParser.Parser .>> semicolon |>> fun value -> InnerValues(name, InnerDataValueList(value)))

    let builderTemplateParser (environment : Ref<Map<string, TemplateEnvironment>>) template =
        let name, uuid, declarations, restrictions = template
        environment := Map.add name { Parser = (applyAll (List.map (buildDeclarationParser environment) declarations)); Restriction = restrictions } !environment

    (* Template converters *)
    let buildTemplateParsers (templates : Template list) =
        let environment : Ref<Map<string, TemplateEnvironment>> = ref Map.empty
        List.iter (builderTemplateParser environment) templates
        !environment
