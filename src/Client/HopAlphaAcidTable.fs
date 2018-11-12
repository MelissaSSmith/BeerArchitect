module Client.HopAlphaAcidTable

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Client.Style
open Shared

let hopAlphaAcidRow hopAlphaAcid =
    tr [] [
        th [] [ str hopAlphaAcid.Hops ]
        th [] [ str (sprintf "%.1f" hopAlphaAcid.AverageAlphaAcids) ]
    ]

let hopAlphaAcidTable hopAlphaAcidList =
    table [Id "hop-alpha-acid-table"
           ClassName "table table-sm"] [
        thead [] [
            tr [] [
                th [Scope "col"] [ str "Hops" ]
                th [Scope "col"] [ str "Average Alpha Acids" ]
            ] ]
        tbody [] [
            for h in hopAlphaAcidList do yield hopAlphaAcidRow h
        ] ]

let hopAlphaAcidModal hopAlphaAcidList =
    div [ClassName "modal fade"
         Id "hop-alpha-acid-modal"
         TabIndex -1
         Role "dialog"
         AriaHidden "true"] [
        div [ClassName "modal-dialog"
             Role "document"] [
            div [ClassName "modal-content"] [
                div [ClassName "modal-header reference-modal-header"] [
                    button [
                        Type "button"
                        ClassName "close"
                        DataDismiss "modal"
                        AriaLabel "Close" ] [
                        span [AriaHidden "true"] [ str "x" ] ] ]
                div [ClassName "modal-body"] [ hopAlphaAcidTable hopAlphaAcidList ]
                div [ClassName "modal-footer reference-modal-footer"] [
                    button [
                        Type "button"
                        Class "btn btn-secondary"
                        DataDismiss "modal"
                    ] [ str "Close" ] ]
            ] ] ]