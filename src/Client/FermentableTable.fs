module Client.FermentableTable

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Pages
open Client.Style
open Shared

type Msg = 
    | SetGrainAmount
    | SetGrainId

let fermentableOption fermentable =
    option [Value (sprintf "%i" fermentable.Id) ] [str (sprintf "%s %s" fermentable.Country  fermentable.Name)]

let fermentableTable (model: FermentableTable) = 
    table [ClassName "table table-sm table-striped"] [
        thead [] [
            tr [] [
                th [Scope "col"] [ str "Pounds" ]
                th [Scope "col"] [ str "Grain" ]
            ] ]
        tbody [] [
            tr [] [
                th [] [
                    input [
                        Id "grain-amount-1" 
                        ClassName "form-control"
                        AutoFocus false
                        HTMLAttr.Type "number"
                        Step "any" 
                        ] ]
                th [] [
                    select [
                        Id "grain-1" 
                        ClassName "form-control"
                        AutoFocus false 
                        DefaultValue "0"] [
                        for f in model.FermentableList do yield fermentableOption f ] ] ]
            tr [] [
                th [] [
                    input [
                        Id "grain-amount-2" 
                        ClassName "form-control"
                        AutoFocus false
                        HTMLAttr.Type "number"
                        Step "any"
                        ] ]
                th [] [
                    select [
                        Id "grain-2" 
                        ClassName "form-control"
                        AutoFocus false 
                        DefaultValue "0"] [
                        for f in model.FermentableList do yield fermentableOption f ] ] ]
            tr [] [
                th [] [
                    input [
                        Id "grain-amount-3" 
                        ClassName "form-control"
                        AutoFocus false
                        HTMLAttr.Type "number"
                        Step "any" 
                        ] ]
                th [] [
                    select [
                        Id "grain-3" 
                        ClassName "form-control"
                        AutoFocus false 
                        DefaultValue "0"] [
                        for f in model.FermentableList do yield fermentableOption f ] ] ]
            tr [] [
                th [] [
                    input [
                        Id "grain-amount-4" 
                        ClassName "form-control"
                        AutoFocus false
                        HTMLAttr.Type "number"
                        Step "any"
                        ] ]
                th [] [
                    select [
                        Id "grain-4" 
                        ClassName "form-control"
                        AutoFocus false 
                        DefaultValue "0"] [
                        for f in model.FermentableList do yield fermentableOption f ] ] ]
            tr [] [
                th [] [
                    input [
                        Id "grain-amount-5" 
                        ClassName "form-control"
                        AutoFocus false
                        HTMLAttr.Type "number"
                        Step "any"
                        ] ]
                th [] [
                    select [
                        Id "grain-5" 
                        ClassName "form-control"
                        AutoFocus false 
                        DefaultValue "0"] [
                        for f in model.FermentableList do yield fermentableOption f ] ] ] ] ]