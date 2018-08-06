namespace Shared

type GravityReading = 
    { OriginalGravity: float
      FinalGravity: float }

type AbvResult = 
    { StandardAbv: float
      AlternateAbv: float
      TotalCalories: float }

type Fermentable = 
    { Id: int
      Name: string
      Country: string
      Category: string
      Type: string
      DegreesLovibond: float
      Ppg: float }

type SrmHex =
    { SrmKey: int
      HexValue: string}