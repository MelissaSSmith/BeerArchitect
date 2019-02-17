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
                div [ClassName "ml-sm-auto col-md-10 px-4 beer-body"] [
                    div [ClassName "row beer-row col-3"] [
                        div [ClassName "card w-100"] [
                            div [ClassName "card-header"] [ str "Browse" ]
                            ul [ClassName "list-group list-group-flush"] [
                                li [ClassName "list-group-item"] [
                                    a [ Href "/fermentables"
                                        ClassName "btn btn-link"] [ str "Fermentables" ]
                                ]
                                li [ClassName "list-group-item"] [
                                    a [ Href "/yeast-profiles"
                                        ClassName "btn btn-link"] [ str "Yeast" ]
                                ]
                            ]
                        ]
                    ]
                ]]]]