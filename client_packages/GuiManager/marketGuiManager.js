let player = mp.players.local;
let closestMarketVehicle = null;
let marketSet = false;
mp.events.add('render', () => {
    let vehicle = mp.game.vehicle.getClosestVehicle(player.position.x, player.position.y, player.position.z, 8, 0, 70);
    if(vehicle!=0){
        let veh = mp.vehicles.atHandle(vehicle);
        if(veh != null && veh.hasVariable("market") && veh.getVariable("market") && veh!=closestMarketVehicle)
        {
            closestMarketVehicle = veh;
            marketSet = false;
        }
    }
    else{
        closestMarketVehicle = null;
        marketSet = false;
        mp.events.call("hideMarketInfo");
    }

    if(closestMarketVehicle != null && !marketSet && closestMarketVehicle.hasVariable("markettune")){
        marketSet = true;
        let vehName = closestMarketVehicle.getVariable("name");
        let vehId = closestMarketVehicle.getVariable("id");
        let vehPrice = closestMarketVehicle.getVariable("marketprice");
        let vehOwner = closestMarketVehicle.getVariable("marketowner");
        let vehDesc = closestMarketVehicle.getVariable("marketdescription");
        let vehTune = closestMarketVehicle.getVariable("markettune");
        mp.events.call("setMarketValues", vehId, vehPrice, vehName, vehOwner, vehDesc, vehTune);
    }

    if(closestMarketVehicle != null && mp.vehicles.exists(closestMarketVehicle) && marketSet){
        point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(closestMarketVehicle.position.x, closestMarketVehicle.position.y, closestMarketVehicle.position.z + 1.1 + ( 0.4 * (getDistance(player.position, closestMarketVehicle.position) / 8.0))));
        if(point)
            mp.events.call("setMarketPosition", point.x, point.y);
    }
});

function getDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}