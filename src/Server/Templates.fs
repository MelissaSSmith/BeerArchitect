module ServerCode.Templates

open Client.Shared
open Giraffe.GiraffeViewEngine
open Fable.Helpers.ReactServer

open Newtonsoft.Json

// The Fable.JsonConverter serializes F# types so they can be deserialized on the
// client side by Fable into full type instances, see http://fable.io/blog/Introducing-0-7.html#JSON-Serialization
// The converter includes a cache to improve serialization performance. Because of this,
// it's better to keep a single instance during the server lifetime.
let private jsonConverter = Fable.JsonConverter() :> JsonConverter

let toJson value =
    JsonConvert.SerializeObject(value, [|jsonConverter|])

let ofJson<'a> (json:string) : 'a =
    JsonConvert.DeserializeObject<'a>(json, [|jsonConverter|])

let index (model: Model option) =
  let jsonState, htmlStr =
    match model with
    | Some model ->
        toJson (toJson model),
        Client.Shared.view model ignore |> renderToString
    | None ->
        "null", ""
  html []
    [ head [] [ 
        meta [ _httpEquiv "Content-Type"; _content "text/html"; _charset "utf-8" ]
        title [] [ rawText "Beer Architect" ]
        link
          [ _rel "stylesheet"
            _href "https://cdnjs.cloudflare.com/ajax/libs/bulma/0.6.1/css/bulma.min.css"
          ]
        link
          [ _rel "stylesheet"
            _href "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"
          ]
        link
          [ _rel "stylesheet"
            _href "https://fonts.googleapis.com/css?family=Open+Sans"
          ]
        link
          [ _rel "stylesheet"
            _href "https://maxcdn.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css"
          ]
        link [ _rel "stylesheet"; _href "style.css" ]
        script [ _src "https://code.jquery.com/jquery-3.2.1.min.js" ] []
        link [ _rel "shortcut icon"; _type "image/png"; _href "/Images/safe_favicon.png" ]
      ]
      body [] [
        div [ _id "beer-architect-main"; ] [
          rawText htmlStr
        ]
        script [ ] [ rawText (sprintf "var __INIT_MODEL__ = %s" jsonState) ]
        script [ _src "https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.9/umd/popper.min.js" ] []
        script [ _src "https://maxcdn.bootstrapcdn.com/bootstrap/4.1.3/js/bootstrap.min.js" ] []
        script [ _src "./js/bundle.js" ] []
      ]
    ]