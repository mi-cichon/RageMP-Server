let player = mp.players.local;
let ghostBarrier = null;
let posOffset = 0;
let rotOffset = 0;
let angle = 0;
let initialPos = 0;
let obj = false;
setInterval(function(){
    //mp.console.logInfo(obj.toString());
    
}, 5000);
mp.events.add("render", () => {
    if(ghostBarrier != null && mp.objects.exists(ghostBarrier)){
        let position = player.position;
        let direction = player.getForwardVector();
        let distance = 3;
        let farAway = new mp.Vector3((direction.x * distance) + (position.x), (direction.y * distance) + (position.y), (direction.z * distance) + (position.z));
        let objPos = ghostBarrier.position;
        let sideDirection = rightVector(ghostBarrier.getForwardVector());
        let rightDistance = 0.71;
        let leftDistance = -0.71;
        let rightAway = new mp.Vector3((sideDirection.x * rightDistance) + (objPos.x), (sideDirection.y * rightDistance) + (objPos.y), (sideDirection.z * rightDistance) + (objPos.z));
        let leftAway = new mp.Vector3((sideDirection.x * leftDistance) + (objPos.x), (sideDirection.y * leftDistance) + (objPos.y), (sideDirection.z * leftDistance) + (objPos.z));
        // mp.game.graphics.drawLine(rightAway.x, rightAway.y, rightAway.z + 2, rightAway.x, rightAway.y, rightAway.z - 2, 255, 0, 0, 255);
        // mp.game.graphics.drawLine(leftAway.x, leftAway.y, leftAway.z + 2, leftAway.x, leftAway.y, leftAway.z - 2, 255, 0, 0, 255);
        let rightResult = mp.raycasting.testPointToPoint(new mp.Vector3(rightAway.x, rightAway.y, rightAway.z + 2), new mp.Vector3(rightAway.x, rightAway.y, rightAway.z - 2), ghostBarrier, 1);
        let leftResult = mp.raycasting.testPointToPoint(new mp.Vector3(leftAway.x, leftAway.y, leftAway.z + 2), new mp.Vector3(leftAway.x, leftAway.y, leftAway.z - 2), ghostBarrier, 1);
        if(rightResult && leftResult){
            if(rightResult.position.z > leftResult.position.z){
                let diff = rightResult.position.z - leftResult.position.z;
                angle = Math.atan(getDistance(leftResult.position, rightResult.position)/diff) * 180 / Math.PI;
                angle = -1 * (90 - angle);
                diff /= 2;
                initialPos = leftResult.position.z + diff;
                obj = rightResult.entity == ghostBarrier;
            }
            else {
                let diff = leftResult.position.z - rightResult.position.z;
                angle = Math.atan(getDistance(leftResult.position, rightResult.position)/diff) * 180 / Math.PI;
                angle = 90 - angle;
                diff /= 2;
                initialPos = rightResult.position.z + diff;
            }
            
        }
        ghostBarrier.position = new mp.Vector3(farAway.x, farAway.y, initialPos);
        let rot = player.getRotation(5).z;
        ghostBarrier.setRotation(0, angle, rot, 2, false);
    }
    if(player.hasVariable("handCuffed") && player.getVariable("handCuffed")){
        mp.game.controls.disableControlAction(32, 75, true);
    }
});

mp.keys.bind(0x42, true, function (){
    if(!player.getVariable("gui") && player.vehicle == null && player.hasVariable("lspd_duty") && player.getVariable("lspd_duty") && ghostBarrier == null || (ghostBarrier != null && !mp.objects.exists(ghostBarrier))){
        let position = player.position;
        let direction = player.getForwardVector();
        let distance = 2;
        let farAway = new mp.Vector3((direction.x * distance) + (position.x), (direction.y * distance) + (position.y), (direction.z * distance) + (position.z));
        initialPos = farAway.z;
        ghostBarrier = mp.objects.new('prop_mp_barrier_02b', farAway, {
            alpha: 128
        });
        setTimeout(function(){
            ghostBarrier.setCollision(false, true);
        }, 200);
    }
});

mp.keys.bind(0x45, true, function(){
    if(!player.getVariable("gui") && ghostBarrier != null &&  mp.objects.exists(ghostBarrier) && !player.vehicle){
        mp.events.callRemote("lspd_createBarrier", ghostBarrier.position, ghostBarrier.getRotation(0));
        ghostBarrier.destroy();
        ghostBarrier = null;
    }
});

mp.keys.bind(0x51, true, function(){
    if(!player.getVariable("gui") && ghostBarrier != null &&  mp.objects.exists(ghostBarrier)){
        ghostBarrier.destroy();
        ghostBarrier = null;
    }
    else if(player.vehicle == null && player.hasVariable("lspd_duty") && player.getVariable("lspd_duty")){
        mp.events.callRemote("lspd_removeClosestBarrier");
    }
});

mp.events.addDataHandler("barrierId", (entity, value, oldvalue) => {
    setTimeout(function(){
        entity.freezePosition(true);
        entity.setInvincible(true);
    },500);
});

mp.keys.bind(0x52, true, function(){
    if(!player.getVariable("gui") && player.vehicle == null && player.hasVariable("lspd_duty") && player.getVariable("lspd_duty")){
        mp.events.callRemote("lspd_cuffClosestPlayer");
    }
});

mp.events.add("handCuff", (officer, state) => {
    if(state){
        if(player.action != "ragdoll" && player.action != "diving" && player.action != "climbing" && player.action != "moving" && player.action != "entering_vehicle" && player.vehicle == null){
            mp.game.streaming.requestAnimDict("mp_prison_break");
            player.taskPlayAnim("mp_prison_break", "handcuffed", 1.0, 1.0, -1, 50, 1.0, false, false, false);
            player.taskFollowToOffsetOf(officer.handle, 0, -1, 0, 1.0, -1, 1.0, true)
            mp.events.callRemote("setPlayerCuffed", true, officer);
        }
        else{
            mp.events.callRemote("setPlayerCuffed", false, officer);
        }
        
    }
    else{
        player.clearTasks();
        player.stopAnimTask("mp_prison_break", "handcuffed", 3.0);
    }
});

mp.events.add("setCuffed", (officer) => {
    mp.game.streaming.requestAnimDict("mp_prison_break");
    player.taskPlayAnim("mp_prison_break", "handcuffed", 1.0, 1.0, -1, 50, 1.0, false, false, false);
    player.taskFollowToOffsetOf(officer.handle, 0, -1, 0, 1.0, -1, 1.0, true)
});

mp.events.add("setPlayerCuffed", (cuffed, officer) => {
    if(cuffed != player){
        setTimeout(function(){
            mp.game.streaming.requestAnimDict("mp_prison_break");
            cuffed.taskPlayAnim("mp_prison_break", "handcuffed", 1.0, 1.0, -1, 50, 1.0, false, false, false);
            cuffed.taskFollowToOffsetOf(officer.handle, 0, -1, 0, 1.0, -1, 1.0, true)
        }, 1000);
    }
});


mp.events.addDataHandler("handCuffed", (entity, value, oldvalue) => {
    if(entity != player && value == true && entity.hasVariable("cuffedBy")){
        let officer =  entity.getVariable("cuffedBy");
        if(officer != null && mp.players.exists(officer)){ 
            setTimeout(function(){
                entity.taskFollowToOffsetOf(officer.handle, 0, -1, 0, 1.0, -1, 1.0, true);
            }, 500)
            if(!mp.game.streaming.hasAnimDictLoaded("mp_prison_break"))
                mp.game.streaming.requestAnimDict("mp_prison_break");
            entity.taskPlayAnim("mp_prison_break", "handcuffed", 1.0, 1.0, -1, 50, 1.0, false, false, false);
        }
    }
    else if(entity != player && value == false){
        entity.clearTasks();
        entity.stopAnimTask("mp_prison_break", "handcuffed", 3.0);
    }
});

mp.events.add("entityStreamIn", (entity) => {
    if(entity.hasVariable("handCuffed") && entity.getVariable("handCuffed") && entity.hasVariable("cuffedBy")){
        let officer = entity.getVariable("cuffedBy");
        if(officer != null && !isNaN(officer)){
            entity.taskFollowToOffsetOf(officer, 0, -1, 0, 1.0, -1, 1.0, true);
            if(!mp.game.streaming.hasAnimDictLoaded("mp_prison_break"))
                mp.game.streaming.requestAnimDict("mp_prison_break");
            entity.taskPlayAnim("mp_prison_break", "handcuffed", 1.0, 1.0, -1, 50, 1.0, false, false, false);
        }
    }
});

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(player.hasVariable("cuffedPlayer") && player.getVariable("cuffedPlayer") != 0){
        let cuffed = player.getVariable("cuffedPlayer");
        let seats = mp.game.vehicle.getVehicleModelMaxNumberOfPassengers(vehicle.model) - 1;
        if(seats > 0)
            for(let i = 0; i <= seats; i++){
                if(vehicle.isSeatFree(i)){
                    mp.events.callRemote("setCuffedPlayerIntoVeh",vehicle, cuffed, i+1);
                    break;
                }
            }
    }
 });

 mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(player.hasVariable("cuffedPlayer") && player.getVariable("cuffedPlayer") != 0){
        let cuffed = player.getVariable("cuffedPlayer");
        if(cuffed != null && cuffed.vehicle != null && cuffed.vehicle == vehicle){
            mp.events.callRemote("warpCuffedPlayerOutOfVeh", cuffed);
        }
    }
})

function rightVector(forward){
    let up = new mp.Vector3(0.0, 0.0, 1.0);
    return crossProduct(forward, up);
}

function crossProduct(vec1, vec2) {
    return new mp.Vector3(
      vec1.y * vec2.z - vec1.z * vec2.y,
      vec1.z * vec2.x - vec1.x * vec2.z,
      vec1.x * vec2.y - vec1.y * vec2.x
    )
  }

function vecToArray(vec){
    return [vec.x, vec.y, vec.z];
}

function degrees_to_radians(degrees)
{
    var pi = Math.PI;
    return degrees * (pi/180);
}

function multVectors(vec1, vec2){
    return new mp.Vector3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z);
}


mp.events.add("createLSPDMarkers", () => {

    createMarker(new mp.Vector3(447.16238, -975.6481, 30.6896), "Rozpocznij/zakończ służbę");
    createMarker(new mp.Vector3(458.48907, -1017.26117, 28.211643), "Parking pojazdów służbowych");
});




function createMarker(position, name){
    mp.markers.new(23, new mp.Vector3(position.x, position.y, position.z - 0.8), 1.0, {
        color: [41, 115, 184, 255]
    });
    mp.markers.new(20, new mp.Vector3(position.x, position.y, position.z - 0.6), 0.7, {
        color: [255, 255, 255, 255],
        rotation: new mp.Vector3(180, 0, 0)
    });
    mp.labels.new(name, new mp.Vector3(position.x, position.y, position.z + 0.4), {
        drawDistance: 6.0,
        font: 4,
        los: true,
        color: [255,255,255,255]
    });
    mp.labels.new("Naciśnij E", new mp.Vector3(position.x, position.y, position.z + 0.2), {
        drawDistance: 6.0,
        font: 4,
        los: true,
        color: [255,255,255,255]
    });
}