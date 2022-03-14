let licenceBrowser = null;
let player = mp.players.local;
mp.events.add("openLicenceBrowser", () => {
    if(licenceBrowser){
        licenceBrowser.destroy();
        licenceBrowser = null;
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setGui", false);
    }
    else if(!player.getVariable("gui")){
        licenceBrowser = mp.browsers.new("package://Licence/HUD/index.html");
        mp.gui.cursor.show(true, true);
        mp.events.callRemote("setGui", true);
    }
});

mp.events.add("closeLicenceBrowser", () => {
    if(licenceBrowser){
        licenceBrowser.destroy();
        licenceBrowser = null;
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setGui", false);
    }
});

mp.events.add("licenceCheckMoney", () => {
    mp.events.callRemote("licenceCheckMoney", 30);
});

mp.events.add("startLicenceTest", () => {
    if(licenceBrowser){
        licenceBrowser.execute("startTest()");
    }
});

mp.events.add("endLicenceTest", (points) => {
    mp.events.call("closeLicenceBrowser");
    if(points < 10){
        mp.events.call("showNotification", `Niestety, Twój wynik był negatywny (${points}/12)!`);
    }
    else{
        mp.events.call("showNotification", `Gratulacje! Pomyślnie ukończyłeś egzamin z wynikiem ${points}/12!`);
        mp.events.callRemote("licenceCompleted");
    }
});