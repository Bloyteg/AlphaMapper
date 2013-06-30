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
open Byte.X.Parser.HelperParser
open Byte.X.Parser.TemplateAST

module TemplateParser =
    (* Template type declarations *)
    let wordDecl = keyword "WORD" >>. identifier |>> fun name -> Primitive (Word, name)
    let dwordDecl = keyword "DWORD" >>. identifier |>> fun name -> Primitive (DWord, name)
    let floatDecl = keyword "FLOAT" >>. identifier |>> fun name -> Primitive (Float, name)
    let doubleDecl = keyword "DOUBLE" >>. identifier |>> fun name -> Primitive (Double, name)
    let byteDecl = (keyword "BYTE" <|> keyword "UCHAR" <|> keyword "CHAR") >>. identifier |>> fun name -> Primitive (Byte, name)
    let stringDecl = keyword "STRING" >>. identifier |>> fun name -> Primitive (String, name)
    let templateDecl = attempt (ws >>. tuple2 identifier (ws >>. identifier)) |>> TemplateRef

    let declaration = attempt (choice [ wordDecl; dwordDecl; floatDecl; doubleDecl; byteDecl; stringDecl; ])

    let arrayDeclaration = keyword "array" >>. (declaration <|> templateDecl) .>> skipManyTill anyChar (pchar ']') .>> semicolon |>> Array
    let variableDeclaration = (declaration <|> templateDecl) .>> semicolon

    (* Restriction parsers *)
    let openRestriction = keyword "[...]" |>> fun _ -> Open
    let closedRestriction = between (pchar '[') (pchar ']') (sepBy (identifier .>>. opt uuid) (pchar ',' .>> ws)) |>> Restricted
    let restriction = openRestriction <|> closedRestriction

    (* Template parser *)
    let template = (keyword "template") >>. tuple4 identifier (openBrace >>. opt uuid) (ws >>.  (many (arrayDeclaration <|> variableDeclaration))) (ws >>. opt restriction .>> closeBrace)

    let templates = many (template .>> ws)
