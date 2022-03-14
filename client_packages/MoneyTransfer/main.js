let moneyTransferBrowser = null;
let player = mp.players.local;
let target;
mp.events.add("openMoneyTransferBrowser", (t) => {
    setTimeout(function(){
        if(moneyTransferBrowser != null && mp.browsers.exists(moneyTransferBrowser)){
            moneyTransferBrowser.destroy();
            moneyTransferBrowser = null;
            mp.gui.cursor.show(false, false);
            mp.events.callRemote("setGui", false);
        }
        else if(!player.getVariable("gui")){
            target = t;
            moneyTransferBrowser = mp.browsers.new("package://MoneyTransfer/index.html");
            mp.gui.cursor.show(true, true);
            mp.events.callRemote("setGui", true);
            moneyTransferBrowser.execute(`setVars('${target.getVariable('username')}')`);
        }
    }, 500);
});

mp.events.add("closeMoneyTransferBrowser", () => {
    if(moneyTransferBrowser){
        moneyTransferBrowser.destroy();
        moneyTransferBrowser = null;
        mp.events.callRemote("setGui", false);
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("transferMoneyToPlayer", (money, title) => {
    mp.events.callRemote("transferMoneyToPlayer", target, money, title);
});