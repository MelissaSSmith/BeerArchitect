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
open Client.Style

type Model = {
    SrmInput : SrmInput
    SrmResult : SrmResult
    ErrorMsg : string }

type Msg =
    | CompleteSrmCalculation of SrmResult
    | UpdateBatchSize of float
    | UpdateGrainAmount of string*float
    | UpdateGrainId of string*int
    | ClickCalculate
    | Error of exn

let init result =
    match result with 
    | _ ->
        { SrmInput = { BatchSize = 0.0; GrainAmounts = List.init 5 (fun f -> 0); GrainIds = List.init 5 (fun f -> 0)}
          SrmResult = { Srm = 0.0; Ebc = 0.0; HexColor = "#FFFFFF"}
          ErrorMsg = "" }

let update (msg:Msg) (model:Model): Model*Cmd<Msg> =
    match msg with
    | UpdateBatchSize batchSize ->
        model, Cmd.none
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-md-12 ml-sm-auto col-lg-12 px-4 beer-body"] [
                    div [ClassName "row beer-row bottom-border"] [ pageHeader "SRM Calculator" ]
                    div [ClassName "col-6"
                         Id "srm-inputs"][
                        div [ClassName "row beer-row justify-content-start"] [
                            label [] [ str "Batch Size (gallons)" ]
                            input [
                                Id "batchSize" 
                                ClassName "form-control"
                                AutoFocus true
                                HTMLAttr.Type "number"
                                Step "any"
                                OnChange (fun ev -> dispatch (UpdateBatchSize !!ev.target?value))
                            ]
                        ]
                        div [ClassName "row beer-row justify-content-start"] [
                            table [ClassName "table table-striped"] [
                                thead [] [
                                    tr [] [
                                        th [Scope "col"] [ str "Pounds" ]
                                        th [Scope "col"] [ str "Grain" ]
                                    ] ]
                                tbody [] [
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "grainAmount1" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" ] ]
                                        th [] [
                                            select [
                                                Id "grainAmount1" 
                                                ClassName "form-control"
                                                AutoFocus false ] [
                                                option [ Disabled true 
                                                         Selected true] [ str "Select Grain ..."] ] ] ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "grainAmount2" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" ] ]
                                        th [] [
                                            select [
                                                Id "grainAmount2" 
                                                ClassName "form-control"
                                                AutoFocus false ] [
                                                option [ Disabled true 
                                                         Selected true] [ str "Select Grain ..."] ] ] ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "grainAmount3" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" ] ]
                                        th [] [
                                            select [
                                                Id "grainAmount3" 
                                                ClassName "form-control"
                                                AutoFocus false ] [
                                                option [ Disabled true 
                                                         Selected true] [ str "Select Grain ..."] ] ] ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "grainAmount4" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" ] ]
                                        th [] [
                                            select [
                                                Id "grainAmount4" 
                                                ClassName "form-control"
                                                AutoFocus false ] [
                                                option [ Disabled true 
                                                         Selected true] [ str "Select Grain ..."] ] ] ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "grainAmount5" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" ] ]
                                        th [] [
                                            select [
                                                Id "grainAmount5" 
                                                ClassName "form-control"
                                                AutoFocus false ] [
                                                option [ Disabled true 
                                                         Selected true] [ str "Select Grain ..."] ] ] ] ] ]
                            button [
                                Type "button"
                                Id "calculateSrm"
                                ClassName "btn btn-info btn-lg btn-block"
                                OnClick (fun _ -> dispatch ClickCalculate)
                            ] [ str "Calculate"] ] ]
                    div [ClassName "col-6"
                         Id "srm-inputs"][
                        div [ClassName "row beer-row justify-content-start"] [ 
                            p [ ClassName "results" ] [ str (sprintf "Standard ABV:  %.2f %%" model.SrmResult.Srm)]
                        ]
                        div [ClassName "row beer-row justify-content-start"] [ 
                            p [ ClassName "results" ] [ str (sprintf "Standard ABV:  %.2f %%" model.SrmResult.Ebc)]
                        ]
                        div [ClassName "row beer-row justify-content-center"] [ 
                            canvas [HTMLAttr.Height "300"
                                    HTMLAttr.Width "300"
                                    Style [BackgroundColor model.SrmResult.HexColor] ] []
                        ]
                    ]
                ] 
            ] 
        ]
    ]