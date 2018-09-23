module Client.Pages

open Elmish.Browser.UrlParser

type Page =
    | Home
    | AbvCalculator
    | SrmCalculator
    | IbuCalculator
    | HydrometerTempCalculator
    | AllGrainCalculator

let toPath =
    function
    | Page.Home -> "/"
    | Page.AbvCalculator -> "/abv-calculator"
    | Page.SrmCalculator -> "/srm-calculator"
    | Page.IbuCalculator -> "/ibu-calculator"
    | Page.HydrometerTempCalculator -> "/hydrometer-temp-calculator"
    | Page.AllGrainCalculator -> "/allgrain-ogfg-calculator"
    
let pageParser : Parser<Page -> Page,_> =
    oneOf
        [ map Page.Home (s "")
          map Page.AbvCalculator (s "abv-calculator") 
          map Page.SrmCalculator (s "srm-calculator")
          map Page.IbuCalculator (s "ibu-calculator")
          map Page.HydrometerTempCalculator (s "hydrometer-temp-calculator") 
          map Page.AllGrainCalculator (s "allgrain-ogfg-calculator")]

let urlParser location = parsePath pageParser location