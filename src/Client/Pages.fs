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

let toPath =
    function
    | Page.Home -> "/"
    | Page.AbvCalculator -> "/abv-calculator"
    | Page.SrmCalculator -> "/srm-calculator"
    | Page.IbuCalculator -> "/ibu-calculator"
    | Page.HydrometerTempCalculator -> "/hydrometer-temp-calculator"
    | Page.AllGrainCalculator -> "/all-grain-calculator"
    | Page.YeastProfiles -> "/yeast-profiles"
    
let pageParser : Parser<Page -> Page,_> =
    oneOf
        [ map Page.Home (s "")
          map Page.AbvCalculator (s "abv-calculator") 
          map Page.SrmCalculator (s "srm-calculator")
          map Page.IbuCalculator (s "ibu-calculator")
          map Page.HydrometerTempCalculator (s "hydrometer-temp-calculator") 
          map Page.AllGrainCalculator (s "all-grain-calculator")
          map Page.YeastProfiles (s "yeast-profiles")]

let urlParser location = parsePath pageParser location