module ServerCode.IbuCalculator

open System

let functionOfTime time = (1.0 - Math.Pow(Math.E, -0.04 * time)) / 4.15

let functionOfGravity boilGravity = 1.65 * Math.Pow(0.000125, (boilGravity - 1.0))

let alphaAcidUnit ouncesOfHops aAPercentage = 
    ouncesOfHops * aAPercentage

let boilGravity massOfExtract poundPerGallon wortVolume = 
    (massOfExtract * poundPerGallon) / wortVolume

let hopUtilization time boilGravity = functionOfTime time * functionOfGravity boilGravity

let hopBitternessUnitsOzGal ouncesOfHops aAPercentage utilizationPercentage finalVolume = 
    ouncesOfHops * aAPercentage * utilizationPercentage * (75.0 / finalVolume)