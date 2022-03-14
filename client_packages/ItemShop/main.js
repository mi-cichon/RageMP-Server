let itemShopBrowser = null;
let player = mp.players.local;
mp.events.add("openItemShopBrowser", () => {
    if(itemShopBrowser != null && mp.browsers.exists(itemShopBrowser)){
        itemShopBrowser.destroy();
        itemShopBrowser = null;
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setGui", false);
    }
    else if(!(player.hasVariable("gui") && player.getVariable("gui"))){
        itemShopBrowser = mp.browsers.new("package://ItemShop/index.html");
        mp.gui.cursor.show(true, true);
        mp.events.callRemote("setGui", true);
    }
});

mp.events.add("closeItemShopBrowser", () => {
    if(itemShopBrowser){
        itemShopBrowser.destroy();
        itemShopBrowser = null;
        mp.events.callRemote("setGui", false);
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("shopBuyItem", (itemId) => {
    mp.events.callRemote("itemShopBuyItem", itemId);
});