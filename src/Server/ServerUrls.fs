module ServerCode.ServerUrls

module PageUrls =
  [<Literal>]
  let Home = "/"
  [<Literal>]
  let AbvCalculator = "/abv-calculator"
  [<Literal>]
  let SrmCalculator = "/srm-calculator"
  [<Literal>]
  let IbuCalculator = "/ibu-calculator"
  [<Literal>]
  let HydrometerCalculator = "/hydrometer-temp-calculator"
  [<Literal>]
  let AllGrainCalculator = "/all-grain-calculator"
  [<Literal>]
  let DilutionBoilOffCalculator = "/dilution-boil-off-calculator"
  [<Literal>]
  let YeastProfiles = "/yeast-profiles"
  [<Literal>]
  let FermentableProfiles = "/fermentables"
  [<Literal>]
  let HopProfiles = "/hops"
  let HopProfile = "/hops/%s"

module APIUrls =

  [<Literal>]
  let CalculateAbv = "/api/abv/calculate"
  [<Literal>]
  let CalculateSrm = "/api/srm/calculate"
  [<Literal>]
  let GetFermentables = "/api/fermentables"
  [<Literal>]
  let CalculateIbu = "/api/ibu/calculate"
  [<Literal>]
  let GetHopAlphaAcids = "/api/hops/get-alpha-acids"
  [<Literal>]
  let GetHops = "/api/hops"
  [<Literal>]
  let GetHop = "/api/hops/%s"
  [<Literal>]
  let CalculateHydrometerAdjustment = "/api/hydrometer-temp/calculate"
  [<Literal>]
  let CalculateAllGrainEstimations = "/api/all-grain/calculate"
  [<Literal>]
  let GetYeasts = "/api/yeast"
  [<Literal>]
  let CalculateDilutionVolume = "/api/brew/boil-off/new-volume"
  [<Literal>]
  let CalculateDilutionGravity = "/api/brew/boil-off/new-gravity"
