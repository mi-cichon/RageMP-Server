let player = mp.players.local;
let jobTruck = null;
let pumpLabels = [];
let panelObjects = {shape: null, marker: null};
let inShape = false;
let pumpingInterval = null;
let orderLiters = 0;
let orderType = 0;
let stations = [];
let pumpBlips = [];
let pumpBlipsInterval = null;
let currentOrder = null;
mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(entity == player && value == "refinery"){
        mp.events.call("refinery_openHUDBrowser");
        createPumpBlips();
    }
    else if(entity == player && oldvalue == "refinery"){
        if(stations.length > 0){
            stations.forEach(station => {
                if(station.marker != null){
                    station.marker.destroy();
                }
                if(station.colshape != null){
                    station.colshape.destroy();
                }
                if(station.blip != null){
                    station.blip.destroy();
                }
            });
        }
        stations = [];
        if(panelObjects.shape != null){
            panelObjects.shape.destroy();
        }
        if(panelObjects.marker != null){
            panelObjects.marker.destroy();
        }
        panelObjects = {shape: null, marker: null};
        if(pumpBlipsInterval != null){
            clearInterval(pumpBlipsInterval);
            pumpBlipsInterval = null;
        }
        pumpBlips.forEach(blip => {
            if(blip != null){
                blip.destroy();
            }
        });
        pumpBlips = [];
        orderLiters = 0;
        orderType = 0;
        startPumping(false, 0);
        jobTruck = null;
        inShape = false;
        currentOrder = null;
        mp.events.call("refinery_closeHUDBrowser");
        mp.events.call("refinery_closeBrowser");
    }
});

mp.events.add("render", () => {
    if(player.hasVariable("job") && player.getVariable("job") == "refinery" && panelObjects.shape != null && mp.colshapes.exists(panelObjects.shape)){
        let sideDirection = rightVector(jobTruck.getForwardVector());
        let leftDistance = -2.1;
        let leftAway = new mp.Vector3((sideDirection.x * leftDistance) + jobTruck.getCoords(true).x, (sideDirection.y * leftDistance) + jobTruck.getCoords(true).y, (sideDirection.z * leftDistance) + jobTruck.getCoords(true).z - 1.4);
        panelObjects.shape.position = leftAway;
        panelObjects.marker.position = leftAway;
    }
})

mp.events.addDataHandler("jobveh", (entity, value, oldvalue) => {
    if(mp.players.local.getVariable("job") == "refinery"){
        if(entity == mp.players.local && value != -1111){
            let v = mp.vehicles.atRemoteId(value);
            if(v != null && mp.vehicles.exists(v) && v.hasVariable("jobtype") && v.getVariable("jobtype") == entity.getVariable("job")){
                jobTruck = v;
            }
        }
    }
});

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(jobTruck != null && mp.vehicles.exists(jobTruck) && vehicle == jobTruck){
        mp.events.callRemote("freezePublicVehicle", jobTruck, false);
        if(player.hasVariable("job") && player.getVariable("job") == "refinery" && panelObjects.shape != null && mp.colshapes.exists(panelObjects.shape)){
            panelObjects.shape.destroy();
            panelObjects.shape = null;
            panelObjects.marker.destroy();
            panelObjects.marker = null;
        }
    }
});

mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(jobTruck != null && mp.vehicles.exists(jobTruck) && vehicle == jobTruck){
        //mp.events.callRemote("freezePublicVehicle", jobTruck, true);
        let sideDirection = rightVector(jobTruck.getForwardVector());
        let leftDistance = -2.1;
        let leftAway = new mp.Vector3((sideDirection.x * leftDistance) + jobTruck.getCoords(true).x, (sideDirection.y * leftDistance) + jobTruck.getCoords(true).y, (sideDirection.z * leftDistance) + jobTruck.getCoords(true).z - 1.4);
        let shape = mp.colshapes.newTube(leftAway.x, leftAway.y, leftAway.z, 1.0, 2.0);
        let marker = mp.markers.new(1, leftAway, 1.0, {color: [0, 204, 153, 255]});
        panelObjects = {shape: shape, marker: marker};
    }
});

mp.events.add("playerEnterColshape", colshape => {
    if(player.hasVariable("job") && player.getVariable("job") == "refinery"){
        if(panelObjects.shape != null && mp.colshapes.exists(panelObjects.shape) && colshape == panelObjects.shape){
            inShape = true;
            mp.events.call("showNotification", "Naciśnij E aby otworzyć panel poboru ropy!");
        }
        if(stations.length > 0 && jobTruck != null && mp.vehicles.exists(jobTruck) && player.vehicle == jobTruck){
            for(let i = 0; i < stations.length; i++){
                let station = stations[i];
                if(station.colshape != null && mp.colshapes.exists(station.colshape) && station.colshape == colshape){
                    
                    startPumping(true, i);
                    mp.events.callRemote("freezePublicVehicle", jobTruck, true);
                    break;
                }
            }
        }
    }
});

mp.events.add("playerExitColshape", colshape => {
    if(player.hasVariable("job") && player.getVariable("job") == "refinery"){
        if(panelObjects.shape != null && mp.colshapes.exists(panelObjects.shape) && colshape == panelObjects.shape){
            inShape = false;
        }
    }
});

mp.keys.bind(0x45, true, function(){
    if(player.hasVariable("job") && player.getVariable("job") == "refinery" && inShape){
        inShape = false;
        mp.events.callRemote("refinery_openPumpingPanel", jobTruck);
    }
});


mp.events.add("refinery_setNewStations", (data, job, order) => {
    currentOrder = order;
    let index = 0;
    orderType = job;
    orderLiters = 0;
    mp.console.logInfo(data);
    data = JSON.parse(data);
    for(let key in data){
        let pos = keyToPos(key);
        let value = data[key];
        orderLiters += parseInt(value);
        let marker = mp.markers.new(0, new mp.Vector3(pos.x, pos.y, pos.z + 2), 0.8, {
            color: [255, 0, 0, 255]
        });
        let blip = mp.blips.new(535 + index, pos, {
            color: 49,
            name: "Zbiornik do napełnienia",
            scale: 0.8
        });
        let colshape = mp.colshapes.newTube(pos.x, pos.y, pos.z, 2.0, 3.0);
        stations.push({colshape: colshape, blip: blip, marker: marker, value: parseInt(value)});
        index++;
    }
    refreshStationValues();
});


function refreshStationValues(){
    let info = [];
    stations.forEach((station, index) => {
        info.push([index, station.value]);
    });
    mp.events.call("refinery_refreshHUDValues", JSON.stringify(info), jobTruck != null && mp.vehicles.exists(jobTruck) ? parseInt(jobTruck.getVariable("oiltank")) : 0);
}

mp.events.add("refinery_refreshStationValues", () => {
    refreshStationValues();
});

function keyToPos(key){
    let nums = key.split(" ");
    return new mp.Vector3(parseFloat(nums[0].replace(',', '.')), parseFloat(nums[1].replace(',', '.')), parseFloat(nums[2].replace(',', '.'))); 
}


function startPumping(state, index){
    if(state){
        if(pumpingInterval != null){
            clearInterval(pumpingInterval);
        }
        pumpingInterval = setInterval(function(){
            if(jobTruck != null && mp.vehicles.exists(jobTruck)){
                refreshStationValues();
                if(jobTruck.getVariable("oiltank") > 0){
                    if(stations[index].value > 0){
                        let oilToPump = 100;
                        if(jobTruck.getVariable("oiltank") < 100){
                            oilToPump = jobTruck.getVariable("oiltank");
                        }
                        if(stations[index].value < oilToPump){
                            oilToPump = stations[index].value;
                        }
                        stations[index].value -= oilToPump;
                        mp.events.callRemote("refinery_updateTruckOil", jobTruck, jobTruck.getVariable("oiltank") - oilToPump)
                    }
                    else{
                        clearInterval(pumpingInterval);
                        pumpingInterval = null;
                        mp.events.call("showNotification", "Zbiornik dystrybutora jest już pełny!");
                        mp.events.callRemote("freezePublicVehicle", jobTruck, false);
                        checkStatus();
                    }
                }
                else{
                    clearInterval(pumpingInterval);
                    pumpingInterval = null;
                    mp.events.call("showNotification", "Zbiornik w pojeździe jest pusty!");
                    mp.events.callRemote("freezePublicVehicle", jobTruck, false);
                }
            }
            else{
                clearInterval(pumpingInterval);
                pumpingInterval = null;
                mp.events.call("showNotification", "Wystąpił problem z pojazdem!");
                
            }
        },1000)
    }
    else{
        if(pumpingInterval != null){
            clearInterval(pumpingInterval);
            pumpingInterval = null;
        }
    }
}

function checkStatus(){
    if(stations.length > 0){
        let status = stations.every(station => {return station.value == 0});
        if(status){
            stations.forEach(station => {
                if(station.marker != null){
                    station.marker.destroy();
                }
                if(station.colshape != null){
                    station.colshape.destroy();
                }
                if(station.blip != null){
                    station.blip.destroy();
                }
            });
            stations = [];
            mp.events.callRemote("refinery_payment", orderLiters, orderType, currentOrder);
            mp.events.call("refinery_selectJob");
        }
    }
}

function createPumpBlips(){
    pumpBlips.push(mp.blips.new(436, new mp.Vector3(1577.3118, -1996.7949, 93.396736), {
        color: 1,
        shortRange: false,
        scale: 1,
        name: "Pole naftowe El Burro Heights"
    }))
    pumpBlips.push(mp.blips.new(436, new mp.Vector3(601.8342, 2921.3938, 40.71636), {
        color: 1,
        shortRange: false,
        scale: 1,
        name: "Pole naftowe Grand Senora"
    }))
}

