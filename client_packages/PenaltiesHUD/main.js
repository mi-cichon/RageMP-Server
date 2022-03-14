let penaltiesBrowser = null;
let player = mp.players.local;
mp.events.add("openPenaltiesBrowser", (data) => {
    penaltiesBrowser = mp.browsers.new('package://PenaltiesHUD/index.html');
    mp.gui.cursor.show(true, true);
    let penalties = JSON.parse(data);
    penalties.forEach(penalty => {
        penaltiesBrowser.execute(`insertData('${penalty[0]}', '${penalty[1]}', '${penalty[2]}', '${penalty[3]}', '${penalty[4]}', '${penalty[5]}');`);
    })
});

mp.events.add("closePenaltiesBrowser", () => {
    if(penaltiesBrowser){
        mp.events.callRemote("setGui", false);
        penaltiesBrowser.destroy();
        penaltiesBrowser = null;
        mp.gui.cursor.show(false, false);
    }
});