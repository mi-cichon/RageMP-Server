let speedoColorBrowser = null;
let vehicle = null;
let showing = false;
let player = mp.players.local;
mp.events.add("speedoColor_openBrowser", (veh) => {
    if(!player.getVariable("gui")){
        speedoColorBrowser = mp.browsers.new('package://SpeedometerColor/index.html');
        veh.freezePosition(true);
        mp.gui.cursor.show(true, true);
        vehicle = veh;
        mp.events.callRemote("setGui", true);
    }
    
});

mp.events.add("speedoColor_preview", color => {
    mp.events.call("speedometer_setColor", color);
});

mp.events.add("speedoColor_confirm", color => {
    mp.events.callRemote("speedoColor_confirm", vehicle, color);
});

mp.events.add("speedoColor_closeBrowser", () => {
    if(speedoColorBrowser){
        speedoColorBrowser.destroy(); 
    }
    vehicle.freezePosition(false);
    mp.events.callRemote("setGui", false);
    mp.gui.cursor.show(false, false);
});