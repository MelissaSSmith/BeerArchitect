module Client.Shared

type PageModel =
    | HomePageModel
    | AbvCalculatorPageModal of AbvCalculator.Model
    | SrmCalculatorPageModel of SrmCalculator.Model

type Model =
    { PageModel : PageModel }

type Msg =
    | AbvCalculatorMsg of AbvCalculator.Msg
    | SrmCalculatorMsg of SrmCalculator.Msg

open Fable.Helpers.React

let viewPage model dispatch =
    match model.PageModel with
    | HomePageModel ->
        Home.view ()
    | AbvCalculatorPageModal m ->
        AbvCalculator.view m (AbvCalculatorMsg >> dispatch)
    | SrmCalculatorPageModel m ->
        SrmCalculator.view m (SrmCalculatorMsg >> dispatch)

let view model dispatch =
    div [] [ viewPage model dispatch ]