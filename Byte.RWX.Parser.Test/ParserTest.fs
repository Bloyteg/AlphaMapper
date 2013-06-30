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

namespace Byte.RWX.Parser.Tests
open NUnit.Framework
open Byte.FSharp.TestHelper.NUnitHelpers
open Byte.RWX.Parser
open Byte.RWX.Parser.AbstractSyntaxTree

[<TestFixture>]
module ParserTest =  

    [<Test>]
    [<Category("Vertex")>]
    let ``vertex exists``() =
        match Parser.parse "vertex 0 0 0" with
        | [Vertex((0.0, 0.0, 0.0), None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Vertex")>]
    let ``vertex should have some uv``() =
        match Parser.parse "vertex 0 0 0 uv 0 0" with
        | [Vertex(_, Some (0.0f, 0.0f), None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Vertex")>]
    let ``vertex should have some prelight``() =
        match Parser.parse "vertex 0 0 0 prelight 0 0 0" with
        | [Vertex(_, None, Some(0.0f, 0.0f, 0.0f))] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Vertex")>]
    let ``vertex should have some uv and some prelight``() =
        match Parser.parse "vertex 0 0 0 uv 0 0 prelight 0 0 0" with
        | [Vertex(_, Some _, Some _)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Vertex")>]
    let ``vertex should have some prelight and some uv``() =
        match Parser.parse "vertex 0 0 0 prelight 0 0 0 uv 0 0" with
        | [Vertex(_, Some _, Some _)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Vertex")>]
    let ``vertexext exists as vertex``() =
        match Parser.parse "vertexext 0 0 0" with
        | [Vertex((0.0, 0.0, 0.0), None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Triangle")>]
    let ``triangle exists``() =
        match Parser.parse "triangle 1 2 3" with
        | [Triangle((1, 2, 3), None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Triangle")>]
    let ``triangle followed by tag should include tag in itself``() =
        match Parser.parse "triangle 1 2 3 tag 100" with
        | [Triangle((1, 2, 3), Some 100)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Triangle")>]
    let ``triangle with tag on newline should not include tag in itself``() =
        match Parser.parse "triangle 1 2 3\n tag 100" with
        | [Triangle((1, 2, 3), None); Tag(100)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Quad")>]
    let ``quad exists``() =
        match Parser.parse "quad 1 2 3 4" with
        | [Quad((1, 2, 3, 4), None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Quad")>]
    let ``quad followed by tag should include tag in itself``() =
        match Parser.parse "quad 1 2 3 4 tag 100" with
        | [Quad((1, 2, 3, 4), Some 100)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Quad")>]
    let ``quad with tag on newline should not include tag in itself``() =
        match Parser.parse "quad 1 2 3 4\n tag 100" with
        | [Quad((1, 2, 3, 4), None); Tag(100)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Polygon")>]
    let ``polygon exists``() =
        match Parser.parse "polygon 5 1 2 3 4 5" with
        | [Polygon((5, [1; 2; 3; 4; 5]), None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Polygon")>]
    let ``polygon followed by tag should include tag as a part of itself``() =
        match Parser.parse "polygon 5 1 2 3 4 5 tag 100" with
        | [Polygon((5, [1; 2; 3; 4; 5]), Some 100)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Polygon")>]
    let ``polygon followed by newline and tag should not include tag as part of itself``() =
        match Parser.parse "polygon 5 1 2 3 4 5\n tag 100" with
        | [Polygon((5, [1; 2; 3; 4; 5]), None); Tag(100)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``modelbegin exists``() =
        match Parser.parse "modelbegin" with
        | [ModelBegin] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``modelend exists``() =
        match Parser.parse "modelend" with
        | [ModelEnd] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``clumpbegin exists``() =
        match Parser.parse "clumpbegin" with
        | [ClumpBegin] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``clumpend exists``() =
        match Parser.parse "clumpend" with
        | [ClumpEnd] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``materialbegin exists``() =
        match Parser.parse "materialbegin" with
        | [MaterialBegin] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``materialend exists``() =
        match Parser.parse "materialend" with
        | [MaterialEnd] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``transformbegin exists``() =
        match Parser.parse "transformbegin" with
        | [TransformBegin] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``transformend exists``() =
        match Parser.parse "transformend" with
        | [TransformEnd] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``protobegin exists with name``() =
        match Parser.parse "protobegin 3some@proto_!.jorg" with
        | [ProtoBegin("3some@proto_!.jorg")] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``protoend exists``() =
        match Parser.parse "protoend" with
        | [ProtoEnd] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``prototype instance has name``() =
        match Parser.parse "protoinstance 12whirl_432" with
        | [ProtoInstance("12whirl_432")] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Blocks")>]
    let ``prototype geometry instance has name``() =
        match Parser.parse "protoinstancegeometry 12whirl_432" with
        | [ProtoInstanceGeometry("12whirl_432")] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Transforms")>]
    let ``transform has 16 elements`` () =
        match Parser.parse "transform 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15" with
        | [Transform((0.0, 1.0, 2.0, 3.0), (4.0, 5.0, 6.0, 7.0), (8.0, 9.0, 10.0, 11.0), (12.0, 13.0, 14.0, 15.0))] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``color has rgb values of 1, 1, 1`` () =
        match Parser.parse "color 1.0 1.0 1.0" with
        | [Color(1.0f, 1.0f, 1.0f)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``opacity has value of 0.5`` () =
        match Parser.parse "opacity 0.5" with
        | [Opacity(0.5f)] -> pass()
        | _ -> fail()    

    [<Test>]
    [<Category("Materials")>]
    let ``texture exists with no mask or bumpmap`` () =
        match Parser.parse "texture bytepants" with
        | [Texture("bytepants", None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texture exists with no mask or bumpmap, with comment in middle`` () =
        match Parser.parse "texture bytepants #mask bytepantsm" with
        | [Texture("bytepants", None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texture allows ext extension`` () =
        match Parser.parse "textureExt bytepants" with
        | [Texture("bytepants", None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texture exists with mask and no bumpmap`` () =
        match Parser.parse "texture bytepants mask bytepantsm" with
        | [Texture("bytepants", Some("bytepantsm"), None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texture with mask followed by tabs is valid`` () =
        match Parser.parse "texture rain3 mask rain3m	" with
        | [Texture("rain3", Some("rain3m"), None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texture exists with bumpmap and no mask`` () =
        match Parser.parse "texture bytepants bump bytebump" with
        | [Texture("bytepants", None, Some("bytebump"))] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texture exists with mask and bumpmap`` () =
        match Parser.parse "texture bytepants mask bytepantsm bump bytebump" with
        | [Texture("bytepants", Some("bytepantsm"), Some("bytebump"))] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``surface has ambient, diffuse, and specular`` () =
        match Parser.parse("surface .5 .3 .0") with
        | [Surface(0.5f, 0.3f, 0.0f)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``ambient has value`` () =
        match Parser.parse("ambient 0.5") with
        | [Ambient(0.5f)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``diffuse has value`` () =
        match Parser.parse("diffuse 0.5") with
        | [Diffuse(0.5f)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``specular has value`` () =
        match Parser.parse("specular 0.5") with
        | [Specular(0.5f)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texturemode has null value`` () =
        match Parser.parse("texturemode null") with
        | [TextureMode(TextureModes.Null)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texturemodes has null value`` () =
        match Parser.parse("texturemodes null") with
        | [TextureMode(TextureModes.Null)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texturemode has composite of three values`` () =
        match Parser.parse("texturemode lit foreshorten filter") with
        | [TextureMode(textureMode)] when textureMode = (TextureModes.Lit ||| TextureModes.Foreshorten ||| TextureModes.Filter) -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texturemode has composite of two values`` () =
        match Parser.parse("texturemode lit filter") with
        | [TextureMode(textureMode)] when textureMode = (TextureModes.Lit ||| TextureModes.Filter) -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texturemode has single value from two values of same type`` () =
        match Parser.parse("texturemode lit lit") with
        | [TextureMode(textureMode)] when textureMode = (TextureModes.Lit) -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texturemode has single value`` () =
        match Parser.parse("texturemode lit") with
        | [TextureMode(textureMode)] when textureMode = (TextureModes.Lit) -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``addtexturemode has value`` () =
        match Parser.parse("addtexturemode lit") with
        | [AddTextureMode(textureMode)] when textureMode = (TextureModes.Lit) -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``removetexturemode has value`` () =
        match Parser.parse("removetexturemode lit") with
        | [RemoveTextureMode(textureMode)] when textureMode = (TextureModes.Lit) -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``materialmode has value`` () =
        match Parser.parse("materialmode double") with
        | [MaterialMode(MaterialModes.Double)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``materialmodes has value`` () =
        match Parser.parse("materialmode double") with
        | [MaterialMode(MaterialModes.Double)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``materialmode is null`` () =
        match Parser.parse("materialmode null") with
        | [MaterialMode(MaterialModes.Null)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``addmaterialmode has value`` () =
        match Parser.parse("addmaterialmode double") with
        | [AddMaterialMode(MaterialModes.Double)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``removematerialmode has value`` () =
        match Parser.parse("removematerialmode double") with
        | [RemoveMaterialMode(MaterialModes.Double)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``geometrysampling is solid`` () =
        match Parser.parse("geometrysampling solid") with
        | [GeometrySampling(GeometrySamplings.Solid)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``geometrysampling is wireframe`` () =
        match Parser.parse("geometrysampling wireframe") with
        | [GeometrySampling(GeometrySamplings.Wireframe)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``geometrysampling is pointcloud`` () =
        match Parser.parse("geometrysampling pointcloud") with
        | [GeometrySampling(GeometrySamplings.Pointcloud)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``lightsampling is facet`` () =
        match Parser.parse("lightsampling facet") with
        | [LightSampling(LightSamplings.Facet)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``lightsampling is vertex`` () =
        match Parser.parse("lightsampling vertex") with
        | [LightSampling(LightSamplings.Vertex)] -> pass()
        | _ -> fail()
        
    [<Test>]
    [<Category("Materials")>]
    let ``textureaddressmode is wrap`` () =
        match Parser.parse("textureaddressmode wrap") with
        | [TextureAddressMode(TextureAddressModes.Wrap)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``textureaddressmode is mirror`` () =
        match Parser.parse("textureaddressmode mirror") with
        | [TextureAddressMode(TextureAddressModes.Mirror)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``textureaddressmode is clamp`` () =
        match Parser.parse("textureaddressmode clamp") with
        | [TextureAddressMode(TextureAddressModes.Clamp)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texturemipmapstate is on`` () =
        match Parser.parse("texturemipmapstate on") with
        | [TextureMipmapState(true)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Materials")>]
    let ``texturemipmapstate is off`` () =
        match Parser.parse("texturemipmapstate off") with
        | [TextureMipmapState(false)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Clump Options")>]
    let ``tag has value`` () =
        match Parser.parse("tag 4") with
        | [Tag(4)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Clump Options")>]
    let ``collision is on`` () =
        match Parser.parse("collision on") with
        | [Collision(true)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Clump Options")>]
    let ``collision is off`` () =
        match Parser.parse("collision off") with
        | [Collision(false)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Object Options")>]
    let ``randomuvs is on`` () =
        match Parser.parse("randomuvs on") with
        | [RandomUVs(true)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Object Options")>]
    let ``randomuvs is off`` () =
        match Parser.parse("randomuvs off") with
        | [RandomUVs(false)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Object Options")>]
    let ``seamless is on`` () =
        match Parser.parse("seamless on") with
        | [Seamless(true)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Object Options")>]
    let ``seamless is off`` () =
        match Parser.parse("seamless off") with
        | [Seamless(false)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Object Options")>]
    let ``opacityfix is on`` () =
        match Parser.parse("opacityfix on") with
        | [OpacityFix(true)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Object Options")>]
    let ``opacityfix is off`` () =
        match Parser.parse("opacityfix off") with
        | [OpacityFix(false)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Object Options")>]
    let ``axisalignment is none`` () =
        match Parser.parse("axisalignment none") with
        | [AxisAlignment(AxisAlignments.None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Object Options")>]
    let ``axisalignment is zorientx`` () =
        match Parser.parse("axisalignment zorientx") with
        | [AxisAlignment(AxisAlignments.ZOrientX)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Object Options")>]
    let ``axisalignment is zorienty`` () =
        match Parser.parse("axisalignment zorienty") with
        | [AxisAlignment(AxisAlignments.ZOrientY)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Object Options")>]
    let ``axisalignment is xyz`` () =
        match Parser.parse("axisalignment xyz") with
        | [AxisAlignment(AxisAlignments.XYZ)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Primitives")>]
    let ``block exists`` () =
        match Parser.parse("block 1 1 1") with
        | [Block(1.0, 1.0, 1.0)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Primitives")>]
    let ``cone exists`` () =
        match Parser.parse("cone 1 1 8") with
        | [Cone(1.0, 1.0, 8)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Primitives")>]
    let ``cylinder exists`` () =
        match Parser.parse("cylinder 1 1 1 8") with
        | [Cylinder(1.0, 1.0, 1.0, 8)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Primitives")>]
    let ``disc exists`` () =
        match Parser.parse("disc 1 1 8") with
        | [Disc(1.0, 1.0, 8)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Primitives")>]
    let ``sphere exists`` () =
        match Parser.parse("sphere 1 8") with
        | [Sphere(1.0, 8)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Primitives")>]
    let ``hemisphere exists`` () =
        match Parser.parse("hemisphere 1 8") with
        | [Hemisphere(1.0, 8)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Ignored")>]
    let ``addhint is ignored`` () =
        match Parser.parse("addhint editable") with
        | [Ignored("addhint editable")] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Ignored")>]
    let ``addhint is ignored after whitespace`` () =
        match Parser.parse("\n\naddhint editable") with
        | [Ignored("addhint editable")] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Ignored")>]
    let ``addhint is ignored, but vertex is not`` () =
        match Parser.parse("addhint editable\nvertex 0 0 0") with
        | [Ignored("addhint editable"); Vertex((0.0, 0.0, 0.0), None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("Ignored")>]
    let ``identityjoint is not parsed as identity`` () =
        match Parser.parse("identityjoint") with
        | [Ignored("identityjoint")] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("General")>]
    let ``line ending in comment does not skip next line`` () =
        match Parser.parse "vertex 0 0 0 #1\r\nvertex 1 1 1 #2\r\nvertex 2 2 2" with
        | [ Vertex((0.0, 0.0, 0.0), None, None); Vertex((1.0, 1.0, 1.0), None, None); Vertex((2.0, 2.0, 2.0), None, None) ] -> pass()
        | _ -> fail()
    [<Test>]
    [<Category("General")>]
    let ``preceeding newlines should be ignored``() = 
        match Parser.parse "\n\nvertex 0 0 0" with
        | [Vertex((0.0, 0.0, 0.0), None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("General")>]
    let ``trailing newlines should be ignored``() = 
        match Parser.parse "vertex 0 0 0\n\n" with
        | [Vertex((0.0, 0.0, 0.0), None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("General")>]
    let ``infix newlines should be ignored``() = 
        match Parser.parse "vertex 0 0 0\n\nvertex 0 0 0" with
        | [
              Vertex((0.0, 0.0, 0.0), None, None)
              Vertex((0.0, 0.0, 0.0), None, None)
          ] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("General")>]
    [<ExpectedException(typedefof<ParseException>)>]
    let ``invalid parse should throw ParseException``() = 
        do Parser.parse "vertex 0 0 0\ngroup" |> ignore
        (* If no exception is thrown, fail. *)
        fail()

    [<Test>]
    [<Category("General")>]
    let ``comment on second line should be ignored``() = 
        match Parser.parse "vertex 0 0 0\n#vertex 0 0 0" with
        | [Vertex((0.0, 0.0, 0.0), None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("General")>]
    let ``comment on first line should be ignored``() = 
        match Parser.parse "#vertex 0 0 0\nvertex 0 0 0" with
        | [Vertex((0.0, 0.0, 0.0), None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("General")>]
    let ``comment at end of line should be ignored``() = 
        match Parser.parse "vertex 0 0 0 #vertex 0 0 0" with
        | [Vertex((0.0, 0.0, 0.0), None, None)] -> pass()
        | _ -> fail()

    [<Test>]
    [<Category("General")>]
    let ``V3 comment block should be included in statement``() = 
        match Parser.parse "vertex 0 0 0 #!prelight 0 0 0" with
        | [Vertex((0.0, 0.0, 0.0), None, Some (0.0f, 0.0f, 0.0f))] -> pass()
        | _ -> fail()
