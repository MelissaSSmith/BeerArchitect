module ServerCode.HydrometerTempCalculator

open Giraffe
open Microsoft.AspNetCore.Http

open Shared

let tempAdjustment temp =
    1.00130346 - 0.000134722124 * temp + 0.00000204052596 * temp - 0.00000000232820948 * temp

let hydrometerAdjustment measuredGravity readingTemp calibrationTemp =
    measuredGravity * ((tempAdjustment(readingTemp)) - (tempAdjustment(calibrationTemp)))

let GetAdjustmentResult correctedGravity = 
    {
        CorrectedGravity = correctedGravity
    } : HydrometerAdjustResult

let calculate : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! hydrometerAdjustInput = ctx.BindJsonAsync<HydrometerAdjustInput>()

            let adjustment = hydrometerAdjustment hydrometerAdjustInput.MeasuredGravity hydrometerAdjustInput.TemperatureReading hydrometerAdjustInput.CalibrationTemperature
            let correctedGravity = hydrometerAdjustInput.MeasuredGravity - adjustment

            let result = GetAdjustmentResult correctedGravity
            return! ctx.WriteJsonAsync result
        }