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

namespace Byte.RWX.Parser
open FParsec
open AbstractSyntaxTree

module Parser =
    let identifierLiteral : Parser<string, unit> = many1Satisfy (function |' '|'\n'|'\t' -> false |_ -> true)

    let internal numberFormat =   NumberLiteralOptions.AllowMinusSign
                              ||| NumberLiteralOptions.AllowPlusSign
                              ||| NumberLiteralOptions.AllowFractionWOIntegerPart
                              ||| NumberLiteralOptions.AllowFraction
                              ||| NumberLiteralOptions.AllowExponent

    let internal integerFormat = NumberLiteralOptions.DefaultInteger

    (* Helper Parsers *)
    let internal comment : Parser<unit, unit> = skipString "#" >>. (skipRestOfLine false)
    let internal spaces = skipMany (choice [ spaces1; comment ]) |>> ignore
    let internal ws = skipMany (choice [ (pstring " ") |>> ignore; (pstring "\t") |>> ignore; (pstring "#!") |>> ignore; comment ])
    let internal eol = skipRestOfLine false

    let internal keyword name = skipStringCI name >>. ws
    let internal keywordExt name = skipStringCI name >>. (optional (skipStringCI "ext")) >>. ws
    let internal keywordPlural name = skipStringCI name >>. (optional (skipStringCI "s")) >>. ws

    let internal floatNumber = numberLiteral(numberFormat) "number" |>> fun numberLiteral -> float numberLiteral.String
    let internal floatNumberWS = floatNumber .>> ws
    let internal floatNumber2 = tuple2 floatNumberWS floatNumberWS
    let internal floatNumber3 = tuple3 floatNumberWS floatNumberWS floatNumberWS
    let internal floatNumber4 = tuple4 floatNumberWS floatNumberWS floatNumberWS floatNumberWS 

    let internal singleNumber = numberLiteral(numberFormat) "number" |>> fun numberLiteral -> single numberLiteral.String
    let internal singleNumberWS = singleNumber .>> ws
    let internal singleNumber2 = tuple2 singleNumberWS singleNumberWS
    let internal singleNumber3 = tuple3 singleNumberWS singleNumberWS singleNumberWS

    let internal integer : Parser<int, unit> = numberLiteral(integerFormat) "integer" |>> fun numberLiteral -> int numberLiteral.String
    let internal integerWS = integer .>> ws
    
    let internal tag = keyword "tag" >>. integerWS

    let internal booleanState =
        let on = keyword "on" >>% true
        let off = keyword "off" >>% false
        on <|> off

    (* Command Parsers *)

    (* Vertex *)
    let internal vertex =
        let uvPrelight = 
            let uv = keyword "uv" >>. singleNumber2 |>> fun (u, v) -> (u, v)
            let prelight = keyword "prelight" >>. singleNumber3 |>> fun (r, g, b) -> (r, g, b)

            ((uv) .>>. (opt prelight) |>> fun(uv, prelight) -> (Some uv, prelight)) <|>
            ((prelight) .>>. (opt uv) |>> fun(prelight, uv) -> (uv, Some prelight))

        keywordExt "vertex" >>. floatNumber3 .>>. opt uvPrelight |>>
            fun ((x, y, z), (uvPrelight)) -> 
                match uvPrelight with
                | None -> Vertex((x, y, z), None, None)
                | Some (uv, prelight) -> Vertex((x , y , z), uv, prelight)

    (* Triangle *)
    let internal triangle = keywordExt "triangle" >>. (tuple3 integerWS integerWS integerWS) .>>. opt tag |>> Triangle

    (* Quad *)
    let internal quad = keywordExt "quad" >>. (tuple4 integerWS integerWS integerWS integerWS) .>>. opt tag |>> Quad

    (* Polygon *)
    let internal polygon = keywordExt "polygon" >>. (integerWS .>>. (many integerWS)) .>>. opt tag |>> Polygon

    (* Clump Settings *)
    let internal clumpTag = keyword "tag" >>. integerWS |>> Tag
    let internal collision = keyword "collision" >>. booleanState |>> Collision

    (* Block Commands *)
    let internal modelBegin = keyword "modelbegin" >>% ModelBegin
    let internal modelEnd = keyword "modelend" >>% ModelEnd
    let internal clumpBegin = keyword "clumpbegin" >>% ClumpBegin
    let internal clumpEnd = keyword "clumpend" >>% ClumpEnd
    let internal materialBegin = keyword "materialbegin" >>% MaterialBegin
    let internal materialEnd = keyword "materialend" >>% MaterialEnd
    let internal transformBegin = keyword "transformbegin" >>% TransformBegin
    let internal transformEnd = keyword "transformend" >>% TransformEnd
    let internal protoBegin = keyword "protobegin" >>. ws >>. identifierLiteral |>> ProtoBegin
    let internal protoEnd = keyword "protoend" >>% ProtoEnd
    let internal protoInstance = keyword "protoinstance" >>. ws >>. identifierLiteral |>> ProtoInstance
    let internal protoInstanceGeometry = keyword "protoinstancegeometry" >>. ws >>. identifierLiteral |>> ProtoInstanceGeometry

    (* Transform Commands *)
    let internal transform = keyword "transform" >>. tuple4 floatNumber4 floatNumber4 floatNumber4 floatNumber4 |>> Transform
    let internal translate = keyword "translate" >>. floatNumber3 |>> Translate
    let internal rotate = keyword "rotate" >>. floatNumber4 |>> Rotate
    let internal scale = keyword "scale" >>. floatNumber3 |>> Scale
    let internal identity = keyword "identity" >>% Identity

    (* Material Commands *)
    let internal color = keyword "color" >>. singleNumber3 |>> Color
    let internal opacity = keyword "opacity" >>. singleNumberWS |>> Opacity
    let internal texture = keywordExt "texture" >>. tuple3 (identifierLiteral .>> ws) (opt (attempt (ws >>. keyword "mask" >>. identifierLiteral .>> ws))) (opt (ws >>. keyword "bump" >>. identifierLiteral)) |>> Texture
    let internal surface = keyword "surface" >>. singleNumber3 |>> Surface
    let internal ambient = keyword "ambient" >>. singleNumberWS |>> Ambient
    let internal diffuse = keyword "diffuse" >>. singleNumberWS |>> Diffuse
    let internal specular = keyword "specular" >>. singleNumberWS |>> Specular

    let internal textureModeNull = keyword "null" >>% TextureModes.Null
    let internal textureModeLit = keyword "lit" >>% TextureModes.Lit
    let internal textureModeForeshorten = keyword "foreshorten" >>% TextureModes.Foreshorten
    let internal textureModeFilter = keyword "filter" >>% TextureModes.Filter
    let internal textureModeChoice = choice [ textureModeLit; textureModeForeshorten; textureModeFilter ] 
    let internal textureModes = keywordPlural "texturemode" >>. (many1 textureModeChoice |>> List.reduce (fun accumulator current -> accumulator ||| current) <|> textureModeNull) |>> TextureMode
    let internal addTextureMode = keyword "addtexturemode" >>. textureModeChoice |>> AddTextureMode
    let internal removeTextureMode = keyword "removetexturemode" >>. textureModeChoice |>> RemoveTextureMode

    let internal materialModeNull = keyword "null" >>% MaterialModes.Null
    let internal materialModeDouble = keyword "double" >>% MaterialModes.Double
    let internal materialMode = keywordPlural "materialmode" >>. (materialModeNull <|> materialModeDouble) |>> MaterialMode
    let internal addMaterialMode = keyword "addmaterialmode" >>. materialModeDouble |>> AddMaterialMode
    let internal removeMaterialMode = keyword "removematerialmode" >>. materialModeDouble |>> RemoveMaterialMode

    let internal geometrySampling = 
        let solid = keyword "solid" >>% GeometrySamplings.Solid
        let wireframe = keyword "wireframe" >>% GeometrySamplings.Wireframe
        let pointcloud = keyword "pointcloud" >>% GeometrySamplings.Pointcloud
    
        keyword "geometrysampling" >>. choice [ solid; wireframe; pointcloud ] |>> GeometrySampling

    let internal lightSampling =
        let facet = keyword "facet" >>% LightSamplings.Facet
        let vertex = keyword "vertex" >>% LightSamplings.Vertex

        keyword "lightsampling" >>. (facet <|> vertex) |>> LightSampling

    let internal textureAddressMode =
        let wrap = keyword "wrap" >>% TextureAddressModes.Wrap
        let mirror = keyword "mirror" >>% TextureAddressModes.Mirror
        let clamp = keyword "clamp" >>% TextureAddressModes.Clamp

        keyword "textureaddressmode" >>. choice [ wrap; mirror; clamp ] |>> TextureAddressMode

    let internal textureMipmapState =  keyword "texturemipmapstate" >>. booleanState |>> TextureMipmapState

    (* Object settings *)
    let randomUVs = keyword "randomuvs" >>. booleanState |>> RandomUVs
    let seamless = keyword "seamless" >>. booleanState |>> Seamless
    let opacityFix = keyword "opacityfix" >>. booleanState |>> OpacityFix

    let internal axisAlignment =
        let none = keyword "none" >>% AxisAlignments.None
        let zOrientX = keyword "zorientx" >>% AxisAlignments.ZOrientX
        let zOrientY = keyword "zorienty" >>% AxisAlignments.ZOrientY
        let xyz = keyword "xyz" >>% AxisAlignments.XYZ

        keyword "axisalignment" >>. choice [ none; zOrientX; zOrientY; xyz ] |>> AxisAlignment

    (* Primitives *)
    let internal block = keyword "block" >>. (tuple3 floatNumberWS floatNumberWS floatNumberWS) |>> Block
    let internal cone = keyword "cone" >>. (tuple3 floatNumberWS floatNumberWS integerWS) |>> Cone
    let internal cylinder = keyword "cylinder" >>. (tuple4 floatNumberWS floatNumberWS floatNumberWS integerWS) |>> Cylinder
    let internal disc = keyword "disc" >>. (tuple3 floatNumberWS floatNumberWS integerWS) |>> Disc
    let internal hemisphere = keyword "hemisphere" >>. floatNumberWS .>>. integerWS |>> Hemisphere
    let internal sphere = keyword "sphere" >>. floatNumberWS .>>. integerWS |>> Sphere

    (* Primary parsers *)
    let isIgnored (stream : CharStream<unit>) =
        let ignoredCommands = [ "addhint"; "hints"; "include"; "includegeometry"; "removehint"; "texturedithering"; "texturegammacorrection"; "trace"; "transformjoint"; "identityjoint"; "jointtransformbegin"; "jointtransformend"; "rotatejoint" ]
        ignoredCommands |> List.exists stream.MatchCaseFolded

    let internal ignoredCommand : Parser<Command, _> =
        fun stream ->
            if isIgnored stream then
                stream |> (restOfLine false |>> Ignored)
            else
                Reply(Error, ErrorMessageList(ErrorMessage.Unexpected("Unknown RWX command.")))

    let internal rwxCommand : Parser<Command, _> = 
        fun stream ->
            match System.Char.ToLower(stream.Peek()) with
            | 'a' -> choice [ ambient; addTextureMode; addMaterialMode; axisAlignment ] stream
            | 'b' -> block stream
            | 'c' -> choice [ clumpBegin; clumpEnd; color; collision; cone; cylinder ] stream
            | 'd' -> choice [ diffuse; disc ] stream
            | 'g' -> geometrySampling stream
            | 'h' -> hemisphere stream
            | 'i' -> identity stream
            | 'l' -> lightSampling stream
            | 'm' -> choice [ modelBegin; modelEnd; materialBegin; materialEnd; materialMode ] stream
            | 'o' -> choice [ opacityFix; opacity ] stream
            | 'p' -> choice [ polygon; protoBegin; protoEnd; protoInstanceGeometry; protoInstance; ] stream
            | 'q' -> quad stream
            | 'r' -> choice [ rotate; removeTextureMode; removeMaterialMode; randomUVs ] stream
            | 's' -> choice [ scale; surface; specular; sphere; seamless ] stream
            | 't' -> choice [ clumpTag; triangle; transformBegin; transformEnd; transform; translate; textureModes; textureAddressMode; textureMipmapState; texture; ] stream
            | 'v' -> vertex stream
            | _ -> Reply(Error, ErrorMessageList(ErrorMessage.Unexpected("Unknown RWX command.")))

    let internal parser = (many (attempt (spaces >>. (ignoredCommand <|> (rwxCommand .>> eol))))) .>> spaces .>> eof
                
    let parse input =
        match run parser input with
        | Success(result, _, _) -> result
        | Failure(errorMsg, _, _) -> raise (ParseException errorMsg)
