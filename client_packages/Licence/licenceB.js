let player = mp.players.local;
let lVeh = null;
let obj = 627123000;
let mistakes = 0;
let maxMistakes = 5;
let collisionTimeout = null;
let speedingTimeout = null;
let noCollisionInterval = null;
let currentCP = {
    data: null,
    cp: null,
    left: false,
    blip: null,
    label: null
};
let insideCP = false;
let stoppedInterval = null;
let damageDone = false;
mp.events.addDataHandler("job", (entity, value, oldvalue) => {
    if(entity == player && value == "licenceB" && player.vehicle){
        mistakes = 0;
        damageDone = false;
        lVeh = player.vehicle;
        let checkpoint = checkpoints[1];
        let next = checkpoints[2].position;
        currentCP.cp = mp.checkpoints.new(1, new mp.Vector3(checkpoint.position.x, checkpoint.position.y, checkpoint.position.z - 1), 2,{
            direction: new mp.Vector3(next.x, next.y, next.z),
            color: [0, 204, 153, 100],
            visible: true,
            dimension: 0
        });
        currentCP.blip = mp.blips.new(8, checkpoint.position,{
            name: 'Kierunek egzaminu',
            color: 15,
            shortRange: false,
            scale: 0.8
        });
        currentCP.blip.setRoute(true);
        currentCP.data = checkpoint;
        let cpPos = new mp.Vector3(currentCP.data.position.x, currentCP.data.position.y, currentCP.data.position.z + 0.6);
        currentCP.label = mp.labels.new("STOP", cpPos,
        {
            los: true,
            font: 2,
            drawDistance: 30,
            color: [255,0,0,255]
        });
    }
});

mp.events.add("render", () => {
    if(lVeh != null && lVeh.doesExist()){
        mp.game.controls.disableControlAction(32, 75, true);
        if(currentCP != null && currentCP.data != null){
            let text = currentCP.data.text;
            let pos = new Float64Array([0.5, 0.95]);
            mp.game.graphics.drawText(text, pos,{ 
                font: 0, 
                color: [255, 253, 141, 255], 
                scale: [0.5, 0.5], 
                outline: true
            });
        }
    }
    if(lVeh != null && lVeh.doesExist() && lVeh.hasCollidedWithAnything() && lVeh.getLastMaterialHitBy() == obj && collisionTimeout == null){
        collisionTimeout = setTimeout(function(){
            collisionTimeout = null;
        }, 5000);
        mistakes++;
        mp.events.call("showNotification", `Uderzyłeś w słupek! (${mistakes}/${maxMistakes})`);
        if(mistakes >= maxMistakes){
            mp.events.call("showNotification", `Przekroczyłeś dozwoloną ilość błędów! Egzamin zakończony niepowodzeniem.`);
            mp.events.callRemote("licenceBfailed");
            destroyCurrentCP();
        }
    }
    if(lVeh != null && lVeh.doesExist() && lVeh.getSpeed() * 3.6 > 72 && speedingTimeout == null){
        speedingTimeout = setTimeout(function(){
            speedingTimeout = null;
        }, 10000);
    
        mistakes++;
        mp.events.call("showNotification", `Przekroczyłeś 70km/h! (${mistakes}/${maxMistakes})`);
        if(mistakes >= maxMistakes){
            mp.events.call("showNotification", `Przekroczyłeś dozwoloną ilość błędów! Egzamin zakończony niepowodzeniem.`);
            mp.events.callRemote("licenceBfailed");
            destroyCurrentCP();
        }
    }
    if(lVeh != null && lVeh.doesExist() && lVeh.getEngineHealth() < 900 && !damageDone){
        damageDone = true;
        mp.events.call("showNotification", `Zbyt mocno uszkodziłeś pojazd! Egzamin zakończony niepowodzeniem.`);
            mp.events.callRemote("licenceBfailed");
            destroyCurrentCP();
    }
});


mp.events.add("playerEnterCheckpoint", (cp) => {
    if(currentCP.cp != null && cp == currentCP.cp){
        if(currentCP.data.type != "stop"){
            if(checkpoints.indexOf(currentCP.data) == checkpoints.length - 1){
                mp.events.callRemote("licenceBpassed");
                destroyCurrentCP();
            }
            else{
                setNewCheckpoint();
            }
        }
        else{
            stoppedInterval = setInterval(function(){
                if(player.vehicle.getSpeed() < 1 && !currentCP.left){
                    if(checkpoints.indexOf(currentCP.data) == 5){
                        mp.events.callRemote("playerLeftPassingArea");
                    }
                    clearInterval(stoppedInterval);
                    stoppedInterval = null;
                    if(checkpoints.indexOf(currentCP.data) == checkpoints.length - 1){
                        mp.events.callRemote("licenceBpassed");
                        destroyCurrentCP();
                    }
                    else{
                        setNewCheckpoint();
                    }
                }
                else if(currentCP.left){
                    clearInterval(stoppedInterval);
                    if(checkpoints.indexOf(currentCP.data) == 5){
                        mp.events.callRemote("playerLeftPassingArea");
                    }
                    mistakes++;
                    mp.events.call("showNotification", `Nie zatrzymałeś się! (${mistakes}/${maxMistakes})`);
                    if(mistakes < maxMistakes){
                        stoppedInterval = null;
                        setNewCheckpoint();
                    }
                    else{
                        mp.events.call("showNotification", `Przekroczyłeś dozwoloną ilość błędów! Egzamin zakończony niepowodzeniem.`);
                        mp.events.callRemote("licenceBfailed");
                        destroyCurrentCP();
                    }
                }
            },10);
        }
    }
});

function destroyCurrentCP(){
    if(currentCP.blip != null && currentCP.blip.doesExist())
        currentCP.blip.destroy();
    currentCP.blip = null;
    if(currentCP.cp != null)
        currentCP.cp.destroy();
    currentCP.cp = null;
    if(currentCP.label != null)
        currentCP.label.destroy();
    currentCP.label = null;
}

mp.events.add("playerExitCheckpoint", (cp) => {
    if(currentCP.cp != null && cp == currentCP.cp && currentCP.data.type == "stop" && !currentCP.left && stoppedInterval != null){
        currentCP.left = true;
    }
});

// noCollisionInterval = setInterval(function(){
//     if(player.vehicle && player.vehicle.hasVariable("collision") && player.vehicle.getVariable("collision") == false){
//         mp.players.forEachInStreamRange(pl => {
//             if(pl.vehicle){
//                 player.vehicle.setNoCollision(pl.vehicle.handle, false);
//                 pl.vehicle.setNoCollision(player.vehicle.handle, false);
//             }
//         });
//     }
//     else if(player.vehicle){
//         mp.players.forEachInStreamRange(pl => {
//             if(pl.vehicle && pl.vehicle.hasVariable("collision") && pl.vehicle.getVariable("collision") == false){
//                 player.vehicle.setNoCollision(pl.vehicle.handle, false);
//                 pl.vehicle.setNoCollision(player.vehicle.handle, false);
//             }
//         })
//     }
//     mp.players.forEachInStreamRange(pl => {
//         if(pl.vehicle && pl.vehicle.hasVariable("collision") && pl.vehicle.getVariable("collision") == false){
//             mp.players.forEachInStreamRange(pl1 => {
//                 if(pl1.vehicle){
//                     pl1.vehicle.setNoCollision(pl.vehicle.handle, false);
//                     pl.vehicle.setNoCollision(pl1.vehicle.handle, false);
//                 }
//             });
//         }
//     });
// }, 300)

function setNewCheckpoint(){
    let index = checkpoints.indexOf(currentCP.data) + 1;
    let checkpoint = checkpoints[index]
    let next;
    if(index < checkpoints.length - 2)
        next = checkpoints[index + 1].position;
    else
        next = checkpoints[index].position;
    if(currentCP.blip != null && currentCP.blip.doesExist())
        currentCP.blip.destroy();
    currentCP.blip = null;
    if(currentCP.cp != null)
        currentCP.cp.destroy();
    currentCP.cp = null;
    if(currentCP.label != null)
        currentCP.label.destroy();
    currentCP.label = null;
    if(checkpoints.indexOf(currentCP.data) == checkpoints.length -1){
        mp.events.callRemote("licenceBpassed");
        if(currentCP.blip != null && currentCP.blip.doesExist())
            currentCP.blip.destroy();
        currentCP.blip = null;
        if(currentCP.cp != null)
            currentCP.cp.destroy();
        currentCP.cp = null;
        if(currentCP.label != null)
            currentCP.label.destroy();
        currentCP.label = null;
    }
    else{
        currentCP.data = checkpoint;
        insideCP = false;
        currentCP.cp = mp.checkpoints.new(1, new mp.Vector3(checkpoint.position.x, checkpoint.position.y, checkpoint.position.z - 1), 2,{
            direction: new mp.Vector3(next.x, next.y, next.z),
            color: [0, 204, 153, 100],
            visible: true,
            dimension: 0
        });
        currentCP.left = false;
        currentCP.blip = mp.blips.new(8, checkpoint.position,{
            name: 'Kierunek egzaminu',
            color: 15,
            shortRange: false,
        });
        currentCP.blip.setRoute(true);
        if(currentCP.data.type == "stop"){
            let cpPos = new mp.Vector3(currentCP.data.position.x, currentCP.data.position.y, currentCP.data.position.z + 0.6);
            currentCP.label = mp.labels.new("STOP", cpPos,
            {
                los: true,
                font: 2,
                drawDistance: 30,
                color: [255,0,0,255]
            });
        }
    }
}

// mp.events.addDataHandler("collision", (entity, value, oldvalue) => {
//     if(value == false){
//         mp.players.forEach(pl => {
//             if(pl.vehicle){
//                 entity.setNoCollision(pl.vehicle.handle, false);
//             }
//         });
//     }
// });

// mp.events.add("entityStreamIn", (entity) => {
//     if(entity.type === "vehicle" && entity.hasVariable("collision") && entity.getVariable("collision") == false){
//         mp.players.forEach(pl => {
//             if(pl.vehicle){
//                 entity.setNoCollision(pl.vehicle.handle, false);
//             }
//         });
//     } 
// });



let checkpoints = [
    {position: new mp.Vector3(1014.363, -2324.1343, 30.00485), type: "start", text: ""},
    {position: new mp.Vector3(1026.534, -2333.1787, 29.998297), type: "stop", text: "Wykonaj manewr na łuku i zatrzymaj się na końcu."},
    {position: new mp.Vector3(1014.363, -2324.1343, 30.00485), type: "stop", text: "Powtórz manewr tyłem i zatrzymaj się."},
    {position: new mp.Vector3(1004.82416, -2335.32, 30.986727), type: "stop", text: "Teraz wjedź przodem na podjazd i zatrzymaj się!"},
    {position: new mp.Vector3(1004.1285, -2343.3477, 30.961025), type: "", text: "Wjedź na szczyt podjazdu i zjedź z niego"},
    {position: new mp.Vector3(1031.4686, -2368.232, 30.347565), type: "stop", text: "Udaj się do wyjazdu z placu i zatrzymaj się przy skrzyżowaniu"},
    {position: new mp.Vector3(1055.0232, -2233.4739, 30.260115), type: "", text: "Skręć w lewo i kontynuuj prosto, pamiętaj że obowiązuje Cię ograniczenie 70km/h"},
    {position: new mp.Vector3(1066.7366, -2093.2607, 33.050858), type: "stop", text: "Zatrzymaj się na najbliższym skrzyżowaniu a następnie skręć w lewo!"},
    {position: new mp.Vector3(1014.3583, -2080.6511, 30.946733), type: "", text: "Na najbliższym skrzyżowaniu jedź prosto"},
    {position: new mp.Vector3(949.55756, -2080.2773, 30.519146), type: "", text: "Przygotuj się do skrętu w prawo"},
    {position: new mp.Vector3(805.9116, -2056.065, 29.089243), type: "", text: "Skręć w prawo"},
    {position: new mp.Vector3(806.2396, -1954.4921, 29.058971), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(833.91284, -1774.4409, 28.99093), type: "stop", text: "Na najbliższym skrzyżowaniu zatrzymaj się i skręć w lewo"},
    {position: new mp.Vector3(602.50476, -1722.6141, 29.24203), type: "", text: "Kontynuuj prosto"},
    {position: new mp.Vector3(548.71844, -1697.1913, 29.233665), type: "", text: "Na najbliższym skrzyżowaniu jedź prosto"},
    {position: new mp.Vector3(488.57892, -1647.7186, 29.22014), type: "", text: "Przygotuj się do skrętu w prawo"},
    {position: new mp.Vector3(411.31955, -1575.6007, 29.076706), type: "", text: "Skręć w prawo na parking"},
    {position: new mp.Vector3(425.45483, -1548.1378, 28.849543), type: "", text: "Przygotuj się do parkowania prostopadłego!"},
    {position: new mp.Vector3(444.8142, -1521.3356, 29.096), type: "stop", text: "Zaparkuj na wyznaczonym miejscu i zatrzymaj się!"},
    {position: new mp.Vector3(430.10635, -1529.4624, 29.114225), type: "", text: "Wyjedź z parkingu i skręć w prawo."},
    {position: new mp.Vector3(395.2019, -1487.3142, 29.06864), type: "stop", text: "Zatrzymaj się i włącz się do ruchu skręcając w prawo"},
    {position: new mp.Vector3(446.96805, -1446.5476, 29.165136), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(519.25916, -1438.3811, 29.168198), type: "", text: "Na najbliższym skrzyżowaniu jedź prosto"},
    {position: new mp.Vector3(701.2478, -1445.0518, 31.08401), type: "", text: "Kontynuuj prosto"},
    {position: new mp.Vector3(773.23334, -1444.5269, 27.005325), type: "", text: "Na skrzyżowaniu jedź prosto"},
    {position: new mp.Vector3(1047.9954, -1440.9713, 36.5626), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(1237.1825, -1436.4631, 35.149273), type: "stop", text: "Zatrzymaj się i skręć w lewo"},
    {position: new mp.Vector3(1242.9576, -1343.5092, 35.0675), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(1181.9503, -1066.2379, 41.546944), type: "", text: "Przygotuj się do skrętu w lewo"},
    {position: new mp.Vector3(1156.355, -967.1319, 46.906525), type: "stop", text: "Zatrzymaj się i skręć w lewo"},
    {position: new mp.Vector3(1060.7195, -969.7594, 44.367695), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(1004.7274, -986.8402, 41.939953), type: "stop", text: "Zatrzymaj się na skrzyżowaniu i kontynuuj prosto"},
    {position: new mp.Vector3(890.8433, -998.37164, 32.88364), type: "", text: "Kontynuuj prosto"},
    {position: new mp.Vector3(804.24945, -1002.40356, 25.980158), type: "", text: "Na skrzyżowaniu jedź prosto"},
    {position: new mp.Vector3(481.3566, -1032.1786, 34.536774), type: "", text: "Trzymaj się prawej i jedź prosto"},
    {position: new mp.Vector3(413.9664, -1039.057, 29.195341), type: "", text: "Na najbliższym skrzyżowaniu kontynuuj prosto"},
    {position: new mp.Vector3(251.94475, -1037.4973, 29.085491), type: "", text: "Skręć w prawo"},
    {position: new mp.Vector3(247.34773, -986.5315, 29.147081), type: "", text: "Jedź prosto i trzymaj się lewej"},
    {position: new mp.Vector3(280.40668, -878.2913, 29.082607), type: "stop", text: "Zatrzymaj się i skręć w lewo"},
    {position: new mp.Vector3(267.7711, -846.96564, 29.158606), type: "", text: "Jedź prosto i zjedź na lewy pas"},
    {position: new mp.Vector3(199.67165, -827.4166, 30.791136), type: "stop", text: "Zatrzymaj się i skręć w lewo"},
    {position: new mp.Vector3(145.29625, -877.2914, 30.42501), type: "", text: "Kontynuuj prosto"},
    {position: new mp.Vector3(110.1849, -972.5531, 29.211105), type: "", text: "Na skrzyżowaniu jedź prosto"},
    {position: new mp.Vector3(74.64643, -1061.6769, 29.248278), type: "", text: "Kontynuuj prosto"},
    {position: new mp.Vector3(58.035828, -1111.7573, 29.215107), type: "", text: "Jedź przed siebie"},
    {position: new mp.Vector3(56.966255, -1220.4982, 29.15677), type: "", text: "Na skrzyżowaniu jedź prosto"},
    {position: new mp.Vector3(53.236195, -1277.7449, 29.154324), type: "", text: "Na skryżowaniu jedź prosto"},
    {position: new mp.Vector3(83.193726, -1338.4844, 29.16185), type: "", text: "Kontynuuj prosto"},
    {position: new mp.Vector3(129.52744, -1386.9916, 29.118706), type: "", text: "Na skrzyżowaniu kontynuuj prosto"},
    {position: new mp.Vector3(192.15794, -1422.8878, 29.149508), type: "", text: "Nadal prosto, pamiętaj o ograniczeniu prędkości!"},
    {position: new mp.Vector3(218.65112, -1438.5317, 29.148691), type: "", text: "Kontynuuj jazdę prosto"},
    {position: new mp.Vector3(295.9304, -1500.5322, 29.05931), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(376.48056, -1568.2396, 29.14874), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(438.3183, -1620.4583, 29.163094), type: "", text: "Kontynuuj prosto"},
    {position: new mp.Vector3(511.11444, -1683.8242, 29.165348), type: "", text: "Na skrzyżowaniu jedź prosto"},
    {position: new mp.Vector3(591.1943, -1730.3943, 29.260872), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(709.74243, -1745.2637, 29.194477), type: "", text: "Kontynuuj prosto"},
    {position: new mp.Vector3(797.1142, -1753.0896, 29.11476), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(907.89874, -1767.4414, 30.42578), type: "", text: "Trzymaj się prawej"},
    {position: new mp.Vector3(945.7229, -1772.5074, 30.948284), type: "", text: "Skręć w prawo"},
    {position: new mp.Vector3(950.7193, -1821.4482, 31.028172), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(940.56903, -1960.3373, 30.294956), type: "", text: "Jedź prosto i przygotuj się do skrętu w lewo"},
    {position: new mp.Vector3(932.23816, -2061.201, 30.345093), type: "stop", text: "Zatrzymaj się i skręć w lewo"},
    {position: new mp.Vector3(956.162, -2085.9639, 30.573442), type: "", text: "Przygotuj się do skrętu w prawo"},
    {position: new mp.Vector3(1016.1757, -2090.2593, 30.87811), type: "", text: "Na skrzyżowaniu skręć w prawo"},
    {position: new mp.Vector3(1058.9191, -2113.658, 32.4235), type: "", text: "Jedź prosto"},
    {position: new mp.Vector3(1053.2504, -2179.9797, 31.344625), type: "", text: "Kontynuuj prosto"},
    {position: new mp.Vector3(1039.0724, -2345.1309, 30.31256), type: "", text: "Przygotuj się do wjazdu na plac"},
    {position: new mp.Vector3(1029.563, -2367.0964, 30.350769), type: "", text: "Wjedź na plac i powoli jedź prosto"},
    {position: new mp.Vector3(1003.32947, -2366.037, 30.33427), type: "stop", text: "Zatrzymaj się przy budynku"}
]