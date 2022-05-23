let player = mp.players.local;
let binBag = null;
let weight = 0;

let containerCol = null;
let containerPos = new mp.Vector3(-1712.341, -992.58655, 5.785871);

let containerMarker = null;
let containerBlip = null;

let maxWeight = 50;
let debris = [];

let debrisPositions = [
    new mp.Vector3(-1706.4617, -1053.6727, 2.346646),
    new mp.Vector3(-1705.261, -1044.4805, 2.8992314),
    new mp.Vector3(-1713.27, -1044.3229, 2.7436664),
    new mp.Vector3(-1722.9868, -1042.5895, 2.1475074),
    new mp.Vector3(-1722.7334, -1034.9575, 3.012834),
    new mp.Vector3(-1714.9587, -1028.1919, 3.955647),
    new mp.Vector3(-1720.2971, -1022.1067, 4.479687),
    new mp.Vector3(-1728.8652, -1024.0564, 3.9164975),
    new mp.Vector3(-1737.243, -1026.971, 2.6393595),
    new mp.Vector3(-1743.748, -1026.2957, 2.2567372),
    new mp.Vector3(-1745.2842, -1020.4136, 2.4989116),
    new mp.Vector3(-1741.0444, -1014.40106, 3.3998861),
    new mp.Vector3(-1734.4132, -1011.3492, 4.0937176),
    new mp.Vector3(-1737.0028, -1005.3363, 4.3004937),
    new mp.Vector3(-1743.0264, -1006.83606, 3.7880576),
    new mp.Vector3(-1748.6636, -1012.288, 2.9216132),
    new mp.Vector3(-1754.067, -1016.1665, 2.314176),
    new mp.Vector3(-1760.4244, -1013.6939, 2.3348083),
    new mp.Vector3(-1761.4121, -1008.154, 2.661808),
    new mp.Vector3(-1758.766, -1002.2547, 3.0944145),
    new mp.Vector3(-1759.8558, -996.28845, 3.7944467),
    new mp.Vector3(-1765.9939, -997.73444, 3.480331),
    new mp.Vector3(-1771.2256, -1000.58276, 3.0442467),
    new mp.Vector3(-1776.3501, -1003.3504, 2.6520796),
    new mp.Vector3(-1781.8038, -1000.35767, 2.6592748),
    new mp.Vector3(-1784.5198, -994.56934, 2.8328831),
    new mp.Vector3(-1782.2485, -989.1694, 3.0312903),
    new mp.Vector3(-1776.6134, -991.20416, 3.4870026),
    new mp.Vector3(-1773.9354, -986.0348, 3.7691214),
    new mp.Vector3(-1766.5614, -986.1321, 4.2316494),
    new mp.Vector3(-1760.4924, -990.52234, 4.218505),
    new mp.Vector3(-1763.172, -999.3295, 3.3394535),
    new mp.Vector3(-1765.2454, -1008.19464, 2.608338),
    new mp.Vector3(-1762.343, -1016.2891, 2.1190479),
    new mp.Vector3(-1754.7478, -1017.58997, 2.1928902),
    new mp.Vector3(-1746.2897, -1015.1693, 2.8440778),
    new mp.Vector3(-1738.9473, -1012.52295, 3.6884172),
    new mp.Vector3(-1733.1908, -1014.60675, 4.0693293),
    new mp.Vector3(-1733.1863, -1020.843, 3.7749033),
    new mp.Vector3(-1737.0231, -1027.4272, 2.6317227),
    new mp.Vector3(-1736.7238, -1033.6348, 2.2695382),
    new mp.Vector3(-1731.0804, -1037.1145, 2.3924012),
    new mp.Vector3(-1722.9253, -1033.9698, 3.170407),
    new mp.Vector3(-1713.4766, -1033.7874, 3.5233002),
    new mp.Vector3(-1713.0424, -1040.7513, 2.7333531),
    new mp.Vector3(-1718.7489, -1043.9377, 2.3988004),
    new mp.Vector3(-1718.5454, -1049.8894, 2.1455207),
    new mp.Vector3(-1725.4459, -1024.2422, 4.1073136),
    new mp.Vector3(-1730.3346, -1013.3574, 4.414798),
    new mp.Vector3(-1739.5688, -1006.6694, 4.0521755)
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
        if(weight < maxWeight){
            text = "Zapełnienie worka: " + weight.toString() + `/${maxWeight}L`;
        }
        else{
            text = "Opróżnij worek przy kontenerze";
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
            maxWeight = player.getVariable("jobBonus_55") ? 120 : player.getVariable("jobBonus_54") ? 90 : player.getVariable("jobBonus_53") ? 60 : 50;
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
                if(weight + debris[i].weight <= maxWeight){
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
                        mp.events.callRemote("debrisCleaner_luckCheck");
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
