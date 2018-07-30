module Server.Templates

open Client.Shared
open Giraffe.GiraffeViewEngine

let index (model: Model option) =
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
        ]
        script [ _src "/public/bundle.js" ] []
      ]
    ]