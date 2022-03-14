function setVars(price, tank, fuel){
    $('.petrolHUD_price p').text(`${price} $/l`);
    $('.petrolHUD_tank p').text(`${tank} l`);
    $('.petrolHUD_fuel p').text(`${fuel} l`);
}

function setPosition(x, y){
    let width = $('body').width();
    let height = $('body').height();
    let body = $('.petrolHUD_body');
    body.css('left', `${parseFloat(x) * width}px`);
    body.css('top', `${parseFloat(y) * height}px`);
}

function updateVars(fuel, cost){
    $('.petrolHUD_cost p').text(`${cost} $`);
    $('.petrolHUD_fuel p').text(`${fuel} l`);
}