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
module AbstractSyntaxTree =
    type ParseException(message : string) = inherit System.Exception(message)

    [<System.Flags>]
    type TextureModes =
        | Null = 0
        | Lit = 1
        | Foreshorten = 2
        | Filter = 4

    type MaterialModes =
        | Null = 0
        | Double = 1

    type GeometrySamplings =
        | Solid = 0
        | Wireframe = 1
        | Pointcloud = 2

    type LightSamplings =
        | Facet = 0
        | Vertex = 1

    type TextureAddressModes =
        | Wrap = 0
        | Mirror = 1
        | Clamp = 2

    type AxisAlignments = 
        | None = 0
        | ZOrientX = 1
        | ZOrientY = 2
        | XYZ = 3

    type Command =
        (* Geometry *)
        | Vertex of (float * float * float) * (single * single) option * (single * single * single) option
        | Triangle of (int * int * int) * int option
        | Quad of (int * int * int * int) * int option
        | Polygon of (int * int list) * int option
        (* Blocks *)
        | Tag of int
        | ModelBegin
        | ModelEnd
        | ClumpBegin
        | ClumpEnd
        | TransformBegin
        | TransformEnd
        | MaterialBegin
        | MaterialEnd
        | ProtoBegin of string
        | ProtoEnd
        | ProtoInstance of string
        | ProtoInstanceGeometry of string
        (* Transforms *)
        | Transform of ((float * float * float * float) * (float * float * float * float) * (float * float * float * float) * (float * float * float * float))
        | Translate of (float * float * float)
        | Rotate of (float * float * float * float)
        | Scale of (float * float * float)
        | Identity
        (* Materials *)
        | Color of (single * single * single)
        | Opacity of single
        | Texture of (string * string option * string option)
        | Surface of (single * single * single)
        | Ambient of single
        | Diffuse of single
        | Specular of single
        | TextureMode of TextureModes
        | AddTextureMode of TextureModes
        | RemoveTextureMode of TextureModes
        | MaterialMode of MaterialModes
        | AddMaterialMode of MaterialModes
        | RemoveMaterialMode of MaterialModes
        | GeometrySampling of GeometrySamplings
        | LightSampling of LightSamplings
        | TextureAddressMode of TextureAddressModes
        | TextureMipmapState of bool
        (* Object Settings *)
        | RandomUVs of bool
        | Seamless of bool
        | OpacityFix of bool
        | AxisAlignment of AxisAlignments
        (* Clump Options *)
        | Collision of bool
        (* Primitives *)
        | Block of (float * float * float)
        | Cone of (float * float * int)
        | Cylinder of (float * float  * float * int)
        | Disc of (float * float * int)
        | Hemisphere of (float * int)
        | Sphere of (float * int)
        (* Ignored Commands *)
        | Ignored of string
