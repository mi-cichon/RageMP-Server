let player = mp.players.local;
let enter = true;
let freePassengerSeat = 0;
let parkingBrake = true;
let lock = true;
let seatBelt = true;
let enterAsPassenger = true;
let damageDict = null;
let washParticle = null;
let particle = null;
var globalSeat = 0;
let hornPressed = false;
let interval = setInterval(() => {
    mp.vehicles.forEachInStreamRange((vehicle) => {
        if(vehicle.hasVariable("type") && vehicle.getVariable("type") === "dealer" && vehicle.isSeatFree(-1)){
            vehicle.setCoords(vehicle.getVariable("dealerposition").x, vehicle.getVariable("dealerposition").y, vehicle.getVariable("dealerposition").z, false, false, false, false);
        }
        if(vehicle.hasVariable("type") && vehicle.getVariable("type") === "public" && vehicle.hasVariable("veh_brake") && vehicle.getVariable("veh_brake") && vehicle.isSeatFree(-1) && vehicle.hasVariable("publicposition")){
            vehicle.setCoords(vehicle.getVariable("publicposition").x, vehicle.getVariable("publicposition").y, vehicle.getVariable("publicposition").z, false, false, false, false);
        }
        if(vehicle.hasVariable("market") && vehicle.getVariable("market")){
            vehicle.setCoords(vehicle.getVariable("lastpos").x, vehicle.getVariable("lastpos").y, vehicle.getVariable("lastpos").z, false, false, false, false);
            vehicle.setRotation(vehicle.getVariable("lastrot").x, vehicle.getVariable("lastrot").y, vehicle.getVariable("lastrot").z, 5, true);
        }
        if(vehicle.hasVariable("type") && vehicle.getVariable("type") == "jobveh" && vehicle.hasVariable("veh_brake") && vehicle.getVariable("veh_brake") && !(vehicle.hasVariable("mech") && vehicle.getVariable("mech")) && !vehicle.getVariable("player")){
            let pos = vehicle.getVariable("spawnPos");
            vehicle.setCoords(pos.x, pos.y, pos.z, false, false, false, false);
        }
    });
}, 5000);

let carWashInterval = setInterval(() => {
    mp.vehicles.forEachInStreamRange((vehicle) => {
        if(vehicle.hasVariable("washtime") && vehicle.getVariable("washtime") != ""){
            vehicle.setDirtLevel(0);
        }
    });
}, 5000);

mp.events.addDataHandler("horn", (entity, value, oldvalue) => {
    if(entity.type === 'vehicle' && mp.vehicles.streamed.includes(entity) && (entity != player.vehicle || (entity == player.vehicle && globalSeat != -1))){
        if(value){
            entity.startHorn(2000, -1, false);
        }
    }
})

function didPlayerEnterTheVehicle(vehicle){
    setTimeout(()=>{
        if(vehicle != null && mp.vehicles.exists(vehicle) && !player.isInVehicle(vehicle.handle, false)){
            player.clearTasks();
            mp.events.callRemote("setIntoVeh", vehicle);
        }
    },5000);
}

mp.events.add('render', () => {

    if(mp.game.controls.isControlPressed(27, 86) && !hornPressed && player.vehicle && globalSeat == -1){
        hornPressed = true;
        mp.events.callRemote("setVehicleHorn", player.vehicle, true);
    }
    else if(!mp.game.controls.isControlPressed(27, 86) && hornPressed && player.vehicle && globalSeat == -1){
        hornPressed = false;
        mp.events.callRemote("setVehicleHorn", player.vehicle, false);
    }
    if (mp.players.local.vehicle) {
        mp.game.audio.setRadioToStationName("OFF");
        mp.game.audio.setUserRadioControlEnabled(false);
    }

    if(player.vehicle)
    {
        if(player.vehicle.getEngineHealth() < 50)
            player.vehicle.setEngineHealth(50);

        if(player.vehicle.getBodyHealth() < 50)
            player.vehicle.setBodyHealth(50);
    }
    //entering vehicle
    if(mp.keys.isDown(70) && enter && player.vehicle == null && !player.getVariable("controlsblocked") && !(player.hasVariable("gui") && player.getVariable("gui"))){
        enter = false;
        let veh = getClosestVehicle();
        
        if(veh){
            if(!(player.hasVariable("handCuffed") && player.getVariable("handCuffed"))){
                if(player.hasVariable("job") && player.getVariable("job") != ""){
                    if(player.hasVariable("jobveh") && player.getVariable("jobveh") == veh.remoteId){
                        player.taskEnterVehicle(veh.handle, 5000, -1, 1, 1, 0);
                        didPlayerEnterTheVehicle(veh);
                        return;
                    }
                    else if(player.getVariable("jobveh") == -1111 && veh.hasVariable("jobtype") && veh.getVariable("jobtype") == player.getVariable("job") && !veh.getVariable("player")){
                        player.taskEnterVehicle(veh.handle, 5000, -1, 1, 1, 0);
                        didPlayerEnterTheVehicle(veh);
                        return;
                    }
                    else if(player.getVariable("job") == "lspd" && veh.getVariable("type") == "lspd"){
                        if(player.hasVariable("licenceBp") && player.getVariable("licenceBp")){
                            if(!player.getVariable("nodriving")){
                                player.taskEnterVehicle(veh.handle, 5, -1, 1, 1, 0);
                                didPlayerEnterTheVehicle(veh);
                            }
                            else
                                mp.events.call("showNotification", "Nie możesz prowadzić pojazdów do: " + player.getVariable("nodrivingto"));
                        }  
                        else{
                            mp.events.call("showNotification", "Nie zdałeś egzaminu na prawo jazdy kategorii B!");
                        } 
                        return;
                    }
                    else{
                        return;
                    }
                }
                if(veh.getVariable("type") === "public" && veh.isSeatFree(-1))
                {
                    player.taskEnterVehicle(veh.handle, 5000, -1, 1, 1, 0);
                    didPlayerEnterTheVehicle(veh);
                    return;
                }
                if(veh.getVariable("type") === "furka" && veh.isSeatFree(-1))
                {
                    player.taskEnterVehicle(veh.handle, 5000, -1, 1, 1, 0);
                    didPlayerEnterTheVehicle(veh);
                    return;
                }
                if(veh.getVariable("type") === "personal" && (veh.getVariable("owner").toString() === player.getVariable("socialclub") || veh.getVariable("id") == player.getVariable("carkeys") || (veh.hasVariable("orgId") && player.hasVariable("orgId") && veh.getVariable("orgId") == player.getVariable("orgId"))) && !(veh.hasVariable("mech") && veh.getVariable("mech")) && veh.isSeatFree(-1))
                {
                    if(player.hasVariable("licenceBp") && player.getVariable("licenceBp")){
                        if(!player.getVariable("nodriving")){
                            player.taskEnterVehicle(veh.handle, 5000, -1, 1, 1, 0);
                            didPlayerEnterTheVehicle(veh);
                        }
                        else
                            mp.events.call("showNotification", "Nie możesz prowadzić pojazdów do: " + player.getVariable("nodrivingto"));
                    }  
                    else{
                        mp.events.call("showNotification", "Nie zdałeś egzaminu na prawo jazdy kategorii B!");
                    } 
                }
            }
        }
    }
    // else if(mp.keys.isDown(71) && enterAsPassenger && !player.vehicle && !player.getVariable("controlsblocked")  && player.getVariable("job") === "")
    // {
    //     enterAsPassenger = false;
    //     let veh = getClosestVehicle();
    //     if(veh){
    //         let seats = mp.game.vehicle.getVehicleModelMaxNumberOfPassengers(veh.model) - 1;
    //         if(veh.hasVariable("locked") && veh.getVariable("locked"))
    //         {
    //             mp.events.call("showNotification", "Pojazd jest zamknięty!");
    //         }
    //         else if(veh.getVariable("type") != "dealer" && veh.getVariable("type") != "public" && !veh.isSeatFree(-1) && !(veh.hasVariable("licence"))
    //         {
    //             if(!veh.isSeatFree(-1) && !(veh.hasVariable("licence")))
    //             {
    //                 for(let i = 0; i <= seats; i++){
    //                     if(veh.isSeatFree(i)){
    //                         player.taskEnterVehicle(veh.handle, 10000, i, 1, 1, 0);
    //                         break;
    //                     }
    //                 }
    //             } 
    //         }   
    //     }
    // }
    if(mp.keys.isUp(70)){
        enter = true;
    }
    if(mp.keys.isUp(71)){
        enterAsPassenger = true;
    }
    //parking brake
    if(player.vehicle && (player.vehicle.getVariable("type") === "personal" || player.vehicle.getVariable("type") === "lspd")  && !player.getVariable("controlsblocked") && !(player.vehicle.hasVariable("wash") && player.vehicle.getVariable("wash")) && !player.getVariable("gui"))
    {
        if(player.vehicle.getSpeed() < 1 && mp.keys.isDown(76) && parkingBrake)
        {
            mp.events.callRemote("toggleParkingBrake", player.vehicle);

            parkingBrake = false;
        }
    }
    if(mp.keys.isUp(76))
    {
        parkingBrake = true;
    }
    if(mp.keys.isDown(75) && seatBelt && player.vehicle && (player.vehicle.getVariable("type") === "personal" || player.vehicle.getVariable("type") === "lspd")  && !player.getVariable("controlsblocked") && !player.getVariable("gui") && player.vehicle.getClass() != 8)
    {
        mp.events.callRemote("toggleSeatBelt", player.vehicle);
        seatBelt = false;
    }
    if(mp.keys.isUp(75))
    {
        seatBelt = true;
    }

    if(player.vehicle && (player.vehicle.getVariable("type") === "personal" || player.vehicle.getVariable("type") === "lspd")  && !player.getVariable("controlsblocked") && !player.getVariable("gui"))
    {
        if(mp.keys.isDown(74) && lock)
        {
            mp.events.callRemote("toggleCarLocked", player.vehicle);

            lock = false;
        }
    }
    if(mp.keys.isUp(74))
    {
        lock = true;
    }


    //DAMAGE
});

mp.events.addDataHandler("veh_lights", (entity, value, oldvalue) => {
    if(value == true){
        entity.setLights(2);
    }
    else{
        entity.setLights(0);
    }
});

mp.events.add('entityStreamIn', (entity) => {
    if (entity.type === 'vehicle') {
        if (entity.hasVariable('veh_brake') && entity.getVariable('veh_brake'))
        {
            entity.freezePosition(true)
            entity.setInvincible(true)
        }   
        else
        {
            entity.freezePosition(false);
            entity.setInvincible(false);
        }
        if(entity.hasVariable("invincible") && entity.getVariable("invincible")){
            entity.setInvincible(entity.getVariable("invincible"));
        }
        if(entity.hasVariable("lights") && entity.getVariable("lights")){
            entity.setLights(2);
        }
        else if(entity.hasVariable("lights") && !entity.getVariable("lights")){
            entity.setLights(0);
        }
    }
    if(entity.type === "vehicle" && entity.hasVariable("type") && entity.getVariable("type") === "personal"){
        entity.setModColor1(parseInt(entity.getVariable("color1mod")), 0, 0);
        entity.setModColor2(parseInt(entity.getVariable("color2mod")), 0);
    }
    if(entity.type === "vehicle" && entity.hasVariable("type") && entity.getVariable("type") === "personal" && entity.getVariable("veh_brake")){
        entity.position = entity.getVariable("lastpos");
        entity.rotation = entity.getVariable("lastrot");
    }
    if(entity.type === "vehicle" && entity.hasVariable("market") && entity.getVariable("market"))
    {
        var pos = entity.getVariable("lastpos");
        entity.setCoords(pos.x, pos.y, pos.z, false, false, false, false);
    }
});

mp.events.addDataHandler('color1mod', (entity, value, oldValue) => {
    entity.setModColor1(parseInt(value), 0, 0);
})

mp.events.addDataHandler('wash', (entity, value, oldValue) => {
    entity.freezePosition(value);
});

mp.events.addDataHandler('color2mod', (entity, value, oldValue) => {
    entity.setModColor2(parseInt(value), 0);
})

mp.events.addDataHandler('invincible', (entity, value, oldValue) => {
    entity.setInvincible(value);
})


 mp.events.addDataHandler('veh_brake', (entity, value, oldValue) => {
    if (entity.type === 'vehicle') {
        if (value === true){
            entity.freezePosition(true);
            entity.setInvincible(true);
        }
        else{
            entity.freezePosition(false);
            entity.setInvincible(false);
        }
            
    }
 });

 mp.events.add("startParticles",(position) => {
    if (!mp.game.streaming.hasNamedPtfxAssetLoaded("core")) {
        mp.game.streaming.requestNamedPtfxAsset("core");
        while (!mp.game.streaming.hasNamedPtfxAssetLoaded("core")) mp.game.wait(0);
    }
    mp.game.graphics.setPtfxAssetNextCall("core");

    particle = mp.game.graphics.startParticleFxLoopedAtCoord("exp_sht_steam", position.x, position.y, position.z, 180, 0, 0, 2, false, false, false, true);
 });

 mp.events.add("stopParticles", () => {
    if(particle){
        mp.game.graphics.stopParticleFxLooped(particle, true);
        particle = null;
    }
 });

 mp.events.add("getMod", (mod) => {
    if(player.vehicle)
    {
        let message = "Ilość " + player.vehicle.getModSlotName(parseInt(mod)) + ": " + player.vehicle.getNumMods(parseInt(mod));
        mp.events.call("sendMessage", "", message, "info");
    }
 });
 
 mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(vehicle.getVariable("type") === "public"){
        mp.events.callRemote("freezePublicVehicle", vehicle, false);
        vehicle.setMaxSpeed(60/3.6);
    }
    globalSeat = seat;
 });

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

 function getDistance(vec1, vec2)
 {
     return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
 }

 mp.events.add("getVehicleDamage", () => {
    if(player.vehicle && player.vehicle.hasVariable("damage") && player.getVariable("vehSeat") == 0){
        let vehicle = player.vehicle;
        damageDict = {};
        let engine = vehicle.getEngineHealth();
        let body = vehicle.getBodyHealth();
        if(engine < 50)
            engine = 50;
        if (body < 50)
            body = 50;
            

        pushKeyValueToDamageDict("engine", Math.round(engine));
        pushKeyValueToDamageDict("body", Math.round(body));
        pushKeyValueToDamageDict("fldoor", vehicle.isDoorDamaged(0) ? 0 : 1);
        pushKeyValueToDamageDict("frdoor", vehicle.isDoorDamaged(1) ? 0 : 1);
        pushKeyValueToDamageDict("hood", vehicle.isDoorDamaged(4) ? 0 : 1);
        pushKeyValueToDamageDict("trunk", vehicle.isDoorDamaged(5) ? 0 : 1);

        if(mp.game.vehicle.getVehicleModelMaxNumberOfPassengers(vehicle.model) > 2)
        {
            pushKeyValueToDamageDict("bldoor", vehicle.isDoorDamaged(2) ? 0 : 1);
            pushKeyValueToDamageDict("brdoor", vehicle.isDoorDamaged(3) ? 0 : 1);
            pushKeyValueToDamageDict("blwindow", !vehicle.isWindowIntact(2) ? 0 : 1);
            pushKeyValueToDamageDict("brwindow", !vehicle.isWindowIntact(3) ? 0 : 1);
        }
        else{
            pushKeyValueToDamageDict("bldoor", 1);
            pushKeyValueToDamageDict("brdoor", 1);
            pushKeyValueToDamageDict("blwindow", 1);
            pushKeyValueToDamageDict("brwindow", 1);
        }

        pushKeyValueToDamageDict("flwindow", !vehicle.isWindowIntact(0) ? 0 : 1);
        pushKeyValueToDamageDict("frwindow", !vehicle.isWindowIntact(1) ? 0 : 1);
        pushKeyValueToDamageDict("fwindow", !vehicle.isWindowIntact(6) ? 0 : 1);
        pushKeyValueToDamageDict("bwindow", !vehicle.isWindowIntact(7) ? 0 : 1);

        let tyre = mp.game.invoke('0xBA291848A0815CA9', vehicle.handle, 0, false);

        pushKeyValueToDamageDict("flwheel", vehicle.isTyreBurst(0, false) ? 0 : 1);
        pushKeyValueToDamageDict("frwheel", vehicle.isTyreBurst(1, false) ? 0 : 1);
        pushKeyValueToDamageDict("blwheel", vehicle.isTyreBurst(4, false) ? 0 : 1);
        pushKeyValueToDamageDict("brwheel", vehicle.isTyreBurst(5, false) ? 0 : 1);

        let damageString = JSON.stringify(damageDict);

        mp.events.callRemote("updateVehicleDamage", vehicle, damageString);
    }
    //refreshDamage();
   });
   
   mp.events.add("entityStreamIn", (vehicle) => {
        if(vehicle.hasVariable("damage")){
            if(vehicle)
            {
                let damage = JSON.parse(vehicle.getVariable("damage"));
                for(var key in damage){
                    var part = key;
                    var value = damage[key];
                    switch(part)
                    {
                        case "engine":
                            vehicle.setEngineHealth(value);
                            break;
                        case "body":
                            vehicle.setBodyHealth(value);
                            break;
                        case "fldoor":
                            value == 0 ? vehicle.setDoorBroken(0, false): null;
                            break;
                        case "frdoor":
                            value == 0 ? vehicle.setDoorBroken(1, false): null;
                            break;
                        case "bldoor":
                            value == 0  ? vehicle.setDoorBroken(2, false): null;
                            break;
                        case "brdoor":
                            value == 0  ? vehicle.setDoorBroken(3, false): null;
                            break;
                        case "hood":
                            value == 0  ? vehicle.setDoorBroken(4, false): null;
                            break;
                        case "trunk":
                            value == 0  ? vehicle.setDoorBroken(5, false): null;
                            break;
                        case "flwindow":
                            value == 0  ? vehicle.smashWindow(0): null;
                            break;                        
                        case "frwindow":
                            value == 0  ? vehicle.smashWindow(1): null;
                            break;
                        case "blwindow":
                            value == 0  ? vehicle.smashWindow(2): null;
                            break;
                        case "brwindow":
                            value == 0  ? vehicle.smashWindow(3): null;
                            break;
                        case "fwindow":
                            value == 0  ? vehicle.smashWindow(6): null;
                            break;
                        case "bwindow":
                            value == 0  ? vehicle.smashWindow(7): null;
                            break;
                        case "flwheel":
                            value == 0 ? vehicle.setTyreBurst(0, false, 1000): null;
                            break;
                        case "frwheel":
                            value == 0 ? vehicle.setTyreBurst(1, false, 1000): null;
                            break;
                        case "blwheel":
                            value == 0 ? vehicle.setTyreBurst(4, false, 1000): null;
                            break;
                        case "brwheel":
                            value == 0 ? vehicle.setTyreBurst(5, false, 1000): null;
                            break;
                    }
                }
            }
        }
   })

   //wash

   mp.events.add("setPower", (power) => {
        player.vehicle.setEnginePowerMultiplier(power);
   });

   mp.events.add("setSpeed", (speed) => {
        player.vehicle.setMaxSpeed(speed / 3.6);
    });

    mp.events.add("setTorque", (torque) => {
        player.vehicle.setEngineTorqueMultiplier(torque);
    });
   
    mp.events.add("setoffset", () => {
        mp.vehicles.forEachInStreamRange((veh) => {
            if(veh.hasVariable("dealerposition") && veh.getVariable("type") === "dealer"){
                veh.setCoords(veh.getVariable("dealerposition").x, veh.getVariable("dealerposition").y, veh.getVariable("dealerposition").z, false, false, false, false);
            }
        });
    });

    mp.events.add("updateDirtLevel", () => {
        if(mp.vehicles.exists(player.vehicle))
            mp.events.callRemote("updateVehiclesDirtLevel", player.vehicle, player.vehicle.getDirtLevel());
    })

    mp.events.add("cleanVehicle", () => {
        player.vehicle.setDirtLevel(0);
    })

    mp.events.add('entityStreamIn', (entity) => {
        if(entity.hasVariable("type") && entity.getVariable("type") === "personal" && entity.hasVariable("dirt"))
        {
            entity.setDirtLevel(entity.getVariable("dirt"));
        }
    });

    function pushKeyValueToDamageDict(key, value){
        damageDict[key] = value;
    }
    
    mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
        if(mp.vehicles.exists(vehicle) && vehicle.hasVariable("type") && vehicle.getVariable("type") === "race")
        {
            mp.events.callRemote("deleteKart", vehicle);
            mp.events.call("stopKartRace");
        }
        if(mp.vehicles.exists(vehicle) && seat == -1){
            mp.events.callRemote("setVehicleHorn", vehicle, false);
        }
    });

    mp.events.add("setVehicleAWD", () => {
        if(player.vehicle)
        {
            player.vehicle.setHandling("fDriveBiasFront", 0.5);
        }
    });

    

    //extra

    mp.events.add("entityStreamIn", entity => {
        if(entity.hasVariable("extra")){
            for(let i = 0; i < 20; i++){
                entity.setExtra(i, true);
            }
            entity.setExtra(entity.getVariable("extra"), false);
        }
    });

    mp.events.addDataHandler("extra", (entity, value, oldvalue) => {
        for(let i = 0; i < 20; i++){
            entity.setExtra(i, true);
        }
        entity.setExtra(value, false);
    });

    let drowningVeh = null;
    setInterval(function(){
        if(player.vehicle){
            if(drowningVeh == null){
                if(player.vehicle && player.hasVariable("vehSeat") && player.getVariable("vehSeat") == 0 && player.vehicle.hasVariable("owner") && player.vehicle.isInWater() && !player.vehicle.getIsEngineRunning()){
                    drowningVeh = player.vehicle;
                }
                else{
                    drowningVeh = null;
                }
            }
            else{
                if(player.vehicle == drowningVeh && player.vehicle.isInWater() && !player.vehicle.getIsEngineRunning()){
                    mp.events.callRemote("removeDrowningVehicle", player.vehicle);
                }
                else{
                    drowningVeh = null;
                }
            }
        }
    }, 2000);

    mp.events.addDataHandler("damage", (entity, value, oldvalue) => {
        if(mp.vehicles.streamed.includes(entity)){
            let vehicle = entity;
            let damage = JSON.parse(value);
            for(var key in damage){
                var part = key;
                var value = damage[key];
                switch(part)
                {
                    case "engine":
                        vehicle.setEngineHealth(value);
                        break;
                    case "body":
                        vehicle.setBodyHealth(value);
                        break;
                    case "fldoor":
                        value == 0 ? vehicle.setDoorBroken(0, false): null;
                        break;
                    case "frdoor":
                        value == 0 ? vehicle.setDoorBroken(1, false): null;
                        break;
                    case "bldoor":
                        value == 0  ? vehicle.setDoorBroken(2, false): null;
                        break;
                    case "brdoor":
                        value == 0  ? vehicle.setDoorBroken(3, false): null;
                        break;
                    case "hood":
                        value == 0  ? vehicle.setDoorBroken(4, false): null;
                        break;
                    case "trunk":
                        value == 0  ? vehicle.setDoorBroken(5, false): null;
                        break;
                    case "flwindow":
                        value == 0  ? vehicle.smashWindow(0): null;
                        break;                        
                    case "frwindow":
                        value == 0  ? vehicle.smashWindow(1): null;
                        break;
                    case "blwindow":
                        value == 0  ? vehicle.smashWindow(2): null;
                        break;
                    case "brwindow":
                        value == 0  ? vehicle.smashWindow(3): null;
                        break;
                    case "fwindow":
                        value == 0  ? vehicle.smashWindow(6): null;
                        break;
                    case "bwindow":
                        value == 0  ? vehicle.smashWindow(7): null;
                        break;
                    case "flwheel":
                        value == 0 ? vehicle.setTyreBurst(0, false, 1000): null;
                        break;
                    case "frwheel":
                        value == 0 ? vehicle.setTyreBurst(1, false, 1000): null;
                        break;
                    case "blwheel":
                        value == 0 ? vehicle.setTyreBurst(4, false, 1000): null;
                        break;
                    case "brwheel":
                        value == 0 ? vehicle.setTyreBurst(5, false, 1000): null;
                        break;
                }
            }
        }
    })



    //trip

    let lastPos = null;
    let tripInterval = null;

    mp.events.add("playerEnterVehicle", (vehicle, seat) => {
        if(vehicle != null && mp.vehicles.exists(vehicle) && vehicle.hasVariable("veh_trip") && seat == -1){
            lastPos = null;
            startTripInterval(true);
        }
     });

     mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
        startTripInterval(false);
        lastPos = null;
    });

    function startTripInterval(state){
        if(state){
            if(tripInterval != null){
                clearInterval(tripInterval);
            }
            tripInterval = setInterval(function(){
                if(player.vehicle != null && mp.vehicles.exists(player.vehicle) && player.vehicle.hasVariable("veh_trip")){
                    if(lastPos == null){
                        lastPos = player.vehicle.position;
                    }
                    else{
                        let dist = getDistance(lastPos, player.vehicle.position);
                        lastPos = player.vehicle.position;
                        mp.events.callRemote("trip_update", player.vehicle, parseFloat(dist)/1000.0);
                    }
                }
                else{
                    clearInterval(tripInterval);
                    tripInterval = null;
                }
                
            }, 1000);
        }
        else{
            if(tripInterval != null){
                clearInterval(tripInterval);
                tripInterval = null;
            }
        }
    }
