let buyTunePanelBrowser = null;
let businessId = null;
let player = mp.players.local;

mp.events.add("openBuyTunePanelBrowser", (bId) => {
    if(buyTunePanelBrowser == null && !player.getVariable("gui")){
        buyTunePanelBrowser = mp.browsers.new("package://Businesses/Tune/BuyPanel/index.html");
        mp.events.callRemote("setGui", true);
        mp.gui.cursor.show(true, true);
        businessId = bId;
    }
});

mp.events.add("closeBuyTunePanelBrowser", () => {
    buyTunePanelBrowser.destroy();
    buyTunePanelBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("setGui", false);
});

mp.events.add("buyTuneBusiness", () => {
    mp.events.callRemote("buyTuneBusiness", businessId);
});

