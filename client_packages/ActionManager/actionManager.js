let waypointPos = null;
let player = mp.players.local;
let val = false;
let test = null;



mp.events.add("freezeFor2Sec", () => {
    player.freezePosition(true);
    setTimeout(function () {
        player.freezePosition(false);
    }, 1000)
});


player.setMaxTimeUnderwater(30);


let int = setInterval(() => {
    if(player.hasVariable("type")){
        let str = "Testuje ";
        if(player.getVariable("type") == "owner" || player.getVariable("type") == "admin"){
            str = "Tworzy ";
        }
        mp.discord.update(str + "BasicRPG.pl", "https://discord.gg/zNhdtxhb9a");
        clearInterval(int);
    }
}, 1000);
player.setProofs(true, false, false, false, false, false, false, false);
mp.events.add('render', () => {
    if(player.hasVariable("superhero") && player.getVariable("superhero")){
        mp.game.invoke("0x57FFF03E423A4C0B", player);
    }
    mp.game.controls.disableControlAction(32, 36, true);
    mp.game.controls.disableControlAction(32, 44, true);
    if(!(player.hasVariable("lspd_duty") && player.getVariable("lspd_duty"))){
        mp.game.controls.disableControlAction(32, 37, true);
    }
    mp.game.controls.disableControlAction(32, 140, true);
    mp.game.controls.disableControlAction(32, 141, true);
    mp.game.controls.disableControlAction(32, 142, true);
    mp.game.controls.disableControlAction(32, 345, true);
    mp.game.controls.disableControlAction(32, 346, true);
    mp.game.controls.disableControlAction(32, 347, true);
    mp.game.controls.disableControlAction(32, 23, true);
    mp.game.controls.disableControlAction(32, 68, true);
    mp.game.controls.disableControlAction(32, 99, true);
    mp.game.controls.disableControlAction(32, 100, true);
    if(player.hasVariable("lspd_duty") && player.getVariable("lspd_duty") && player.weapon == mp.game.joaat("weapon_flashlight")){
        mp.game.controls.disableControlAction(32, 91, true);
        mp.game.controls.disableControlAction(32, 92, true);
        mp.game.controls.disableControlAction(32, 24, true)
    }
    if(player.getVariable("job") != "hunter" && !((!player.vehicle && player.hasVariable("lspd_duty") && player.getVariable("lspd_duty")))){
        mp.game.controls.disableControlAction(32, 25, true);
        mp.game.controls.disableControlAction(32, 24, true);
        mp.game.controls.disableControlAction(32, 91, true);
        mp.game.controls.disableControlAction(32, 92, true);
    }
    else{
        let entity = mp.game.player.getEntityIsFreeAimingAt();
        if(entity && (entity.type === 'vehicle' || entity.type === 'player')){
            if(!(player.hasVariable("lspd_duty") && player.getVariable("lspd_duty")))
                mp.game.player.disableFiring(true);
        }
    }
    if(mp.players.local.getVariable("disablecontrols")){
        mp.game.controls.disableAllControlActions(32);
    }

    mp.game.player.restoreStamina(100);


    mp.objects.forEachInStreamRange((object) => {
        if(object.hasVariable("type") && object.getVariable("type") == "dropped"){
            object.setCollision(false, false);
        }
    });
    if(test != null){
        mp.game.graphics.drawLine(test.pos.x, test.pos.y, test.pos.z, test.pos2.x, test.pos2.y, test.pos2.z, 255, 0, 0, 255);
    }
});

mp.events.add("playerCreateWaypoint", (position) => {
    waypointPos = position;
    // if(player.vehicle){
    //     mp.events.callRemote("setPassengersWaypoint", player.vehicle, position.x, position.y, position.z);
    // }
});

// mp.events.add("setWaypoint", (x, y) => {
//     mp.game.ui.setNewWaypoint(x, y);
// });

mp.events.add("teleportToWaypoint", () => {
    if(waypointPos != null){
        if(player.vehicle == null)
        {
            player.position = waypointPos;
        }     
        else{
            mp.events.callRemote("tpWPVeh", waypointPos);
        }      
    }
    else 
        mp.events.call("showNotification", "Nie zaznaczyłeś ostatnio żadnego waypointa!");
});

mp.events.add("setPlayerInvincible", () => {
    val = !val;
    player.setInvincible(val);
    player.setProofs(val, val, val, val, val, val, val, val);
    if(val)
        mp.events.call("showNotification", "Jesteś nieśmiertelny!");
    else
        mp.events.call("showNotification", "Jesteś śmiertelny!");
});

mp.events.add("playerDeath", (player, reason, killer) => {
    if(player.getVariable("job") != "")
        mp.events.callRemote("endJob");
});

mp.events.add("testSound", (sound, library) => {
    mp.game.audio.playSoundFrontend(-1, sound, library, true);
});

mp.events.add("setWeather", (type, transition) => {
    if(!transition){
        mp.game.gameplay.setWeatherTypeNow(type);
    }
    else{
        mp.game.gameplay.setWeatherTypeOverTime(type, 120);
    }
});

let attached = false;

mp.events.add("testbox", () => {
    // let pos = player.vehicle.getWorldPositionOfBone(player.vehicle.getBoneIndexByName("forks_attach"));
    // mp.events.callRemote("savePos", pos);
    let position = player.position;
    let direction = player.getForwardVector();
    let distance = 5;
    let farAway = new mp.Vector3((direction.x * distance) + (position.x), (direction.y * distance) + (position.y), (direction.z * distance) + (position.z + 2) - 25);
    test = {pos: new mp.Vector3(position.x, position.y, position.z+2), pos2: farAway}
});

let currentCars = null;
let currentIndex = 0;
mp.keys.bind(0x25, true, function() {
    if(currentCars != null){
        if(currentIndex > 0){
            currentIndex--;
            mp.events.callRemote("createShowroomVehicle", currentCars[currentIndex]);
        }
        else{
            currentIndex = currentCars.length - 1;
            mp.events.callRemote("createShowroomVehicle", currentCars[currentIndex]);
        }
    }
});
mp.keys.bind(0x27, true, function() {
    if(currentCars != null){
        if(currentIndex < currentCars.length - 1){
            currentIndex++;
            mp.events.callRemote("createShowroomVehicle", currentCars[currentIndex]);
        }
        else{
            currentIndex = 0;
            mp.events.callRemote("createShowroomVehicle", currentCars[currentIndex]);
        }
    }
});

mp.keys.bind(0x0D, true, function() {
    if(currentCars != null){
        mp.events.callRemote("saveShowroomVehicle");
    }
});

mp.events.add("showCars", cars => {
    currentCars = JSON.parse(cars);
    currentIndex = 0;
    let camera = mp.cameras.new("showCarsCam", new mp.Vector3(228.59344, -989.3078, -98.99988), new mp.Vector3(0,0,177), 75);
    camera.setActive(true);
    mp.game.cam.renderScriptCams(true, false, 0, true, false);

    mp.events.callRemote("createShowroomVehicle", currentCars[currentIndex]);
});

function getDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}

function getEucDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2));
}

mp.events.add("testAnim", (dict, name) => {
    mp.game.streaming.requestAnimDict(dict);
    player.taskPlayAnim(dict, name, 1.0, 1.0, -1, 50, 1.0, false, false, false);
});

mp.events.add("testAnim2", (dict, name) => {
    mp.game.streaming.requestAnimDict(dict);
    player.taskPlayAnim(dict, name, 1.0, 1.0, -1, 2, 1.0, false, false, false);
});