function buyItem(itemId){
    mp.trigger("shopBuyItem", itemId);
}

function closeBrowser(){
    mp.trigger("closeItemShopBrowser");
}