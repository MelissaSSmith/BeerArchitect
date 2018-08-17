module ServerCode.WebServer

open ServerCode
open ServerCode.ServerUrls
open Giraffe
open Giraffe.TokenRouter
open RequestErrors
open Microsoft.AspNetCore.Http

let webApp : HttpHandler =
    let apiPathPrefix = PathString("/api")
    let notfound: HttpHandler =
        fun next ctx ->
            if ctx.Request.Path.StartsWithSegments(apiPathPrefix) then
                NOT_FOUND "Page not found" next ctx
            else
                Pages.notfound next ctx

    router notfound [
        GET [
            route PageUrls.Home Pages.home
            route APIUrls.GetFermentables Fermentables.getAllFermentables
        ]
        POST [
            route APIUrls.CalculateAbv AbvCalculator.calculate
        ]
    ]