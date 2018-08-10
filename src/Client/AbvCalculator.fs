module Client.AbvCalculator

open Elmish
open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.Import.Browser
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

open ServerCode
open Shared
open Client.Home
open Client.Pages

type Model = {
    GravityReading : GravityReading
    AbvResult : AbvResult
    ErrorMsg : string }

type Msg =
    | CompleteCalculate of AbvResult
    | ClickCalculate of GravityReading
    | Error of exn

let calculateAbv (readings:GravityReading) =
    promise {
        let body = toJson readings

        let props =
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [
                  HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body ]

        try 
            return! Fetch.fetchAs<AbvResult> ServerUrls.APIUrls.CalculateAbv props
        with _ ->
            return! failwithf "An error has occured"
    }

let calculateAbvCmd readings =
    Cmd.ofPromise calculateAbv readings CompleteCalculate Error

let init result = 
    match result with
    | _ ->
        { GravityReading = { OriginalGravity = 0.0; FinalGravity = 0.0 }
          AbvResult = { StandardAbv = 0.0; AlternateAbv = 0.0; TotalCalories = 0.0 }
          ErrorMsg = "" }

let update (msg:Msg) model: Model*Cmd<Msg> = 
    match msg with 
    | CompleteCalculate results ->
        console.log(results.StandardAbv)
        { model with AbvResult = { model.AbvResult with StandardAbv = results.StandardAbv; AlternateAbv = results.AlternateAbv; TotalCalories = results.TotalCalories } }, Cmd.none
    | ClickCalculate input ->
        console.log(input)
        { model with GravityReading = { model.GravityReading with OriginalGravity = input.OriginalGravity; FinalGravity = input.FinalGravity } }, calculateAbvCmd model.GravityReading
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        nav [ClassName "navbar navbar-expand-lg navbar-dark bg-dark"] [
            div [ClassName "navbar-brand"] [ str ("Beer Architect") ]  
            div [ClassName "navbar-nav"] [
                yield viewLink Page.Home "Home"
                yield viewLink Page.AbvCalculator "ABV Calculator"
            ]]
        div [ClassName "row abv-row"] [
            div [ClassName "col"] []
            div [ClassName "col"] [
                input [
                    Id "originalGravity" 
                    ClassName "form-control"
                    Placeholder "Original Gravity"
                    AutoFocus true
                    Type "number"
                    Step "Any"
                ]
            ]
            div [ClassName "col"] []
        ]
        div [ClassName "row abv-row"] [
            div [ClassName "col"] []
            div [ClassName "col"] [
                input [
                    Id "finalGravity" 
                    ClassName "form-control"
                    Placeholder "Final Gravity"
                    AutoFocus false
                    Type "number"
                    Step "any"
                ]
            ]
            div [ClassName "col"] []
        ]
        div [ClassName "row abv-row"] [
            div [ClassName "col"] []
            div [ClassName "col"] [
                button [
                    Id "calculateAbv"
                    ClassName "btn btn-info btn-lg btn-block"
                    OnClick (fun _ -> dispatch (ClickCalculate { OriginalGravity = 0.0; FinalGravity = 0.0 } ))
                ] [ str "Calculate"]
            ]
            div [ClassName "col"] []
        ]
        div [ClassName "row abv-row"] [
            div [ClassName "col"] []
            div [ClassName "col"
                 Id "StandardAbv"] [
                p [ ClassName "lead" ] [ str "Standard ABV: "]
                p [ ClassName "lead" ] [ str (sprintf "%f" model.AbvResult.StandardAbv)]
            ]
            div [ClassName "col"] []
        ]
        div [ClassName "row abv-row"] [
            div [ClassName "col"] []
            div [ClassName "col"
                 Id "AlternateAbv"] [
                p [ ClassName "lead" ] [ str "Alternate ABV: "]
                p [ ClassName "lead" ] [ str (sprintf "%f" model.AbvResult.AlternateAbv)]
            ]
            div [ClassName "col"] []
        ]
        div [ClassName "row abv-row"] [
            div [ClassName "col"] []
            div [ClassName "col"
                 Id "TotalCalories"] [
                p [ ClassName "lead" ] [ str "Total Calories: "]
                p [ ClassName "lead" ] [ str (sprintf "%f" model.AbvResult.TotalCalories)]
            ]
            div [ClassName "col"] []
        ]
    ]