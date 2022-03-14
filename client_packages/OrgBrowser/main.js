let orgBrowser = null;
let player = mp.players.local;
mp.events.add("openOrgBrowser", (data) => {
    if(orgBrowser){
        mp.events.callRemote("setGui", false);
        orgBrowser.destroy();
        orgBrowser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        mp.events.callRemote("setGui", true);
        orgBrowser = mp.browsers.new('package://OrgBrowser/index.html');
        mp.gui.cursor.show(true, true);
        let dataJ = JSON.parse(data);
        dataJ.forEach(d => {
            orgBrowser.execute(`insertData('${d[0]}', '${d[1]}', '${d[2]}', '${d[3]}');`);
        })
        
    }
});

mp.events.add("closeOrgBrowser", () => {
    if(orgBrowser){
        mp.events.callRemote("setGui", false);
        orgBrowser.destroy();
        orgBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("orgBrowserError", (text) => {
    if(orgBrowser){
        orgBrowser.execute(`showError('${text}')`);
    }
});

mp.events.add("sendOrgRequest", orgId => {
    mp.events.callRemote("sendOrgRequest", parseInt(orgId));
});

mp.events.add("createOrg", (name, tag) => {
    mp.events.callRemote("createOrg", name, tag);
});

