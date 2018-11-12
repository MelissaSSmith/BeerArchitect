module ServerCode.Fermentables

open Giraffe
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open System.IO
open FSharp.Control.Tasks.V2
open Shared

[<Literal>]
let FermentableFile = "Data/Fermentables.json"

let getAllFermentablesFromFile =
    FermentableFile
    |> File.ReadAllText
    |> JsonConvert.DeserializeObject<Fermentable list>

let getAllFermentables : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let fermentableList = getAllFermentablesFromFile

            return! ctx.WriteJsonAsync fermentableList
        }