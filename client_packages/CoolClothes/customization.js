const player = mp.players.local;
let clothesCEF = null,
    customizatorOpened = false,
    bodyCam = null,
    bodyCamStart = null;
    rotateTimer = null;

const genders = [
    "mp_m_freemode_01",
    "mp_f_freemode_01"
];

getCameraOffset = (pos, angle, dist) => {
    angle = angle * 0.0174533;
    pos.y = pos.y + dist * Math.sin(angle);
    pos.x = pos.x + dist * Math.cos(angle);
    return pos;
}

mp.events.add({

    'client:clothes.show': () => {
        player.freezePosition(true);
        if (customizatorOpened == true) return;
        clothesCEF = mp.browsers.new('package://CoolClothes/creator-interface/creator.html');
        let gender = player.model == mp.game.joaat(genders[0]) ? 0 : 1;
        setTimeout(function(){
            clothesCEF.execute(`setGender(${gender})`);
        }, 500);
        setTimeout(() => { mp.gui.cursor.show(true, true); }, 500);
        mp.game.ui.displayRadar(false);
        mp.events.call('client:clothes.cam', true);
        customizatorOpened = true;
    },

    'client:clothes.cam': (toggle) => {
        if (toggle) {
            bodyCamStart = player.position;
            let camValues = { Angle: player.getRotation(2).z + 90, Dist: 2.6, Height: 0.2 };
            let pos = getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), camValues.Angle, camValues.Dist);
            bodyCam = mp.cameras.new('default', pos, new mp.Vector3(0, 0, 0), 50);
            bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
            bodyCam.setActive(true);
            mp.game.cam.renderScriptCams(true, false, 500, true, false);
        } else {
            if (bodyCam == null) return;
            bodyCam.setActive(false);
            bodyCam.destroy();
            mp.game.cam.renderScriptCams(false, false, 3000, true, true);

            bodyCam = null;
        }
        player.taskPlayAnim("amb@world_human_guard_patrol@male@base", "base", 8.0, 1, -1, 1, 0.0, false, false, false);
    },

    'client:clothes.cam.set': (flag) => {
        let camValues = { Angle: 0, Dist: 1, Height: 0.2 };
        switch (flag) {
            case 0: // Torso
                {
                    camValues = { Angle: 0, Dist: 2.6, Height: 0.2 };
                    break;
                }
            case 1: // Head
                {
                    camValues = { Angle: 0, Dist: 1, Height: 0.5 };
                    break;
                }
            case 2: // Hair / Bear / Eyebrows
                {
                    camValues = { Angle: 0, Dist: 0.5, Height: 0.7 };
                    break;
                }
            case 3: // pants
                {
                    camValues = { Angle: 0, Dist: 2.6, Height: -0.3 };
                    break;
                }
            case 4: // shoes
                {
                    camValues = { Angle: 0, Dist: 2.6, Height: -0.5 };
                    break;
                }
        }
        const camPos = getCameraOffset(new mp.Vector3(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height), player.getRotation(2).z + 90 + camValues.Angle, camValues.Dist);
        bodyCam.setCoord(camPos.x, camPos.y, camPos.z);
        bodyCam.pointAtCoord(bodyCamStart.x, bodyCamStart.y, bodyCamStart.z + camValues.Height);
    },

    'client:clothes.reload': () => {
        let shirt = player.getNumberOfDrawableVariations(11),
            bottom = player.getNumberOfDrawableVariations(4),
            shoes = player.getNumberOfDrawableVariations(6);
        clothesCEF.execute(`customization.max([${shirt}, ${bottom}, ${shoes}]);`);
    },

    'client:clothes.finish': () => {
        mp.events.call('client:clothes.cam', false);
        if (mp.browsers.exists(clothesCEF)) { clothesCEF.destroy() }
        customizatorOpened = false;
        mp.game.ui.displayRadar(true);
        setTimeout(() => { mp.gui.cursor.show(false, false); }, 500);
        player.freezePosition(false);
        mp.events.call('client:hud.show', true);

        mp.events.callRemote("saveClothes");
    },

    'client:clothes.cancel': () => {
        mp.events.call('client:clothes.cam', false);
        if (mp.browsers.exists(clothesCEF)) { clothesCEF.destroy() }
        customizatorOpened = false;
        mp.game.ui.displayRadar(true);
        setTimeout(() => { mp.gui.cursor.show(false, false); }, 500);
        player.freezePosition(false);
        mp.events.call('client:hud.show', true);

        mp.events.callRemote("loadClothes");
        mp.events.callRemote("setPlayersDimension", 0);
        mp.events.callRemote("setGui", false);
    },

    'client:clothes.preview': (x, data) => {
        data = JSON.parse(data);
        switch (x) {
            case 'top':
                mp.events.call('client:clothes.cam.set', 0);
                mp.events.callRemote("previewClothes", 11, parseInt(data[0]), parseInt(data[1]));
                break;
            case 'undershirt':
                mp.events.call('client:clothes.cam.set', 0);
                mp.events.callRemote("previewClothes", 8, parseInt(data[0]), parseInt(data[1]));
                break;
            case 'torso':
                mp.events.call('client:clothes.cam.set', 0);
                mp.events.callRemote("previewClothes", 3, parseInt(data[0]), parseInt(data[1]));
                break;
            case 'pants':
                mp.events.call('client:clothes.cam.set', 3);
                mp.events.callRemote("previewClothes", 4, parseInt(data[0]), parseInt(data[1]));

                break;
            case 'shoes':
                mp.events.call('client:clothes.cam.set', 4);
                mp.events.callRemote("previewClothes", 6, parseInt(data[0]), parseInt(data[1]));
                break;
        }
    },

    'client:clothes.rotate': () => {
        if(rotateTimer == null){
            player.setRotation(0, 0, player.getRotation(5).z + 180, 5, true)
            player.taskPlayAnim("amb@world_human_guard_patrol@male@base", "base", 8.0, 1, -1, 1, 0.0, false, false, false);
            rotateTimer = setTimeout(function(){
                player.setRotation(0, 0, player.getRotation(5).z - 180, 5, true)
                rotateTimer = null;
                player.taskPlayAnim("amb@world_human_guard_patrol@male@base", "base", 8.0, 1, -1, 1, 0.0, false, false, false);
            }, 5000);
        }
        
    },
    'refreshClothes': () => {
        setTimeout(function(){
            mp.events.callRemote("loadClothes");
        }, 500);
    }
});