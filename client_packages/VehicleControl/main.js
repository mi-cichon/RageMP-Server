let vehicleControlBrowser = null;
let player = mp.players.local;

mp.events.add("vehc_switchHUD", state => {
    if(vehicleControlBrowser && mp.browsers.exists(vehicleControlBrowser)){
        vehicleControlBrowser.execute(`switchMenu(${state})`);
    }
});


mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(vehicleControlBrowser && mp.browsers.exists(vehicleControlBrowser)){
        vehicleControlBrowser.destroy();
        vehicleControlBrowser = null;
    }
})

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    mp.players.local.setConfigFlag(429, true);
    if(vehicleControlBrowser && mp.browsers.exists(vehicleControlBrowser)){
        vehicleControlBrowser.destroy();
    }

    let lights = false, engine = false, parking = false, seatbelt = false, locks = false;
    let type = "";

    player.setConfigFlag(32 , true);

    if(vehicle.hasVariable("type")){
        type = vehicle.getVariable("type");
        if(!(seat != -1 && vehicle.getClass==8)){
            vehicleControlBrowser = mp.browsers.new("package://VehicleControl/index.html");
            vehicleControlBrowser.execute(`setType('${type}', ${player.vehicle.getClass() != 8}, ${seat == -1})`);
            if(vehicle.hasVariable("veh_lights") && vehicle.getVariable("veh_lights")){
                lights = true;
            }
            if(vehicle.hasVariable("veh_engine") && !vehicle.getVariable("veh_engine")){
                vehicle.setEngineOn(false, true, true);
            }
            else if(vehicle.hasVariable("veh_engine") && vehicle.getVariable("veh_engine")){
                vehicle.setEngineOn(true, true, true);
                engine = true;
            }
            if(vehicle.hasVariable("veh_brake") && vehicle.getVariable("veh_brake")){
                parking = true;
            }
            if(vehicle.hasVariable("veh_locked") && vehicle.getVariable("veh_locked")){
                locks = true;
            }
            vehicleControlBrowser.execute(`setValues(${lights}, ${engine}, ${parking}, ${seatbelt}, ${locks})`);
        }
    }
    
});

mp.events.add("vehc_switchLights", state => {
    mp.events.callRemote("vehc_switchLights", state);
});

mp.events.add("vehc_switchEngine", state => {
    if(player.vehicle){
        if(!state){
            player.vehicle.setEngineOn(false, true, true);
            mp.events.callRemote("vehc_switchEngine", state);
        }
        else{
            if(!(player.vehicle.hasVariable("petrol") && player.vehicle.getVariable("petrol") <= 0)){
                player.vehicle.setEngineOn(true, true, true);
                mp.events.callRemote("vehc_switchEngine", state);
            }
            else{
                if(mp.browsers.exists(vehicleControlBrowser)){
                    vehicleControlBrowser.execute(`setEngine(false)`);
                }
            }
        }
    }
});

mp.events.add("vehc_switchParkingbrake", state => {
    if(player.vehicle && player.vehicle.getSpeed() < 1){
        mp.events.callRemote("vehc_switchParkingbrake", state);
    }
    else{
        if(mp.browsers.exists(vehicleControlBrowser)){
            vehicleControlBrowser.execute(`setParkingBrake(false)`);
        }
    }
});

mp.events.add("vehc_switchSeatbelt", state => {
    player.setConfigFlag(32 , !state);
});

mp.events.add("vehc_switchLocks", state => {
    mp.events.callRemote("vehc_switchLocks", state);
});

mp.events.add("vehc_kickPassengers", () => {
    mp.events.callRemote("vehc_kickPassengers");
});

mp.events.add("vehc_move", state => {
    if(mp.browsers.exists(vehicleControlBrowser)){
        vehicleControlBrowser.execute(`move(${state})`);
    }
});

mp.events.add("vehc_select", () => {
    if(mp.browsers.exists(vehicleControlBrowser)){
        vehicleControlBrowser.execute(`select()`);
    }
});

mp.events.add("vehc_setEngine", (state) => {
    if(mp.browsers.exists(vehicleControlBrowser)){
        vehicleControlBrowser.execute(`setEngine(${state})`);
    }
});

mp.events.add('playerStartExitVehicle', (player) => {
    if (player.vehicle.hasVariable("veh_engine") && player.vehicle.getVariable("veh_engine")) player.vehicle.setEngineOn(true, true, true);
});