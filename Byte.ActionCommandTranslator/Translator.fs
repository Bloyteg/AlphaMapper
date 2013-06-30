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

namespace Byte.ActionCommandTranslator
open Byte.ActionCommand.Parser.Parser
open Byte.ActionCommand.Parser.ActionCommandAST
open Byte.ActionCommandTranslator
open Byte.FSharp.Helpers.Null
open System.Globalization

type public ActionCommandTranslator() =

    (* Function for clamping values between a minimum and maximum range *)
    let clamp value min max =
        match value with
        | _ when value < min -> min
        | _ when value > max -> max
        | _ -> value

    let knownColors = Map.ofList [ ("aquamarine", "70DB93")
                                   ("black", "000000")
                                   ("blue", "0000FF")
                                   ("brass", "B5A642")
                                   ("bronze", "8C7853")
                                   ("brown", "A62A2A")
                                   ("copper", "B87333")
                                   ("cyan", "00FFFF")
                                   ("darkgrey", "303030")
                                   ("forestgreen", "238E23")
                                   ("gold", "CD7F32")
                                   ("green", "00FF00")
                                   ("grey", "707070")
                                   ("lightgrey", "C0C0C0")
                                   ("magenta", "FF00FF")
                                   ("maroon", "8E236B")
                                   ("navyblue", "23238E")
                                   ("orange", "FF7F00")
                                   ("orangered", "FF2400")
                                   ("orchid", "DB70DB")
                                   ("pink", "FF6EC7")
                                   ("red", "FF0000")
                                   ("salmon", "6F4242")
                                   ("scarlet", "8C1717")
                                   ("silver", "E6E8FA")
                                   ("skyblue", "3299CC")
                                   ("tan", "DB9370")
                                   ("teal", "007070")
                                   ("turquoise", "ADEAEA")
                                   ("violet", "4F2F4F")
                                   ("white", "FFFFFF")
                                   ("yellow", "FFFF00") ]

    (* Function for processing color names and values *)
    let processColor color =
        let rec convertFromHex = function
            | "" -> None |> toNull
            | hexValue -> match System.Int32.TryParse (hexValue, NumberStyles.HexNumber, null) with 
                          | (false, _) -> convertFromHex (hexValue.Substring (0, hexValue.Length - 1))
                          | (true, value) -> { R = byte (value >>> 16); G = byte (value >>> 8); B = byte value; }
        match knownColors.TryFind color with
        | Some value -> convertFromHex value
        | None -> convertFromHex color

    (* Functions for processing individual action commands *)
    let handleMove moveCommand properties =
        match moveCommand with
        | Move(position, (time, wait, _, loop, _, reset, _, _, _), _)
            when ((time @?? 1.0) <= 1.0 && (wait @?? 60.0) >= 60.0) ||
                 ((time @?? 1.0 < 0.5) && (loop @?? false) && (reset @?? false)) ->
                match position with
                | (x, None, None) -> { properties with Translation = { X = x; Y = 0.0; Z = 0.0 } }
                | (x, Some y, None) -> { properties with Translation = { X = x; Y = y; Z = 0.0 } }
                | (x, Some y, Some z) -> { properties with Translation = { X = x; Y = y; Z = z } }
                | _ -> properties
        | _ -> properties

    let handleRotate rotateCommand properties =
        match rotateCommand with
        | Rotate(rotation, (time, wait, _, _, _, _, _), _) 
            when (wait @?? 0.0) >= 60.0 && (time @?? 1.0) <= 1.0 ->
                let rpms = (time @?? 0.0) * 60.0
                match rotation with
                | (y, None, None) -> { properties with Rotation = { X = 0.0; Y = y*rpms; Z = 0.0; } }
                | (x, Some y, None) -> { properties with Rotation = { X = x*rpms; Y = y*rpms; Z = 0.0; } }
                | (x, Some y, Some z) -> { properties with Rotation = { X = x*rpms; Y = y*rpms; Z = z*rpms; } }
                | _ -> properties
        | _ -> properties

    let handleScale scaleCommand properties =
        let clampValue = fun value -> clamp value 0.2 5.0
        match scaleCommand with
        | Scale(scale, (time, wait, _, _, _, _, _), _)
            when (time @?? 0.0) <= 1.0 && (wait @?? 9e9) >= 60.0 ->
                match scale with
                | (x, None, None) -> let xClamped = x |> clampValue
                                     { properties with Scale = { X = xClamped; Y = xClamped; Z = xClamped } }
                | (x, Some y, None) -> { properties with Scale = { X = x |> clampValue; Y = y |> clampValue; Z = 1.0 } }
                | (x, Some y, Some z) -> { properties with Scale = { X = x |> clampValue; Y = y |> clampValue; Z = z |> clampValue } }
                | _ -> properties
        | _ -> properties

    let handleSkew skewCommand (properties : ObjectProperties) =
        let clampValue = fun value -> clamp value -89.0 89.0
        match skewCommand with
        | Skew((x1, y1, z1, x2, y2, z2), _, _) ->
            {
                properties with Shear = Some {
                                            Type = ShearType.Skew;
                                            Shear = {
                                                        X1 = x1 |> clampValue;
                                                        Y1 = y1 @?? 0.0 |> clampValue;
                                                        Z1 = z1 @?? 0.0 |> clampValue;
                                                        X2 = x2 @?? 0.0 |> clampValue;
                                                        Y2 = y2 @?? 0.0 |> clampValue;
                                                        Z2 = z2 @?? 0.0 |> clampValue;
                                                    }
                                        }
            }
        | _ -> properties

    let handleShear shearCommand (properties : ObjectProperties) =
        let clampValue = fun value -> clamp value -5.0 5.0
        match shearCommand with
        | Shear((x1, y1, z1, x2, y2, z2), _, _) ->
                {
                    properties with Shear = Some {
                                                Type = ShearType.Shear;
                                                Shear = {
                                                            X1 = x1 |> clampValue;
                                                            Y1 = y1 @?? 0.0 |> clampValue;
                                                            Z1 = z1 @?? 0.0 |> clampValue;
                                                            X2 = x2 @?? 0.0 |> clampValue;
                                                            Y2 = y2 @?? 0.0 |> clampValue;
                                                            Z2 = z2 @?? 0.0 |> clampValue;
                                                        }
                                            }
                }
        | _ -> properties

    let handleTexture textureCommand (properties : ObjectProperties) =
        match textureCommand with
        | Texture(texture, (mask, tag, _), _) ->
            let mask = match mask with
                       | Some(mask) -> Some (File mask)
                       | _ -> None
            let material = match properties.Materials |> Map.tryFind tag with
                           | None -> { Material.Default with Texture = Some { Texture = File texture; Mask = mask; } }
                           | Some(material) -> { material with Texture = Some { Texture = File texture; Mask = mask; } }
            
            { properties with Materials = properties.Materials |> Map.add tag material }
        | _ -> properties
        
    let handlePicture pictureCommand (properties : ObjectProperties) =
        match pictureCommand with
        | Picture((Some uri, _, _, _), _) ->
            let material = match properties.Materials |> Map.tryFind (Some 200) with
                           | None -> { Material.Default with Texture = Some { Texture = Uri uri; Mask = None } }
                           | Some(material) -> { material with Texture = Some { Texture = Uri uri; Mask = None } }

            { properties with Materials = properties.Materials |> Map.add (Some 200) material }
        | _ -> properties

    let handleColor colorCommand (properties : ObjectProperties) =
        match colorCommand with
        | Color((Some color, _, tint), _) ->
            let tint = match tint with
                       | Some () -> true
                       | None -> false

            let material = match properties.Materials |> Map.tryFind None with
                           | None -> { Material.Default with Color = Some { ColorData = (processColor color); Tint = tint } }
                           | Some(material) -> { material with Color = Some { ColorData = (processColor color); Tint = tint } }

            { properties with Materials = properties.Materials |> Map.add None material }

        | _ -> properties

    let handleAnimate animateCommand (properties : ObjectProperties) =
        let buildAnimateTexture name mask tag properties =
            let material = match properties.Materials |> Map.tryFind tag with
                           | None -> { Material.Default with Texture = Some { Texture = File name; Mask = mask } }
                           | Some(material) -> { material with Texture = Some { Texture = File name; Mask = mask; } }

            { properties with Materials = properties.Materials |> Map.add tag material }

        let buildMaskName hasMask (textureName : string) (frameNumber : int) =
            match hasMask with
            | Some true -> File (textureName + frameNumber.ToString() + "m") |> Some
            | _ -> None

        match animateCommand with
        | Animate(tag, _, (_, texture, _, _, _, _), _) when texture.EndsWith(".") ->
            let name = texture.TrimEnd('.')
            buildAnimateTexture name None tag properties

        | Animate(tag, mask, (_, texture, _, _, _, []), _) ->
            let name = texture + (1).ToString()
            let mask = buildMaskName mask texture 1
            buildAnimateTexture name mask tag properties

        | Animate(tag, mask, (_, texture, _, _, _, frame::_), _) ->
            let name = texture + frame.ToString()
            let mask = buildMaskName mask texture frame
            buildAnimateTexture name mask tag properties

        | _ -> properties

    let handleVisible visibleCommand (properties : ObjectProperties) =
        match visibleCommand with
        | Visible((false, _, _), _) -> { properties with Visible = false }
        | _ -> properties

    let handleOpacity opacityCommand (properties : ObjectProperties) =
        match opacityCommand with
        | Opacity(opacity, _, _) ->
            let material = match properties.Materials |> Map.tryFind None with
                           | None -> { Material.Default with Opacity = opacity }
                           | Some(material) -> { material with Opacity = opacity }

            { properties with Materials = properties.Materials |> Map.add None material }

        | _ -> properties


    (* Functions for processing any action command into the corresponding object properties *)
    let getProperties propertiesMap =
        function
        | Move(_, (_, _, name, _, _, _, _, _, _), _)
        | Rotate(_, (_, _, name, _, _, _, _), _) 
        | Scale(_, (_, _, name, _, _, _, _), _)
        | Skew(_, name, _)
        | Shear(_, name, _)
        | Texture(_, (_, _, name), _) 
        | Picture((_, _, _, name), _)
        | Color((_, name, _), _)
        | Visible((_, name, _), _)
        | Animate(_, _, (name, _, _, _, _, _), _) 
        | Opacity(_, name, _) -> (name, (propertiesMap |> Map.tryFind name) @?? ObjectProperties.Default)
        | _ -> (None, (propertiesMap |> Map.tryFind None) @?? ObjectProperties.Default)

    let translateCommand (propertiesMap : Map<string option, ObjectProperties>) command =
        let (name, properties) = getProperties propertiesMap command
        match command with
        | Move(_, _, _) as moveCommand -> propertiesMap |> Map.add name (handleMove moveCommand properties)
        | Rotate(_, _, _) as rotateCommand -> propertiesMap |> Map.add name (handleRotate rotateCommand properties)
        | Scale(_, _, _) as scaleCommand -> propertiesMap |> Map.add name (handleScale scaleCommand properties)
        | Skew(_, _, _) as skewCommand -> propertiesMap |> Map.add name (handleSkew skewCommand properties)
        | Shear(_, _, _) as shearCommand -> propertiesMap |> Map.add name (handleShear shearCommand properties)
        | Texture(_, _, _) as textureCommand -> propertiesMap |> Map.add name (handleTexture textureCommand properties)
        | Picture(_, _) as pictureCommand -> propertiesMap |> Map.add name (handlePicture pictureCommand properties)
        | Color(_, _) as colorCommand -> propertiesMap |> Map.add name (handleColor colorCommand properties)
        | Visible(_, _) as visibleCommand -> propertiesMap |> Map.add name (handleVisible visibleCommand properties)
        | Animate(_, _, _, _) as animateCommand -> propertiesMap |> Map.add name (handleAnimate animateCommand properties)
        | Opacity(_, _, _) as opacityCommand -> propertiesMap |> Map.add name (handleOpacity opacityCommand properties)
        | _ -> propertiesMap

    (* Translate the list of commands into a map of ObjectProperties *)
    let rec translateCommands (propertiesMap : Map<string option, ObjectProperties>) =
        function
        | InvalidCommand(_)::commands -> propertiesMap
        | command::commands -> translateCommands (translateCommand propertiesMap command) commands
        | [] -> propertiesMap

    (* Handles translating triggers into a map of ObjectProperties *)
    let rec translateTriggers (propertiesMap : Map<string option, ObjectProperties>) handleAdoneCase =
        let hasInvalidCommand = List.exists (fun c -> match c with InvalidCommand(_) -> true | _ -> false)
        function
        | Create(commands)::triggers -> translateTriggers (translateCommands propertiesMap commands) handleAdoneCase triggers
        | Adone(commands)::triggers when handleAdoneCase -> translateTriggers (translateCommands propertiesMap commands) handleAdoneCase triggers
        | Activate(commands)::_
        | Bump(commands)::_
        | Adone(commands)::_
        | Sdone(commands)::_ 
        | At(_, commands)::_
        | EnterZone(_, commands)::_
        | ExitZone(_, commands)::_
        | Collide(commands)::_ when (commands |> hasInvalidCommand) -> propertiesMap
        | InvalidInput(_)::_ -> propertiesMap
        | trigger::triggers -> translateTriggers propertiesMap handleAdoneCase triggers
        | [] -> propertiesMap

    (* Determines whether or not if the Adone case should be handled *)
    let shouldHandleAdoneCase triggers =
        let isValidAnimateAdone commands =
            let cachedCommands = commands |> Seq.cache
            let containsAnimateMe = (cachedCommands |> Seq.exists (function | Animate(_, _, (None, _, _, _, _, _), _) -> true | _ -> false))
            let containsAstop = (cachedCommands |> Seq.exists (function | Astop(None, _) -> true | _ -> false))
            let result = containsAnimateMe && (not containsAstop)

            result

        triggers |> Seq.tryPick (function 
                                 | Create(commands) -> Some commands
                                 | _ -> None)
                 |> function
                    | Some(commands) -> commands |> isValidAnimateAdone
                    | None -> false

    (* Extracts the object name *)
    let objectName triggers =
        triggers |> Seq.tryPick (function
                                 | Create(commands) -> Some commands
                                 | _ -> None)
                 |> function
                    | Some(commands) -> 
                        commands |> Seq.tryPick (function 
                                                 | Name(name) -> Some name
                                                 | _ -> None)
                    | None -> None

    (* Public method that takes an action command and returns an ObjectProperties *)
    member public this.ToObjectProperties (input : string) = 
        let triggers = (parse input)
        let name = triggers |> objectName
        (name, triggers |> translateTriggers Map.empty (shouldHandleAdoneCase triggers))
