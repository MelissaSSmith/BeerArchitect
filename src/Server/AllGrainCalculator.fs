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
        0.66
    | YeastTolerance.Medium ->
        0.72
    | YeastTolerance.High ->
        0.77

let specificGravity gravUnits =
    gravUnits / 1000.0 + 1.0 

let gravityPerLbs mashEff potentialExtract =
    mashEff * potentialExtract

let getPpgForFermentable id fermentableList =
    match id with 
    | 0 -> 0.0
    | _ -> (Seq.find(fun f -> f.Id = id) fermentableList).Ppg

let ingredientGravityPoints lbsNeeded gravPerLbs =
    lbsNeeded * gravPerLbs

let getTotalGravPoints ppgList quantityList =
    List.zip quantityList ppgList
    |> List.sumBy (fun (lbs, g) -> ingredientGravityPoints lbs g) 

let estimatedGravity efficiency gravPoints =
    (efficiency/100.0) * gravPoints

let attenuationCalc attenuation gravPoints =
    (1.0  - attenuation) * gravPoints 

let endGravPoint wortSize totalGravPoints =
    totalGravPoints / wortSize

let estimatedPreBoilGravity allGrainInput totalGravPoints =
    estimatedGravity allGrainInput.Effciency totalGravPoints
    |> endGravPoint allGrainInput.PreBoilSize
    |> specificGravity

let estimatedOriginalGravity allGrainInput totalGravPoints =
    estimatedGravity allGrainInput.Effciency totalGravPoints
    |> endGravPoint allGrainInput.BatchSize
    |> specificGravity

let estimatedFinalGravity totalGravPoints attenuationPercent allGrainInput =
    estimatedGravity allGrainInput.Effciency totalGravPoints
    |> attenuationCalc attenuationPercent
    |> endGravPoint allGrainInput.BatchSize
    |> specificGravity

let calculate : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! allGrainInput = ctx.BindJsonAsync<AllGrainInput>()

            let fermentableList = getAllFermentablesFromFile
            let ppgList = List.init allGrainInput.GrainIds.Length (fun i -> getPpgForFermentable (allGrainInput.GrainIds |> List.item i) fermentableList)
            let totalGravPoints = getTotalGravPoints ppgList allGrainInput.GrainAmounts
            let attenuationPercent = getAttenuation allGrainInput.YeastTolerance

            let preOg = estimatedPreBoilGravity allGrainInput totalGravPoints
            let og = estimatedOriginalGravity allGrainInput totalGravPoints
            let fg = estimatedFinalGravity totalGravPoints attenuationPercent allGrainInput
            let abv = Abv og fg
            let allGrainResult = { EstPreBoilOG = preOg; EstOG = og; EstFG = fg; EstABV = abv }
            return! ctx.WriteJsonAsync allGrainResult
        }