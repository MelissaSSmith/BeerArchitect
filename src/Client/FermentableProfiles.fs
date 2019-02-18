module Client.FermentableProfiles

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
    FermentableList: Fermentable list
    ErrorMsg: string
}

type Msg = 
    | RetrievedFermentableList of Fermentable list
    | Done of unit
    | Error of exn

let getFermentableList a = 
    promise {
        let props = 
            [ RequestProperties.Method HttpMethod.GET ]
        
        try
            let! result = Fetch.fetch ServerUrls.APIUrls.GetFermentables props
            let! text = result.text()
            return Decode.Auto.unsafeFromString<Fermentable list> text
        with _ ->
            return! failwithf "An error has occured"
    }

[<Emit("fermentableTableSearchAndSort()")>]
let setUpSearchAndSort a : unit = failwith "JS only"

let init result =
    match result with
    | _ ->
        { FermentableList = list.Empty
          ErrorMsg = "" },
          Cmd.batch [
              Cmd.ofPromise getFermentableList [] RetrievedFermentableList Error
          ]

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
    match msg with
    | RetrievedFermentableList fermentableList ->
        { model with FermentableList = fermentableList }, Cmd.ofFunc setUpSearchAndSort [] Done Error
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none
    | Done unit ->
        model, Cmd.none

type FerementableRowProps =
    { fermentable: Fermentable; }

let fermentableRowComponent { fermentable = fermentable; } =
    tr [] [
        td [ClassName "name"] [ str (sprintf "%s" fermentable.Name) ]
        td [ClassName "country"] [ str (sprintf "%s" fermentable.Country) ]
        td [ClassName "category"] [ str (sprintf "%s" fermentable.Category) ]
        td [ClassName "type"] [ str (sprintf "%s" fermentable.Type) ]
        td [ClassName "degrees-lovibond"] [ str (sprintf "%.0f \u00b0L" fermentable.DegreesLovibond)  ]
        td [ClassName "ppg"] [ str (sprintf "%.2f" fermentable.Ppg) ]
    ]

let inline FermentableRowComponent props = (ofFunction fermentableRowComponent) props []

let getFermentableRows fermentableList = Seq.map (fun f -> FermentableRowComponent {fermentable=f}) fermentableList

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-md-10 ml-sm-auto px-4 beer-body"] [
                    div [ClassName "row beer-row bottom-border"] [ pageHeader "Fermentables" ]
                    div [ClassName "row beer-row"
                         Id "fermentable-table"] [
                        input [ClassName "form-control search"
                               Type "text"
                               Placeholder "Search"]
                        table [ClassName "table table-sm table-striped fermentable-list"] [
                            thead [] [
                                tr[] [
                                    th [ClassName "sort"
                                        DataSort "name"] [ str "Fermentable"]
                                    th [ClassName "sort"
                                        DataSort "country"] [ str "Country"]
                                    th [ClassName "sort"
                                        DataSort "category"] [ str "Category"]
                                    th [ClassName "sort"
                                        DataSort "type"] [ str "Type"]
                                    th [ClassName "sort"
                                        DataSort "degrees-lovibond"] [ str "Color"]
                                    th [ClassName "sort"
                                        DataSort "ppg"] [ str "PPG"]
                                ]
                            ]
                            tbody [ClassName "list"] [
                                for f in model.FermentableList do
                                    yield FermentableRowComponent {
                                        fermentable = f
                                    }
                            ]
                        ]
                        ul [ClassName "pagination mx-auto"] []
                    ]
                ]
            ]
        ]
    ]