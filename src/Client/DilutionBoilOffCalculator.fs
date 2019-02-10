module Client.DilutionBoilOffCalculator

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

open ServerCode
open Shared
open Client.NavigationMenu
open Client.Style

type Model = {
    DilutionBoilOffInput: DilutionBoilOffInput
    DilutionBoilOffResult: DilutionBoilOffResult
    ErrorMsg: string }

type Msg =
    | CompleteCalculation of DilutionBoilOffResult
    | SetWortVolumeForNewVolume of float
    | SetCurrentGravityForNewVolume of float
    | SetDesiredGravity of float
    | SetWortVolumeForNewGrav of float
    | SetCurrentGravityForNewGrav of float
    | SetTargetVolume of float
    | CalculateNewVolume
    | CalculateNewGravity
    | Error of exn

let init result =
    match result with
    | _ ->
        { DilutionBoilOffInput = {NewVolWortVol = 0.0; NewVolCurrGrav = 0.0; DesiredGravity = 0.0; NewGravWortVol = 0.0; NewGravCurrGrav = 0.0; TargetVolume = 0.0}
          DilutionBoilOffResult = {NewVolume = 0.0; NewVolumeDiff = 0.0; NewGravity = 0.0; NewGravityDiff = 0.0}
          ErrorMsg = "" }, Cmd.none

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
    match msg with
    | CompleteCalculation results ->
        { model with DilutionBoilOffResult = results }, Cmd.none
    | SetWortVolumeForNewVolume wortVolume ->
        { model with DilutionBoilOffInput = { model.DilutionBoilOffInput with NewVolWortVol = wortVolume } }, Cmd.none
    | SetCurrentGravityForNewVolume currentGravity ->
        { model with DilutionBoilOffInput = { model.DilutionBoilOffInput with NewVolCurrGrav = currentGravity } }, Cmd.none
    | SetDesiredGravity desiredGravity ->
        { model with DilutionBoilOffInput = { model.DilutionBoilOffInput with DesiredGravity = desiredGravity } }, Cmd.none
    | SetWortVolumeForNewGrav wortVolume ->
        { model with DilutionBoilOffInput = { model.DilutionBoilOffInput with NewGravWortVol = wortVolume } }, Cmd.none
    | SetCurrentGravityForNewGrav currentGravity ->
        { model with DilutionBoilOffInput = { model.DilutionBoilOffInput with NewGravCurrGrav = currentGravity } }, Cmd.none
    | SetTargetVolume targetVolume ->
        { model with DilutionBoilOffInput = { model.DilutionBoilOffInput with TargetVolume = targetVolume } }, Cmd.none
    | CalculateNewVolume ->
        model, Cmd.none
    | CalculateNewGravity ->
        model, Cmd.none
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-md-10 ml-sm-auto col-lg-10 px-4 beer-body"] [
                    div [ClassName "row beer-row bottom-border"] [ pageHeader "Dilution and Boil Off Gravity Calculator"] 
                    div [ClassName "row beer-row justify-content-start"] [
                        div [ClassName "col-6 border-right"] [
                            h4 [] [str "Find New Volume"] 
                            div [] [
                                label [] [str ("Wort Volume")]
                                input [
                                    Id "newVolWortVol" 
                                    ClassName "form-control"
                                    AutoFocus false
                                    HTMLAttr.Type "number"
                                    Step "any"
                                    OnChange (fun ev -> dispatch (SetWortVolumeForNewVolume !!ev.target?value))
                                ]
                            ]
                            div [] [
                                label [] [str ("Current Gravity")]
                                input [
                                    Id "newVolCurrentGrav" 
                                    ClassName "form-control"
                                    AutoFocus false
                                    HTMLAttr.Type "number"
                                    Step "any"
                                    OnChange (fun ev -> dispatch (SetCurrentGravityForNewVolume !!ev.target?value))
                                ]
                            ]
                            div [] [
                                label [] [str ("Desired Gravity")]
                                input [
                                    Id "desirecGrav" 
                                    ClassName "form-control"
                                    AutoFocus false
                                    HTMLAttr.Type "number"
                                    Step "any"
                                    OnChange (fun ev -> dispatch (SetDesiredGravity !!ev.target?value))
                                ]
                            ]
                            div [ ClassName "row beer-row"] [
                                button [
                                    Type "button"
                                    Id "calculateNewVolume"
                                    ClassName "btn btn-info btn-lg btn-block"
                                    OnClick (fun _ -> dispatch CalculateNewVolume)
                                ] [ str "Calculate New Volume"]
                            ]
                            div [] [
                                p [ ClassName "results" ] [ str (sprintf "New Volume:  %.2f" model.DilutionBoilOffResult.NewVolume)]
                            ]
                            div [] [
                                p [ ClassName "results" ] [ str (sprintf "Difference:  %.2f" model.DilutionBoilOffResult.NewVolumeDiff)]
                            ]
                        ]
                        div [ClassName "col-6"] [
                            h4 [] [str "Find New Gravity"]
                            div [] [
                                label [] [str ("Wort Volume")]
                                input [
                                    Id "newGravWortVol" 
                                    ClassName "form-control"
                                    AutoFocus false
                                    HTMLAttr.Type "number"
                                    Step "any"
                                    OnChange (fun ev -> dispatch (SetWortVolumeForNewGrav !!ev.target?value))
                                ]
                            ]
                            div [] [
                                label [] [str ("Current Gravity")]
                                input [
                                    Id "newGravCurrentGrav" 
                                    ClassName "form-control"
                                    AutoFocus false
                                    HTMLAttr.Type "number"
                                    Step "any"
                                    OnChange (fun ev -> dispatch (SetCurrentGravityForNewGrav !!ev.target?value))
                                ]
                            ]
                            div [] [
                                label [] [str ("Target Volume")]
                                input [
                                    Id "targetVolume" 
                                    ClassName "form-control"
                                    AutoFocus false
                                    HTMLAttr.Type "number"
                                    Step "any"
                                    OnChange (fun ev -> dispatch (SetTargetVolume !!ev.target?value))
                                ]
                            ]
                            div [ ClassName "row beer-row"] [
                                button [
                                    Type "button"
                                    Id "calculateNewGravity"
                                    ClassName "btn btn-info btn-lg btn-block"
                                    OnClick (fun _ -> dispatch CalculateNewGravity)
                                ] [ str "Calculate New Gravity"]
                            ]
                            div [] [
                                p [ ClassName "results" ] [ str (sprintf "New Gravity:  %.2f" model.DilutionBoilOffResult.NewGravity)]
                            ]
                            div [] [
                                p [ ClassName "results" ] [ str (sprintf "Difference:  %.2f" model.DilutionBoilOffResult.NewGravityDiff)]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]