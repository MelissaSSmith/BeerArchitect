module ServerCode.Hops

open Giraffe
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open System.IO
open Shared

[<Literal>]
let HopsAlphaAcidFile = "Data/HopsAlphaAcid.json"

let getAllHopAlphaAcidsFromFile =
    HopsAlphaAcidFile
    |> File.ReadAllText
    |> JsonConvert.DeserializeObject<HopAlphaAcid list>

let getAllHopAlphaAcids : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let hopAlphaAcidList = getAllHopAlphaAcidsFromFile

            return! ctx.WriteJsonAsync hopAlphaAcidList
        }