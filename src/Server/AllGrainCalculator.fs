module ServerCode.AllGrainCalculator

open Giraffe
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2

open ServerCode.AbvCalculator
open ServerCode.Fermentables
open Shared

let getAttenuation yeastTolerance =
    match yeastTolerance with
    | YeastTolerance.Low ->
        66.0
    | YeastTolerance.Medium ->
        72.0
    | YeastTolerance.High ->
        77.0

let specificGravity gravUnits =
    gravUnits / 1000.0 + 1.0 

let gravityPerLbs mashEff potentialExtract =
    mashEff * potentialExtract
    
let ingredientGravity lbsNeeded gravPerLbs =
    lbsNeeded * gravPerLbs

let estimatedOriginalGravity targetOg boilSize batchSize =
    (targetOg * boilSize) / batchSize

let estimatedFinalGravity totalGravPoints attenuationPercent =
    (1.0  - attenuationPercent) * totalGravPoints
    |> specificGravity

let calculate : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! allGrainInput = ctx.BindJsonAsync<AllGrainInput>()

            let fermentableList = getAllFermentablesFromFile
            let attenuationPercent = getAttenuation allGrainInput.YeastTolerance

            let og = estimatedOriginalGravity 0.0 allGrainInput.PreBoilSize allGrainInput.BatchSize
            let fg = estimatedFinalGravity 0.0 attenuationPercent
            let abv = Abv og fg
            let allGrainResult = { EstPreBoilOG = 1.0; EstOG = og; EstFG = fg; EstABV = abv }
            return! ctx.WriteJsonAsync allGrainResult
        }