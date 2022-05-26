let player = mp.players.local;
let lawnMower = null;
let backMarker = null;
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

let maxCapacity = 16;
let currentCapacity = 0;

mp.events.add("render", () => {
    if(lawnMower != null && mp.vehicles.exists(lawnMower)){
        let text = "";
        if(currentCapacity < maxCapacity){
            text = "Zapełnienie kosza: " + (currentCapacity).toFixed(1) + "/" + maxCapacity + "L";
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
            if(player.getVariable("jobBonus_103")){
                maxCapacity *= 2;
            }
            else if(player.getVariable("jobBonus_102")){
                maxCapacity *= 1.5;
            }
            else if(player.getVariable("jobBonus_101")){
                maxCapacity *= 1.25;
            }
        }
        if(oldvalue === "lawnmowing" && value === ""){
            if(mp.markers.exists(backMarker)){
               backMarker.destroy();
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
            currentCapacity = 0;
            maxCapacity = 16;
            lawnMower = null;
        }
    }
});

mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(mp.vehicles.exists(lawnMower) && vehicle == lawnMower && !mp.objects.exists(grassBox) && player.getVariable("job") === "lawnmowing" && currentCapacity >= maxCapacity){
        mp.events.callRemote("freezeLawnmower", lawnMower, true);
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
});

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(mp.vehicles.exists(lawnMower) && vehicle == lawnMower){
        mp.events.callRemote("freezeLawnmower", lawnMower, false);
    }
});


mp.events.addDataHandler("jobveh", (entity, value, oldvalue) => {
    if(mp.players.local.getVariable("job") == "lawnmowing"){
        if(entity == mp.players.local && value != -1111){
            let v = mp.vehicles.atRemoteId(value);
            if(v != null && mp.vehicles.exists(v) && v.hasVariable("jobtype") && v.getVariable("jobtype") == entity.getVariable("job")){
                lawnMower = v;
            }
        }
    }
});

mp.events.add("playerEnterColshape", (shape) => {
    if(player.getVariable("job") == "lawnmowing"){
        if(mp.colshapes.exists(containerCol) && shape == containerCol && mp.objects.exists(grassBox)){
            containerCol.destroy();
            grassBox.destroy();
            player.clearTasksImmediately();
            containerBlip.destroy();
            containerMarker.destroy();
            mp.events.callRemote("lawnmowingReward");
            currentCapacity = 0;
        }
        else if(shape.hasVariable("type") && shape.getVariable("type") === "grass" && currentCapacity < maxCapacity && player.vehicle != null && player.vehicle === lawnMower){
            mp.events.callRemote("lawnmowingRemoveGrass", shape.getVariable("grassId"));
        }
    }
});

mp.events.add("lawnmowingGrassRemoved", amount => {
    if(player.getVariable("job") == "lawnmowing"){
        amount /= 100;

        currentCapacity += amount;

        if(currentCapacity >= 0.8 * maxCapacity && !(containerCol != null && mp.colshapes.exists(containerCol))){
            createContainer();
        }
    
        if(currentCapacity > maxCapacity){
            currentCapacity = maxCapacity;
        }
    }
});

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

mp.events.add("saveData_lawnmowing_load", (data) => {
    let saveData = JSON.parse(data);
    currentCapacity = saveData[2];
    mp.events.callRemote("saveData_giveJobVeh", "lawnmowing", JSON.parse(saveData[1]), "");
});

mp.events.add("saveData_lawnmowing_save", () => {
    
    if(mp.vehicles.exists(lawnMower)){
        let saveData = ["lawnmowing", JSON.stringify(lawnMower.position), currentCapacity];
        mp.events.callRemote("saveData_saveJobData", JSON.stringify(saveData));
    }
});