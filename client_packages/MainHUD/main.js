let hudBrowser = null;
let player = mp.players.local;

mp.events.add("openMainHUD", (remoteid) => {
    hudBrowser = mp.browsers.new('package://MainHUD/index.html');
    hudBrowser.execute(`setId(${remoteid});`);
    if(player.hasVariable("power")){
        hudBrowser.execute(`setPlayersPower(${player.getVariable("power")})`);
    }
    hudBrowser.execute(`setScales(${player.getVariable("settings_HudSize")}, ${player.getVariable("settings_ChatSize")})`);
    hudBrowser.execute(`setAvatar('${player.getVariable("socialclub")}')`);
});

mp.events.add("closeMainHUD", () => {
    if(storageBrowser){
        storageBrowser.destroy();
    }
    mp.gui.cursor.show(false, false);
});

mp.events.add("settings_setHUDScales", (hudScale, chatScale) => {
    if(hudBrowser){
        hudBrowser.execute(`setScales(${hudScale}, ${chatScale})`);
    }
});

mp.events.add("updateMainHUD", (nickname, money, lvl, exp) => {
    if(hudBrowser){
        hudBrowser.execute(`UpdateInfo('${nickname}', '${money}', '${lvl}', '${exp}');`);
    }
});

mp.events.add("showNotification", (text) => {
    if(hudBrowser){
        hudBrowser.execute(`showNotification('${text}')`);
    }
});

mp.events.add("sendMessage", (id, nickname, text, type, usertype = "", time, socialclub) => {
    if(hudBrowser){
        hudBrowser.execute(`sendMessage('${id}', '${nickname}', '${text}', '${type}', '${usertype}', '${time}', '${socialclub}');`);
    }
});

mp.events.add("messageSent", (text) => {
    if(text[0] === "/"){
        text = text.substring(1);
        if(text.includes(' ')){
            let command = text.split(" ")[0];
            let args = text.replace(command + " ", "");
            mp.events.callRemote("playerCommandHandler", command, args);
        }
        else{
            let command = text;
            let args = [];
            mp.events.callRemote("playerCommandHandler", command, args);
        }
    }
    else{
        mp.events.callRemote("playerMessageHandler", text);
    }
});

mp.events.add("showChatInput", () => {
    if(hudBrowser){
        mp.events.callRemote("setPlayersControlsBlocked", true);
        hudBrowser.execute(`showChatInput();`);
        mp.gui.cursor.show(true, true);
    }
});

mp.events.add("showChatInputWithSlash", () => {
    if(hudBrowser){
        mp.events.callRemote("setPlayersControlsBlocked", true);
        hudBrowser.execute(`showChatInputWithSlash();`);
        mp.gui.cursor.show(true, true);
    }
});

mp.events.add("hideChatInput", () => {
    if(hudBrowser){
        hudBrowser.execute(`hideChatInput();`);
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setPlayersControlsBlocked", false);
    }
});

mp.events.add("setChatEnabled", state => {
    if(!state){
        mp.events.callRemote("setGui", false);
        mp.events.callRemote("setPlayersControlsBlocked", false);
    }
}); 

mp.events.add("logConsole", (data) =>{
    mp.console.logInfo(data);
});

mp.events.add("updateJobVars", (money, pp, pj) => {
    hudBrowser.execute(`updateJobVars(${money}, ${pp}, ${pj})`);
});

mp.events.add("toggleJobHUD", () => {
    hudBrowser.execute('switchJobHUD();');
});

mp.events.add("startJob", (name, type) => {
    hudBrowser.execute(`startJob('${name}', '${type}');`);
});

mp.events.add("stopJob", () => {
    hudBrowser.execute('stopJob();');
});

mp.events.add('terminateJob', () => {
    mp.events.callRemote('endJob');
});

mp.events.add("setTime", time => {
    if(hudBrowser != null){
        hudBrowser.execute(`setTime('${time}')`);
    }
});

mp.events.add("warnPlayer", () => {
    if(hudBrowser != null){
        hudBrowser.execute(`warn()`);
    }
});


let lastMessageId = -1;

mp.events.add("newMessageSound", (messageIds) => {
    if(hudBrowser != null){
        let messages = JSON.parse(messageIds);
        if(messages[messages.length-1] != lastMessageId){
            hudBrowser.execute(`notification()`);
            lastMessageId = messages[messages.length-1];
        }
    }
});

mp.events.add("messages_setNewMessages", (amount) => {
    if(hudBrowser != null){
        hudBrowser.execute(`setNewMessages(${amount})`);
    }
});

mp.events.add("setTexting", state => {
 mp.events.callRemote("setPlayerTexting", state);
});

mp.events.add("playmusic", () => {
    if(hudBrowser != null){
        hudBrowser.execute(`playmusic()`);
    }
});

mp.events.add("setMarketValues", (id, price, name, owner, desc, tune) => {
    if(hudBrowser != null){
        hudBrowser.execute(`setMarketValues('${id}','${price}','${name}','${owner}','${desc}','${tune}')`);
    }
});

mp.events.add("setMarketPosition", (x,y) => {
    if(hudBrowser != null){
        hudBrowser.execute(`setMarketPosition(${x}, ${y})`);
    }
});

mp.events.add("hideMarketInfo", () => {
    if(hudBrowser != null){
        hudBrowser.execute(`hideMarketInfo()`);
    }
});

mp.events.add("mainHUD_useEmojis", state => {
    if(hudBrowser != null){
        hudBrowser.execute(`useEmojis(${state})`);
    }  
});


mp.events.add("test_mugshot", () => {
    mp.gui.takeScreenshot("gielda.png", 1, 100, 0);
    if(hudBrowser != null){
        setTimeout(()=> {
            hudBrowser.execute(`screenShotData('screenshots://gielda.png')`);
        },3000);
    }
});

mp.events.add("carMugshot_send", (urls) => {
    let part1 = urls.slice(0, urls.length/4);
    let part2 = urls.slice(urls.length/4, urls.length/2);
    let part3 = urls.slice(urls.length/2, urls.length * 3/4);
    let part4 = urls.slice(urls.length * 3/4, urls.length);
    mp.events.callRemote("carMugshot_create", part1, part2, part3, part4);
});