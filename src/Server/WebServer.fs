module ServerCode.WebServer

open ServerCode
open ServerCode.ServerUrls
open Giraffe
open Giraffe.TokenRouter
open RequestErrors
open Client

let webApp : HttpHandler =
    let notfound: HttpHandler =
        fun next ctx ->
            NOT_FOUND "Page not found" next ctx

    router notfound [
        GET [
            route PageUrls.Home Pages.home
            route PageUrls.AbvCalculator Pages.abvCalculator
            route PageUrls.SrmCalculator Pages.srmCalculator
            route PageUrls.HydrometerCalculator Pages.hydrometerCalculator
            route PageUrls.AllGrainCalculator Pages.allGrainCalculator
            route APIUrls.GetFermentables Fermentables.getAllFermentables
            route APIUrls.GetHopAlphaAcids Hops.getAllHopAlphaAcids
        ]
        POST [
            route APIUrls.CalculateAbv AbvCalculator.calculate
            route APIUrls.CalculateSrm SrmCalculator.calculate
            route APIUrls.CalculateIbu IbuCalculator.calculate
            route APIUrls.CalculateHydrometerAdjustment HydrometerTempCalculator.calculate
            route APIUrls.CalculateAllGrainEstimations AllGrainCalculator.calculate
        ]
    ]