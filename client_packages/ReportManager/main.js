let reportBrowser = null;
let player = mp.players.local;
mp.events.add("openReportBrowser", () => {
    if(reportBrowser){
        mp.events.callRemote("setGui", false);
        reportBrowser.destroy();
        reportBrowser = null;
        mp.gui.cursor.show(false, false);
    }
    else if(!mp.players.local.getVariable("gui")){
        mp.events.callRemote("setGui", true);
        reportBrowser = mp.browsers.new('package://ReportManager/index.html');
        mp.gui.cursor.show(true, true);
        mp.events.callRemote("requireReports");
    }
});

mp.events.add("insertReportsToBrowser", data => {
    let reports = JSON.parse(data);
    reports.forEach(report => { 
        reportBrowser.execute(`insertData('${report[0]}', '${report[1]}', '${report[2]}', '${report[3]}', '${report[4]}');`);
    });
})

mp.events.add("closeReportBrowser", () => {
    if(reportBrowser){
        mp.events.callRemote("setGui", false);
        reportBrowser.destroy();
        reportBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("selectReport", (id) => {
    mp.events.callRemote("markReportAsSolved", id);
    if(reportBrowser){
        mp.events.callRemote("setGui", false);
        reportBrowser.destroy();
        reportBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});

mp.events.add("removeReport", (id) => {
    mp.events.callRemote("removeReport", id);
    if(reportBrowser){
        mp.events.callRemote("setGui", false);
        reportBrowser.destroy();
        reportBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});