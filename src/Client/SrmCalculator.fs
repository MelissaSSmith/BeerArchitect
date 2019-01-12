module Client.SrmCalculator

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
    SrmInput : SrmInput
    SrmResult : SrmResult
    FermentableTableModel : FermentableTable.Model
    ErrorMsg : string }

type Msg =
    | Success of unit
    | InitializeData
    | CompleteSrmCalculation of SrmResult
    | SetBatchSize of float
    | FermentableTableMsg of FermentableTable.Msg
    | ClickCalculate
    | Error of exn

let inline charToInt c = int c - int '0'

let updateElement place newValue list =
    list |> List.mapi (fun index value -> if index = place then newValue else value ) 

let calculateSrm (inputs:SrmInput) =
    promise {
        let body = Encode.Auto.toString(0, inputs)

        let props =
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [
                  HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body ]
        
        try
            let! result = Fetch.fetch ServerUrls.APIUrls.CalculateSrm props
            let! text = result.text()
            return Decode.Auto.unsafeFromString<SrmResult> text
        with _ ->
            return! failwithf "An error has occured"
    }

let calculateSrmCmd inputs =
    Cmd.ofPromise calculateSrm inputs CompleteSrmCalculation Error

let init result =
    let subModel, cmd = FermentableTable.init 5
    match result with 
    | _ ->
        { SrmInput = { BatchSize = 0.0; GrainBill = List.init 5 (fun f -> 0.0,0); GrainIds = List.init 5 (fun f -> 0); GrainAmounts = List.init 5 (fun f -> 0.0)}
          SrmResult = { Srm = 0.0; Ebc = 0.0; HexColor = "#FFFFFF"}
          FermentableTableModel = subModel
          ErrorMsg = "" }, 
            Cmd.batch [
                Cmd.map FermentableTableMsg cmd ]

let update (msg:Msg) (model:Model): Model*Cmd<Msg> =
    match msg with
    | CompleteSrmCalculation results ->
        { model with SrmResult = { model.SrmResult with Srm = results.Srm; Ebc = results.Ebc; HexColor = results.HexColor } }, Cmd.none
    | SetBatchSize batchSize ->
        { model with SrmInput = { model.SrmInput with BatchSize = batchSize }}, Cmd.none
    | FermentableTableMsg msg ->
        match msg with 
        | FermentableTable.Msg.SetGrainAmount (inputId, grainAmount)->
            let grainAmounts = model.SrmInput.GrainAmounts
            { model with SrmInput = { model.SrmInput with GrainAmounts = updateElement inputId grainAmount grainAmounts }}, Cmd.none
        | FermentableTable.Msg.SetGrainId (inputId, grainId) ->
            let grainIds = model.SrmInput.GrainIds
            { model with SrmInput = { model.SrmInput with GrainIds = updateElement inputId grainId grainIds }}, Cmd.none
        | _ ->
            let submodel, cmd = FermentableTable.update msg model.FermentableTableModel
            { model with FermentableTableModel = submodel}, Cmd.map FermentableTableMsg cmd
    | ClickCalculate ->
        model, calculateSrmCmd model.SrmInput
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none
    | _ ->
        model, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-md-10 ml-sm-auto col-lg-10 px-4 beer-body"] [
                    div [ClassName "row beer-row bottom-border"] [ pageHeader "SRM Calculator" ]
                    div [ClassName "row beer-row"] [
                        div [ClassName "col-6"
                             Id "srm-inputs"][
                            div [ClassName "row beer-row justify-content-start"] [
                                label [] [ str "Batch Size (gallons)" ]
                                input [
                                    Id "batch-size" 
                                    ClassName "form-control"
                                    AutoFocus true
                                    HTMLAttr.Type "number"
                                    Step "any"
                                    OnChange (fun ev -> dispatch (SetBatchSize !!ev.target?value))
                                ]
                            ]
                            div [ClassName "row beer-row justify-content-start"] [
                                FermentableTable.view model.FermentableTableModel (dispatch << FermentableTableMsg)
                                button [
                                    Id "calculate-srm"
                                    ClassName "btn btn-info btn-lg btn-block"
                                    OnClick (fun _ -> dispatch ClickCalculate)
                                ] [ str "Calculate"] ] ]
                        div [ClassName "offset-1 col-5"
                             Id "srm-results"][
                            div [ClassName "row beer-row justify-content-start"] [ 
                                    p [ ClassName "results" ] [ str (sprintf "SRM:  %.2f" model.SrmResult.Srm)]
                            ]
                            div [ClassName "row beer-row justify-content-start"] [ 
                                    p [ ClassName "results" ] [ str (sprintf "EBC:  %.2f" model.SrmResult.Ebc)]
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
]