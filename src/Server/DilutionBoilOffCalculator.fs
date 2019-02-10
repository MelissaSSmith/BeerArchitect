module ServerCode.DilutionBoilOffCalculator

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2

open ServerCode.GravityEquations
open Shared
open System

let checkAnswer answer =
    match answer with
    | f when f = nan -> 0.0
    | _ -> answer

let getNewVolume currentGrav desiredGrav volume = 
    match volume with 
    | 0.0 -> 0.0
    | _ ->
    let curGravPts = getGravPoints currentGrav
    let desiredGravPts = getGravPoints desiredGrav
    (curGravPts * volume) / desiredGravPts

let getNewGravity currentVol currentGrav targetVol = 
    match targetVol with 
    | 0.0 -> 0.0
    | _ ->
    let curGravPts = getGravPoints currentGrav
    (curGravPts * currentVol) / targetVol
    |> getSpecificGravity

let GetDilutionBoilOffResults inputs =
    let newVolume = getNewVolume inputs.NewVolCurrGrav inputs.DesiredGravity inputs.NewVolWortVol
    let newGravity = getNewGravity inputs.NewGravWortVol inputs.NewGravCurrGrav inputs.TargetVolume
    {
        NewVolume = newVolume
        NewVolumeDiff = abs (newVolume - inputs.NewVolWortVol)
        NewGravity = newGravity
        NewGravityDiff = abs (newGravity - inputs.NewGravCurrGrav)
    } : DilutionBoilOffResult

let calculateNewVolume : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! inputs = ctx.BindJsonAsync<DilutionBoilOffInput>()

            let dilutionBoilOffResults = GetDilutionBoilOffResults inputs
            return! ctx.WriteJsonAsync dilutionBoilOffResults
        }

let calculateNewGravity : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! inputs = ctx.BindJsonAsync<DilutionBoilOffInput>()

            let dilutionBoilOffResults = GetDilutionBoilOffResults inputs
            return! ctx.WriteJsonAsync dilutionBoilOffResults
        }