let mainPanel_browser = null;
let player = mp.players.local;
let closing = false;
mp.events.add("mainPanel_openBrowser", () => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        if(!closing){
            closing = true;
            mainPanel_browser.execute('closeTablet()');
            setTimeout(function(){
                mainPanel_browser.destroy();
                mainPanel_browser = null;
                mp.gui.cursor.show(false, false);
                mp.events.callRemote("setGui", false);
                closing = false;
            }, 1100);
        }
    }
    else if(!player.getVariable("gui")){
        mainPanel_browser = mp.browsers.new("package://MainPanel/index.html");
        mp.events.callRemote('mainPanel_requestData');
        mp.gui.cursor.show(true, true);
        mp.events.callRemote("setGui", true);
    }
});


mp.events.add("mainPanel_setData", (playersData, skillsData, vehiclesData, settingsData, time) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertVehicles('${vehiclesData}')`);
        mainPanel_browser.execute(`insertPlayerData('${playersData}', ${skillsData})`);
        mainPanel_browser.execute(`insertSettings('${settingsData}')`);
        mainPanel_browser.execute(`setTime('${time}')`);
    }
});

mp.events.add("mainPanel_requestVehicleData", id => {
    mp.events.callRemote('mainPanel_requestVehicleData', id);
});

mp.events.add('mainPanel_setVehicleData', (vehInfo, mechInfo, visuInfo) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`setVehiclesData('${vehInfo}', '${mechInfo}', '${visuInfo}')`);
    }
});

mp.events.add("mainPanel_setSkillsToUpgrade", (sp, skills) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`setSkillPoints(${sp}, '${skills}')`);
    }
});

mp.events.add("mainPanel_addPoint", index => {
    mp.events.callRemote("mainPanel_addSkillPoint", index);
});

mp.events.add('mainPanel_saveSettings', settings => {
    mp.events.callRemote("savePlayerSettings", settings);
});

mp.events.add("setTime", time => {
    if(mainPanel_browser != null){
        mainPanel_browser.execute(`setTime('${time}')`);
    }
});