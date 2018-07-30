module Server.ServerUrls

module PageUrls =
  [<Literal>]
  let Home = "/"

module APIUrls =

  [<Literal>]
  let WishList = "/api/wishlist/"
  [<Literal>]
  let ResetTime = "/api/wishlist/resetTime/"
  [<Literal>]
  let Login = "/api/users/login/"