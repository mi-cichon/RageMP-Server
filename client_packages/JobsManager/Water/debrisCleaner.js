let player = mp.players.local;
let binBag = null;
let weight = 0;

let containerCol = null;
let containerPos = new mp.Vector3(-1712.341, -992.58655, 5.785871);

let containerMarker = null;
let containerBlip = null;

let debris = [];

let debrisPositions = [
    new mp.Vector3(-1743.9023, -1021.5256, 2.4997644),
    new mp.Vector3(-1750.2355, -1031.2618, 1.8297617),
    new mp.Vector3(-1752.7649, -1040.8584, 1.1679027),
    new mp.Vector3(-1746.1033, -1046.9844, 1.0570533),
    new mp.Vector3(-1737.0421, -1049.3613, 1.3741031),
    new mp.Vector3(-1731.2186, -1057.1754, 1.0973002),
    new mp.Vector3(-1725.5424, -1057.3756, 1.4145538),
    new mp.Vector3(-1717.1254, -1065.8263, 1.2945476),
    new mp.Vector3(-1717.7551, -1052.3176, 2.1028676),
    new mp.Vector3(-1723.1213, -1041.5679, 2.3142464),
    new mp.Vector3(-1715.4507, -1036.0283, 3.1088474),
    new mp.Vector3(-1718.8005, -1029.5549, 3.8507445),
    new mp.Vector3(-1729.8583, -1023.9126, 3.861432),
    new mp.Vector3(-1738.214, -1016.24963, 3.4822752),
    new mp.Vector3(-1748.2498, -1018.0385, 2.4724507),
    new mp.Vector3(-1755.7936, -1010.4245, 2.5975156),
    new mp.Vector3(-1753.3347, -1000.34705, 3.4521744),
    new mp.Vector3(-1769.6407, -1007.8667, 2.489786),
    new mp.Vector3(-1777.8212, -1003.1178, 2.5911171),
    new mp.Vector3(-1791.4591, -1003.6607, 1.6540551),
    new mp.Vector3(-1784.1492, -988.6508, 2.8620002),
    new mp.Vector3(-1771.6382, -978.9892, 4.2982926),
    new mp.Vector3(-1757.3529, -980.6201, 4.951533), 
];

let debrisObjects = [
    {obj: "prop_rub_binbag_03", weight: 2},
    {obj:"prop_michael_backpack", weight: 3},
    {obj:"prop_rub_tyre_dam2", weight: 4},
    {obj:"prop_pizza_box_02", weight: 2},
    {obj:"prop_rub_litter_05", weight: 1},
    {obj:"prop_rub_trainers_01c", weight: 1},
    {obj:"prop_food_bs_juice01", weight: 1},
    {obj:"prop_food_bs_burg3", weight: 2},
    {obj:"prop_rub_litter_06", weight: 2},
    {obj:"prop_rub_tyre_03", weight: 3}
];

mp.events.add("render", () => {
    if(mp.objects.exists(binBag)){
        let text = "";
        if(weight < 50){
            text = "Pojemność worka: " + weight.toString() + "/50L";
        }
        else{
            text = "Opróżnij worek przy kontenerach";
        }
        let pos = new Float64Array([0.5, 0.95]);
            mp.game.graphics.drawText(text, pos, { 
                font: 4, 
                color: [255, 253, 141, 255], 
                scale: [0.7, 0.7], 
                outline: true
            });
    }
});
mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(entity.type === "player" && entity == player)
    {
        if(oldvalue === "" && value === "debriscleaner"){
            binBag = mp.objects.new(mp.game.joaat("prop_cs_rub_binbag_01"), player.position, {
                alpha: 0
            });
            containerCol = mp.colshapes.newTube(containerPos.x, containerPos.y, containerPos.z, 1.0, 2.0);
            setTimeout(() => {
                if(mp.objects.exists(binBag)){
                    binBag.attachTo(player.handle, player.getBoneIndex(57005), 0.12, 0.02, 0, -90, -110, 0, true, false, false, false, 0, true);
                    binBag.setAlpha(255);
                }
                
            }, 500);
            containerCol = mp.colshapes.newTube(containerPos.x, containerPos.y, containerPos.z, 1.0, 2.0);
            containerMarker = mp.markers.new(27, containerPos.add(new mp.Vector3(0,0,-0.8)), 4, {
                color: [0, 204, 153, 255]
            });
            containerBlip = mp.blips.new(365, containerPos, {
                color: 15,
                name: "Kontener z odpadami",
                scale: 0.6
            });
            createObjects();
            mp.events.callRemote("putItemInHand", "binbag");
        }
        if(oldvalue === "debriscleaner" && value === ""){
            mp.events.callRemote("removeItemFromHand");
            debris.forEach(d => {
                if(d.object != null && mp.objects.exists(d.object)){
                    d.object.destroy();
                }
                if(d.colshape != null && mp.colshapes.exists(d.colshape)){
                    d.colshape.destroy();
                }
                if(d.blip != null && mp.blips.exists(d.blip)){
                    d.blip.destroy();
                }
                if(d.marker != null && mp.markers.exists(d.marker)){
                    d.marker.destroy();
                }
            });
            if(mp.blips.exists(containerBlip)){
                containerBlip.destroy();
            }
            if(mp.markers.exists(containerMarker)){
                containerMarker.destroy();
            }
            if(mp.colshapes.exists(containerCol)){
                containerCol.destroy();
            }
            if(mp.objects.exists(binBag)){
                binBag.destroy();
            }
            debris = [];
            weight = 0;
        }
    }
});

mp.events.add("playerEnterColshape", (shape) => {
    if(player.getVariable("job") == "debriscleaner")
    {
        for(let i = 0; i < debris.length; i++){
            if(debris[i].colshape == shape){
                if(weight + debris[i].weight <= 50){
                    let d = debris[i];
                    mp.game.streaming.requestAnimDict("amb@prop_human_bum_bin@base");
                    player.taskPlayAnim("amb@prop_human_bum_bin@base", "base", 1.0, 1.0, 1000, 2, 1.0, false, false, false);
                    player.freezePosition(true);
                    setTimeout(function(){
                        player.freezePosition(false);
                        weight += d.weight;
                        d.colshape.destroy();
                        d.blip.destroy();
                        d.object.destroy();
                        d.marker.destroy();
                        createRandomObject(i);
                    }, 1000);
                    break;
                }
                else{
                    mp.events.call("showNotification", "Zapełniłeś worek, opróżnij go przy kontenerze!");
                    
                }
            }
        }
    }
    if(mp.colshapes.exists(containerCol) && shape == containerCol){
        if(containerBlip != null && mp.blips.exists(containerBlip)){
            containerBlip.destroy();
            containerBlip = null;
        }
        if(weight > 0){
            calculateReward();
        }
    }
});

function calculateReward(){
    mp.events.callRemote("debrisCleanerReward", weight);
    weight = 0;
}

function createObjects(){
    let p = debrisPositions.sort(function () {
        return Math.random() - 0.5;
    });
    p = p.splice(0, 4);

    p.forEach(pos => {
        let obj = debrisObjects[getRandomInt(0, debrisObjects.length)];
        debris.push({
            position: pos - new mp.Vector3(0, 0, 1),
            object: mp.objects.new(obj.obj, new mp.Vector3(pos.x, pos.y, pos.z - 1)),
            colshape: mp.colshapes.newTube(pos.x, pos.y, pos.z, 1.5, 2.0),
            blip: mp.blips.new(570, pos, {
                color: 15,
                name: "Odpad do zebrania",
                scale: 0.6
            }),
            marker: mp.markers.new(0, pos.add(new mp.Vector3(0,0,1.3)), 0.6, {
                color: [0, 204, 153, 255]
            }),
            weight: obj.weight
        })
    });
}

function createRandomObject(index){
    let pos;
    let exists = false;
    do{
        pos = debrisPositions[getRandomInt(0, debrisPositions.length)];
        exists = false;
        debris.forEach(d => {
            if(d.pos == pos){
                exists = true;
            }
        });
    }while(exists);

    let obj = debrisObjects[getRandomInt(0, debrisObjects.length)];
    debris[index] = {
        position: pos,
        object: mp.objects.new(obj.obj, new mp.Vector3(pos.x, pos.y, pos.z - 1)),
        colshape: mp.colshapes.newTube(pos.x, pos.y, pos.z, 1.5, 2.0),
        blip: mp.blips.new(570, pos, {
            color: 15,
            name: "Odpad do zebrania",
            scale: 0.6
        }),
        marker: mp.markers.new(0, pos.add(new mp.Vector3(0,0,1.3)), 0.6, {
            color: [0, 204, 153, 255]
        }),
        weight: obj.weight
    }
}

function getRandomInt(min, max){
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
}
