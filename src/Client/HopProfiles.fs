module Client.HopProfiles

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
    HopList: Hop list
    ErrorMsg: string
}

type Msg =
    | RetrievedHopList of Hop list
    | Done of unit
    | Error of exn

let getHopList a = 
    promise {
        let props = 
            [ RequestProperties.Method HttpMethod.GET ]
        
        try
            let! result = Fetch.fetch ServerUrls.APIUrls.GetHops props
            let! text = result.text()
            return Decode.Auto.unsafeFromString<Hop list> text
        with _ ->
            return! failwithf "An error has occured"
    }

[<Emit("hopTableSearchAndSort()")>]
let setUpSearchAndSort a : unit = failwith "JS only"

let init result =
    match result with
    | _ ->
        { HopList = list.Empty
          ErrorMsg = "" },
          Cmd.batch [
              Cmd.ofPromise getHopList [] RetrievedHopList Error
          ]

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
    match msg with
    | RetrievedHopList hopList ->
        { model with HopList = hopList }, Cmd.ofFunc setUpSearchAndSort [] Done Error
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none
    | Done unit ->
        model, Cmd.none

type HopRowProps =
    { hop: Hop; }

let hopRowComponent { hop = hop; } =
    tr [] [
        td [ClassName "name"] [ str (sprintf "%s" hop.Name) ]
        td [ClassName "use"] [ str (sprintf "%s" hop.Use) ]
        td [ClassName "country"] [ str (sprintf "%s" hop.Country) ]
        td [ClassName "alpha-acids"] [ str (sprintf "%.1f-%.1f%%" hop.AlphaAcidsLow hop.AlphaAcidsHigh) ]
        td [ClassName "beta-acids"] [ str (sprintf "%.1f-%.1f%%" hop.BetaAcidsLow hop.BetaAcidsHigh)  ]
    ]

let inline HopRowComponent props = (ofFunction hopRowComponent) props []

let getHopRows hopList = Seq.map (fun h -> HopRowComponent {hop=h}) hopList

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-md-10 ml-sm-auto px-4 beer-body"] [
                    div [ClassName "row beer-row bottom-border"] [ pageHeader "Hops" ]
                    div [ClassName "row beer-row"
                         Id "hop-table"] [
                        input [ClassName "form-control search"
                               Type "text"
                               Placeholder "Search"]
                        table [ClassName "table table-sm table-striped hop-list"] [
                            thead [] [
                                tr [] [
                                    th [ClassName "sort"
                                        DataSort "name"] [ str "Hop" ]
                                    th [ClassName "sort"
                                        DataSort "use"] [ str "Purpose" ]
                                    th [ClassName "sort"
                                        DataSort "country"] [ str "Country" ]
                                    th [ClassName "sort"
                                        DataSort "alpha-acids"] [ str "Alpha Acid Range" ]
                                    th [ClassName "sort"
                                        DataSort "beta-acids"] [ str "Beta Acid Range" ]
                                ]
                            ]
                            tbody [ClassName "list"] [
                                for h in model.HopList do
                                    yield HopRowComponent {
                                        hop = h
                                    }
                            ]
                        ]
                        ul [ClassName "pagination mx-auto"] []
                    ]
                ]
            ]
        ]
    ]