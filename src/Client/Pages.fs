module Client.Pages

open Elmish.Browser.UrlParser

type Page =
    | Home

let toPath =
    function
    | Page.Home -> "/"

let pageParser : Parser<Page -> Page,_> =
    oneOf
        [ map Page.Home (s "") ]

let urlParser location = parsePath pageParser location