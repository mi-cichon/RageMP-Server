let lspdStorageBrowser = null;
let player = mp.players.local;
mp.events.add("openLspdStorageBrowser", (d) => {
    if(lspdStorageBrowser && mp.browsers.exists(lspdStorageBrowser)){
        lspdStorageBrowser.destroy();
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setGui", false);
    }
    else if(!player.getVariable("gui")){
        lspdStorageBrowser = mp.browsers.new('package://Groups/LSPD/LSPDStorage/index.html');
        mp.gui.cursor.show(true, true);
        mp.events.callRemote("setGui", true);
        let data = JSON.parse(d);
        data.forEach(veh => {
            lspdStorageBrowser.execute(`insertCar('${veh[0]}', '${veh[1]}', '${veh[2]}');`);
        });
    }
    
});

mp.events.add("closeLspdStorageBrowser", () => {
    if(lspdStorageBrowser){
        lspdStorageBrowser.destroy();
    }
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("setGui", false);
});

mp.events.add("createLspdVehicle", (id) =>{
    mp.events.callRemote("lspd_CreateVehicle", id);
});