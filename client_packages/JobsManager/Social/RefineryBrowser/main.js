let refinery_browser = null;
let player = mp.players.local;
let vehicle = null;
let pumpIndex = 0;
mp.events.add("refinery_openBrowser", (tank, veh, index) => {
    if(refinery_browser){
        mp.events.callRemote("setGui", false);
        refinery_browser.destroy();
        refinery_browser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        pumpIndex = index;
        vehicle = veh;
        mp.events.callRemote("setGui", true);
        refinery_browser = mp.browsers.new('package://JobsManager/Social/RefineryBrowser/index.html');
        mp.gui.cursor.show(true, true);
        refinery_browser.execute(`updateVehiclesTank(${tank})`);
    }
});

mp.events.add("refinery_closeBrowser", () => {
    if(refinery_browser){
        mp.events.callRemote("setGui", false);
        refinery_browser.destroy();
        refinery_browser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("refinery_addOil", amount => {
    mp.events.callRemote("refinery_addOil", vehicle, pumpIndex, amount);
});

mp.events.add("refinery_refreshTank", tank => {
    if(refinery_browser){
        refinery_browser.execute(`updateVehiclesTank(${tank})`);
    }
});