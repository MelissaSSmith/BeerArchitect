module ServerCode.WebServer

open ServerCode
open ServerCode.ServerUrls
open Giraffe
open Giraffe.TokenRouter
open RequestErrors

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
            route PageUrls.DilutionBoilOffCalculator Pages.dilutionBoilOffCalculator
            route PageUrls.YeastProfiles Pages.yeastProfiles
            route PageUrls.FermentableProfiles Pages.fermentableProfiles
            route APIUrls.GetFermentables Fermentables.getAllFermentables
            route APIUrls.GetHopAlphaAcids Hops.getAllHopAlphaAcids
            route APIUrls.GetYeasts Yeast.getAllYeast
            route APIUrls.GetHops Hops.getAllHops
        ]
        POST [
            route APIUrls.CalculateAbv AbvCalculator.calculate
            route APIUrls.CalculateSrm SrmCalculator.calculate
            route APIUrls.CalculateIbu IbuCalculator.calculate
            route APIUrls.CalculateHydrometerAdjustment HydrometerTempCalculator.calculate
            route APIUrls.CalculateAllGrainEstimations AllGrainCalculator.calculate
            route APIUrls.CalculateDilutionVolume DilutionBoilOffCalculator.calculateNewVolume
            route APIUrls.CalculateDilutionGravity DilutionBoilOffCalculator.calculateNewGravity
        ]
    ]