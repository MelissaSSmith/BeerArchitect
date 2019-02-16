module Client.Style

open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.Import
open Fable.PowerPack
open Elmish.Browser.Navigation
module R = Fable.Helpers.React

type HTMLAttr = 
     | [<CompiledName("data-dismiss")>] DataDismiss of string
     | [<CompiledName("data-target")>] DataTarget of string
     | [<CompiledName("aria-label")>] AriaLabel of string
     | [<CompiledName("aria-hidden")>] AriaHidden of string
     | [<CompiledName("aria-controls")>] AriaControls of string
     | [<CompiledName("data-sort")>] DataSort of string
     interface IHTMLProp 

let goToUrl (e: React.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

let viewLink page description =
  R.a [ ClassName "nav-item nav-link sidebar-tab"
        Href (Pages.toPath page)
        OnClick goToUrl]
      [ R.str description]

let centerStyle direction =
    Style [ Display "flex"
            FlexDirection direction
            AlignItems "center"
            JustifyContent "center"
            Padding "20px 0"
    ]

let words size message =
    R.span [ Style [ FontSize (size |> sprintf "%dpx") ] ] [ R.str message ]

let buttonLink cssClass onClick elements =
    R.a [ ClassName cssClass
          OnClick (fun _ -> onClick())
          OnTouchStart (fun _ -> onClick())
          Style [ Cursor "pointer" ] ] elements

let onEnter msg dispatch =
    function
    | (ev:React.KeyboardEvent) when ev.keyCode = Keyboard.Codes.enter ->
        ev.preventDefault()
        dispatch msg
    | _ -> ()
    |> OnKeyDown

let pageHeader title =
    R.h2 [] [R.str title]