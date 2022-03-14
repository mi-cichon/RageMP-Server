let gardenerOrdersBrowser = null;
let player = mp.players.local;
mp.events.add("openGardenerOrdersBrowser", (data) => {
    if(gardenerOrdersBrowser){
        mp.events.callRemote("setGui", false);
        gardenerOrdersBrowser.destroy();
        gardenerOrdersBrowser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        mp.events.callRemote("setGui", true);
        gardenerOrdersBrowser = mp.browsers.new('package://JobsManager/Nature/GardenerOrdersHUD/index.html');
        mp.gui.cursor.show(true, true);
        gardenerOrdersBrowser.execute(`insertData('${data}');`);
    }
});

mp.events.add("closeGardenerOrdersBrowser", () => {
    if(gardenerOrdersBrowser){
        mp.events.callRemote("setGui", false);
        gardenerOrdersBrowser.destroy();
        gardenerOrdersBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("gardener_insertData", data => {
    if(mp.browsers.exists(gardenerOrdersBrowser)){
        gardenerOrdersBrowser.execute(`insertData('${data}');`);
    }
});

mp.events.add("gardener_pickOrder", (id) => {
    mp.events.callRemote("gardener_selectOrder", id);
});

mp.events.add("gardener_refreshOrders", () => {
    mp.events.callRemote("gardener_refreshOrders");
});

mp.events.add("gardener_cancelOrder", () => {
    mp.events.callRemote("gardener_cancelOrder");
    if(gardenerOrdersBrowser){
        mp.events.callRemote("setGui", false);
        gardenerOrdersBrowser.destroy();
        gardenerOrdersBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});