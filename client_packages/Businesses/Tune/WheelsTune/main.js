let wheelsTuneBrowser = null;
let player = mp.players.local;
let veh;
let businessId;
mp.events.add("openWheelsTuneBrowser", (bId, availableWheels, currentWheels, vehicle) => {
    if(wheelsTuneBrowser){
        mp.events.callRemote("setGui", false);
        wheelsTuneBrowser.destroy();
        wheelsTuneBrowser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        businessId = bId;
        veh = vehicle;
        mp.events.callRemote("setGui", true);
        wheelsTuneBrowser = mp.browsers.new('package://Businesses/Tune/WheelsTune/index.html');
        mp.gui.cursor.show(true, true);
        let wheels = JSON.parse(availableWheels);
        if(wheels.length > 0){
            wheelsTuneBrowser.execute(`setOwnedWheels('${availableWheels}', ${vehicle.getClass()})`);
            wheelsTuneBrowser.execute(`setCurrentWheels('${currentWheels}')`);
        }
    }
});

mp.events.add("closeWheelsTuneBrowser", () => {
    if(wheelsTuneBrowser){
        mp.events.callRemote("setGui", false);
        wheelsTuneBrowser.destroy();
        wheelsTuneBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("removeCurrentWheels", (price) => {
    mp.events.callRemote("removeCurrentWheels", veh, price);
});

mp.events.add("previewWheelTune", (type, id, sport) => {
    mp.events.callRemote("previewWheelTune", veh, type, id, sport, businessId);
});

mp.events.add("sendWheelTuneOffer", (installType, name, type, id, price, partId = -1) => {
    mp.events.callRemote("sendWheelTuneOffer", veh, businessId, installType, name, type, id, price, partId);
});