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
open System
open System.Linq

[<AutoOpen>]
module public ParserHelpers =
    let internal tryCreateUri (input : string) : Uri option =
        match Uri.TryCreate(input, UriKind.Absolute) with
        | (true, uri) -> Some(uri)
        | (false, _) when input.Any (fun c -> c = '.') ->
            match Uri.TryCreate("http://" + input, UriKind.Absolute) with
            | (true, uri) -> Some(uri)
            | _ -> None
        | _ -> None 

    let puri : Parser<Uri, unit> =
        let stringParser = many1Satisfy (function |' '|'\n'|'\t' -> false |_ -> true)
        fun stream ->
            let state = stream.State
            let mutable reply = stringParser stream
            
            if reply.Status <> Ok then
                stream.BacktrackTo state
                Reply(reply.Status, reply.Error)
            else
                match tryCreateUri reply.Result with
                | None -> stream.BacktrackTo state
                          Reply(Error, ErrorMessageList(ErrorMessage.Message("Invalid URI format.")))
                | Some(uri) -> Reply(uri)

    let internal unorderedOptList (ps : Parser<'a, 'u> list) = 
        fun (stream : CharStream<'u>) ->
            let keys = [ 0 .. (ps |> List.length) - 1 ]
            let parsers = ref (ps |> List.zip keys |> Map.ofList)
            let results = keys |> List.map (fun key -> (key, ref (Reply(None)))) |> Map.ofList
            let mutable continueLooping = true
        
            let runParser (key : int) (value : Parser<'a, 'u>) =
                let state = stream.State
                let result : Reply<'a> = (value stream)
                
                if result.Status = Ok then
                    //NOTE:  The below line, if uncommented, allows only one occurence of each parser.
                    //parsers := !parsers |> Map.remove key
                    Some(key, result)
                else
                    if stream.State <> state then
                        stream.BacktrackTo(state)
                    None

            while continueLooping do
                match !parsers |> Map.tryPick runParser with
                | None -> continueLooping <- false
                | Some(key, value) -> results.[key] := Reply(Some(value.Result))

            results |> Seq.sortBy (fun kvp -> kvp.Key) |> Seq.map (fun kvp -> !kvp.Value) |> List.ofSeq

    let internal unboxOption (value : obj option) : 't option =
        match value with
        | Some(o) -> Some(o |> unbox)
        | None -> None

    let unorderedOpt2 p1 p2 : Parser<'a option * 'b option, 'u> =
        let parserList = [ p1 |>> box; p2 |>> box] 
        fun (stream : CharStream<'u>) ->
            match unorderedOptList parserList stream with
            | [ r1; r2 ]  -> Reply ((r1.Result |> unboxOption, r2.Result |> unboxOption))
            | _ -> raise (System.InvalidOperationException())
   
    let unorderedOpt3 p1 p2 p3 : Parser<'a option * 'b option * 'c option, 'u> =
        let parserList = [ p1 |>> box; p2 |>> box; p3 |>> box]
        fun (stream : CharStream<'u>) ->
            match unorderedOptList parserList stream with
            | [ r1; r2; r3 ] -> Reply ((r1.Result |> unboxOption, r2.Result |> unboxOption, r3.Result |> unboxOption))
            | _ -> raise (System.InvalidOperationException())

    let unorderedOpt4 p1 p2 p3 p4 : Parser<'a option * 'b option * 'c option * 'd option, 'u> =
        let parserList = [ p1 |>> box; p2 |>> box; p3 |>> box; p4 |>> box] 
        fun (stream : CharStream<'u>) ->
            match unorderedOptList parserList stream with
            | [ r1; r2; r3; r4 ] -> Reply ((r1.Result |> unboxOption, r2.Result |> unboxOption, r3.Result |> unboxOption, r4.Result |> unboxOption))
            | _ -> raise (System.InvalidOperationException())

    let unorderedOpt5 p1 p2 p3 p4 p5 : Parser<'a option * 'b option * 'c option * 'd option * 'e option, 'u>  =
        let parserList = [ p1 |>> box; p2 |>> box; p3 |>> box; p4 |>> box; p5 |>> box ] 
        fun (stream : CharStream<'u>) ->
            match unorderedOptList parserList stream with
            | [ r1; r2; r3; r4; r5 ] -> Reply ((r1.Result |> unboxOption, r2.Result |> unboxOption, r3.Result |> unboxOption, r4.Result |> unboxOption, r5.Result |> unboxOption))
            | _ -> raise (System.InvalidOperationException())

    let unorderedOpt7 p1 p2 p3 p4 p5 p6 p7 : Parser<'a option * 'b option * 'c option * 'd option * 'e option * 'f option * 'g option, 'u>  =
        let parserList = [ p1 |>> box; p2 |>> box; p3 |>> box; p4 |>> box; p5 |>> box; p6 |>> box; p7 |>> box ] 
        fun (stream : CharStream<'u>) ->
            match unorderedOptList parserList stream with
            | [ r1; r2; r3; r4; r5; r6; r7 ] -> Reply ((r1.Result |> unboxOption, r2.Result |> unboxOption, r3.Result |> unboxOption, r4.Result |> unboxOption, r5.Result |> unboxOption, r6.Result |> unboxOption, r7.Result |> unboxOption))
            | _ -> raise (System.InvalidOperationException())

    let unorderedOpt9 p1 p2 p3 p4 p5 p6 p7 p8 p9 : Parser<'a option * 'b option * 'c option * 'd option * 'e option * 'f option * 'g option * 'h option * 'i option, 'u>  =
        let parserList = [ p1 |>> box; p2 |>> box; p3 |>> box; p4 |>> box; p5 |>> box; p6 |>> box; p7 |>> box; p8 |>> box; p9 |>> box ] 
        fun (stream : CharStream<'u>) ->
            match unorderedOptList parserList stream with
            | [ r1; r2; r3; r4; r5; r6; r7; r8; r9 ] -> Reply ((r1.Result |> unboxOption, r2.Result |> unboxOption, r3.Result |> unboxOption, r4.Result |> unboxOption, r5.Result |> unboxOption, r6.Result |> unboxOption, r7.Result |> unboxOption, r8.Result |> unboxOption, r9.Result |> unboxOption))
            | _ -> raise (System.InvalidOperationException())

    let tuple6 p1 p2 p3 p4 p5 p6 =
        tuple5 p1 p2 p3 p4 (tuple2 p5 p6) |>> fun (r1, r2, r3, r4, (r5, r6)) -> (r1, r2, r3, r4, r5, r6)

    let takeUntilWithEscape1 (charUntil: char -> bool) (charEscaped: char -> bool) : Parser<string, 'u> =
        fun stream ->
            let mutable escaped = false
            let mutable continueLooping = true
            let mutable charCount = 0
            let sb = new System.Text.StringBuilder()

            while continueLooping do
                if stream.IsEndOfStream then
                    continueLooping <- false
                else
                    let peekedChar = stream.Peek()
                    if (charEscaped peekedChar) then
                        escaped <- not escaped
                        sb.Append(stream.Read()) |> ignore
                        charCount <- charCount + 1
                    elif (charUntil peekedChar) || escaped then
                        sb.Append(stream.Read()) |> ignore
                        charCount <- charCount + 1                       
                    else
                        continueLooping <- false
            
            if charCount > 0 then
                Reply(sb.ToString())
            else
                Reply(Error, ErrorMessageList(ErrorMessage.Expected("Expected at least one character.")))

module internal TerminalParsers =        
    
    (* Options *)
    let numberFormat =   
            NumberLiteralOptions.AllowMinusSign
        ||| NumberLiteralOptions.AllowPlusSign
        ||| NumberLiteralOptions.AllowFractionWOIntegerPart
        ||| NumberLiteralOptions.AllowFraction
        ||| NumberLiteralOptions.AllowExponent

    let integerFormat = NumberLiteralOptions.DefaultInteger
    let integerStringFormat = NumberLiteralOptions.AllowPlusSign ||| NumberLiteralOptions.AllowMinusSign

    (* Helper Parsers *)
    let ws = spaces
    let comma : Parser<string, unit> = ws >>. pstring "," .>> ws
    let equals : Parser<unit, unit> = ws >>. skipString "="

    let identifierLiteral : Parser<string, unit> = many1Satisfy (function |' '|'\n'|'\t'|';'|','|'=' -> false |_ -> true)
    let identifierWS = identifierLiteral .>> ws .>> notFollowedBy (pchar '=')

    let keyword name = skipStringCI name >>. ws
    
    (* Primitive Value Parsers *)
//    //Hexdecimnal values
//    let hexDigit : Parser<char, unit> = anyOf "0123456789abcdefABCDEF"
//    let hexValue = attempt (parray 6 hexDigit |>> fun hexArray -> Seq.fold (fun current next -> current + next) "" (Seq.map (fun c -> c.ToString()) hexArray))

    //Integer values
    let integer : Parser<int, unit> = numberLiteral(integerFormat) "integer" |>> fun numberLiteral -> int numberLiteral.String
    let integerWS = integer .>> ws

    //Floating point number values
    let number : Parser<float, unit> = numberLiteral(numberFormat) "number" |>> fun numberLiteral -> float numberLiteral.String
    let numberWS = number .>> ws
    let number3 = tuple3 numberWS (opt numberWS) (opt numberWS)
    let number6 = tuple2 number3 (opt number3) |>> 
                  fun ((x, y, z), part) -> match part with
                                           | Some (u, v, w) -> (x, y, z, Some u, v, w)
                                           | None -> (x, y, z, None, None, None)
    
    //Boolean values
    let boolean : Parser<bool, unit> = 
        let falseValue = (skipStringCI "off" <|> skipStringCI "no" <|> skipStringCI "false") >>% false
        let trueValue = (skipStringCI "on" <|> skipStringCI "yes" <|> skipStringCI "true") >>% true
        falseValue <|> trueValue

    (* String Parsers *)
    let stringLiteral : Parser<string, unit> = between (pstring "\"") (pstring "\"") (charsTillString "\"" false 255)

    (* Parameter parsers *)
    let keywordParameter name valueParser = ((keyword name) >>? equals) >>? valueParser .>>? ws
    let booleanParameter trueName falseName = ((stringCIReturn trueName true) <|> (stringCIReturn falseName false)) .>> (notFollowedBy equals) .>> ws
    let flagParameter name = (stringCIReturn name ()) .>> (notFollowedBy equals) .>> ws
    let stringParameter : Parser<string, unit> = many1Satisfy (function |','|';'|'\n' -> false |_ -> true) |>> (fun s -> s.Trim())
    let listParameter ptype separator = (sepEndBy ptype (pstring separator))
