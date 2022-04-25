let playtimeBonusBrowser = null;
let player = mp.players.local;
mp.events.add("openPlaytimeBonusBrowser", (data) => {
    playtimeBonusBrowser = mp.browsers.new('package://PlaytimeBonus/index.html');
});

mp.events.add("bonus_reward", (rewardId) => {
    if(playtimeBonusBrowser){
        playtimeBonusBrowser.destroy();
        playtimeBonusBrowser = null;
    }

    mp.events.callRemote("bonus_reward", rewardId);
});