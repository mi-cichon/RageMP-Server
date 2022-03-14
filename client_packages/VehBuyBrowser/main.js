let vehBuyBrowser = null;
let player = mp.players.local;
let vehicle = null;
mp.events.add("openVehBuyBrowser", (data, veh) => {
    if(vehBuyBrowser){
        mp.events.callRemote("setGui", false);
        vehBuyBrowser.destroy();
        vehBuyBrowser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        vehicle = veh;
        mp.events.callRemote("setGui", true);
        vehBuyBrowser = mp.browsers.new('package://VehBuyBrowser/index.html');
        mp.gui.cursor.show(true, true);
        let d = JSON.parse(data);
        vehBuyBrowser.execute(`insertData('${d[0]}', '${d[1]}', '${d[2]}', '${d[3]}', '${d[4]}', '${d[5]}');`);
    }
});

mp.events.add("closeVehBuyBrowser", () => {
    if(vehBuyBrowser){
        mp.events.callRemote("setGui", false);
        vehBuyBrowser.destroy();
        vehBuyBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("buyVehicleHUD", () => {
    mp.events.callRemote("confirmVehicleBuy", vehicle);
});