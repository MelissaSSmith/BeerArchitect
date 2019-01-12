module Client.FermentableTable

open Elmish
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.Import.React
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

open ServerCode
open Shared

type Model = {
    TableSize : int
    FermentableList : Fermentable list
    ErrorMsg : string }

type Msg =
    | FillInFermentableList of Fermentable list
    | SetGrainAmount of int*float
    | SetGrainId of int*int
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

let fermentableOption fermentable =
    option [Value (sprintf "%i" fermentable.Id) ] [str (sprintf "%s %s" fermentable.Country  fermentable.Name)]
    
let getFermentableListCmd =
    Cmd.ofPromise getFermentableList 0 FillInFermentableList Error

let init (tableSize:int) =
    { TableSize = tableSize; FermentableList = list.Empty; ErrorMsg = "" }, getFermentableListCmd

let update (msg:Msg) (model:Model): Model*Cmd<Msg> =
    match msg with
    | FillInFermentableList fermentableList ->
        console.log(fermentableList)
        { model with FermentableList = fermentableList }, Cmd.none
    | Error exn ->
        { model with ErrorMsg = string (exn.Message); FermentableList = list.Empty }, Cmd.none
    | _ ->
        model, Cmd.none

type FermentableRowProps = 
    { rowId: int; 
      fermentableList: Fermentable list; 
      setGrainAmount: FormEvent -> unit; 
      setGrainId: FormEvent -> unit }

let fermentableRowComponent { rowId = rowId; fermentableList = fermentableList; setGrainAmount = setGrainAmount; setGrainId = setGrainId } =
    tr [] [
        th [] [
            input [
                Id (sprintf "grain-amount-%i" rowId)
                ClassName "form-control"
                AutoFocus false
                HTMLAttr.Type "number"
                Step "any" 
                OnChange setGrainAmount ] ]
        th [] [
            select [
                Id (sprintf "grain-%i" rowId)
                ClassName "form-control"
                AutoFocus false 
                OnChange setGrainId
                DefaultValue "0"] [
                for f in fermentableList do yield fermentableOption f ] ] ]

let inline FermentableRowComponent props = (ofFunction fermentableRowComponent) props []

let view (model:Model) dispatch =
    table [ClassName "table table-sm table-striped"] [
        thead [] [
            tr [] [
                th [Scope "col"] [ str "Pounds" ]
                th [Scope "col"] [ str "Grain" ]
            ] ]
        tbody [] [
            for i in [1..model.TableSize] do 
                yield FermentableRowComponent {
                    rowId = i
                    fermentableList = model.FermentableList
                    setGrainAmount = (fun ev -> dispatch (SetGrainAmount (i, !!ev.target?value) ) )
                    setGrainId = (fun ev -> dispatch (SetGrainId (i, !!ev.target?value) ) )
                }
        ] ]