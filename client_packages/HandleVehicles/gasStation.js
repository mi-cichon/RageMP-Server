let player = mp.players.local;
let refueling = false;
let fuel_price = 3;
let refueling_cost = 0;
let refueling_vehicle = null;
let refueling_petrol = 0;

let refueling_interval = null;

mp.events.add("playerEnterColshape", (shape) => {
    if(shape != null && mp.colshapes.exists(shape) && shape.getVariable('type') === "distributor"){
        mp.events.callRemote("petrol_checkStation", shape);
    }
});

mp.events.add("playerExitColshape", (shape) => {
    if(shape != null && mp.colshapes.exists(shape) && shape.getVariable('type') === "distributor" && refueling){
        mp.events.callRemote('petrol_stopRefueling', shape, refueling_vehicle, Math.ceil(refueling_cost), refueling_petrol);
        refuelingInterval(false);
        refueling = false;
        refueling_cost = 0;
        refueling_vehicle = null;
        refueling_petrol = 0;
        mp.events.call("petrol_closeBrowser");
    }
});

mp.keys.bind(0x45, true, function() {
    if(refueling){
        refuelingInterval(true);
    }
});

mp.keys.bind(0x45, false, function() {
    if(refueling){
        refuelingInterval(false);
    }
});

mp.events.add('petrol_startRefueling', (vehicle, price) => {
    fuel_price = price;
    refueling_vehicle = vehicle;
    mp.events.call("petrol_openBrowser", fuel_price, vehicle.getVariable("petroltank"), vehicle.getVariable("petrol"));
    refueling = true;
});

mp.events.add("render", () => {
    if(refueling && refueling_vehicle != null && mp.vehicles.exists(refueling_vehicle)){
        let pos = refueling_vehicle.position;
        if(pos && typeof pos != "undefined"){
            let point = mp.game.graphics.world3dToScreen2d(pos.x, pos.y, pos.z + 2);
            if(point && typeof point != "undefined"){
                mp.events.call("petrol_setBrowserPos", point.x, point.y);
            }
        }
    }
});

function refuelingInterval(start){
    if(refueling){
        if(start){
            if(refueling_interval == null){
                clearInterval(refueling_interval);
                refueling_interval = null;
            }
            refueling_interval = setInterval(() => {
                if(refueling_vehicle != null && mp.vehicles.exists(refueling_vehicle) && refueling_vehicle.getVariable("petrol_onStation")){
                    if(refueling_vehicle.getVariable("petrol") + refueling_petrol <= refueling_vehicle.getVariable("petroltank")){
                        refueling_cost += fuel_price / 5;
                        refueling_petrol += 0.2;
                        mp.events.call('petrol_updateBrowser', (refueling_petrol + refueling_vehicle.getVariable('petrol')).clamp(0, refueling_vehicle.getVariable('petroltank')).toFixed(2), Math.ceil(refueling_cost));
                    }else{
                        clearInterval(refueling_interval);
                        refueling_interval = null;
                        mp.events.call("showNotification", "Bak pojazdu jest pełny!");
                    }
                }
                else{
                    mp.events.call("showNotification", "Pojazd nie spełnia warunków tankowania!");
                    mp.events.call("petrol_closeBrowser");
                    clearInterval(refueling_interval);
                    refueling_interval = null;
                    refueling = false;
                    refueling_cost = 0;
                    refueling_vehicle = null;
                    refueling_petrol = 0;
                    mp.events.callRemote("petrol_cancelRefueling", refueling_vehicle);
                }
            }, 200);
        }
        else if(refueling_interval != null){
            clearInterval(refueling_interval);
            refueling_interval = null;
        }
    }
    else if(refueling_interval != null){
        clearInterval(refueling_interval);
        refueling_interval = null;
    }
}

Number.prototype.clamp = function(min, max) {
    return Math.min(Math.max(this, min), max);
};