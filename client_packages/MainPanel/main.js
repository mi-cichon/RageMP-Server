let mainPanel_browser = null;
let player = mp.players.local;
let closing = false;
let rollingBonus = false;
mp.events.add("mainPanel_openBrowser", () => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        if(!closing && !rollingBonus){
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
        mainPanel_browser.execute(`useEmojis(${player.hasVariable("settings_UseEmojis") ? player.getVariable('settings_UseEmojis').toString() : 'true'})`);
        player.getVariable("settings_WallpaperUrl") !== "" ? mainPanel_browser.execute(`setWallpaper('${player.getVariable('settings_WallpaperUrl')}')`) : null;
        mp.gui.cursor.show(true, true);
        mp.events.callRemote("setGui", true);
    }
});

mp.events.add("mainPanel_setBankingData", (bankingData, transfers) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`setBankingData('${bankingData}', '${transfers}')`);
    }
});

mp.events.add("mainPanel_requestBankingData", () => {
    mp.events.callRemote("mainPanel_requestBankingData");
});

mp.events.add("mainPanel_requestHourBonusInfo", () => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`setHourBonus(${player.getVariable("bonustime")})`)
    }
});

mp.events.add("mainPanel_requestStatsGeneralData", () => {
    mp.events.callRemote("mainPanel_requestStatsGeneralData");
});

mp.events.add("mainPanel_setStatsGeneralData", (generalData) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertStatsGeneralData('${generalData}')`);
    }
});

mp.events.add("mainPanel_requestStatsJobData", () => {
    mp.events.callRemote("mainPanel_requestStatsJobData");
});

mp.events.add("mainPanel_setStatsJobData", (jobData) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertStatsJobData('${jobData}')`);
    }
});

mp.events.add("mainPanel_requestStatsSkillData", () => {
    mp.events.callRemote("mainPanel_requestStatsSkillData");
});

mp.events.add("mainPanel_setStatsSkillsData", (skillsData) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertStatsSkillsData('${skillsData}', ${player.getVariable('skillpoints')})`);
    }
});


mp.events.add("mainPanel_setVehiclesData", (vehiclesData) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        let vehicles = JSON.parse(vehiclesData);
        let newVehicles = [];
        vehicles.forEach(vehicle => {
            newVehicles.push([
                vehicle[0],
                vehicle[1],
                vehicle[3],
                mp.game.vehicle.getVehicleClassFromName(parseInt(vehicle[2])) == 8 ? "motorcycle" : "car"
            ]);
        });
        mainPanel_browser.execute(`insertVehiclesData('${JSON.stringify(newVehicles)}')`);
    }
});

mp.events.add("mainPanel_requestVehiclesData", () => {
    mp.events.callRemote("mainPanel_requestVehiclesData");
});

mp.events.add("mainPanel_requestGeneralSettingsData", () => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertGeneralSettings('${player.getVariable('settings')}')`);
    }
});

mp.events.add("mainPanel_requestInterfaceSettingsData", () => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertInterfaceSettings('${player.getVariable('settings')}')`);
    }
});

mp.events.add("mainPanel_requestAuthSettingsData", () => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertAuthSettings('${player.getVariable('authcode')}')`);
    }
})

mp.events.add("mainPanel_requestVehicleData", id => {
    mp.events.callRemote('mainPanel_requestVehicleData', id);
});

mp.events.add('mainPanel_setVehicleData', (vehInfo, mechInfo, visuInfo) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`setVehicleData('${vehInfo}', '${mechInfo}', '${visuInfo}')`);
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


mp.events.add("messenger_requestConversationsData", () => {
    mp.events.callRemote("messenger_requestConversationsData");
});

mp.events.add("messenger_receiveConversationsData", (conversations) => {
    if(mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`setConversationsData('${conversations}')`);
    }
});

mp.events.add("messenger_requestMessageData", playerToId => {
    mp.events.callRemote("messenger_requestMessageData", playerToId);
});

mp.events.add("messenger_receiveMessageData", (data) => {
    if(mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`setMessageData('${data}')`);
    }
});

mp.events.add("messenger_sendMessage", (playerToId, text) => {
    mp.events.callRemote("messenger_sendMessage", playerToId, text);
});

mp.events.add("messenger_searchForPlayers", keyword => {
    mp.events.callRemote("messenger_searchForPlayers", keyword);
});

mp.events.add("messenger_receiveSearchedPlayers", players => {
    if(mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertPlayers('${players}')`);
    }
});

setInterval(()=>{
    mp.events.callRemote("messenger_checkNewMessages");
}, 5000);

mp.events.add("mainPanel_setBonus", state => {
    rollingBonus = state;
    mainPanel_browser.execute(`setHourBonusState(${state})`);
});

mp.events.add("mainPanel_bonusReward", (rewardId) => {
    mp.events.callRemote("bonus_reward", rewardId);
    mainPanel_browser.execute(`setHourBonusState(false)`);
    rollingBonus = false;
});

mp.events.add("mainPanel_requestMoneyTransfer", (target, amount, desc) => {
    mp.events.callRemote("mainPanel_requestMoneyTransfer", target, amount, desc);
});

mp.events.add("mainPanel_transferCompleted", () => {
    if(mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`transferCompleted()`);
    }
}); 

mp.events.add("mainPanel_requestProgressData", () => {
    mp.events.callRemote("mainPanel_requestProgressData");
});

mp.events.add("mainPanel_setProgressData", (jobData, nodesData) => {
    if(mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`setProgressData('${jobData}', '${nodesData}')`);
    }
}); 


setTimeout(() => {
    mp.console.logInfo(server_conf.version);
}, 5000);