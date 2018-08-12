module Client.App

open Elmish
open Elmish.Browser.Navigation
open Elmish.React
open Elmish.HMR

open Fable.Core.JsInterop
open Fable.Import

open Client
open Client.Pages
open Shared 

let handleNotFound (model: Model) =
    Browser.console.error("Error parsing url: " + Browser.window.location.href)
    ( model, Navigation.modifyUrl (toPath Page.Home) )

let urlUpdate (result:Page option) (model: Model) =
    match result with
    | None ->
        handleNotFound model
    | Some Page.AbvCalculator ->
        let m = AbvCalculator.init()
        { model with PageModel = AbvCalculatorPageModal m }, Cmd.none
    | Some Page.SrmCalculator ->
        let m = SrmCalculator.init()
        { model with PageModel = SrmCalculatorPageModel m}, Cmd.none
    | Some Page.Home ->
        { model with PageModel = HomePageModel }, Cmd.none

let init result =
    let stateJson: string option = !!Browser.window?__INIT_MODEL__
    match stateJson, result with
    | Some json, Some Page.Home ->
        let model: Model = ofJson json
        { model with PageModel = HomePageModel }, Cmd.none
    | _ ->
        let model =
            { PageModel = HomePageModel }

        urlUpdate result model

let update msg model =
    match msg, model.PageModel with
    | AbvCalculatorMsg msg, AbvCalculatorPageModal m ->
        let m, cmd = AbvCalculator.update msg m
        { model with
            PageModel = AbvCalculatorPageModal m},
                Cmd.batch [
                    Cmd.map AbvCalculatorMsg cmd
                ]
    | AbvCalculatorMsg _, _ -> model, Navigation.newUrl (toPath Page.AbvCalculator)
    | SrmCalculatorMsg msg, SrmCalculatorPageModel m ->
        let m, cmd = SrmCalculator.update msg m
        { model with 
            PageModel = SrmCalculatorPageModel m },
                Cmd.batch [
                    Cmd.map SrmCalculatorMsg cmd
                ]
    | SrmCalculatorMsg _,_ -> model, Navigation.newUrl (toPath Page.SrmCalculator)


Program.mkProgram init update view
|> Program.toNavigable Pages.urlParser urlUpdate
|> Program.withReact "beer-architect-main"
|> Program.run
