let speedometerBrowser = null;
let player = mp.players.local;
let rpm = 0;
let speed = 0;
let alive = false;
let lastStreet = "";
mp.events.add('render', () => {
    if(player.isInAnyVehicle(false))
    {
        if(mp.browsers.exists(speedometerBrowser))
        {
            let petrol = 0;
            if(player.vehicle.hasVariable("petrol") && player.vehicle.hasVariable("petroltank")){
                petrol = player.vehicle.getVariable("petrol") / player.vehicle.getVariable("petroltank");
            }
            let street = mp.game.pathfind.getStreetNameAtCoord(player.position.x, player.position.y, player.position.z, 0, 0);
            let streetName = street.lastStreet == street.crossingRoad ? street.crossingRoad : street.streetName;
            lastStreet = streetName;
            let rpm = player.vehicle.getIsEngineRunning() ? player.vehicle.rpm * 0.85 : 0;
            
            speedometerBrowser.execute(`setVars(${player.vehicle.getSpeed() * 3.6}, ${rpm}, ${petrol}, "${mp.game.ui.getStreetNameFromHashKey(streetName)}", ${player.vehicle.hasVariable("veh_trip") ? player.vehicle.getVariable("veh_trip") : -1});`);
        }
    }
});

mp.events.add("settings_setSpeedometerScale", scale => {
    if(speedometerBrowser){
        speedometerBrowser.execute(`setScale(${scale})`);
    }
});

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(player.hasVariable("settings")){
        if(speedometerBrowser != null && mp.browsers.exists(speedometerBrowser)){
            speedometerBrowser.destroy();
            speedometerBrowser = null;
        }
        let maxSpeed = 180;
        if(vehicle.hasVariable("speed")){
            maxSpeed = round(vehicle.getVariable("speed"), 20, 0);
        }
        let speedometer = player.vehicle.hasVariable("speedometer") ? player.vehicle.getVariable("speedometer") : "#0c9";
        speedometerBrowser = mp.browsers.new('package://Speedometers/Speedometer0/index.html');
        speedometerBrowser.execute(`instantiate(${maxSpeed}, ${vehicle.hasVariable("petrol")}, ${player.getVariable("settings_SpeedometerSize")}, '${player.vehicle.hasVariable("name") ? player.vehicle.getVariable("name") : mp.game.vehicle.getDisplayNameFromVehicleModel(player.vehicle.model)}', ${player.vehicle.hasVariable('veh_trip')}, "${speedometer}")`);
        alive = true;
    }
 });

 mp.events.add("closeSpeedometerBrowser", () => {
    if(speedometerBrowser){
        speedometerBrowser.destroy();
        speedometerBrowser = null;
        alive = false;
    }
 });

mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(alive)
    {
        alive = false;
        if(speedometerBrowser)
        {
            speedometerBrowser.destroy();
            speedometerBrowser = null;
        }
        
    }
});

mp.events.add("showSpeedometerHUD", (state, driver) => {
    if(speedometerBrowser){
    speedometerBrowser.execute(`showHUD(${state}, ${driver});`);
    }
});

mp.events.add("console", (text) => {
    mp.console.logInfo(text);
});

function round(number, increment, offset) {
    return Math.ceil((number - offset) / increment ) * increment + offset;
}


mp.events.add("speedometer_setColor", color => {
    if(speedometerBrowser!=null && mp.browsers.exists(speedometerBrowser)){
        speedometerBrowser.execute(`setColor("${color}")`);
    }
});