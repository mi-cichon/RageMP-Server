let player = mp.players.local;
let lastBone = null;
let vehVars = {
    name: "default",
    power: "default",
    maxspeed: "default"
}

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(vehicle.hasVariable("speed"))
    {
        vehicle.setMaxSpeed(vehicle.getVariable("speed")/3.6);
    }
    if(vehicle.hasVariable("power"))
    {
        vehicle.setEnginePowerMultiplier(vehicle.getVariable("power"));
    }
    if(vehicle.hasVariable("offroad") && vehicle.getVariable("offroad")){
        vehicle.setHandling('FTRACTIONLOSSMULT', 0.8);
    }
    else{
        vehicle.setHandling('FTRACTIONLOSSMULT', 2.5);
    }
 });

 mp.events.addDataHandler("offroad", (entity, value, oldvalue) => {
    if(player.vehicle && player.vehicle == entity){
        if(value){
            entity.setHandling('FTRACTIONLOSSMULT', 0.8);
        }
        else{
            entity.setHandling('FTRACTIONLOSSMULT', 2.5);
        }
    }
 });


 mp.events.add("setHandlings", (type, value) => {
    if(player.vehicle){
        switch(type){
            case 'power':
                player.vehicle.setEnginePowerMultiplier(value);
            break;
            case 'speed':
                player.vehicle.setMaxSpeed(value/3.6);
            break;
        }
    }
 })

 mp.events.add("zts", speed => {
    let time = 0;
    let ztsInterval = setInterval(function(){
        if(player.vehicle){
            if(player.vehicle.getSpeed() < 1){
                mp.events.call("showNotification", "Pomiar gotowy");
                clearInterval(ztsInterval);
                let speedInterval = setInterval(function(){
                    if(player.vehicle){
                        if(player.vehicle.getSpeed() > 1){
                            time += 10;
                        }
                        if(player.vehicle.getSpeed() >= speed/3.6){
                            clearInterval(speedInterval);
                            mp.events.call("showNotification", "Osiągnąłeś prędkość " + speed.toString() + "km/h w " + (time/1000).toString() + " sek.");
                        }
                    }
                    else{
                        clearInterval(speedInterval);
                    }
                }, 10);
            }
        }else{
            clearInterval(ztsInterval);
        }
    }, 10);
 });

 mp.events.addDataHandler("power", (entity, value, oldvalue) => {
    if(entity != null && entity.type === "vehicle" && player.vehicle != null && entity == player.vehicle){
        setTimeout(function(){
            entity.setEnginePowerMultiplier(value);
        }, 1000)
    }
 });

 mp.events.addDataHandler("speed", (entity, value, oldvalue) => {
    if(entity != null && entity.type === "vehicle" && player.vehicle != null && entity == player.vehicle){
        setTimeout(function(){
            entity.setMaxSpeed(value/3.6);
        }, 1000)
    }
 });


 mp.events.add({
     "test_setDirtStick" : (amount) => {
        if(player.vehicle){
            player.vehicle.setHandling('FTRACTIONLOSSMULT', parseFloat(amount));
        }
     },
     "test_setDrag" : (min, max) => {
        if(player.vehicle){
            player.vehicle.setHandling('fTractionCurveMin', parseFloat(min));
            player.vehicle.setHandling('fTractionCurveMax', parseFloat(max));
            mp.game.vehicle.hand
        }
     }
 })