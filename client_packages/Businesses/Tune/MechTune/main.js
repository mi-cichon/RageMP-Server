let mechTuneBrowser = null;
let player = mp.players.local;
let veh;
let businessId;
mp.events.add("openMechTuneBrowser", (bId, currentTune, vehicle) => {
    if(mechTuneBrowser){
        mp.events.callRemote("setGui", false);
        mechTuneBrowser.destroy();
        mechTuneBrowser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        businessId = bId;
        veh = vehicle;
        mp.events.callRemote("setGui", true);
        mechTuneBrowser = mp.browsers.new('package://Businesses/Tune/MechTune/index.html');
        mp.gui.cursor.show(true, true);
        mechTuneBrowser.execute(`setTune('${currentTune}');`);
    }
});

mp.events.add("closeMechTuneBrowser", () => {
    if(mechTuneBrowser){
        mp.events.callRemote("setGui", false);
        mechTuneBrowser.destroy();
        mechTuneBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("sendMechTuneOffer", (installType, id, name, fullprice, offer) => {
    mp.events.callRemote("sendMechTuneOffer", veh, businessId, installType, id, name, fullprice, offer);
});