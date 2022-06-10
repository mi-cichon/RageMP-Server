let animalToHunt = null;
let hunterVehicle = null;
let player = mp.players.local;
let deadAnimalBlip = null;
let hunterPedPosition = null;
let peltType = null;
let jobCenter = new mp.Vector3(-1469.4724, 4568.7334, 39.568737);
let animalColshape = null;
let notified = false;
let skinning = false;
let animalFleeing = false;
let fleeTask = null;
let taskInterval = null;
let lastView = 0;

let roundAmount = 50;
mp.events.add("render", () => {
    if(player.getVariable("job") === "hunter"){
        if(getEucDistance(jobCenter, player.position) > 550 && !notified){
            notified = true;
            mp.events.call("showNotification", "Oddalasz się od miejsca pracy! Zawróć!");
        }
        else if(notified && getEucDistance(jobCenter, player.position) < 450){
                notified = false;
        }
        if(getEucDistance(jobCenter, player.position) > 700 && notified){
            removeItems();
            mp.events.callRemote('endJob');
        }
        if(animalToHunt != null){
            if(animalToHunt.doesExist() && !animalToHunt.isDeadOrDying(true) && taskInterval == null){
                let taskFlee = -100;
                taskInterval = setInterval(function(){
                    if(animalToHunt != null && animalToHunt.doesExist() && getDistance(animalToHunt.getCoords(true), player.position) < 17 && taskFlee == -100){
                        taskFlee = animalToHunt.taskSmartFlee(player.handle, 30, -1, false, false);
                    }
                    else if(animalToHunt != null && animalToHunt.doesExist() && getDistance(animalToHunt.getCoords(true), player.position) > 30 && taskFlee != -100){
                        taskFlee = -100;
                        animalToHunt.taskWanderStandard(10, 10);
                    }
                }, 1000);
            }
            else if(taskInterval != null && (animalToHunt == null || !animalToHunt.doesExist() || animalToHunt.isDeadOrDying(true))){
                clearInterval(taskInterval);
                taskInterval = null;
            }
            let distance = roundnum(getDistance(player.position, animalToHunt.getCoords(true)), roundAmount);
            let text;
            if(distance < 500){
                text = "Zwierzę jest około " + distance + " metrów od Ciebie";
            }
            else{
                text = "Zwierze jest daleko od Ciebie";
            }
            let pos = new Float64Array([0.5, 0.95]);
            mp.game.graphics.drawText(text, pos, { 
                font: 0, 
                color: [255, 253, 141, 255], 
                scale: [0.5, 0.5], 
                outline: true
            });
            if(animalToHunt.doesExist() && animalColshape != null && mp.colshapes.exists(animalColshape)){
                animalColshape.position = animalToHunt.getCoords(true);
                if(deadAnimalBlip != null && deadAnimalBlip.doesExist()){
                    deadAnimalBlip.position = animalToHunt.getCoords(true);
                }
            }
            else if(animalToHunt.doesExist()){
                animalToHunt.setProofs(false, false, false, false, false, false, false, false);
                animalToHunt.setInvincible(false);
                animalToHunt.freezePosition(false);
            }
        }
    
        if(animalToHunt != null && animalToHunt.doesExist() && animalToHunt.isDeadOrDying(true) && animalColshape == null){
            animalColshape = mp.colshapes.newTube(animalToHunt.getCoords(true).x, animalToHunt.getCoords(true).y, animalToHunt.getCoords(true).z, 2.0, 5.0);
            deadAnimalBlip = mp.blips.new(141, animalToHunt.getCoords(true), {
                name: "Zwierzę do oskórowania"
            });
        }
        if(mp.game.controls.isControlJustPressed(32, 25)){
            mp.game.cam.setFollowPedCamViewMode(4);
        }
        else if(mp.game.controls.isControlJustReleased(32, 25)){
            mp.game.cam.setFollowPedCamViewMode(2);
        }
    } 
});

mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(value == "hunter" && entity == player){
        if(player.getVariable("jobBonus_119")){
            roundAmount = 25;
        }
        setTimeout(function(){
            mp.game.invoke("0xD966D51AA5B28BB9", player.handle, 487013001, mp.game.joaat("COMPONENT_AT_AR_FLSH"));
        }, 1000);
    }
    if(oldvalue == "hunter" && entity==player){
        player.setCanBeKnockedOffVehicle(0);
        removeItems();
    }
});


mp.events.add('playerEnterColshape', (shape) => {
    if(shape == animalColshape && animalToHunt != null && !player.vehicle){
        mp.events.callRemote("takeHunterPelt", peltType);
    }
    // if(shape == hunterColshape){
    //     hunterColshape.destroy();
    //     hunterColshape = null;
    //     animalToHunt = null;
    //     deadAnimalBlip = null;
    //     hunterPedPosition = null;
    //     peltType = null;
    // }
});


mp.events.add("peltTaken", (state) => {
    if(state){
        let time = 4000;

        if(player.getVariable("jobBonus_120")){
            time /= 2;
        }
        animalColshape.destroy();
        animalColshape = null;
        animalToHunt.destroy();
        animalToHunt = null;
        deadAnimalBlip.destroy();
        deadAnimalBlip = null;
        mp.game.streaming.requestAnimDict("amb@prop_human_bum_bin@base");
        player.taskPlayAnim("amb@prop_human_bum_bin@base", "base", 1.0, 1.0, time, 2, 1.0, false, false, false);
        player.freezePosition(true);
        skinning = true;
        setTimeout(function(){
            skinning = false;
            player.freezePosition(false);
            mp.events.callRemote("animalHunted", peltType);
        }, time);
    }
    else{
        mp.events.call("showNotification", "Masz pełny ekwipunek! Sprzedaj skóry u myśliwego lub zrób więcej miejsca!");
    }
});


mp.events.add("huntNewAnimal", (animalPos, animalType, hunterPed) => {
    player.setCanBeKnockedOffVehicle(1);
    settingsReset = false;
    if(!hunterPedPosition){
        hunterPedPosition = hunterPed;
    }
    animalToHunt = mp.peds.new(animalType, animalPos, 0, player.dimension);
    peltType = animalToHunt.model.toString();
    let interval = setInterval(() => {
        if(animalToHunt != null && animalToHunt.doesExist())
        {
            setAnimalProofs();
            clearInterval(interval);
        }        
    }, 3000)
});

function getDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}


function getEucDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2));
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

function roundnum(num, type){
    return Math.round(num / type)*type;
}

function setAnimalProofs(){
    setTimeout(() => {
        if(animalToHunt)
        {
            animalToHunt.setProofs(false, false, false, false, false, false, false, false);
            animalToHunt.setInvincible(false);
            animalToHunt.taskWanderStandard(10, 10);
            animalToHunt.freezePosition(false);
            animalSpawned = true;
        }
    }, 5000);
}

function removeItems(){
    if(animalColshape != null){
        animalColshape.destroy();
        animalColshape = null;
    }
    
    if(deadAnimalBlip != null){
        deadAnimalBlip.destroy();
    }
    deadAnimalBlip = null;
    if(animalToHunt != null){
        animalToHunt.destroy();
    }
    animalToHunt = null;
    hunterVehicle = null;
}