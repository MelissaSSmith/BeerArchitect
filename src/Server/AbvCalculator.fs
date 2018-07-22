module AbvCalculator

let Abv og fg = (og - fg) * 131.25

let AlternateAbv og fg = (76.08 * (og - fg) / (1.775 - og)) * (fg / 0.794)

let CalFromAlcohol og fg = 1881.22 * fg * (og - fg)/(1.775 - og)

let CalFromCarbs og fg = 3550.0 * fg * ((0.1808 * og) + (0.819 * fg) - 1.0004)

let TotalCal og fg = CalFromAlcohol og fg + CalFromCarbs og fg