module Client.Home

open Fable.Helpers.React
open Fable.Helpers.React.Props

let view () =
    nav [ClassName  "navbar navbar-expand-lg navbar-dark bg-dark"] [
        div [ClassName "navbar-brand"] [ str ("Beer Architect") ]  
        div [ClassName "navbar-nav"] [
            a [ ClassName "nav-item nav-link active" 
                Href "index.html"] [str "Home"] 
            a [ ClassName "nav-item nav-link" 
                Href "abvCalculator.html"] [str "ABV Calculator"] 
            a [ ClassName "nav-item nav-link" 
                Href "#"] [str "SRM Calculator"] 
        ]]