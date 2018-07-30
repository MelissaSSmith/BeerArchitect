module SrmCalculator

open System

let Power power b =
    Math.Pow(b, power)

let MultiplyByConstant variable constant = 
    variable * constant

let Mcu grainColor grainWeightLbs volGal  =
    (grainColor * grainWeightLbs) / volGal

let SrmColor grainColorList grainWeightLbsList volGal =
    List.zip grainWeightLbsList grainColorList
    |> List.sumBy (fun (a, dl) -> Mcu dl a volGal)
    |> Power 0.6859
    |> MultiplyByConstant 1.4922

let Ebc srm = 
    MultiplyByConstant srm 1.97