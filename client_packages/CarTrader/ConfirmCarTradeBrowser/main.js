let carTrade_browser = null;
let player = mp.players.local;
let vehicle_id = null;
let veh_price = null;
let buyTimeout = null;
let seller = null;
mp.events.add("carTrade_openBrowser", (sel, veh, price, vehInfo, mechInfo, visuInfo) => {
    if(carTrade_browser){
        mp.events.callRemote("setGui", false);
        carTrade_browser.destroy();
        carTrade_browser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        vehicle_id = veh;
        veh_price = price;
        seller = sel;
        mp.events.callRemote("setGui", true);
        carTrade_browser = mp.browsers.new('package://CarTrader/ConfirmCarTradeBrowser/index.html');
        mp.gui.cursor.show(true, true);
        carTrade_browser.execute(`setVehiclesData('${seller.getVariable("username")}', '${price}', '${vehInfo}', '${mechInfo}', '${visuInfo}');`);
        buyTimeout = setTimeout(function(){
            if(mp.browsers.exists(carTrade_browser)){
                mp.events.call("showNotification", "Czas na akceptację oferty minął!");
                mp.events.callRemote("setGui", false);
                carTrade_browser.destroy();
                carTrade_browser = null;
                mp.gui.cursor.show(false, false);
                buyTimeout = null;
                mp.events.callRemote("setTradeOffer", false);
            }
        }, 30000)
    }
});

mp.events.add("carTrade_closeBrowser", () => {
    if(carTrade_browser){
        mp.events.callRemote("setGui", false);
        carTrade_browser.destroy();
        carTrade_browser = null;
        mp.gui.cursor.show(false, false);
        if(buyTimeout != null){
            clearTimeout(buyTimeout);
            mp.events.callRemote("setTradeOffer", false);
        }
    }
});

mp.events.add("carTrade_confirmTrade", () => {
    mp.events.callRemote("carTrade_confirmTrade", seller, vehicle_id, veh_price);
    if(buyTimeout != null){
        clearTimeout(buyTimeout);
        mp.events.callRemote("setTradeOffer", false);
    }
});