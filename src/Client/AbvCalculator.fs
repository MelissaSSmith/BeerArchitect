module Client.AbvCalculator

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
    GravityReading : GravityReading
    AbvResult : AbvResult
    ErrorMsg : string }

type Msg =
    | CompleteCalculate of AbvResult
    | SetOriginalGravity of float
    | SetFinalGravity of float
    | ClickCalculate
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

let update (msg:Msg) (model:Model): Model*Cmd<Msg> = 
    match msg with 
    | CompleteCalculate results ->
        { model with AbvResult = { model.AbvResult with StandardAbv = results.StandardAbv; AlternateAbv = results.AlternateAbv; TotalCalories = results.TotalCalories } }, Cmd.none
    | SetOriginalGravity originalGravity ->
        { model with GravityReading = { model.GravityReading with OriginalGravity = originalGravity } }, Cmd.none
    | SetFinalGravity finalGravity ->
        { model with GravityReading = { model.GravityReading with FinalGravity = finalGravity } }, Cmd.none
    | ClickCalculate ->
        model, calculateAbvCmd model.GravityReading
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-md-10 ml-sm-auto col-lg-10 px-4 beer-body"] [
                    div [ClassName "row beer-row bottom-border"] [ pageHeader "ABV Calculator" ]
                    div [ClassName "row beer-row justify-content-start"] [
                        div [ClassName "col-8"] [
                            label [] [str ("Original Gravity")]
                            input [
                                Id "originalGravity" 
                                ClassName "form-control"
                                AutoFocus true
                                HTMLAttr.Type "number"
                                Step "any"
                                OnChange (fun ev -> dispatch (SetOriginalGravity !!ev.target?value))
                            ]]]
                    div [ClassName "row beer-row justify-content-start"] [
                        div [ClassName "col-8"] [
                            label [] [str ("Final Gravity")]
                            input [
                                Id "finalGravity" 
                                ClassName "form-control"
                                AutoFocus false
                                HTMLAttr.Type "number"
                                Step "any"
                                OnChange (fun ev -> dispatch (SetFinalGravity !!ev.target?value))
                            ]]]
                    div [ClassName "row beer-row justify-content-start"] [
                        div [ClassName "col-8"] [
                            button [
                                Type "button"
                                Id "calculateAbv"
                                ClassName "btn btn-info btn-lg btn-block"
                                OnClick (fun _ -> dispatch ClickCalculate)
                            ] [ str "Calculate"]
                        ]]
                    div [ClassName "row beer-row justify-content-start"] [
                        div [ClassName "col-8"
                             Id "StandardAbv"] [
                            p [ ClassName "results" ] [ str (sprintf "Standard ABV:  %.2f %%" model.AbvResult.StandardAbv)]
                        ]]
                    div [ClassName "row beer-row justify-content-start"] [
                        div [ClassName "col-8"
                             Id "AlternateAbv"] [
                            p [ ClassName "results" ] [ str (sprintf "Alternate ABV:  %.2f %%" model.AbvResult.AlternateAbv)]
                        ]]
                    div [ClassName "row beer-row justify-content-start"] [
                        div [ClassName "col-8"
                             Id "TotalCalories"] [
                            p [ ClassName "results" ] [ str (sprintf "Total Calories:  %.2f per 12 oz." model.AbvResult.TotalCalories)]
                        ]]]]]]