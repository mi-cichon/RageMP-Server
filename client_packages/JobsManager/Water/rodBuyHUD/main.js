let rodShopBrowser = null;
let player = mp.players.local;
mp.events.add("openRodShopBrowser", () => {
    if(rodShopBrowser != null && mp.browsers.exists(rodShopBrowser)){
        rodShopBrowser.destroy();
        rodShopBrowser = null;
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setGui", false);
    }
    else if(!(player.hasVariable("gui") && player.getVariable("gui"))){
        rodShopBrowser = mp.browsers.new("package://JobsManager/Water/rodBuyHUD/index.html");
        mp.gui.cursor.show(true, true);
        mp.events.callRemote("setGui", true);
    }
});

mp.events.add("closeRodShopBrowser", () => {
    if(rodShopBrowser){
        rodShopBrowser.destroy();
        rodShopBrowser = null;
        mp.events.callRemote("setGui", false);
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("shopBuyRod", (itemId) => {
    mp.events.callRemote("shopBuyRod", itemId);
});