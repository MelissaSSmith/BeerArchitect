module Client.Pages

open Elmish.Browser.UrlParser

type Page =
    | Home
    | AbvCalculator

let toPath =
    function
    | Page.Home -> "/"
    | Page.AbvCalculator -> "/abv-calculator"
    
let pageParser : Parser<Page -> Page,_> =
    oneOf
        [ map Page.Home (s "")
          map Page.AbvCalculator (s "abv-calculator") ]

let urlParser location = parsePath pageParser location