let player = mp.players.local;
let clicked = false;
let spectateCamera = null;
let specPlayer = null;
let lastCoords = null;
mp.events.add("render", () => {
    if(player.hasVariable("power") && player.getVariable("power") >= 3){
        if(mp.keys.isDown(8) && !clicked && !player.getVariable("controlsblocked") && !player.getVariable("gui")){
            clicked = true;
        }
        if(mp.keys.isUp(8) && clicked){
            let veh = pointingAt(100, 2);
            if(veh != null && veh){
                mp.events.callRemote("removeIndicatedVehicle", veh.entity);
            }
            clicked = false;
        }
    }
    if(spectateCamera && specPlayer){
        spectateCamera.pointAtCoord(specPlayer.position.x, specPlayer.position.y, specPlayer.position.z);
        player.position = new mp.Vector3(specPlayer.position.x, specPlayer.position.y, specPlayer.position.z + 10);
    }
    if(lastCoords != null){
        mp.game.graphics.drawLine(lastCoords[0], lastCoords[1], lastCoords[2], lastCoords[3], lastCoords[4], lastCoords[5], 255, 222, 0, 255);
    }
    
});

function pointingAt(distance, type) {
    const camera = mp.cameras.new("gameplay");
    let position = camera.getCoord();
    let direction = camera.getDirection();
    let farAway = new mp.Vector3((direction.x * distance) + (position.x), (direction.y * distance) + (position.y), (direction.z * distance) + (position.z));
    let result = mp.raycasting.testPointToPoint(position, farAway, type);
    return result;
}

mp.events.add("startSpectatingPlayer", (pl) =>{
    if(spectateCamera != null){
        spectateCamera.setActive(false);
        spectateCamera.destroy();
        spectateCamera = null;
        specPlayer = null;
    }
    player.freezePosition(true);
    setTimeout(() => {
        specPlayer = pl;
        spectateCamera = mp.cameras.new("spectCam", new mp.Vector3(0,0,0), new mp.Vector3(0,0,0), 90);
        spectateCamera.attachTo(pl.handle, 0, -4, 2, true);
        spectateCamera.setActive(true);
        mp.game.cam.renderScriptCams(true, false, 0, true, false);
    }, 500);
    
});

mp.events.add("stopSpectatingPlayer", () => {
    if(spectateCamera != null){
        player.freezePosition(false);
        spectateCamera.setActive(false);
        spectateCamera.destroy();
        spectateCamera = null;
        specPlayer = null;
        mp.game.cam.renderScriptCams(false, false, 0, false, false);
    }
});

mp.events.add("warnSound", () => {
    //mp.game.audio.playSoundFrontend(-1, "Drill_Pin_Break", "DLC_HEIST_FLEECA_SOUNDSET", true);
});

mp.events.add("reportNotify", () => {
    mp.game.audio.playSoundFrontend(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true);
});


mp.events.add("rayCastTest", () => {
    let position = player.position;
    let direction = player.getForwardVector();
    let distance = 15;
    let farAway = new mp.Vector3((direction.x * distance) + (position.x), (direction.y * distance) + (position.y), (direction.z * distance) + (position.z + 2) - 25);
    let res = mp.raycasting.testPointToPoint(position, farAway, null, -1); 
    lastCoords = [player.position.x, player.position.y, player.position.z + 2, farAway.x, farAway.y, farAway.z];
    // let res = mp.game.water.testProbeAgainstWater(player.position.x, player.position.y, player.position.z, farAway.x, farAway.y, farAway.z, true);
    // if(res){
    //     mp.events.call("showNotification", "Można łowić");
    // }else{
    //     mp.events.call("showNotification", "Nie można łowić");
    // }
    // lastCoords = [player.position.x, player.position.y, player.position.z, farAway.x, farAway.y, farAway.z];
});

mp.events.add("admin_deleteCar", () => {
    mp.events.callRemote("admin_deleteCar", contextVehicle);
});

mp.events.add("admin_bringCar", () => {
    mp.events.callRemote("admin_bringCar", contextVehicle);
});

mp.events.add("admin_lastDriver", () => {
    mp.events.callRemote("admin_lastDriver", contextVehicle);
});

mp.events.add("admin_carOwner", () => {
    mp.events.callRemote("admin_carOwner", contextVehicle);
});

mp.events.add("admin_flipCar", () => {
    mp.events.callRemote("admin_flipCar", contextVehicle);
});

mp.events.add("admin_fixCar", () => {
    mp.events.callRemote("admin_fixCar", contextVehicle);
});

let contextVehicle = null;

mp.events.add('click', (x, y, upOrDown, leftOrRight, relativeX, relativeY, worldPosition, hitEntity) => {
    if(player.hasVariable("power") && player.getVariable("power") >=3 && mp.gui.cursor.visible && !player.getVariable("gui") && upOrDown == "down" && leftOrRight == "right"){
        let closest = getClosestVehicleToCoord(worldPosition);
        if(closest != null){

            contextVehicle = closest;
            let point = mp.game.graphics.world3dToScreen2d(contextVehicle.position.x, contextVehicle.position.y, contextVehicle.position.z + 1);
            mp.events.call("openAdminContextBrowser", point.x, point,y);
        }
    }
});

function getClosestVehicleToCoord(position){
    let closest = null;
    mp.vehicles.forEachInStreamRange(vehicle => {
        if(closest == null && getDistance(vehicle.position, position) < 5){
            closest = vehicle;
        }
        else if (closest != null){
            let closestDist = getDistance(closest.position, position);
            let currentDist = getDistance(vehicle.position, position);
            if(closestDist > currentDist && currentDist < 5){
                closest = vehicle;
            }
        }
    });
    return closest;
}

function getDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}

mp.events.add("test_createObject", model => {
    mp.objects.new(model, player.position);
});

mp.events.add("test_attachVehicle", (veh1, veh2) => {
    setTimeout(()=>{
        if(mp.vehicles.exists(veh1) && mp.vehicles.exists(veh2)){
            veh2.attachTo(veh1.handle, 0, 0, -1.6, 0.5, 0, 0, 0, true, true, false, false, 0, true);
        }
    }, 3000);
    
});