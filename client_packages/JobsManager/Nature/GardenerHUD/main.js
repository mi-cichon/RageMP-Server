let gardenerHUDBrowser = null;
let player = mp.players.local;
mp.events.add("openGardenerHUDBrowser", (data) => {
    if(mp.browsers.exists(gardenerHUDBrowser)){
        gardenerHUDBrowser.destroy();
    }
    gardenerHUDBrowser = mp.browsers.new('package://JobsManager/Nature/GardenerHUD/index.html');
    gardenerHUDBrowser.execute(`insertData('${data}');`);
    
});

mp.events.add("closeGardenerHUDBrowser", () => {
    if(gardenerHUDBrowser){
        gardenerHUDBrowser.destroy();
        gardenerHUDBrowser = null;
    }
});

mp.events.add("gardener_insertHUDData", data => {
    if(mp.browsers.exists(gardenerHUDBrowser)){
        gardenerHUDBrowser.execute(`insertData('${data}');`);
    }
});