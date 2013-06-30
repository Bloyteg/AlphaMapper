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
open FParsec.StaticMapping
open Byte.ActionCommand.Parser.ActionCommandAST
open Byte.ActionCommand.Parser.ASTNodeBuilders
open Byte.ActionCommand.Parser.TerminalParsers

module Parser = 
    type Commands =
        | Unknown = 0
        | Ignored = 1
        | Move = 2
        | Rotate = 3
        | Scale = 4
        | Shear = 5
        | Skew = 6
        | Texture = 7
        | Color = 8
        | Picture = 9
        | Animate = 10
        | Visible = 11
        | Opacity = 12
        | Astop = 13
        | Name = 14

    (* Action Command Parsers *)
    let internal globalFlag = opt (flagParameter "global")

    let internal knownCommands = createStaticStringMapping 
                                    Commands.Unknown
                                    [
                                        "move", Commands.Move
                                        "rotate", Commands.Rotate
                                        "scale", Commands.Scale
                                        "shear", Commands.Shear
                                        "skew", Commands.Skew
                                        "texture", Commands.Texture
                                        "color", Commands.Color
                                        "picture", Commands.Picture
                                        "visible", Commands.Visible
                                        "animate", Commands.Animate
                                        "opacity", Commands.Opacity
                                        "astop", Commands.Astop
                                        "name", Commands.Name
                                        (* Ignored Commands *)
                                        "addforce", Commands.Ignored
                                        "addtorque", Commands.Ignored
                                        "alpharef", Commands.Ignored
                                        "astart", Commands.Ignored
                                        "ballsocket", Commands.Ignored
                                        "camera", Commands.Ignored
                                        "collider", Commands.Ignored
                                        "colltag", Commands.Ignored
                                        "corona", Commands.Ignored
                                        "envi", Commands.Ignored
                                        "examine", Commands.Ignored
                                        "frame", Commands.Ignored
                                        "group", Commands.Ignored //This may be useful in the future
                                        "hinge", Commands.Ignored
                                        "light", Commands.Ignored
                                        "link", Commands.Ignored
                                        "lock", Commands.Ignored
                                        "matfx", Commands.Ignored //Maybe?
                                        "media", Commands.Ignored
                                        "midi", Commands.Ignored
                                        "noise", Commands.Ignored
                                        "reset", Commands.Ignored
                                        "say", Commands.Ignored
                                        "seq", Commands.Ignored //Someday?
                                        "shadow", Commands.Ignored //LOL...
                                        "sign", Commands.Ignored
                                        "solid", Commands.Ignored
                                        "sound", Commands.Ignored //STREET VIEW!!!
                                        "tag", Commands.Ignored
                                        "teleport", Commands.Ignored
                                        "teleportx", Commands.Ignored
                                        "timer", Commands.Ignored //Would be useful alongside animate/adone one day
                                        "url", Commands.Ignored
                                        "velocity", Commands.Ignored
                                        "warp", Commands.Ignored
                                        "web", Commands.Ignored
                                    ]
                 
    let internal timeParameter = keywordParameter "time" number
    let internal waitParameter = keywordParameter "wait" number
    let internal nameParameter = keywordParameter "name" identifierWS
    let internal loopParameter = booleanParameter "loop" "noloop"     
    let internal syncParameter = booleanParameter "sync" "nosync"
    let internal resetParameter = booleanParameter "reset" "noreset"
    let internal smoothParameter = flagParameter "smooth"
    let internal tagParameter = keywordParameter "tag" integerWS

    let internal move = 
        let gravityParameter = flagParameter "gravity"
        let ltmParameter = flagParameter "ltm"
        tuple3 number3 (unorderedOpt9 timeParameter waitParameter nameParameter loopParameter syncParameter resetParameter smoothParameter gravityParameter ltmParameter) globalFlag |>> Move

    let internal rotate = tuple3 number3 (unorderedOpt7 timeParameter waitParameter nameParameter loopParameter syncParameter resetParameter smoothParameter) globalFlag |>> Rotate

    let internal scale = tuple3 number3 (unorderedOpt7 timeParameter waitParameter nameParameter loopParameter syncParameter resetParameter smoothParameter) globalFlag |>> Scale

    let internal skew = tuple3 number6 (opt nameParameter) globalFlag |>> Skew

    let internal shear = tuple3 number6 (opt nameParameter) globalFlag |>> Shear

    let internal texture =
        let maskParameter = keywordParameter "mask" identifierWS
        tuple3 identifierWS (unorderedOpt3 maskParameter tagParameter nameParameter) globalFlag |>> Texture

    let internal color = 
        let colorParameter = notFollowedBy (pstringCI "tint") >>. identifierWS
        let tintParameter = flagParameter "tint"
        tuple2 (unorderedOpt3 colorParameter nameParameter tintParameter) globalFlag |>> Color

    let internal picture = 
        let updateParameter = keywordParameter "update" integerWS
        let mipParameter = keywordParameter "mip" (boolean .>> ws)
        let uriParameter = puri .>> ws
        tuple2 (unorderedOpt4 uriParameter updateParameter mipParameter nameParameter) globalFlag |>> Picture

    let internal visible =
        let radiusParameter = (keywordParameter "radius" integerWS)
        let visibilityAndRadius = (radiusParameter .>>.? (boolean .>> ws) |>> fun (radius, visibility) -> (visibility, Some radius)) <|>
                                  ((boolean .>> ws) .>>. opt radiusParameter |>> fun (visibility, radius) -> (visibility, radius))
                                  
        ((visibilityAndRadius |>> fun (visibility, radius) -> (visibility, None, radius)) <|>
         (identifierWS .>>. visibilityAndRadius |>> fun (name, (visibility, radius)) -> (visibility, Some name, radius)))
         .>>. globalFlag |>> Visible

    let internal animate =
        let maskParameter = booleanParameter "mask" "nomask"
        let objectNameParameter = (keyword "me" >>% None) <|> (identifierWS |>> Some)
        let animationParameter = tuple6 objectNameParameter identifierWS integerWS integerWS integerWS (many integerWS)
        tuple4 (opt tagParameter) (opt maskParameter) animationParameter globalFlag |>> Animate

    let internal opacity = tuple3 numberWS (opt nameParameter) globalFlag |>> Opacity
         
    let internal astop = tuple2 (opt identifierWS) globalFlag |>> Astop

    let internal name = identifierWS |>> Name

    //Returns invalid commands and their location within the text.
    let internal skipCommand : Parser<string, _> = takeUntilWithEscape1 (function |';'|',' -> false |_ -> true) (function |'"' -> true | _ -> false)
    let internal invalidCommand = skipCommand |>> InvalidCommand

    //Parses known action commands.
    let internal actionCommands : Parser<Command, _> = 
        fun stream ->
            let line = stream.Line
            let column = stream.Column
            let state = stream.State
            let reply = identifierWS stream
            if reply.Status = Ok then
                match knownCommands (reply.Result.ToLower ()) with
                | Commands.Move -> move stream
                | Commands.Rotate -> rotate stream
                | Commands.Scale -> scale stream
                | Commands.Shear -> shear stream
                | Commands.Skew -> skew stream
                | Commands.Texture -> texture stream
                | Commands.Color -> color stream
                | Commands.Picture -> picture stream
                | Commands.Animate -> animate stream
                | Commands.Visible -> visible stream
                | Commands.Opacity -> opacity stream
                | Commands.Astop -> astop stream
                | Commands.Name -> name stream
                | Commands.Ignored -> (skipCommand |>> (fun str -> reply.Result + " " + str) |>> IgnoredCommand) stream
                | _ -> stream.BacktrackTo(state)
                       invalidCommand stream
            else
                stream.BacktrackTo(state)
                Reply(reply.Status, reply.Error)
                

    let internal commands = sepEndBy ((attempt actionCommands) <|> invalidCommand) (ws .>> pstring "," .>> ws)

    (* Action Trigger Parsers *)
    let internal create = keyword "create" >>. commands |>> Create

    let internal activate = keyword "activate" >>. commands |>> Activate

    let internal bump = keyword "bump" >>. commands |>> Bump

    let internal adone = keyword "adone" >>. commands |>> Adone

    let internal sdone = keyword "sdone" >>. commands |>> Sdone

    let internal collide = keyword "collide" >>. commands |>> Collide

    let internal at = 
        let timer = keyword "tm"  >>. tuple3 identifierWS (ws >>. pfloat .>> ws) (opt (keywordParameter "loop" integerWS)) |>> NamedTimer
        let vrt = keyword "vrt" >>. tuple3 (opt integer .>> skipString ":") (opt integer .>> skipString ":") (opt integer) |>> VRTTimer

        keyword "at" >>. (timer <|> vrt) .>>. (comma >>. commands) |>> At

    let internal namedTrigger = identifierWS .>> comma

    let internal enterZone = keyword "enter zone" >>. namedTrigger .>>. commands |>> EnterZone

    let internal exitZone =  keyword "exit zone" >>. namedTrigger .>>. commands |>> ExitZone

    let internal actionTriggers : Parser<Trigger, _> =
        fun stream ->
            match System.Char.ToLower(stream.Peek()) with
            | 'a' -> choice [ activate; adone; at ] stream
            | 'b' -> bump stream
            | 'c' -> create stream
            | 'e' -> choice [ enterZone; exitZone ] stream
            | 's' -> sdone stream
            | _ -> Reply(Error, ErrorMessageList(ErrorMessage.Unexpected("Unknown action trigger.")))

    (* Primary Parser *)
    let internal parser = ws >>. sepEndBy (ws >>? actionTriggers) (pstring ";" .>> ws) .>> eof     
                
    let parse (input : string) =
        match run parser input with
        | Success(result, _, _) -> result
        | Failure(errorMsg, _, _) -> [InvalidInput(input)]
