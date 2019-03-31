module ServerCode.Hops

open Giraffe
open Microsoft.AspNetCore.Http
open Newtonsoft.Json
open System
open System.IO
open FSharp.Control.Tasks.V2
open Shared

[<Literal>]
let HopsAlphaAcidFile = "Data/HopsAlphaAcid.json"

[<Literal>]
let HopsFile = "Data/Hops.json"

let getAllHopsFromFile =
    HopsFile
    |> File.ReadAllText
    |> JsonConvert.DeserializeObject<Hop list>

let getAllHopAlphaAcidsFromFile =
    HopsAlphaAcidFile
    |> File.ReadAllText
    |> JsonConvert.DeserializeObject<HopAlphaAcid list>

let getHop : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let urlId = "admiral"
            let hop = getAllHopsFromFile
                      |> List.tryFind (fun h -> String.Equals(h.UrlId, urlId))

            return! ctx.WriteJsonAsync hop
        }

let getAllHops : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let hops = getAllHopsFromFile

            return! ctx.WriteJsonAsync hops
        }

let getAllHopAlphaAcids : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let hopAlphaAcidList = getAllHopAlphaAcidsFromFile

            return! ctx.WriteJsonAsync hopAlphaAcidList
        }