let player = mp.players.local;
let towtruck = null;

let vehicleToTow = null;
let wreckToTow = null;

let currentDistance = null;

let propObj = null;

let vehBlip = null;

let vehMarker = null;

let vehShape = null;

let towType = null;

let wreckModel;

let hookingObjects = {shape: null, marker: null, label: null}

let hookInHand = false;

let defaultHookLength = 20;

let bases = [
    {pos: new mp.Vector3(561.56323, -3037.9924, 6.091237), shape: null, blip: null, marker: null},
    {pos: new mp.Vector3(1736.1683, 3290.8918, 41.148464), shape: null, blip: null, marker: null},
]
mp.events.add("render", () => {
    if(player.getVariable("job") == "towtruck")
    {
        if(hookInHand && (vehicleToTow != null || wreckToTow != null)){
            let textToDisplay = "";
            let maxDist = defaultHookLength;
            if(player.getVariable("jobBonus_23")){
                maxDist *= 1.5;
            }
            else if(player.getVariable("jobBonus_22")){
                maxDist *= 1.25;
            }
    
            maxDist = parseInt(maxDist);

            let dist = parseInt(getDistance(towtruck.position, player.position));
            textToDisplay = `Linka: ${dist}/${maxDist}m`;

            if(dist > maxDist){
                hookInHand = false;
                mp.events.call("showNotification", "Linka się zerwała!");
            }
        
            if(textToDisplay != ""){
                let pos = new Float64Array([0.5, 0.95]);
                mp.game.graphics.drawText(textToDisplay, pos, { 
                    font: 4, 
                    color: [255, 253, 141, 255], 
                    scale: [0.7, 0.7], 
                    outline: true
                });
            }
        }            
    }
});

mp.events.add("playerEnterColshape", (colshape) => {
    if(player.getVariable("job") == "towtruck"){
        if(hookingObjects.shape != null && colshape == hookingObjects.shape){
            hookInHand = true;
        }
        else if(vehShape != null && hookInHand && colshape == vehShape){
            attachVehicle();
        }
        else{
            bases.forEach(base => {
                if(base.shape != null && base.shape == colshape){
                    removeTowItems(true);
                    removeBases();
                    mp.events.callRemote("vehicleTowed", towType, currentDistance, towtruck.getEngineHealth() / 1000);
                    mp.events.callRemote("getVehicleToTow");
                }
            });
        }
    }
});

function attachVehicle(){
    hookInHand = false;
    let time = 8000;

    if(player.getVariable("jobBonus_26")){
        time *= 0.25;
    }
    else if(player.getVariable("jobBonus_25")){
        time *= 0.5;
    }
    else if(player.getVariable("jobBonus_24")){
        time *= 0.75;
    }


    if(vehicleToTow != null && vehicleToTow.doesExist()){
        
        let pos = new mp.Vector3(vehicleToTow.position.x, vehicleToTow.position.y, vehicleToTow.position.z - 0.4);
        let heading = vehicleToTow.heading;
        let rotation = vehicleToTow.getRotation(5);
        mp.events.callRemote("removeTowedVehicle", vehicleToTow);
        vehicleToTow = null;
        player.freezePosition(true);
        
        mp.events.call("showNotification", "Trwa załadunek pojazdu!");
        propObj = mp.objects.new('imp_prop_covered_vehicle_03a', pos, {
            rotation: rotation
        });

        setTimeout(() => {
            
            mp.events.callRemote("createTowObject", towtruck, "car");

            propObj.attachTo(towtruck.handle, 0, 0, -1.6, 0.4, 0, 0, 0, true, true, false, false, 0, true);
            removeTowItems(false);
            player.freezePosition(false);
            mp.events.call("showNotification", "Pojazd załadowany, udaj się do bazy!");

            createBases();
        }, time);
    }

    if(wreckToTow != null && wreckToTow.doesExist()){

        propObj = wreckToTow;
        wreckToTow = null;
        player.freezePosition(true);
        mp.events.call("showNotification", "Trwa załadunek wraku!");

        setTimeout(() => {
            mp.events.callRemote("createTowObject", towtruck, wreckModel);
            removeTowItems(false);
            player.freezePosition(false);
            mp.events.call("showNotification", "Wrak załadowany, udaj się do bazy!");
            propObj.attachTo(towtruck.handle, 0, 0, -1.6, 0.4, 0, 0, 0, true, true, false, false, 0, true);

            createBases();
        }, time);
    }
}

function createBases(){

    if(player.getVariable("jobBonus_28")){
        bases[1].blip = mp.blips.new(8, bases[1].pos, {
            color: 15,
            name: "Baza Sandy Shores",
            scale: 0.8
        });

        bases[1].marker = mp.markers.new(27, subVectors(bases[1].pos, new mp.Vector3(0,0,0.8)), 5, {
            bopUpAndDown: true,
            color: [0, 204, 153, 255]
        });
    
        bases[1].shape = mp.colshapes.newTube(bases[1].pos.x, bases[1].pos.y, bases[1].pos.z, 5, 2);
    }
    
    bases[0].blip = mp.blips.new(8, bases[0].pos, {
        color: 15,
        name: "Baza Los Santos",
        scale: 0.8
    });

    bases[0].marker = mp.markers.new(27, subVectors(bases[0].pos, new mp.Vector3(0,0,0.8)), 5, {
        bopUpAndDown: true,
        color: [0, 204, 153, 255]
    });

    bases[0].shape = mp.colshapes.newTube(bases[0].pos.x, bases[0].pos.y, bases[0].pos.z, 5, 2);

    if(player.getVariable("jobBonus_28")){
        let closestBlip = getDistance(bases[0].pos, player.position) < getDistance(bases[1].pos, player.position) ? bases[0].blip : bases[1].blip;
        closestBlip.setRoute(true);
    }
    else{
        bases[0].blip.setRoute(true);
    }
}

function removeBases(){
    bases.forEach(base => {
        if(base.marker != null){
            base.marker.destroy();
            base.marker = null;
        }
        if(base.blip != null){
            base.blip.destroy();
            base.blip = null;
        }
        if(base.shape != null){
            base.shape.destroy();
            base.shape = null;
        }
    })
}



mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(entity == player && value == "towtruck"){

    }
    else if(entity == player && oldvalue == "towtruck"){
        removeTowItems(true);
        removeBases();
        towtruck = null;
        if(vehicleToTow != null){
            mp.events.callRemote("markVehicleAsNotTowed", vehicleToTow);
        }
    }
});

mp.events.add("setVehicleToTow", (vehtotow) =>{
    if(vehtotow != null)
    {
        setVehicleToTow(vehtotow)
        towType = "car";
    }
    else
    {
        setWreckToTow()
        towType = "wreck";
    } 
});

function removeTowItems(prop){
    if(wreckToTow != null){
        wreckToTow.destroy();
        wreckToTow = null;
    }
    if(vehBlip != null){
        vehBlip.destroy();
        vehBlip = null;
    }
    if(vehMarker != null){
        vehMarker.destroy();
        vehMarker = null;
    }
    if(vehShape != null){
        vehShape.destroy();
        vehShape = null;
    }
    if(prop){
        if(propObj != null){
            propObj.destroy();
            propObj = null;
        }
    }
    if(hookingObjects.shape != null){
        hookingObjects.shape.destroy();
        hookingObjects.shape = null;
    }
    if(hookingObjects.marker != null){
        hookingObjects.marker.destroy();
        hookingObjects.marker = null;
    }
    if(hookingObjects.label != null){
        hookingObjects.label.destroy();
        hookingObjects.label = null;
    }
}


function setVehicleToTow(veh){
    vehicleToTow = veh;
    mp.events.call("showNotification", "Pojazd do odholowania został oznaczony na mapie!");
    // reward = getDistance(vehicleToTow.getVariable("lastpos"), player.position) /7;
    currentDistance = getDistance(veh.getVariable("lastpos"), player.position);

    vehBlip = mp.blips.new(8, veh.getVariable("lastpos"), {
        color: 46,
        name: "Pojazd do odholowania"
    });

    vehMarker = mp.markers.new(0, subVectors(veh.getVariable("lastpos"), new mp.Vector3(0,0,-2)), 0.6, {
        bopUpAndDown: true,
        color: [0, 204, 153, 255]
    });

    let pos = veh.getVariable("lastpos");
    vehShape = mp.colshapes.newTube(pos.x, pos.y, pos.z -1, 2, 3);

    vehBlip.setRoute(true);
}

function setWreckToTow(){
    mp.events.call("showNotification", "Wrak do odholowania został oznaczony na mapie!");

    let wreckDims = wrecksPosition[getRandomInt(0, wrecksPosition.length)];
    wreckModel = wreckModels[getRandomInt(0, wreckModels.length)];

    currentDistance = getDistance(wreckDims.position, player.position);

    wreckToTow = mp.objects.new(wreckModel, wreckDims.position, {
        rotation: new mp.Vector3(0, 0, wreckDims.rotation)
    });

    vehBlip = mp.blips.new(8, wreckToTow.position, {
        color: 46,
        name: "Wrak do odholowania"
    });
    vehMarker = mp.markers.new(0, subVectors(wreckToTow.position, new mp.Vector3(0,0,-2)), 0.6, {
        bopUpAndDown: true,
        color: [0, 204, 153, 255]
    });

    let pos = wreckDims.position;
    vehShape = mp.colshapes.newTube(pos.x, pos.y, pos.z -1, 2, 3);

    vehBlip.setRoute(true);
}

function getDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}


function subVectors(vec1, vec2){
    return new mp.Vector3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
}


mp.events.addDataHandler("jobveh", (entity, value, oldvalue) => {
    if(mp.players.local.getVariable("job") == "towtruck"){
        if(entity == mp.players.local && value != -1111){
            let v = mp.vehicles.atRemoteId(value);
            if(v != null && mp.vehicles.exists(v) && v.hasVariable("jobtype") && v.getVariable("jobtype") == entity.getVariable("job")){
                towtruck = v;
            }
        }
    }
});

mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(towtruck != null && mp.vehicles.exists(towtruck) && vehicle == towtruck){
        let sideDirection = rightVector(towtruck.getForwardVector());
        let leftDistance = -2.1;
        let leftAway = new mp.Vector3((sideDirection.x * leftDistance) + towtruck.getCoords(true).x, (sideDirection.y * leftDistance) + towtruck.getCoords(true).y, (sideDirection.z * leftDistance) + towtruck.getCoords(true).z - 1.4);
        let shape = mp.colshapes.newTube(leftAway.x, leftAway.y, leftAway.z, 1.0, 2.0);
        let marker = mp.markers.new(1, leftAway, 1.0, {color: [0, 204, 153, 255]});
        let label = mp.labels.new("Weź hak od wyciągarki", new mp.Vector3(leftAway.x, leftAway.y, leftAway.z + 1), {color: [255,255,255,255], drawDistance: 5, font: 4});
        hookingObjects = {shape: shape, marker: marker, label: label};
    }
});

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(towtruck != null && mp.vehicles.exists(towtruck) && vehicle == towtruck){
        if(hookingObjects.shape != null && mp.colshapes.exists(hookingObjects.shape)){
            hookingObjects.shape.destroy();
            hookingObjects.shape = null;
            hookingObjects.marker.destroy();
            hookingObjects.marker = null;
            hookingObjects.label.destroy();
            hookingObjects.label = null;
            hookInHand = false;
        }
    }
});

//entity.getOffsetFromInWorldCoords(offsetX, offsetY, offsetZ);

let wrecksPosition = [
    {position: new mp.Vector3(-203.97333, 6297.5386, 31.493357), rotation: -50.374035},
    {position: new mp.Vector3(-444.3977, 6355.5103, 12.838197), rotation: 35.35763},
    {position: new mp.Vector3(-399.31998, 6382.3647, 14.148448), rotation: 111.12557},
    {position: new mp.Vector3(-599.738, 5320.955, 70.39913), rotation: 172.68416},
    {position: new mp.Vector3(-1585.2396, 5158.7603, 19.622728), rotation: -156.45143},
    {position: new mp.Vector3(-2163.9705, 4280.33, 48.90571), rotation: 149.78302},
    {position: new mp.Vector3(-2248.044, 3479.8757, 30.442886), rotation: -174.94078},
    {position: new mp.Vector3(-2501.5242, 3634.5906, 13.758295), rotation: 166.3186},
    {position: new mp.Vector3(-3063.3708, 1736.377, 36.249866), rotation: -167.3892},
    {position: new mp.Vector3(-3155.6445, 1134.3867, 20.855612), rotation: -21.204744},
    {position: new mp.Vector3(-3019.88, 397.39795, 14.424074), rotation: 170.85068},
    {position: new mp.Vector3(-3052.7195, 171.04027, 11.588536), rotation: -179.69632},
    {position: new mp.Vector3(-2191.59, -417.74026, 13.105328), rotation: -44.64445},
    {position: new mp.Vector3(-1859.8204, -629.19543, 11.231764), rotation: 43.5452},
    {position: new mp.Vector3(-1597.839, -845.7576, 9.997881), rotation: -40.76553},
    {position: new mp.Vector3(-1529.4089, -984.87317, 13.017388), rotation: 140.96523},
    {position: new mp.Vector3(-1320.383, -1255.2214, 4.593919), rotation: -86.1667},
    {position: new mp.Vector3(-965.9996, -1593.1469, 5.0187464), rotation: 20.105507},
    {position: new mp.Vector3(433.01578, 6510.385, 28.284616), rotation: 44.255676},
    {position: new mp.Vector3(-1139.0026, -1252.8375, 6.9506955), rotation: 113.22834},
    {position: new mp.Vector3(1461.7985, 6549.1743, 14.377715), rotation: 171.4294},
    {position: new mp.Vector3(1484.5519, 6366.3745, 23.677631), rotation: 106.57629},
    {position: new mp.Vector3(1682.4733, 6435.378, 32.15536), rotation: -166.77614},
    {position: new mp.Vector3(1969.6942, 5178.3037, 47.84426), rotation: 145.14207},
    {position: new mp.Vector3(2035.0391, 6126.547, 41.829315), rotation: 36.440533},
    {position: new mp.Vector3(1712.4115, 4940.7534, 42.08031), rotation: 55.53721},
    {position: new mp.Vector3(1691.8256, 4805.766, 41.8484), rotation: -175.14703},
    {position: new mp.Vector3(3335.66, 5177.6895, 18.26879), rotation: 143.67531},
    {position: new mp.Vector3(2488.7234, 4123.9707, 38.156944), rotation: 155.51262},
    {position: new mp.Vector3(3812.2998, 4473.5767, 3.8009112), rotation: 131.0244},
    {position: new mp.Vector3(2416.5566, 3743.062, 41.887886), rotation: -0.5267032},
    {position: new mp.Vector3(3804.0906, 4443.765, 4.059309), rotation: -10.169591},
    {position: new mp.Vector3(1978.5438, 3781.0454, 32.180786), rotation: -147.79404},
    {position: new mp.Vector3(3507.3335, 4619.5303, 34.77802), rotation: -88.24081},
    {position: new mp.Vector3(1525.9463, 3916.868, 31.682104), rotation: -108.25423},
    {position: new mp.Vector3(1503.3607, 3763.418, 33.993996), rotation: -150.31505},
    {position: new mp.Vector3(2815.157, 4982.7656, 33.318333), rotation: 134.55954},
    {position: new mp.Vector3(1530.8547, 3608.5894, 35.357777), rotation: -150.13744},
    {position: new mp.Vector3(951.4775, 3615.3416, 32.629345), rotation: 94.11447},
    {position: new mp.Vector3(340.28778, 3569.5676, 33.460636), rotation: -16.89611},
    {position: new mp.Vector3(2866.2996, 4730.606, 48.725784), rotation: 12.066908},
    {position: new mp.Vector3(2933.1072, 4630.3325, 48.545315), rotation: 129.42584},
    {position: new mp.Vector3(58.682243, 3664.3728, 39.609383), rotation: -157.12178},
    {position: new mp.Vector3(-62.55006, 4402.7896, 56.222317), rotation: -27.833866},
    {position: new mp.Vector3(2861.5962, 4468.34, 48.35687), rotation: -169.38882},
    {position: new mp.Vector3(374.95197, 4418.942, 62.260044), rotation: 23.77243},
    {position: new mp.Vector3(705.9432, 4171.987, 40.931328), rotation: -78.701065},
    {position: new mp.Vector3(2950.2192, 4302.7627, 53.08884), rotation: -120.928055},
    {position: new mp.Vector3(2943.4307, 4323.74, 52.40996), rotation: -19.451504},
    {position: new mp.Vector3(959.0364, 4419.678, 47.679794), rotation: -113.66868},
    {position: new mp.Vector3(2977.1152, 3502.7576, 71.38192), rotation: -109.5663},
    {position: new mp.Vector3(2981.048, 3485.9238, 71.38178), rotation: -95.1736},
    {position: new mp.Vector3(2049.8076, 3456.4653, 43.778114), rotation: 81.18279},
    {position: new mp.Vector3(2055.0825, 3179.368, 45.168964), rotation: 64.43525},
    {position: new mp.Vector3(1972.4199, 3032.0974, 47.056297), rotation: -21.135992},
    {position: new mp.Vector3(3644.163, 3760.3152, 28.51574), rotation: 7.414484},
    {position: new mp.Vector3(1245.8246, 2741.5945, 38.424644), rotation: -175.68562},
    {position: new mp.Vector3(3485.1042, 3693.472, 33.888374), rotation: -29.197514},
    {position: new mp.Vector3(1089.8344, 2638.8914, 37.92193), rotation: -5.4999824},
    {position: new mp.Vector3(575.9499, 2794.483, 42.139935), rotation: -83.92071},
    {position: new mp.Vector3(2733.9004, 4295.2134, 48.254818), rotation: 97.38031},
    {position: new mp.Vector3(233.33182, 2572.5532, 46.10053), rotation: -32.314404},
    {position: new mp.Vector3(259.02863, 2578.444, 45.137714), rotation: 97.78712},
    {position: new mp.Vector3(313.64426, 2824.2769, 43.436253), rotation: 30.246862},
    {position: new mp.Vector3(2481.9863, 3766.342, 41.630985), rotation: 69.64343},
    {position: new mp.Vector3(-1161.846, 2671.3938, 18.093895), rotation: -137.02817},
    {position: new mp.Vector3(-1916.0626, 2028.7905, 140.73781), rotation: -109.17096},
    {position: new mp.Vector3(2354.4404, 3154.1416, 48.30034), rotation: -93.40316},
    {position: new mp.Vector3(2371.7637, 3167.1912, 48.07975), rotation: 123.05455},
    {position: new mp.Vector3(2334.061, 3118.6543, 48.19715), rotation: 173.13681},
    {position: new mp.Vector3(2336.9, 3081.9233, 48.140007), rotation: -88.9169},
    {position: new mp.Vector3(-447.92538, 1589.8335, 358.66385), rotation: -124.27985},
    {position: new mp.Vector3(2560.5088, 2738.6426, 43.079655), rotation: -62.749916},
    {position: new mp.Vector3(196.53122, 1238.502, 225.45987), rotation: -76.08265},
    {position: new mp.Vector3(2542.6252, 2577.9858, 37.944874), rotation: -48.32857},
    {position: new mp.Vector3(421.71744, 1196.6372, 248.83478), rotation: 174.93587},
    {position: new mp.Vector3(2527.5688, 2030.6335, 19.81731), rotation: -133.87689},
    {position: new mp.Vector3(1048.5261, 707.19275, 158.36165), rotation: -34.892715},
    {position: new mp.Vector3(2794.2517, 3512.921, 54.767406), rotation: 57.35923},
    {position: new mp.Vector3(1211.7423, 1809.1741, 78.391426), rotation: 36.131138},
    {position: new mp.Vector3(2676.3447, 3463.103, 55.693634), rotation: 140.92511},
    {position: new mp.Vector3(1580.5682, 2210.5437, 78.73199), rotation: 96.5438},
    {position: new mp.Vector3(1219.5852, 2389.6628, 65.6884), rotation: 172.6899},
    {position: new mp.Vector3(2658.6685, 3276.1653, 55.24056), rotation: 119.39909},
    {position: new mp.Vector3(2583.39, 3150.1997, 50.624706), rotation: -83.25035},
    {position: new mp.Vector3(2549.3552, 343.01913, 108.46501), rotation: -96.15909},
    {position: new mp.Vector3(2434.656, -371.45752, 92.99294), rotation: -83.7259},
    {position: new mp.Vector3(1711.3411, -1568.2009, 112.617165), rotation: -88.55594},
    {position: new mp.Vector3(2780.675, -714.30817, 5.3259444), rotation: 76.89136},
    {position: new mp.Vector3(1738.6285, -1633.3168, 112.47537), rotation: 12.043986},
    {position: new mp.Vector3(1523.9326, -2533.8901, 57.294247), rotation: 78.62169},
    {position: new mp.Vector3(2529.6929, -554.15643, 67.17502), rotation: 50.048275},
    {position: new mp.Vector3(1285.0728, -2559.2136, 44.02382), rotation: -29.919437},
    {position: new mp.Vector3(349.51892, 355.05057, 105.00956), rotation: 163.76892},
    {position: new mp.Vector3(234.00594, 132.30872, 102.5997), rotation: 167.89996},
    {position: new mp.Vector3(95.1757, 64.08774, 73.416824), rotation: 77.27524},
    {position: new mp.Vector3(-71.16508, 208.23308, 96.450356), rotation: 88.77449},
    {position: new mp.Vector3(-274.26398, 204.3723, 85.75728), rotation: -119.41544},
    {position: new mp.Vector3(-412.12814, 298.23965, 83.229164), rotation: 172.98782},
    {position: new mp.Vector3(-376.86475, 189.14236, 80.74513), rotation: 93.98841},
    {position: new mp.Vector3(-488.6399, 160.5993, 70.93153), rotation: 81.016655},
    {position: new mp.Vector3(-770.2628, 373.64792, 87.876045), rotation: -175.89761},
    {position: new mp.Vector3(-2284.0598, 407.23547, 174.46666), rotation: 126.710014},
    {position: new mp.Vector3(-2319.268, 317.6795, 169.4671), rotation: 22.609331},
    {position: new mp.Vector3(-1824.299, 780.5335, 137.92355), rotation: -133.42426},
    {position: new mp.Vector3(-1512.7744, 1495.1028, 115.681076), rotation: -107.99066},
    {position: new mp.Vector3(-1159.8665, 932.623, 197.83403), rotation: -36.032894},
    {position: new mp.Vector3(-1070.9232, 677.81665, 142.36612), rotation: -172.86095},
    {position: new mp.Vector3(-806.82477, -569.56757, 30.126255), rotation: -89.33516},
    {position: new mp.Vector3(-300.34216, -744.5085, 43.605663), rotation: 156.0512},
    {position: new mp.Vector3(-330.36984, -931.4518, 31.080614), rotation: 72.63426},
    {position: new mp.Vector3(-339.40146, -987.4482, 30.338968), rotation: -109.079285},
    {position: new mp.Vector3(-347.55884, -1333.4706, 31.365347), rotation: -178.14362},
    {position: new mp.Vector3(-337.23306, -1557.0168, 25.22815), rotation: 64.56131},
    {position: new mp.Vector3(-650.2981, -1722.4233, 24.617552), rotation: -171.42854},
    {position: new mp.Vector3(-610.3172, -1597.5397, 26.751026), rotation: 86.379524},
    {position: new mp.Vector3(-854.54553, -1256.9288, 5.00018), rotation: -131.17253},
    {position: new mp.Vector3(-657.1142, -1153.1501, 9.152241), rotation: -140.2711},
    {position: new mp.Vector3(-810.2396, -764.43896, 21.640207), rotation: 91.10069},
    {position: new mp.Vector3(1376.0197, -740.4522, 67.23283), rotation: 72.56552},
    {position: new mp.Vector3(160.50903, -1145.1268, 29.29142), rotation: -176.82008},
    {position: new mp.Vector3(299.61682, -1182.0145, 29.388418), rotation: 101.70614}
]

let wreckModels = [
    "prop_rub_carwreck_14",
    "prop_rub_carwreck_11",
    "prop_rub_carwreck_9",
    "prop_rub_carwreck_16",
    "prop_wrecked_buzzard",
    "prop_rub_carwreck_7"
]

mp.events.add("attachTowtruck", (towtruck, object) => {
    object.attachTo(towtruck.handle, 0, 0, -1.6, 0.4, 0, 0, 0, true, true, false, false, 0, true);
});

let towingObjects = [];

setInterval(function (){
    for(let i = 0; i < towingObjects.length; i++){
        let object = towingObjects[i];
        if(mp.objects.exists(object.object)){
            if(mp.vehicles.exists(object.vehicle) && mp.vehicles.streamed.includes(object.vehicle)){
                object.object.attachTo(object.vehicle.handle, 0, 0, -1.6, 0.4, 0, 0, 0, true, true, false, false, 0, true);
            }
            else{
                object.object.destroy();
                towingObjects.splice(i, 1);
                break;
            }
        }
        else{
            towingObjects.splice(i, 1);
            break;
        }
    }
}, 5000);

mp.events.add("entityStreamIn", (entity) => {
    if(entity != towtruck && entity.hasVariable("towingobj") && entity.getVariable("towingobj") != ""){
        let obj = mp.objects.new(entity.getVariable("towingobj"), new mp.Vector3(entity.position.x, entity.position.y, entity.position.z + 2.2), {alpha: 0});
        setTimeout(function(){
            if(mp.vehicles.exists(entity) && mp.objects.exists(obj)){
                obj.position = new mp.Vector3(entity.position.x, entity.position.y, entity.position.z + 0.2);
                obj.attachTo(entity.handle, 0, 0, -1.6, 0.4, 0, 0, 0, true, true, false, false, 0, true);
                obj.setAlpha(255);
                towingObjects.push({
                    vehicle: entity,
                    object: obj
                });
            }
        }, 5000);
    }
})

mp.events.add("entityStreamOut", (entity) => {
    if(entity != towtruck && entity.hasVariable("towingobj") && entity.getVariable("towingobj") != ""){
        for(let i = 0; i < towingObjects.length; i++){
            if(towingObjects[i].vehicle == entity){
                if(towingObjects[i].object != null && mp.objects.exists(towingObjects[i].object)){
                    towingObjects[i].object.destroy();
                    towingObjects.splice(i, 1);
                    break;
                }
            }
        }
    }
});

mp.events.addDataHandler("towingobj", (entity, value, oldvalue) => {
    if(entity != towtruck){
        mp.vehicles.forEachInStreamRange(vehicle => {

            if(vehicle == entity){
                if(value != ""){
                    let obj = mp.objects.new(value, new mp.Vector3(entity.position.x, entity.position.y, entity.position.z + 2.2), {alpha: 0});
                    setTimeout(function(){
                       if(mp.vehicles.exists(entity) && mp.objects.exists(obj)){
                            obj.position = new mp.Vector3(entity.position.x, entity.position.y, entity.position.z + 0.2);
                            obj.attachTo(vehicle.handle, 0, 0, -1.6, 0.4, 0, 0, 0, true, true, false, false, 0, true);
                            obj.setAlpha(255);
                            towingObjects.push({
                                vehicle: entity,
                                object: obj
                            });
                       }
                        
                    }, 5000);
                    
                }
                else{
                    for(let i = 0; i < towingObjects.length; i++){
                        if(towingObjects[i].vehicle == entity){
                            if(towingObjects[i].object != null && mp.objects.exists(towingObjects[i].object)){
                                towingObjects[i].object.destroy();
                                towingObjects.splice(i, 1);
                                break;
                            }
                        }
                    }
                }
            }
        })
    }
});