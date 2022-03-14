let bankingBrowser = null;
let player = mp.players.local;
mp.events.add("openBankingBrowser", (money) => {
    if(bankingBrowser != null && mp.browsers.exists(bankingBrowser)){
        bankingBrowser.destroy();
        bankingBrowser = null;
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setGui", false);
    }
    else if(!(player.hasVariable("gui") && player.getVariable("gui"))){
        bankingBrowser = mp.browsers.new("package://Banking/index.html");
        mp.gui.cursor.show(true, true);
        mp.events.callRemote("setGui", true);
        bankingBrowser.execute(`setVars('${money}', '${player.getVariable('socialclub')}')`);
    }
});

mp.events.add("closeBankingBrowser", () => {
    if(bankingBrowser){
        bankingBrowser.destroy();
        bankingBrowser = null;
        mp.events.callRemote("setGui", false);
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("withdrawBankMoney", (money) => {
    mp.events.callRemote("withdrawBankMoney", money);
});

mp.events.add("depositBankMoney", (money) => {
    mp.events.callRemote("depositBankMoney", money);
});

mp.events.add("setBankingVars", (money) => {
    bankingBrowser.execute(`setVars('${money}', '${player.getVariable('socialclub')}')`);
});