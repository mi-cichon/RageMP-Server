let carMarketBrowser = null;
let player = mp.players.local;
mp.events.add("openCarMarketBrowser", (name) => {
    if(!mp.browsers.exists(carMarketBrowser) && !player.getVariable("gui")){
        carMarketBrowser = mp.browsers.new("package://CarMarket/index.html");
        carMarketBrowser.execute(`setName('${name}');`);
        mp.gui.cursor.show(true, true);
        mp.events.callRemote("setGui", true);
    }
});

mp.events.add("closeCarMarketBrowser", () => {
    carMarketBrowser.destroy();
    carMarketBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("setGui", false);
});

mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(mp.browsers.exists(carMarketBrowser))
    {
        carMarketBrowser.destroy();
        mp.events.callRemote("setGui", false);
    }
});

mp.events.add("confirmCarMarket", (price, desc) => {
    mp.events.callRemote("setGui", false);
    carMarketBrowser.destroy();
    carMarketBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("applyCarToMarket", player.vehicle, price, desc);
});

