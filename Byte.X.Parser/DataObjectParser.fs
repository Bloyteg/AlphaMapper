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

open FParsec
open Byte.X.Parser
open Byte.X.Parser.HelperParser
open Byte.X.Parser.DataAST
open Byte.X.Parser.TemplateAST
open Byte.X.Parser.TemplateParser
open Byte.X.Parser.TemplatedParserBuilder

module DataObjectParser =

    let rec processDataObject (environment, restrictedEnvironment) (template, name) =
        let templateEnv = 
            match restrictedEnvironment with
            | None -> (Map.tryFind template environment)
            | Some(environment) -> (Map.tryFind template environment)

        let childParserBuilder environment : Parser<NestedDataObject, unit> =
            ((between openBrace closeBrace nameUuidPair) |>> fun (name, uuid) -> NestedReference ({ Name = name; UUID = uuid })) <|>
            ((dataObject environment) |>> NestedData) .>> ws

        match templateEnv with
        | None -> fail "Expecting a valid template name."
        | Some(templateEnv) ->
            let childParser =
                match templateEnv.Restriction with
                | None -> preturn None
                | Some(restriction) ->
                    match restriction with
                    | Open -> many (childParserBuilder (environment, None)) |>> Some
                    | Restricted(restriction) -> many (childParserBuilder (environment, Some (Map.filter (fun key value -> List.exists (fun (name, uuid) -> name = key) restriction) environment))) |>> Some

            templateEnv.Parser .>>. childParser .>> ws
                |>> fun (data, childData) ->
                    {
                        Type = template;
                        Identifier = { Name = name; UUID = None };
                        Data = data;
                        Children = childData;
                    }

    and dataObject (evironment, restrictedEnvironment) = ws >>. (identifier .>>. (ws >>. opt identifier) .>> openBrace) >>= (processDataObject (evironment, restrictedEnvironment)) .>> closeBrace

    let parser = (many (attempt template) |>> buildTemplateParsers) >>= fun environment -> many (dataObject (environment, None))
