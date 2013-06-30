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

module HelperParser = 
    (* Helper parsers *)
    let ws : Parser<unit, unit> = skipMany (spaces1 <|> (skipChar '#' .>> skipRestOfLine true) <|> (skipString "//" .>> skipRestOfLine true)) |>> ignore
    let identifier : Parser<string, unit> =
        let isIdentifierFirstChar c = isLetter c || c = '_'
        let isIdentifierChar c = isLetter c || isDigit c || c = '_'
        many1Satisfy2L isIdentifierFirstChar isIdentifierChar "identifier"

    let semicolon = ws >>. skipChar ';' .>> ws
    let openBrace = ws >>. skipChar '{' >>. ws
    let closeBrace = ws >>. skipChar '}'
    let uuid = ws >>. between (pchar '<') (pchar '>') (charsTillString ">" false 255) |>> fun value -> new System.Guid(value)
    let keyword value = ws >>. pstring value .>> ws 

    let quotedString : Parser<string, unit> = between (pchar '"') (pchar '"') (charsTillString "\"" false 255)

    let nameUuidPair = opt identifier .>>. (ws >>. opt uuid)

    (* Applies a sequence of parsers in order and returns the results as a list *)
    let applyAll (parsers : Parser<'a, 'u> seq) = 
        fun stream ->
            use it = parsers.GetEnumerator()
            let mutable error = NoErrorMessages
            let mutable status = Ok
            let mutable result = []

            while status = Ok && it.MoveNext() do
                let current = it.Current stream
                if current.Status = Ok then
                    result <- current.Result :: result
                error <- current.Error
                status <- current.Status
            Reply(status, List.rev result, error)

    let parse parser input =
        match run parser input  with
        | Success(result, _, _) -> result
        | Failure(errorMessage, _, _) -> failwith errorMessage

