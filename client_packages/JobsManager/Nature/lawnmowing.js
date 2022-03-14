let player = mp.players.local;
let lawnMower = null;
let grassAmount = 0;
let mowingInterval = null;
let backMarker = null;
let backColshape = null;
let grassBox = null;
let containerBlip = null;
let containerCol = null;
let containerMarker = null;
let lawnTimeout = null;
let containersPositions = [
    new mp.Vector3(-1346.3715, 122.44186, 56.632965),
    new mp.Vector3(-1277.0546, 195.34116, 61.10448),
    new mp.Vector3(-1114.1919, 232.76508, 65.88773),
    new mp.Vector3(-1053.7552, 118.742424, 56.0336),
    new mp.Vector3(-947.9489, -52.70294, 41.56715),
    new mp.Vector3(-1006.0686, -128.81084, 40.3812),
    new mp.Vector3(-1155.7465, -104.74771, 43.01223),
    new mp.Vector3(-1297.4814, -23.23775, 49.25843)

];

mp.events.add("render", () => {
    if(lawnMower != null && mp.vehicles.exists(lawnMower)){
        let text = "";
        if(grassAmount < 1){
            text = "Zapełnienie kosza: " + Math.round(grassAmount * 100) + "%";
        }
        else{
            text = "Weź skoszoną trawę i zanieś ją do kontenera";
        }
        let pos = new Float64Array([0.5, 0.95]);
        mp.game.graphics.drawText(text, pos, { 
            font: 4, 
            color: [255, 253, 141, 255], 
            scale: [0.7, 0.7], 
            outline: true
        });
    }
});

mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(entity.type === "player" && entity == player)
    {
        if(oldvalue === "" && value === "lawnmowing"){
        }
        if(oldvalue === "lawnmowing" && value === ""){
            if(mowingInterval){
               clearInterval(mowingInterval);
               mowingInterval = null;
            }
            if(mp.markers.exists(backMarker)){
               backMarker.destroy();
            }
            if(mp.colshapes.exists(backColshape)){
               backColshape.destroy();
            }
            if(mp.objects.exists(grassBox)){
               grassBox.destroy();
            }
            if(mp.blips.exists(containerBlip)){
               containerBlip.destroy();
            }
            if(mp.colshapes.exists(containerCol)){
            containerCol.destroy();
            }
            if(mp.markers.exists(containerMarker)){
                containerMarker.destroy();
            }
            grassAmount = 0;
            lawnMower = null;
        }
    }
});

mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(mp.vehicles.exists(lawnMower) && vehicle == lawnMower && !mp.colshapes.exists(backColshape) && player.getVariable("job") === "lawnmowing" && grassAmount >= 1){
        mp.events.callRemote("freezeLawnmower", lawnMower, true);
        let { x, y } = offsetPosition(lawnMower.position.x, lawnMower.position.y, vehicle.getHeading() - 180, 1.8);
        let pos = new mp.Vector3(x, y, lawnMower.position.z);
        backMarker = mp.markers.new(0, pos.add(new mp.Vector3(0,0,1.3)), 0.6, {
            color: [0, 204, 153, 255]
        });
        backColshape = mp.colshapes.newTube(pos.x, pos.y, pos.z, 1.0, 2.0);
    }
});

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(mp.vehicles.exists(lawnMower) && vehicle == lawnMower){
        mp.events.callRemote("freezeLawnmower", lawnMower, false);
        startMowing();
    }
});


mp.events.addDataHandler("jobveh", (entity, value, oldvalue) => {
    if(mp.players.local.getVariable("job") == "lawnmowing"){
        if(entity == mp.players.local && value != -1111){
            let v = mp.vehicles.atRemoteId(value);
            if(v != null && mp.vehicles.exists(v) && v.hasVariable("jobtype") && v.getVariable("jobtype") == entity.getVariable("job")){
                lawnMower = v;
                startMowing();
            }
        }
    }
});

mp.events.add("playerEnterColshape", (shape) => {
    if(mp.colshapes.exists(backColshape) && shape == backColshape){
        backMarker.destroy();
        backColshape.destroy();
        grassBox = mp.objects.new(1009806427, player.position, {
            alpha: 0
        });
        mp.game.streaming.requestAnimDict("anim@heists@box_carry@");
        player.taskPlayAnim("anim@heists@box_carry@", "idle", 1.0, 1.0, -1, 63, 1.0, false, false, false);
        setTimeout(() => {
            grassBox.attachTo(player.handle, player.getBoneIndex(57005), 0.08, 0, -0.27, 0, 65, 20, true, false, false, false, 0, true);
            grassBox.setAlpha(255);
        }, 100);
    }
    else if(mp.colshapes.exists(containerCol) && shape == containerCol && mp.objects.exists(grassBox)){
        containerCol.destroy();
        grassBox.destroy();
        player.clearTasksImmediately();
        containerBlip.destroy();
        containerMarker.destroy();
        mp.events.callRemote("lawnmowingReward");
        grassAmount = 0;
    }
});

function startMowing(){
    if(mowingInterval == null){
        mowingInterval = setInterval(() => {
            if(mp.vehicles.exists(lawnMower)){
                if(lawnMower.getSpeed() > 12/3.6){
                    grassAmount += 0.005;
                }
                if(grassAmount >= 1){
                    createContainer();
                    clearInterval(mowingInterval);
                    mowingInterval = null;
                }
            }
            else{
                clearInterval(mowingInterval);
                mowingInterval = null;
            }
        }, 1000);
    }
}

function createContainer(){
    let pos = containersPositions[getRandomInt(0, containersPositions.length)];
    containerBlip = mp.blips.new(652, pos, {
        color: 2,
        name: "Kontener",
        scale: 0.6
    });
    containerCol = mp.colshapes.newTube(pos.x, pos.y, pos.z, 1.0, 2.0);
    containerMarker = mp.markers.new(0, pos.add(new mp.Vector3(0,0,1.3)), 0.6, {
        color: [128, 204, 124, 255]
    });
}

function getRandomInt(min, max){
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
}

var offsetPosition = function(x, y, rot, distance) {
    return {
        x: x + Math.sin(-rot * Math.PI / 180) * distance,
        y: y + Math.cos(-rot * Math.PI / 180) * distance,
    };
};

function getDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}

function getClosestVehicle()
{
    let closestVehicle = null;
    mp.vehicles.forEachInRange(player.position, 5, (vehicle) => {
        if(closestVehicle && closestVehicle.position){
            if(getDistance(player.position, closestVehicle.position) > getDistance(player.position, vehicle.position)){
                closestVehicle = vehicle;
            }
        }
        else
        {
            closestVehicle = vehicle;
        }
    });
    return closestVehicle;
}