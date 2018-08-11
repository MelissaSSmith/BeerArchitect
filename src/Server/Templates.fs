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
        // Note we call ofJson twice here,
        // because Elmish's model can be some complicated type instead of pojo.
        // The first one will seriallize the state to a json string,
        // and the second one will seriallize the json string to a js string,
        // so we can deseriallize it by Fable's ofJson and get the correct types.
        toJson (toJson model),
        Client.Shared.view model ignore |> renderToString
    | None ->
        "null", ""
  html []
    [ head [] [ 
        meta [ _httpEquiv "Content-Type"; _content "text/html"; _charset "utf-8" ]
        title [] [ rawText "SAFE-Stack sample" ]
        link
          [ _rel "stylesheet"
            _href "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css";
            attr "integrity" "sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u"
            _crossorigin "anonymous"
          ]
        link
          [ _rel "stylesheet"
            _href "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css"
            attr "integrity" "sha384-rHyoN1iRsVXV4nD0JutlnGaslCJuC7uwjduW9SVrLvRYooPp2bWYgmgJQIXwl/Sp"
            _crossorigin "anonymous" ]
        link [ _rel "stylesheet"; _href "css/site.css" ]
        link [ _rel "shortcut icon"; _type "image/png"; _href "/Images/safe_favicon.png" ]
      ]
      body [ _class "app-container" ] [
        div [ _id "beer-architect-main"; ] [
          rawText htmlStr
        ]
        script [ ] [ rawText (sprintf "var __INIT_MODEL__ = %s" jsonState) ]
        script [ _src "/public/js/bundle.js" ] []
      ]
    ]