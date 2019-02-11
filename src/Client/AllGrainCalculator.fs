module Client.AllGrainCalculator

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
    AllGrainInput : AllGrainInput
    AllGrainResult: AllGrainResult
    FermentableTableModel : FermentableTable.Model
    ErrorMsg : string }

type Msg =
    | SetPreBoilSize of float
    | SetBatchSize of float
    | SetEfficiency of float
    | SetYeastTolerance of int
    | FermentableTableMsg of FermentableTable.Msg
    | ClickCalculate
    | CompleteAllGrainCalculation of AllGrainResult
    | Error of exn

let calculateAllGrainEstimations (inputs:AllGrainInput) = 
    promise {
        let body = Encode.Auto.toString(0, inputs)

        let props = 
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [
                  HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body ]
        
        try
            let! result = Fetch.fetch ServerUrls.APIUrls.CalculateAllGrainEstimations props
            let! text = result.text()
            return Decode.Auto.unsafeFromString<AllGrainResult> text
        with _ ->
            return! failwithf "An error has occured"
    }

let calculateAllGrainEstimationsCmd inputs =
    Cmd.ofPromise calculateAllGrainEstimations inputs CompleteAllGrainCalculation Error

let getYeastTolerance inputValue =
    match inputValue with
    | 1 -> YeastTolerance.Low
    | 2 -> YeastTolerance.Medium
    | 3 -> YeastTolerance.High
    | _ -> YeastTolerance.Medium

let initInput listSize = 
    { PreBoilSize = 0.0
      BatchSize = 0.0
      Effciency = 0.0
      YeastTolerance = YeastTolerance.Medium
      GrainBill = List.init listSize (fun f -> 0.0,0)
      GrainIds = List.init listSize (fun f -> 0)
      GrainAmounts = List.init listSize (fun f -> 0.0) }

let init result =
    let initialSize = 5
    let table, cmd = FermentableTable.init initialSize
    let input = initInput initialSize
    match result with 
    | _ -> 
        { AllGrainInput = input
          AllGrainResult = { EstPreBoilOG = 0.0; EstOG = 0.0; EstFG = 0.0; EstABV = 0.0 }
          FermentableTableModel = table
          ErrorMsg = "" }, 
            Cmd.batch [
                Cmd.map FermentableTableMsg cmd ]

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
    match msg with
    | CompleteAllGrainCalculation allGrainResult ->
        { model with AllGrainResult = allGrainResult }, Cmd.none
    | SetPreBoilSize preBoilSize ->
        { model with AllGrainInput = { model.AllGrainInput with PreBoilSize = preBoilSize } }, Cmd.none
    | SetBatchSize batchSize ->
        { model with AllGrainInput = { model.AllGrainInput with BatchSize = batchSize } }, Cmd.none
    | SetEfficiency efficiency ->
        { model with AllGrainInput = { model.AllGrainInput with Effciency = efficiency } }, Cmd.none
    | SetYeastTolerance yeastTolerance ->
        let tolerance = getYeastTolerance yeastTolerance
        { model with AllGrainInput = { model.AllGrainInput with YeastTolerance = tolerance } }, Cmd.none
    | FermentableTableMsg msg ->
        match msg with 
        | FermentableTable.Msg.SetGrainAmount (inputId, grainAmount)->
            let grainAmounts = model.AllGrainInput.GrainAmounts
            { model with AllGrainInput = { model.AllGrainInput with GrainAmounts = FermentableTable.updateElement inputId grainAmount grainAmounts }}, Cmd.none
        | FermentableTable.Msg.SetGrainId (inputId, grainId) ->
            let grainIds = model.AllGrainInput.GrainIds
            { model with AllGrainInput = { model.AllGrainInput with GrainIds = FermentableTable.updateElement inputId grainId grainIds }}, Cmd.none
        | _ ->
            let submodel, cmd = FermentableTable.update msg model.FermentableTableModel
            { model with FermentableTableModel = submodel}, Cmd.map FermentableTableMsg cmd
    | ClickCalculate ->
        model, calculateAllGrainEstimationsCmd model.AllGrainInput
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "ml-sm-auto col-md-10 px-4 beer-body"] [
                    div [ClassName "row beer-row bottom-border"] [ pageHeader "All Grain OG, FG, ABV Calculator"]
                    div [ClassName "row beer-row"] [
                        div [ClassName "col-12"
                             Id "all-grain-inputs"] [
                            div [ClassName "row beer-row justify-content-start"] [
                                div [ClassName "col left-input"] [
                                    label [] [ str "Pre-Boil Wort Collected (gallons)" ]
                                    input [
                                        Id "pre-boil-size" 
                                        ClassName "form-control"
                                        AutoFocus true
                                        HTMLAttr.Type "number"
                                        Step "any"
                                        OnChange (fun ev -> dispatch (SetPreBoilSize !!ev.target?value))
                                    ]
                                ]
                                div [ClassName "col right-input"] [
                                    label [] [ str "Post-Boil Batch Size (gallons)" ]
                                    input [
                                        Id "batch-size" 
                                        ClassName "form-control"
                                        AutoFocus false
                                        HTMLAttr.Type "number"
                                        Step "any"
                                        OnChange (fun ev -> dispatch (SetBatchSize !!ev.target?value))
                                    ]
                                ]
                            ]
                            div [ClassName "row beer-row justify-content-start"] [ 
                                div [ClassName "col left-input"] [
                                    label [] [ str "Efficiency (%)" ]
                                    input [
                                        Id "efficiency" 
                                        ClassName "form-control"
                                        AutoFocus false
                                        HTMLAttr.Type "number"
                                        Step "any"
                                        OnChange (fun ev -> dispatch (SetEfficiency !!ev.target?value))
                                    ]
                                ]
                                div [ClassName "col right-input"] [
                                    label [] [ str "Yeast Alcohol Tolerance" ]
                                    select [
                                        Id "hop-type-1" 
                                        ClassName "form-control"
                                        AutoFocus false 
                                        OnChange (fun ev -> dispatch (SetYeastTolerance !!ev.target?value ) ) 
                                        DefaultValue "2"] [
                                            option [ Value "1" ] [ str "Low - Attenution ~ 66%" ]
                                            option [ Value "2" ] [ str "Medium - Attenution ~ 72%" ]  
                                            option [ Value "3" ] [ str "High - Attenution ~ 77%" ]  
                                        ]
                                ]
                            ]
                            div [ClassName "row beer-row justify-content-start"] [
                                div [ClassName "col left-input info-text"] [
                                    p [] [ 
                                        str "Most yeasts fall into the medium category" 
                                        a [
                                            Href "/yeast-profiles"
                                            Target "_blank"
                                            ClassName "btn btn-link"] [ str "Yeast Strength Tables" ]
                                    ]
                                ]
                            ]
                            div [ClassName "row beer-row justify-content-start"] [
                                FermentableTable.view model.FermentableTableModel (dispatch << FermentableTableMsg)
                                button [
                                    Id "calculate-estimations"
                                    ClassName "btn btn-info btn-lg btn-block"
                                    OnClick (fun _ -> dispatch ClickCalculate)
                                ] [ str "Calculate"]
                            ]
                        ]
                        div [ClassName "col-12"
                             Id "all-grain-results"] [ 
                            div [ClassName "row beer-row justify-content-start"] [ 
                                    p [ ClassName "results" ] [ str (sprintf "Estimated Pre Boil OG:  %.3f" model.AllGrainResult.EstPreBoilOG)]
                            ]
                            div [ClassName "row beer-row justify-content-start"] [ 
                                    p [ ClassName "results" ] [ str (sprintf "Estimated Original Gravity:  %.3f" model.AllGrainResult.EstOG)]
                            ]
                            div [ClassName "row beer-row justify-content-start"] [ 
                                    p [ ClassName "results" ] [ str (sprintf "Estimated Final Gravity:  %.3f" model.AllGrainResult.EstFG)]
                            ]
                            div [ClassName "row beer-row justify-content-start"] [ 
                                    p [ ClassName "results" ] [ str (sprintf "Estimated Alcohol By Volume:  %.2f %%" model.AllGrainResult.EstABV)]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]