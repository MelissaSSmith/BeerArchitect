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
    | SetGrainAmount of string*float
    | SetGrainId of string*int
    | ClickCalculate
    | Error of exn

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
    ["grain1"; "grain2"; "grain3"; "grain4"; "grain5"]
    |> List.iter (fun g -> fillInFermentableDropdown g sortedList)

let calculateSrmCmd inputs =
    Cmd.ofPromise calculateSrm inputs CompleteSrmCalculation Error
let getFermentableListCmd =
    Cmd.ofPromise getFermentableList 0 FillInFermentableLists Error

let init result =
    match result with 
    | _ ->
        { SrmInput = { BatchSize = 0.0; GrainBill = List.init 5 (fun f -> 0.0,0); GrainIds = List.init 5 (fun f -> 0)}
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
                                                Id "grainAmount1" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" ] ]
                                        th [] [
                                            select [
                                                Id "grain1" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                DefaultValue "0"] [
                                                option [ Disabled true
                                                         Value "0" ] [ str "Select Grain ..."] ] ] ]
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
                                                Id "grain2" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                DefaultValue "0"] [
                                                option [ Disabled true
                                                         Value "0" ] [ str "Select Grain ..."] ] ] ]
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
                                                Id "grain3" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                DefaultValue "0"] [
                                                option [ Disabled true
                                                         Value "0" ] [ str "Select Grain ..."] ] ] ]
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
                                                Id "grain4" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                DefaultValue "0"] [
                                                option [ Disabled true
                                                         Value "0" ] [ str "Select Grain ..."] ] ] ]
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
                                                Id "grain5" 
                                                ClassName "form-control"
                                                AutoFocus false 
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