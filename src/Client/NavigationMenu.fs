module Client.NavigationMenu

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Pages
open Client.Style

let navigationList = 
    ul [ClassName "nav flex-column nav-pills"] [
        yield viewLink Page.Home "Home"
        yield viewLink Page.AbvCalculator "ABV Calculator"
        yield viewLink Page.SrmCalculator "SRM Calculator"
        yield viewLink Page.IbuCalculator "IBU Calculator"
        yield viewLink Page.HydrometerTempCalculator "Hydrometer Adjustment Calculator"
        yield viewLink Page.AllGrainCalculator "All Grain Calculator"
        yield viewLink Page.DilutionBoilOffCalculator "Dilution & Boil Off Calculator"
    ]

let navigationBar = 
    nav [ClassName  "navbar navbar-dark fixed-top bg-dark flex-md-nowrap p-0 shadow"] [
        div [ClassName "navbar-brand col-sm-3 col-md-2 mr-0 logo"] [ str ("Beer Architect") ]  
        button [
                Type "button"
                ClassName "navbar-toggler nav-btn"
                DataToggle "collapse"
                DataTarget "#nav-menu"
                AriaControls "nav-menu"
                AriaExpanded false] [
            span [ClassName "navbar-toggler-icon"] []
        ]
        div [ClassName "collapse navbar-collapse"
             Id "nav-menu"] [
            navigationList
        ]
    ]
let sidebarNavigationMenu =
    nav [ClassName "col-md-2 d-none d-md-block sidebar"] [
        div [ClassName "navigation-menu"] [
            navigationList
        ]
    ]
    