module Client.HydrometerTempCalculator

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.Import.Browser
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

open ServerCode
open Shared
open Client.NavigationMenu
open Client.Style

type Model = {
    HydrometerAdjustInput : HydrometerAdjustInput
    HydrometerAdjustResult : HydrometerAdjustResult
    ErrorMsg : string }

type Msg =
    | ChangeEvent
    | SetMeasuredGravity of float
    | SetTemperatureReading of float
    | SetCalibratedTemperature of float
    | SetTemperatureUnit of string
    | CompleteCalculation of HydrometerAdjustResult 
    | ClickCalculate
    | Error of exn

let changeTemperatureUnit selectedUnit =
    match selectedUnit with
    | "f" -> Farenheit
    | _ -> Celsius

let farenheitToCelcius temp =
    (temp - 32.0) * (5.0/9.0)

let celciusToFarenheit temp =
    (temp * 9.0/5.0) + 32.0

let convertTemp temp unit =
    match unit with
    | Farenheit -> celciusToFarenheit temp
    | Celsius -> farenheitToCelcius temp

let calculateHydrometerAdjustment (input:HydrometerAdjustInput) = 
    promise {
        let body = toJson input

        let props =
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [
                  HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body ]

        try
            return! Fetch.fetchAs<HydrometerAdjustResult> ServerUrls.APIUrls.CalculateHydrometerAdjustment props
        with _ ->
            return! failwithf "An error has occured"
    }

let calculateHydrometerAdustmentCmd input =
    Cmd.ofPromise calculateHydrometerAdjustment input CompleteCalculation Error

let init result =
    match result with 
    | _ -> 
        { HydrometerAdjustInput = { MeasuredGravity = 0.0; TemperatureReading = 70.0; CalibrationTemperature = 68.0; TemperatureUnit = Farenheit }
          HydrometerAdjustResult = { CorrectedGravity = 0.0 } 
          ErrorMsg = "" }, Cmd.none

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
    match msg with
    | SetMeasuredGravity measuredGravity ->
        { model with HydrometerAdjustInput = { model.HydrometerAdjustInput with MeasuredGravity = measuredGravity } }, Cmd.none
    | SetTemperatureReading temperatureReading ->
        { model with HydrometerAdjustInput = { model.HydrometerAdjustInput with TemperatureReading = temperatureReading } }, Cmd.none
    | SetCalibratedTemperature calibratedTemp ->
        { model with HydrometerAdjustInput = { model.HydrometerAdjustInput with CalibrationTemperature = calibratedTemp } }, Cmd.none
    | SetTemperatureUnit selectedUnit ->
        let unit = changeTemperatureUnit selectedUnit
        let calibratedTemp = convertTemp model.HydrometerAdjustInput.CalibrationTemperature unit
        let tempReading = convertTemp model.HydrometerAdjustInput.TemperatureReading unit
        document.getElementById("calibration")?value <- sprintf "%.0f" calibratedTemp
        document.getElementById("temperature")?value <- sprintf "%.0f" tempReading
        { model with HydrometerAdjustInput = { model.HydrometerAdjustInput with TemperatureUnit = unit; CalibrationTemperature = calibratedTemp; TemperatureReading = tempReading } }, Cmd.none
    | CompleteCalculation result ->
        { model with HydrometerAdjustResult = { model.HydrometerAdjustResult with CorrectedGravity = result.CorrectedGravity } }, Cmd.none
    | ClickCalculate ->
        model, calculateHydrometerAdustmentCmd model.HydrometerAdjustInput
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
                    div [ClassName "row beer-row bottom-border"] [ pageHeader "Hydrometer Temperature Adjustment Calculator"]
                    div [ClassName "row beer-row"] [
                        div [ClassName "col-8"] [
                            label [] [ str ("Temperature Units") ]
                            select [
                                Id "temperature-unit" 
                                ClassName "form-control"
                                AutoFocus false 
                                HTMLAttr.EncType "utf-8"
                                OnChange (fun ev -> dispatch (SetTemperatureUnit !!ev.target?value) ) 
                                DefaultValue "1"] [
                                option [ Value "f" ] [ str "Farenheit"]
                                option [ Value "c" ] [ str "Celcius"]  ]
                        ] ]
                    div [ClassName "row beer-row"] [
                        div [ClassName "col-8"] [
                            label [] [ str ("Hydrometer Reading")]
                            input [
                                Id "measured-gravity" 
                                ClassName "form-control"
                                AutoFocus true
                                HTMLAttr.Type "number"
                                Step "any"
                                OnChange (fun ev -> dispatch (SetMeasuredGravity !!ev.target?value))
                            ] ] ]
                    div [ClassName "row beer-row"] [
                        div [ClassName "col-8"] [
                            label [] [ str ("Temperature")]
                            input [
                                Id "temperature" 
                                ClassName "form-control"
                                AutoFocus false
                                HTMLAttr.Type "number"
                                Step "any"
                                DefaultValue "70"
                                OnChange (fun ev -> dispatch (SetTemperatureReading !!ev.target?value))
                            ] ] ]
                    div [ClassName "row beer-row"] [
                        div [ClassName "col-8"] [
                            label [] [ str ("Calibration")]
                            input [
                                Id "calibration" 
                                ClassName "form-control"
                                AutoFocus false
                                HTMLAttr.Type "number"
                                Step "any"
                                DefaultValue "68"
                                OnChange (fun ev -> dispatch (SetCalibratedTemperature !!ev.target?value))
                            ] ] ]
                    div [ClassName "row beer-row"] [
                        div [ClassName "col-8"] [
                            button [
                                Type "button"
                                Id "calculate-adjustment"
                                ClassName "btn btn-info btn-lg btn-block"
                                OnClick (fun _ -> dispatch ClickCalculate)
                            ] [ str "Calculate"] ] ]
                    div [ClassName "row beer-row"] [
                        div [ClassName "col-8"
                             Id "adjusted-reading"] [
                            p [ClassName "results"] [ str (sprintf "Adjusted Hydrometer Reading: %.3f" model.HydrometerAdjustResult.CorrectedGravity) ]
                        ] ]
                ]
            ]
        ]
    ]