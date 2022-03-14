let refinery_HUDBrowser = null;
let player = mp.players.local;
mp.events.add("refinery_openHUDBrowser", () => {
    if(mp.browsers.exists(refinery_HUDBrowser)){
        refinery_HUDBrowser.destroy();
    }
    refinery_HUDBrowser = mp.browsers.new('package://JobsManager/Social/RefineryHUD/index.html');    
    mp.gui.cursor.show(true, true);
});

mp.events.add("refinery_closeHUDBrowser", () => {
    if(refinery_HUDBrowser){
        refinery_HUDBrowser.destroy();
        refinery_HUDBrowser = null;
    }
});

mp.events.add("refinery_selectJobType", data => {
    mp.events.callRemote("refinery_selectJobType", data);
    mp.gui.cursor.show(false, false);
});

mp.events.add("refinery_selectJob", () => {
    if(refinery_HUDBrowser){
        mp.gui.cursor.show(true, true);
        refinery_HUDBrowser.execute(`selectJob()`);
    }
});

mp.events.add("refinery_refreshHUDValues", (data, tank) => {
    if(refinery_HUDBrowser){
        refinery_HUDBrowser.execute(`insertData('${data}', ${tank})`);
    }
});