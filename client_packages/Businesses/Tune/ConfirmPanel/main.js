let confirmWheelsTunePanel = null;
let businessId = null;
let player = mp.players.local;
let info = [];
let mechInfo = [];
mp.events.add("openConfirmWheelsTunePanel", (bId, type, id, name, price, state, partId) => {
    if(confirmWheelsTunePanel == null && !player.getVariable("gui")){
        confirmWheelsTunePanel = mp.browsers.new("package://Businesses/Tune/ConfirmPanel/index.html");
        mp.events.callRemote("setGui", true);
        mp.gui.cursor.show(true, true);
        businessId = bId;
        confirmWheelsTunePanel.execute(`setVars('${name}', '${price}', ${state}, 'wheels')`)
        info = [bId, type, id, name, price, state, partId];
    }
});

mp.events.add("openConfirmTunePanel", (bId, state, name, price, partId, offer) => {
    if(confirmWheelsTunePanel == null && !player.getVariable("gui")){
        confirmWheelsTunePanel = mp.browsers.new("package://Businesses/Tune/ConfirmPanel/index.html");
        mp.events.callRemote("setGui", true);
        mp.gui.cursor.show(true, true);
        businessId = bId;
        confirmWheelsTunePanel.execute(`setVars('${name}', '${price}', ${state}, 'mech')`)
        mechInfo = [bId, state, name, price, partId, offer];
    }
});

mp.events.add("closeConfirmWheelsTunePanel", () => {
    confirmWheelsTunePanel.destroy();
    confirmWheelsTunePanel = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("setGui", false);
});

mp.events.add("declineWheelsTuneOffer", () => {
    mp.events.callRemote("declineWheelsTuneOffer", info[0], info[1], info[2], info[4], info[5]);
});

mp.events.add("acceptWheelsTuneOffer", () => {
    mp.events.callRemote("acceptWheelsTuneOffer", info[0], info[1], info[2], info[4], info[5], info[6]);
});

mp.events.add("declineMechTuneOffer", () => {
    mp.events.callRemote("declineMechTuneOffer", parseInt(mechInfo[0]));
});

mp.events.add("acceptMechTuneOffer", () => {
    mp.events.callRemote("acceptMechTuneOffer", parseInt(mechInfo[0]), mechInfo[1] == true ? "1" : "0", mechInfo[2], parseInt(mechInfo[3]), parseInt(mechInfo[4]), parseInt(mechInfo[5]));
});