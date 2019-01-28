module Client.YeastProfiles

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.Import.Browser
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
        { model with YeastList = yeastList }, Cmd.none
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none

type YeastRowProps =
    { yeast: Yeast; }

let yeastRowComponent { yeast = yeast; } =
    tr [] [
        th [Scope "col"] [ str (sprintf "%s" yeast.Company) ]
        th [Scope "col"] [ str (sprintf "%s" yeast.YeastId) ]
        th [Scope "col"] [ str (sprintf "%s" yeast.Name) ]
        th [Scope "col"] [ str (sprintf "%.0f-%.0f%%" yeast.LowAttenuation yeast.HighAttenuation) ]
        th [Scope "col"] [ str (sprintf "%s" yeast.Flocculation) ]
        th [Scope "col"] [ str (sprintf "%.0f-%.0f F" yeast.LowTemp yeast.HighTemp) ]
        th [Scope "col"] [ str (sprintf "%s" yeast.AlcoholTolerance) ]
        th [Scope "col"] [ str (sprintf "%s" yeast.Type) ]
        th [Scope "col"] [ str (sprintf "%s" yeast.YeastFormat) ] ]

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
                    div [ClassName "row beer-row"] [
                        table [ClassName "table table-sm table-striped"] [
                            thead [] [
                                tr [] [
                                    th [Scope "col"] [ str "Company" ]
                                    th [Scope "col"] [ str "Id" ]
                                    th [Scope "col"] [ str "Name" ]
                                    th [Scope "col"] [ str "Attenuation" ]
                                    th [Scope "col"] [ str "Flocculation" ]
                                    th [Scope "col"] [ str "Optimum Temp" ]
                                    th [Scope "col"] [ str "Alcohol Tolerance" ]
                                    th [Scope "col"] [ str "Type" ]
                                    th [Scope "col"] [ str "Yeast Format" ]
                                ]
                            ]
                            tbody [] [
                                for y in model.YeastList do
                                    yield YeastRowComponent {
                                        yeast = y
                                    }
                            ]
                        ]
                    ]
                    ] ] ] ]