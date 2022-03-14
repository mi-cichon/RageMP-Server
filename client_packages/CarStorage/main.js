let storageBrowser = null;

mp.events.add("openStorageHUD", (sId) => {
    storageBrowser = mp.browsers.new('package://CarStorage/index.html');
    mp.events.callRemote("requestStorageData");
    storageBrowser.execute(`setStorageId(${sId});`);
    mp.gui.cursor.show(true, true);
});

mp.events.add("insertStorageVehicles", (d) => {
    if(d != ""){
        let data = JSON.parse(d);
        data.forEach(veh => {
            storageBrowser.execute(`insertCar('${veh[0]}', '${veh[1]}', '${veh[2]}', '${veh[3]}');`);
        });
    }
});

mp.events.add("closeStorageBrowser", () => {
    if(storageBrowser){
        storageBrowser.destroy();
    }

    mp.gui.cursor.show(false, false);
});

mp.events.add("createVehicle", (id, sId) =>{
    mp.events.callRemote("spawnSelectedVehicle", id, sId);
});