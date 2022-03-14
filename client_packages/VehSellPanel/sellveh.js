function insertData(name, price, trip){
    $(".name").text(name);
    $(".price").text(betterNumbers(price) + "$");
    $(".trip").text(parseFloat(trip).toFixed(1).toString() + " km");
}

$(".button_cancel").on("click", function(){
    mp.trigger("sellVeh_closeBrowser");
});

$(".button_select").on("click", function(){
    mp.trigger("sellVeh_sell");
    mp.trigger("sellVeh_closeBrowser");
});

function betterNumbers(val) {
    let spacer = ' ';
    let result = val.toString().trim();
    for(var i = val.toString().trim().length-1; i >= 0; i--) {
        if((val.toString().trim().length - i) % 3 == 0) {
            if(val.toString().slice(0,i) != '')
            result = result.slice(0,i) + spacer + result.slice(i);
        }
    }
    return result;
}