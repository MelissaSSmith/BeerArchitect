module ServerCode.DilutionBoilOffCalculator

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2

open Shared


let GetDilutionBoilOffResults inputs =
    {
        NewVolume = 0.0
        NewVolumeDiff = 0.0
        NewGravity = 0.0
        NewGravityDiff = 0.0
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