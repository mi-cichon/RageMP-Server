let lplayer = mp.players.local;
let vehicleBlips = [];
let playerBlips = [];
let streamedBlips = [];

mp.events.add("updatePlayerBlips", () => {
    playerBlips.forEach((blip) => {
        blip.destroy();
    });
    playerBlips = [];
    mp.players.forEach((player) => {
        if(player!=lplayer && !mp.players.streamed.includes(player) && player.hasVariable("lastpos") && player.hasVariable("username") && !(player.vehicle && lplayer.vehicle && lplayer.vehicle == player.vehicle) && player.getVariable("lastpos") != new mp.Vector3(0,0,0) && player.dimension == lplayer.dimension  && !(player.hasVariable("spec") && player.getVariable("spec")))
        {
            
            let color = 4;
            let name = player.getVariable("username")
            if(player.hasVariable("orgId") && lplayer.hasVariable("orgId") && lplayer.getVariable("orgId") != 0 && lplayer.getVariable("orgId") == player.getVariable("orgId")){
                color = 83;
            }
            if(player.hasVariable("lspd_duty") && player.getVariable("lspd_duty") && lplayer.hasVariable("lspd_duty") && lplayer.getVariable("lspd_duty")){
                color = 38;
            }
            let blip = mp.blips.new(1, player.getVariable("lastpos"),{
                name: name,
                color: color,
                scale: 0.8
            })
            blip.setCategory(7);
            playerBlips.push(blip);
        }
    });
});

mp.events.add("updateVehicleBlips", () => {
    vehicleBlips.forEach((blip) => {
        blip.destroy();
    });
    vehicleBlips = [];
    mp.vehicles.forEach((vehicle) => {
        if(vehicle.hasVariable("owner") && lplayer.getVariable("socialclub") == vehicle.getVariable("owner").toString() && lplayer.vehicle != vehicle && !vehicle.hasVariable("destroyed"))
        {
            vehicleBlips.push(mp.blips.new(225, vehicle.getVariable("lastpos"), {
                name: vehicle.getVariable("id") + ": " + vehicle.getVariable("name"), 
                color: 47,
                scale: 0.6
            }));
        }
        else if(vehicle.hasVariable("orgId") && lplayer.hasVariable("orgId") && lplayer.getVariable("orgId") != 0 && lplayer.getVariable("orgId") == vehicle.getVariable("orgId") && lplayer.vehicle != vehicle && !vehicle.hasVariable("destroyed")){
            vehicleBlips.push(mp.blips.new(225, vehicle.getVariable("lastpos"), {
                name: vehicle.getVariable("id") + ": " + vehicle.getVariable("name") + "(Organizacja)", 
                color: 83,
                scale: 0.6,
            }));
        }
        else if(lplayer.hasVariable("lspd_duty") && lplayer.getVariable("lspd_duty") && vehicle.hasVariable("type") && vehicle.getVariable("type") == "lspd" && !(lplayer.vehicle && lplayer.vehicle == vehicle)){
            vehicleBlips.push(mp.blips.new(225, vehicle.getVariable("lastpos"), {
                name: vehicle.getVariable("id") + ": " + vehicle.getVariable("name") + "(LSPD)", 
                color: 38,
                scale: 0.6,
            }));
        }
    });
});

// mp.events.add("render", () => {
//     streamedBlips.forEach((blip) => {
//         blip.destroy();
//     });
//     streamedBlips = [];
//     mp.players.forEachInStreamRange((player)=>{
//         if(player!=lplayer && player.hasVariable("lastpos") && player.hasVariable("username") && !(player.vehicle && lplayer.vehicle && lplayer.vehicle == player.vehicle) && player.getVariable("lastpos") != new mp.Vector3(0,0,0) && player.dimension == lplayer.dimension  && !(player.hasVariable("spec") && player.getVariable("spec")))
//         {
//             let color = 4;
//             let name = "Gracz"
//             if(player.hasVariable("orgId") && lplayer.hasVariable("orgId") && lplayer.getVariable("orgId") != 0 && lplayer.getVariable("orgId") == player.getVariable("orgId")){
//                 color = 83;
//             }
//             streamedBlips.push(mp.blips.new(1, player.getVariable("lastpos"),{
//                 name: "Gracz",
//                 color: color,
//                 scale: 0.8
//             }));
//         }
//     })
//});

mp.events.add('entityStreamIn', (player) => {    
    if (player.type === 'player') 
    {
        if(!(player.hasVariable("spec") && player.getVariable("spec")) && player.hasVariable("username") && !(player.vehicle && lplayer.vehicle && lplayer.vehicle == player.vehicle) && player.dimension == lplayer.dimension){
            let color = 4;
            let name = player.getVariable("username")
            if(player.hasVariable("orgId") && lplayer.hasVariable("orgId") && lplayer.getVariable("orgId") != 0 && lplayer.getVariable("orgId") == player.getVariable("orgId")){
                color = 83;
            }
            if(player.hasVariable("lspd_duty") && player.getVariable("lspd_duty") && lplayer.hasVariable("lspd_duty") && lplayer.getVariable("lspd_duty")){
                color = 38;
            }
            const blipHandle = mp.game2.hud.getBlipFromEntity(player.handle) || mp.game2.hud.addBlipForEntity(player.handle);
            mp.game2.hud.setBlipColour(blipHandle, isNaN(color) ? 0 : color);
            mp.game2.hud.setBlipCategory(blipHandle, 7);
            mp.game2.hud.setBlipScale(blipHandle, 0.8);
            mp.game.invoke("0x5FBCA48327B914DF", blipHandle, true);
            mp.game.invoke("0xF9113A30DE5C6670", 'STRING');
            mp.game.ui.addTextComponentSubstringPlayerName(name);
            mp.game.invoke("0xBC38B49BCB83BC9B", blipHandle);
        }
    }
});

mp.events.addDataHandler("spec", (entity, value, oldvalue) => {
    if(value == true){
        const blipHandle = mp.game2.hud.getBlipFromEntity(entity.handle);
        if(blipHandle){
            mp.game2.hud.setBlipAlpha(blipHandle, 0);
        }
    }
    else{
        const blipHandle = mp.game2.hud.getBlipFromEntity(entity.handle);
        if(blipHandle){
            mp.game2.hud.setBlipAlpha(blipHandle, 255);
        }
    }
});

mp.events.addDataHandler("username", (entity, value, oldvalue) => {
    if(entity === mp.players.local){
        mp.players.local.name = value;
    }
});


