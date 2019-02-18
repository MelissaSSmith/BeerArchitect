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

let abvCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m = Client.AbvCalculator.init None
                PageModel.AbvCalculatorPageModal m
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let srmCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m,_ = Client.SrmCalculator.init None
                PageModel.SrmCalculatorPageModel m
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let ibuCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m,_ = Client.IbuCalculator.init None
                PageModel.IbuCalculatorPageModel m
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let hydrometerCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m,_ = Client.HydrometerTempCalculator.init None
                PageModel.HydrometerTempCalculatorPageModel m
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let allGrainCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m,_ = Client.AllGrainCalculator.init None
                PageModel.AllGrainCalculatorPageModel m
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let yeastProfiles: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel = 
                let m,_ = Client.YeastProfiles.init None
                PageModel.YeastProfilesPageModel m
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let dilutionBoilOffCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m,_ = Client.DilutionBoilOffCalculator.init None
                PageModel.DilutionBoilOffCalculatorPageModel m
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let fermentableProfiles: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel = 
                let m,_ = Client.FermentableProfiles.init None
                PageModel.FermentableProfilesPageModel m
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let hopProfiles: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel = 
                let m,_ = Client.HopProfiles.init None
                PageModel.HopProfilesPageModel m
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let notfound: HttpHandler = fun _ ctx ->
    ctx.WriteHtmlViewAsync (Templates.index None)