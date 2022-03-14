let player = mp.players.local;
let jobVehBlip = null;
let jobVeh = null;

mp.events.add("render", () => {
    if(jobVehBlip != null && mp.blips.exists(jobVehBlip) && jobVeh != null && mp.vehicles.exists(jobVeh) && mp.vehicles.streamed.includes(jobVeh)){
        jobVehBlip.position = jobVeh.getCoords(true);
    }
});

mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(entity == player && value == ""){
        if(jobVehBlip != null && mp.blips.exists(jobVehBlip)){
            jobVehBlip.destroy();
            jobVehBlip = null;
        }
    }
});

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(jobVehBlip != null && mp.blips.exists(jobVehBlip)){
        jobVehBlip.destroy();
        jobVehBlip = null;
        jobVeh = null;
    }
 });

mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(vehicle != null && mp.vehicles.exists(vehicle) && player.getVariable("jobveh") != -1111 && player.getVariable("jobveh") == vehicle.remoteId){
        if(jobVehBlip != null && mp.blips.exists(jobVehBlip)){
            jobVehBlip.destroy();
        }
        jobVehBlip = mp.blips.new(745, vehicle.position, {
            color: 15,
            name: "Pojazd pracy",
            scale: 0.8
        });
        jobVeh = vehicle;
    }
});
