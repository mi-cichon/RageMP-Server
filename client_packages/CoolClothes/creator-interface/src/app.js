let clothes = {
    top: [0, 0],
    torso: [0, 0],
    undershirt: [0, 0],
    pants: [0, 0],
    shoes: [0, 0]
}
const toRemove = {
    maleTops: [2, 15, 18, 19, 45, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 67, 71, 79, 81, 83, 85, 90, 91, 92, 96, 97, 98, 107, 108, 109, 110, 112, 114, 116, 124, 125, 129, 130, 131, 132, 133, 137, 144, 145, 147, 148, 149, 152, 155, 158, 159, 160, 163, 164, 165, 168, 176, 177, 178, 179, 180, 183, 186, 194, 195, 196, 197, 198, 199, 201, 213, 214, 225, 226, 227, 228, 231, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 270, 272, 273, 274, 275, 276, 277, 278, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 310, 314, 315, 316, 317, 318, 319, 320, 321, 322, 324, 326, 327, 328, 329, 330, 331, 332, 333, 336, 337, 352, 353, 356, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 372, 375, 376, 380, 382],
    maleUndershirts: [19, 20, 55, 56, 58, 59, 61, 62, 78, 79, 80, 91, 92, 97, 103, 104, 105, 106, 107, 108, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 137, 143, 145, 151, 152, 153, 154, 155, 156, 159, 160, 161, 162, 163, 164, 170, 171, 172, 180, 181, 182, 183, 184],
    malePants: [2, 11, 29, 30, 31, 32, 33, 34, 35, 36, 38, 39, 40, 41, 44, 46, 53, 56, 57, 58, 63, 65, 66, 67, 68, 70, 77, 84, 85, 91, 92, 93, 94, 95, 96, 97, 98, 99, 101, 104, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 131, 133, 134, 135, 136, 137, 138],
    maleShoes: [0, 2, 11, 13, 17, 19, 24, 27, 29, 30, 33, 39, 41, 46, 47, 48, 49, 55, 56, 58, 59, 64, 67, 68, 69, 74, 78, 83, 84, 85, 86, 87, 88, 89, 90, 91, 95, 96, 97, 98, 99, 100, 101, 102],




    femaleTops: [4,12,17,19,20,29,34,41,42,43,44,45,46,47,48,49,50,51,56,59,60,61,67,69,72,77,80,82,83,89,97,98,99,100,102,104,105,108,122,127,142,143,144,145,146,149,152,155,156,157,161,162,178,179,180,181,182,188,196,197,198,199,200,201,203,217,218,229,230,231,232,236,237,238,241,251,252,253,254,255,256,257,259,260,261,263,282,285,287,288,289,290,291,295,296,297,298,299,300,301,302,303,304,325,326,327,328,329,330,331,332,333,336,341,342,343,344,345,346,347,348,351,352,374,378,379,380,381,382],
    femaleUndershirts: [19, 20, 55, 56, 58, 59, 61, 62, 78, 79, 80, 91, 92, 97, 103, 104, 105, 106, 107, 108, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 137, 143, 145, 151, 152, 153, 154, 155, 156, 159, 160, 161, 162, 163, 164, 170, 171, 172, 180, 181, 182, 183, 184],
    femalePants: [5, 13, 15, 17, 21, 29, 33, 35, 38, 39, 40, 42, 46, 48, 57, 59, 61, 68, 69, 70, 72, 79, 86, 88, 94, 95, 96, 98, 100, 101, 103, 105, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 125, 126, 127, 129, 130, 131, 132, 134, 136],
    femaleShoes: [12, 17, 34, 40, 48, 61, 63, 65, 70, 71, 73, 74, 75, 76, 77, 83, 85, 88, 89, 91, 92, 95, 100]
}
let gender = 0;

Object.size = function(obj) {
    var size = 0,
        key;
    for (key in obj) {
        if (obj.hasOwnProperty(key)) size++;
    }
    return size;
};


let tops = [Object.keys(removeItemsFromJson(JSON.parse(maletops), toRemove.maleTops)), Object.keys(removeItemsFromJson(JSON.parse(femaletops), toRemove.femaleTops))];
let torsos = [
    [...Array(16).keys()],
    [...Array(16).keys()]
]
let undershirts = [Object.keys(removeItemsFromJson(JSON.parse(maleundershirts), toRemove.maleUndershirts)), Object.keys(removeItemsFromJson(JSON.parse(femaleundershirts), toRemove.femaleUndershirts))];
let pants = [Object.keys(removeItemsFromJson(JSON.parse(malepants), toRemove.malePants)), Object.keys(removeItemsFromJson(JSON.parse(femalepants), toRemove.femalePants))];
let shoes = [Object.keys(removeItemsFromJson(JSON.parse(maleshoes), toRemove.maleShoes)), Object.keys(removeItemsFromJson(JSON.parse(femaleshoes), toRemove.femaleShoes))];

// let topsV = [JSON.parse(maletops)]
let topsV = [itemsToVariants(JSON.parse(maletops), toRemove.maleTops), itemsToVariants(JSON.parse(femaletops), toRemove.femaleTops)];
let undershirtsV = [itemsToVariants(JSON.parse(maleundershirts), toRemove.maleUndershirts), itemsToVariants(JSON.parse(femaleundershirts), toRemove.femaleUndershirts)];
let pantsV = [itemsToVariants(JSON.parse(malepants), toRemove.malePants), itemsToVariants(JSON.parse(femalepants), toRemove.femalePants)];
let shoesV = [itemsToVariants(JSON.parse(maleshoes), toRemove.maleShoes), itemsToVariants(JSON.parse(femaleshoes), toRemove.femaleShoes)];




const sliders = {
    element: $('.slides'),
    elements: $('.slide'),
    indicators: $('.navigation > li'),
    active: 0,

    open: (n) => {
        if (n >= sliders.elements.length) { n = 0; }
        if (n < 0) { n = sliders.elements.length - 1 }
        sliders.active = n;
        $(sliders.indicators).removeClass('active')
        $(sliders.indicators[n]).addClass('active')
        $(sliders.elements).removeClass("show")
        $(sliders.elements[n]).addClass("show")
    }
}

const customize = (x, id, val) => {

    switch (x) {
        case "top":
            clothes[x][id] = parseInt(tops[gender][val]);
            clothes[x][1] = 0;
            $(".tops .mslider").attr("value", 0);
            $(".tops .mslider").attr("max", Object.size(topsV[gender][tops[gender][val]]) - 1);
            break;
        case "topV":
            x = "top";
            clothes[x][1] = parseInt(val);
            break;
        case "torso":
            clothes[x][id] = parseInt(torsos[gender][val]);
            break;
        case "undershirt":
            clothes[x][id] = parseInt(undershirts[gender][val]);
            clothes[x][1] = 0;
            $(".undershirts .mslider").attr("value", 0);
            $(".undershirts .mslider").attr("max", Object.size(undershirtsV[gender][tops[gender][val]]) - 1);
            break;
        case "undershirtV":
            x = "undershirt";
            clothes[x][1] = parseInt(val);
            break;
        case "pants":
            clothes[x][id] = parseInt(pants[gender][val]);
            clothes[x][1] = 0;
            $(".pants .mslider").attr("value", 0);
            $(".pants .mslider").attr("max", Object.size(pantsV[gender][tops[gender][val]]) - 1);
            break;
        case "pantsV":
            x = "pants";
            clothes[x][1] = parseInt(val);
            break;
        case "shoes":
            clothes[x][id] = parseInt(shoes[gender][val]);
            clothes[x][1] = 0;
            $(".shoes .mslider").attr("value", 0);
            $(".shoes .mslider").attr("max", Object.size(shoesV[gender][tops[gender][val]]) - 1);
            break;
        case "shoesV":
            x = "shoes";
            clothes[x][1] = parseInt(val);
            break;

    }
    mp.trigger('client:clothes.preview', x, JSON.stringify(clothes[x]));
}


const customization = {
    init() {
        // for (let f in data.faceFeatures) {
        //     $('.faceFeatures').append(`<div class='slider-handler'> <label> ${data.faceFeatures[f]} </label>
        //     <input type='range' class='slider' oninput='customize("faceFeatures", ${f}, this.value)' min='-1' max='1' step='0.1' > </div>`)
        // }
    },

    reload() {
        if (character.gender == 1) {
            $('.beard').css('display', 'none');
            $('#headOverlay-7').css('display', 'flex');
            $('#headOverlay-3').css('display', 'flex');
            $('#headOverlay-4').css('display', 'flex');
            $('#headOverlay-9').css('display', 'none');
        } else {
            $('.beard').css('display', 'flex');
            $('#headOverlay-9').css('display', 'flex');
            $('#headOverlay-7').css('display', 'none');
            $('#headOverlay-3').css('display', 'none');
            $('#headOverlay-4').css('display', 'none');
        }

        mp.trigger('client:creator.reload')
    },

    max(t, l, s) {
        $('.clothings-0').attr('max', t);
        $('.clothings-1').attr('max', l);
        $('.clothings-2').attr('max', s);
    },

    finish() {
        mp.trigger('client:clothes.finish');
    },
    cancel(){
        mp.trigger('client:clothes.cancel');
    },
    rotate(){
        mp.trigger('client:clothes.rotate');
    }
}

$(window).on('load', () => {
    sliders.open(0);
    customization.init();
})

$('#char-birth-date').change(function() {
    let date = this.value.split('-'),
        year = parseInt(date[0]),
        dateFormat = date[2] + '/' + date[1] + '/' + date[0];
    year > 2001 || year < 1916 ? ($(this).css('borderColor', 'tomato'), character.birth = null) : ($(this).css('borderColor', 'transparent'), character.birth = dateFormat);
})


function removeItemsFromJson(json, i) {
    let items = json;
    for (let item in items) {
        if (i.includes(parseInt(item))) {
            delete items[item];
        }
    }
    return items;
}

function itemsToVariants(json, toRemove) {
    variants = {};
    for (let item in json) {
        if (!toRemove.includes(parseInt(item))) {
            let list = Object.keys(json[item]);
            let res = list.map(function(x) {
                return parseInt(x);
            });
            variants[item] = res;
        }

    }
    return variants;
}

function setGender(g) {
    gender = g;
    $(".tops .slider").attr("max", Object.size(tops[gender]) - 1);
    $(".torsos .slider").attr("max", Object.size(torsos[gender]) - 1);
    $(".undershirts .slider").attr("max", Object.size(undershirts[gender]) - 1);
    $(".pants .slider").attr("max", Object.size(pants[gender]) - 1);
    $(".shoes .slider").attr("max", Object.size(shoes[gender]) - 1);

    $(".tops .mslider").attr("max", Object.size(topsV[gender][0]) - 1);
    $(".undershirts .mslider").attr("max", Object.size(undershirtsV[gender][0]) - 1);
    $(".pants .mslider").attr("max", Object.size(pantsV[gender][0]) - 1);
    $(".shoes .mslider").attr("max", Object.size(shoesV[gender][0]) - 1);
}

function logInfo(text){
    mp.events.call("logInfo", text);
}


$(`.top-left`).on("click", function(){
    let slider = $(`.top-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('top', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.top-right`).on("click", function(){
    let slider = $(`.top-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('top', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.topv-left`).on("click", function(){
    let slider = $(`.topv-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('topV', 1, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.topv-right`).on("click", function(){
    let slider = $(`.topv-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('topV', 1, parseInt(slider.attr("value")))
    refreshSlider(slider);
});





$(`.torso-left`).on("click", function(){
    let slider = $(`.torso-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('torso', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.torso-right`).on("click", function(){
    let slider = $(`.torso-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('torso', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});




$(`.undershirt-left`).on("click", function(){
    let slider = $(`.undershirt-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('undershirt', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.undershirt-right`).on("click", function(){
    let slider = $(`.undershirt-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('undershirt', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.undershirtv-left`).on("click", function(){
    let slider = $(`.undershirtv-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('undershirtV', 1, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.undershirtv-right`).on("click", function(){
    let slider = $(`.undershirtv-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('undershirtV', 1, parseInt(slider.attr("value")))
    refreshSlider(slider);
});






$(`.pants-left`).on("click", function(){
    let slider = $(`.pants-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('pants', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.pants-right`).on("click", function(){
    let slider = $(`.pants-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('pants', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.pantsv-left`).on("click", function(){
    let slider = $(`.pantsv-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('pantsV', 1, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.pantsv-right`).on("click", function(){
    let slider = $(`.pantsv-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('pantsV', 1, parseInt(slider.attr("value")))
    refreshSlider(slider);
});




$(`.shoes-left`).on("click", function(){
    let slider = $(`.shoes-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('shoes', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.shoes-right`).on("click", function(){
    let slider = $(`.shoes-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('shoes', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.shoesv-left`).on("click", function(){
    let slider = $(`.shoesv-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('shoesV', 1, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.shoesv-right`).on("click", function(){
    let slider = $(`.shoesv-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('shoesV', 1, parseInt(slider.attr("value")))
    refreshSlider(slider);
});



function refreshSlider(slider){
    let before = $(slider).prev();
    $(slider).remove();
    before.after(slider);
}