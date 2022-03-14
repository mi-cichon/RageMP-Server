var color1 = [0, 204, 153];
var color2 = [0, 204, 153];

var color1mode = 0; //0, 1 i 3
var color2mode = 0;

$('.paintshop_primaryButtons input').on("click", function (){
    $('.paintshop_primaryButtons *').removeClass("paintshop_selected");
    $(this).addClass("paintshop_selected");
    switch($('.paintshop_primaryButtons *').index(this))
    {
        case 0:
            color1mode = 0;
            break;
        case 1:
            color1mode = 1;
            break;
        case 2:
            color1mode = 3;
            break;
    }
});

$('.paintshop_secondaryButtons input').on("click", function (){
    $('.paintshop_secondaryButtons *').removeClass("paintshop_selected");
    $(this).addClass("paintshop_selected");
    switch($('.paintshop_secondaryButtons *').index(this))
    {
        case 0:
            color2mode = 0;
            break;
        case 1:
            color2mode = 1;
            break;
        case 2:
            color2mode = 3;
            break;
    }
});

$(".paintshop_preview").on("click", function (){
    mp.trigger('showVehicleColor', JSON.stringify([color1[0], color1[1], color1[2], color1mode]), JSON.stringify([color2[0], color2[1], color2[2], color2mode]));
});

$(".paintshop_confirm").on("click", function (){
    mp.trigger('chooseVehicleColor', JSON.stringify([color1[0], color1[1], color1[2], color1mode]), JSON.stringify([color2[0], color2[1], color2[2], color2mode]));
});

$(".paintshop_close").on("click", function (){
    mp.trigger('closePaintShopBrowser');
});