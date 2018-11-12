module ServerCode.Pages

open Giraffe
open FSharp.Control.Tasks.V2
open Client.Shared

let home: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel = PageModel.HomePageModel
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let notfound: HttpHandler = fun _ ctx ->
    ctx.WriteHtmlViewAsync (Templates.index None)