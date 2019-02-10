module ServerCode.DilutionBoilOffCalculator

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2

open Shared

let getGravPoints specificGrav = 
    (specificGrav - 1.0) * 1000.0

let getSpecificGravity gravPoints =
    (gravPoints / 1000.0) + 1.0

let getNewVolume currentGrav desiredGrav volume = 
    let curGravPts = getGravPoints currentGrav
    let desiredGravPts = getGravPoints desiredGrav
    (curGravPts * volume) / desiredGravPts

let getNewGravity currentVol currentGrav targetVol = 
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