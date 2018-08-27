module Client.Shared

type PageModel =
    | HomePageModel
    | AbvCalculatorPageModal of AbvCalculator.Model
    | SrmCalculatorPageModel of SrmCalculator.Model
    | IbuCalculatorPageModel of IbuCalculator.Model

type Model =
    { PageModel : PageModel }

type Msg =
    | AbvCalculatorMsg of AbvCalculator.Msg
    | SrmCalculatorMsg of SrmCalculator.Msg
    | IbuCalculatorMsg of IbuCalculator.Msg

open Fable.Helpers.React

let viewPage model dispatch =
    match model.PageModel with
    | HomePageModel ->
        Home.view ()
    | AbvCalculatorPageModal m ->
        AbvCalculator.view m (AbvCalculatorMsg >> dispatch)
    | SrmCalculatorPageModel m ->
        SrmCalculator.view m (SrmCalculatorMsg >> dispatch)
    | IbuCalculatorPageModel m ->
        IbuCalculator.view m (IbuCalculatorMsg >> dispatch)

let view model dispatch =
    div [] [ viewPage model dispatch ]