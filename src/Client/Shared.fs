module Client.Shared

type PageModel =
    | HomePageModel

type Model =
    { PageModel : PageModel }

type Msg =
    | StorageFailure of exn

open Fable.Helpers.React
open Fable.Helpers.React.Props

let viewPage model dispatch =
    match model.PageModel with
    | HomePageModel ->
        Home.view ()

let view model dispatch =
    div [] [ viewPage model dispatch
    ]