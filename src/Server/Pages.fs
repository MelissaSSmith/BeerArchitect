module ServerCode.Pages

open Giraffe
open FSharp.Control.Tasks.V2
open Client.Pages
open Client.Shared

let home : HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel = PageModel.HomePageModel;
            CurrentPage = Client.Pages.Page.Home
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let abvCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m = Client.AbvCalculator.init None
                PageModel.AbvCalculatorPageModal m;
            CurrentPage = Client.Pages.Page.AbvCalculator
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let srmCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m,_ = Client.SrmCalculator.init None
                PageModel.SrmCalculatorPageModel m;
            CurrentPage = Client.Pages.Page.SrmCalculator
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let ibuCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m,_ = Client.IbuCalculator.init None
                PageModel.IbuCalculatorPageModel m;
            CurrentPage = Client.Pages.Page.IbuCalculator
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let hydrometerCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m,_ = Client.HydrometerTempCalculator.init None
                PageModel.HydrometerTempCalculatorPageModel m;
            CurrentPage = Client.Pages.Page.HydrometerTempCalculator
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let allGrainCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m,_ = Client.AllGrainCalculator.init None
                PageModel.AllGrainCalculatorPageModel m;
            CurrentPage = Client.Pages.Page.AllGrainCalculator
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let yeastProfiles: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel = 
                let m,_ = Client.YeastProfiles.init None
                PageModel.YeastProfilesPageModel m;
            CurrentPage = Client.Pages.Page.YeastProfiles
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let dilutionBoilOffCalculator: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel =
                let m,_ = Client.DilutionBoilOffCalculator.init None
                PageModel.DilutionBoilOffCalculatorPageModel m;
            CurrentPage = Client.Pages.Page.DilutionBoilOffCalculator
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let fermentableProfiles: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel = 
                let m,_ = Client.FermentableProfiles.init None
                PageModel.FermentableProfilesPageModel m;
            CurrentPage = Client.Pages.Page.FermentableProfiles
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let hopProfiles: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel = 
                let m,_ = Client.HopProfiles.init
                PageModel.HopProfilesPageModel m;
            CurrentPage = Client.Pages.Page.HopProfiles
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let hopProfile s: HttpHandler = fun _ ctx ->
    task {
        let model: Model = {
            PageModel = 
                let m,_ = Client.HopProfile.init s
                PageModel.HopProfilePageModel m;
            CurrentPage = Client.Pages.Page.HopProfile "admiral"
        }
        return! ctx.WriteHtmlViewAsync (Templates.index (Some model))
    }

let notfound: HttpHandler = fun _ ctx ->
    ctx.WriteHtmlViewAsync (Templates.index None)