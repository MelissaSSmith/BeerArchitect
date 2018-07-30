namespace Shared

type Fermentable = 
    { Id: int
      Name: string
      Country: string
      Category: string
      Type: string
      DegreesLovibond: int
      Ppg: int }

type SrmHex =
    { SrmKey: int
      HexValue: string}