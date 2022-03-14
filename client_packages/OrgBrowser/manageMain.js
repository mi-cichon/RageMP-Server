let manageOrgBrowser = null;
let player = mp.players.local;
mp.events.add("openManageOrgBrowser", (members, vehicles, memberRequests, vehicleRequests, vehiclesToShare) => {
    if(manageOrgBrowser){
        mp.events.callRemote("setGui", false);
        manageOrgBrowser.destroy();
        manageOrgBrowser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        mp.events.callRemote("setGui", true);
        manageOrgBrowser = mp.browsers.new('package://OrgBrowser/manageIndex.html');
        mp.gui.cursor.show(true, true);
        let dataJ = JSON.parse(members);
        dataJ.forEach(d => {
            let self = player.getVariable("sid").toString() == d[0];
            manageOrgBrowser.execute(`insertMember('${d[0]}', '${d[1]}', ${self});`);
        })

        dataJ = JSON.parse(vehicles);
        dataJ.forEach(d => {
            manageOrgBrowser.execute(`insertVehicle('${d[0]}', '${d[1]}', '${d[2]}');`);
        })

        dataJ = JSON.parse(memberRequests);
        dataJ.forEach(d => {
            manageOrgBrowser.execute(`insertMemberRequest('${d[0]}', '${d[1]}', '${d[2]}');`);
        })

        dataJ = JSON.parse(vehicleRequests);
        dataJ.forEach(d => {
            manageOrgBrowser.execute(`insertVehicleRequest('${d[0]}', '${d[1]}', '${d[2]}');`);
        })

        dataJ = JSON.parse(vehiclesToShare);
        dataJ.forEach(d => {
            manageOrgBrowser.execute(`insertVehiclesToShare('${d[0]}', '${d[1]}');`);
        })
    }
});

mp.events.add("closeManageOrgBrowser", () => {
    if(manageOrgBrowser){
        mp.events.callRemote("setGui", false);
        manageOrgBrowser.destroy();
        manageOrgBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("answerMemberRequest", (ID, state) => {
    mp.events.callRemote("answerMemberRequest", ID, state);
});

mp.events.add("answerVehicleRequest", (ID, state) => {
    mp.events.callRemote("answerVehicleRequest", ID, state);
});

mp.events.add("removeOrg", () => {
    mp.events.callRemote("removeOrg");
});

mp.events.add("removeSharedVehicle", (id) => {
    mp.events.callRemote("removeSharedVehicle", id, "manage");
});

mp.events.add("shareManageVehicle", (ID) => {
    mp.events.callRemote("shareVehicle", ID, "manage");
});

mp.events.add("kickMemberFromOrg", (ID) => {
    mp.events.callRemote("kickMemberFromOrg", ID);
});

mp.events.add("refreshManageOrgData", (members, vehicles, memberRequests, vehicleRequests, vehiclesToShare) => {
    if(manageOrgBrowser){
        manageOrgBrowser.execute(`resetData()`);
        let dataJ = JSON.parse(members);
        dataJ.forEach(d => {
            let self = player.getVariable("sid").toString() == d[0];
            manageOrgBrowser.execute(`insertMember('${d[0]}', '${d[1]}');`);
        })

        dataJ = JSON.parse(vehicles);
        dataJ.forEach(d => {
            manageOrgBrowser.execute(`insertVehicle('${d[0]}', '${d[1]}', '${d[2]}');`);
        })

        dataJ = JSON.parse(memberRequests);
        dataJ.forEach(d => {
            manageOrgBrowser.execute(`insertMemberRequest('${d[0]}', '${d[1]}', '${d[2]}');`);
        })

        dataJ = JSON.parse(vehicleRequests);
        dataJ.forEach(d => {
            manageOrgBrowser.execute(`insertVehicleRequest('${d[0]}', '${d[1]}', '${d[2]}');`);
        })

        dataJ = JSON.parse(vehiclesToShare);
        dataJ.forEach(d => {
            manageOrgBrowser.execute(`insertVehiclesToShare('${d[0]}', '${d[1]}');`);
        })
    }
});