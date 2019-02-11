module Client.Home

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.NavigationMenu
open Client.Pages
open Client.Style

let view () =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "ml-sm-auto col-md-10 px-4 beer-body"] [
                    div [ClassName "row beer-row"] [
                        div [ClassName "card"] [
                            div [ClassName "card-header"] [ str "Browse" ]
                            ul [ClassName "list-group list-group-flush"] [
                                li [ClassName "list-group-item"] [
                                    plainLink Pages.YeastProfiles "Yeast"
                                ]
                            ]
                        ]
                    ]
                ]]]]