let fisherManBrowser = null;
let houseId = null;

mp.events.add("openFisherManBrowser", (level, rodType) => {
    if(fisherManBrowser == null){
        fisherManBrowser = mp.browsers.new("package://JobsManager//Water//fisherManHUD/index.html");
        fisherManBrowser.execute(`startFishing(${level}, ${rodType})`);
    }
});

mp.events.add("closeFisherManBrowser", () => {
    fisherManBrowser.destroy();
    fisherManBrowser = null;
});

mp.events.add("fishermanEnd", state => {
    fisherManBrowser.destroy();
    fisherManBrowser = null;
    mp.events.call("fishGameEnd", state);
});