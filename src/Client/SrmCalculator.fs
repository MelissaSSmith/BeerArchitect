module Client.SrmCalculator

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
open Client.JqueryEmitter
open Fable.Import.Browser

type Model = {
    SrmInput : SrmInput
    SrmResult : SrmResult
    ErrorMsg : string }

type Msg =
    | Success of unit
    | InitializeData
    | CompleteSrmCalculation of SrmResult
    | FillInFermentableLists of Fermentable list
    | SetBatchSize of float
    | SetGrainAmount of int*float
    | SetGrainId of int*int
    | ClickCalculate
    | Error of exn

let inline charToInt c = int c - int '0'

let updateElement place newValue list =
    list |> List.mapi (fun index value -> if index = place then newValue else value ) 

let calculateSrm (inputs:SrmInput) =
    promise {
        let body = toJson inputs

        let props =
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [
                  HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body ]
        
        try
            return! Fetch.fetchAs<SrmResult> ServerUrls.APIUrls.CalculateSrm props
        with _ ->
            return! failwithf "An error has occured"
    }

let getFermentableList a =
    promise {
        let props = 
            [ RequestProperties.Method HttpMethod.GET ]
        
        try
            return! Fetch.fetchAs<Fermentable list> ServerUrls.APIUrls.GetFermentables props
        with _ ->
            return! failwithf "An error has occured"
    }

let fillInFermentableDropdown dropdownId fermentableList = 
    for f in fermentableList do
        let newRow = String.Format("<option value={0}>{1} {2}</option>", f.Id, f.Country, f.Name)
        JQuery.select("#"+dropdownId)
        |> JQuery.append newRow
        |> ignore

let fillInFermentableLists fermentableList = 
    let sortedList = fermentableList
                    |> Seq.sortBy(fun f -> f.Category)
                    |> Seq.sortBy(fun f -> f.Country)
                    |> Seq.sortBy(fun f -> f.Name)
    ["grain-1"; "grain-2"; "grain-3"; "grain-4"; "grain-5"]
    |> List.iter (fun g -> fillInFermentableDropdown g sortedList)

let calculateSrmCmd inputs =
    Cmd.ofPromise calculateSrm inputs CompleteSrmCalculation Error
let getFermentableListCmd =
    Cmd.ofPromise getFermentableList 0 FillInFermentableLists Error

let init result =
    match result with 
    | _ ->
        { SrmInput = { BatchSize = 0.0; GrainBill = List.init 5 (fun f -> 0.0,0); GrainIds = List.init 5 (fun f -> 0); GrainAmounts = List.init 5 (fun f -> 0.0)}
          SrmResult = { Srm = 0.0; Ebc = 0.0; HexColor = "#FFFFFF"}
          ErrorMsg = "" }, getFermentableListCmd

let update (msg:Msg) (model:Model): Model*Cmd<Msg> =
    match msg with
    | FillInFermentableLists fermentableList ->
        model, Cmd.ofFunc fillInFermentableLists fermentableList Success Error
    | CompleteSrmCalculation results ->
        { model with SrmResult = { model.SrmResult with Srm = results.Srm; Ebc = results.Ebc; HexColor = results.HexColor } }, Cmd.none
    | SetBatchSize batchSize ->
        { model with SrmInput = { model.SrmInput with BatchSize = batchSize }}, Cmd.none
    | SetGrainAmount (inputId, grainAmount) ->
        let grainAmounts = model.SrmInput.GrainAmounts
        { model with SrmInput = { model.SrmInput with GrainAmounts = updateElement inputId grainAmount grainAmounts }}, Cmd.none
    | SetGrainId (inputId, grainId) ->
        let grainIds = model.SrmInput.GrainIds
        { model with SrmInput = { model.SrmInput with GrainIds = updateElement inputId grainId grainIds }}, Cmd.none
    | ClickCalculate ->
        console.log(model.SrmInput.GrainIds)
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
                                Id "batchSize" 
                                ClassName "form-control"
                                AutoFocus true
                                HTMLAttr.Type "number"
                                Step "any"
                                OnChange (fun ev -> dispatch (SetBatchSize !!ev.target?value))
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
                                                Id "grain-amount-1" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                Name "grain-amount-1"
                                                OnChange (fun ev -> dispatch (SetGrainAmount (1, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "grain-1" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetGrainId (1, !!ev.target?value) ) )
                                                DefaultValue "0"] [
                                                option [ Disabled true
                                                         Value "0" ] [ str "Select Grain ..."] ] ] ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "grain-amount-2" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"
                                                OnChange (fun ev -> dispatch (SetGrainAmount (2, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "grain-2" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetGrainId (2, !!ev.target?value) ) )
                                                DefaultValue "0"] [
                                                option [ Disabled true
                                                         Value "0" ] [ str "Select Grain ..."] ] ] ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "grain-amount-3" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetGrainAmount (3, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "grain-3" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetGrainId (3, !!ev.target?value) ) )
                                                DefaultValue "0"] [
                                                option [ Disabled true
                                                         Value "0" ] [ str "Select Grain ..."] ] ] ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "grain-amount-4" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"
                                                OnChange (fun ev -> dispatch (SetGrainAmount (4, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "grain-4" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetGrainId (4, !!ev.target?value) ) )
                                                DefaultValue "0"] [
                                                option [ Disabled true
                                                         Value "0" ] [ str "Select Grain ..."] ] ] ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "grain-amount-5" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"
                                                OnChange (fun ev -> dispatch (SetGrainAmount (5, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "grain-5" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetGrainId (5, !!ev.target?value) ) )
                                                DefaultValue "0"] [
                                                option [ Disabled true
                                                         Value "0" ] [ str "Select Grain ..."] ] ] ] ] ]
                            button [
                                Id "calculateSrm"
                                ClassName "btn btn-info btn-lg btn-block"
                                OnClick (fun _ -> dispatch ClickCalculate)
                            ] [ str "Calculate"] ] ]
                    div [ClassName "offset-1 col-5"
                         Id "srm-inputs"][
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