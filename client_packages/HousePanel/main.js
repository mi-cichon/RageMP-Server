let housePanelBrowser = null;
let houseId = null;
let player = mp.players.local;

mp.events.add("openHousePanelBrowser", (houseid, houseprice) => {
    if(housePanelBrowser == null && !player.getVariable("gui")){
        housePanelBrowser = mp.browsers.new("package://HousePanel/index.html");
        mp.events.callRemote("setGui", true);
        housePanelBrowser.execute(`setPrice('${houseprice}');`);
        houseId = houseid;
        mp.gui.cursor.show(true, true);
    }
});

mp.events.add("openOwnHousePanelBrowser", (houseid, housetime, houseprice) => {
    if(housePanelBrowser == null && !player.getVariable("gui")){
        housePanelBrowser = mp.browsers.new("package://HousePanel/index2.html");
        housePanelBrowser.execute(`setPrice('${houseprice}');`);
        housePanelBrowser.execute(`changeDate('${housetime}');`);
        houseId = houseid;
        mp.gui.cursor.show(true, true);
        mp.events.callRemote("setGui", true);
    }
});

mp.events.add("closeHousePanelBrowser", () => {
    housePanelBrowser.destroy();
    housePanelBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("setGui", false);
});

mp.events.add("confirmHouseBuy", (days) => {
    mp.events.callRemote("confirmHouseBuy", houseId, days);
});

mp.events.add("confirmHouseExtend", (days) => {
    mp.events.callRemote("confirmHouseExtend", houseId, days);
})

mp.events.add("callHousePanelError", (message) => {
    if(housePanelBrowser != null){
        housePanelBrowser.execute(`showError('${message}');`);
    }
});

mp.events.add("updateHousePanelTime", (time) => {
    if(housePanelBrowser != null){
        housePanelBrowser.execute(`changeDate('${time}');`);
    }
});

mp.events.add("giveHouseUp", () => {
    mp.events.callRemote("giveHouseUp", houseId);
})
