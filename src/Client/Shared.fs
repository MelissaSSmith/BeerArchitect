module Client.Shared

type PageModel =
    | HomePageModel
    | AbvCalculatorPageModal of AbvCalculator.Model

type Model =
    { PageModel : PageModel }

type Msg =
    | AbvCalculatorMsg of AbvCalculator.Msg
    | StorageFailure of exn

open Fable.Helpers.React
open Fable.Helpers.React.Props

let viewPage model dispatch =
    match model.PageModel with
    | HomePageModel ->
        Home.view ()
    | AbvCalculatorPageModal m ->
        AbvCalculator.view m (AbvCalculatorMsg >> dispatch)

let view model dispatch =
    div [] [ viewPage model dispatch
    ]