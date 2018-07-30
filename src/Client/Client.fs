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

let viewPage model dispatch =
    match model.PageModel with
    | HomePageModel ->
        Home.view ()

let handleNotFound (model: Model) =
    Browser.console.error("Error parsing url: " + Browser.window.location.href)
    ( model, Navigation.modifyUrl (toPath Page.Home) )

let urlUpdate (result:Page option) (model: Model) =
    match result with
    | None ->
        handleNotFound model
    | Some Page.Home ->
        { model with PageModel = HomePageModel }, Cmd.none

let init result =
    let stateJson: string option = !!Browser.window?__INIT_MODEL__
    match stateJson, result with
    | _ ->
        let model =
            { PageModel = HomePageModel }

        urlUpdate result model

let update msg model =
    match msg, model.PageModel with
    | StorageFailure e, _ ->
        printfn "Unable to access local storage: %A" e
        model, Cmd.none

Program.mkProgram init update view
|> Program.toNavigable Pages.urlParser urlUpdate
|> Program.withReact "beer-architect-main"
|> Program.run
