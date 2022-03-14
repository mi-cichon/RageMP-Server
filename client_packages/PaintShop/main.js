let paintShopBrowser = null;
let vehicle = null;
let showing = false;
let player = mp.players.local;
mp.events.add("openPaintShopBrowser", (veh) => {
    if(!player.getVariable("gui")){
        paintShopBrowser = mp.browsers.new('package://PaintShop/index.html');
        veh.freezePosition(true);
        mp.gui.cursor.show(true, true);
        vehicle = veh;
        mp.events.callRemote("setGui", true);
    }
    
});

mp.events.add("showVehicleColor", (color1, color2) => {
    if(!showing){
        var currentMods = JSON.stringify([vehicle.getVariable("color1mod"), vehicle.getVariable("color2mod")]);
        showing = true;
        mp.events.callRemote("showVehicleColor", vehicle, color1, color2, "");
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setGui", false);
        setTimeout(function(){
            mp.events.callRemote("setGui", true);
                mp.events.callRemote("showVehicleColor", vehicle, "", "", currentMods);
                mp.gui.cursor.show(true, true);
                showing = false;
            },3000);
    }
});

mp.events.add("chooseVehicleColor", (color1, color2) => {
    mp.events.callRemote("changeVehicleColor", vehicle, color1, color2);
});

mp.events.add("closePaintShopBrowser", () => {
    if(paintShopBrowser){
        paintShopBrowser.destroy(); 
    }
    vehicle.freezePosition(false);
    mp.events.callRemote("setGui", false);
    mp.gui.cursor.show(false, false);
});