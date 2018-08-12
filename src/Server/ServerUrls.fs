module ServerCode.ServerUrls

module PageUrls =
  [<Literal>]
  let Home = "/"
  [<Literal>]
  let AbvCalculator = "/abv-calculator"
  [<Literal>]
  let ASrmCalculator = "/srm-calculator"

module APIUrls =

  [<Literal>]
  let CalculateAbv = "/api/abv/calculate"
  [<Literal>]
  let CalculateSrm = "/api/srm/calculate"