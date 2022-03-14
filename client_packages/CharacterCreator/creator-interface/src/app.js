let character = {
    gender: 0,
    blendData: [0, 0, 0, 0, 0, 0],
    headOverlays: [255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255],
    headOverlaysColors: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    hair: [0, 0, 0],
    beard: [0, 0],
    faceFeatures: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
}


const data = {
    faceFeatures: [
        'Szerokość nosa', 'Wysokość nosa', 'Długość nosa', 'Nos – szerokość mostka', 'Nos – pozycja',
        'Nos – przemieszczenie grzbietu nosa', 'Wysokość brwi', 'Szerokość brwi', 'Wysokość policzków',
        'Szerokość kości policzkowej', 'Szerokość policzków', 'Oczy', 'Usta', 'Długość szczęki', 'Wysokość szczęki',
        'Długość podbródka', 'Pozycja podbródka', 'Szerokość podbródka', 'Kształt podbródka', 'Szerokość szyi',
    ],

    invalidHairs: [23, 24],

    hairColors: [
        '#0c0c0c', '#1d1a17', '#281d18', '#3d1f15', '#682e19', '#954b29', '#a35234', '#9b5f3d', '#b57e54', '#c19167',
        '#af7f53', '#be9560', '#d0ac75', '#b37f43', '#dbac68', '#e4ba7e', '#bd895a', '#83422c', '#8e3a28', '#8a241c',
        '#962b20', '#a7271d', '#c4351f', '#d8421f', '#c35731', '#d24b21', '#816755', '#917660', '#a88c74', '#d0b69e',
        '#513442', '#744557', '#a94663', '#cb1e8e', '#f63f78', '#ed9393', '#0b917e', '#248081', '#1b4d6b', '#578d4b',
        '#235433', '#155146', '#889e2e', '#71881b', '#468f21', '#cc953d', '#ebb010', '#ec971a', '#e76816', '#e64810',
        '#ec4d0e', '#c22313', '#e43315', '#ae1b18', '#6d0c0e', '#281914', '#3d241a', '#4c281a', '#5d3929', '#69402b',
        '#291b16', '#0e0e10', '#e6bb84', '#d8ac74'
    ],

    beardColors: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 26, 27, 28, 29, 55, 56, 57, 58, 59, 60, 61, 62, 63],
    headOverlays: ['Plamy', 'Brwi', 'Zmarszczki', 'Makijaż', 'Rumieńce', 'Opalenizna', 'Oparzenia słoneczne', 'Szminka', 'Piegi', 'Włosy na klatce piersiowej'],

    blendData: [
        ['Kształt twarzy matki', 0, 45, 1],
        ['Kształt twarzy ojca', 0, 45, 1],
        ['Kolor skóry matki', 0, 45, 1],
        ['Kolor skóry ojca', 0, 45, 1],
        ['Podobieństwo wizualne', -1, 1, 0.1],
        ['Podobieństwo rasowe', -1, 1, 0.1]
    ]
}

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

const gender = (el, i) => {
    character.gender = i;
    $('.genders h2').removeClass('active');
    $(el).addClass('active');
    mp.trigger('client:creator.preview', 'gender', i, 0, JSON.stringify(character));
    customization.reload();
}

const customize = (x, id, val) => {
    character[x][id] = val;

    if (x == 'faceFeatures')
        mp.trigger('client:creator.preview', x, JSON.stringify([id, val]), JSON.stringify(character))
    else{
        mp.trigger('client:creator.preview', x, JSON.stringify(character[x]), JSON.stringify(character));
        
    }
        
}


const customization = {
    init() {
        for (let f in data.faceFeatures) {
            $('.faceFeatures').append(`<div class='slider-handler'> <label> ${data.faceFeatures[f]} </label>
            <div class="scroll-body"><div class="button-left ff-${f}-left"><i class='far fa-arrow-alt-circle-left'></i></div><input type='range' class='slider ff-${f}-slider' oninput='customize("faceFeatures", ${f}, this.value)' value='0' min='-1' max='1' step='0.1' ><div class="button-right ff-${f}-right"><i class='far fa-arrow-alt-circle-right'></i></div></div> </div>`)
        
            $(`.ff-${f}-left`).on("click", function(){
                let slider = $(`.ff-${f}-slider`);
                if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
                    slider.attr("value", (parseFloat(slider.attr("value")) - parseFloat(slider.attr("step"))).toFixed(1));
                }
                customize("faceFeatures", f, slider.attr("value"))
                refreshSlider(slider);
            });
            
            $(`.ff-${f}-right`).on("click", function(){
                let slider = $(`.ff-${f}-slider`);
                if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
                    slider.attr("value", (parseFloat(slider.attr("value")) + parseFloat(slider.attr("step"))).toFixed(1));
                }
                customize("faceFeatures", f, slider.attr("value"))
                refreshSlider(slider);
            });
        }

        for (let h in data.hairColors) {
            $('.hairColors-1').append(`<li style='background: ${data.hairColors[h]};' onclick='customize("hair", 1, ${h})'> </li>`);
            $('.hairColors-2').append(`<li style='background: ${data.hairColors[h]};' onclick='customize("hair", 2, ${h})'> </li>`);
        }

        for (let b in data.beardColors) {
            let hair = data.beardColors[b],
                color = data.hairColors[hair];
            $('.beardColors').append(`<li style='background: ${color};' onclick='customize("beard", 1, ${hair})'> </li>`);
        }

        for (let b in data.blendData) {
            let blend = data.blendData[b];
            $('.blendData').append(`<div class='slider-handler'> <label> ${blend[0]} </label>
            <div class="scroll-body"><div class="button-left bd-${b}-left"><i class='far fa-arrow-alt-circle-left'></i></div><input type='range' class='slider bd-${b}-slider' value='0' oninput='customize("blendData", ${b}, this.value)' min='${blend[1]}' value='0' max='${blend[2]}' step='${blend[3]}' ><div class="button-right bd-${b}-right"><i class='far fa-arrow-alt-circle-right'></i></div></div> </div>`)
        


            $(`.bd-${b}-left`).on("click", function(){
                let slider = $(`.bd-${b}-slider`);
                if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
                    slider.attr("value", (parseFloat(slider.attr("value")) - parseFloat(slider.attr("step"))).toFixed(1));
                }
                customize("blendData", b, slider.attr("value"))
                refreshSlider(slider);
            });
            
            $(`.bd-${b}-right`).on("click", function(){
                let slider = $(`.bd-${b}-slider`);
                if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
                    slider.attr("value", (parseFloat(slider.attr("value")) + parseFloat(slider.attr("step"))).toFixed(1));
                }
                customize("blendData", b, slider.attr("value"))
                refreshSlider(slider);
            });
        }

        for (let h in data.headOverlays) {
            let hOverlay = data.headOverlays[h];
            $('.headOverlays').append(`<div class='slider-handler headOverlay' id='headOverlay-${h}' > <label> ${hOverlay} </label> 
            <button class='btn' onclick='customize("headOverlays", ${h}, 255)'>usuń</button>
            <div class="scroll-body"><div class="button-left ho-${h}-left"><i class='far fa-arrow-alt-circle-left'></i></div><input type='range' class='slider ho-${h}-slider' value='0' oninput='customize("headOverlays", ${h}, this.value)' min='0' max='12' ><div class="button-right ho-${h}-right"><i class='far fa-arrow-alt-circle-right'></i></div></div><ul class="overlayColors"></ul></div>`)
            
            
            $(`.ho-${h}-left`).on("click", function(){
                let slider = $(`.ho-${h}-slider`);
                if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
                    slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
                }
                customize("headOverlays", h, slider.attr("value"))
                refreshSlider(slider);
            });
            
            $(`.ho-${h}-right`).on("click", function(){
                let slider = $(`.ho-${h}-slider`);
                if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
                    slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
                }
                customize("headOverlays", h, slider.attr("value"))
                refreshSlider(slider);
            });
            
            
            
            for (let hair in data.hairColors) {
                if(h == 1){
                    $(`#headOverlay-${h} .overlayColors`).append(`<li style='background: ${data.hairColors[hair]};' onclick='customize("headOverlaysColors", ${h}, ${hair})'> </li>`);
                }
            }
        }

        
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
        if(character.gender != null){
            mp.trigger('client:creator.finish', JSON.stringify(character));
        }
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

$(`.hair-left`).on("click", function(){
    let slider = $(`.hair-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('hair', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.hair-right`).on("click", function(){
    let slider = $(`.hair-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('hair', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.beard-left`).on("click", function(){
    let slider = $(`.beard-slider`);
    if(parseFloat(slider.attr("value")) > parseFloat(slider.attr("min"))){
        slider.attr("value", (parseFloat(slider.attr("value")) - 1).toFixed(1));
    }
    customize('beard', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});

$(`.beard-right`).on("click", function(){
    let slider = $(`.beard-slider`);
    if(parseFloat(slider.attr("value")) < parseFloat(slider.attr("max"))){
        slider.attr("value", (parseFloat(slider.attr("value")) + 1).toFixed(1));
    }
    customize('beard', 0, parseInt(slider.attr("value")))
    refreshSlider(slider);
});


function refreshSlider(slider){
    let before = $(slider).prev();
    $(slider).remove();
    before.after(slider);
}