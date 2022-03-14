let handObjects = [];
let objTypes = [
    {name: "fishingrod", obj:"prop_fishing_rod_01", bone: 18905, val: [0.12, 0.1, 0, -120, -80, 0]},
    {name: "binbag", obj:"prop_cs_rub_binbag_01", bone: 57005, val: [0.12, 0.02, 0, -90, -110, 0]},
    {name: "package", obj: 1302435108, bone: 57005, val: [0.08, 0, -0.27, 0, 65, 340]},
    {name: "forkliftsBox", obj: "prop_boxpile_06a", bone: 4, val: [0, 0.2, -0.1, 0, 0, 90]}
]
setInterval(function(){
    handObjects.forEach(obj => {
        for(let i = 0; i < handObjects.length; i++){
            if(handObjects[i].player != null && !mp.players.exists(handObjects[i].player) && mp.objects.exists(handObjects[i].obj)){
                handObjects[i].obj.destroy();
                handObjects.splice(i, 1);
                break;
            }
        }
    });
}, 1000)
mp.events.add("entityStreamIn", (entity) => {
    if(entity.type === "player" && entity!=mp.players.local && entity.hasVariable("handObj") && entity.getVariable("handObj") != ""){
        objTypes.forEach(obj => {
            if(obj.name == entity.getVariable("handObj")){
                let item = mp.objects.new(obj.obj, new mp.Vector3(entity.position.x,entity.position.y,entity.position.z + 2), {
                    alpha: 0
                });
                setTimeout(() => {
                    if(obj.name == "forkliftsBox" && mp.objects.exists(item) && mp.players.exists(entity) && entity.vehicle && mp.vehicles.exists(entity.vehicle)){
                        item.attachTo(entity.vehicle.handle, entity.vehicle.getBoneIndexByName("forks_attach"), obj.val[0], obj.val[1], obj.val[2], obj.val[3], obj.val[4], obj.val[5], true, false, false, false, 0, true);
                        item.setAlpha(255);
                    }
                    else if(mp.players.exists(entity) && entity.vehicle && mp.vehicles.exists(entity.vehicle) && mp.objects.exists(item)){
                        item.attachTo(entity.handle, entity.getBoneIndex(obj.bone), obj.val[0], obj.val[1], obj.val[2], obj.val[3], obj.val[4], obj.val[5], true, false, false, false, 0, true);
                        item.setAlpha(255);
                        if(obj.name == "package"){
                            mp.game.streaming.requestAnimDict("anim@heists@box_carry@");
                            entity.taskPlayAnim("anim@heists@box_carry@", "idle", 1.0, 1.0, -1, 63, 1.0, false, false, false);
                        }
                    }
                }, 500);
                let exists = false;
                handObjects.forEach(handO => {
                    if(handO.player == entity){
                        if(handO.obj != null && mp.objects.exists(handO.obj)){
                            handO.obj.destroy();
                            handO.obj = item;
                            exists = true;
                        }
                    }
                })
                if(!exists){
                    handObjects.push({
                        player: entity,
                        obj: item
                    })
                }
            }
        }); 
    }
})

mp.events.add("entityStreamOut", (entity) => {
    if(entity === "player" && entity!=mp.players.local && entity.hasVariable("handObj") && entity.getVariable("handObj") != ""){
        for(let i = 0; i < handObjects.length; i++){
            if(handObjects[i].player == entity){
                if(handObjects[i].obj != null && mp.objects.exists(handObjects[i].obj)){
                    handObjects[i].obj.destroy();
                    handObjects.splice(i, 1);
                    entity.stopAnimTask("anim@heists@box_carry@", "idle", 3.0);
                    break;
                }
            }
        }
    }
});

mp.events.addDataHandler("handObj", (entity, value, oldvalue) => {
    if(entity.type === "player" && entity != mp.players.local){
        mp.players.forEachInStreamRange(player => {
            if(player == entity){
                if(value != ""){
                    objTypes.forEach(obj => {
                        if(obj.name == value)
                        {
                            let item = mp.objects.new(obj.obj, new mp.Vector3(entity.position.x,entity.position.y,entity.position.z + 2), {
                                alpha: 0
                            });
                            setTimeout(() => {
                                if(item != null && mp.objects.exists(item) && mp.players.exists(entity)){
                                    if(obj.name == "forkliftsBox" && player.vehicle){
                                        item.attachTo(player.vehicle.handle, player.vehicle.getBoneIndexByName("forks_attach"), obj.val[0], obj.val[1], obj.val[2], obj.val[3], obj.val[4], obj.val[5], true, false, false, false, 0, true);
                                        item.setAlpha(255);
                                    }
                                    else{
                                        item.attachTo(entity.handle, entity.getBoneIndex(obj.bone), obj.val[0], obj.val[1], obj.val[2], obj.val[3], obj.val[4], obj.val[5], true, false, false, false, 0, true);
                                        item.setAlpha(255);
                                        if(obj.name == "package"){
                                            mp.game.streaming.requestAnimDict("anim@heists@box_carry@");
                                            player.taskPlayAnim("anim@heists@box_carry@", "idle", 1.0, 1.0, -1, 63, 1.0, false, false, false);
                                        }
                                    }
                                }
                            }, 500);
                            handObjects.push({
                                player: entity,
                                obj: item
                            })
                        }
                    });
                }
                else{
                    for(let i = 0; i < handObjects.length; i++){
                        if(handObjects[i].player == entity){
                            if(handObjects[i].obj != null && mp.objects.exists(handObjects[i].obj)){
                                handObjects[i].obj.destroy();
                                handObjects.splice(i, 1);
                                player.stopAnimTask("anim@heists@box_carry@", "idle", 3.0);
                                break;
                            }
                        }
                    }
                }
            }
        })
    }
});