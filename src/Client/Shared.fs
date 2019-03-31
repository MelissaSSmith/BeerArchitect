module Client.Shared

type PageModel =
    | HomePageModel
    | AbvCalculatorPageModal of AbvCalculator.Model
    | SrmCalculatorPageModel of SrmCalculator.Model
    | IbuCalculatorPageModel of IbuCalculator.Model
    | HydrometerTempCalculatorPageModel of HydrometerTempCalculator.Model
    | AllGrainCalculatorPageModel of AllGrainCalculator.Model
    | YeastProfilesPageModel of YeastProfiles.Model
    | DilutionBoilOffCalculatorPageModel of DilutionBoilOffCalculator.Model
    | FermentableProfilesPageModel of FermentableProfiles.Model
    | HopProfilesPageModel of HopProfiles.Model
    | HopProfilePageModel of HopProfile.Model

type Model =
    { PageModel : PageModel }

type Msg =
    | AbvCalculatorMsg of AbvCalculator.Msg
    | SrmCalculatorMsg of SrmCalculator.Msg
    | IbuCalculatorMsg of IbuCalculator.Msg
    | HydrometerTempCalculatorMsg of HydrometerTempCalculator.Msg
    | AllGrainCalculatorMsg of AllGrainCalculator.Msg
    | YeastProfilesMsg of YeastProfiles.Msg
    | DilutionBoilOffCalculatorMsg of DilutionBoilOffCalculator.Msg
    | FermentableProfilesMsg of FermentableProfiles.Msg
    | HopProfilesMsg of HopProfiles.Msg
    | HopProfileMsg of HopProfile.Msg

open Fable.Helpers.React

let viewPage model dispatch =
    match model.PageModel with
    | HomePageModel ->
        Home.view ()
    | AbvCalculatorPageModal m ->
        AbvCalculator.view m (AbvCalculatorMsg >> dispatch)
    | SrmCalculatorPageModel m ->
        SrmCalculator.view m (SrmCalculatorMsg >> dispatch)
    | IbuCalculatorPageModel m ->
        IbuCalculator.view m (IbuCalculatorMsg >> dispatch)
    | HydrometerTempCalculatorPageModel m ->
        HydrometerTempCalculator.view m (HydrometerTempCalculatorMsg >> dispatch)
    | AllGrainCalculatorPageModel m ->
        AllGrainCalculator.view m (AllGrainCalculatorMsg >> dispatch)
    | YeastProfilesPageModel m ->
        YeastProfiles.view m (YeastProfilesMsg >> dispatch)
    | DilutionBoilOffCalculatorPageModel m ->
        DilutionBoilOffCalculator.view m (DilutionBoilOffCalculatorMsg >> dispatch)
    | FermentableProfilesPageModel m ->
        FermentableProfiles.view m (FermentableProfilesMsg >> dispatch)
    | HopProfilesPageModel m -> 
        HopProfiles.view m (HopProfilesMsg >> dispatch)
    | HopProfilePageModel m ->
        HopProfile.view m (HopProfileMsg >> dispatch)

let view model dispatch =
    div [] [ viewPage model dispatch ]