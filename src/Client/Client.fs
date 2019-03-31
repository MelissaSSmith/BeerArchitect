module Client.App

open Elmish
open Elmish.Browser.Navigation
open Elmish.React
open Elmish.HMR

open Fable.Core.JsInterop
open Fable.Import
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

open Client
open Client.Pages
open Shared 

let handleNotFound (model: Model) =
    Browser.console.error("Error parsing url: " + Browser.window.location.href)
    ( model, Navigation.modifyUrl (toPath Page.Home) )

let urlUpdate (result: Page option) (model: Model) =
    match result with
    | None ->
        handleNotFound model
    | Some Page.AbvCalculator ->
        let m = AbvCalculator.init()
        { model with PageModel = AbvCalculatorPageModal m }, Cmd.none
    | Some Page.SrmCalculator ->
        let m, cmd = SrmCalculator.init() 
        { model with PageModel = SrmCalculatorPageModel m }, Cmd.map SrmCalculatorMsg cmd
    | Some Page.IbuCalculator ->
        let m, cmd = IbuCalculator.init()
        { model with PageModel = IbuCalculatorPageModel m }, Cmd.map IbuCalculatorMsg cmd
    | Some Page.HydrometerTempCalculator ->
        let m, cmd = HydrometerTempCalculator.init()
        { model with PageModel = HydrometerTempCalculatorPageModel m }, Cmd.map HydrometerTempCalculatorMsg cmd
    | Some Page.AllGrainCalculator ->
        let m, cmd = AllGrainCalculator.init()
        { model with PageModel = AllGrainCalculatorPageModel m }, Cmd.map AllGrainCalculatorMsg cmd
    | Some Page.YeastProfiles ->
        let m, cmd = YeastProfiles.init()
        { model with PageModel = YeastProfilesPageModel m }, Cmd.map YeastProfilesMsg cmd
    | Some Page.DilutionBoilOffCalculator ->
        let m, cmd = DilutionBoilOffCalculator.init()
        { model with PageModel = DilutionBoilOffCalculatorPageModel m }, Cmd.map DilutionBoilOffCalculatorMsg cmd
    | Some Page.FermentableProfiles ->
        let m, cmd = FermentableProfiles.init()
        { model with PageModel = FermentableProfilesPageModel m }, Cmd.map FermentableProfilesMsg cmd
    | Some Page.HopProfiles ->
        let m, cmd = HopProfiles.init
        { model with PageModel = HopProfilesPageModel m }, Cmd.map HopProfilesMsg cmd
    | Some Page.HopProfile ->
        let m, cmd = HopProfile.init "admiral"
        { model with PageModel = HopProfilePageModel m}, Cmd.map HopProfileMsg cmd
    | Some Page.Home ->
        { model with PageModel = HomePageModel }, Cmd.none

let init result =
    let stateJson: string option = !!Browser.window?__INIT_MODEL__
    match stateJson, result with
    | Some json, Some Page.Home ->
        let model: Model = Decode.Auto.unsafeFromString<Model> json
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
    | IbuCalculatorMsg msg, IbuCalculatorPageModel m ->
        let m, cmd = IbuCalculator.update msg m
        { model with 
            PageModel = IbuCalculatorPageModel m },
                Cmd.batch [
                    Cmd.map IbuCalculatorMsg cmd
                ]
    | IbuCalculatorMsg _,_ -> model, Navigation.newUrl (toPath Page.IbuCalculator)
    | HydrometerTempCalculatorMsg msg, HydrometerTempCalculatorPageModel m ->
        let m, cmd = HydrometerTempCalculator.update msg m
        { model with 
            PageModel = HydrometerTempCalculatorPageModel m },
                Cmd.batch [
                    Cmd.map HydrometerTempCalculatorMsg cmd
                ]
    | HydrometerTempCalculatorMsg _,_ -> model, Navigation.newUrl (toPath Page.HydrometerTempCalculator)
    | AllGrainCalculatorMsg msg, AllGrainCalculatorPageModel m ->
        let m, cmd = AllGrainCalculator.update msg m
        { model with 
            PageModel = AllGrainCalculatorPageModel m },
                Cmd.batch [
                    Cmd.map AllGrainCalculatorMsg cmd
                ]
    | AllGrainCalculatorMsg _,_ -> model, Navigation.newUrl (toPath Page.AllGrainCalculator)
    | YeastProfilesMsg msg, YeastProfilesPageModel m ->
        let m, cmd = YeastProfiles.update msg m
        { model with
            PageModel = YeastProfilesPageModel m},
                Cmd.batch [
                    Cmd.map YeastProfilesMsg cmd
                ]
    | YeastProfilesMsg _, _ -> model, Navigation.newUrl (toPath Page.YeastProfiles)
    | DilutionBoilOffCalculatorMsg msg, DilutionBoilOffCalculatorPageModel m ->
        let m, cmd = DilutionBoilOffCalculator.update msg m
        { model with 
            PageModel = DilutionBoilOffCalculatorPageModel m},
                Cmd.batch [
                    Cmd.map DilutionBoilOffCalculatorMsg cmd
                ]
    | DilutionBoilOffCalculatorMsg _, _ -> model, Navigation.newUrl (toPath Page.DilutionBoilOffCalculator)
    | FermentableProfilesMsg msg, FermentableProfilesPageModel m ->
        let m, cmd = FermentableProfiles.update msg m
        { model with
            PageModel = FermentableProfilesPageModel m },
                Cmd.batch [
                    Cmd.map FermentableProfilesMsg cmd
                ]
    | FermentableProfilesMsg _, _ -> model, Navigation.newUrl (toPath Page.FermentableProfiles)
    | HopProfilesMsg msg, HopProfilesPageModel m ->
        let m, cmd = HopProfiles.update msg m
        { model with
            PageModel = HopProfilesPageModel m },
                Cmd.batch [
                    Cmd.map HopProfilesMsg cmd
                ]
    | HopProfilesMsg _, _ -> model, Navigation.newUrl (toPath Page.HopProfiles)
    | HopProfileMsg msg, HopProfilePageModel m ->
        let m, cmd = HopProfile.update msg m
        { model with
            PageModel = HopProfilePageModel m },
                Cmd.batch [
                    Cmd.map HopProfileMsg cmd
                ]
    | HopProfileMsg _, _ -> model, Navigation.newUrl (toPath Page.HopProfile)


Program.mkProgram init update view
|> Program.toNavigable Pages.urlParser urlUpdate
|> Program.withReact "beer-architect-main"
|> Program.run
