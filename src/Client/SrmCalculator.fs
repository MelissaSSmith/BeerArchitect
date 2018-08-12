module Client.SrmCalculator

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

open ServerCode
open Shared
open Client.NavigationMenu

type Model = {
    ErrorMsg : string }

type Msg =
    | ClickCalculate
    | Error of exn

let init result =
    match result with 
    | _ ->
        { ErrorMsg = "" }

let update (msg:Msg) (model:Model): Model*Cmd<Msg> =
    match msg with
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-md-9 ml-sm-auto col-lg-10 px-4 beer-body"] [
                    div [ClassName "row abv-row bottom-border"] [h2 [] [str("SRM Calculator")] ]
                ] ] ] ]