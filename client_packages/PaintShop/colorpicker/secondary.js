// moduled querySelector
function qs_s(selectEl){return document.querySelector(selectEl)}
// select RGB inputs
const red_s = qs_s('#red_secondary'), 
green_s = qs_s('#green_secondary'), 
blue_s = qs_s('#blue_secondary') 

// selet num inputs
const redNumVal_s = qs('#redNum_secondary'), 
greenNumVal_s = qs('#greenNum_secondary'), 
blueNumVal_s = qs('#blueNum_secondary')

// select labels
const redLbl_s = qs('#red_secondary'), 
greenLbl_s = qs('#green_secondary'), 
blueLbl_s = qs('#blue_secondary')

// initial color val when DOM is loaded 
function initColorNumbrVals_s(){
    redNumVal_s.value = red_s.value
    greenNumVal_s.value = green_s.value
    blueNumVal_s.value = blue_s.value
}

// initial colors when DOM is loaded
function initSliderColors_s(){
    // label bg color
    let preview = $(".picker_secondary");
    preview.css("background-color", `rgb(${red_s.value},${green_s.value},${blue_s.value})`);
    preview.css("box-shadow", `0 0 1.5vh 0 rgb(${red_s.value},${green_s.value},${blue_s.value})`);
    // slider bg colors
    setSliderFill_s(red_s)
    setSliderFill_s(green_s)
    setSliderFill_s(blue_s)
    color2 = [parseInt(red_s.value), parseInt(green_s.value), parseInt(blue_s.value)];
}

// Slider Fill offset
function setSliderFill_s(clr){
    let val = (clr.value - clr.min) / (clr.max - clr.min)
    let percent = val * 100

    // clr input
    if(clr === red_s){
        clr.style.background = `linear-gradient(to right, rgb(${clr.value},0,0) ${percent}%, #1f1f1f 0%)`  
    } else if (clr === green_s) {
        clr.style.background = `linear-gradient(to right, rgb(0,${clr.value},0) ${percent}%, #1f1f1f 0%)`    
    } else if (clr === blue_s) {
        clr.style.background = `linear-gradient(to right, rgb(0,0,${clr.value}) ${percent}%, #1f1f1f 0%)`    
    }
}

// change range values by number input
function changeRangeNumVal_s(){
    validateBtnValues(redNumVal_s, red_s)
    validateBtnValues(greenNumVal_s, green_s)
    validateBtnValues(blueNumVal_s, blue_s)
}

function validateBtnValues_s(btn, inputs){
    // Validate number range
    btn.addEventListener('change', () => {
        // make sure numbers are entered between 0 to 255
        if (btn.value > 255) {
            btn.value = inputs.value
        } else if (btn.value < 0) {
            btn.value = inputs.value
        } else if(btn.value == '') {
            btn.value = inputs.value
            initSliderColors_s()
        } else {
            inputs.value = btn.value           
            initSliderColors_s()
        }
    })
}

// Color Sliders controls
function colorSliders_s(){
    btnInputEvents_s(red_s)
    btnInputEvents_s(green_s)
    btnInputEvents_s(blue_s)
}

function btnInputEvents_s(btn){
    btn.addEventListener('input', () => {
        initColorNumbrVals_s()
        initSliderColors_s()
        changeRangeNumVal_s()
    })
}

function initApp_s(){
    // init Colors controls
    colorSliders_s()
    // init display Colors
    // init Color Vals
    initColorNumbrVals_s()
    // init ColorSliderVals
    initSliderColors_s()
    // init Change Range Val
    changeRangeNumVal_s()
}

window.addEventListener('DOMContentLoaded', initApp_s)
