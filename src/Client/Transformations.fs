module Client.Transformations

open Fable.Core
open System

[<Emit("isNaN(parseFloat($0)) ? null : parseFloat($0)  ")>]
let ParseFloat (e : obj) : float option = jsNative

[<Emit("isNaN(parseInt($0)) ? null : parseInt($0)  ")>]
let ParseInt (e : obj) : int option = jsNative

let RoundResult (input : float) (precision: int) = 
    Math.Round(input, precision)

let RoundToPrecisonTwo x =
    RoundResult x 2

let RoundToPrecisonThree x =
    RoundResult x 3

let TransformToString r = r |> string

let TransformToFloat e = 
    match ParseFloat e with
    | Some value -> value |> float
    | None -> 0.0

let TransformToInt e : int = 
    match ParseFloat e with
    | Some value -> value |> int
    | None -> 0