module Client.HopProfile

open Elmish
open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

open ServerCode
open Shared
open Client.NavigationMenu
open Client.Style

type Model = {
    Hop: Hop
    ErrorMsg: string
}

type Msg =
    | RetrievedHop of Hop
    | Error of exn

let getHop a =
    promise {
        let props = 
            [ RequestProperties.Method HttpMethod.GET ]
        
        try
            let! result = Fetch.fetch ServerUrls.APIUrls.GetHop props
            let! text = result.text()
            return Decode.Auto.unsafeFromString<Hop> text
        with _ ->
            return! failwithf "An error has occured"
    }

let initializeHop urlId =
    { UrlId = urlId
      Name = ""
      Characteristics = ""
      Use = ""
      AlphaAcidsLow = 0.0
      AlphaAcidsHigh = 0.0
      BetaAcidsLow = 0.0
      BetaAcidsHigh = 0.0
      CoHumuloneComposition = ""
      Country = ""
      TotalOilComposition = ""
      MyrceneOilComposition = ""
      HumuleneOilComposition = ""
      CaryophylleneOil = ""
      FarneseneOil = ""
      Substitutes = list.Empty
      BeerStyles = list.Empty}

let init urlId =
    { Hop = initializeHop urlId
      ErrorMsg = "None" },
      Cmd.batch [
          Cmd.ofPromise getHop [urlId] RetrievedHop Error
      ]
    

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
    match msg with
    | RetrievedHop hop ->
        { model with Hop = hop }, Cmd.none
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-md-10 ml-sm-auto px-4 beer-body"] [
                    div [ClassName "row beer-row bottom-border"] [ pageHeader model.Hop.Name ]
                ]
            ]
        ]
    ]