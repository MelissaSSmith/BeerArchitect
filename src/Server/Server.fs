open System.IO
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Giraffe.TokenRouter
open Saturn
open RequestErrors
open Microsoft.AspNetCore.Http

open Giraffe.Serialization

open Server
open Server.ServerUrls

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let webApp =
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
        ]
    ]

let configureSerialization (services:IServiceCollection) =
    let fableJsonSettings = Newtonsoft.Json.JsonSerializerSettings()
    fableJsonSettings.Converters.Add(Fable.JsonConverter())
    services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer fableJsonSettings)

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    router webApp
    memory_cache
    use_static publicPath
    service_config configureSerialization
    use_gzip
}

run app
