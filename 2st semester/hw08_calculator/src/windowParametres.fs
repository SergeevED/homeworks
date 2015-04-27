module windowParametres

let windowWidth       = 512
let windowHeight      = 512
let quantityOfLines   = 6
let quantityOfColumns = 5
let font              = 20.0F
let interval          = 10

let buttonWidth       = (windowWidth - interval * (quantityOfColumns + 1)) / quantityOfColumns
let buttonHeigth      = (windowHeight - interval * (quantityOfLines + 2) - int(font)) / quantityOfLines
