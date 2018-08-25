module SrmCalculator

open Giraffe
open Microsoft.AspNetCore.Http
open System

open Shared

let Power power b =
    Math.Pow(b, power)

let MultiplyByConstant variable constant = 
    variable * constant

let Mcu grainColor grainWeightLbs volGal  =
    (grainColor * grainWeightLbs) / volGal

let SrmColor grainColorList (grainWeightLbsList:float list) volGal =
    List.zip grainWeightLbsList grainColorList
    |> List.sumBy (fun (a, dl) -> Mcu dl a volGal)
    |> Power 0.6859
    |> MultiplyByConstant 1.4922

let Ebc srm = 
    MultiplyByConstant srm 1.97

let GetSrmResults srm ebc = 
    {
        Srm = srm
        Ebc = ebc
        HexColor = "#232323"
    } : SrmResult

let calculate : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task {
            let! srmInput = ctx.BindJsonAsync<SrmInput>()

            let srm = SrmColor srmInput.GrainAmounts srmInput.GrainAmounts srmInput.BatchSize
            let ebc = srm
            let srmResult = GetSrmResults srm ebc
            return! ctx.WriteJsonAsync srmResult
        }