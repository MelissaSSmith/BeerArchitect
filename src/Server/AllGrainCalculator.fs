module ServerCode.AllGrainCalculator

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2

open Shared

let calculate : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let allGrainResult = { EstPreBoilOG = 1.0; EstOG = 1.0; EstFG = 1.0; EstABV = 1.0 }
            return! ctx.WriteJsonAsync allGrainResult
        }