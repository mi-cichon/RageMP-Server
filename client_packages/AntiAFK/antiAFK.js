let player = mp.players.local;

let afkTimeout = setTimeout(markAsAFK, 300000);

mp.keys.bind(0x57, true, ()=>{ //W
    markAsNotAFK();
});
mp.keys.bind(0x41, true, ()=>{ //A
    markAsNotAFK();
});
mp.keys.bind(0x53, true, ()=>{ //S
    markAsNotAFK();
});
mp.keys.bind(0x44, true, ()=>{ //D
    markAsNotAFK();
});

function markAsAFK(){
    if(!(player.hasVariable("afk") && player.getVariable("afk"))){
        mp.events.callRemote("afk_setAFK", true);
    }
    afkTimeout = null;
}


function markAsNotAFK(){
    if(player.hasVariable("afk") && player.getVariable("afk")){
        mp.events.callRemote("afk_setAFK", false);
    }
    if(afkTimeout != null){
        clearTimeout(afkTimeout);
    }
    afkTimeout = setTimeout(markAsAFK, 300000);
}