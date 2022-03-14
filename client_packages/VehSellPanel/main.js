let sellVeh_browser = null;
let player = mp.players.local;
let vehicle = null;
let veh_price = null;;
mp.events.add("sellVeh_openBrowser", (veh, name, price, trip) => {
    if(sellVeh_browser){
        mp.events.callRemote("setGui", false);
        sellVeh_browser.destroy();
        sellVeh_browser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        vehicle = veh;
        veh_price = price;
        mp.events.callRemote("setGui", true);
        sellVeh_browser = mp.browsers.new('package://VehSellPanel/index.html');
        mp.gui.cursor.show(true, true);
        sellVeh_browser.execute(`insertData('${name}', '${price}', '${trip}');`);
    }
});

mp.events.add("sellVeh_closeBrowser", () => {
    if(sellVeh_browser){
        mp.events.callRemote("setGui", false);
        sellVeh_browser.destroy();
        sellVeh_browser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("sellVeh_sell", () => {
    mp.events.callRemote("sellVeh_sellVehicle", vehicle, veh_price);
});