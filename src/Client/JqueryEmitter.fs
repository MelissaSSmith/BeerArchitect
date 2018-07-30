module JqueryEmitter

open Fable.Core

type IJQuery = 
    [<Emit("$0.click($1)")>]
    abstract OnClick : (obj -> unit) -> IJQuery

module JQuery =

    [<Emit("window['$']($0)")>]
    let ready (handler: unit -> unit) : unit = jsNative
      
    [<Emit("$2.css($0, $1)")>]
    let css (prop: string) (value: string) (el: IJQuery) : IJQuery = jsNative
      
    [<Emit("$1.click($0)")>]
    let click (handler: obj -> unit) (el: IJQuery) : IJQuery = jsNative

    [<Emit("window['$']($0)")>]
    let select (selector: string) : IJQuery = jsNative

    [<Emit("$0.empty()")>]
    let empty (el: IJQuery) : IJQuery = jsNative

    [<Emit("$1.append($0)")>]
    let append (prop: string) (el: IJQuery) : IJQuery = jsNative

    [<Emit("$2.prop($0, $1)")>]
    let prop (property: string) (value: int) (el: IJQuery) : IJQuery = jsNative