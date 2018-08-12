module Client.Home

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.NavigationMenu

let view () =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-10 beer-body"] []]]]