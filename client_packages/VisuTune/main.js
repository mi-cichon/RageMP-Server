let visuTuneBrowser = null;
let player = mp.players.local;
let veh;
mp.events.add("openVisuTuneBrowser", (data, currentData, vehicle) => {
    if(visuTuneBrowser){
        mp.events.callRemote("setGui", false);
        visuTuneBrowser.destroy();
        visuTuneBrowser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        veh = vehicle;
        mp.events.callRemote("setGui", true);
        visuTuneBrowser = mp.browsers.new('package://VisuTune/index.html');
        mp.gui.cursor.show(true, true);
        let tune = JSON.parse(data);
        vehicle.freezePosition(true);
        tune.forEach(t => {
            if(vehicle.getNumMods(parseInt(t[0])) > 0 && vehicle.getNumMods(parseInt(t[0])) != 255 && !isVehiclesTuneBlocked(vehicle.model, parseInt(t[0])))
                visuTuneBrowser.execute(`insertData('${t[1]}','${t[0]}',${vehicle.getNumMods(parseInt(t[0]))}, ${parseInt(t[2])})`);
            else if(parseInt(t[0]) == 22){
                visuTuneBrowser.execute(`insertData('${t[1]}','${t[0]}', 1, ${parseInt(t[2])})`);
            }
        });
        visuTuneBrowser.execute(`insertCurrent('${currentData}')`);
    }
});

mp.events.add("refreshVisuTuneBrowser", (data, currentData, vehicle) => {
    if(visuTuneBrowser){
        visuTuneBrowser.execute("removeData()");
        veh = vehicle;
        let tune = JSON.parse(data);
        tune.forEach(t => {
            if(vehicle.getNumMods(parseInt(t[0])) > 0 && vehicle.getNumMods(parseInt(t[0])) != 255 && !isVehiclesTuneBlocked(vehicle.model, parseInt(t[0])))
                visuTuneBrowser.execute(`insertData('${t[1]}','${t[0]}',${vehicle.getNumMods(parseInt(t[0]))}, ${parseInt(t[2])})`);
            else if(parseInt(t[0]) == 22){
                visuTuneBrowser.execute(`insertData('${t[1]}','${t[0]}', 1, ${parseInt(t[2])})`);
            }
        });
        visuTuneBrowser.execute(`insertCurrent('${currentData}')`);
    }
});

mp.events.add("closeVisuTuneBrowser", () => {
    if(visuTuneBrowser){
        mp.events.callRemote("setGui", false);
        visuTuneBrowser.destroy();
        visuTuneBrowser = null;
        mp.gui.cursor.show(false, false);
        veh.freezePosition(false);
        mp.events.call("switchVehiclesDoor", false);
    }
});

mp.events.add("showVisuTune", (id, i) => {
    mp.events.callRemote("previewVisuTune", veh, parseInt(id), parseInt(i));
});

mp.events.add("removeVisu", (id, i, price) => {
    mp.events.callRemote("removeVisuTune", veh, parseInt(id), parseInt(i), parseInt(price));
});


mp.events.add("applyVisuTune", (id, i, price) => {
    mp.events.callRemote("applyVisuTune", veh, parseInt(id), parseInt(i), parseInt(price));
});

mp.events.add("removeVisuType", (id) => {
    if(visuTuneBrowser){
        visuTuneBrowser.execute(`removeType(${id})`);
    }
});

mp.events.add("showVehicleVisuTune", () => {
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("setGui", false);
    setTimeout(function(){
        mp.gui.cursor.show(true, true);
        if(visuTuneBrowser != null){
            visuTuneBrowser.execute(`showBody()`);
            mp.gui.cursor.show(true, true);
            mp.events.callRemote("setGui", true);
        }
    },5000);
});

mp.events.add("switchVehiclesDoor", (state) => {
    if(player.vehicle){
        for(let i = 0; i < 8; i++){
            if(state)
                player.vehicle.setDoorOpen(i, false, true);
            else
                player.vehicle.setDoorShut(i, true);
        }
    }
});


const blockedTunes = [
    {model: 3884762073, tunes: [10]}
]


function isVehiclesTuneBlocked(model, tune){
    blockedTunes.forEach(veh => {
        if(veh.model == model && veh.tunes.includes(tune)){
            return true;
        }
    })
    return false;
}