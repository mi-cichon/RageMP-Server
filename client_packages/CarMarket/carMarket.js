function confirmCarMarket()
{
    let price = $('.priceInput').val();
    let desc = $('.descInput').val();
    if(!isNaN(parseInt(price)) && parseInt(price) > 0)
    {
        mp.trigger("confirmCarMarket", parseInt(price), desc);
    }
}

function setName(name){
    $(".nameText").html(name);
}

function closeCarMarketBrowser(){
    mp.trigger("closeCarMarketBrowser");
}