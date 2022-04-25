let player = mp.players.local;
let object = null;
let soundHint = null;

mp.events.add("render", () => {
    if(player.hasVariable("job") && player.getVariable("job") == "diver" && object != null && object.obj != null && mp.objects.exists(object.obj) && getDistance(player.position, object.obj.getCoords(true)) <= 15){
        if(soundHint == null){
            soundHint = setInterval(function(){
                if(player.hasVariable("job") && player.getVariable("job") == "diver" && object != null && object.obj != null && mp.objects.exists(object.obj) && getDistance(player.position, object.obj.getCoords(true)) <= 15){
                    let pos = object.obj.getCoords(true);
                    mp.game.audio.playSoundFrontend(-1, "Power_Down", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true);
                }
                else{
                    clearInterval(soundHint);
                    soundHint = null;
                }
            },2000);
        }
    }   
});

mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(entity.type === "player" && entity == player)
    {
        if(oldvalue === "" && value === "diver"){
            player.setMaxTimeUnderwater(300);
            createObject();
        }
        if(oldvalue === "diver" && value === ""){
            player.setMaxTimeUnderwater(30);
            if(object != null){
                object.shape.destroy();
                object.obj.destroy();
                object.blip.destroy();
                object.marker.destroy();
                object = null;
            }
            if(soundHint != null){
                clearInterval(soundHint);
            }
        }
    }
});

mp.events.add("playerEnterColshape", (shape) => {
    if(player.getVariable("job") == "diver")
    {
        if(object != null && object.shape == shape){
            mp.events.callRemote("diver_payment", object.price);
            createObject();
        }
    }
});

function createObject(){
    if(object != null){
        object.shape.destroy();
        object.obj.destroy();
        object.blip.destroy();
        object.marker.destroy();
        object = null;
    }
    let pos = objPositions[getRandomInt(0, objPositions.length)];
    let objVals = objects[getRandomInt(0, objects.length)];
    let shape = mp.colshapes.newTube(pos.x, pos.y, pos.z, 1.0, 1.0);
    let obj = mp.objects.new(objVals.obj, pos);
    let priceMult = objVals.price;
    let randNumX = getRandomInt(-25, 26);
    let randNumY = getRandomInt(-25, 26);
    let randPos = new mp.Vector3(pos.x + randNumX, pos.y + randNumY, pos.z - 25)
    let blip = mp.blips.new(66, randPos, {
        name: "Przybliżone położenie przedmiotu",
        shortRange: false,
        color: 30,
        scale: 0.8
    })
    let marker = mp.markers.new(1, randPos, 75, {
        color: [0,204,153,100]
    })
    object = {shape: shape, blip: blip, marker: marker, obj: obj, price: priceMult};
}

function getRandomInt(min, max){
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
}

let objPositions = [
    new mp.Vector3(2166.512, 4366.335, 1.97214),
    new mp.Vector3(2169.818, 4372.874, 2.498489),
    new mp.Vector3(2176.809, 4368.945, 1.570783),
    new mp.Vector3(2225.883, 4238.33, 0.1371989),
    new mp.Vector3(2222.541, 4233.639, -0.2134709),
    new mp.Vector3(2117.097, 4210.601, -10.28428),
    new mp.Vector3(2126.603, 4230.643, -13.61009),
    new mp.Vector3(2077.344, 4222.788, -8.198623),
    new mp.Vector3(2113.744, 4268.587, -11.62111),
    new mp.Vector3(2162.382, 4244.979, -13.63813),
    new mp.Vector3(1484.208, 4122.399, -4.668849),
    new mp.Vector3(1484.208, 4112.657, -5.013647),
    new mp.Vector3(1506.676, 4109.729, -5.08209),
    new mp.Vector3(1805.124, 4147.916, 1.61786),
    new mp.Vector3(1801.377, 4143.227, 2.094444),
    new mp.Vector3(1807.578, 4154.926, 1.243599),
    new mp.Vector3(1679.741, 4165.909, -0.3365602),
    new mp.Vector3(1663.032, 4165.909, 0.9063841),
    new mp.Vector3(108.932, 4055.773, -10.37599),
    new mp.Vector3(123.7105, 4050.409, -12.68833),
    new mp.Vector3(117.6761, 4088.503, -12.645),
    new mp.Vector3(92.14497, 4087.305, -12.59654),
    new mp.Vector3(366.6047, 4011.9, 0.09211206),
    new mp.Vector3(366.6047, 3985.303, -6.321949),
    new mp.Vector3(409.5891, 3983.381, 2.507679),
    new mp.Vector3(419.7058, 4026.941, 11.50285),
    new mp.Vector3(543.1458, 3900.063, 1.294696),
    new mp.Vector3(537.6809, 3886.361, 3.64163),
    new mp.Vector3(524.4313, 3888.568, 3.95455),
    new mp.Vector3(650.3351, 3948.557, 2.552982),
    new mp.Vector3(647.576, 3956.862, 2.867309),
    new mp.Vector3(657.9222, 3946.973, 3.586605),
    new mp.Vector3(728.2129, 3915.902, 0.7616251),
    new mp.Vector3(719.543, 3911.644, 1.875236),
    new mp.Vector3(732.8088, 3925.742, 4.58344),
    new mp.Vector3(824.3887, 3934.373, -6.623455),
    new mp.Vector3(817.7214, 3922.487, -9.262172),
    new mp.Vector3(810.4413, 3938.139, -5.895222),
    new mp.Vector3(889.1727, 3990.705, 4.023368),
    new mp.Vector3(886.7363, 3981.533, 3.729982),
    new mp.Vector3(876.45, 3993.63, 4.325731),
    new mp.Vector3(1156.58, 3981.732, -8.51223),
    new mp.Vector3(1157.626, 3992.544, -6.062324),
    new mp.Vector3(1158.364, 3986.541, -7.454257),
    new mp.Vector3(1222.317, 3890.598, 3.394983),
    new mp.Vector3(1217.168, 3894.464, 3.385759),
    new mp.Vector3(1308.201, 4115.805, 11.2819),
    new mp.Vector3(1298.171, 4126.578, 14.83771),
    new mp.Vector3(1295.929, 4117.827, 13.80123),
    new mp.Vector3(1527.538, 4256.793, 7.31969),
    new mp.Vector3(1524.978, 4245.802, 6.245814),
    new mp.Vector3(1515.081, 4254.031, 10.00557),
    new mp.Vector3(1949.95, 4349.125, 2.620837),
    new mp.Vector3(1962.745, 4337.38, 4.417394),
    new mp.Vector3(1963.952, 4349.606, 5.075187),
    new mp.Vector3(1501.444, 4173.236, 12.76101),
    new mp.Vector3(1485.879, 4195.067, 12.35108),
    new mp.Vector3(1515.642, 4199.245, 12.19005),
    new mp.Vector3(1375.683, 4020.459, 6.504751),
    new mp.Vector3(1348.88, 4012.789, 6.016066),
    new mp.Vector3(1344.808, 4046.704, -1.052157),
    new mp.Vector3(1256.402, 4057.135, 2.109062),
    new mp.Vector3(1251.781, 4041.636, 2.219529),
    new mp.Vector3(1274.05, 4037.297, 1.349946)
]

let objects = [
    {obj: "prop_gold_bar", price: 5},
    {obj: "vw_prop_vw_pogo_gold_01a", price: 5},
    {obj: "v_res_m_spanishbox", price: 4},
    {obj: "prop_cardbordbox_05a", price: 1},
    {obj: "prop_ld_case_01", price: 2},
    {obj: "prop_ld_suitcase_02", price: 2},
    {obj: "prop_big_bag_01", price: 1},
    {obj: "prop_rub_bike_01", price: 1},
    {obj: "hei_heist_acc_vase_02", price: 3},
    {obj: "prop_kitch_pot_huge", price: 1},
    {obj: "v_res_r_perfume", price: 3}
]

mp.events.add("playerLeaveVehicle", (vehicle, seat) => {
    if(player.getVariable("job") == "diver" && vehicle != null && mp.vehicles.exists(vehicle) && vehicle.remoteId == player.getVariable("jobveh")){
        mp.events.callRemote("freezeJobVeh", true, vehicle);
    }
});

mp.events.add("playerEnterVehicle", (vehicle, seat) => {
    if(player.getVariable("job") == "diver" && vehicle != null && mp.vehicles.exists(vehicle) && vehicle.remoteId == player.getVariable("jobveh")){
        mp.events.callRemote("freezeJobVeh", false, vehicle);
    }
});