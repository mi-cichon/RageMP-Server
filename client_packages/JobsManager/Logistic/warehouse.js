let player = mp.players.local;
let boxColshape = null;
let boxBlip = null;
let boxMarker = null;
let boxObj = null;
let whTimeout = null;

let boxesPoint = new mp.Vector3(-82.46917, 6496.3374, 31.490892);
let boxesBlip = null;
let boxesMarker = null;
let boxesColshape = null;

let boxPositions = [
    new mp.Vector3(-88.3214, 6483.71, 31.49091),
    new mp.Vector3(-96.12928, 6491.716, 31.490917),
    new mp.Vector3(-67.33175, 6487.8296, 31.494705),
    new mp.Vector3(-62.492645, 6492.4937, 31.495716),
    new mp.Vector3(-63.79983, 6501.7734, 31.490894)
];

mp.events.add("render", () => {
    if(!player.getVariable("jobBonus_6") && mp.objects.exists(boxObj)){
        mp.game.controls.disableControlAction(32, 21, true);
        mp.game.controls.disableControlAction(32, 22, true);
    }
});


mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(entity.type === "player" && entity == player)
    {
        if(oldvalue === "" && value === "warehouse"){
            createNewBoxPoint();
            mp.events.call("showNotification", "Masz 90 sekund na przyniesienie wyznaczonej paczki!");
        }
        if(oldvalue === "warehouse" && value === ""){
           if(mp.blips.exists(boxBlip)){
                boxBlip.destroy();
            }
           if(mp.blips.exists(boxesBlip)){
                boxesBlip.destroy();
            }
            if(mp.colshapes.exists(boxColshape)){
                boxColshape.destroy();
            }
            if(mp.colshapes.exists(boxesColshape)){
                boxesColshape.destroy();
            }
            if(mp.markers.exists(boxMarker)){
                boxMarker.destroy();
            }
            if(mp.markers.exists(boxesMarker)){
                boxesMarker.destroy();
            }
            if(mp.objects.exists(boxObj)){
                mp.events.callRemote("removeItemFromHand");
                boxObj.destroy();
            }
            if(whTimeout){
                clearTimeout(whTimeout);
                whTimeout = null;
            }
            player.clearTasksImmediately();
        }
    }
});

mp.events.add("playerEnterColshape", (shape) => {
    if(shape == boxColshape){
        boxColshape.destroy();
        boxBlip.destroy();
        boxMarker.destroy();
        boxObj = mp.objects.new(1302435108, player.position, {
            alpha: 0
        });
        mp.game.streaming.requestAnimDict("anim@heists@box_carry@");
        player.taskPlayAnim("anim@heists@box_carry@", "idle", 1.0, 1.0, -1, 63, 1.0, false, false, false);
        setTimeout(() => {
            boxObj.attachTo(player.handle, player.getBoneIndex(57005), 0.08, 0, -0.27, 0, 65, 340, true, false, false, false, 0, true);
            boxObj.setAlpha(255);
            mp.events.callRemote("putItemInHand", "package");
        }, 100);
        
        boxesBlip = mp.blips.new(570, boxesPoint, {
            color: 15,
            name: "Punkt składania paczek",
            scale: 0.6
        });
        boxesMarker = mp.markers.new(27, new mp.Vector3(boxesPoint.x, boxesPoint.y, boxesPoint.z - 0.6), 2.0, {
            color: [0, 204, 153, 255]
        });
        boxesColshape = mp.colshapes.newTube(boxesPoint.x, boxesPoint.y, boxesPoint.z, 2.0, 2.0);
    }

    if(mp.colshapes.exists(boxesColshape) && shape == boxesColshape){
        boxObj.destroy();
        mp.events.callRemote("removeItemFromHand");
        player.clearTasksImmediately();
        mp.events.callRemote("warehouseBoxDelievered");
        boxesColshape.destroy();
        boxesMarker.destroy();
        boxesBlip.destroy();
        createNewBoxPoint();
    }
});

function createNewBoxPoint(){
    warehouseTimeout();
    let pos = boxPositions[getRandomInt(0, boxPositions.length)];
    boxColshape = mp.colshapes.newTube(pos.x, pos.y, pos.z, 1.0, 2.0);
    boxBlip = mp.blips.new(570, pos, {
        color: 15,
        name: "Paczka",
        scale: 0.6
    });
    boxMarker = mp.markers.new(27, new mp.Vector3(pos.x, pos.y, pos.z - 0.6), 0.6, {
        color: [0, 204, 153, 255]
    });
}


function getRandomInt(min, max){
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
}

function warehouseTimeout(){
    if(whTimeout){
        clearTimeout(whTimeout);
    }
    whTimeout = setTimeout(() => {
        mp.events.call("showNotification", "Praca została zakończona ponieważ nie byłeś wystarczająco aktywny!");
        mp.events.callRemote("endJob");
        whTimeout = null;
    }, 90000);
}