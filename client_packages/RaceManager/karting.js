let player = mp.players.local;
let countdown = false;
let time = 3;
let startPosition = null;
let cpPosition = null;
let racing = false;
let raceTime = 0;
let raceInt = null;
let lap = 1;
let finalLap = 7;
let startCP = null;
let midCP = null;
mp.events.add('startKartingRace', (startP, cpP) => {
    startPosition = new mp.Vector3(startP.x, startP.y, startP.z-1);
    cpPosition = new mp.Vector3(cpP.x, cpP.y, cpP.z-1);
    player.vehicle.freezePosition(true);
    player.vehicle.setOnGroundProperly();
    countdown = true;
    time = 3;
    lap = 1;
    let startInt = setInterval(() => {
        time--;
        if(time == 0)
        {
            if(mp.vehicles.exists(player.vehicle))
            {
                mp.game.audio.playSoundFrontend(-1, "5_Second_Timer", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", true);
                player.vehicle.freezePosition(false);
                startRace();
            }
            
        }
        else if(time != -1){
            mp.game.audio.playSoundFrontend(-1, "5_SEC_WARNING", "HUD_MINI_GAME_SOUNDSET", true);
        }
        if(time == -1)
        {
            countdown = false;
            clearInterval(startInt);
        }
            
    }, 1000);
});

mp.events.add('render', () => {
    if(countdown)
    {
        let text = time == 0?"Start":time.toString();
        let pos = new Float64Array([0.49, 0.2]);
        mp.game.graphics.drawText(text, pos, { 
            font: 4, 
            color: [255, 255, 255, 255], 
            scale: [1.7, 1.7], 
            outline: true
        });
    }
    if(racing)
    {
        let text = (raceTime/1000).toString() + "s";
        let pos = new Float64Array([0.94, 0.96]);
        mp.game.graphics.drawText(text, pos, { 
            font: 4, 
            color: [255, 255, 255, 255], 
            scale: [0.7, 0.7], 
            outline: true
        });

        let laptext = lap.toString() + "/" + finalLap.toString();
        let poss = new Float64Array([0.89, 0.96]);
        mp.game.graphics.drawText(laptext, poss, { 
            font: 4, 
            color: [255, 255, 255, 255], 
            scale: [0.7, 0.7], 
            outline: true
        });
    }
});

function startRace(){
    startTimer(true);
    racing = true;
    midCP = mp.checkpoints.new(0, cpPosition, 2, 
    {
        color: [128, 128, 255, 255],
        visible: true
    });
}

mp.events.add("stopKartRace", () => {
    racing = false;
    startTimer(false);
});

function startTimer(bool){
    if(bool == true)
    {
        raceInt = setInterval(() => {
            raceTime += 10;
        }, 10);
    }
    if(bool == false)
    {
        clearInterval(raceInt);
        raceInt = null;
    }
    
}

mp.events.add("playerEnterCheckpoint", (checkpoint) => {
    if(checkpoint == midCP)
    {
        mp.game.audio.playSoundFrontend(-1, "5_SEC_WARNING", "HUD_MINI_GAME_SOUNDSET", true);
        midCP.destroy();
        startCP = mp.checkpoints.new(0, startPosition, 2, 
            {
                color: [128, 128, 255, 255],
                visible: true
            });
    }
    if(checkpoint == startCP)
    {
        if(lap < finalLap)
        {
            startCP.destroy();
            midCP = mp.checkpoints.new(0, cpPosition, 2, 
                {
                    color: [128, 128, 255, 255],
                    visible: true
                });
            lap++;
        }
        else if(lap==finalLap)
        {
            startTimer(false);
            let endTime = raceTime;
            racing = false;
            raceTime = 0;
            startCP.destroy();
            mp.events.callRemote("deleteKart", player.vehicle);
            mp.events.callRemote("endKartingRace", endTime/1000);
        }
    }
   });