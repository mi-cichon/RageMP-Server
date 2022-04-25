// moduled querySelector
function qs(selectEl){return document.querySelector(selectEl)}
// select RGB inputs
const red = qs('#red_primary'), 
green = qs('#green_primary'), 
blue = qs('#blue_primary') 

// selet num inputs
const redNumVal = qs('#redNum_primary'), 
greenNumVal = qs('#greenNum_primary'), 
blueNumVal = qs('#blueNum_primary')

// select labels
const redLbl = qs('#red_primary'), 
greenLbl = qs('#green_primary'), 
blueLbl = qs('#blue_primary')

// initial color val when DOM is loaded 
function initColorNumbrVals(){
    redNumVal.value = red.value
    greenNumVal.value = green.value
    blueNumVal.value = blue.value
}

// initial colors when DOM is loaded
function initSliderColors(){
    // label bg color

    convertToHexcode(red.value, green.value, blue.value)
    let preview = $(".picker_primary");
    preview.css("background-color", `rgb(${red.value},${green.value},${blue.value})`);
    preview.css("box-shadow", `0 0 1.5vh 0 rgb(${red.value},${green.value},${blue.value})`);
    // slider bg colors
    setSliderFill(red)
    setSliderFill(green)
    setSliderFill(blue)
    color1 = [parseInt(red.value), parseInt(green.value), parseInt(blue.value)];
}

// Slider Fill offset
function setSliderFill(clr){
    let val = (clr.value - clr.min) / (clr.max - clr.min)
    let percent = val * 100

    // clr input
    if(clr === red){
        clr.style.background = `linear-gradient(to right, rgb(${clr.value},0,0) ${percent}%, #1f1f1f 0%)`  
    } else if (clr === green) {
        clr.style.background = `linear-gradient(to right, rgb(0,${clr.value},0) ${percent}%, #1f1f1f 0%)`    
    } else if (clr === blue) {
        clr.style.background = `linear-gradient(to right, rgb(0,0,${clr.value}) ${percent}%, #1f1f1f 0%)`    
    }
}

function convertToHexcode(r, g, b){
    let rHex = parseInt(r)
    let gHex = parseInt(g)
    let bHex = parseInt(b)
}

function hexIt(num){
    hex = num.toString(16)
    num < 16 ? hex = "0" + hex : 0
    return hex
}

// change range values by number input
function changeRangeNumVal(){
    validateBtnValues(redNumVal, red)
    validateBtnValues(greenNumVal, green)
    validateBtnValues(blueNumVal, blue)
}

function validateBtnValues(btn, inputs){
    // Validate number range
    btn.addEventListener('change', () => {
        // make sure numbers are entered between 0 to 255
        if (btn.value > 255) {
            btn.value = inputs.value
        } else if (btn.value < 0) {
            btn.value = inputs.value
        } else if(btn.value == '') {
            btn.value = inputs.value
            initSliderColors()
        } else {
            inputs.value = btn.value           
            initSliderColors()
        }
    })
}

// Color Sliders controls
function colorSliders(){
    btnInputEvents(red)
    btnInputEvents(green)
    btnInputEvents(blue)
}

function btnInputEvents(btn){
    btn.addEventListener('input', () => {
        initColorNumbrVals()
        initSliderColors()
        changeRangeNumVal()
    })
}

function initApp(){
    // init Colors controls
    colorSliders()
    // init display Colors
    // init Color Vals
    initColorNumbrVals()
    // init ColorSliderVals
    initSliderColors()
    // init Change Range Val
    changeRangeNumVal()
}

window.addEventListener('DOMContentLoaded', initApp)
