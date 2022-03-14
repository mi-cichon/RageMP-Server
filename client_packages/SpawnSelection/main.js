let spawnSelectionBrowser = null;
let camera = null;
mp.events.add("openSpawnSelection", (hasHouse) => {
    mp.players.local.freezePosition(true);
    mp.players.local.setInvincible(true);
    camera = mp.cameras.new("spawncamera", new mp.Vector3(113.96158, -917.0692, 47.739582), new mp.Vector3(0,0, -125), 45);
    camera.setActive(true);
    mp.game.cam.renderScriptCams(true, false, 0, true, false);
    spawnSelectionBrowser = mp.browsers.new('package://SpawnSelection/index.html');
    if(hasHouse == false)
    {
        spawnSelectionBrowser.execute(`hasNotHouse();`);
    }
    mp.gui.cursor.show(true, true);
});


function closeBrowser()
{
    if(spawnSelectionBrowser){
        spawnSelectionBrowser.destroy(); 
        camera.setActive(false);
        mp.game.cam.renderScriptCams(false, false, 0, false, false);
        mp.gui.cursor.show(false, false);
        mp.players.local.freezePosition(false);
        mp.players.local.setInvincible(false);
        mp.events.callRemote("setGui", false);
    }

    
}

mp.events.add("spawnSelected", (spawn) => {
    mp.events.callRemote("spawnSelected", spawn);
    closeBrowser();
});
