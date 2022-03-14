let mechBrowser = null;
let mechColshape = null;
let player = mp.players.local;
let stationVeh = null;

mp.events.add("openMechHUD", (vehicle, modelPrice, mechCol) => {
    if(!player.getVariable("gui")){
        stationVeh = vehicle;
        mechColshape = mechCol;
        mechBrowser = mp.browsers.new('package://Mechanic/index.html');
        mp.gui.cursor.show(true, true);
        mechBrowser.execute(`setInfo('${vehicle.getVariable("id")}', '${vehicle.getVariable("name")}', '${vehicle.getVariable("damage")}', ${modelPrice});`)
        mp.events.callRemote("setGui", true);
    }
});

mp.events.add("mech_confirmRepair", (price, time, names) => {
    mp.events.callRemote("mech_confirmRepair", stationVeh, parseInt(price), parseInt(time), names, mechColshape);
});

mp.events.add("closeMechBrowser", () => {
    if(mechBrowser){
        mechBrowser.destroy();
    }
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("setGui", false);
});
