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
open Byte.ActionCommand.Parser
open Byte.ActionCommand.Parser.ActionCommandAST

[<TestFixture>]
module ParserTest =

    (* Command tests *)
    [<Test>]
    let ``commands should be split by commas`` () =
        match Parser.parse "create solid no, visible no" with
        | [Create([ _; _ ])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``command followed by comma is valid`` () =
        match Parser.parse "create solid no," with
        | [Create([ _ ])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``commmand ignores known commands``() =
        match Parser.parse "create say \"hello\r\nthere!\"" with
        | [Create([IgnoredCommand(_)])] -> pass()
        | _ -> fail()

    (* Move command tests *)
    [<Test>]
    let ``move should only require one position parameter`` () =
        match Parser.parse "create move 1" with
        | [Create([Move((1.0, None, None), _, _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``move can have two position parameters`` () =
        match Parser.parse "create move 1 1" with
        | [Create([Move((1.0, Some(1.0), None), _, _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``move can have three position parameters`` () =
        match Parser.parse "create move 1 1 1" with
        | [Create([Move((1.0, Some(1.0), Some(1.0)), _, _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``move should allow time parameter`` () =
        match Parser.parse "create move 1 time=1" with
        | [Create([Move((1.0, None, None), (Some(1.0), _, _, _, _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``move should allow wait parameter`` () =
        match Parser.parse "create move 1 wait=1" with
        | [Create([Move((1.0, None, None), (None, Some(1.0), _, _, _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``move should allow name parameter`` () =
        match Parser.parse "create move 1 name=test" with
        | [Create([Move((1.0, None, None), (None, None, Some("test"), _, _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``move should allow time then wait parameter`` () =
        match Parser.parse "create move 1 time=1 wait=1" with
        | [Create([Move((1.0, None, None), (Some(1.0), Some(1.0), _, _, _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``move should allow wait then time parameter`` () =
        match Parser.parse "create move 1 wait=1 time=1" with
        | [Create([Move((1.0, None, None), (Some(1.0), Some(1.0), _, _, _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``move should allow same parameter twice, taking only the final occurrence`` () =
        match Parser.parse "create move 1 time=1 time=2" with
        | [Create([Move((1.0, None, None), (Some(2.0), _, _, _, _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    (* Rotate Command Tests *)
    [<Test>]
    let ``rotate should only require one rotation parameter`` () =
        match Parser.parse "create rotate 1" with
        | [Create([Rotate((1.0, None, None), _, _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``rotate can have two rotation parameters`` () =
        match Parser.parse "create rotate 1 1" with
        | [Create([Rotate((1.0, Some(1.0), None), _, _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``rotate can have three rotation parameters`` () =
        match Parser.parse "create rotate 1 1 1" with
        | [Create([Rotate((1.0, Some(1.0), Some(1.0)), _, _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``rotate should allow time parameter`` () =
        match Parser.parse "create rotate 1 time=1" with
        | [Create([Rotate((1.0, None, None), (Some(1.0), _, _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``rotate should allow wait parameter`` () =
        match Parser.parse "create rotate 1 wait=1" with
        | [Create([Rotate((1.0, None, None), (None, Some(1.0), _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``rotate should allow name parameter`` () =
        match Parser.parse "create rotate 1 name=test" with
        | [Create([Rotate((1.0, None, None), (None, None, Some("test"), _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``rotate should allow time then wait parameter`` () =
        match Parser.parse "create rotate 1 time=1 wait=1" with
        | [Create([Rotate((1.0, None, None), (Some(1.0), Some(1.0), _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``rotate should allow wait then time parameter`` () =
        match Parser.parse "create rotate 1 wait=1 time=1" with
        | [Create([Rotate((1.0, None, None), (Some(1.0), Some(1.0), _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    (* Scale Command Tests *)
    [<Test>]
    let ``scale should only require one scaling parameter`` () =
        match Parser.parse "create scale 1" with
        | [Create([Scale((1.0, None, None), _, _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``scale can have two scaling parameters`` () =
        match Parser.parse "create scale 1 1" with
        | [Create([Scale((1.0, Some(1.0), None), _, _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``scale can have three scaling parameters`` () =
        match Parser.parse "create scale 1 1 1" with
        | [Create([Scale((1.0, Some(1.0), Some(1.0)), _, _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``scale should allow time parameter`` () =
        match Parser.parse "create scale 1 time=1" with
        | [Create([Scale((1.0, None, None), (Some(1.0), _, _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``scale should allow wait parameter`` () =
        match Parser.parse "create scale 1 wait=1" with
        | [Create([Scale((1.0, None, None), (None, Some(1.0), _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``scale should allow name parameter`` () =
        match Parser.parse "create scale 1 name=test" with
        | [Create([Scale((1.0, None, None), (None, None, Some("test"), _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``scale should allow time then wait parameter`` () =
        match Parser.parse "create scale 1 time=1 wait=1" with
        | [Create([Scale((1.0, None, None), (Some(1.0), Some(1.0), _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``scale should allow wait then time parameter`` () =
        match Parser.parse "create scale 1 wait=1 time=1" with
        | [Create([Scale((1.0, None, None), (Some(1.0), Some(1.0), _, _, _, _, _), _)])] -> pass()
        | _ -> fail()

    (* Shear Tests *)
    [<Test>]
    let ``shear requires at least one element`` () =
        match Parser.parse "create shear 1" with
        | [Create([Shear((1.0, None, None, None, None, None), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``shear allows two elements`` () =
        match Parser.parse "create shear 1 1" with
        | [Create([Shear((1.0, Some 1.0, None, None, None, None), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``shear allows three elements`` () =
        match Parser.parse "create shear 1 1 1" with
        | [Create([Shear((1.0, Some 1.0, Some 1.0, None, None, None), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``shear allows four elements`` () =
        match Parser.parse "create shear 1 1 1 1" with
        | [Create([Shear((1.0, Some 1.0, Some 1.0, Some 1.0, None, None), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``shear allows five elements`` () =
        match Parser.parse "create shear 1 1 1 1 1" with
        | [Create([Shear((1.0, Some 1.0, Some 1.0, Some 1.0, Some 1.0, None), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``shear allows six elements`` () =
        match Parser.parse "create shear 1 1 1 1 1 1" with
        | [Create([Shear((1.0, Some 1.0, Some 1.0, Some 1.0, Some 1.0, Some 1.0), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``shear allows name parameter`` () =
        match Parser.parse "create shear 1 name=test" with
        | [Create([Shear((1.0, None, None, None, None, None), Some "test", None)])] -> pass()
        | _ -> fail()

    (* Skew Tests *)
    [<Test>]
    let ``skew requires at least one element`` () =
        match Parser.parse "create skew 1" with
        | [Create([Skew((1.0, None, None, None, None, None), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``skew allows two elements`` () =
        match Parser.parse "create skew 1 1" with
        | [Create([Skew((1.0, Some 1.0, None, None, None, None), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``skew allows three elements`` () =
        match Parser.parse "create skew 1 1 1" with
        | [Create([Skew((1.0, Some 1.0, Some 1.0, None, None, None), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``skew allows four elements`` () =
        match Parser.parse "create skew 1 1 1 1" with
        | [Create([Skew((1.0, Some 1.0, Some 1.0, Some 1.0, None, None), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``skew allows five elements`` () =
        match Parser.parse "create skew 1 1 1 1 1" with
        | [Create([Skew((1.0, Some 1.0, Some 1.0, Some 1.0, Some 1.0, None), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``skew allows six elements`` () =
        match Parser.parse "create skew 1 1 1 1 1 1" with
        | [Create([Skew((1.0, Some 1.0, Some 1.0, Some 1.0, Some 1.0, Some 1.0), None, None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``skew allows name parameter`` () =
        match Parser.parse "create skew 1 name=test" with
        | [Create([Skew((1.0, None, None, None, None, None), Some "test", None)])] -> pass()
        | _ -> fail()

    (* Texture tests *)
    [<Test>]
    let ``texture requires a value`` () =
        match Parser.parse "create texture wood2" with
        | [Create([Texture("wood2", (None, None, None), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``texture allows . in name`` () =
        match Parser.parse "create texture wood2.jpg" with
        | [Create([Texture("wood2.jpg", (None, None, None), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``texture allows for mask`` () =
        match Parser.parse "create texture wood2 mask=wood2m" with
        | [Create([Texture("wood2", (Some "wood2m", None, None), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``texture allows for mask with . in name`` () =
        match Parser.parse "create texture wood2 mask=wood2m.bmp" with
        | [Create([Texture("wood2", (Some "wood2m.bmp", None, None), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``texture allows for tag`` () =
        match Parser.parse "create texture wood2 tag=200" with
        | [Create([Texture("wood2", (None, Some 200, None), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``texture allows for tag then mask`` () =
        match Parser.parse "create texture wood2 tag=200 mask=wood2m" with
        | [Create([Texture("wood2", (Some "wood2m", Some 200, None), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``texture allows for mask then tag`` () =
        match Parser.parse "create texture wood2 mask=wood2m tag=200" with
        | [Create([Texture("wood2", (Some "wood2m", Some 200, None), None)])] -> pass()
        | _ -> fail()

    (* Color tests *)
    [<Test>]
    let ``color has color value parameter`` () =
        match Parser.parse "create color red" with
        | [Create([Color((Some "red", None, None), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``color allows tint parameter after color name`` () =
        match Parser.parse "create color red tint" with
        | [Create([Color((Some "red", None, Some ()), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``color allows tint parameter before color name`` () =
        match Parser.parse "create color tint red" with
        | [Create([Color((Some "red", None, Some ()), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``color allows tint parameter before color name with name parameter first`` () =
        match Parser.parse "create color name=test tint red" with
        | [Create([Color((Some "red", Some "test", Some ()), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``color allows tint parameter after color name with name parameter first`` () =
        match Parser.parse "create color name=test red tint" with
        | [Create([Color((Some "red", Some "test", Some ()), None)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``color allows tint parameter after color name with name parameter last`` () =
        match Parser.parse "create color red tint name=test" with
        | [Create([Color((Some "red", Some "test", Some ()), None)])] -> pass()
        | _ -> fail()

    (* Picture tests *)
    [<Test>]
    let ``picture allows url parameter`` () =
        match Parser.parse "create picture www.test.com/test.jpg" with
        | [Create([Picture((Some uri, None, None, None), _)])] when uri.AbsoluteUri = "http://www.test.com/test.jpg" -> pass()
        | _ -> fail()

    [<Test>]
    let ``picture allows url parameter before update parameter`` () =
        match Parser.parse "create picture www.test.com/test.jpg update=60" with
        | [Create([Picture((Some uri, Some 60, None, None), _)])] when uri.AbsoluteUri = "http://www.test.com/test.jpg" -> pass()
        | _ -> fail()

    [<Test>]
    let ``picture allows url parameter after update parameter`` () =
        match Parser.parse "create picture update=60 www.test.com/test.jpg" with
        | [Create([Picture((Some uri, Some 60, None, None), _)])] when uri.AbsoluteUri = "http://www.test.com/test.jpg" -> pass()
        | _ -> fail()

    [<Test>]
    let ``picture allows url parameter after update parameter followed by mip parameter`` () =
        match Parser.parse "create picture update=60 www.test.com/test.jpg mip=on" with
        | [Create([Picture((Some uri, Some 60, Some true, None), _)])] when uri.AbsoluteUri = "http://www.test.com/test.jpg" -> pass()
        | _ -> fail()

    [<Test>]
    let ``picture allows url parameter after update parameter preceeded by mip parameter`` () =
        match Parser.parse "create picture update=60 mip=on www.test.com/test.jpg" with
        | [Create([Picture((Some uri, Some 60, Some true, None), _)])] when uri.AbsoluteUri = "http://www.test.com/test.jpg" -> pass()
        | _ -> fail()

    (* Animate tests *)
    [<Test>]
    let ``animate allows self reference with static frames`` () =
        match Parser.parse "create animate me test 1 1 100" with
        | [Create([Animate(None, None, (None, "test", 1, 1, 100, []), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``animate allows self reference with defined frames`` () =
        match Parser.parse "create animate me test 5 5 100 1 2 3 4 5" with
        | [Create([Animate(None, None, (None, "test", 5, 5, 100, [1; 2; 3; 4; 5]), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``animate allows external reference with static frames`` () =
        match Parser.parse "create animate other test 1 1 100" with
        | [Create([Animate(None, None, (Some "other", "test", 1, 1, 100, []), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``animate allows tag`` () =
        match Parser.parse "create animate tag=200 me test 1 1 100" with
        | [Create([Animate(Some 200, None, (None, "test", 1, 1, 100, []), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``animate allows mask`` () =
        match Parser.parse "create animate mask  me test 1 1 100" with
        | [Create([Animate(None, Some true, (None, "test", 1, 1, 100, []), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``animate allows nomask`` () =
        match Parser.parse "create animate nomask me test 1 1 100" with
        | [Create([Animate(None, Some false, (None, "test", 1, 1, 100, []), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``animate allows tag and mask`` () =
        match Parser.parse "create animate tag=200 mask me test 1 1 100" with
        | [Create([Animate(Some 200, Some true, (None, "test", 1, 1, 100, []), _)])] -> pass()
        | _ -> fail()

    (* Visible tests *)
    [<Test>]
    let ``visible allows for false value`` () =
        match Parser.parse "create visible no" with
        | [Create([Visible((false, None, None), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``visible allows for false value with radius before flag`` () =
        match Parser.parse "create visible radius=50 no" with
        | [Create([Visible((false, None, Some 50), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``visible allows for false value with radius after flag`` () =
        match Parser.parse "create visible no radius=50" with
        | [Create([Visible((false, None, Some 50), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``visible allows for false value with name`` () =
        match Parser.parse "create visible test no" with
        | [Create([Visible((false, Some "test", None), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``visible allows for false value with name and radius before flag`` () =
        match Parser.parse "create visible test radius=50 no" with
        | [Create([Visible((false, Some "test", Some 50), _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``visible allows for false value with name and radius after flag`` () =
        match Parser.parse "create visible test no radius=50" with
        | [Create([Visible((false, Some "test", Some 50), _)])] -> pass()
        | _ -> fail()

    (* Opacity tests *)
    [<Test>]
    let ``opacity has value`` () =
        match Parser.parse "create opacity 0.5" with
        | [Create([Opacity(0.5, None, _)])] -> pass()
        | _ -> fail()

    [<Test>]
    let ``opacity has value and name`` () =
        match Parser.parse "create opacity 0.5 name=test" with
        | [Create([Opacity(0.5, Some "test", _)])] -> pass()
        | _ -> fail()

    (* Trigger tests *)
    [<Test>]
    let ``create trigger should yield Create case`` () =
        match Parser.parse("create test") with
        | [Create(_)] -> pass()
        | _ -> fail()

    [<Test>]
    let ``activate trigger should yield Activate case`` () =
        match Parser.parse("activate test") with
        | [Activate(_)] -> pass()
        | _ -> fail()  

    [<Test>]
    let ``bump trigger should yield Bump case`` () =
        match Parser.parse("bump test") with
        | [Bump(_)] -> pass()
        | _ -> fail()  

    [<Test>]
    let ``adone trigger should yield Adone case`` () =
        match Parser.parse("adone test") with
        | [Adone(_)] -> pass()
        | _ -> fail()  

    [<Test>]
    let ``sdone trigger should yield Sdone case`` () =
        match Parser.parse("sdone test") with
        | [Sdone(_)] -> pass()
        | _ -> fail()  

    [<Test>]
    let ``enter zone trigger should yield EnterZone case with name`` () =
        match Parser.parse("enter zone zone1, test") with
        | [EnterZone("zone1", _)] -> pass()
        | _ -> fail()  

    [<Test>]
    let ``exit zone trigger should yield ExitZone case with name`` () =
        match Parser.parse("exit zone zone1, test") with
        | [ExitZone("zone1", _)] -> pass()
        | _ -> fail()

    [<Test>]
    let ``at trigger with tm parameter should yield At case with NamedTimer`` () =
        match Parser.parse("at tm abc 1000, test") with
        | [At(NamedTimer("abc", 1000.0, None), _)] -> pass()
        | _ -> fail() 

    [<Test>]
    let ``at trigger with tm and loop parameters should yield At case with NamedTimer and loop value`` () =
        match Parser.parse("at tm abc 1000 loop=10, test") with
        | [At(NamedTimer("abc", 1000.0, Some(10)), _)] -> pass()
        | _ -> fail() 

    [<Test>]
    let ``at trigger with VRT parameter should yield At case with VRTTimer`` () =
        match Parser.parse("at VRT 01:00:00, test") with
        | [At(VRTTimer(Some(1), Some(0), Some(0)), _)] -> pass()
        | _ -> fail()

    [<Test>]
    let ``at trigger with VRT parameter and no hour should yield At case with VRTTimer`` () =
        match Parser.parse("at VRT :00:00, test") with
        | [At(VRTTimer(None, Some(0), Some(0)), _)] -> pass()
        | _ -> fail() 

    [<Test>]
    let ``at trigger with VRT parameter and no hour and minute should yield At case with VRTTimer`` () =
        match Parser.parse("at VRT ::00, test") with
        | [At(VRTTimer(None, None, Some(0)), _)] -> pass()
        | _ -> fail()

    [<Test>]
    let ``at trigger with VRT parameter and no hour, minute, and second should yield At case with VRTTimer`` () =
        match Parser.parse("at VRT ::, test") with
        | [At(VRTTimer(None, None, None), _)] -> pass()
        | _ -> fail() 

    [<Test>]
    let ``triggers should be split by semicolons`` () =
        match Parser.parse "create solid no; activate solid yes" with
        | [Create(_); Activate(_)] -> pass()
        | _ -> fail()

    [<Test>]
    let ``trigger followed by semicolon is valid`` () =
        match Parser.parse "create solid no;" with
        | [Create(_)] -> pass()
        | _ -> fail()

    [<Test>]
    let ``trigger followed by no semicolon is valid`` () =
        match Parser.parse "create solid no;" with
        | [Create(_)] -> pass()
        | _ -> fail()

    [<Test>]
    let ``triggers are case-insensitive`` () =
        match Parser.parse "CREATE TEST" with
        | [Create(_)] -> pass()
        | _ -> fail()

