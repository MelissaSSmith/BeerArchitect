module Client.FermentableTable

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import.React
open Shared

let fermentableOption fermentable =
    option [Value (sprintf "%i" fermentable.Id) ] [str (sprintf "%s %s" fermentable.Country  fermentable.Name)]

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