module Client.Pages

open Elmish.Browser.UrlParser

type Page =
    | Home
    | AbvCalculator
    | SrmCalculator
    | IbuCalculator
    | HydrometerTempCalculator
    | AllGrainCalculator
    | YeastProfiles
    | DilutionBoilOffCalculator
    | FermentableProfiles

let toPath =
    function
    | Page.Home -> "/"
    | Page.AbvCalculator -> "/abv-calculator"
    | Page.SrmCalculator -> "/srm-calculator"
    | Page.IbuCalculator -> "/ibu-calculator"
    | Page.HydrometerTempCalculator -> "/hydrometer-temp-calculator"
    | Page.AllGrainCalculator -> "/all-grain-calculator"
    | Page.YeastProfiles -> "/yeast-profiles"
    | Page.DilutionBoilOffCalculator -> "/dilution-boil-off-calculator"
    | Page.FermentableProfiles -> "/fermentables"
    
let pageParser : Parser<Page -> Page,_> =
    oneOf
        [ map Page.Home (s "")
          map Page.AbvCalculator (s "abv-calculator") 
          map Page.SrmCalculator (s "srm-calculator")
          map Page.IbuCalculator (s "ibu-calculator")
          map Page.HydrometerTempCalculator (s "hydrometer-temp-calculator") 
          map Page.AllGrainCalculator (s "all-grain-calculator")
          map Page.YeastProfiles (s "yeast-profiles")
          map Page.DilutionBoilOffCalculator (s "dilution-boil-off-calculator")
          map Page.FermentableProfiles (s "fermentables")]

let urlParser location = parsePath pageParser location