module ServerCode.Yeast

open Giraffe
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open System.IO
open FSharp.Control.Tasks.V2
open Shared

[<Literal>]
let YeastFile = "Data/Yeast.json"

let getAllYeastFromFile =
    YeastFile
    |> File.ReadAllText
    |> JsonConvert.DeserializeObject<Yeast list>

let getAllYeast : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let yeastList = getAllYeastFromFile

            return! ctx.WriteJsonAsync yeastList
        }