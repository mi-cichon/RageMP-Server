let player = mp.players.local;
let junkObject = null;
let junkColshape = null;
let junkMarker = null;
let junkBlip = null;
let pedColshape = null;
let pedText = null;
let pedPosition = new mp.Vector3(2400.5923, 3125.3843, 48.153015);

mp.events.add("render", () => {

    if(mp.players.local.getVariable("job") === "junkyard")
    {
        if(pedText != null && getDistance(player.position, pedPosition) < 10){
            point = mp.game.graphics.world3dToScreen2d(new mp.Vector3(pedPosition.x, pedPosition.y, pedPosition.z + 1.6));
            if(point){
                let pos = new Float64Array([point.x,point.y]);
                mp.game.graphics.drawText(pedText, pos, { 
                    font: 4, 
                    color: [255, 255, 255, 255], 
                    scale: [0.5, 0.5], 
                    outline: true
                });
            }
        }
    }
});

function removeJunkItems(){
    if(junkObject != null){
        junkObject.destroy();
        junkObject = null;
    }
    if(junkColshape != null){
        junkColshape.destroy();
        junkColshape = null;
    }
    if(junkMarker != null){
        junkMarker.destroy();
        junkMarker = null;
    }
    if(junkBlip != null){
        junkBlip.destroy();
        junkBlip = null;
    }
    if(pedColshape != null){
        pedColshape.destroy();
        pedColshape = null;
    }
    pedText = null;
}

mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(entity == player && value == "junkyard"){
        createJunk();
    }
    else if(entity == player && oldvalue == "junkyard"){
        removeJunkItems();
    }
});

mp.events.add("playerEnterColshape", (colshape) => {
    if(junkColshape != null && junkColshape == colshape){
        mp.game.streaming.requestAnimDict("amb@prop_human_bum_bin@base");
        player.taskPlayAnim("amb@prop_human_bum_bin@base", "base", 1.0, 1.0, 2000, 2, 1.0, false, false, false);
        player.freezePosition(true);
        setTimeout(function(){

            removeJunkItems();

            player.freezePosition(false);
            mp.events.call("showNotification", "Zanieś przedmiot kierownikowi!");
            pedColshape = mp.colshapes.newTube(pedPosition.x ,pedPosition.y, pedPosition.z, 2, 2.0);

            junkMarker = mp.markers.new(0, subVectors(pedPosition, new mp.Vector3(0,0,-2)), 0.6, {
                bopUpAndDown: true,
                color: [0, 204, 153, 255]
            });

            junkBlip = mp.blips.new(8, pedPosition, {
                color: 15,
                name: "Kierownik złomowiska",
                scale: 0.8
            });
        }, 2000);
    }
    else if(pedColshape != null && pedColshape == colshape){
        mp.events.callRemote("junkDelievered");

        removeJunkItems();
        createJunk();
    }
});


function createJunk(){
    let object = objects[getRandomInt(0, objects.length)];
    let position = positions[getRandomInt(0, positions.length)];

    junkObject = mp.objects.new(object.object, subVectors(position, object.localPos), {
        rotation: object.localRot
    });

    junkColshape = mp.colshapes.newTube(position.x ,position.y, position.z, 1.2, 2.0);

    junkMarker = mp.markers.new(0, subVectors(position, new mp.Vector3(0,0,-1)), 0.6, {
        bopUpAndDown: true,
        color: [0, 204, 153, 255]
    });

    junkBlip = mp.blips.new(8, position, {
        color: 15,
        name: "Przedmiot do przyniesienia",
        scale: 0.8
    });

    pedText = object.text;
}

function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
}

function getDistance(vec1, vec2){
    return Math.sqrt(Math.pow(vec1.x - vec2.x, 2) + Math.pow(vec1.y - vec2.y, 2) + Math.pow(vec1.z - vec2.z, 2));
}

function subVectors(vec1, vec2){
    return new mp.Vector3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
}


// function subVectors(vec1, vec2){
//     return new mp.Vector3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
// }

let positions = [
    new mp.Vector3(2420.7393, 3095.3608, 48.152897),
    new mp.Vector3(2418.2961, 3052.8958, 48.1523),
    new mp.Vector3(2401.457, 3048.2158, 48.153145),
    new mp.Vector3(2376.5044, 3062.235, 48.152847),
    new mp.Vector3(2349.2107, 3070.9626, 48.152176),
    new mp.Vector3(2356.5947, 3033.6743, 48.152184),
    new mp.Vector3(2379.3936, 3028.3079, 48.227486),
    new mp.Vector3(2396.5122, 3090.9592, 48.15267),
    new mp.Vector3(2433.1106, 3116.503, 48.16659),
    new mp.Vector3(2404.0742, 3074.2454, 48.152897),
    new mp.Vector3(2406.6763, 3144.284, 48.155945),
    new mp.Vector3(2424.9148, 3155.0928, 48.184723),
    new mp.Vector3(2421.3137, 3125.044, 48.196724),
    new mp.Vector3(2425.6453, 3143.6929, 48.15142),
    new mp.Vector3(2400.0942, 3084.5344, 48.70945)
]

let objects = [
    {
        object: "hei_prop_hei_paper_bag",
        text: "Zgubiłem gdzieś śniadanie :( Poszukasz?",
        localPos: new mp.Vector3(0, 0, 0.9),
        localRot: new mp.Vector3(0, 0, 0)
    },
    {
        object: "prop_tool_screwdvr01",
        text: "Przynieś no śrubokręt!",
        localPos: new mp.Vector3(0, 0, 1),
        localRot: new mp.Vector3(0, 0, 0)
    },
    {
        object: "prop_tool_hardhat",
        text: "Te, podaj no kask!",
        localPos: new mp.Vector3(0, 0, 1),
        localRot: new mp.Vector3(0, 0, 0)
    },
    {
        object: "prop_tool_box_06",
        text: "Przynieś skrzynkę z narzędziami!",
        localPos: new mp.Vector3(0, 0, 1),
        localRot: new mp.Vector3(0, 0, 0)
    },
    {
        object: "prop_tool_consaw",
        text: "Staszek prosi o piłę!",
        localPos: new mp.Vector3(0, 0, 0.9),
        localRot: new mp.Vector3(0, 0, 0)
    },
    {
        object: "prop_tool_drill",
        text: "Wiertarkę gdzieś zgubiłem, podasz?",
        localPos: new mp.Vector3(0, 0, 0.85),
        localRot: new mp.Vector3(0, 0, 0)
    },
    {
        object: "prop_tool_mallet",
        text: "Jak ja mogłem zgubić taki wielki młot?",
        localPos: new mp.Vector3(0, 0, 1),
        localRot: new mp.Vector3(0, 0, 0)
    },
    {
        object: "prop_tool_jackham",
        text: "Gdzie ja podziałem ten młot pneumatyczny? D:",
        localPos: new mp.Vector3(0, 0, 0.95),
        localRot: new mp.Vector3(0, 0, 0)
    },
    {
        object: "prop_tool_spanner03",
        text: "Tam za rogiem zostawiłem klucz, przynieś!",
        localPos: new mp.Vector3(0, 0, 1),
        localRot: new mp.Vector3(0, 0, 0)
    },
    {
        object: "prop_tool_hammer",
        text: "Gdzie ja zostawiłem ten młotek?",
        localPos: new mp.Vector3(0, 0, 1),
        localRot: new mp.Vector3(0, 0, 0)
    }
]