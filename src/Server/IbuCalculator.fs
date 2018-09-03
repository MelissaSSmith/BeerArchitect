module ServerCode.IbuCalculator

open Giraffe
open Microsoft.AspNetCore.Http
open System

open Shared

let functionOfTime time = (1.0 - Math.Pow(Math.E, -0.04 * time)) / 4.15

let functionOfGravity boilGravity = 1.65 * Math.Pow(0.000125, (boilGravity - 1.0))

let alphaAcidUnit ouncesOfHops aAPercentage = 
    ouncesOfHops * aAPercentage

let boilGravity massOfExtract poundPerGallon boilSize = 
    (massOfExtract * poundPerGallon) / boilSize

let estimatedBoilGravity targetOriginalGravity boilSize batchSize = 
    ((batchSize * (targetOriginalGravity % 1.0)) / boilSize) + 1.0

let hopUtilization time boilGravity = 
    functionOfGravity boilGravity * functionOfTime time

let ibusOzGal ouncesOfHops aAPercentage utilizationPercentage finalVolume = 
    ouncesOfHops * aAPercentage * utilizationPercentage * (75.0 / finalVolume)

let adjustForPelletHops hopType ibu =
    match hopType with
    | 2 -> ibu + (ibu * 0.10)
    | _ -> ibu


let calculateHopIbus hopIbuInputs hopUtilizations finalVolume = 
    List.zip hopIbuInputs hopUtilizations
    |> List.map (fun (h,u) -> ibusOzGal h.Ounces h.AlphaAcids u finalVolume)
    |> List.zip hopIbuInputs
    |> List.map (fun (h,i) -> adjustForPelletHops h.Type i)

let calculateHopUtilizaitons hopIbuInputs estimatedBoilGravity =
    hopIbuInputs
    |> List.map (fun h -> hopUtilization h.BoilTime estimatedBoilGravity)

let GetIbuResults boilGravity hopIbus hopUtilizations =
    {
        EstimatedBoilGravity = boilGravity
        TotalIbu = hopIbus |> List.sum 
        HopIbuResults = List.zip hopIbus hopUtilizations |> List.map (fun (i,u) -> { Utilization = u; Ibus = i})
    } : IbuResult

let calculate : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! ibuInput = ctx.BindJsonAsync<IbuInput>()

            let boilGravity = estimatedBoilGravity ibuInput.TargetOriginalGravity ibuInput.BoilSize ibuInput.BatchSize
            let hopUtilizations = calculateHopUtilizaitons ibuInput.HopIbuInputs boilGravity
            let hopIbuResults = calculateHopIbus ibuInput.HopIbuInputs hopUtilizations ibuInput.BatchSize
            let ibuResult = GetIbuResults boilGravity hopIbuResults hopUtilizations
            return! ctx.WriteJsonAsync ibuResult
        }