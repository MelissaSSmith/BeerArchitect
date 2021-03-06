namespace Shared

type TemperatureUnits =
    | Farenheit
    | Celsius

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
      HexValue: string }

type SrmResult = 
    { Srm: float
      Ebc: float
      HexColor: string }

type SrmInput = 
    { BatchSize: float 
      GrainIds: int list
      GrainAmounts: float list
      GrainBill: list<float*int> }

type HopType =
    | Pellet
    | Whole

type HopIbuInput = 
    { Ounces: float
      AlphaAcids: float
      BoilTime: float
      Type: int }

type IbuInput = 
    { BoilSize: float
      BatchSize: float
      TargetOriginalGravity: float
      HopIbuInputs: HopIbuInput list }

type HopIbuResult =
    { Utilization: float
      Ibus: float }

type IbuResult = 
    { EstimatedBoilGravity: float
      TotalIbu: float
      HopIbuResults: HopIbuResult list }

type HopAlphaAcid = 
    { Hops: string
      AverageAlphaAcids: float }

type Hop = 
    { Name: string
      UrlId: string
      Characteristics: string
      Use: string
      AlphaAcidsLow: float
      AlphaAcidsHigh: float
      BetaAcidsLow: float
      BetaAcidsHigh: float
      CoHumuloneComposition: string
      Country: string
      TotalOilComposition: string
      MyrceneOilComposition: string
      HumuleneOilComposition: string
      CaryophylleneOil: string
      FarneseneOil: string
      Substitutes: string list
      BeerStyles: string list }

type HydrometerAdjustInput =
    { MeasuredGravity: float
      TemperatureReading: float
      CalibrationTemperature: float
      TemperatureUnit: TemperatureUnits }

type HydrometerAdjustResult = 
    { CorrectedGravity: float }

type YeastTolerance =
    | Low
    | Medium
    | High

type AllGrainInput =
    { PreBoilSize: float
      BatchSize: float
      Effciency: float
      YeastTolerance: YeastTolerance
      GrainIds: int list
      GrainAmounts: float list
      GrainBill: list<float*int> }

type AllGrainResult =
    { EstPreBoilOG: float
      EstOG: float
      EstFG: float
      EstABV: float }

type YeastBrand =
    | Wyeast
    | WhiteLabs
    | Safale
    | DanstarLallemand
    | Imperial

type Yeast = 
    { Name: string
      YeastId: string
      LowAttenuation: float
      HighAttenuation: float
      Flocculation: string
      LowTemp: float
      HighTemp: float
      AlcoholTolerance: string
      Type: string
      YeastFormat: string
      Company: string }

type DilutionBoilOffInput =
    { NewVolWortVol: float
      NewVolCurrGrav: float
      DesiredGravity: float
      NewGravWortVol: float
      NewGravCurrGrav: float
      TargetVolume: float }

type DilutionBoilOffResult =
    { NewVolume: float
      NewVolumeDiff: float
      NewGravity: float
      NewGravityDiff: float }