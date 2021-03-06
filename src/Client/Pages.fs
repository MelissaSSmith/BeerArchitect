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
    | HopProfiles
    | HopProfile of string

let toHash =
    function
    | Page.Home -> "#/"
    | Page.AbvCalculator -> "#/abv-calculator"
    | Page.SrmCalculator -> "#/srm-calculator"
    | Page.IbuCalculator -> "#/ibu-calculator"
    | Page.HydrometerTempCalculator -> "#/hydrometer-temp-calculator"
    | Page.AllGrainCalculator -> "#/all-grain-calculator"
    | Page.YeastProfiles -> "#/yeast-profiles"
    | Page.DilutionBoilOffCalculator -> "#/dilution-boil-off-calculator"
    | Page.FermentableProfiles -> "#/fermentables"
    | Page.HopProfiles -> "#/hops"
    | Page.HopProfile s -> "#/hops/" + s
    
let pageParser : Parser<Page -> Page,Page> =
    oneOf
        [ map Page.Home (s "")
          map Page.AbvCalculator (s "abv-calculator") 
          map Page.SrmCalculator (s "srm-calculator")
          map Page.IbuCalculator (s "ibu-calculator")
          map Page.HydrometerTempCalculator (s "hydrometer-temp-calculator") 
          map Page.AllGrainCalculator (s "all-grain-calculator")
          map Page.YeastProfiles (s "yeast-profiles")
          map Page.DilutionBoilOffCalculator (s "dilution-boil-off-calculator")
          map Page.FermentableProfiles (s "fermentables")
          map Page.HopProfiles (s "hops")
          map Page.HopProfile (s "hops" </> str)]

let urlParser location = parsePath pageParser location