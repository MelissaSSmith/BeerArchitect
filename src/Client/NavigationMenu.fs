module Client.NavigationMenu

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
open Client.Pages
open Client.Style

let navigationBar = 
    nav [ClassName  "navbar navbar-dark fixed-top bg-dark flex-md-nowrap p-0 shadow"] [
        div [ClassName "navbar-brand col-sm-3 col-md-2 mr-0 logo"] [ str ("Beer Architect") ]  
        div [ClassName "navbar-nav"] []]

let sidebarNavigationMenu =
    nav [ClassName "col-md-2 d-none d-md-block sidebar"] [
        div [ClassName "navigation-menu"] [
            ul [ClassName "nav flex-column nav-pills"] [
                yield viewLink Page.Home "Home"
                yield viewLink Page.AbvCalculator "ABV Calculator"
            ]
        ]
    ]
    