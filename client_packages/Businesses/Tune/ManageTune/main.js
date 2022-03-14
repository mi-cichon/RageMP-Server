let manageTuneBusinessBrowser = null;
let businessId = null;
let player = mp.players.local;

mp.events.add("openManageTuneBusinessBrowser", (bId) => {
    if(manageTuneBusinessBrowser == null && !player.getVariable("gui")){
        manageTuneBusinessBrowser = mp.browsers.new("package://Businesses/Tune/ManageTune/index.html");
        mp.events.callRemote("setGui", true);
        mp.gui.cursor.show(true, true);
        businessId = bId;
        manageTuneBusinessBrowser.execute(`setJobStatus(${player.hasVariable("job") && player.getVariable("job") == "business-tune" ? true : false})`)
    }
});

mp.events.add("closeManageTuneBusinessBrowser", () => {
    if(manageTuneBusinessBrowser != null && mp.browsers.exists(manageTuneBusinessBrowser)){
        manageTuneBusinessBrowser.destroy();
        manageTuneBusinessBrowser = null;
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setGui", false);
    }
});

mp.events.add("requestAvailableWheels", () => {
    mp.events.callRemote("requestAvailableWheels");
});

mp.events.add("sendAvailableWheels", data => {
    if(manageTuneBusinessBrowser){
        manageTuneBusinessBrowser.execute(`setAvailableWheels('${data}')`);
    }
});

mp.events.add("createWheelsOrder", (currentType, currentId, currentAmount, currentPrice) => {
    mp.events.callRemote("createWheelsOrder", parseInt(businessId), parseInt(currentType), parseInt(currentId), parseInt(currentAmount), parseInt(currentPrice));
});

mp.events.add("requestOwnedWheels", () => {
    mp.events.callRemote("requestOwnedWheels", businessId);
});

mp.events.add("sendOwnedWheels", (data) => {
    if(manageTuneBusinessBrowser){
        manageTuneBusinessBrowser.execute(`setOwnedWheels('${data}')`);
    }
});

mp.events.add("requestShipmentWheels", () => {
    mp.events.callRemote("requestShipmentWheels", businessId);
});

mp.events.add("sendShipmentWheels", (data) => {
    if(manageTuneBusinessBrowser){
        manageTuneBusinessBrowser.execute(`setShipmentWheels('${data}')`);
    }
});

mp.events.add("requestManageData", () => {
    mp.events.callRemote("requestManageData", businessId);
});

mp.events.add("sendManageData", (date, price) => {
    if(manageTuneBusinessBrowser){
        manageTuneBusinessBrowser.execute(`setManageData('${date}', '${price}')`);
    }
});

mp.events.add("extendTuneTime", days => {
    mp.events.callRemote("extendTuneBusinessTime", businessId, parseInt(days));
});

mp.events.add("switchBusinessJob", () => {
    mp.events.callRemote("switchBusinessJob", businessId);
});

mp.events.add("closeBusiness", () => {
    mp.events.callRemote("closeBusiness", businessId);
});