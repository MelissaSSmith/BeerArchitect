module ServerCode.GravityEquations

let getGravPoints specificGrav = 
    (specificGrav - 1.0) * 1000.0

let getSpecificGravity gravPoints =
    (gravPoints / 1000.0) + 1.0