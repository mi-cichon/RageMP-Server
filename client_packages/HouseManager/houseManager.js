let player = mp.players.local;
let blipsShown = false;
mp.events.add("setHouseAsOwn", (houseblip, housemarker) => {
    houseblip.setColour(77);
    housemarker.colour = [60, 60, 255, 255];
    houseblip.setScale(1.0)
    houseblip.setAlpha(255);
})

mp.events.add("setHouseAsNotOwn", (houseblip, housemarker) => {
    houseblip.setColour(2);
    housemarker.colour = [60, 255, 60, 255];
    houseblip.setScale(0.6)
    houseblip.setAlpha(0);
})

mp.events.add("render", () => {

    
    mp.markers.forEach((marker) => {
        if(marker != null && marker.position != null && marker.hasVariable("ownername")  && getDistance(player.position, marker.position) < 10)
        {
            point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(marker.position.x, marker.position.y, marker.position.z + 1.0));
            if(marker.getVariable("ownername") == "")
            {
                if(point && point.x < 1 && point.y < 1 && point.x > 0 && point.y > 0){
                    let pos = new Float64Array([point.x,point.y]);
                    mp.game.graphics.drawText("Dom do wynajęcia", pos, { 
                        font: 4, 
                        color: [60, 255, 60, 200], 
                        scale: [0.6, 0.6], 
                        outline: true
                    });
                }
            }
            else
            {
                if(point && point.x < 1 && point.y < 1 && point.x > 0 && point.y > 0){
                    let pos = new Float64Array([point.x,point.y]);
                    mp.game.graphics.drawText("Dom " + marker.getVariable("ownername"), pos, { 
                        font: 4, 
                        color: [255, 60, 60, 200], 
                        scale: [0.6, 0.6], 
                        outline: true
                    });
                }
            }
        }

        if(mp.keys.isDown(77) && !blipsShown && !player.getVariable("controlsblocked")){
            blipsShown = true;
            mp.blips.forEach((blip) => {
                if(blip.hasVariable("houseid") && blip.getVariable("houseid") != player.getVariable("houseid")){
                    blip.setAlpha(255);
                }
            })
        }
        if(mp.keys.isUp(77) && blipsShown){
            blipsShown = false;
            mp.blips.forEach((blip) => {
                if(blip.hasVariable("houseid") && blip.getVariable("houseid") != player.getVariable("houseid")){
                    blip.setAlpha(0);
                }
            })
        }
    });


    // mp.colshapes.forEachInDimension(player.position, 10, 0, (colshape) => {
    //     mp.console.logInfo(colshape.getVariable("type"));
    // });


    // mp.colshapes.forEachInStreamRange((colshape) => {
    //     if(colshape != null && colshape.position != null && colshape.hasVariable("type") && colshape.getVariable("type") == "house" && getDistance(player.position, colshape.position) < 10){
    //         point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(colshape.position.x, colshape.position.y, colshape.position.z + 1.6));
    //         if(colshape.getVariable("owner") == "")
    //         {
    //             if(point && point.x < 1 && point.y < 1 && point.x > 0 && point.y > 0){
    //                 let pos = new Float64Array([point.x,point.y]);
    //                 mp.game.graphics.drawText("Dom do wynajęcia", pos, { 
    //                     font: 0, 
    //                     color: [255, 255, 255, 200], 
    //                     scale: [0.7, 0.7], 
    //                     outline: true
    //                 });
    //             }
    //         }
    //         else
    //         {
    //             if(point && point.x < 1 && point.y < 1 && point.x > 0 && point.y > 0){
    //                 let pos = new Float64Array([point.x,point.y]);
    //                 mp.game.graphics.drawText(colshape.getVariable("ownername"), pos, { 
    //                     font: 0, 
    //                     color: [60, 255, 60, 200], 
    //                     scale: [0.7, 0.7], 
    //                     outline: true
    //                 });
    //             }
    //         }
            
    //     }
    // });
});


function getDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}


mp.events.add("setHouseBlipsInvisible", () => {
    mp.blips.forEach((blip) => {
        if(blip.hasVariable("houseid") && blip.getVariable("houseid") != player.getVariable("houseid")){
            blip.setAlpha(0);
        }
    })
});