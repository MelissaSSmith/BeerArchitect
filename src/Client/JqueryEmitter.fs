module Client.JqueryEmitter

open Fable.Core

type IJQuery = 
    [<Emit("$0.click($1)")>]
    abstract OnClick : (obj -> unit) -> IJQuery

module JQuery =

    [<Emit("window['$']($0)")>]
    let ready (handler: unit -> unit) : unit = jsNative
      
    [<Emit("$2.css($0, $1)")>]
    let css (_: string) (_: string) (_: IJQuery) : IJQuery = jsNative
      
    [<Emit("$1.click($0)")>]
    let click (handler: obj -> unit) (_: IJQuery) : IJQuery = jsNative

    [<Emit("window['$']($0)")>]
    let select (_: string) : IJQuery = jsNative

    [<Emit("$0.empty()")>]
    let empty (_: IJQuery) : IJQuery = jsNative

    [<Emit("$1.append($0)")>]
    let append (_: string) (_: IJQuery) : IJQuery = jsNative

    [<Emit("$2.prop($0, $1)")>]
    let prop (_: string) (_: int) (_: IJQuery) : IJQuery = jsNative

    [<Emit("$0.val()")>]
    let value (_: IJQuery) : string = jsNative

    [<Emit("$1.attr($0)")>]
    let attr (_: string) (_: IJQuery) : string = jsNative