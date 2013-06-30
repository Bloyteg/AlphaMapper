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

(* Define useful data structures *)
type public Vector3 =
    { 
        X : float; 
        Y : float; 
        Z : float
    }

type public Vector6 =
    {
        X1 : float;
        X2 : float; 
        Y1 : float;
        Y2 : float;
        Z1 : float;
        Z2 : float
    }

(* Define shear related types *)
type public Shear = 
    {
        Type : ShearType;
        Shear : Vector6
    }

and public ShearType =
    | None = 0
    | Skew = 1
    | Shear = 2

(* Define material related types *)
type public Material =
    { 
        Texture: Texture option;
        Color: Color option;
        Opacity: float;
    }
    with
        static member Default
            with get() = { Texture = None; Color = None; Opacity = 1.0 }

and public Texture = 
    { 
        Texture : TextureType;
        Mask : TextureType option;
    }

and public TextureType =
    | File of string
    | Uri of System.Uri

and public Color =
    {
        ColorData : ColorData; 
        Tint : bool;
    }

and public ColorData =
    {
        R : byte;
        G : byte;
        B: byte;
    }

(* Define object property related types *)
type public ObjectProperties =
    {
        Translation : Vector3;
        Rotation : Vector3;
        Scale : Vector3;
        Shear : Shear option;
        Materials : Map<int option, Material>;
        Visible : bool;
    }
    with
        static member Default
            with get() = 
                 {
                     Translation = { X = 0.0; Y = 0.0; Z = 0.0 }
                     Rotation = { X = 0.0; Y = 0.0; Z = 0.0 }
                     Scale = { X = 1.0; Y = 1.0; Z = 1.0 }
                     Shear = None
                     Materials = Map.empty
                     Visible = true
                 }
    end
