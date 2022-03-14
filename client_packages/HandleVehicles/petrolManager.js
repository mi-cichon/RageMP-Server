let player = mp.players.local;
let petrolInterval = null;
let updatePetrolInterval = null;
let currentPetrol = null;
let combustion = null;
let petrolTank = null;
let veh = null;
let refuelVeh = null
let refueling = false;
let refuelInterval = null;
let updatePassengerPetrolInterval = null;
mp.events.add("render", () => {
    if(player.vehicle && currentPetrol != null && mp.game.controls.isControlPressed(27,71) && player.vehicle.getIsEngineRunning()){
        startCombustion(true);
    }
    else{
        if(currentPetrol && !mp.game.controls.isControlPressed(27,71)){
            startCombustion(false);
        }
    }
    if(player.vehicle && mp.vehicles.exists(player.vehicle) && player.vehicle.getIsEngineRunning() && player.vehicle.hasVariable("petrol") && player.vehicle.getVariable("petrol") < 0){
        player.vehicle.setEngineOn(false, true, true);
        mp.events.callRemote("vehc_switchEngine", false);
    }
});

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(vehicle && vehicle.hasVariable("petrol") && seat == -1){
        currentPetrol = vehicle.getVariable("petrol");
        petrolTank = vehicle.getVariable("petroltank");
        combustion = vehicle.getVariable("combustion");
        
        mp.events.call("setPetrolLevel", currentPetrol/petrolTank);
        veh = vehicle;
        if(updatePetrolInterval){
            clearInterval(updatePetrolInterval);
            updatePetrolInterval = null;
        }
            
        updatePetrolInterval = setInterval(() => {
            mp.events.call("setPetrolLevelHard", currentPetrol/petrolTank);
            if(veh && mp.vehicles.exists(veh) && veh.doesExist() && currentPetrol != null)
                mp.events.callRemote("savePetrolLevel", veh, currentPetrol);
        }, 5000);
    }
    else if(vehicle && vehicle.hasVariable("type") && vehicle.getVariable("type") === "personal" && seat != -1){
        if(updatePassengerPetrolInterval != null){
            clearInterval(updatePassengerPetrolInterval);
            updatePassengerPetrolInterval = null;
        }
        updatePassengerPetrolInterval = setInterval(function(){
            if(player.vehicle){
                mp.events.call("setPetrolLevelHard", vehicle.getVariable("petrol")/vehicle.getVariable("petroltank"));
            }
        }, 1000);
    }
 });

mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(vehicle && mp.vehicles.exists(vehicle) && vehicle.doesExist() && vehicle.hasVariable("petrol")){
        startCombustion(false);
        if(player.vehicle && currentPetrol)
        if(currentPetrol != null)
                mp.events.callRemote("savePetrolLevel", vehicle, currentPetrol);
            currentPetrol = null;
            petrolTank = null;
            combustion = null;
            veh = null;
            if(updatePetrolInterval){
                clearInterval(updatePetrolInterval);
                updatePetrolInterval = null;
            }
    }
    else{
        if(updatePetrolInterval){
            clearInterval(updatePetrolInterval);
            updatePetrolInterval = null;
        }
        currentPetrol = null;
        petrolTank = null;
        combustion = null;
        veh = null;
    }
    if(updatePassengerPetrolInterval != null){
        clearInterval(updatePassengerPetrolInterval);
        updatePassengerPetrolInterval = null;
    }
    
});


function startCombustion(val){
    if(val == false && petrolInterval){
        clearInterval(petrolInterval);
        petrolInterval = null;
    }
    else if(!petrolInterval){
        petrolInterval = setInterval(() => {
            if(currentPetrol >= 0){
                currentPetrol -= 0.00015 * combustion * (1 - (player.getVariable("skill-2")*2/100));
            }
        }, 100);
    }
}

function getDistance(vec1, vec2)
{
     return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}




