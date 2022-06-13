let onlinePlayersBrowser = null;
let player = mp.players.local;
mp.events.add("openOnlinePlayersBrowser", () => {
    if(onlinePlayersBrowser && mp.browsers.exists(onlinePlayersBrowser)){
        onlinePlayersBrowser.destroy();
        onlinePlayersBrowser = null;
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setGui", false);
    }
    else if(!player.getVariable("gui"))
    {
        mp.events.callRemote("setGui", true);
        onlinePlayersBrowser = mp.browsers.new('package://OnlinePanel/index.html');
        mp.gui.cursor.show(true, true);
        let type = "";
        let color = "";
        let players = [];
        mp.players.forEach((player) =>{ 
            switch(player.getVariable('type'))
            {
                case "owner":
                    type = "Rcon";
                    color = [255, 0, 0, 255];
                    break;
                case "admin":
                    type = "Admin";
                    color = [196, 49, 36, 255];
                    break;
                case "jadmin":
                    type = "J.Admin";
                    color = [245, 102, 0, 255];
                    break;
                case "smod":
                    type = "S.Mod";
                    color = [0, 99, 28, 255];
                    break;
                case "mod":
                    type = "Mod";
                    color = [0, 153, 43, 255];
                    break;
                case "jmod":
                    type = "J.Mod";
                    color = [0, 250, 70, 255];
                    break;
                case "tester":
                    type = "Tester";
                    color = [0, 170, 255, 255];
                    break;
                default:
                    type = "";
                    color = [255, 255, 255, 255];
                    break;
            }
            if(players.length == 0 && player.hasVariable("username")){
                players.push({
                    "id": player.remoteId,
                    "name": player.getVariable("username"),
                    "pp": player.getVariable("level"),
                    "type": type,
                    "ping": player.getVariable("ping"),
                    "org" : player.hasVariable("orgName") ? player.getVariable("orgName") : "",
                    "color" : color
                });
            }
            else if(player.hasVariable("username")){
                let pushed = false;
                players.forEach(pl => {
                    if(pl["id"] > player.remoteId && !pushed){
                        players.splice(players.indexOf(pl), 0, {
                            "id": player.remoteId,
                            "name": player.getVariable("username"),
                            "pp": player.getVariable("level"),
                            "type": type,
                            "ping": player.getVariable("ping"),
                            "org" : player.hasVariable("orgName") ? player.getVariable("orgName") : "",
                            "color" : color
                        });
                        pushed = true;
                    }
                });
                if(!pushed && player.hasVariable("username")){
                    players.push({
                        "id": player.remoteId,
                        "name": player.getVariable("username"),
                        "pp": player.getVariable("level"),
                        "type": type,
                        "ping": player.getVariable("ping"),
                        "org" : player.hasVariable("orgName") ? player.getVariable("orgName") : "",
                        "color" : color
                    });
                }
            }
        });
        if(players.length > 0){
            players.forEach(player => {
                onlinePlayersBrowser.execute(`insertData('${player["id"]}', '${player['name']}', '${player['ping']}', '${player["type"]}', '${player['pp']}', '${player['org']}', '${JSON.stringify(player['color'])}');`);
            });
        }
        onlinePlayersBrowser.execute(`setOnline(${players.length})`);
    }
});

mp.events.add("closeOnlinePlayersBrowser", () => {
    if(onlinePlayersBrowser){
        onlinePlayersBrowser.destroy();
        onlinePlayersBrowser = null;
        mp.gui.cursor.show(false, false);
        mp.events.callRemote("setGui", false);
    }
});