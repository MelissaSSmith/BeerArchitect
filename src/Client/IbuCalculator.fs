module Client.IbuCalculator

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
open System

open ServerCode
open Shared
open Client.NavigationMenu
open Client.Style

type Model = {
    IbuInput : IbuInput
    IbuResult : IbuResult
    ErrorMsg : string }

type Msg =
    | SetBoilSize of float
    | SetBatchSize of float
    | SetTargetOriginalGravity of float
    | FinishIbuCalculation of IbuResult
    | ClickCalculate
    | Error of exn

let init result = 
    match result with
    | _ ->
        {IbuInput = { BoilSize = 0.0; BatchSize = 0.0; TargetOriginalGravity = 0.0; HopIbuInputs = List.init 6 (fun f -> { Ounces = 0.0; AlphaAcids = 0.0; BoilTime = 0.0; Type = 1 } ) } 
         IbuResult = { EstimatedBoilGravity = 0.0; TotalIbu = 0.0; HopIbuResults = List.init 6 (fun f -> { Utilization = 0.0; Ibus = 0.0 } ) }
         ErrorMsg = "" }, Cmd.none

let update (msg:Msg) (model:Model): Model*Cmd<Msg> =
    match msg with
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none
    | _ ->
        model, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            sidebarNavigationMenu
            div [ClassName "col-md-10 ml-sm-auto col-lg-10 px-4 beer-body"] [
                div [ClassName "row beer-row bottom-border"] [ pageHeader "IBU Calculator" ]
                div [ClassName "row beer-row"] [
                    div [ClassName "col-12"
                         Id "ibu-inputs"] [
                        div [ClassName "row beer-row justify-content-start"] [
                            div [ClassName "col left-input"] [
                                label [] [ str "Boil Size (gallons)" ]
                                input [
                                    Id "boil-size" 
                                    ClassName "form-control"
                                    AutoFocus true
                                    HTMLAttr.Type "number"
                                    Step "any"
                                    OnChange (fun ev -> dispatch (SetBoilSize !!ev.target?value))
                                ]
                            ]
                            div [ClassName "col"] [
                                label [] [ str "Batch Size (gallons)" ]
                                input [
                                    Id "batch-size" 
                                    ClassName "form-control"
                                    AutoFocus false
                                    HTMLAttr.Type "number"
                                    Step "any"
                                    OnChange (fun ev -> dispatch (SetBatchSize !!ev.target?value))
                                ]
                            ]
                            div [ClassName "col right-input"] [
                                label [] [ str "Target Original Gravity (OG)" ]
                                input [
                                    Id "target-original-gravity" 
                                    ClassName "form-control"
                                    AutoFocus false
                                    HTMLAttr.Type "number"
                                    Step "any"
                                    OnChange (fun ev -> dispatch (SetTargetOriginalGravity !!ev.target?value))
                                ]
                            ]
                        ]
                        div [ClassName "row beer-row justify-content-start"] [
                            table [ClassName "table table-striped"] [
                                thead [] [
                                    tr [] [
                                        th [Scope "col"] [ str "Ounces" ]
                                        th [Scope "col"] [ str "Alpha Acids" ]
                                        th [Scope "col"] [ str "Boil Time" ]
                                        th [Scope "col"] [ str "Type" ]
                                        th [Scope "col"] [ str "Utilization" ]
                                        th [Scope "col"] [ str "IBUs" ]
                                    ] ]
                                tbody [] [
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "hop-ounces-1" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"  ] ]
                                        th [] [
                                            input [
                                                Id "alpha-acids-1" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"  ] ]
                                        th [] [
                                            input [
                                                Id "boil-time-1" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"  ] ]
                                        th [] [
                                            select [
                                                Id "hop-type-1" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                DefaultValue "1"] [
                                                    option [ Value "1" ] [ str "Whole/Plug"]
                                                    option [ Value "2" ] [ str "Pellet"]  ] ]
                                        th [] [
                                            p [] [str (sprintf "%.4f" model.IbuResult.HopIbuResults.Head.Utilization)]
                                        ]
                                        th [] [
                                            p [] [str (sprintf "%.2f" model.IbuResult.HopIbuResults.Head.Ibus)]
                                        ]
                                    ]
                                ]
                            ]
                            div [ClassName "offset-9 col-3"] [
                                button [
                                    Id "calculate-ibu"
                                    ClassName "btn btn-info btn-lg btn-block"
                                    OnClick (fun _ -> dispatch ClickCalculate)
                                ] [ str "Calculate"] ]
                            div [ClassName "col-12"
                                 Id "ibu-results"] [
                                div [ClassName "row beer-row justify-content-start"] [
                                    p [ClassName "results"] [ str (sprintf "Estimated Boil Gravity: %.3f" model.IbuResult.EstimatedBoilGravity) ]
                                ]
                                div [ClassName "row beer-row justify-content-start"] [
                                    p [ClassName "results"] [ str (sprintf "Total IBU: %.2f" model.IbuResult.TotalIbu)]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]