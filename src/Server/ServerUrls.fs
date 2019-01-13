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

module APIUrls =

  [<Literal>]
  let CalculateAbv = "/api/abv/calculate"
  [<Literal>]
  let CalculateSrm = "/api/srm/calculate"
  [<Literal>]
  let GetFermentables = "/api/fermentable/get"
  [<Literal>]
  let CalculateIbu = "/api/ibu/calculate"
  [<Literal>]
  let GetHopAlphaAcids = "/api/hops/get-alpha-acids"
  [<Literal>]
  let CalculateHydrometerAdjustment = "/api/hydrometer-temp/calculate"
  [<Literal>]
  let CalculateAllGrainEstimations = "/api/all-grain/calculate"