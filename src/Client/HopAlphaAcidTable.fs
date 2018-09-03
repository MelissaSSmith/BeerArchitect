module Client.HopAlphaAcidTable

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Style

let hopAlphaAcidTable =
    table [Id "hop-alpha-acid-table"
           ClassName "table table-sm"] [
        thead [] [
            tr [] [
                th [Scope "col"] [ str "Hops" ]
                th [Scope "col"] [ str "Average Alpha Acids" ]
            ] ]
        tbody [] [

        ] ]

let hopAlphaAcidModal =
    div [ClassName "modal fade"
         Id "hop-alpha-acid-modal"
         TabIndex -1.0
         Role "dialog"
         AriaHidden "true"] [
        div [ClassName "modal-dialog"
             Role "document"] [
            div [ClassName "modal-content"] [
                div [ClassName "modal-header"] [
                    button [
                        Type "button"
                        ClassName "close"
                        DataDismiss "modal"
                        AriaLabel "Close" ] [
                        span [AriaHidden "true"] [ str "x" ] ] ]
                div [ClassName "modal-body"] [ hopAlphaAcidTable ]
                div [ClassName "modal-footer"] [
                    button [
                        Type "button"
                        Class "btn btn-secondary"
                        DataDismiss "modal"
                    ] [ str "Close" ] ]
            ] ] ]