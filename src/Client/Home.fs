module Client.Home

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.Import
open Elmish.Browser.Navigation
module R = Fable.Helpers.React
open Client.Pages

let goToUrl (e: React.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

let viewLink page description =
  R.a [ ClassName "nav-item nav-link"
        Href (Pages.toPath page)
        OnClick goToUrl]
      [ R.str description]

let view () =
    nav [ClassName  "navbar navbar-expand-lg navbar-dark bg-dark"] [
        div [ClassName "navbar-brand"] [ str ("Beer Architect") ]  
        div [ClassName "navbar-nav"] [
            yield viewLink Page.Home "Home"
            yield viewLink Page.AbvCalculator "ABV Calculator"
        ]]