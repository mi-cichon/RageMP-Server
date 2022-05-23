function buyRod(itemId){
    mp.trigger("shopBuyRod", itemId);
}

function closeRodBrowser(){
    mp.trigger("closeRodShopBrowser");
}