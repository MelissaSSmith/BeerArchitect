module Client.AllGrainCalculator

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.Import.Browser
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

open ServerCode
open Shared
open Client.NavigationMenu
open Client.Style

type Model = {
    FermentableList : Fermentable list
    FermentableTableModel : FermentableTable.Model
    ErrorMsg : string }

type Msg =
    | FermentableTableMsg of FermentableTable.Msg
    | ClickCalculate
    | Error of exn

let init result =
    let subModel, cmd = FermentableTable.init 5
    match result with 
    | _ -> 
        { FermentableList = List.empty
          FermentableTableModel = subModel
          ErrorMsg = "" }, 
            Cmd.batch [
                Cmd.map FermentableTableMsg cmd ]

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
    match msg with
    | FermentableTableMsg msg ->
        match msg with 
        | FermentableTable.Msg.SetGrainAmount (inputId, grainAmount)->
            model, Cmd.none
        | FermentableTable.Msg.SetGrainId (inputId, grainId) ->
            model, Cmd.none
        | _ ->
            let submodel, cmd = FermentableTable.update msg model.FermentableTableModel
            { model with FermentableTableModel = submodel}, Cmd.map FermentableTableMsg cmd
    | Error exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none
    | _ ->
        model, Cmd.none

let view model (dispatch: Msg -> unit) =
    div [] [
        navigationBar
        div [ClassName "container-fluid"] [
            div [ClassName "row"] [
                sidebarNavigationMenu
                div [ClassName "col-md-10 ml-sm-auto col-lg-10 px-4 beer-body"] [
                    div [ClassName "row beer-row bottom-border"] [ pageHeader "All Grain OG, FG, ABV Calculator"]
                    div [ClassName "row beer-row"] [
                        div [ClassName "col-10"] [
                            div [ClassName "row beer-row justify-content-start"] [
                                FermentableTable.view model.FermentableTableModel (dispatch << FermentableTableMsg)
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]