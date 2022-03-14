 let player = mp.players.local;
 let supplyTruck = null;
 let currentTrailer = null;
 let trailerAttached = false;
 let trailerBlip = null;
 let basePosition = null;
 let baseBlip = null;
 mp.events.addDataHandler("job", (entity, value, oldvalue) => {
     if(entity.type === "player" && entity == player)
     {
        if(oldvalue === "" && value === "supplier"){
            if(player.vehicle){
                supplyTruck = player.vehicle;
            }
        }
        if(oldvalue === "supplier" && value === ""){
            
        }
    }
 });


 mp.events.add("render", () => {
    if(player.hasVariable("job") && player.getVariable("job") == "supplier" && player.vehicle != null && currentTrailer != null){
        if(getDistance(player.position, currentTrailer.position) < 5 && !trailerAttached){
            player.vehicle.attachToTrailer(currentTrailer.handle, 1.0);
            trailerAttached = true;
        }
    //     let temp;
    //     let trailerHandle = mp.game.invoke('0x1CDD6BADC297830D', player.vehicle.handle, temp);
    //     if((trailerHandle == 0 && trailerAttached) || (trailerHandle != 0 && !trailerAttached)){
    //         if(trailerHandle == 0){
    //             trailerAttached = false;
    //             mp.console.logInfo("Zgubiłeś naczepę!");
    //             trailerBlip = mp.blips.new(479, currentTrailer.position, {
    //                 color: 81,
    //                 name: "Naczepa"
    //             });
    //             trailerBlip.setRoute(true);
    //             if(baseBlip != null){
    //                 baseBlip.destroy();
    //             }
    //         }
    //         else{
    //             if(mp.vehicles.atHandle(supplyTruck.getTrailer(trailerHandle)) == currentTrailer){
    //                 trailerAttached = true;
    //                 mp.console.logInfo("To jest twoja naczepa");
    //                 trailerBlip.destroy();
    //                 trailerBlip = null;
    //                 baseBlip = mp.blips.new(357, basePosition, {
    //                     color: 81,
    //                     name: "Miejsce odstawienia naczepy"
    //                 });
    //                 baseBlip.setRoute(true);
    //             }
    //             else{
    //                 player.vehicle.detachFromTrailer();
    //                 mp.console.logInfo("To nie twoja naczepa!");
    //             }
    //         }
    //     }
    }
});

mp.events.add("markNewTrailer", (trailer, trailerPos, destination) => {
    currentTrailer = trailer;
    trailerBlip = mp.blips.new(479, trailerPos, {
        color: 81,
        name: "Naczepa"
    });
    trailerBlip.setRoute(true);
    basePosition = destination;
});

// mp.events.add("entityControllerChange", (entity, newController) => {
//     if(currentTrailer != null && newController == player && entity.hasVariable("type") && entity.getVariable("type") == "trailer"){
//         player.vehicle.attachToTrailer(currentTrailer.handle, 1.0);
//     }
// });


function getDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}

function getTime(){
    let today = new Date();
    let dd = String(today.getDate()).padStart(2, '0');
    let mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    let yyyy = today.getFullYear();
    today = mm + '/' + dd + '/' + yyyy;
    return today;
}