let memberOrgBrowser = null;
let player = mp.players.local;
mp.events.add("openMemberOrgBrowser", (members, vehicles, vehiclesToShare, sharedVehicles) => {
    if(memberOrgBrowser){
        mp.events.callRemote("setGui", false);
        memberOrgBrowser.destroy();
        memberOrgBrowser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        mp.events.callRemote("setGui", true);
        memberOrgBrowser = mp.browsers.new('package://OrgBrowser/memberIndex.html');
        mp.gui.cursor.show(true, true);

        let dataJ = JSON.parse(members);
        dataJ.forEach(d => {
            memberOrgBrowser.execute(`insertMember('${d[0]}', '${d[1]}');`);
        })

        dataJ = JSON.parse(vehicles);
        dataJ.forEach(d => {
            memberOrgBrowser.execute(`insertVehicle('${d[0]}', '${d[1]}', '${d[2]}');`);
        })

        dataJ = JSON.parse(vehiclesToShare);
        dataJ.forEach(d => {
            memberOrgBrowser.execute(`insertVehiclesToShare('${d[0]}', '${d[1]}');`);
        })

        dataJ = JSON.parse(sharedVehicles);
        dataJ.forEach(d => {
            memberOrgBrowser.execute(`insertSharedVehicles('${d[0]}', '${d[1]}');`);
        })
    }
});

mp.events.add("closeMemberOrgBrowser", () => {
    if(memberOrgBrowser){
        mp.events.callRemote("setGui", false);
        memberOrgBrowser.destroy();
        memberOrgBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("refreshMemberOrgData", (members, vehicles, vehiclesToShare, sharedVehicles) => {
    if(memberOrgBrowser){
        memberOrgBrowser.execute(`resetData()`);
        let dataJ = JSON.parse(members);
        dataJ.forEach(d => {
            memberOrgBrowser.execute(`insertMember('${d[0]}', '${d[1]}');`);
        })

        dataJ = JSON.parse(vehicles);
        dataJ.forEach(d => {
            memberOrgBrowser.execute(`insertVehicle('${d[0]}', '${d[1]}', '${d[2]}');`);
        })

        dataJ = JSON.parse(vehiclesToShare);
        dataJ.forEach(d => {
            memberOrgBrowser.execute(`insertVehiclesToShare('${d[0]}', '${d[1]}');`);
        })

        dataJ = JSON.parse(sharedVehicles);
        dataJ.forEach(d => {
            memberOrgBrowser.execute(`insertSharedVehicles('${d[0]}', '${d[1]}');`);
        })
    }
});

mp.events.add("removeSharedVehicleMember", (ID) => {
    mp.events.callRemote("removeSharedVehicle", ID, "");
});

mp.events.add("shareVehicle", (ID) => {
    mp.events.callRemote("shareVehicle", ID, "");
});

mp.events.add("leaveOrg", () => {
    mp.events.callRemote("leaveOrg");
});

