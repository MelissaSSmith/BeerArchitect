module Client.Home

open Fable.Helpers.React
open Fable.Helpers.React.Props
module R = Fable.Helpers.React
open Client.Pages
open Client.Style

let view () =
    nav [ClassName  "navbar navbar-expand-lg navbar-dark bg-dark"] [
        div [ClassName "navbar-brand"] [ str ("Beer Architect") ]  
        div [ClassName "navbar-nav"] [
            yield viewLink Page.Home "Home"
            yield viewLink Page.AbvCalculator "ABV Calculator"
        ]]