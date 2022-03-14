let adminContextBrowser = null;
let player = mp.players.local;
mp.events.add("openAdminContextBrowser", (x, y) => {
    if(adminContextBrowser && mp.browsers.exists(adminContextBrowser)){
        adminContextBrowser.destroy();
    }
    adminContextBrowser = mp.browsers.new("package://AdminContext/index.html");
    adminContextBrowser.execute(`setPosition('${JSON.stringify([x, y])}')`);
});

mp.events.add("closeAdminContextBrowser", () => {
    if(adminContextBrowser){
        adminContextBrowser.destroy();
        adminContextBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});