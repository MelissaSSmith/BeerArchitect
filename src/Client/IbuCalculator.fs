module Client.IbuCalculator

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
open Client.HopAlphaAcidTable
open Client.NavigationMenu
open Client.Style

type Model = {
    IbuInput : IbuInput
    IbuResult : IbuResult
    HopAlphaAcidList : HopAlphaAcid list
    ErrorMsg : string }

type Msg =
    | SetBoilSize of float
    | SetBatchSize of float
    | SetTargetOriginalGravity of float
    | SetHopOunces of int*float
    | SetHopAlphaAcids of int*float
    | SetHopBoilTime of int*float
    | SetHopType of int*int
    | FinishIbuCalculation of IbuResult
    | ClickCalculate
    | GetHopAlphaAcids
    | FillHopAlphaAcidTable of HopAlphaAcid list
    | Error of exn

let inline charToInt c = int c - int '0'

let updateElement place newValue list =
    list |> List.mapi (fun index value -> if index = place then newValue else value )

let getHopAlphaAcidList a =
    promise {
        let props = 
            [ RequestProperties.Method HttpMethod.GET ]

        try
            let! result = Fetch.fetch ServerUrls.APIUrls.GetHopAlphaAcids props
            let! text = result.text()
            return Decode.Auto.unsafeFromString<HopAlphaAcid list> text
        with _ ->
            return failwithf "An error has occured"
    }

let getHopAlphaAcidListCmd =
    Cmd.ofPromise getHopAlphaAcidList 0 FillHopAlphaAcidTable Error

let calculateIbu (inputs:IbuInput) =
    promise {
        let body = Encode.Auto.toString(0, inputs) 

        let props = 
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [
                  HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body ]

        try 
            let! result = Fetch.fetch ServerUrls.APIUrls.CalculateIbu props
            let! text = result.text()
            return Decode.Auto.unsafeFromString<IbuResult> text
        with _ ->
            return failwithf "An error has occured"
    }

let calculateIbuCmd inputs = 
    Cmd.ofPromise calculateIbu inputs FinishIbuCalculation Error

let init result = 
    match result with
    | _ ->
        {IbuInput = { BoilSize = 0.0; BatchSize = 0.0; TargetOriginalGravity = 0.0; HopIbuInputs = List.init 6 (fun f -> { Ounces = 0.0; AlphaAcids = 0.0; BoilTime = 0.0; Type = 1 } ) } 
         IbuResult = { EstimatedBoilGravity = 0.0; TotalIbu = 0.0; HopIbuResults = List.init 6 (fun f -> { Utilization = 0.0; Ibus = 0.0 } ) }
         HopAlphaAcidList = List.init 1 (fun _ -> { Hops = ""; AverageAlphaAcids = 0.0 })
         ErrorMsg = "" }, getHopAlphaAcidListCmd

let update (msg:Msg) (model:Model): Model*Cmd<Msg> =
    match msg with
    | SetBoilSize boilSize ->
        { model with IbuInput = { model.IbuInput with BoilSize = boilSize }}, Cmd.none
    | SetBatchSize batchSize ->
        { model with IbuInput = { model.IbuInput with BatchSize = batchSize }}, Cmd.none
    | SetTargetOriginalGravity targetOriginalGravity ->
        { model with IbuInput = { model.IbuInput with TargetOriginalGravity = targetOriginalGravity }}, Cmd.none
    | SetHopOunces (inputId, ounces) ->
        let hopInputList = model.IbuInput.HopIbuInputs
        let hopInput = { List.item inputId hopInputList with Ounces = ounces }
        { model with IbuInput = { model.IbuInput with HopIbuInputs = updateElement inputId hopInput hopInputList }}, Cmd.none
    | SetHopAlphaAcids (inputId, alphaAcids) ->
        let hopInputList = model.IbuInput.HopIbuInputs 
        let hopInput = { List.item inputId hopInputList with AlphaAcids = alphaAcids }
        { model with IbuInput = { model.IbuInput with HopIbuInputs = updateElement inputId hopInput hopInputList }}, Cmd.none
    | SetHopBoilTime (inputId, boilTime) ->
        let hopInputList = model.IbuInput.HopIbuInputs
        let hopInput = { List.item inputId hopInputList with BoilTime = boilTime }
        { model with IbuInput = { model.IbuInput with HopIbuInputs = updateElement inputId hopInput hopInputList }}, Cmd.none
    | SetHopType (inputId, hopType) ->
        let hopInputList = model.IbuInput.HopIbuInputs
        let hopInput = { List.item inputId hopInputList with Type = hopType }
        { model with IbuInput = { model.IbuInput with HopIbuInputs = updateElement inputId hopInput hopInputList }}, Cmd.none
    | ClickCalculate ->
        model, calculateIbuCmd model.IbuInput
    | FinishIbuCalculation results ->
        { model with IbuResult = { model.IbuResult with EstimatedBoilGravity = results.EstimatedBoilGravity; TotalIbu = results.TotalIbu; HopIbuResults = results.HopIbuResults }}, Cmd.none
    | GetHopAlphaAcids ->
        model, getHopAlphaAcidListCmd
    | FillHopAlphaAcidTable hopAlphaAcidList ->
        { model with HopAlphaAcidList = hopAlphaAcidList }, Cmd.none
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            sidebarNavigationMenu
            div [ClassName "col-md-10 ml-sm-auto beer-body"] [
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
                            div [ClassName "col center-input"] [
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
                            table [ClassName "table table-sm table-striped"] [
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
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopOunces (0, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "alpha-acids-1" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopAlphaAcids (0, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "boil-time-1" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"
                                                OnChange (fun ev -> dispatch (SetHopBoilTime (0, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "hop-type-1" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetHopType (0, !!ev.target?value) ) ) 
                                                DefaultValue "1"] [
                                                    option [ Value "1" ] [ str "Whole/Plug"]
                                                    option [ Value "2" ] [ str "Pellet"]  ] ]
                                        th [] [
                                            p [] [str (sprintf "%.4f" model.IbuResult.HopIbuResults.Head.Utilization)]
                                        ]
                                        th [] [
                                            p [] [str (sprintf "%.2f" model.IbuResult.HopIbuResults.Head.Ibus)]
                                        ] ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "hop-ounces-2" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopOunces (1, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "alpha-acids-2" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopAlphaAcids (1, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "boil-time-2" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"
                                                OnChange (fun ev -> dispatch (SetHopBoilTime (1, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "hop-type-2" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetHopType (1, !!ev.target?value) ) ) 
                                                DefaultValue "1"] [
                                                    option [ Value "1" ] [ str "Whole/Plug"]
                                                    option [ Value "2" ] [ str "Pellet"]  ] ]
                                        th [] [
                                            p [] [str (sprintf "%.4f" model.IbuResult.HopIbuResults.Tail.Head.Utilization)]
                                        ]
                                        th [] [
                                            p [] [str (sprintf "%.2f" model.IbuResult.HopIbuResults.Tail.Head.Ibus)]
                                        ]
                                    ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "hop-ounces-3" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopOunces (2, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "alpha-acids-3" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopAlphaAcids (2, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "boil-time-3" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"
                                                OnChange (fun ev -> dispatch (SetHopBoilTime (2, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "hop-type-3" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetHopType (2, !!ev.target?value) ) ) 
                                                DefaultValue "1"] [
                                                    option [ Value "1" ] [ str "Whole/Plug"]
                                                    option [ Value "2" ] [ str "Pellet"]  ] ]
                                        th [] [
                                            p [] [str (sprintf "%.4f" model.IbuResult.HopIbuResults.Tail.Tail.Head.Utilization)]
                                        ]
                                        th [] [
                                            p [] [str (sprintf "%.2f" model.IbuResult.HopIbuResults.Tail.Tail.Head.Ibus)]
                                        ]
                                    ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "hop-ounces-4" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopOunces (3, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "alpha-acids-4" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopAlphaAcids (3, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "boil-time-4" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"
                                                OnChange (fun ev -> dispatch (SetHopBoilTime (3, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "hop-type-4" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetHopType (3, !!ev.target?value) ) ) 
                                                DefaultValue "1"] [
                                                    option [ Value "1" ] [ str "Whole/Plug"]
                                                    option [ Value "2" ] [ str "Pellet"]  ] ]
                                        th [] [
                                            p [] [str (sprintf "%.4f" model.IbuResult.HopIbuResults.Tail.Tail.Tail.Head.Utilization)]
                                        ]
                                        th [] [
                                            p [] [str (sprintf "%.2f" model.IbuResult.HopIbuResults.Tail.Tail.Tail.Head.Ibus)]
                                        ]
                                    ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "hop-ounces-5" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopOunces (4, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "alpha-acids-5" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopAlphaAcids (4, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "boil-time-5" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"
                                                OnChange (fun ev -> dispatch (SetHopBoilTime (4, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "hop-type-5" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetHopType (4, !!ev.target?value) ) ) 
                                                DefaultValue "1"] [
                                                    option [ Value "1" ] [ str "Whole/Plug"]
                                                    option [ Value "2" ] [ str "Pellet"]  ] ]
                                        th [] [
                                            p [] [str (sprintf "%.4f" model.IbuResult.HopIbuResults.Tail.Tail.Tail.Tail.Head.Utilization)]
                                        ]
                                        th [] [
                                            p [] [str (sprintf "%.2f" model.IbuResult.HopIbuResults.Tail.Tail.Tail.Tail.Head.Ibus)]
                                        ]
                                    ]
                                    tr [] [
                                        th [] [
                                            input [
                                                Id "hop-ounces-6" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopOunces (5, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "alpha-acids-6" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any" 
                                                OnChange (fun ev -> dispatch (SetHopAlphaAcids (5, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            input [
                                                Id "boil-time-6" 
                                                ClassName "form-control"
                                                AutoFocus false
                                                HTMLAttr.Type "number"
                                                Step "any"
                                                OnChange (fun ev -> dispatch (SetHopBoilTime (5, !!ev.target?value) ) ) ] ]
                                        th [] [
                                            select [
                                                Id "hop-type-6" 
                                                ClassName "form-control"
                                                AutoFocus false 
                                                OnChange (fun ev -> dispatch (SetHopType (5, !!ev.target?value) ) ) 
                                                DefaultValue "1"] [
                                                    option [ Value "1" ] [ str "Whole/Plug"]
                                                    option [ Value "2" ] [ str "Pellet"]  ] ]
                                        th [] [
                                            p [] [str (sprintf "%.4f" model.IbuResult.HopIbuResults.Tail.Tail.Tail.Tail.Tail.Head.Utilization)]
                                        ]
                                        th [] [
                                            p [] [str (sprintf "%.2f" model.IbuResult.HopIbuResults.Tail.Tail.Tail.Tail.Tail.Head.Ibus)]
                                        ]
                                    ]
                                ]
                            ]
                            div [ClassName "col-9"] [
                                button [
                                    Type "button"
                                    ClassName "btn btn-link"
                                    DataToggle "modal"
                                    DataTarget "#hop-alpha-acid-modal" 
                                ] [ str "Hops Alpha Acid Table" ]
                            ]
                            div [ClassName "col-3"] [
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
                                    p [ClassName "results"] [ str (sprintf "Total IBU: %.2f" model.IbuResult.TotalIbu) ]
                                ] ]
                        ] ] ] ] ] 
        hopAlphaAcidModal model.HopAlphaAcidList ] 