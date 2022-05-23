let player = mp.players.local;
let currentOrder = null;
let orderState = null;
let areaBlips = [];
let gardenerVeh = null;
mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(value == "gardener" && entity == player){
        areaBlips.push(mp.blips.new(535, new mp.Vector3(2605.381, 4875.1772, 35.280144), {
            color: 15,
            name: "Ogrodnik: Fioletowe bratki",
            scale: 0.6,
            shortRange: false
        }));

        areaBlips.push(mp.blips.new(536, new mp.Vector3(-982.2783, 309.72278, 71.02862), {
            color: 15,
            name: "Ogrodnik: Różowe bratki",
            scale: 0.6,
            shortRange: false
        }));

        areaBlips.push(mp.blips.new(537, new mp.Vector3(-1653.5127, 4999.3184, 37.90199), {
            color: 15,
            name: "Ogrodnik: Żółte bratki",
            scale: 0.6,
            shortRange: false
        }));

        areaBlips.push(mp.blips.new(538, new mp.Vector3(1448.2893, -2661.536, 41.61088), {
            color: 15,
            name: "Ogrodnik: Figowce",
            scale: 0.6,
            shortRange: false
        }));

        areaBlips.push(mp.blips.new(539, new mp.Vector3(-468.00656, 1557.298, 377.93823), {
            color: 15,
            name: "Ogrodnik: Draceny",
            scale: 0.6,
            shortRange: false
        }));
        areaBlips.push(mp.blips.new(480, new mp.Vector3(-1275.7875, -1139.7943, 6.7924547), {
            color: 75,
            name: "Ogrodnik: Zleceniodawca",
            scale: 0.8,
            shortRange: false
        }));
        if(player.vehicle)
            gardenerVeh = player.vehicle;
    }
    if(oldvalue == "gardener" && entity==player){
        areaBlips.forEach(blip => {
            if(blip != null){
                blip.destroy();
            }
        });
        gardenerVeh = null;
        areaBlips = [];
    }
});

mp.events.add("gardener_setNewOrder", order => {
    currentOrder = JSON.parse(order);
    orderState = JSON.parse(order);
    mp.events.call("openGardenerHUDBrowser", order);
});

mp.events.add("gardener_pickupAnimate", (type) => {
    let time = (type == 904 || type == 903) ? 5000 : 3000;
    if (player.getVariable("jobBonus_112"))
    {
        time /= 2;
    }

    mp.game.streaming.requestAnimDict("amb@prop_human_bum_bin@base");
    player.taskPlayAnim("amb@prop_human_bum_bin@base", "base", 1.0, 1.0, time, 2, 1.0, false, false, false);
    player.freezePosition(true);
    setTimeout(function(){
        player.freezePosition(false);
    }, time);
});

mp.events.add("gardener_updateOrder", order => {
    orderState = JSON.parse(order);
    mp.events.call("gardener_insertHUDData", order);
})

mp.events.add("gardener_sellPlants", () => {
    if(currentOrder && orderState){
        mp.events.callRemote("gardener_sellPlants", JSON.stringify(currentOrder), JSON.stringify(orderState));
    }
});


mp.events.add("saveData_gardener_load", (data) => {
    let saveData = JSON.parse(data);
    currentOrder = JSON.parse(saveData[1]);
    orderState = JSON.parse(saveData[1]);
    mp.events.call("openGardenerHUDBrowser", saveData[1]);
    mp.events.callRemote("saveData_giveJobVeh", "gardener", JSON.parse(saveData[2]), saveData[3]);
});

mp.events.add("saveData_gardener_save", () => {
    if(mp.vehicles.exists(gardenerVeh) && currentOrder != null){
        let saveData = ["gardener", JSON.stringify(currentOrder), JSON.stringify(gardenerVeh.position), gardenerVeh.getVariable("trunk")];
        mp.events.callRemote("saveData_saveJobData", JSON.stringify(saveData));
    }
});

mp.events.addDataHandler("jobveh", (entity, value, oldvalue) => {
    if(mp.players.local.getVariable("job") == "gardener"){
        if(entity == mp.players.local && value != -1111){
            let v = mp.vehicles.atRemoteId(value);
            if(v != null && mp.vehicles.exists(v) && v.hasVariable("jobtype") && v.getVariable("jobtype") == entity.getVariable("job")){
                gardenerVeh = v;
            }
        }
    }
});