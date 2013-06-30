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

namespace Byte.RWX
open Byte.RWX.Parser
open Byte.RWX.Parser.AbstractSyntaxTree
open Byte.IntermediateModel.Builder
open Byte.Math
open Byte.FSharp.Helpers.Null

module internal Helpers =
    let toMatrix matrixTuple =
        let ((m00, m01, m02, m03), (m10, m11, m12, m13), (m20, m21, m22, m23), (m30, m31, m32, m33)) = matrixTuple
        let elementArray = [| m00; m01; m02; m03; m10; m11; m12; m13; m20; m21; m22; m23; m30; m31; m32; m33; |];
        let matrix = new Matrix4()
        
        for row = 0 to 3 do
            for column = 0 to 3 do
                matrix.[row, column] <- elementArray.[4 * row + column]

        matrix

    let inline convertEnum enumValue = enum<'a>(LanguagePrimitives.EnumToValue(enumValue))

    let inline scaleUp (x, y, z) = (x * 10.0, y * 10.0, z * 10.0)

open Helpers

type Loader() =
    let modelBuilder = new ModelBuilder()
    let rec handleCommands =
        function
            | [] -> modelBuilder.ToModel()
            | (command::commands) -> 
                match command with
                //Blocks
                | ClumpBegin -> modelBuilder.BeginClump ()
                | ClumpEnd -> modelBuilder.EndClump ()
                | TransformBegin -> modelBuilder.BeginTransform ()
                | TransformEnd -> modelBuilder.EndTransform ()
                | MaterialBegin -> modelBuilder.BeginMaterial ()
                | MaterialEnd -> modelBuilder.EndMaterial ()
                | ProtoBegin(name) -> modelBuilder.BeginPrototype name
                | ProtoEnd -> modelBuilder.EndPrototype ()
                | ProtoInstance(name) -> modelBuilder.AddProtoInstance name
                | ProtoInstanceGeometry(name) -> modelBuilder.AddProtoInstanceGeometry name
                //Geometry
                | Vertex(position, uv, prelight) -> modelBuilder.AddVertex (position (*|> scaleUp*), uv |> toNull, prelight |> toNull)
                | Triangle((i1, i2, i3), tag) -> modelBuilder.AddTriangle (i1 - 1, i2 - 1, i3 - 1, tag |> toNullable)
                | Quad((i1, i2, i3, i4), tag) -> modelBuilder.AddQuad (i1 - 1, i2 - 1, i3 - 1, i4 - 1, tag |> toNullable)
                | Polygon((indexCount, indices), tag) -> modelBuilder.AddPolygon (indexCount, indices |> Seq.map (fun index -> index - 1), tag |> toNullable)
                //Transforms
                | Transform(matrixTuple) -> matrixTuple |> toMatrix |> modelBuilder.SetTransformMatrix
                | Translate(translateTuple) -> modelBuilder.AddTranslate translateTuple
                | Rotate(rotateTuple) -> modelBuilder.AddRotate rotateTuple
                | Scale(scaleTuple) -> modelBuilder.AddScale scaleTuple
                | Identity -> modelBuilder.SetIdentityTransform ()
                //Materials
                | Color(color) -> modelBuilder.SetColor color
                | Opacity(opacity) -> modelBuilder.SetOpacity opacity
                | Texture(texture, mask, bumpmap) -> modelBuilder.SetTexture (texture, mask |> toNull, bumpmap |> toNull)
                | Surface(values) -> modelBuilder.SetSurface values 
                | Ambient(value) -> modelBuilder.SetAmbient value
                | Diffuse(value) -> modelBuilder.SetDiffuse value
                | Specular(value) -> modelBuilder.SetSpecular value
                | TextureMode(value) -> value |> convertEnum |> modelBuilder.SetTextureMode
                | AddTextureMode(value) -> value |> convertEnum |> modelBuilder.AddTextureMode
                | RemoveTextureMode(value) -> value |> convertEnum |> modelBuilder.RemoveTextureMode
                | MaterialMode(value) -> value |> convertEnum |> modelBuilder.SetMaterialMode
                | AddMaterialMode(value) -> value |> convertEnum |> modelBuilder.AddMaterialMode
                | RemoveMaterialMode(value) -> value |> convertEnum |> modelBuilder.RemoveMaterialMode
                | GeometrySampling(value) -> value |> convertEnum |> modelBuilder.SetGeometrySampling
                | LightSampling(value) -> value |> convertEnum |> modelBuilder.SetLightSampling
                | TextureAddressMode(value) -> value |> convertEnum |> modelBuilder.SetTextureAddressMode 
                | TextureMipmapState(value) -> modelBuilder.SetTextureMipmapState value
                //Clump options
                | Tag(value) -> modelBuilder.SetClumpTag value
                | Collision(value) -> modelBuilder.SetCollision value 
                //Model options
                | RandomUVs(value) -> modelBuilder.SetRandomUVs value
                | Seamless(value) -> modelBuilder.SetSeamless value
                | OpacityFix(value) -> modelBuilder.SetOpacityFix value
                | AxisAlignment(value) -> value |> convertEnum |> modelBuilder.SetAxisAlignment
                //Primtives
                | Block(value) -> modelBuilder.AddBlock value
                | Cone(value) -> modelBuilder.AddCone value
                | Cylinder(value) -> modelBuilder.AddCylinder value
                | Disc(value) -> modelBuilder.AddDisc value
                | Hemisphere(value) -> modelBuilder.AddHemisphere value
                | Sphere(value) -> modelBuilder.AddSphere value
                | _ -> ()

                handleCommands commands

    member public this.LoadFromString (input : string) =
        do modelBuilder.Reset()
        input |> Parser.parse |> handleCommands

    member public this.LoadFromFile (fileName : string) =
        do modelBuilder.Reset()
        fileName |> System.IO.File.ReadAllText |> Parser.parse |> handleCommands

    member public this.LoadFromStream (stream : System.IO.Stream) =
        do modelBuilder.Reset()
        use streamReader = new System.IO.StreamReader(stream)
        streamReader.ReadToEnd () |> Parser.parse |> handleCommands
