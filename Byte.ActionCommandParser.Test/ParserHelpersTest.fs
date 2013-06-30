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

namespace Byte.ActionCommand.Parser.Test
open NUnit.Framework
open Byte.FSharp.TestHelper.NUnitHelpers
open Byte.ActionCommand.Parser.ParserHelpers
open FParsec

[<TestFixture>]
module ParserHelpersTests =
    let internal fail = Byte.FSharp.TestHelper.NUnitHelpers.fail;

    [<Test>]
    let ``unorderedOpt2 returns two items that exist`` () =
        match run (unorderedOpt2 (pstring "hello") (pstring "world")) "helloworld" with
        | Success(result, _, _) -> match result with
                                   | (Some("hello"), Some("world")) -> pass()
                                   | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()

    [<Test>]
    let ``unorderedOpt2 returns two items that are out of order`` () =
        match run (unorderedOpt2 (pstring "hello") (pstring "world")) "worldhello" with
        | Success(result, _, _) -> match result with
                                   | (Some("hello"), Some("world")) -> pass()
                                   | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()

    [<Test>]
    let ``unorderedOpt2 returns one item with right missing`` () =
        match run (unorderedOpt2 (pstring "hello") (pstring "world")) "hello" with
        | Success(result, _, _) -> match result with
                                   | (Some("hello"), None) -> pass()
                                   | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()

    [<Test>]
    let ``unorderedOpt2 returns one item with left missing`` () =
        match run (unorderedOpt2 (pstring "hello") (pstring "world")) "world" with
        | Success(result, _, _) -> match result with
                                   | (None, Some("world")) -> pass()
                                   | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()

    [<Test>]
    let ``unorderedOpt2 returns no items when left and right are missing`` () =
        match run (unorderedOpt2 (pstring "hello") (pstring "world")) "" with
        | Success(result, _, _) -> match result with
                                   | (None, None) -> pass()
                                   | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()

    [<Test>]
    let ``unorderedOpt2 returns two items of different types`` () =
        match run (unorderedOpt2 (pstring "hello") (pfloat)) "hello3.33" with
        | Success(result, _, _) -> match result with
                                   | (Some("hello"), Some(3.33)) -> pass()
                                   | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()

    [<Test>]
    let ``parse valid uri`` () =
        match run (puri) "http://test.com/" with
        | Success(result, _, _) -> if result.AbsoluteUri = "http://test.com/" then pass() else Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()

    [<Test>]
    let ``parse valid uri without protocol`` () =
        match run (puri) "test.com" with
        | Success(result, _, _) -> if result.AbsoluteUri = "http://test.com/" then pass() else Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()

    [<Test>]
    let ``parse valid uri with query string`` () =
        match run (puri) "http://test.com?x=5&q=ss%20&t=5eB34" with
        | Success(result, _, _) -> if result.AbsoluteUri = "http://test.com/?x=5&q=ss%20&t=5eB34" then pass() else Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> Byte.FSharp.TestHelper.NUnitHelpers.fail()

    [<Test>]
    let ``parse invalid url`` () =
        match run (puri) "name=test.com" with
        | Success(_, _, _) -> Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> pass()

    [<Test>]
    let ``parse invalid url with single word`` () =
        match run (puri) "name" with
        | Success(_, _, _) -> Byte.FSharp.TestHelper.NUnitHelpers.fail()
        | _ -> pass()

    (* stringUntilWithEscape tests *)
    let internal validChars =
        function
        | ';' | ',' -> false
        | _ -> true

    let internal escapeChars =
        function
        | '"' -> true
        | _ -> false

    [<Test>]
    let ``takeUntilWithEscape takes until false char`` () =
        match run (takeUntilWithEscape1 validChars escapeChars) "test;" with
        | Success("test", _, _) -> pass();
        | _ -> fail()

    [<Test>]
    let ``takeUntilWithEscape takes until EOF`` () =
        match run (takeUntilWithEscape1 validChars escapeChars) "test" with
        | Success("test", _, _) -> pass();
        | _ -> fail()

    [<Test>]
    let ``takeUntilWithEscape skips over invalid char inside escape sequence`` () =
        match run (takeUntilWithEscape1 validChars escapeChars) "test\"test,test\"" with
        | Success("test\"test,test\"", _, _) -> pass();
        | _ -> fail()

    [<Test>]
    let ``takeUntilWithEscape skips over invalid char inside escape sequence, but stops after escape sequence`` () =
        match run (takeUntilWithEscape1 validChars escapeChars) "test\"test,test\"test,test" with
        | Success("test\"test,test\"test", _, _) -> pass();
        | _ -> fail()
