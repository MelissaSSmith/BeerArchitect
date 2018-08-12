module Client.Pages

open Elmish.Browser.UrlParser

type Page =
    | Home
    | AbvCalculator
    | SrmCalculator

let toPath =
    function
    | Page.Home -> "/"
    | Page.AbvCalculator -> "/abv-calculator"
    | Page.SrmCalculator -> "/srm-calculator"
    
let pageParser : Parser<Page -> Page,_> =
    oneOf
        [ map Page.Home (s "")
          map Page.AbvCalculator (s "abv-calculator") 
          map Page.SrmCalculator (s "srm-calculator") ]

let urlParser location = parsePath pageParser location