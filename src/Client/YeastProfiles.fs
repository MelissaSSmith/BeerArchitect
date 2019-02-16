module Client.YeastProfiles

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
    YeastList: Yeast list
    ErrorMsg: string }

type Msg =
    | RetrievedYeastList of Yeast list
    | Done of unit
    | Error of exn

let getYeastList a =
    promise {
        let props = 
            [ RequestProperties.Method HttpMethod.GET ]
        
        try
            let! result = Fetch.fetch ServerUrls.APIUrls.GetYeasts props
            let! text = result.text()
            return Decode.Auto.unsafeFromString<Yeast list> text
        with _ ->
            return! failwithf "An error has occured"
    }

[<Emit("yeastTableSearchAndSort($0)")>]
let setUpSearchAndSort yeastList : unit = failwith "JS only"

let init result =
    match result with
    | _ ->
        { YeastList = list.Empty
          ErrorMsg = "" },
          Cmd.batch [
              Cmd.ofPromise getYeastList [] RetrievedYeastList Error
          ]

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
    match msg with
    | RetrievedYeastList yeastList ->
        let jsonYeastList = Encode.Auto.toString(0, yeastList)
        { model with YeastList = yeastList }, Cmd.ofFunc setUpSearchAndSort jsonYeastList Done Error
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none
    | Done unit ->
        model, Cmd.none

type YeastRowProps =
    { yeast: Yeast; }

let yeastRowComponent { yeast = yeast; } =
    tr [] [
        td [ClassName "company"] [ str (sprintf "%s" yeast.Company) ]
        td [ClassName "yeastId"] [ str (sprintf "%s" yeast.YeastId) ]
        td [ClassName "name"] [ str (sprintf "%s" yeast.Name) ]
        td [Scope "col"] [ str (sprintf "%.0f-%.0f%%" yeast.LowAttenuation yeast.HighAttenuation) ]
        td [Scope "col"] [ str (sprintf "%s" yeast.Flocculation) ]
        td [Scope "col"] [ str (sprintf "%.0f-%.0f F" yeast.LowTemp yeast.HighTemp) ]
        td [Scope "col"] [ str (sprintf "%s" yeast.AlcoholTolerance) ]
        td [ClassName "type"] [ str (sprintf "%s" yeast.Type) ]
        td [ClassName "format"] [ str (sprintf "%s" yeast.YeastFormat) ] ]

let inline YeastRowComponent props = (ofFunction yeastRowComponent) props []

let getYeastRows yeastList = Seq.map (fun y -> YeastRowComponent {yeast = y}) yeastList

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-md-10 ml-sm-auto col-lg-10 px-4 beer-body"] [
                    div [ClassName "row beer-row bottom-border"] [ pageHeader "Yeast Profiles"] 
                    div [ClassName "row beer-row" 
                         Id "yeast-profile-table"] [
                        input [ClassName "form-control search"
                               Type "text"
                               Placeholder "Search"]
                        table [ClassName "table table-sm table-striped yeast-list"] [
                            thead [] [
                                tr [] [
                                    th [ClassName "sort"
                                        DataSort "company"] [ str "Company"]
                                    th [ClassName "sort"
                                        DataSort "yeastId"] [ str "Id" ]
                                    th [ClassName "sort"
                                        DataSort "name"] [ str "Name" ]
                                    th [Scope "col"] [ str "Attenuation" ]
                                    th [Scope "col"] [ str "Flocculation" ]
                                    th [Scope "col"] [ str "Optimum Temp" ]
                                    th [Scope "col"] [ str "Alcohol Tolerance" ]
                                    th [ClassName "sort"
                                        DataSort "type"] [ str "Type" ]
                                    th [ClassName "sort"
                                        DataSort "format"] [ str "Yeast Format" ]
                                ]
                            ]
                            tbody [ClassName "list"] [
                                for y in model.YeastList do
                                    yield YeastRowComponent {
                                        yeast = y
                                    }
                            ]
                        ]
                        ul [ClassName "pagination mx-auto"] []
                    ]
                    ] ] ] ]