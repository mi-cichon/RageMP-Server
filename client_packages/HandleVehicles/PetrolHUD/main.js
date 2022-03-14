let petrol_browser = null;
let player = mp.players.local;
mp.events.add("petrol_openBrowser", (price, tank, fuel) => {
    if(petrol_browser != null && mp.browsers.exists(petrol_browser)){
        petrol_browser.destroy();
        petrol_browser = null;
    }
    petrol_browser = mp.browsers.new("package://HandleVehicles/PetrolHUD/index.html");
    petrol_browser.execute(`setVars(${price}, ${tank}, ${(fuel).toFixed(2)})`);
});

mp.events.add("petrol_closeBrowser", () => {
    if(petrol_browser){
        petrol_browser.destroy();
        petrol_browser = null;
    }
});

mp.events.add("petrol_setBrowserPos", (x, y) => {
    if(petrol_browser){
        petrol_browser.execute(`setPosition(${x}, ${y})`);
    }
});

mp.events.add("petrol_updateBrowser", (fuel, cost) => {
    if(petrol_browser){
        petrol_browser.execute(`updateVars(${fuel}, ${cost})`);
    }
});