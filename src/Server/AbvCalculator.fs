module ServerCode.AbvCalculator

open Giraffe
open Microsoft.AspNetCore.Http

open Shared

let Abv og fg = (og - fg) * 131.25

let AlternateAbv og fg = (76.08 * (og - fg) / (1.775 - og)) * (fg / 0.794)

let CalFromAlcohol og fg = 1881.22 * fg * (og - fg)/(1.775 - og)

let CalFromCarbs og fg = 3550.0 * fg * ((0.1808 * og) + (0.819 * fg) - 1.0004)

let TotalCal og fg = CalFromAlcohol og fg + CalFromCarbs og fg

let GetAbvResults gravReadings =
    {
        StandardAbv = Abv gravReadings.OriginalGravity gravReadings.FinalGravity
        AlternateAbv = AlternateAbv gravReadings.OriginalGravity gravReadings.FinalGravity
        TotalCalories = TotalCal gravReadings.OriginalGravity gravReadings.FinalGravity
    } : AbvResult

let calculate : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task {
            let! gravReadings = ctx.BindJsonAsync<GravityReading>()

            let abvResult = GetAbvResults gravReadings
            return! ctx.WriteJsonAsync abvResult
        }