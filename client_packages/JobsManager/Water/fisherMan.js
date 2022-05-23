let player = mp.players.local;
let rodObject = null;
let fishSize = 0;
let fishingTimeout = null;
let fishing = false;
var fishingColshapes = [];
let spotType = null;
mp.events.add("render", () => {
});
mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(entity.type === "player" && entity == player)
    {
        if(value == "fisherman"){
            rodObject = mp.objects.new("prop_fishing_rod_01", player.position);
            setTimeout(() => {
                rodObject.attachTo(player.handle, player.getBoneIndex(18905), 0.12, 0.1, 0, -120, -80, 0, true, false, false, false, 0, true);
                rodObject.setAlpha(255);
                mp.events.callRemote("putItemInHand", "fishingrod");
            }, 500);
        }
        if(oldvalue == "fisherman"){
            fishSize = 0;
            rodObject.destroy();
            mp.events.callRemote("removeItemFromHand");
            rodObject = null;
            player.stopAnimTask("amb@world_human_stand_fishing@idle_a", "idle_a", 3.0);
            player.freezePosition(false);
            if(fishingTimeout != null){
                clearTimeout(fishingTimeout);
                fishingTimeout = null;
            }
            fishing = false;
        }
    }
});

function setNewFish(){
    fishSize = getRandomInt(1, 6);
    mp.events.call("showNotification", "Rozpoczęto połów!");
    player.freezePosition(true);
    let timeMult = 1;
    if(player.getVariable("jobBonus_68")){
        timeMult = 0.8;
    }
    else if(player.getVariable("jobBonus_67")){
        timeMult = 0.9;
    }
    let time = parseInt(getRandomInt(15, 60) * timeMult);
    setTimeout(function(){
        mp.game.streaming.requestAnimDict("amb@world_human_stand_fishing@idle_a");
        player.taskPlayAnim("amb@world_human_stand_fishing@idle_a", "idle_a", 1.0, 1.0, -1, 1, 1.0, false, false, false);
    },500)
    fishingTimeout = setTimeout(() => {
        mp.events.call("showNotification", "Masz branie!");
        mp.game.audio.playSoundFrontend(-1, "SELECT", "HUD_MINI_GAME_SOUNDSET", true);
        mp.events.call("openFisherManBrowser", fishSize, player.getVariable("fisherman_rodType"));
        fishing = true;
    }, time * 1000);
}

function getRandomInt(min, max){
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
}

mp.events.add("fishGameEnd", state => {
    fishing = false;
    player.stopAnimTask("amb@world_human_stand_fishing@idle_a", "idle_a", 3.0);
    player.freezePosition(false);
    if(state){
        mp.events.callRemote("fishingDone", fishSize, spotType);
        fishSize = 0;
        spotType = null;
    }
    else{
        mp.events.call("showNotification", "Połów się nie powiódł! Naciśnij E aby zacząć ponownie!");
        fishSize = 0;
        spotType = null;
    }
});

mp.events.add("startFishing", (spot) => {
    if(spot = "ocean" && !player.getVariable("jobBonus_69")){
        mp.events.call("showNotification", "Nie masz uprawnień do połowu w oceanach!");
    }
    else{
        if(!fishing){
            if(fishSize == 0){
                spotType = spot;
                setNewFish();
            }else{
                mp.events.call("showNotification", "Połów przerwany!");
                if(fishingTimeout != null){
                    clearTimeout(fishingTimeout);
                    fishingTimeout = null;
                }
                fishSize = 0;
                player.freezePosition(false);
                player.stopAnimTask("amb@world_human_stand_fishing@idle_a", "idle_a", 3.0);
            }
        }
    }
});