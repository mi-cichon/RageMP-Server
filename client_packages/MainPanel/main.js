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

mp.events.add("mainPanel_setPlayerData", (playersData, skillsData, jobInfo) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertPlayerData('${playersData}', '${skillsData}', '${jobInfo}')`);
    }
});

mp.events.add("mainPanel_requestPlayerData", () => {
    mp.events.callRemote("mainPanel_requestPlayerData");
});

mp.events.add("mainPanel_setVehiclesData", (vehiclesData) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertVehicles('${vehiclesData}')`);
    }
});

mp.events.add("mainPanel_requestVehiclesData", () => {
    mp.events.callRemote("mainPanel_requestVehiclesData");
});

mp.events.add("mainPanel_setSettings", (settingsData, authcode) => {
    if(mainPanel_browser != null && mp.browsers.exists(mainPanel_browser)){
        mainPanel_browser.execute(`insertSettings('${settingsData}', '${authcode}')`);
    }
});

mp.events.add("mainPanel_requestSettingsData", () => {
    mp.events.callRemote("mainPanel_requestSettingsData");
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