module SrmCalculator

open Giraffe
open Microsoft.AspNetCore.Http
open System
open System.IO
open FSharp.Control.Tasks.V2

open ServerCode.Fermentables
open Shared
open Newtonsoft.Json

[<Literal>]
let SrmHexFile = "Data/SrmHex.json"

let GetAllSrmHexValues =
    SrmHexFile
    |> File.ReadAllText
    |> JsonConvert.DeserializeObject<SrmHex list>

let Power power b =
    Math.Pow(b, power)

let MultiplyByConstant variable constant = 
    variable * constant

let Mcu grainColor grainWeightLbs volGal  =
    (grainColor * grainWeightLbs) / volGal

let SrmColor grainColorList grainWeightLbsList volGal =
    List.zip grainWeightLbsList grainColorList
    |> List.sumBy (fun (a, dl) -> Mcu dl a volGal)
    |> Power 0.6859
    |> MultiplyByConstant 1.4922

let Ebc srm = 
    MultiplyByConstant srm 1.97

let getDegreesLovibondForFermentable id fermentableList =
    match id with
    | 0 -> 0.0
    | _ -> (Seq.find(fun f -> f.Id = id) fermentableList).DegreesLovibond

let getSrmHexValue srm =
    let hex = Seq.find(fun f -> f.SrmKey = (int srm)) GetAllSrmHexValues
    hex.HexValue

let GetSrmResults srm ebc hexColor = 
    {
        Srm = srm
        Ebc = ebc
        HexColor = hexColor
    } : SrmResult

let calculate : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task {
            let! srmInput = ctx.BindJsonAsync<SrmInput>()

            let fermentableList = getAllFermentablesFromFile

            let grainColorList = List.init srmInput.GrainIds.Length (fun i -> getDegreesLovibondForFermentable (srmInput.GrainIds |> List.item i) fermentableList)

            let srm = SrmColor grainColorList srmInput.GrainAmounts srmInput.BatchSize
            let ebc = Ebc srm
            let hexColor = getSrmHexValue srm
            let srmResult = GetSrmResults srm ebc hexColor
            return! ctx.WriteJsonAsync srmResult
        }