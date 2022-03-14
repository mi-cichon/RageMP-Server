let nickNameBrowser = null;
let player = mp.players.local;

mp.events.add("openNicknameBrowser", (val) => {
    if(nickNameBrowser == null && !player.getVariable("gui")){
        nickNameBrowser = mp.browsers.new("package://ChangeNickname/index.html");
        mp.events.callRemote("setGui", true);
        if(val == true)
        {
            nickNameBrowser.execute("freeChange();");
        }
        mp.gui.cursor.show(true, true);
    }
});

mp.events.add("closeNicknameBrowser", () => {
    nickNameBrowser.destroy();
    nickNameBrowser = null;
    mp.gui.cursor.show(false, false);
    mp.events.callRemote("setGui", false);
});

mp.events.add("changeNickname", (nickname) => {
    mp.events.callRemote("changeNickname", nickname);
});


mp.events.add("callNicknameError", (message) => {
    if(nickNameBrowser != null){
        nickNameBrowser.execute(`showError('${message}');`);
    }
});

